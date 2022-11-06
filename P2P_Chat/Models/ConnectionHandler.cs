using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace P2P_Chat.Models
{
    public class ConnectionHandler
    {
        TcpClient? client;
        public void sendMessage(String message)
        {
            // Here is the code which sends the data over the network.
            // No user interaction shall exist in the model.
            MessageBox.Show(message);
        }
        public void connect(String ip, Int32 port)
        {
            client = new TcpClient(ip,port);
        }

    }
}
