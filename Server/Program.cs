using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    class Program
    {
        static Listener listener;
        static List<Socket> sockets;
        static void Main(string[] args)
        {
            Console.WriteLine("Hello Server!");
            listener = new Listener(8888);
            listener.SocketAccepted += new Listener.SocketAcceptedHandler(listener_SocketAccepted);
            sockets = new List<Socket>();
            listener.Start();

            Console.Read();
        }

        static void listener_SocketAccepted(Socket e)
        {
            Console.WriteLine("New Connection: {0}\n=============", e.RemoteEndPoint);
            sockets.Add(e);
        }
    }
}
