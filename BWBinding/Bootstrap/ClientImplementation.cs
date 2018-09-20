using System;
using BWBinding;

namespace Bootstrap
{
    class ClientImplementation
    {
        static void Main(string[] args)
        {
            ResponseHandler responseHandler = new ResponseHandler();
            // MakeEntityRequest entityRequest = new MakeEntityRequest("test_contact", "test entity", DateTime.MaxValue, Int64.MaxValue, new List<string>(), false);


            BossWaveClient bwClient = new BossWaveClient("localhost", 28589);

            bwClient.Connect();

            // Purpose is to execute / test commands
            bwClient.SetEntity("C:/Users/Emil S. Kolvig-Raun/bankroll.ent", responseHandler);
            // bwClient.MakeEntity(entityRequest, responseHandler, null);

            Console.WriteLine(bwClient.Subscribe());
            Console.WriteLine(bwClient.Publish());
            Console.WriteLine(bwClient.List());
            Console.WriteLine(bwClient.MakeChain());
            Console.WriteLine(bwClient.MakeDoT());
            Console.WriteLine(bwClient.Query());

            // bwClient.Dispose();
        }
    }
}
