using System;
using System.Numerics;
using System.Diagnostics;

class EnumPrimes
{
    static bool Listing = true;

    // 試し割り法(1)
    static void TrialDivision1(int limit) {
        bool isPrime(int number) {
            if (number <= 1)
                return false;

            for (int i = 2; i * i <= number; i++) {
                if (number % i == 0)
                    return false;
            }
            return true;
        }
        var sw = new Stopwatch();
        sw.Start();
        var primes = new List<int>(limit + 1) { 2 };
        for (int i = 3; i <= limit; i+= 2) {
            if (isPrime(i)) {
                primes.Add(i);
            }
        }
        sw.Stop();
        Console.WriteLine($"TrialDivision1:{sw.ElapsedMilliseconds}ms");
        if (Listing) {
            foreach (var p in primes) {
                Console.Write(p + " ");
            }
            Console.WriteLine();
        }
    }

    // 試し割り法(2)
    static void TrialDivision2(int limit) {
        var sw = new Stopwatch();
        sw.Start();
        var primes = new List<int>(limit + 1) { 2 };
        for (int n = 3; n <= limit; n += 2) {
            bool isPrme = true;
            for (int i = 0; i < primes.Count; i++) {
                var p = primes[i];
                if (p * p > n) {
                    break;
                }
                if (n % p == 0) {
                    isPrme = false;
                    break;
                }
            }
            if (isPrme) {
                primes.Add(n);
            }
        }
        sw.Stop();
        Console.WriteLine($"TrialDivision2:{sw.ElapsedMilliseconds}ms");
        if (Listing) {
            foreach (var p in primes) {
                Console.Write(p + " ");
            }
            Console.WriteLine();
        }
    }

    // エラトステネスの篩
    static void SieveOfEratosthenes(int limit) {
        var sw = new Stopwatch();
        sw.Start();

        bool[] isPrime = new bool[limit + 1];
        for (int i = 2; i <= limit; i++) {
            isPrime[i] = true;
        }

        for (int p = 2; p * p <= limit; p++) {
            if (isPrime[p]) {
                for (int i = p * p; i <= limit; i += p) {
                    isPrime[i] = false;
                }
            }
        }

        sw.Stop();
        Console.WriteLine($"SieveOfEratosthenes:{sw.ElapsedMilliseconds}ms");
        if (Listing) {
            for (int i = 2; i <= limit; i++) {
                if (isPrime[i])
                    Console.Write(i + " ");
            }
            Console.WriteLine();
        }
    }

    // ミラーラビン素数判定法
    static BigInteger ModExp(BigInteger a, BigInteger b, BigInteger n) {
        if (b == 0)
            return 1;

        BigInteger t = ModExp(a, b / 2, n);
        BigInteger result = (t * t) % n;

        if (b % 2 == 1)
            result = (result * a) % n;

        return result;
    }

    static bool MillerRabinTest(BigInteger num, int iterations) {
        if (num <= 1)
            return false;

        if (num <= 3)
            return true;

        if (num % 2 == 0)
            return false;

        BigInteger d = num - 1;
        int r = 0;
        while (d % 2 == 0) {
            d /= 2;
            r++;
        }

        for (int i = 0; i < iterations; i++) {
            BigInteger a = RandomBigInt(2, num - 2); // 2からnum-2までのランダムな整数
            BigInteger x = ModExp(a, d, num);

            if (x == 1 || x == num - 1)
                continue;

            for (int j = 1; j < r; j++) {
                x = ModExp(x, 2, num);
                if (x == num - 1)
                    break;
            }

            if (x != num - 1)
                return false;
        }

        return true;
    }

    static Random randomB = new Random();
    static BigInteger RandomBigInt(BigInteger min, BigInteger max) {
        byte[] bytes = new byte[max.ToByteArray().LongLength];
        randomB.NextBytes(bytes);
        BigInteger result = new BigInteger(bytes);
        return BigInteger.Abs(result) % (max - min) + min;
    }

    static void MillerRabin(int limit) {
        var sw = new Stopwatch();
        sw.Start();
        var primes = new List<int>(limit + 1) { 2 };
        for (int i = 3; i <= limit; i += 2) {
            if (MillerRabinTest(i, 10)) {
                primes.Add(i);
            }
        }
        sw.Stop();
        Console.WriteLine($"MillerRabin:{sw.ElapsedMilliseconds}ms");
        if (Listing) {
            foreach (var p in primes) {
                Console.Write(p + " ");
            }
            Console.WriteLine();
        }
    }

    // フェルマーテスト
    static Random random = new Random();

    static int ModExpF(int a, int b, int n) {
        int result = 1;
        a = a % n;

        while (b > 0) {
            if (b % 2 == 1)
                result = (result * a) % n;

            a = (a * a) % n;
            b /= 2;
        }

        return result;
    }

    static bool FermatTest(int num, int iterations) {
        if (num <= 1)
            return false;

        if (num <= 3)
            return true;

        for (int i = 0; i < iterations; i++) {
            int a = random.Next(2, num - 1);
            if (ModExpF(a, num - 1, num) != 1)
                return false;
        }

        return true;
    }

    static void FermatTest(int limit) {
        var sw = new Stopwatch();
        sw.Start();
        var primes = new List<int>(limit + 1) { 2 };
        for (int i = 3; i <= limit; i += 2) {
            if (FermatTest(i, 10)) {
                primes.Add(i);
            }
        }
        sw.Stop();
        Console.WriteLine($"FermatTest:{sw.ElapsedMilliseconds}ms");
        if (Listing) {
            foreach (var p in primes) {
                Console.Write(p + " ");
            }
            Console.WriteLine();
        }
    }

    static void Main(string[] args) {
        int limit = 1000;
        Listing = false;
        TrialDivision1(limit);
        TrialDivision2(limit);
        SieveOfEratosthenes(limit);
        MillerRabin(limit);
        FermatTest(limit);
    }
}
