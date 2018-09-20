using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BWBinding.Common
{
    class PayloadObject
    {
        private PayloadType type;
        private byte[] load { get; }

        public PayloadObject(PayloadType type, byte[] load)
        {
            this.type = type;
            this.load = load;
        }
        public void Write(StreamWriter outputStream)
        {
            Console.WriteLine("Gets here. 4");
            string header = string.Format("po %s %d\n", type, load.Length);
            outputStream.Write(Encoding.UTF8.GetBytes(header));
            outputStream.Write(load);
            outputStream.Write('\n');
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
