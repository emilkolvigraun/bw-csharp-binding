using BWBinding.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BWBinding.Common
{
    class ClientController
    {
        public Dictionary<int, IResponseHandler> responseHandlers { get; set; }
        public Dictionary<int, IMessageHandler> messageHandlers { get; set; }
        public Dictionary<int, IListResultHandler> resultHandlers { get; set; }
        public object responseLock { get; set; }
        public object messageLock { get; set; }
        public object resultLock { get; set; }

        public ClientController()
        {
            // Singleton Instance
        }
    }
}
