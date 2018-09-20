using System;
using System.Collections.Generic;
using System.Text;
using BWBinding.Common;
using BWBinding.Interfaces;

namespace Bootstrap
{
    class ResponseHandler : IResponseHandler
    {
        public void ResponseReceived(Response result)
        {
            Console.Write("Received response: ");
            if (!result.status.Equals("okay"))
            {
                Console.WriteLine("'SetEntity(...)' action Failed because: " + result.reason);
            }
            else
            {
                Console.WriteLine("'SetEntity(...)' action Complete.'");
            }
        }
    }
}
