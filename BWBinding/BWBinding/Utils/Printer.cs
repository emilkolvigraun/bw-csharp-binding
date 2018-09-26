using BWBinding.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BWBinding.Utils
{
    public static class Printer
    {
        public static void ConstructMessage()
        {

        }

        public static string ConstructResponse(RequestType reason, string status)
        {
            string wall = " |";
            if (reason.ToString().Length < 13)
            {
                wall = "";
                int sl = 13 - reason.ToString().Length;
                for (int i = 0; i < sl; i++)
                {
                    wall += " ";
                }

                wall += "|";
            }
            return " _________________________ " + "\n|          |              |" + "\n| Response | status: " + status + " |" + "\n|__________| " + reason.ToString() + wall + "\n           |______________|"; ;
        }
    }
}
