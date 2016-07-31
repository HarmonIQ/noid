// Copyright (c) 2016 NoID Developers
// Distributed under the MIT/X11 software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using Newtonsoft.Json;

namespace NoID.Base
{
    public abstract class EntityBase : IEntity
    {
        private static string _typeName;

        protected EntityBase()
        {
            if (_typeName == null)
            {
                _typeName = GetType().Name;
            }
            Type = _typeName;
        }

        [JsonProperty("id")]
        public string Id { get; set; }

        public string Type { get; set; }

        public ulong Cas { get; set; }

        public DateTime Updated { get; set; }
    }
}