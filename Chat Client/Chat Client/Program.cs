using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Threading;

namespace Chat_Client
{
    class Program
    {


        static void Main(string[] args)
        {
            byte messageType = 0;
            int ip = 0;
            Console.ForegroundColor = ConsoleColor.White;

            // Ensures the selected Message Type isn't out of range.
            Console.WriteLine("Plaintext = 1, XML = 2, Symmetric Encryption = 3, Asymmetric Encryption = 4 (Currently Unavailable)");
            while (messageType < 1 || messageType > 3)
            {
                Console.Write("Enter Message Type: ");
                byte.TryParse(Console.ReadLine(), out messageType);

                if (messageType < 1 || messageType > 3)
                {
                    ChatController.ClearCurrentConsoleLine(1);
                }
            }

            // Ensures Senders's name doesn't contain forbidden characters.
            string userName = "<";
            Console.WriteLine();
            while (userName.Contains('<') || userName.Contains('>') || userName.Contains(':'))
            {
                Console.Write("Enter Username: ");
                userName = Console.ReadLine();

                if (userName.Contains('<') || userName.Contains('>') || userName.Contains(':'))
                {

                    ChatController.ClearCurrentConsoleLine(1);
                }
            }

            // Ensures Receiver's IP isn't out of range.
            string receiverIP = "172.16.2.";
            Console.WriteLine();
            while (ip < 1 || ip > 99)
            {
                Console.Write("Enter Final Receiver IP Digits: ");
                Int32.TryParse(Console.ReadLine(), out ip);

                if (ip < 1 || ip > 99)
                {
                    ChatController.ClearCurrentConsoleLine(1);
                }
            }

            // Ensures Receiver's name doesn't contain forbidden characters.
            string receiverName = ">";
            Console.WriteLine();
            while (receiverName.Contains('<') || receiverName.Contains('>') || receiverName.Contains(':'))
            {
                Console.Write("Enter Receiver's Name: ");
                receiverName = Console.ReadLine();

                if (receiverName.Contains('<') || receiverName.Contains('>') || receiverName.Contains(':'))
                {
                    ChatController.ClearCurrentConsoleLine(1);
                }
            }

            

            ChatController chatController = new ChatController(receiverIP, Dns.GetHostByName(Dns.GetHostName()).AddressList[0].ToString(),
                1, new SocketHandler(), userName, receiverName);

            //ChatController chatController = new ChatController(Dns.GetHostByName(Dns.GetHostName()).AddressList[0].ToString(),
            //    Dns.GetHostByName(Dns.GetHostName()).AddressList[0].ToString(), 3, new SocketHandler(), "Kenneth", "Kenneth");

            Console.Clear();
            
            Thread loadingThread = new Thread(new ThreadStart(chatController.Connect));

            //loadingThread.Start();
            while (loadingThread.IsAlive)
            {
                Console.WriteLine("LOADING");
                Console.WriteLine();
                Console.Write(". ");

                Thread.Sleep(1000);

                Console.Write(". ");

                Thread.Sleep(1000);

                Console.Write(". ");

                Thread.Sleep(1000);

                Console.Clear();
            }

            chatController.UpdateUI();

            Thread receiveThread = new Thread(new ThreadStart(chatController.ReceiveMessage));

            //receiveThread.Start();

            while (true)
            {
                chatController.SendMessage(Console.ReadLine());
            }
        }
    }
}
