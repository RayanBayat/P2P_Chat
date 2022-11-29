using P2P_Chat.Models;
using P2P_Chat.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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

        private Message _messagetostore;

        private bool _popupActive;
        private String _toIP = "127.0.0.1",_name="Bob",_messageToSend,_status="Disconnected";
        private Int32 _port = 22;


        private ConnectionHandler _connection;
        private ObservableCollection<Message> _messageslist;
        private FileWriter fileWriter;
        private FileWriter FileWriter
        {
            get { return fileWriter; }
            set { fileWriter = value; }
        }
        //commands
        public ICommand MessageCommand { get; set; }
        public ICommand ToIPCommand { get; set; }
        public ICommand ListenCommand { get; set; }
        public ICommand AcceptConnectionCommand { get; set; }
        public ICommand DeclineConnectionCommand { get; set; }
        public ICommand DisconnectCommand { get; set; }
        public ICommand ShowOldConversationCommand { get; set; }

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
        private ObservableCollection<Conversation> convoHistory;
        public ObservableCollection<Conversation> ConvoHistory
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
        //public struct Messagelist
        //{
        //    public string? _messages { get; set; }
        //    public string? _names{ get; set; }
        //}
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

        public Int32 Port
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

            this.ToIPCommand = new Connect(this);
            this.MessageCommand = new SendMessageCommand(this);
            this.ListenCommand = new Listen(this);
            this.AcceptConnectionCommand = new AcceptConnectionCommand(this);
            this.DeclineConnectionCommand = new DeclineConnectionCommand(this);
            this.DisconnectCommand = new DisconnectCommand(this);
            this.FileWriter = new FileWriter();
            this.ShowOldConversationCommand = new ShowOldConversationCommand(this);
            _messageslist = new ObservableCollection<Message>();
            
            connectionHandler.Messageslist.CollectionChanged += Messageslist_CollectionChanged;
            this.ConvoHistory = new ObservableCollection<Conversation>(FileWriter.GetHistory());
            MessageBox.Show(this.ConvoHistory.ToString());
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
            if (e.PropertyName == "Call_Incoming")
            {
                PopUpActive = Connection.Call_Incoming;
            }
            else if(e.PropertyName == "Status")
            {
                if (Connection.Status =="Connected")
                {
               
                    fileWriter.InitConversation(Connection.Othername);

                }
                Status = Connection.Status;
            }
            //else if (e.PropertyName == "MessageForStore")
            //{
            //    Messagetostore = Connection.MessageForStore;
            //    Saveit(Messagetostore);
            //}
           // MessageBox.Show(e. + e.PropertyName + " has changed");
          //  throw new NotImplementedException();
        }





        // to Model
        public void sendMessage()
        {
            Connection.prepare_to_send(MessageToSend);
            MessageToSend = "";


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
            Connection.connect(ToIP, Port,Name);
        }
        public void startListening()
        {
            Connection.startListening(Port,Name);
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
            _connection.accept_connection();
        }
        public void DeclineConnection()
        {
            _connection.decline_connection();
        }
        public void Disconnect()
        {
            _connection.close_connection();
        }

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            _connection.close_connection();
        }

        public void Saveit(Message msg)
        {
            FileWriter.WriteToFile(msg.msgToJson());
        }


        public void ShowOldConversationMethod(List<Message> aList)
        {
            if (!Connection.conencted)
            {
                Messageslist.Clear();
                aList.ToList().ForEach(a => Messageslist.Add(a)); ;
            }
        }
        //protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        //{
        //    //if (propertyName == "Search")
        //    //{
        //    //    FilterSearch();
        //    //}
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        //    }
        //}
    }
}
