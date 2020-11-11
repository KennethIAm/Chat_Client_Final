using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Security.Cryptography;

namespace Chat_Client
{
    public class MessageFactory
    {
        // Creates a Message object from a string.
        public static Message CreateMessage(byte messageType, string messageText, User to, User from)
        {
            MessageBody messageBody = new MessageBody();
            messageBody.Body = messageText;

            if (messageType == 1)
            {
                // Creates a Message.
                return new PlainMessage(to, from, messageBody);
            }
            else if (messageType == 2)
            {
                // For Camilla's server.
                Message message = new Message(to, from, messageBody);

                // For the class server.
                // SocketMessage message = new SocketMessage(to, from, messageBody);

                // Creates an XMLMessage.
                return new XMLMessage(to, from, messageBody, SerializeElement(message));
            }
            else if (messageType == 3)
            {
                // Encrypts the message.
                messageBody.Body = Convert.ToBase64String(EncryptSymmetricMessage(messageBody.Body));

                // For Camilla's server.
                Message message = new Message(to, from, messageBody);

                // For the class server.
                // SocketMessage message = new SocketMessage(to, from, messageBody);

                // Creates a SymmetricMessage.
                return new SymmetricMessage(to, from, messageBody, SerializeElement(message));
            }
            else if (messageType == 4)
            {
                // Creates a AsymmetricMessage.
                throw new NotImplementedException();
            }
            else
            {
                // Returns null in case of error.
                return null;
            }
        }

        // Serializes the element into an XML byte array.
        // Because the class server's data structure isn't like Camilla's server, it's taking an object
        // as its parameter, because it couldn't inherit from the Message class.
        private static byte[] SerializeElement(object message /*Message message*/)
        {
            XmlSerializer ser = new XmlSerializer(message.GetType());
            byte[] bufferStream = new byte[1024];
            MemoryStream s = new MemoryStream(bufferStream);
            ser.Serialize(s, message);
            return Encoding.UTF8.GetBytes(Encoding.UTF8.GetString(bufferStream).Replace("\0", ""));
        }

        // Encrypts message.
        private static byte[] EncryptSymmetricMessage(string plaintextMessage)
        {
            string key = "W+jcxfBJm37AAZujiktg4qCdy3k8D+vIrj4exFxFpIY=";
            byte[] messageArray = Encoding.UTF8.GetBytes(plaintextMessage);
            using (var aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Padding = PaddingMode.PKCS7;
                aesAlg.KeySize = 128;
                aesAlg.Key = Convert.FromBase64String(key);
                aesAlg.IV = new byte[128 / 8];

                using (var memoryStream = new MemoryStream())
                {
                    var cryptoStream = new CryptoStream(memoryStream, aesAlg.CreateEncryptor(),
                        CryptoStreamMode.Write);

                    cryptoStream.Write(messageArray, 0, messageArray.Length);
                    cryptoStream.FlushFinalBlock();

                    var encryptBytes = Convert.FromBase64String(Convert.ToBase64String(memoryStream.ToArray()));

                    return encryptBytes;
                }
            }
        }

        // Decrypts message.
        public static byte[] DecryptSymmetricMessage(byte[] encryptedMessage)
        {
            string key = "W+jcxfBJm37AAZujiktg4qCdy3k8D+vIrj4exFxFpIY=";
            using (var aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Padding = PaddingMode.PKCS7;
                aesAlg.KeySize = 128;
                aesAlg.Key = Convert.FromBase64String(key);
                aesAlg.IV = new byte[128 / 8];

                using (var memoryStream = new MemoryStream())
                {
                    var cryptoStream = new CryptoStream(memoryStream, aesAlg.CreateDecryptor(),
                        CryptoStreamMode.Write);

                    cryptoStream.Write(encryptedMessage, 0, encryptedMessage.Length);
                    cryptoStream.FlushFinalBlock();

                    var decryptBytes = memoryStream.ToArray();

                    return decryptBytes;
                }
            }
        }
    }
}
