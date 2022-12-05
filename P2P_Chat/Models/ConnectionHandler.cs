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
using System.Media;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace P2P_Chat.Models
{





    public class ConnectionHandler : INotifyPropertyChanged
    {
        //================================================================================================
        //Private Variables

        bool _connectionIsAccepted, _callIncoming, _isListening, _connected = false, isServer;
        string? _status = "Disconnected", _localhost = "192.168.1.240", _myname, _othername, _ip, _errorMessage;
        ObservableCollection<Message>? _messageslist = new ObservableCollection<Message>();
        Thread? _listenthread, _sendthread;
        int acceptconnection = 1, rejectconnection = 2, disconnect = 3, stoplistening = 4;
        Int32 _port, _buffersize = 524288;
        Message? _messages;
        TcpClient? _client;
        TcpListener? _server;
        NetworkStream? _stream;


        //================================================================================================
        //Public Variables
        public event PropertyChangedEventHandler? PropertyChanged;
        public ObservableCollection<Message> Messageslist { get { return _messageslist!; } set { _messageslist = value; } }
        public string Othername { get { return _othername!; } set { _othername = value; } }
        public string ErrorMessage { get { return _errorMessage!; } set { _errorMessage = value; OnPropertyChanged("ErrorMessage"); } }
        public bool Connected { get { return _connected!; } set { _connected = value; OnPropertyChanged("Connected"); } }
        public string Status { get { return _status!; } set { _status = value; OnPropertyChanged("Status"); } }
        public Message Message { get { return _messages!; } set { _messages = value; OnPropertyChanged("Message"); } }
        public bool Call_Incoming { get { return _callIncoming; } set { _callIncoming = value; OnPropertyChanged("Call_Incoming"); } }


        //================================================================================================
        //Private functions
        private void init(string ip, Int32 port, string myname)
        {
            this._myname = myname;
            this._ip = ip;
            this._port = port;
        }

        //=============================
        private Message makeMessage(string jsrequesttype, string jsname = "", string jsmsg = "")
        {
            //create messages with our protocol
            if (jsmsg == "Buzz")
                jsrequesttype = "Buzz";


            var theMessage = new Message
            {
                jsrequesttype = jsrequesttype,

                jsname = jsname,

                jsmsg = jsmsg,

                jstime = DateTime.Now.ToString()

            };
            return theMessage;
        }
        //=============================
        private void sendWithThread(Message message)
        {
            //send the message with the send thread
            _sendthread = new Thread(() => senddata(message));
            _sendthread.Name = "sending thread";
            _sendthread.Start();
        }
        //=============================
        private void listenWithThread(bool isServer)
        {
            //go into listening loop with listening thread
            _listenthread = new Thread(() => listeningloop(isServer));
            _listenthread.Name = "listening thread";
            _listenthread.Start();
        }
        //=============================
        private void tryConnectingAsClient()
        {
            isServer = false;
            try
            {
                //get stream and client
                _client = new TcpClient(_ip!, _port);
                _stream = _client.GetStream();

                //make tmpstring handshakemessage
                var handshake = makeMessage("HandShake", _myname!);
                Message = handshake;

                Status = "Trying to connect";
                //setting up listening and sending threads
                sendWithThread(Message);
                listenWithThread(isServer);
                // sendWithThread(handshake);
                //changing the status


            }
            catch (SocketException e)
            {
                ErrorMessage = "No one is listening on IP " + _ip + " and Port " + _port;
            }
        }
        //=============================
        private void read_data()
        {
            while (_client!.Connected)
            {
                try
                {
                    Byte[] data = new Byte[_buffersize];
                    String responseData = String.Empty;
                    if (_stream is not null)
                    {
                        Int32 bytes = _stream!.Read(data, 0, data.Length);
                        responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                        string tmpstring = responseData;

                        Message = System.Text.Json.JsonSerializer.Deserialize<Message>(tmpstring)!;
                    }
                    else
                    {
                        Status = "Disconnected";
                        Connected = false;
                        if (_isListening)
                        {
                            Status = "Listening";
                        }
                        break;
                    }


                    if (Message!.jsrequesttype == "BasicChat")
                    {
                        Application.Current.Dispatcher.Invoke((System.Action)delegate
                        {
                            Messageslist.Add(Message);
                        });

                    }
                    else if (Message.jsrequesttype == "HandShake")
                    {

                        Othername = Message.jsname!;
                        Status = "Connected";
                        Connected = true;


                    }
                    else if (Message.jsrequesttype == "Rejected")
                    {
                        Connected = false;
                        break;

                    }
                    else if (Message.jsrequesttype == "Closing_connection")
                    {

                        Connected = false;
                        break;
                    }
                    else if (Message.jsrequesttype == "Buzz")
                    {
                        OnPropertyChanged("Buzz");

                    }
                }
                catch (IOException e)
                {
                    //ErrorMessage = "IOException: {0}" +  e;
                    Debug.WriteLine("IOException: {0}" + e);
                }
                catch (ArgumentNullException e)
                {
                    //ErrorMessage = "ArgumentNullException: {0}" + e;
                    Debug.WriteLine("ArgumentNullException: {0}" + e);

                }
                catch (SocketException e)
                {
                    // ErrorMessage = "SocketException: {0}" + e;
                    Debug.WriteLine("SocketException: {0}" + e);

                }
                catch (ObjectDisposedException e)
                {
                    ErrorMessage = "ObjectDisposedException: {0}" + e;
                    //ErrorMessage = "Message recieved doesn't follow this apps protocol, make sure the user you're trying to connect to uses same application.";
                    Debug.WriteLine(ErrorMessage);

                }
                catch (System.Text.Json.JsonException e)
                {
                    //ErrorMessage = "ObjectDisposedException: {0}" + e;
                    ErrorMessage = "Message recieved doesn't follow this apps protocol, make sure the user you're trying to connect to uses same application.";
                    Debug.WriteLine(ErrorMessage);
                    break;
                }

            }
            Status = "Disconnected";
            if (_isListening)
            {
                Status = "Listening";
            }
        }
        //=============================
        private void senddata(Message message)
        {
            try
            {
                if (Status != "Disconnected")
                {
                    string json_data = System.Text.Json.JsonSerializer.Serialize(message);
                    Byte[] data = System.Text.Encoding.ASCII.GetBytes(json_data);
                    _stream!.Write(data, 0, data.Length);

                    if (message.jsrequesttype == "BasicChat")
                    {
                        Messageslist.Add(message);
                    }



                }
            }
            catch (ArgumentNullException e)
            {
                //ErrorMessage = "ArgumentNullException: {0}" + e;
                Debug.WriteLine("ArgumentNullException: {0}" + e);

            }
            catch (SocketException e)
            {
                //ErrorMessage = "SocketException: {0}" + e;
                Debug.WriteLine("SocketException: {0}" + e);

            }

        }
        //=============================
        private void listeningloop(bool isServer)
        {
            //MessageBox.Show(Thread.CurrentThread.Name);
            if (!isServer)
            {
                read_data();
            }
            else
            {
                while (_isListening)
                {
                    if (_connectionIsAccepted)
                    {
                        read_data();
                    }
                    if (!Call_Incoming)
                    {
                        try
                        {
                            _client = _server!.AcceptTcpClient();
                            _stream = _client.GetStream();
                            Call_Incoming = true;
                            Status = "Getting a call";

                        }
                        catch (SocketException e)
                        {
                            //ErrorMessage = "Cannot pick connection to " + _port;
                            Debug.WriteLine("Cannot pick connection to " + _port);

                        }

                    }

                }

            }

        }
        //=============================
        private void accept_connection()
        {
            _connectionIsAccepted = true;
            Call_Incoming = false;
            // Call_Incoming = false;

            Message = makeMessage("HandShake", _myname!);
            senddata(Message);
        }
        //=============================
        private void decline_connection()
        {
            Message = makeMessage("Rejected", _myname!);
            senddata(Message);
            _connectionIsAccepted = false;
            Call_Incoming = false;
            Connected = false;
            _client!.Close();
            Status = "Disconnected";
            if (_isListening)
            {
                Status = "Listening";
            }
        }
        //=============================
        private void close_connection()
        {
            if (Connected)
            {
                Message = makeMessage("Closing_connection", _myname!);
                senddata(Message);
                _connectionIsAccepted = false;
                Call_Incoming = false;

                Connected = false;
                _client!.Close();
            }
            Status = "Disconnected";
            if (_isListening)
            {
                Status = "Listening";
            }
        }
        //=============================
        private void stopListening()
        {
            if (Connected)
            {
                _connectionIsAccepted = false;
                Call_Incoming = false;
                Message = makeMessage("Closing_connection", _myname!);
                senddata(Message);
                Connected = false;

                Status = "Disconnected";
                if (_isListening)
                {
                    Status = "Listening";
                }
            }
            _server!.Stop();
            _isListening = false;
        }
        //================================================================================================
        //Public functions

        public void connect(String ip, Int32 port, string myname)
        {
            if (_isListening)
            {
                ErrorMessage = "You're trying to connect to the port " + _port + ", you're listening to this port";
                return;
            }
            init(ip, port, myname);
            Thread localthread = new Thread(() => tryConnectingAsClient());
            localthread.Start();

        }
        //=============================
        public void prepare_to_send(String message)
        {
            Message = makeMessage("BasicChat", _myname!, message);
            sendWithThread(Message);
        }
        //=============================
        public void startListening(Int32 port, String myname)
        {
            if (_isListening)
            {
                ErrorMessage = "You're already listening to Port " + _port;
                return;
            }
            init(_localhost!, port, myname);
            isServer = true;
            try
            {
                Status = "Listening";
                //starting a tcp listener with the given port
                IPAddress localAddr = IPAddress.Parse(_ip!);
                _server = new TcpListener(localAddr, port);
                _server.Start();

                // setting up the listening loop
                listenWithThread(isServer);

                _isListening = true;
                
            }
            catch (SocketException e)
            {
                ErrorMessage = "Cannot listen to Port " + port;

                Debug.WriteLine(ErrorMessage);
            }

        }
        //=============================
        public void take_action(int action)
        {

            Thread localthread;

            if (action == acceptconnection)
            {
                localthread = new Thread(() => accept_connection());
                localthread.Start();
            }
            else if (action == rejectconnection)
            {
                localthread = new Thread(() => decline_connection());
                localthread.Start();
            }
            else if (action == disconnect)
            {
                localthread = new Thread(() => close_connection());
                localthread.Start();
            }
            else if (action == stoplistening)
            {
                localthread = new Thread(() => stopListening());
                localthread.Start();
            }


        }
        //=============================
        public void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

    }
}
