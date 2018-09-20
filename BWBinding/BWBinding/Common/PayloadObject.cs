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

        private bool ArraysEqual<T>(T[] a1, T[] a2)
        {
            if (ReferenceEquals(a1, a2))
                return true;

            if (a1 == null || a2 == null)
                return false;

            if (a1.Length != a2.Length)
                return false;

            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < a1.Length; i++)
            {
                if (!comparer.Equals(a1[i], a2[i])) return false;
            }
            return true;
        }
        public override bool Equals(object o)
        {
            if (o == this)
            {
                return true;
            }
            else if (o == null)
            {
                return false;
            }
            else if (!(o is PayloadObject)) {
                return false;
            } else {
                PayloadObject other = (PayloadObject)o;
                return this.type.Equals(other.type) && ArraysEqual(this.load, other.load);
            }
        }
    }
}
