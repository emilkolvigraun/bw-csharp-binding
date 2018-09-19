using System;
using System.IO;
using System.Text;

namespace BWBinding.Common
{
    class PayloadObject
    {
        private Type type;
        private byte[] load { get; }

        public PayloadObject(Type type, byte[] load)
        {
            this.type = type;
            this.load = load;
        }

        public void writeToStream(StreamWriter outputStream)
        {
            string header = string.Format("po %s %d\n", type, load.Length);
            outputStream.Write(Encoding.UTF8.GetBytes(header));
            outputStream.Write(load);
        }
    }
}
