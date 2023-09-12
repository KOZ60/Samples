using System;
using System.Collections;
using System.Collections.Generic;

public class CircularList<TKey, TValue>
    : IEnumerable<KeyValuePair<TKey, TValue>>
{
    private readonly IComparer<TKey> comparer;
    private readonly List<TKey> keys;
    private readonly List<TValue> values;
    private int version;
    private int originIndex;
    private const int defaultCapacity = 4;

    public CircularList() : this(defaultCapacity, Comparer<TKey>.Default) { }
    public CircularList(int capacity) : this(capacity, Comparer<TKey>.Default) { }
    public CircularList(IComparer<TKey> comparer) : this(defaultCapacity, comparer) { }

    public CircularList(int capacity, IComparer<TKey> comparer) {
        this.comparer = comparer;
        keys = new List<TKey>(capacity);
        values = new List<TValue>(capacity);
        version = int.MaxValue;
        originIndex = 0;
    }

    public int OriginIndex {
        get {
            return originIndex;
        }
        set {
            if (value > Count || value < 0) {
                throw new ArgumentOutOfRangeException();
            }
            originIndex = value;
        }
    }

    public int Count => keys.Count;

    public TValue this[int index] {
        get {
            return values[index];
        }
        set {
            values[index] = value;
        }
    }

    public TValue[] GetValues(TKey key) {
        var r = GetRange(key);
        var array = new TValue[r.LastIndex - r.FirstIndex + 1];
        for (int i = r.FirstIndex; i <= r.LastIndex; i++) {
            array[i] = this[i];
        }
        return array;
    }

    public bool ContainsKey(TKey key) {
        return (GetIndex(key) >= 0);
    }

    public void Add(TKey key, TValue value) {
        AddCore(key, value);
        version++;
    }

    public void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> keyValues) {
        foreach (var pair in keyValues) {
            AddCore(pair.Key, pair.Value);
        }
        version++;
    }

    private void AddCore(TKey key, TValue value) {
        var index = GetIndex(key);
        if (index < 0) {
            index = ~index;
        } else {
            index = GetLastIndex(key, index) + 1;
        }
        keys.Insert(index, key);
        values.Insert(index, value);
    }

    public void RemoveAt(int index) {
        RemoveCore(index);
        version++;
    }

    public void Remove(TKey key) {
        RemoveCore(key);
        version++;
    }

    private TValue RemoveCore(int index) {
        var value = values[index];
        keys.RemoveAt(index);
        values.RemoveAt(index);
        return value;
    }

    private List<TValue> RemoveCore(TKey key) {
        var removeList = new List<TValue>();
        var r = GetRange(key);
        for (var i = r.LastIndex; i >= r.FirstIndex; i--) {
            removeList.Insert(0, RemoveCore(i));
        }
        return removeList;
    }

    public void ChangeKey(TKey oldKey, TKey newKey) {
        foreach (var value in RemoveCore(oldKey)) {
            AddCore(newKey, value);
        }
        version++;
    }

    private int GetIndex(TKey key) {
        return keys.BinarySearch(key, comparer);
    }

    private struct Range
    {
        public int FirstIndex;
        public int LastIndex;
    }

    private Range GetRange(TKey key) {
        var index = GetIndex(key);
        return new Range() {
            FirstIndex = GetFirstIndex(key, index),
            LastIndex = GetLastIndex(key, index)
        };
    }

    private int GetFirstIndex(TKey key, int index) {
        if (index < 0) return 0;
        while (index > 0 &&
                comparer.Compare(keys[index - 1], key) == 0) {
            index--;
        }
        return index;
    }

    private int GetLastIndex(TKey key, int index) {
        if (index < 0) return -1;
        while (index < keys.Count - 1 &&
                comparer.Compare(keys[index + 1], key) == 0) {
            index++;
        }
        return index;
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
        return new Enumerator(this);
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return new Enumerator(this);
    }

    private class Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
    {
        private readonly CircularList<TKey, TValue> owner;
        private readonly int version;
        private readonly int originIndex;
        private readonly int count;
        private int currentIndex;
        private int numberOfCall;

        public Enumerator(CircularList<TKey, TValue> owner) {
            this.owner = owner;
            version = owner.version;
            originIndex = owner.OriginIndex;
            count = owner.Count;
            Reset();
        }

        public void Reset() {
            currentIndex = originIndex - 1;
            numberOfCall = 0;
        }

        public KeyValuePair<TKey, TValue> Current {
            get {
                var key = owner.keys[currentIndex];
                var value = owner.values[currentIndex];
                return new KeyValuePair<TKey, TValue>(key, value);
            }
        }

        public void Dispose() { }

        public bool MoveNext() {
            if (owner.version != version) {
                throw new InvalidOperationException();
            }
            if (numberOfCall >= count) {
                return false;
            }
            numberOfCall++;
            currentIndex = (currentIndex + 1) % count;
            return true;
        }

        object IEnumerator.Current => Current;
    }
}
