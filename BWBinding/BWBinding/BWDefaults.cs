using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BWBinding.Common;
using BWBinding.Interfaces;

namespace BWBinding
{
    public static class BWDefaults
    {
        public static readonly int DEFAULT_PORT_NUMBER = 28589;
        private static Dictionary<int, IResponseHandler> defaultResponseHandlers = new Dictionary<int, IResponseHandler>();
        private static Dictionary<int, IMessageHandler> defaultMessageHandlers = new Dictionary<int, IMessageHandler>();

        public static IResponseHandler DEFAULT_RESPONSEHANDLER(int id)
        {
            if (defaultResponseHandlers.Count >= 1)
            {
                if (defaultResponseHandlers.ContainsKey(id))
                {
                    return defaultResponseHandlers[id];
                }
                else
                {
                    defaultResponseHandlers.Add(id, new ResponseHandler());
                    return defaultResponseHandlers[id];
                }
            }
            else
            {
                defaultResponseHandlers.Add(id, new ResponseHandler());
                return defaultResponseHandlers[id];
            }
        }

        public static IMessageHandler DEFAULT_MESSAGEHANDLER(int id)
        {
            if (defaultMessageHandlers.Count >= 1)
            {
                if (defaultMessageHandlers.ContainsKey(id))
                {
                    return defaultMessageHandlers[id];
                }
                else
                {
                    defaultMessageHandlers.Add(id, new MessageHandler());
                    return defaultMessageHandlers[id];
                }
            }
            else
            {
                defaultMessageHandlers.Add(id, new MessageHandler());
                return defaultMessageHandlers[id];
            }
        }
    }

    class ResponseHandler : IResponseHandler
    {
        public Response result { get; private set; }

        public bool received { get; private set; }

        public void ResponseReceived(Response result)
        {
            this.result = result;
        }
    }

    class MessageHandler : IMessageHandler
    {
        public Message message { get; private set; }

        public bool received { get; private set; }

        public void ResultReceived(Message message)
        {
            this.message = message;
        }
    }
}
