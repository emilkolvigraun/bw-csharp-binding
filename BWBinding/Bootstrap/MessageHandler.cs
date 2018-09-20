using System;
using System.Collections.Generic;
using System.Text;
using BWBinding.Common;
using BWBinding.Interfaces;

namespace Bootstrap
{
    class MessageHandler : IMessageHandler
    {
        public MessageHandler()
        {
            // Initializer
        }

        void IMessageHandler.ResultReceived(Message message)
        {
            string uri = message.uri;
            string from = message.from;

            byte[] messageBytes = message.payloadObjects[0].load;
            PayloadType type = message.payloadObjects[0].type;

            Console.WriteLine("uri: " + uri + "\nfrom: " + from + "\ntype: " + type.ToString() + "\nmessage content: " + Encoding.UTF8.GetString(messageBytes));

        }
    }
}
