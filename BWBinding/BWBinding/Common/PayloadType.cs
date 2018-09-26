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
                return string.Format("{0}.{1}.{2}.{3}:{4}", digitalInformation[0], digitalInformation[1], digitalInformation[2], digitalInformation[3], number);
            }
            else if (digitalInformation != null)
            {
                return string.Format("{0}.{1}.{2}.{3}:", digitalInformation[0], digitalInformation[1], digitalInformation[2], digitalInformation[3]);
            }
            else
            {
                return string.Format(":{0}", number);
            }
        }

        public override bool Equals(object obj)
        {
            var type = obj as PayloadType;
            return type != null &&
                   EqualityComparer<byte[]>.Default.Equals(digitalInformation, type.digitalInformation) &&
                   number == type.number;
        }
    }
}
