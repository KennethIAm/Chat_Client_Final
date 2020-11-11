using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat_Client
{
    public class Message
    {
        public User To { get; set; }
        public User From { get; set; }
        public MessageBody Mb { get; set; }

        public Message()
        {

        }

        public Message(User to, User from, MessageBody mb)
        {
            To = to;
            From = from;
            Mb = mb;
        }
    }
}
