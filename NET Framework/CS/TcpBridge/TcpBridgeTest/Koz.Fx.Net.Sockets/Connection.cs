using System;
using System.Net.Sockets;

namespace Koz.Fx.Net.Sockets
{
    class Connection : IDisposable
    {
        private readonly TcpClient client;
        private readonly TcpClient server;

        private readonly Forwarder clientToServer;
        private readonly Forwarder serverToClient;

        public event EventHandler CloseEvent;

        public Connection(TcpClient client, TcpClient server) {
            this.client = client;
            this.server = server;

            clientToServer = new Forwarder(client, server);
            serverToClient = new Forwarder(server, client);

            clientToServer.EndOfStream += EndOfStreamEvent;
            serverToClient.EndOfStream += EndOfStreamEvent;
        }

        private void EndOfStreamEvent(object sender, EventArgs e) {
            if (clientToServer.AtEndOfStream && serverToClient.AtEndOfStream) {
                Close();
            }
        }

        public void Close() {
            Dispose();
        }

        protected virtual void OnCloseEvent(EventArgs e) {
            CloseEvent?.Invoke(this, e);
        }

        private bool disposedValue;

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                disposedValue = true;
                if (disposing) {
                    Console.WriteLine("Close {0}", client.Client.RemoteEndPoint);
                    Console.WriteLine("Close {0}", server.Client.RemoteEndPoint);
                    client.Close();
                    server.Close();
                    OnCloseEvent(EventArgs.Empty);
                }
            }
        }

        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
