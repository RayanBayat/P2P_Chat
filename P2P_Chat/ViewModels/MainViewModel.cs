using Microsoft.Win32;
using P2P_Chat.Models;
using P2P_Chat.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Media;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;

namespace P2P_Chat.ViewModels
{
    public class MainViewModel :INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        int acceptconnection = 1, rejectconnection = 2, disconnect = 3, stoplistening = 4;
        private Message _messagetostore;

        private bool _popupActive,connected = false;
        private String _toIP ,_name,_messageToSend,_status="Disconnected",_search;
        private String _port ;


        private ConnectionHandler _connection;
        private ObservableCollection<Message> _messageslist;
        private WritingToStorage _writingToStorage;
        private WritingToStorage WritingToStorage
        {
            get { return _writingToStorage; }
            set { _writingToStorage = value; }
        }
        //commands
        public ICommand MessageCommand { get; set; }
        public ICommand ToIPCommand { get; set; }
        public ICommand ListenCommand { get; set; }
        public ICommand AcceptConnectionCommand { get; set; }
        public ICommand DeclineConnectionCommand { get; set; }
        public ICommand DisconnectCommand { get; set; }
        public ICommand ShowOldConversationCommand { get; set; }

        public bool Connected
        {
            get
            {
                return connected;
            }
            set
            {
                connected = value;
                OnPropertyChanged("Connected");
            }
        }
        public Message Messagetostore
        {
            get
            {
                return _messagetostore;
            }
            set
            {
                _messagetostore = value;
            }
        }
        private ObservableCollection<oneConversation> convoHistory;
        public ObservableCollection<oneConversation> ConvoHistory
        {
            get 
            { 
                return convoHistory; 
            }
            set
            {
                convoHistory = value;
                OnPropertyChanged("ConvoHistory");
            }
        }
        public String Search
        {
            get
            {
                return _search;
            }
            set
            {
                _search = value;
                Searchnames();
            }
        }
        public String Status
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
        public ConnectionHandler Connection
        {
            get
            {
                return _connection;
            }
            set
            {
                _connection = value;
            }
        }

        public String ToIP
        {
            get
            {
                return _toIP;
            }
            set
            {
                _toIP = value;
            }
        }

        public String Port
        {
            get
            {
                return _port;
            }
            set
            {
                _port = value;
            }
        }


        public String MessageToSend {
            get
            {
                return _messageToSend;
            }
            set
            {
                _messageToSend = value;
                OnPropertyChanged("MessageToSend");
            }

        }

        public bool PopUpActive
        {
            get
            {
                return _popupActive;

            }
            set
            {
                _popupActive = value;
                OnPropertyChanged("PopupActive");
            }
        }
        public ObservableCollection<Message> Messageslist
        {
            get
            {
                return _messageslist;
            }
            set
            {
                _messageslist = value;
                OnPropertyChanged("Messageslist");
            }
        }
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

       
      



        public MainViewModel(ConnectionHandler connectionHandler)
        {
            this.Connection = connectionHandler;
            // prenumere på event från _connection handler
            connectionHandler.PropertyChanged += ConnectionHandler_PropertyChanged;
            connectionHandler.Messageslist.CollectionChanged += Messageslist_CollectionChanged;
            

            this.ToIPCommand = new Connect(this);
            this.MessageCommand = new SendMessageCommand(this);
            this.ListenCommand = new Listen(this);
            this.AcceptConnectionCommand = new AcceptConnectionCommand(this);
            this.DeclineConnectionCommand = new DeclineConnectionCommand(this);
            this.DisconnectCommand = new DisconnectCommand(this);
            this.WritingToStorage = new WritingToStorage();
            this.ConvoHistory = new ObservableCollection<oneConversation>(WritingToStorage.GetHistory());
            
            this.ShowOldConversationCommand = new ShowOldConversationCommand(this);
            _messageslist = new ObservableCollection<Message>();
            

            //MessageBox.Show(this.ConvoHistory.ToString());
        }

        private void Messageslist_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (var item in e.NewItems)
            {
                print_on_screen((Message)item);
                Saveit((Message)item);
                //MessageBox.Show(item.ToString());
            }
          //  MessageBox.Show(e.NewItems);
        }

        private void ConnectionHandler_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Call_Incoming":
                    PopUpActive = Connection.Call_Incoming;break;
                case "Status":
                    if (Connection.Status == "Connected")
                    {

                        WritingToStorage.InitConversation(Connection.Othername);
                        if (Status != Connection.Status)
                        {
                            Application.Current.Dispatcher.Invoke((System.Action)delegate
                            {
                                Messageslist.Clear();
                            });
                        }
                        

                    }
                    if (!Connection.Connected)
                    {
                        this.ConvoHistory = new ObservableCollection<oneConversation>(WritingToStorage.GetHistory());
                    }
                    Status = Connection.Status;
                    break;

                case "ErrorMessage":
                    MessageBox.Show(Connection.ErrorMessage); break;

                case "Buzz":
                    playBuzz(); break;
                case "Connected":
                   // MessageBox.Show("now connected " + Connection.Connected.ToString());
                    this.Connected = Connection.Connected; break;

            }


        }





        // to Model
        public void sendMessage()
        {

           Connection.prepare_to_send(MessageToSend);
           MessageToSend = String.Empty;


        }

        public void print_on_screen(Message msg)
        {
            Application.Current.Dispatcher.Invoke((System.Action)delegate
            {
                Messageslist.Add(msg);
            });
        }
        public void establishConnection()
        {
            if (string.IsNullOrEmpty(Port) || string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(ToIP))
            {
                MessageBox.Show("Make sure to fill in IP, port and your name to connect.");
                return;
            }
            Connection.connect(ToIP, Int32.Parse(Port), Name);
        }
        public void startListening()
        {
            if (string.IsNullOrEmpty(Port) || string.IsNullOrEmpty(Name))
            {
                MessageBox.Show("Make sure to fill in both port and your name to start listening.");
                return;
            }
            Connection.startListening(Int32.Parse(Port), Name);
        }

        void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
        public void AcceptConnection()
        {
            _connection.take_action(acceptconnection);
        }
        public void DeclineConnection()
        {
            _connection.take_action(rejectconnection);
        }
        public void Disconnect()
        {
            _connection.take_action(disconnect);
        }

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            _connection.take_action(stoplistening);
        }

        public void Saveit(Message msg)
        {
            WritingToStorage.WriteToFile(msg.msgToJson());
        }


        public void ShowOldConversationMethod(List<Message> aList)
        {
            if (!Connection.Connected)
            {
                Messageslist.Clear();
                aList.ToList().ForEach(a => Messageslist.Add(a)); ;
            }
        }
        private void Searchnames()
        {
            ConvoHistory = new ObservableCollection<oneConversation>(WritingToStorage.filter(Search));
        }
        private void playBuzz()
        {
            SystemSounds.Beep.Play();
        }
    }
}
