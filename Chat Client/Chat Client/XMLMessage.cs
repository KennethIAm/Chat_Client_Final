using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat_Client
{
    class XMLMessage : Message
    {
        private byte[] messageBuffer;
        public byte[] MessageBuffer { get; set; }

        public XMLMessage (User to, User from, MessageBody mb, byte[] messageBuffer) : base(to, from, mb)
        {
            MessageBuffer = messageBuffer;
        }
    }
}
