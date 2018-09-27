using System;
using System.Text;
using System.Threading;
using BWBinding;
using BWBinding.Common;
using BWBinding.Interfaces;
using BWBinding.Utils;

namespace Bootstrap
{
    class ClientImplementation
    {
        private static Semaphore semaphore;
        private static IResponseHandler subscribeHandler;
        private static IResponseHandler publishHandler;
        private static IResponseHandler publishHandler1;
        private static IResponseHandler publishHandler2;
        private static IMessageHandler nsgHandler;

        static void Main(string[] args)
        {

            semaphore = new Semaphore(0, 1);
            subscribeHandler = new ResponseHandler();
            publishHandler = new ResponseHandler();
            publishHandler1 = new ResponseHandler();
            publishHandler2 = new ResponseHandler();
            nsgHandler = new MessageHandler();

            test();
        }

        private static void test()
        {

            BossWaveClient bwClient = new BossWaveClient("localhost", BWDefaults.DEFAULT_PORT_NUMBER);

            bwClient.Connect();

            bwClient.SetEntity("C:/Users/Emil S. Kolvig-Raun/stubbe.ent", BWDefaults.DEFAULT_RESPONSEHANDLER(1));

            Request subscribeRequest = new RequestUtils("stubbe.demo.dk/sharptesting", RequestType.SUBSCRIBE)
                .SetExpiryDelta(3600000)
                .SetAutoChain(true)
                .BuildSubcriber();

            bwClient.Subscribe(subscribeRequest, subscribeHandler, nsgHandler);

            semaphore.WaitOne();
            RequestUtils publishRequestUtils = new RequestUtils("stubbe.demo.dk/sharptesting", RequestType.PUBLISH)
                .SetPrimaryAccessChain("klAVQ9UOajySAZZWzdU4IBrdyqs85JOk3TJhnpecFqg=");
            byte[] message = Encoding.UTF8.GetBytes("Rest is for the wicked.");
            byte[] text = { 64, 0, 0, 0 };
            PayloadObject payload = new PayloadObject(new PayloadType(text), message);
            publishRequestUtils.AddPayloadObject(payload);
            Request publishRequest = publishRequestUtils.BuildPublisher();
            bwClient.Publish(publishRequest, publishHandler);

            semaphore.WaitOne();
            Printer.PrintResponse(RequestType.MAKE_ENTITY, BWDefaults.DEFAULT_RESPONSEHANDLER(1).result.status);
            Printer.PrintResponse(RequestType.SUBSCRIBE, subscribeHandler.result.status);
            Printer.PrintResponse(RequestType.PUBLISH, publishHandler.result.status);
            semaphore.WaitOne();
            Printer.PrintMessage(nsgHandler.message);

            RequestUtils publishRequestUtils1 = new RequestUtils("stubbe.demo.dk/sharptesting", RequestType.PUBLISH)
                .SetPrimaryAccessChain("klAVQ9UOajySAZZWzdU4IBrdyqs85JOk3TJhnpecFqg=");
            string message1 = "Oh man, dawg! That sucks, cause I could really use some goddamn sleep right now. Ya' know, I have been awake for 43 hours? Oh man, dawg! That sucks, cause I could really use some goddamn sleep right now. Ya' know, I have been awake for 43 hours? Oh man, dawg! That sucks, cause I could really use some goddamn sleep right now. Ya' know, I have been awake for 43 hours?";
            byte[] text1 = { 64, 0, 0, 0 };
            publishRequestUtils1.AddPayloadObject(new PayloadObject(new PayloadType(text1), Encoding.UTF8.GetBytes(message1)));
            Request publishRequest1 = publishRequestUtils1.BuildPublisher();
            bwClient.Publish(publishRequest1, publishHandler1);

            semaphore.WaitOne();
            semaphore.WaitOne();
            Printer.PrintMessage(nsgHandler.message);


            RequestUtils publishRequestUtils2 = new RequestUtils("stubbe.demo.dk/sharptesting", RequestType.PUBLISH)
                .SetPrimaryAccessChain("klAVQ9UOajySAZZWzdU4IBrdyqs85JOk3TJhnpecFqg=");
            byte[] message2 = Encoding.UTF8.GetBytes("You can sleep at 11PM!");
            byte[] text2 = { 64, 0, 0, 0 };
            PayloadObject payload2 = new PayloadObject(new PayloadType(text2), message2);
            publishRequestUtils2.AddPayloadObject(payload2);
            Request publishRequest2 = publishRequestUtils2.BuildPublisher();
            bwClient.Publish(publishRequest2, publishHandler2);

            semaphore.WaitOne();
            semaphore.WaitOne();
            Printer.PrintMessage(nsgHandler.message);

        }


        class ResponseHandler : IResponseHandler
        {
            public Response result { get; set; }

            public bool received { get; set; }

            public void ResponseReceived(Response result)
            {
                this.result = result;
                received = true;
                semaphore.Release();
            }
        }

        class MessageHandler : IMessageHandler
        {
            public Message message { get; set; }

            public bool received { get; set; }

            public void ResultReceived(Message message)
            {
                this.message = message;
                received = true;
                semaphore.Release();
            }
        }

    }

}
