using System;
using System.IO;
using System.ComponentModel;
using System.Net.Sockets;

namespace Koz.Fx.Net.Sockets
{
    class Forwarder
    {
        private readonly NetworkStream reader;
        private readonly NetworkStream writer;
        private readonly Socket writerSocket;
        private readonly byte[] readBuffer = new byte[2048];
        private readonly byte[] writeBuffer = new byte[2048];
        private readonly AsyncCallback OnReadCallback;

        public event EventHandler EndOfStream;
        public bool AtEndOfStream { get; private set; } = false;

        public Forwarder(TcpClient readClient, TcpClient writeClient) {
            reader = readClient.GetStream();
            writer = writeClient.GetStream();
            writerSocket = writeClient.Client;
            OnReadCallback = new AsyncCallback(OnRead);
            reader.BeginRead(readBuffer, 0, readBuffer.Length, OnReadCallback, null);
        }

        private void OnRead(IAsyncResult ar) {
            int size = EndRead(ar);
            if (size > 0) {
                Array.Copy(readBuffer, writeBuffer, size);
                reader.BeginRead(readBuffer, 0, readBuffer.Length, OnReadCallback, null);
                try {
                    if (writer.CanWrite) {
                        writer.Write(writeBuffer, 0, size);
                    }
                } catch (IOException) {
                } catch (ObjectDisposedException) {
                }
            } else {
                OnEndOfStream(EventArgs.Empty);
            }
        }

        private int EndRead(IAsyncResult ar) {
            int size = 0;
            try {
                size = reader.EndRead(ar);
            } catch (IOException) {
            } catch (ObjectDisposedException) {
            }

            // size がゼロ以上なら正常
            if (size >= 0) {
                return size;
            } else {
                // ゼロ未満ならエラーが発生している
                Win32Exception exception = new Win32Exception();
                Console.WriteLine(exception.ToString());
                return 0;
            }
        }

        protected virtual void OnEndOfStream(EventArgs e) {
            if (writerSocket.Connected) {
                writerSocket.Shutdown(SocketShutdown.Send);
            }
            AtEndOfStream = true;
            EndOfStream?.Invoke(this, e);
        }
    }
}
