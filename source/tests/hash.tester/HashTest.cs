// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using NoID.Cryptographic.Hash;

namespace NoID.Cryptographic.Hash.Test
{
    public class HashTest
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
