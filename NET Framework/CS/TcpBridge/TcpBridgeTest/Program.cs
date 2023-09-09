using Koz.Fx.Net.Sockets;
using System;
using System.Net;

namespace TcpBridgeTest
{
    class Program
    {
        private const int listenPort = 13389;
        private const string oracleIPAdress = "127.0.0.1";
        private const int oraclePort = 1521;

        static void Main(string[] args) {
            TcpBridge breidge = new TcpBridge(IPAddress.Any, listenPort,
                                        IPAddress.Parse(oracleIPAdress), oraclePort);
            Console.WriteLine("breidge.Start()");
            breidge.Start();
            Console.WriteLine("何かキーを押すまで接続を受け付けます。");
            Console.ReadKey();
            Console.WriteLine("breidge.Stop(true)");
            // 強制切断
            breidge.Stop(true);

            Console.WriteLine("breidge.Start()");
            breidge.Start();
            Console.WriteLine("何かキーを押すまで接続を受け付けます。");
            Console.ReadKey();
            Console.WriteLine("breidge.Stop(false)");
            // 全てのクライアントが切断するまで待機
            breidge.Stop(false);

            Console.WriteLine("Enter キーを押すと終了します。");
            Console.ReadLine();
        }
    }
}
