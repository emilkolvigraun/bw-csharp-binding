using System;
using System.Collections.Generic;

namespace BWBinding.Common
{

    public class PayloadType
    {
        private byte[] digitalInformation; // Octed
        private int number;

        public PayloadType(byte[] digitalInformation)
        {
            this.digitalInformation = digitalInformation;
            number = -1;
        }

        public PayloadType(int number)
        {
            this.number = number;
            digitalInformation = null;
        }

        public PayloadType(byte[] digitalInformation, int number)
        {
            this.digitalInformation = digitalInformation;
            this.number = number;
            if (!ValidateTypes(digitalInformation, number))
            {
                throw new ArgumentException("The Type Object is invalid.\n");
            }

        }

        private bool ValidateTypes(byte[] octet, int number)
        {
            int octetValue = (octet[0] << 24) + (octet[1] << 16) + (octet[2] << 8) + octet[3];
            return octetValue == number;

        }

        public override string ToString()
        {
            if (digitalInformation != null && number > 0)
            {
                return string.Format("%d.%d.%d.%d:%d", digitalInformation[0], digitalInformation[1], digitalInformation[2], digitalInformation[3], number);
            }
            else if (digitalInformation != null)
            {
                return string.Format("%d.%d.%d.%d:", digitalInformation[0], digitalInformation[1], digitalInformation[2], digitalInformation[3]);
            }
            else
            {
                return string.Format(":%d", number);
            }
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
            else if (!(o is PayloadType)) {
                return false;
            } else {
                PayloadType other = (PayloadType)o;
                return this.number == other.number && ArraysEqual(this.digitalInformation, other.digitalInformation);
            }
        }
    }
}
