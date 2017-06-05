using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoID.Cryptographic.Hash;

namespace hash.tester
{
    class test
    {
        static void Main(string[] args)
        {
            HashWriter.ArgonParams argonParams = new HashWriter.ArgonParams(2, 8192, 4);
            Console.WriteLine("hash test 1, salt = C560325F-6617-4FE9-BF30-04E67D591637");
            Console.WriteLine(HashWriter.Hash("hash test 1", "C560325F-6617-4FE9-BF30-04E67D591637", argonParams));
            Console.WriteLine("hash test 2, salt = C560325F-6617-4FE9-BF30-04E67D591637");
            Console.WriteLine(HashWriter.Hash("hash test 2", "C560325F-6617-4FE9-BF30-04E67D591637", argonParams));
            Console.WriteLine("hash test 1, salt = dsdadasd25F-6s17-4FE9-BF30-04E67D591637");
            Console.WriteLine(HashWriter.Hash("hash test 1", "dsdadasd25F-6s17-4FE9-BF30-04E67D591637", argonParams));
            Console.ReadKey();
        }
    }
}
