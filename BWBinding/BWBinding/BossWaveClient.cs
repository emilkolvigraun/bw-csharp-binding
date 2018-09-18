using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BWBinding.Common;
using BWBinding.Interfaces;

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
        private string server;
        private Int32 portNumber;
        private TcpClient client;
        private NetworkStream inputStream;

        private Dictionary<int, IResponseHandler> responseHandler;


        public BossWaveClient(string server, Int32 portNumber)
        {
            this.server = server;
            this.portNumber = portNumber;

            responseHandler = new Dictionary<int, IResponseHandler>();

            // Connect
            // client = new TcpClient(server, portNumber);
            // inputStream = client.GetStream();

            // Creating a ThreadStart delegate and pass in 'Run' (similar to run on Java's Runnable)
            new Thread(new ThreadStart(new BossListener().Run)).Start();
        }
        
        public void Dispose()
        {
            // Dispose of all components. Close the socket and the stream.
            // client.Close();
        }
    }
}
