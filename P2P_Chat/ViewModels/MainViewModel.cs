using P2P_Chat.Models;
using P2P_Chat.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace P2P_Chat.ViewModels
{
    public class MainViewModel
    {
        private ConnectionHandler _connection;
        private String _messageToSend,_IP;
        private ICommand _pushCommand;
        private ICommand _ToIPCommand;

        public ConnectionHandler Connection
        {
            get { return _connection; }
            set
            {
                _connection = value;
            }

        }
        public String ToIP
        {
            get { return _IP; }
            set { _IP = value; }
        }
        public String MessageToSend
        {
            get { return _messageToSend; }
            set { _messageToSend = value; }
        }

        public ICommand PushCommand
        {
            get { return _pushCommand; }
            set { _pushCommand = value; }
        }
        public ICommand ToIPCommand
        {
            get { return _ToIPCommand; }
            set { _ToIPCommand = value; }
        }

        public MainViewModel(ConnectionHandler connectionHandler)
        {
            this.Connection = connectionHandler;
            this.ToIPCommand = new Connect(this);
            this.PushCommand = new SendMessageCommand(this);
            
        }
        public void sendMessage()
        {
            Connection.sendMessage(MessageToSend);
        }
        public void establishConnection()
        {
           
            Connection.sendMessage(ToIP);
        }
    }
}
