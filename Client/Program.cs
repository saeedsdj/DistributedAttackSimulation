using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

namespace Client
{
    class Program
    {
        private static Thread threadConsole;
        private static Thread threadAttackState;
        private static bool attacking = false;
        private static Client client = new Client();

        static void Main(string[] args)
        {
            Console.Title = "Distributed Attacker Client";

            Console.WriteLine("Enter server IP address:");
            String serverIP = Console.ReadLine();

            client.OnConnect += new Client.OnConnectEventHandler(client_OnConnect);
            client.OnReceived += new Client.OnReceivedEventHandler(client_Received);
            client.OnDisconnected += new Client.OnDisconnectedEventHandler(client_Disconnected);

            client.Connect(serverIP, 8888);

            threadConsole = new Thread(new ThreadStart(ConsoleThread));
            threadConsole.Start();
        }

        static void client_OnConnect(Client sender, bool connected)
        {
            Console.WriteLine("Successfully Connected to Server");
            threadAttackState = new Thread(new ThreadStart(AttackThread));
            threadAttackState.Start();
        }

        static void client_Received(Client sender, byte[] data)
        {
            String msg = Encoding.Default.GetString(data);
            attacking = true;
            Console.WriteLine("Server Commands:  {0}", msg);
        }

        static void client_Disconnected(Client sender)
        {
            Console.WriteLine("Disconnected From Server");
        }

        private static void ConsoleThread()
        {
            while (true)
            {
                Thread.Sleep(100);
            }
        }

        private static void AttackThread()
        {
            while (true)
            {
                if (!client.Connected || attacking)
                {
                    break;
                }

                Random rnd = new Random();
                int attackState = rnd.Next(0, 2);
                if (attackState == 1)
                {
                    client.SendStatus("ready");
                }
                else
                {
                    client.SendStatus("not ready");
                }

                Thread.Sleep(10000);
            }
        }
    }
}
