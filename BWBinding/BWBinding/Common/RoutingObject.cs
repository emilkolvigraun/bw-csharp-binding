using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace BWBinding.Common
{
    class RoutingObject
    {
        private int routingNumber;
        private byte[] load;

        public RoutingObject(int routingNumber, byte[] load)
        {
            if (routingNumber < 0 || routingNumber > 255)
            {
                throw new ArgumentException("The Routing number must be: 0 < routingNumber > 255.");
            }
            this.routingNumber = routingNumber;
            this.load = load;
        }
        public void Write(BinaryWriter outputStream)
        {
            string header = string.Format("ro %d %d\n", routingNumber, load.Length);
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
            else if (!(o is PayloadObject))
            {
                return false;
            }
            else
            {
                RoutingObject other = (RoutingObject)o;
                return this.routingNumber.Equals(other.routingNumber) && ArraysEqual(this.load, other.load);
            }
        }
    }
}
