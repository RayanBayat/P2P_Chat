﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Text.Json;
using System.ComponentModel;
using static P2P_Chat.Models.ConnectionHandler;
using System.Windows.Interop;
using System.IO;

namespace P2P_Chat.Models
{



    public class Message
    {

        public string jsrequesttype { get; set; }

        public string jsname { get; set; }
        public string jsmsg { get; set; }


    }
    public class ConnectionHandler : INotifyPropertyChanged
    {

        bool connectionisAccepted, callincoming;
        private bool call_incoming;
        private Message _messages;
        TcpClient? client;
        TcpListener? server;
        NetworkStream? stream;
        public event PropertyChangedEventHandler? PropertyChanged;

        public Message Messages
        {
            get
            {
                return _messages;
            }
            set
            {
               _messages = value;
                OnPropertyChanged("Messages");
            }
        }
        public bool Call_Incoming
        {
            get
            {
                return callincoming;
            }
            set
            {
                callincoming = value;
               
                OnPropertyChanged("Call_Incoming");
            }
        }


        public void sendMessage(String message)
        {
            // Here is the code which sends the data over the network.
            // No user interaction shall exist in the model.
           
        }
        public void connect(String ip, Int32 port)
        {
            try
            {
                client = new TcpClient(ip, port);
                stream = client.GetStream();
                var handshake = new Message
                {
                    jsrequesttype = "HandShake",

                    jsname = "Rayan",

                    jsmsg = ""


                };
                senddata(handshake);
                Thread thread = new Thread(new ThreadStart(read_data));
                thread.Name = "en annan tråd";
                thread.Start();

            }
            catch (SocketException e)
            {
                MessageBox.Show("No one is listening on IP " + ip + " and Port " + port);
               
            }
           
        }


        public void read_data()
        {
            while (stream != null)
            {
                try
                {
                    Byte[] data = new Byte[256];

                    // String to store the response ASCII representation.
                    String responseData = String.Empty;
                    // Read the first batch of the TcpServer response bytes.
                    Int32 bytes = stream.Read(data, 0, data.Length);
                    responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                    string a = responseData;
                    if (a.IndexOf("Closing_connection") != -1)
                    {
                        break;
                    }
                    Message? msg = JsonSerializer.Deserialize<Message>(a);

                    if (msg.jsrequesttype == "BasicChat")
                    {
                        Messages = msg;
                    }
                    
                }
                catch (IOException e)
                {
                    Console.WriteLine("IOException: {0}", e);
                }
                catch (ArgumentNullException e)
                {
                    Console.WriteLine("ArgumentNullException: {0}", e);
                }
                catch (SocketException e)
                {
                    Console.WriteLine("SocketException: {0}", e);
                }

            }
        }
            public void prepare_to_send(String name, String message)
        {
            var msg = new Message
            {
                jsrequesttype = "BasicChat",

                jsname = name,

                jsmsg = message


            };
            senddata(msg);
        }
        public void senddata(Message message)
        {

                try
                {



                string json_data = JsonSerializer.Serialize(message);

                

                // Console.WriteLine(tcpc);
                // Translate the passed message into ASCII and store it as a Byte array.
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(json_data);

                    // Get a client stream for reading and writing.
                    

                    // Send the message to the connected TcpServer.
                    stream.Write(data, 0, data.Length);
                }
                catch (ArgumentNullException e)
                {
                    Console.WriteLine("ArgumentNullException: {0}", e);
                }
                catch (SocketException e)
                {
                    Console.WriteLine("SocketException: {0}", e);
                }
            
        }










        public void startListening(Int32 port)
        {
            try
            {

                IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                server = new TcpListener(localAddr, port);
                server.Start();
                Thread thread = new Thread(new ThreadStart(listeningloop));
                thread.Name = "en annan tråd";
                thread.Start();
            }
            catch (SocketException e)
            {
                MessageBox.Show("Cannot listen to Port " + port);

            }

        }

        private void listeningloop()
        {
            //MessageBox.Show(Thread.CurrentThread.Name);
            while (true)
            {
                if (!Call_Incoming)
                {
                    Byte[] bytes = new Byte[8096];
                    String data = null;

                    client = server.AcceptTcpClient();

                    Call_Incoming = true;
                    stream = client.GetStream();
                }

                
                if (connectionisAccepted)
                {
                    read_data();
                }
                

                

            }


        }


















        public void accept_connection()
        {
            connectionisAccepted = true;
            Call_Incoming = false;
        }
        public void decline_connection()
        {
            connectionisAccepted = false;
            Call_Incoming = false;
            client.Close();
        }
        public void close_connection()
        {
            connectionisAccepted = false;
            Call_Incoming = false;
            var close_connection = new Message
            {
                jsrequesttype = "Closing_connection",

                jsname = "System",

                jsmsg = ""


            };
            senddata(close_connection);
            client.Close();
        }



















        void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

    }
}
