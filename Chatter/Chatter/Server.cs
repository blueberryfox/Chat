﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Chatter
{
   public static class Server
    {
        public static List<Client> Clients = new List<Client>();
        public static void NewClient(Socket handle)
        {
            try
            {
                Client newClient = new Client(handle);
                Clients.Add(newClient);
                newClient.UpdateUser();
        
                Console.WriteLine("New client connected: "+ handle.RemoteEndPoint);
            }
            catch (Exception exp)
            {
                Console.WriteLine("Error with addNewClient: "+ exp.Message);
            }

           
        }
        public static void DisconnectClient(Client client)
        {
            try
            {
                client.Disconnect();
                Clients.Remove(client);
                Console.WriteLine("User " + client.UserName + " has been disconnected");
                client.UpdateUser();

            }
            catch (Exception exp)
            {
                Console.WriteLine("Error with endClient: "+ exp.Message);
            }
        }
        public static void UpdateAllChats()
        {
            try
            {
                int countUsers = Clients.Count;
                for (int i = 0; i < countUsers; i++)
                {
                    Clients[i].UpdateChat();

                }
            }
            catch (Exception exp) { Console.WriteLine("Error with updateAlLChats: "+ exp.Message); }
        }

    }
}
