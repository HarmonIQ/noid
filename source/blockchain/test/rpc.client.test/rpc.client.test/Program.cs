using System;
using Blockchain.RPC;

namespace noid.blockchain.client.test
{
    class Program
    {
        static void Main(string[] args)
        {
            string json = "";

            json = RPCClient.CallBlockchainRPC(Chain.Sequence, SequenceRpcMethods.getinfo, null);
            Console.WriteLine(json);
            Console.ReadLine();
        }
    }
}
