using System;
using BWBinding;
using BWBinding.Interfaces;

namespace Bootstrap
{
    class ClientImplementation
    {
        static void Main(string[] args)
        {
            IResponseHandler responseHandler = new ResponseHandler();
            IMessageHandler messageHandler = new MessageHandler();
            BossWaveClient bwClient = new BossWaveClient("localhost", 28589);

            bwClient.Connect();

            // bwClient.SetEntity("path to key file", responseHandler);
            // bwClient.Publish(publishrequest, responseHandler);
            // bwClient.Subscribe(subscriberequest, responseHandler, messageHandler);

            Console.WriteLine(bwClient.List());
            Console.WriteLine(bwClient.MakeChain());
            Console.WriteLine(bwClient.MakeDoT());
            Console.WriteLine(bwClient.Query());

            bwClient.Dispose();
        }
    }
}
