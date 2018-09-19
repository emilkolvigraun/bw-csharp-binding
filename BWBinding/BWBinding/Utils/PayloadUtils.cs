using BWBinding.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BWBinding.Utils
{
    public static class PayloadUtils
    {
        private static byte[] Parse(string str)
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

        public static PayloadType FromString(string str)
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
    }
}
