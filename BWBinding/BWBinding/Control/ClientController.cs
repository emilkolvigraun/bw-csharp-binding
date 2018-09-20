using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using BWBinding.Interfaces;

namespace BWBinding.Control
{
    class ClientController
    {
        public Dictionary<int, IResponseHandler> responseHandlers { get; set; }
        public Dictionary<int, IMessageHandler> messageHandlers { get; set; }
        public Dictionary<int, IListResultHandler> resultHandlers { get; set; }
        public object responseLock { get; set; }
        public object messageLock { get; set; }
        public object resultLock { get; set; }
        public NetworkStream inputStream { get; set; }
        public BinaryWriter outputStream { get; set; }

        public ClientController()
        {
            // Singleton Instance
        }
    }
}
