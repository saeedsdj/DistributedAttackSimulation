using System;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    class Client
    {
        public String ID { get; set; }
        public IPEndPoint EndPoint { get; set; }
        public bool ReadyToAttack { get; set; }

        Socket socket;

        public Client(Socket accepted)
        {
            socket = accepted;
            ID = Guid.NewGuid().ToString();
            ReadyToAttack = false;
            EndPoint = (IPEndPoint)socket.RemoteEndPoint;
            socket.BeginReceive(new byte[]{0}, 0, 0, 0, callback, null);
        }

        void callback(IAsyncResult ar)
        {
            try
            {
                socket.EndReceive(ar);

                byte[] buffer = new byte[8192];

                int rec = socket.Receive(buffer, buffer.Length, 0);

                if (rec <= 0)
                {
                    throw new Exception($"Error receiving 0 byte data from {EndPoint} with player Id: {ID}");
                }

                if (rec < buffer.Length)
                {
                    Array.Resize<byte>(ref buffer, rec);
                }

                if (Received != null)
                {
                    Received(this, buffer);
                }

                socket.BeginReceive(new byte[]{0}, 0, 0, 0, callback, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Close();

                if (Disconnected != null)
                {
                    Disconnected(this);
                }
            }
        }

        public void Close()
        {
            socket.Close();
            socket.Dispose();
        }

        public delegate void ClientReceivedHandler(Client sender, byte[] data);
        public delegate void ClientDisconnectedHandler(Client sender);

        public event ClientReceivedHandler Received;
        public event ClientDisconnectedHandler Disconnected;
    }
}