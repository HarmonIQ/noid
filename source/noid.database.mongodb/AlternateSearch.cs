// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NoID.Database.Wrappers
{

    /// <summary>
    /// Class used as an alternate search for patients that can't provider biometrics.
    /// Stored in MongoDB
    /// </summary>

    public class AlternateSearch
    {
        [JsonConstructor]
        public AlternateSearch()
        {
        }

        ~AlternateSearch() { }

        [JsonProperty("_id")]
        public string _id { get; set; }

        [JsonProperty("LocalReference")]
        public string LocalReference { get; set; }

        [JsonProperty("Language")]
        public string Language { get; set; }

        [JsonProperty("FirstName")]
        public string FirstName { get; set; }

        [JsonProperty("MiddleName")]
        public string MiddleName { get; set; }

        [JsonProperty("LastName")]
        public string LastName { get; set; }

        [JsonProperty("Gender")]
        public string Gender { get; set; }

        [JsonProperty("BirthDate")]
        public string BirthDate { get; set; }

        [JsonProperty("StreetAddress")]
        public string StreetAddress { get; set; }

        [JsonProperty("StreetAddress2")]
        public string StreetAddress2 { get; set; }

        [JsonProperty("City")]
        public string City { get; set; }

        [JsonProperty("State")]
        public string State { get; set; }

        [JsonProperty("PostalCode")]
        public string PostalCode { get; set; }
        
        [JsonProperty("Country")]
        public string Country { get; set; }

        [JsonProperty("Email")]
        public string Email { get; set; }

        [JsonProperty("Phone")]
        public string Phone { get; set; }

        [JsonProperty("ExceptionReason")]
        public string ExceptionReason { get; set; }

        [JsonProperty("Question1")]
        public string Question1 { get; set; }

        [JsonProperty("Answer1")]
        public string Answer1 { get; set; }

        [JsonProperty("Question2")]
        public string Question2 { get; set; }

        [JsonProperty("Answer2")]
        public string Answer2 { get; set; }
    }
}
