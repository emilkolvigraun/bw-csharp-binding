using BWBinding.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BWBinding.Utils
{
    public static class Printer
    {
        public static string ConstructMessage(Message message)
        {
            string f_wall = "";
            string s_wall = "";
            string t_wall = ""; 
            string fo_wall = "";
            
            string roof = " ______________________________________________________________________________";
            string roof1 = "\n|           |                                                                  |\n";
            string bottom = "\n            |__________________________________________________________________|";

            string uri = message.uri;
            string from = message.from;
            string type = message.payloadObjects[0].type.ToString();
            string load = Encoding.UTF8.GetString(message.payloadObjects[0].load);

            if (uri.Length <= 59)
            {
                int count = 59 - uri.Length;
                for (int i = 0; i < count; i++)
                {
                    f_wall += " ";
                }

                f_wall += "|";
            }
            else
            {
                uri = BreakTillFit(uri, 53);
            }

            if (from.Length <= 58)
            {
                int count = 58 - from.Length;
                for (int i = 0; i < count; i++)
                {
                    s_wall += " ";
                }

                s_wall += "|";
            }
            else
            {
                from = BreakTillFit(from, 53);
            }

            if (type.Length <= 58)
            {
                int count = 58 - type.Length;
                for (int i = 0; i < count; i++)
                {
                    t_wall += " ";
                }

                t_wall += "|";
            }
            else
            {
                type = BreakTillFit(type, 53);
            }

            if (load.Length <= 55)
            {
                int count = 55 - load.Length;
                for (int i = 0; i < count; i++)
                {
                    fo_wall += " ";
                }

                fo_wall += "|";
            }
            else
            {
                load = BreakTillFit(load, 53);
            }
            return roof + roof1 + "|  Message  |  uri: " + uri + f_wall + "\n|___________|  from: " + from + s_wall + "\n            |  type: " + type + t_wall + "\n            |  message: " + load + fo_wall + bottom;
        }

        private static string AddEnd(int length, int next, string concerns)
        {
            int count = next - length;
            string temp = "";
            for (int i = 0; i < count; i++)
            {
                temp += " ";
            }

            temp += "|";
            return concerns.Insert(length, temp);
        }

        private static string BreakTillFit(string con, int first_divider)
        {
            string str = con;

            if (str[first_divider].Equals(' '))
            {
                str = str.Insert(first_divider, "  |\n            | ");
            }
            else
            {
                str = str.Insert(first_divider, "  |\n            |  ");
            } 
            int length = str.Length;
            int itt = 1;
            int ideal_length = 134;
            for (int i = 0; i < itt; i++)
            {
                if (length > ideal_length - 79)
                {


                    if (str[ideal_length].Equals(' '))
                    {
                        str = str.Insert(ideal_length, "  |\n            | ");
                    }
                    else
                    {
                        str = str.Insert(ideal_length, "  |\n            |  ");
                    }
                    ideal_length += 81;
                    itt += 1;
                }
            }

            str = AddEnd(str.Length, ideal_length + 2, str);

            return str;
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

        public static void PrintResponse(RequestType reason, string status)
        {
            Console.WriteLine(ConstructResponse(reason, status));
        }

        public static void PrintMessage(Message message)
        {
            Console.WriteLine(ConstructMessage(message));
        }
    }
}
