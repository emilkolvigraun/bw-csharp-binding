using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace BWBinding.Common
{
    public class RoutingObject
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

        public override bool Equals(object obj)
        {
            var @object = obj as RoutingObject;
            return @object != null &&
                   routingNumber == @object.routingNumber &&
                   EqualityComparer<byte[]>.Default.Equals(load, @object.load);
        }

        public void Write(Stream outputStream)
        {
            byte[] header = Encoding.UTF8.GetBytes(string.Format("ro {0} {1}\n", routingNumber, load.Length));
            byte[] newLine = Encoding.UTF8.GetBytes("\n");
            outputStream.Write(header, 0, header.Length);
            outputStream.Write(load, 0, load.Length);
            outputStream.Write(newLine, 0, newLine.Length);
            outputStream.Flush();
        }
    }
}
