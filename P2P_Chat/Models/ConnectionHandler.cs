using System;
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

namespace P2P_Chat.Models
{

 


    public class ConnectionHandler : INotifyPropertyChanged
    {
        public class Message
        {

            public string jsrequesttype { get; set; }

            public string jsname { get; set; }
            public string jsmsg { get; set; }


        }

        TcpClient? client;
        TcpListener? server;
        public string ab;
        public string a
        {
            get
            {
                return ab;
            }
            set
            {
                ab = value;
                OnPropertyChanged(ab);

            }

        }
        public event PropertyChangedEventHandler? PropertyChanged;

        public void sendMessage(String message)
        {
            // Here is the code which sends the data over the network.
            // No user interaction shall exist in the model.
            MessageBox.Show(message);
        }
        public void connect(String ip, Int32 port)
        {
            port = 21;
            ip = "127.0.0.1";
            try
            {
                client = new TcpClient(ip, port);
            }
            catch (SocketException e)
            {
                MessageBox.Show("No one is listening on IP " + ip + " and port " + port);
               
            }
           
        }
        public void startListening(String ip, Int32 port)
        {
            try
            {
                MessageBox.Show("starting to listen");
                IPAddress localAddr = IPAddress.Parse(ip);
                server = new TcpListener(localAddr, port);
                server.Start();
                Thread thread = new Thread(new ThreadStart(listeningloop));
                thread.Name = "en annan tråd";
                thread.Start();
            }
            catch (SocketException e)
            {
                MessageBox.Show("Cannot listen to IP " + ip + " and port " + port);

            }

        }

        public void senddata(String message)
        {

                try
                {

                var Message = new Message
                {
                    jsrequesttype = "BasicChat",

                    jsname = "someone",

                    jsmsg = message
                   
                };

                a = message;
                string json_data = JsonSerializer.Serialize(Message);

                

                // Console.WriteLine(tcpc);
                // Translate the passed message into ASCII and store it as a Byte array.
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(json_data);

                    // Get a client stream for reading and writing.
                    NetworkStream stream = client.GetStream();

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
            private void listeningloop()
        {
            MessageBox.Show(Thread.CurrentThread.Name);
            while (true)
            {
                Byte[] bytes = new Byte[8096];
                String data = null;

                client = server.AcceptTcpClient();
                if (client != null)
                {
                    MessageBox.Show("client connected");
                    client.Close();
                    continue;
                }
                //if (client == null)
                //{
                //    client.Close();
                //}
                NetworkStream stream = client.GetStream();
                int i;

                // Loop to receive all the data sent by the client.
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    // Translate data bytes to a ASCII string.
                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

                    string jsondata = data;

                    Message? msg = JsonSerializer.Deserialize<Message>(jsondata);

                    MessageBox.Show(msg.jsmsg, msg.jsrequesttype);
                   // Console.WriteLine("Sent: {0}", data);
                }
            }


        }
        void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
                MessageBox.Show("something changed");
            }
        }

    }
}
