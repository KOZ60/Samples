using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Koz.Fx.Net.Sockets
{
    public class TcpBridge
    {
        private readonly AsyncCallback OnAcceptTcpClientCallback;
        private readonly IPAddress serverAddress;
        private readonly int serverPort;
        private readonly ConnectionCollection connections = new ConnectionCollection();
        private readonly TcpListener listener;

        public bool IsStoped { get; private set; } = true;

        public TcpBridge(IPAddress listenAddress, int listenPort,
                         IPAddress serverAddress, int serverPort) {
            this.serverAddress = serverAddress;
            this.serverPort = serverPort;
            OnAcceptTcpClientCallback = new AsyncCallback(OnAcceptTcpClient);
            listener = new TcpListener(listenAddress, listenPort);
        }

        public void Start() {
            if (!IsStoped) {
                throw new InvalidOperationException("既に開始しています。");
            }
            IsStoped = false;
            try {
                listener.Start();
                listener.BeginAcceptTcpClient(OnAcceptTcpClientCallback, null);
            } catch {
                IsStoped = true;
                throw;
            }
        }

        public void Stop() {
            Stop(false);
        }

        public void Stop(bool force) {
            if (IsStoped) {
                throw new InvalidOperationException("既に終了しています。");
            }
            IsStoped = true;
            connections.WaitAll(force);
            listener.Stop();
        }

        private void OnAcceptTcpClient(IAsyncResult ar) {
            TcpClient client = EndAcceptTcpClient(ar);
            if (client == null) {
                return;
            }
            listener.BeginAcceptTcpClient(OnAcceptTcpClientCallback, null);

            try {
                TcpClient server = new TcpClient();
                server.Connect(serverAddress, serverPort);
                Console.WriteLine("Connect To {0}", server.Client.RemoteEndPoint);
                Connection connection = new Connection(client, server);
                connections.Add(connection);

            } catch (SocketException) {
                client.Close();
            }
        }

        private TcpClient EndAcceptTcpClient(IAsyncResult ar) {
            TcpClient client = null;
            try {
                client = listener.EndAcceptTcpClient(ar);
                Console.WriteLine("Connect From {0}", client.Client.RemoteEndPoint);
                if (IsStoped) {
                    Console.WriteLine("Close {0}", client.Client.RemoteEndPoint);
                    client.Close();
                    client = null;
                }
            } catch (IOException) {
            } catch (ObjectDisposedException) {
            }
            return client;
        }
    }
}
