using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat_Client
{
    class AsymmetricMessage : Message
    {
        public AsymmetricMessage(User to, User from, MessageBody mb) : base(to, from, mb)
        {

        }
    }
}
