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
        static void Main(string[] args)
        {
            Console.WriteLine(Printer.ConstructResponse(RequestType.MAKE_ENTITY, "okay"));
            Console.WriteLine(Printer.ConstructResponse(RequestType.SUBSCRIBE, "okay"));
            Console.WriteLine(Printer.ConstructResponse(RequestType.MAKE_DOT, "okay"));
            Console.WriteLine(Printer.ConstructResponse(RequestType.PUBLISH, "okay"));
            
        }

        private void test()
        {
            BossWaveClient bwClient = new BossWaveClient("localhost", BWDefaults.DEFAULT_PORT_NUMBER);

            bwClient.Connect();

            bwClient.SetEntity("C:/Users/Emil S. Kolvig-Raun/stubbe.ent", BWDefaults.DEFAULT_RESPONSEHANDLER(1));


            Request subscribeRequest = new RequestUtils("stubbe.demo.dk/sharptesting", RequestType.SUBSCRIBE)
                .SetExpiryDelta(3600000)
                .SetAutoChain(true)
                .BuildSubcriber();

            bwClient.Subscribe(subscribeRequest, BWDefaults.DEFAULT_RESPONSEHANDLER(2), BWDefaults.DEFAULT_MESSAGEHANDLER(2));

            if (!BWDefaults.DEFAULT_RESPONSEHANDLER(2).received)
            {
                Console.Write("Working"); // This looks awesome
                Thread.SpinWait(6666666); //Wait for subscribe to finish, before publishing.
                Console.Write(".");
                Thread.SpinWait(6666666); //Wait for subscribe to finish, before publishing.
                Console.Write(".");
                Thread.SpinWait(6666666); //Wait for subscribe to finish, before publishing.
                Console.WriteLine(".");

            }

            RequestUtils publishRequestUtils = new RequestUtils("stubbe.demo.dk/sharptesting", RequestType.PUBLISH)
                .SetPrimaryAccessChain("klAVQ9UOajySAZZWzdU4IBrdyqs85JOk3TJhnpecFqg=");
            byte[] message = Encoding.UTF8.GetBytes("Rest is for the wicked.");
            byte[] text = { 64, 0, 0, 0 };
            PayloadObject payload = new PayloadObject(new PayloadType(text), message);
            publishRequestUtils.AddPayloadObject(payload);
            Request publishRequest = publishRequestUtils.BuildPublisher();
            bwClient.Publish(publishRequest, BWDefaults.DEFAULT_RESPONSEHANDLER(3));

            if (!BWDefaults.DEFAULT_RESPONSEHANDLER(3).received)
            {
                Thread.SpinWait(10000000); // Wait before printing
            }

            Console.WriteLine(" ________________ \n|                |\n" + "|  status: " + BWDefaults.DEFAULT_RESPONSEHANDLER(1).result.status + "  |" + "\n|  set entity    |" + "\n|________________|");
            Console.WriteLine(" ________________ \n|                |\n" + "|  status: " + BWDefaults.DEFAULT_RESPONSEHANDLER(2).result.status + "  |" + "\n|  subscribed    |" + "\n|________________|");
            Console.WriteLine(" ________________ \n|                |\n" + "|  status: " + BWDefaults.DEFAULT_RESPONSEHANDLER(3).result.status + "  |" + "\n|  published     |" + "\n|________________|");
            Console.WriteLine(" __________________________________________________________________\n|                                                                  |\n" + "|  uri: " + BWDefaults.DEFAULT_MESSAGEHANDLER(2).message.uri + "  |" + "\n|  from: " + BWDefaults.DEFAULT_MESSAGEHANDLER(2).message.from + "              |" + "\n|  type: " + BWDefaults.DEFAULT_MESSAGEHANDLER(2).message.payloadObjects[0].type.ToString() + "                                       |" + "\n|  message: " + Encoding.UTF8.GetString(BWDefaults.DEFAULT_MESSAGEHANDLER(2).message.payloadObjects[0].load) + "                                |" + "\n|__________________________________________________________________|");

            RequestUtils publishRequestUtils1 = new RequestUtils("stubbe.demo.dk/sharptesting", RequestType.PUBLISH)
                .SetPrimaryAccessChain("klAVQ9UOajySAZZWzdU4IBrdyqs85JOk3TJhnpecFqg=");
            string message1 = "That sucks, cause I could really use some sleep.";
            byte[] text1 = { 64, 0, 0, 0 };
            publishRequestUtils1.AddPayloadObject(new PayloadObject(new PayloadType(text1), Encoding.UTF8.GetBytes(message1)));
            Request publishRequest1 = publishRequestUtils1.BuildPublisher();
            bwClient.Publish(publishRequest1, BWDefaults.DEFAULT_RESPONSEHANDLER(4));

            if (!BWDefaults.DEFAULT_RESPONSEHANDLER(4).received)
            {
                Thread.SpinWait(10000000); // Wait before printing
            }

            Console.WriteLine(" __________________________________________________________________\n|                                                                  |\n" + "|  uri: " + BWDefaults.DEFAULT_MESSAGEHANDLER(2).message.uri + "  |" + "\n|  from: " + BWDefaults.DEFAULT_MESSAGEHANDLER(2).message.from + "              |" + "\n|  type: " + BWDefaults.DEFAULT_MESSAGEHANDLER(2).message.payloadObjects[0].type.ToString() + "                                       |" + "\n|  message: " + Encoding.UTF8.GetString(BWDefaults.DEFAULT_MESSAGEHANDLER(2).message.payloadObjects[0].load) + "       |" + "\n|__________________________________________________________________|");

            RequestUtils publishRequestUtils2 = new RequestUtils("stubbe.demo.dk/sharptesting", RequestType.PUBLISH)
                .SetPrimaryAccessChain("klAVQ9UOajySAZZWzdU4IBrdyqs85JOk3TJhnpecFqg=");
            byte[] message2 = Encoding.UTF8.GetBytes("You can sleep at 11PM!");
            byte[] text2 = { 64, 0, 0, 0 };
            PayloadObject payload2 = new PayloadObject(new PayloadType(text2), message2);
            publishRequestUtils2.AddPayloadObject(payload2);
            Request publishRequest2 = publishRequestUtils2.BuildPublisher();
            bwClient.Publish(publishRequest2, BWDefaults.DEFAULT_RESPONSEHANDLER(5));

            if (!BWDefaults.DEFAULT_RESPONSEHANDLER(5).received)
            {
                Thread.SpinWait(10000000); // Wait before printing
            }

            Console.WriteLine(" __________________________________________________________________\n|                                                                  |\n" + "|  uri: " + BWDefaults.DEFAULT_MESSAGEHANDLER(2).message.uri + "  |" + "\n|  from: " + BWDefaults.DEFAULT_MESSAGEHANDLER(2).message.from + "              |" + "\n|  type: " + BWDefaults.DEFAULT_MESSAGEHANDLER(2).message.payloadObjects[0].type.ToString() + "                                       |" + "\n|  message: " + Encoding.UTF8.GetString(BWDefaults.DEFAULT_MESSAGEHANDLER(2).message.payloadObjects[0].load) + "                                 |" + "\n|__________________________________________________________________|");

        }
    }


}
