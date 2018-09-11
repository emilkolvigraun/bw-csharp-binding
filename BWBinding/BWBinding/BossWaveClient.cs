using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BWBinding
{
    /**
     *  Aggregator class
     *  @Author: Emil Stubbe Kolvig-Raun
     *  
     *  Implements IDisposable, c# equivalent to Java's AutoClosable
    **/
    public class BossWaveClient : IDisposable
    {
        private string hostName;
        private int portNumber;

        public BossWaveClient(String hostName, int portNumber)
        {
            this.hostName = hostName;
            this.portNumber = portNumber;
            // Declare other logic.
        }
        
        public void Dispose()
        {
            // Dispose of all components. Close the socket and the stream.
        }
    }
}
