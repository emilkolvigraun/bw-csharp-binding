using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BWBinding.Common
{
    public class PayloadObject
    {
        public PayloadType type;
        public byte[] load { get; }

        public PayloadObject(PayloadType type, byte[] load)
        {
            this.type = type;
            this.load = load;
        }

        public void Write(Stream outputStream)
        {
            byte[] header = Encoding.UTF8.GetBytes(string.Format("po {0} {1}\n", type, load.Length));
            byte[] newLine = Encoding.UTF8.GetBytes("\n");
            outputStream.Write(header, 0, header.Length);
            outputStream.Write(load, 0, load.Length);
            outputStream.Write(newLine, 0, newLine.Length);
            outputStream.Flush();
        }

        public override bool Equals(object obj)
        {
            var @object = obj as PayloadObject;
            return @object != null &&
                   EqualityComparer<PayloadType>.Default.Equals(type, @object.type) &&
                   EqualityComparer<byte[]>.Default.Equals(load, @object.load);
        }
    }
}
