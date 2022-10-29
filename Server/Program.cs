using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    class Program
    {
        private static Thread threadConsole;

        static Server server;
        static List<Socket> sockets;
        static void Main(string[] args)
        {
            Console.Title = "Distributed Computing Server";

            server = new Server(8888);
            server.SocketAccepted += new Server.SocketAcceptedHandler(server_SocketAccepted);
            sockets = new List<Socket>();
            server.Start();

            threadConsole = new Thread(new ThreadStart(ConsoleThread));
            threadConsole.Start();
        }

        static void server_SocketAccepted(Socket e)
        {
            Console.WriteLine("New Connection: {0}\n=============", e.RemoteEndPoint);
            sockets.Add(e);
        }

        private static void ConsoleThread()
        {
            while (true)
            {
                Thread.Sleep(100);
            }
        }
    }
}
