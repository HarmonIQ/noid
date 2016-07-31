// Copyright (c) 2016 NoID Developers
// Distributed under the MIT/X11 software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

namespace NoID.Base.Data
{
    public interface IEntity
    {
        string Id { get; set; }

        string Type { get; set; }

        ulong Cas { get; set; }
    }
}