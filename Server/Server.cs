using System;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    class Server
    {
        Socket socket;

        public bool Listening { get; set; }

        public int Port { get; set; }

        public Server(int _port)
        {
            Port = _port;
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Start()
        {
            if (Listening)
            {
                return;
            }

            socket.Bind(new IPEndPoint(0, Port));
            socket.Listen(0);

            socket.BeginAccept(callback, null);
            Listening = true;
        }

        public void Stop()
        {
            if (!Listening)
            {
                return;
            }

            socket.Close();
            socket.Dispose();
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        void callback(IAsyncResult ar)
        {
            try
            {
                Socket s = socket.EndAccept(ar);

                if (SocketAccepted != null)
                {
                    SocketAccepted(s);
                }

                socket.BeginAccept(callback, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public delegate void SocketAcceptedHandler(Socket e);
        public event SocketAcceptedHandler SocketAccepted;
    }
}