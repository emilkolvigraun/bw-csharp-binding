using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace BWBinding.Common
{
    public class VSKeyPair
    {
        public string key { get; private set; }
        public byte[] value { get; private set; }

        public VSKeyPair(string key, byte[] value)
        {
            this.key = key;
            this.value = value;
        }

        public void Write(Stream outputStream)
        {
            byte[] header = Encoding.UTF8.GetBytes(string.Format("kv {0} {1}\n", key, value.Length));
            byte[] newLine = Encoding.UTF8.GetBytes("\n");
            outputStream.Write(header, 0, header.Length);
            outputStream.Write(value, 0, value.Length);
            outputStream.Write(newLine, 0, newLine.Length);
            outputStream.Flush();
        }

    }
}
