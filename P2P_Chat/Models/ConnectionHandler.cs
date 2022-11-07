using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace P2P_Chat.Models
{
    public class ConnectionHandler
    {
        TcpClient? client;
        TcpListener? server;
        public void sendMessage(String message)
        {
            // Here is the code which sends the data over the network.
            // No user interaction shall exist in the model.
            MessageBox.Show(message);
        }
        public void connect(String ip, Int32 port)
        {
            try
            {
                client = new TcpClient(ip, port);
            }
            catch (SocketException e)
            {
                MessageBox.Show("No one is listening on IP " + ip + " and port " + port);
               
            }
           
        }
        public void startListening(String ip, Int32 port)
        {
            try
            {
                IPAddress localAddr = IPAddress.Parse(ip);
                server = new TcpListener(localAddr, port);
                server.Start();
            }
            catch (SocketException e)
            {
                MessageBox.Show("Cannot listen to IP " + ip + " and port " + port);

            }

        }

        public void senddata(String message)
        {

                try
                {
                    message += "\n";
                    // Console.WriteLine(tcpc);
                    // Translate the passed message into ASCII and store it as a Byte array.
                    Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

                    // Get a client stream for reading and writing.
                    NetworkStream stream = client.GetStream();

                    // Send the message to the connected TcpServer.
                    stream.Write(data, 0, data.Length);
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
            private void listeningloop()
        {
            while (true)
            {
                Byte[] bytes = new Byte[8096];
                String data = null;

                client = server.AcceptTcpClient();
                data = null;
                NetworkStream stream = client.GetStream();
            }

        }

    }
}
