using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BWBinding.Common
{

    class Type
    {
        private byte[] digitalInformation; // Octed
        private int number;

        public Type(byte[] digitalInformation)
        {
            this.digitalInformation = digitalInformation;
            number = -1;
        }

        public Type(int number)
        {
            this.number = number;
            digitalInformation = null;
        }

        public Type(byte[] digitalInformation, int number)
        {
            this.digitalInformation = digitalInformation;
            this.number = number;
            if (!ValidateTypes(digitalInformation, number))
            {
                throw new ArgumentException("The Type Object is invalid.\n");
            }

        }

        private static bool ValidateTypes(byte[] octet, int number)
        {
            int octetValue = (octet[0] << 24) + (octet[1] << 16) + (octet[2] << 8) + octet[3];
            return octetValue == number;

        }
    }
}
