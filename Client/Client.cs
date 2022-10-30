using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    class Client
    {
        public delegate void OnConnectEventHandler(Client sender, bool connected);
        public event OnConnectEventHandler OnConnect;

        public delegate void OnReceivedEventHandler(Client sender, byte[] data);
        public event OnReceivedEventHandler OnReceived;

        public delegate void OnDisconnectedEventHandler(Client sender);
        public event OnDisconnectedEventHandler OnDisconnected;

        Socket socket;

        public bool Connected
        {
            get
            {
                if (socket != null)
                {
                    return socket.Connected;
                }

                return false;
            }
        }

        public bool Attacking { get; set; }

        public Client()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Connect(string ip, int port)
        {
            if (socket != null)
            {
                socket.BeginConnect(ip, port, connectCallback, null);
            }
        }

        public void connectCallback(IAsyncResult ar)
        {
            try
            {
                socket.EndConnect(ar);

                if (OnConnect != null)
                {
                    OnConnect(this, Connected);
                }

                socket.BeginReceive(new byte[]{0}, 0, 0, 0, receiveCallback, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        void receiveCallback(IAsyncResult ar)
        {
            try
            {
                socket.EndReceive(ar);

                byte[] buffer = new byte[8192];

                int rec = socket.Receive(buffer, buffer.Length, 0);

                if (rec <= 0)
                {
                    throw new Exception($"Error receiving 0 byte data from Server");
                }

                if (rec < buffer.Length)
                {
                    Array.Resize<byte>(ref buffer, rec);
                }

                if (OnReceived != null)
                {
                    OnReceived(this, buffer);
                }

                socket.BeginReceive(new byte[]{0}, 0, 0, 0, receiveCallback, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                if (OnDisconnected != null)
                {
                    OnDisconnected(this);
                }
            }
        }

        public void SendStatus(string msg)
        {
            if (Connected)
            {
                int s = socket.Send(Encoding.Default.GetBytes(msg));

                if (s > 0)
                {
                    if (msg == "ready")
                    {
                        Console.WriteLine("I am Ready");
                    }
                    else
                    {
                        Console.WriteLine("I am NOT Ready");
                    }
                }
            }
        }

        public void Disconnect()
        {
            try
            {
                if (socket.Connected)
                {
                    socket.Close();
                    socket = null;

                    if (OnDisconnected != null)
                    {
                        OnDisconnected(this);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}