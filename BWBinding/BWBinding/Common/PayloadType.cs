using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private byte[] Parse(string str)
        {
            string[] informationTokens = str.Split('\\');
            if (informationTokens.Length != 4)
            {
                throw new ArgumentException("The Octed() has to contain four elements. Yours doesn't. \n");
            }
            byte[] octetBytes = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                try
                {
                    octetBytes[i] = byte.Parse(informationTokens[i]);
                }
                catch (FormatException ex)
                {
                    throw new ArgumentException("Some of the elements in the Octet() are invalid: " + informationTokens[i], ex);
                }

                if (octetBytes[i] < 0)
                {
                    throw new ArgumentException("There's a negative Octed() element: " + informationTokens[i]);
                }
            }
            return octetBytes;
        }

        public PayloadType FromString(string str)
        {
            if (str.StartsWith(":"))
            {
                int payloadNumber;
                try
                {
                    payloadNumber = int.Parse(str.Substring(1));
                }
                catch (FormatException ex)
                {
                    throw new ArgumentException("The Payload contains an invalid number.\n", ex);
                }

                if (payloadNumber < 0 || payloadNumber > 99)
                {
                    throw new ArgumentException("The Payload number can only consist of one or two digits.\n");
                }
                return new PayloadType(payloadNumber);
            }
            else if (str.EndsWith(":"))
            {
                string digitalInformationPayload = str.Substring(0, str.Length - 1);
                byte[] digitalInformation;
                try
                {
                    digitalInformation = Parse(digitalInformationPayload);
                }
                catch (ArgumentException ex)
                {
                    throw new ArgumentException("The Payload contains an invalid Octed().\n", ex);
                }
                return new PayloadType(digitalInformation);
            }
            else
            {
                string[] payloadTypeTokens = str.Split(':');
                if (payloadTypeTokens.Length != 2)
                {
                    throw new ArgumentException("The PayloadType is malformed.");
                }
                byte[] digitalInformation;
                try
                {
                    digitalInformation = Parse(payloadTypeTokens[0]);
                }
                catch (ArgumentException ex)
                {
                    throw new ArgumentException("The Payload contains an invalid Octed().\n", ex);
                }

                int payloadNumber;
                try
                {
                    payloadNumber = int.Parse(payloadTypeTokens[1]);
                }
                catch (FormatException ex)
                {
                    throw new ArgumentException("The Payload contains an invalid number.\n", ex);
                }
                return new PayloadType(digitalInformation, payloadNumber);
            }
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
