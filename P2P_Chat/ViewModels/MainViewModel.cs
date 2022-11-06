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
        public ConnectionHandler Connection { get; set; }
        public String ToIP { get; set; }
        public Int32 port { get; set; }
        public String MessageToSend { get; set; }
        public ICommand PushCommand { get; set; }
        public ICommand ToIPCommand { get; set; }

        public MainViewModel(ConnectionHandler connectionHandler)
        {
            this.Connection = connectionHandler;
            this.ToIPCommand = new Connect(this);
            this.PushCommand = new SendMessageCommand(this);
            
        }



        // to Model
        public void sendMessage()
        {
            Connection.sendMessage(MessageToSend);
        }
        public void establishConnection()
        {
            Connection.connect(ToIP, port);
        }
    }
}
