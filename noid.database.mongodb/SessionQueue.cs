// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NoID.Database.Wrappers
{

    /// <summary>
    /// Class used to store NoID session queue information.
    /// </summary>

    public class SessionQueue
    {
        [JsonConstructor]
        public SessionQueue()
        {

        }

        ~SessionQueue() { }

        [JsonProperty("_id")]
        public string _id { get; set; }

        [JsonProperty("SparkReference")]
        public string SparkReference { get; set; }

        [JsonProperty("LocalReference")]
        public string LocalReference { get; set; }

        [JsonProperty("RemoteHubReference")]
        public string RemoteHubReference { get; set; }

        [JsonProperty("PatientStatusType")]
        public string PatientStatusType { get; set; } //new, returning

        [JsonProperty("PatientStatus")]
        public string PatientStatus { get; set; } //pending, denied, approved, hold.

        [JsonProperty("ClinicArea")]
        public string ClinicArea { get; set; }

        [JsonProperty("SessionComputerName")]
        public string SessionComputerName { get; set; }

        [JsonProperty("ReviewUser")]
        public string ReviewUser { get; set; }

        [JsonProperty("SubmitDate")]
        public DateTimeOffset SubmitDate { get; set; }

        [JsonProperty("PatientBeginDate")]
        public DateTimeOffset PatientBeginDate { get; set; }

        [JsonProperty("AcceptDenyDate")]
        public DateTimeOffset AcceptDenyDate { get; set; }

        public string Serialize()
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;
            return JsonConvert.SerializeObject(this);
        }

        public SessionQueue Deserialize(string json)
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;
            return JsonConvert.DeserializeObject<SessionQueue>(json);
        }
    }
}
