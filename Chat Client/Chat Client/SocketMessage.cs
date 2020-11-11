using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat_Client
{
    public class SocketMessage
    {
        public string NickName { get; set; }
        public string SenderHostName { get; set; }
        public string SenderIpAddress { get; set; }
        public string ReceiverHostName { get; set; }
        public string ReceiverIpAddress { get; set; }
        public string ChatMessage { get; set; }

        public SocketMessage()
        {

        }

        public SocketMessage(User to, User from, MessageBody mb)
        {
            NickName = from.Name;
            SenderHostName = from.Name;
            SenderIpAddress = from.Ip;
            ReceiverHostName = to.Name;
            ReceiverIpAddress = to.Ip;
            ChatMessage = mb.Body;
        }
    }
}
