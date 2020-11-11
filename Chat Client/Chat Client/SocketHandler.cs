using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Chat_Client
{
    class SocketHandler : ICommunicationHandler, IConnect
    {
        // Server IP: 172.16.2.30
        // Port: 8888
        // XML port: 8889
        // Symmetric port: 8890
        // Asymmetric port: 8891

        IPAddress host = IPAddress.Parse("172.16.2.30");
        int port = 8888;
        TcpClient tcpClient = new TcpClient();
        Socket socket;

        public SocketHandler()
        {
            socket = tcpClient.Client;
        }

        // Connects to the server.
        public void Connect()
        {
            bool connected = false;
            while (connected == false)
            {
                try
                {
                    socket.Connect(host, port);
                    connected = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Thread.Sleep(5000);
                }
            }
        }

        // Receives data from the server.
        public string Receive()
        {
            byte[] bufferReceiver = new byte[60000];
            bool check = false;

            if (!tcpClient.GetStream().DataAvailable && !check) 
            {
                check = true;
                return null;
            }
            else
            {
                try
                {
                    tcpClient.GetStream().Read(bufferReceiver, 0, tcpClient.Available);
                    Thread.Sleep(300);
                    check = false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                return Encoding.UTF8.GetString(bufferReceiver);
            }
        }

        // Sends message.
        public void Send(byte[] byteBuffer)
        {
            try
            {
                socket.Send(byteBuffer);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        // Changes the port.
        public void SetPort(int newPort)
        {
            port = newPort;
        }
    }
}
