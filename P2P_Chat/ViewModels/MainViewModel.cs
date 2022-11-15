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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace P2P_Chat.ViewModels
{
    public class MainViewModel :INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private bool _popupActive;
        private String messagerec;
        private String toIP;
        private ConnectionHandler connection;
        private Int32 port;
        private String messageToSend;
        private ObservableCollection<Messagelist> messageslist;

        public struct Messagelist
        {
            public string? msgrec { get; set; }
        }

        public ConnectionHandler Connection
        {
            get
            {
                return connection;
            }
            set
            {
                connection = value;
            }
        }

        public String ToIP
        {
            get
            {
                return toIP;
            }
            set
            {
                toIP = value;
            }
        }

        public Int32 Port
        {
            get
            {
                return port;
            }
            set
            {
                port = value;
            }
        }


        public String MessageToSend {
            get
            {
                return messageToSend;
            }
            set
            {
                messageToSend = value;
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
        public ObservableCollection<Messagelist> Messageslist
        {
            get
            {
                return messageslist;
            }
            set
            {
                messageslist = value;
                OnPropertyChanged("Messageslist");
            }
        }

       
      
        public ICommand MessageCommand { get; set; }
        public ICommand ToIPCommand { get; set; }
        public ICommand ListenCommand { get; set; }

        //{
        //    get { return _popupActive; }
        //    set { _popupActive = value;
        //     //   OnPropertyChanged("PopUpActive");
        //    }
        //}

        public MainViewModel(ConnectionHandler connectionHandler)
        {
            this.Connection = connectionHandler;
            // prenumere på event från connection handler
            connectionHandler.PropertyChanged += ConnectionHandler_PropertyChanged;

            this.ToIPCommand = new Connect(this);
            this.MessageCommand = new SendMessageCommand(this);
            this.ListenCommand = new Listen(this);
            messageslist = new ObservableCollection<Messagelist>();
        }

        private void ConnectionHandler_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Call_Incoming")
            {
                PopUpActive = connection.Call_Incoming;
            }
           // MessageBox.Show(e. + e.PropertyName + " has changed");
          //  throw new NotImplementedException();
        }





        // to Model
        public void sendMessage()
        {
            Connection.prepare_to_send(MessageToSend);
            Messageslist.Add(new Messagelist { msgrec = MessageToSend});
        }
        public void establishConnection()
        {
            Connection.connect(ToIP, Port);
        }
        public void startListening()
        {
            Connection.startListening(Port);
        }

        void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
                MessageBox.Show("changed");
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
