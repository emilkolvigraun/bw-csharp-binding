using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BWBinding.Common
{
    class Message
    {
        private string from;
        private string uri;

        public Message(string from, string uri)
        {
            this.from = from;
            this.uri = uri;
        }
    }
}
