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
        public String ToIP { get; set; }
        public Int32 port { get; set; }
        public String MessageToSend { get; set; }
        public ICommand PushCommand { get; set; }
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
            this.PushCommand = new SendMessageCommand(this);
            this.ListenCommand = new Listen(this);
            PopUpActive = true;
        }

        private void ConnectionHandler_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }





        // to Model
        public void sendMessage()
        {
            Connection.senddata(MessageToSend);
            
        }
        public void establishConnection()
        {
            Connection.connect(ToIP, port);
        }
        public void startListening()
        {
            Connection.startListening(ToIP, port);
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
