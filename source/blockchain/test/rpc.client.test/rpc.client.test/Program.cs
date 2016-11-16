using System;
using Blockchain.RPC;

namespace noid.blockchain.client.test
{
    class Program
    {
        static void Main(string[] args)
        {
            string json = "";
            try
            {
                json = RPCClient.CallBlockchainRPC(Chain.Sequence, SequenceRpcMethods.name_new, null);
                Console.WriteLine(json);
            }
            catch (Exception exception)
            {
                Console.WriteLine("There was a problem sending the request to the wallet. Error Message: " + exception.Message + "\n Stack Trace: " + exception.StackTrace);
            }
            Console.ReadLine();
        }
    }
}
