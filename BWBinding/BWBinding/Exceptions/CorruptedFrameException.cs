using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BWBinding.Exceptions
{
    public class CorruptedFrameException : Exception
    {
        public CorruptedFrameException(string message) : base(message) => Console.WriteLine(message);

        public CorruptedFrameException(string message, Exception innerException) : base(message, innerException) => Console.WriteLine(message + "\n" + innerException.ToString());
    }
}
