using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BWBinding.Common
{
    public class VSKeyPair
    {
        private String key { get; }
        private byte[] value { get; }

        public VSKeyPair(String key, byte[] value)
        {
            this.key = key;
            this.value = value;
        }

        public void writeToStream(StreamWriter outputStream)
        {
            String header = String.Format("kv %s %d\n", key, this.value.Length);
            outputStream.Write(header);
            outputStream.Write(this.value);
        }

    }
}
