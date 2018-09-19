using System;
using BWBinding;

namespace Bootstrap
{
    class ClientImplementation
    {
        static void Main(string[] args)
        {
            BossWaveClient bwClient = new BossWaveClient("localhost", 28589);

            bwClient.Connect();

            // Purpose is to execute / test commands
            Console.WriteLine(bwClient.MakeEntity());
            Console.WriteLine(bwClient.Subscribe());
            Console.WriteLine(bwClient.Publish());
            Console.WriteLine(bwClient.List());
            Console.WriteLine(bwClient.MakeChain());
            Console.WriteLine(bwClient.MakeDoT());
            Console.WriteLine(bwClient.Query());

            bwClient.Dispose();
        }
    }
}
