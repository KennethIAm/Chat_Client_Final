using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat_Client
{
    class PlainMessage : Message
    {
        private byte[] messageBuffer;
        public byte[] MessageBuffer { get; set; }

        public PlainMessage(User to, User from, MessageBody mb) : base(to, from, mb)
        {
            // For Camilla's server.
            MessageBuffer = Encoding.UTF8.GetBytes(From.Name + ":" + From.Ip + ":" + To.Name + ":" + To.Ip + ":" + Mb.Body + "\r\n");

            // For the class server.
            // MessageBuffer = Encoding.UTF8.GetBytes(From.Name + ":" + From.Name + ":"  + From.Ip + ":" + To.Name + ":" + To.Ip + ":" + Mb.Body + "{END}");
        }
    }
}
