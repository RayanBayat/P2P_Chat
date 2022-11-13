using P2P_Chat.Models;
using P2P_Chat.ViewModels.Commands;
using System;
using System.Collections.Generic;
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
        public ConnectionHandler Connection { get; set; }

        private String toIP;
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
        public Int32 port { get; set; }

        private String messageToSend;
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
        private String messagerec;
        public String Messagerec
        {
            get
            {
                return messagerec;
            }
            set
            {
                messagerec = value;
            }

        }
        private String conv;
        public String conversation 
        {
            get
            {
                return conv;
            }
            set
            {
                conv = value;
                OnPropertyChanged("conversation");
            }
        
        }
        public ICommand MessageCommand { get; set; }
        public ICommand ToIPCommand { get; set; }
        public ICommand ListenCommand { get; set; }

        public bool PopUpActive { get; set; }
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
            PopUpActive = true;
        }

        private void ConnectionHandler_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
           
           // MessageBox.Show(e. + e.PropertyName + " has changed");
          //  throw new NotImplementedException();
        }





        // to Model
        public void sendMessage()
        {
            Connection.senddata(MessageToSend);
            conversation = MessageToSend;
        }
        public void establishConnection()
        {
            Connection.connect(ToIP, port);
        }
        public void startListening()
        {
            Connection.startListening(ToIP, port);
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
