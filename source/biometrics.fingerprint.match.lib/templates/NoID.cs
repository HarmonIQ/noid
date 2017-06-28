// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;

namespace SourceAFIS.Templates
{
    [Serializable]
    public class NoID
    {
        public string SessionID = "";
        public string LocalNoID = "";
        public string RemoteNoID = "";
        public DateTime SessionStartDateTime = DateTime.UtcNow;
    }
}
