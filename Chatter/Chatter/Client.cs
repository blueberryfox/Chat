﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chatter
{
   public class Client
    {
        private string userName;
        private Socket handler;
        private Thread userThread;
        
        public Client(Socket socket)
        {
            handler = socket;
            userThread = new Thread(listner);
            userThread.IsBackground = true;
            userThread.Start();
           

        }
        public string UserName
        {
            get { return userName; }
        }
        private void listner()
        {
            while (true)
            {
               
                    byte[] buffer = new byte[1024];
                    int bytesRec;
                    try
                    {
                        bytesRec = handler.Receive(buffer);
                    }
                    catch (SocketException)
                    {
                        Server.DisconnectClient(this);
                        break;
                    }
                    string[] data = Encoding.UTF8.GetString(buffer, 0, bytesRec).Split('\n');
                    for (int i = 0; i < data.Length - 1; i++)
                        handleCommand(data[i]);
               
            }
        }
        public void Disconnect()
        {
            try
            {
                handler.Close();
                try
                {
                    userThread.Interrupt();
              
                }
                catch (SocketException e)
                {
                }
            }
            catch (SocketException exp) { Console.WriteLine("Error with end: {0}.", exp.Message); }
        }
        private void handleCommand(string data)
        {
            if (data.Contains("#setname"))
            {
                userName = data.Split('&')[1];
                UpdateChat();
                return;
            }
            if (data.Contains("#newmsg"))
            {
                string message = data.Split('&')[1];
                ChatController.AddMessage(userName, message);
                return;
            }
            if (data.Contains("#updateuser"))
            {
                Server.UpdateAllChats();
            }
            if (data.Contains("#blacklist"))
            {
                Server.ToggleIgnore(this, data.Split('&')[1]);
            }
        }

        public void UpdateChat()
        {
            if (Server.BlackList.ContainsKey(UserName))
                Send(ChatController.GetChat(Server.BlackList[UserName]));
            else
                Send(ChatController.GetChat());
        }
        

        public void UpdateUser()
        {
               Send(ChatController.GetUser());
        }
        public void Send(string command)
        {
            try
            {
                Console.WriteLine(command);
                int bytesSent = handler.Send(Encoding.UTF8.GetBytes(command + '\n'));
                if (bytesSent > 0) Console.WriteLine("Success");
            }
            catch (ArgumentException exp) {
                Console.WriteLine("Error with send command: "+ exp.Message);
                Server.DisconnectClient(this);
            }
        }
    }
}
