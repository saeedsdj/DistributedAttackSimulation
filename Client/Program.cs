using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Client
{
    class Program
    {
        private static Thread threadConsole;

        static void Main(string[] args)
        {
            Console.Title = "Distributed Attacker Client";

            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            s.Connect("127.0.0.1", 8888);

            Thread.Sleep(5000);
            s.Close();
            s.Dispose();

            // threadConsole = new Thread(new ThreadStart(ConsoleThread));
            // threadConsole.Start();
        }

        private static void ConsoleThread()
        {
            while (true)
            {
                Thread.Sleep(5000);
            }
        }
    }
}
