using BWBinding.Common;
using System;
using System.ComponentModel;
using System.Reflection;
using BWBinding.Exceptions;

namespace BWBinding.Utils
{
    class CommandUtils
    {
        public static string GetCode(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            else
            {
                return value.ToString();
            }
        }
        public static Command GetCommand(string value)
        {
            string[] names = Enum.GetNames(typeof(Command));
            foreach (string name in names)
            {
                if (GetCode((Enum)Enum.Parse(typeof(Command), name)).Equals(value))
                {
                    return (Command) Enum.Parse(typeof(Command), name);
                }
            }

            throw new CorruptedFrameException("The Frame Header contains an invalid command: " + value);
        }
    }
}
