using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat_Client
{
    interface ICommunicationHandler
    {
        void Send(byte[] byteBuffer);
        string Receive();
        void SetPort(int newPort);
    }
}
