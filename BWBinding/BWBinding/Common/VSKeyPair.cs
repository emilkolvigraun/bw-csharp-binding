using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BWBinding.Common
{
    public class VSKeyPair
    {
        private string key { get; }
        private byte[] value { get; }

        public VSKeyPair(string key, byte[] value)
        {
            this.key = key;
            this.value = value;
        }

        public void Write(StreamWriter outputStream)
        {
            string header = string.Format("kv %s %d\n", key, value.Length);
            outputStream.Write(header);
            outputStream.Write(value);
        }

    }
}
