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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static P2P_Chat.Models.ConnectionHandler;
using System.Windows.Interop;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using System.Timers;
using static P2P_Chat.ViewModels.MainViewModel;
using System.Collections.ObjectModel;
using Application = System.Windows.Application;

namespace P2P_Chat.Models
{



    public class Message
    {

        public string? jsrequesttype { get; set; }

        public string? jsname { get; set; }
        public string? jsmsg { get; set; }
        public string? jstime { get; set; } 

        public JObject msgToJson()
        {
            return new JObject(
                    new JProperty("name", jsname),
                    new JProperty("msg", jsmsg),
                    new JProperty("time", jstime)
                    );
        }

    }
    public class ConnectionHandler : INotifyPropertyChanged
    {

        bool connectionisAccepted, callincoming;
        private bool call_incoming, islistening,conencted = false;
        private string? _status = "Disconnected", myname,othername;
        private Message? _messages;
        private Int32 port;
        private String ip;
        TcpClient? client;
        TcpListener? server;
        NetworkStream? stream;
        public event PropertyChangedEventHandler? PropertyChanged;
        private Message messageforstore;
        Thread ListenThread,SendThread;
        private ObservableCollection<Message> ?_messageslist = new ObservableCollection<Message>();
        public ObservableCollection<Message> Messageslist
        {
            get
            {
                return _messageslist;
            }
            set
            {
                _messageslist = value;
                //_messageslist.Add(value);
            }
        }
        public String Othername
        {
            get
            {
                return othername;
            }
            set
            {
                othername = value;

            }
        }
        public Message MessageForStore
        {
            get
            {
                return messageforstore;
            }
            set
            {
                messageforstore = value;
                
                OnPropertyChanged("MessageForStore");
            }
        }
        public string Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                OnPropertyChanged("Status");
            }
        }
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
        public void connect(String ip, Int32 port,string myname)
        {
            this.myname = myname;
            this.ip = ip;
            this.port = port;
            Thread thread = new Thread(new ThreadStart(tryconnecting));
            thread.Name = "en annan tråd";
            thread.Start();

        }

        private void tryconnecting()
        {
           
            try
            {
                client = new TcpClient(ip, port);
                stream = client.GetStream();
                var handshake = new Message
                {
                    jsrequesttype = "HandShake",

                    jsname = myname,

                    jsmsg = "",

                    jstime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")

                };
                senddata(handshake);
                ListenThread = new Thread(new ThreadStart(read_data));
                Thread thread1 = new Thread(new ThreadStart(last_handshake));
                ListenThread.Name = "lysnande tråd";
                ListenThread.Start();
                thread1.Name = "last shake";
                thread1.Start();
                Status = "Trying to connect";

            }
            catch (SocketException e)
            {
                MessageBox.Show("No one is listening on IP " + ip + " and Port " + port);

            }
        }
        public void read_data()
        {
            while (true)
            {
                try
                {
                    Byte[] data = new Byte[262144];

                    // String to store the response ASCII representation.
                    String responseData = String.Empty;
                    // Read the first batch of the TcpServer response bytes.
                    if (!client.Connected)
                    {
                        Status = "Disconnected";
                        if (islistening)
                        {
                            Status = "Listening";
                        }
                        break;
                    }
                    Int32 bytes = stream.Read(data, 0, data.Length);
                    responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                    string a = responseData;
                    Message? msg = System.Text.Json.JsonSerializer.Deserialize<Message>(a);

                    if (msg.jsrequesttype == "BasicChat")
                    {
                        Messages = msg;
                        Application.Current.Dispatcher.Invoke((System.Action)delegate
                        {
                            Messageslist.Add(msg);
                        });
                        MessageForStore = msg;
                    }
                    else if (msg.jsrequesttype == "HandShake")
                    {
                        
                        Othername = msg.jsname;
                        Status = "Connected";
                        conencted = true;
                        MessageBox.Show("Connected");

                    }
                    else if (msg.jsrequesttype == "Rejected")
                    {
                        Status = "Disconnected";
                        conencted = false;
                        if (islistening)
                        {
                            Status = "Listening";
                        }
                        MessageBox.Show("Connection was rejected");
                    }
                    else if(msg.jsrequesttype == "Closing_connection")
                    {
                        Status = "Disconnected";
                        conencted = false;
                        if (islistening)
                        {
                            Status = "Listening";
                        }
                        MessageBox.Show("connection was closed by user");
                        break;
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
                catch (ObjectDisposedException e)
                {
                    Console.WriteLine("ObjectDisposedException: {0}", e);
                }
                catch (System.Text.Json.JsonException e)
                {
                    Console.WriteLine("ObjectDisposedException: {0}", e);
                }

            }
        }
            public void prepare_to_send(String message)
        {
            
             var msg = new Message
            {
                jsrequesttype = "BasicChat",

                jsname = myname,

                jsmsg = message,

                jstime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")


            };
            Messages = msg;
            SendThread = new Thread(() => senddata(Messages));
            SendThread.Start();
        }

        private void last_handshake()
        {
            while(!conencted)
            {

            }
            var handshake = new Message
            {
                jsrequesttype = "HandShake",

                jsname = myname,

                jsmsg = "",

                jstime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")


            };
            senddata(handshake);
        }
        public void senddata(Message message)
        {
            JObject conversations;
                try
                {
                if (Status != "Disconnected")
                {
                    
                  //  File.Create(@"details.json");
                    string json_data = System.Text.Json.JsonSerializer.Serialize(message);
                    Byte[] data = System.Text.Encoding.ASCII.GetBytes(json_data);
                    stream.Write(data, 0, data.Length);

                    if (message.jsrequesttype == "BasicChat")
                    {
                        MessageForStore = message;
                        Messageslist.Add(message);
                    }
                    


                }
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










        public void startListening(Int32 port,String myname)
        {
            this.myname = myname;
            try
            {

                IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                server = new TcpListener(localAddr, port);
                server.Start();
                ListenThread = new Thread(new ThreadStart(listeningloop));
                ListenThread.Name = "lysnande tråd";
                islistening = true;
                ListenThread.Start();
                Status = "Listening";
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
                    try
                    {
                        if(!islistening)
                        {
                            break;
                        }
                        Byte[] bytes = new Byte[8096];
                        String data = null;

                        client = server.AcceptTcpClient();

                        Call_Incoming = true;
                        stream = client.GetStream();
                        Status = "Getting a call";
                    }
                    catch (SocketException e)
                    {
                        MessageBox.Show("Cannot listen to Port " + port);

                    }

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
            
 
            var handshake = new Message
            {
                jsrequesttype = "HandShake",

                jsname = myname,

                jsmsg = "",

                jstime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")


            };
            
            senddata(handshake);
        }
        public void decline_connection()
        {
            connectionisAccepted = false;
            Call_Incoming = false;
            conencted = false;
            var handshake = new Message
            {
                jsrequesttype = "Rejected",

                jsname = "",

                jsmsg = "",

                jstime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")


            };
            senddata(handshake);
            client.Close();
            Status = "Disconnected";
            if (islistening)
            {
                Status = "Listening";
            }
        }
        public void close_connection()
        {
            connectionisAccepted = false;
            Call_Incoming = false;
            conencted = false;
            var close_connection = new Message
            {
                jsrequesttype = "Closing_connection",

                jsname = "System",

                jsmsg = "test1",

                jstime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")


            };
            senddata(close_connection);
            client.Close();
            Status = "Disconnected";
            if (islistening)
            {
                Status = "Listening";
            }
            //islistening = false;
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
