using System;
using System.Collections.Generic;
using System.Threading;

namespace Koz.Fx.Net.Sockets
{
    class ConnectionCollection
    {
        private readonly ManualResetEvent resetEvent = new ManualResetEvent(true);
        private readonly List<Connection> list = new List<Connection>();

        public void Add(Connection connection) {
            lock (this) {
                resetEvent.Reset();
                list.Add(connection);
            }
            connection.CloseEvent += Connection_CloseEvent;
        }

        public int Count {
            get {
                return list.Count;
            }
        }

        public Connection this[int index] {
            get {
                return list[index];
            }
        }

        private void Connection_CloseEvent(object sender, EventArgs e) {
            Connection connection = (Connection)sender;
            connection.CloseEvent -= Connection_CloseEvent;
            lock (this) {
                list.Remove(connection);
                if (list.Count == 0) {
                    resetEvent.Set();
                }
            }
        }

        public void WaitAll(bool force) {
            if (force) {
                lock (this) {
                    foreach (Connection con in list.ToArray()) {
                        con.Close();
                    }
                }
            }
            resetEvent.WaitOne();
        }
    }
}
