using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BWBinding.Common
{
    class Response
    {
        private string status { get; }
        private string reason { get; }

        public Response(string status, string reason)
        {
            this.status = status;
            this.reason = reason;
        }
    }
}
