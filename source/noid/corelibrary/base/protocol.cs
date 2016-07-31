// Copyright (c) 2016 NoID Developers
// Distributed under the MIT/X11 software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using NoID.Base.Data;
using Couchbase.Core;
using Couchbase.Views;

namespace NoID.Base.Protocol
{
    // Gets & Sets protocol bucket data
    public class MatchWeights : EntityBase
    {
        [JsonProperty("matchweightID")]
        public string MatchWeightID { get; set; }
        public string DataPointName { get; set; }
        public string DataConstruct { get; set; }
        public string HashCode { get; set; }
        public decimal ScoreWeight { get; set; }
    }

    public class HashAlgorithms : EntityBase
    {
        [JsonProperty("hashalgorithmID")]
        public string HashAlgorithmID { get; set; }
        public string Algorithm { get; set; }
        public string HashSalt { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpirationDate { get; set; }
    }

    public class RootNames : EntityBase
    {
        [JsonProperty("rootnameID")]
        public string RootNameID { get; set; }
        public string Name { get; set; }
        public string RootName { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpirationDate { get; set; }
    }

    public class Protocol
    {
        public Protocol ()
        {
            //TODO (Amir): Get and load protocol JSON documents from Coachbase Server Bucket
            // get matchWeights.  If blank, set matchWeights with repo default JSON document. Check if default is used or not.
            // get hashAlgorithms If blank, set hashAlgorithms with repo default JSON document. Check if default is used or not.
            // get hashAlgorithms If blank, set hashAlgorithms with repo default JSON document. Check if default is used or not.
        }

        public RootNames rootNames;
        public MatchWeights matchWeights;
        public HashAlgorithms hashAlgorithms;

        ~Protocol ()
        {
            //TODO (Amir): Make sure all JSON & Coachbase objects are closed and disposed
        }
    }

    public class ProtocolMatchWeights : Repository<MatchWeights>
    {
        public ProtocolMatchWeights(IBucket bucket)
            : base(bucket)
        {
        }

        public IEnumerable<ViewRow<MatchWeights>> SelectAllHashAlgorithms(int index, int limit)
        {
            var query = Bucket.CreateQuery("match_weights", "all_match_weights")
                .Skip(index)
                .Limit(limit);

            var results = Bucket.Query<MatchWeights>(query);
            if (!results.Success)
            {
                var message = results.Error;
                throw new ViewRequestException(message, results.StatusCode);
            }
            return results.Rows;
        }
    }

    public class ProtocolHashAlgorithms : Repository<HashAlgorithms>
    {
        public ProtocolHashAlgorithms(IBucket bucket)
            : base(bucket)
        {
        }

        public IEnumerable<ViewRow<HashAlgorithms>> SelectAllHashAlgorithms(int index, int limit)
        {
            var query = Bucket.CreateQuery("hash_algorithms", "all_hash_algorithms")
                .Skip(index)
                .Limit(limit);

            var results = Bucket.Query<HashAlgorithms>(query);
            if (!results.Success)
            {
                var message = results.Error;
                throw new ViewRequestException(message, results.StatusCode);
            }
            return results.Rows;
        }
    }

    public class ProtocolRootNames : Repository<RootNames>
    {
        public ProtocolRootNames(IBucket bucket)
            : base(bucket)
        {
        }

        public IEnumerable<ViewRow<RootNames>> SelectAllRootName(int index, int limit)
        {
            var query = Bucket.CreateQuery("root_names", "all_root_names")
                .Skip(index)
                .Limit(limit);

            var results = Bucket.Query<RootNames>(query);
            if (!results.Success)
            {
                var message = results.Error;
                throw new ViewRequestException(message, results.StatusCode);
            }
            return results.Rows;
        }
    }
}