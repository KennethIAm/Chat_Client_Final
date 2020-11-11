using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chat_Client
{
    class ChatController
    {
        private User sender;
        private User receiver;
        private byte messageType;
        private ICommunicationHandler handler;
        private List<string> chatLog = new List<string>();

        public User Sender { get => sender; set => sender = value; }
        public User Receiver { get => receiver; set => receiver = value; }
        public byte MessageType { get => messageType; set => messageType = value; }
        public ICommunicationHandler Handler { get => handler; set => handler = value; }
        public List<string> ChatLog { get => chatLog; set => chatLog = value; }

        public ChatController(string receiverIP, string senderIP, byte messageType, ICommunicationHandler handler,
            string senderName, string receiverName )
        {
            Sender = new User(senderName, senderIP);
            Receiver = new User(receiverName, receiverIP);
            MessageType = messageType;

            // Instantiates the communication handler.
            Handler = handler;

            // Sets the port depending on what message type the user chose.
            if (messageType == 2)
            {
                handler.SetPort(8889);
            }
            else if (messageType == 3)
            {
                handler.SetPort(8890);
            }
            else if (messageType == 3)
            {
                handler.SetPort(8891);
            }

            ChatLog.Add("~~ Welcome to Chat3000 - the chat client of the future ~~");
        }

        // Connects to the server if the handler implements IConnect.
        public void Connect()
        {
            // As ICommunicationHandler doesn't have the Connect method, it casts the handler as an IConnect if it has the interface.
            if (Handler is IConnect)
            {
                IConnect sHandler = (IConnect)handler;
                sHandler.Connect();
            }
        }

        // Creates and sends a message.
        public void SendMessage(string userMessage)
        {
            if (userMessage.Remove(1, userMessage.Length - 1) == "/")
            {
                // Runs the HandleCommand message if starts with "/", as that's a command.
                HandleCommand(userMessage);
            }
            else
            {
                // Creates a Message object.
                Message message = InstantiateMessage(userMessage);

                if (message is PlainMessage)
                {
                    // Casts the Message as a PlainMessage to reach its MessageBuffer.
                    PlainMessage plainMessage = (PlainMessage)message;
                    Handler.Send(plainMessage.MessageBuffer);
                }
                else if (message is XMLMessage)
                {
                    // Casts the Message as an XMLnMessage to reach its MessageBuffer.
                    XMLMessage xmlMessage = (XMLMessage)message;
                    Handler.Send(xmlMessage.MessageBuffer);
                }
                else if (message is SymmetricMessage)
                {
                    // Casts the Message as a SymmetricMessage to reach its MessageBuffer.
                    SymmetricMessage symmetricMessage = (SymmetricMessage)message;
                    Handler.Send(symmetricMessage.MessageBuffer);
                }
                else if (message is AsymmetricMessage)
                {
                    // Casts the Message as an AsymmetricMessage to reach its MessageBuffer.
                    //AsymmetricMessage asymmetricMessage = (AsymmetricMessage);
                    //Handler.Send(asymmetricMessage.MessageBuffer);
                }
            }
            UpdateUI();
        }

        // Instatiates and returns a message.
        private Message InstantiateMessage(string userMessage)
        {
            Message message = null;
            switch (MessageType)
            {
                case 1:
                    message = MessageFactory.CreateMessage(1, userMessage, Receiver, Sender);
                    break;
                case 2:
                    message = MessageFactory.CreateMessage(2, userMessage, Receiver, Sender);
                    break;
                case 3:
                    message = MessageFactory.CreateMessage(3, userMessage, Receiver, Sender);
                    break;
                case 4:
                    break;
                default:
                    break;
            }
            return message;
        }

        // Handles commands.
        public void HandleCommand(string userMessage)
        {
            if (userMessage.Contains("/ip ") && userMessage.Remove(4, userMessage.Length - 4).ToLower() == "/ip ")
            {
                // Changes Receiver's IP or IP and name.
                UpdateReceiver(userMessage);
            }
            else if (userMessage.Contains("/name "))
            {
                // Changes the Sander's name.
                Sender.Name = userMessage.Remove(0, 6);
            }
            else
            {
                // Informs usser about an incorrect co
                chatLog.Add("Error: " + userMessage + " is not a valid command.");
                UpdateUI();
            }
        }

        // Updates either ReceiverIP or ReceiverIP and ReceiverName
        public void UpdateReceiver(string userMessage)
        {
            if (userMessage.Contains(";"))
            {
                // If the command contains ";" after the IP, it will also update the Receiver's name. 
                Receiver.Ip = userMessage.Remove(0, 4);
                Receiver.Ip = "172.16.2." + Receiver.Ip.Remove(receiver.Ip.IndexOf(":"), Receiver.Ip.Length - Receiver.Ip.IndexOf(":"));
                Receiver.Name = userMessage.Remove(0, 7);
            }
            else
            {
                // Just updates Receiver's IP.
                Receiver.Ip = "172.16.2." + userMessage.Remove(0, 4);
            }
        }

        // Recieves a message fromt he server.
        public void ReceiveMessage()
        {
            string returnValue;
            while (true)
            {
                returnValue = Handler.Receive();

                if (returnValue != null)
                {
                    switch (MessageType)
                    {
                        case 1:
                            PlaintextConverter(returnValue);
                            break;
                        case 2:
                            XMLConverter(returnValue);
                            break;
                        case 3:
                            SymmetricConverter(returnValue);
                            break;
                        case 4:
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        // Converts Plaintext messages from the server to chat messages.
        public void PlaintextConverter(string returnValue)
        {
            // Finds the user's name.
            string user = returnValue.Substring(0, returnValue.IndexOf(':'));

            // Find's the message body.
            string post = "";
            for (int i = 0; i < 4; i++)
            {
                post = returnValue.Substring(returnValue.LastIndexOf(':') + 1);
            }

            // Updates the chat log and the UI.
            UpdateChatLog(user, post.Replace("\0", "").Replace("\n", ""));
            UpdateUI();
        }

        // Converts XML messages from the server to chat messages.
        public void XMLConverter(string returnValue)
        {
            if (returnValue.Contains("<Name>"))
            {
                // Finds the user's name.
                string user = returnValue.Replace("\0", "");
                for (int i = 0; i < 10; i++)
                    user = user.Substring(user.IndexOf('>') + 1);
                user = user.Substring(0, user.IndexOf('<'));

                // Finds the message body.
                string body = returnValue.Replace("\0", "");
                for (int i = 0; i < 16; i++)
                    body = body.Substring(body.IndexOf('>') + 1);
                body = body.Substring(0, body.IndexOf('<'));

                // Updates the chat log and the UI.
                UpdateChatLog(user, body);
                UpdateUI();
            }
        }

        // Converts Symmetric messages from the server to chat messages.
        public void SymmetricConverter(string returnValue)
        {
            if (returnValue.Contains("<Name>"))
            {
                // Finds the user's name.
                string user = returnValue.Replace("\0", "");
                for (int i = 0; i < 10; i++)
                    user = user.Substring(user.IndexOf('>') + 1);
                user = user.Substring(0, user.IndexOf('<'));

                // Finds the message body.
                string body = returnValue.Replace("\0", "");
                for (int i = 0; i < 16; i++)
                    body = body.Substring(body.IndexOf('>') + 1);
                body = body.Substring(0, body.IndexOf('<'));

                // Decrypts the encrypted message.
                body = Encoding.UTF8.GetString(MessageFactory.DecryptSymmetricMessage(Convert.FromBase64String(body)));

                // Updates the chat log and the UI.
                UpdateChatLog(user, body);
                UpdateUI();
            }
        }

        // Gets the key from the server.
        public void GetKey()
        {

        }

        // Clears current console line.
        public static void ClearCurrentConsoleLine(int cursor)
        {
            Console.SetCursorPosition(0, Console.CursorTop - cursor);
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }

        // Updates the ChatLog with the new message.
        public void UpdateChatLog(string user, string post)
        {
            ChatLog.Add("[" + DateTime.Now.ToString() + "] " + user + " : " + post);
        }
        
        // Updates the UI.
        public void UpdateUI()
        {
            Console.Clear();

            foreach (string s in ChatLog)
            {
                try
                {
                    if (s.Remove(0, 22).Substring(0, 11) == "USER-ONLINE" || s == "~~ Welcome to Chat3000 - the chat client of the future ~~" || s.Substring(0, 2) == "OK")
                    {
                        // Makes chat and server messages cyan.
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine(s);
                    }
                    else if (s.Remove(0, 22).Substring(0, Sender.Name.Length) == Sender.Name)
                    {
                        // Makes the user's messages green.
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(s);
                    }
                    else
                    {
                        // Makes the other users' messages red.
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(s);
                    }
                }
                catch
                {
                    // Makes unknown text yellow.
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(s);
                }
            }
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("_________________________________________________________");
            Console.Write("Enter Message: ");
        }
    }
}
