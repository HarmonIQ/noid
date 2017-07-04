// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Web;
using System.Web.Configuration;
using NoID.Database.Wrappers;
using NoID.Match.Database.FingerPrint;

namespace NoID.Network.Services
{
    /// <summary>
    /// WARNING, running this web service will delete all NoID databases.
    /// </summary>

    public class PurgeAllDatabases : IHttpHandler
    {
        private static readonly string NoIDMongoDBAddress = WebConfigurationManager.AppSettings["NoIDMongoDBAddress"].ToString();
        private static readonly string SparkMongoDBAddress = WebConfigurationManager.AppSettings["SparkMongoDBAddress"].ToString();
        private static readonly string DestroyKey = WebConfigurationManager.AppSettings["DestroyKey"].ToString();
        private string DatabaseLocation = WebConfigurationManager.AppSettings["DatabaseLocation"];
        private string BackupLocation = WebConfigurationManager.AppSettings["BackupLocation"];

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string purgeResult = "";
            string destroyKey = "";
            try
            {
                foreach (String key in context.Request.QueryString.AllKeys)
                {
                    if (key == "destroykey")
                    {
                        destroyKey = context.Request.QueryString[key];
                        break;
                    }
                }
                if (destroyKey == DestroyKey)
                {
                    MongoDBWrapper dbwrapper = new MongoDBWrapper(NoIDMongoDBAddress, SparkMongoDBAddress);
                    if (dbwrapper.DeleteMongoDBs() == true)
                    {
                        FingerPrintMatchDatabase dbMinutia = new FingerPrintMatchDatabase(DatabaseLocation, BackupLocation);
                        if (dbMinutia.DeleteMatchDatabase())
                        {
                            purgeResult = "Successful.";
                        }
                        else
                        {
                            purgeResult = "Error in PurgeAllDatabases::ProcessRequest: Unable to delete all databases.";
                        }
                    }
                }
                else
                {
                    //TODO: log this event as an invalid attempt due to mismatched keys.
                }
            }
            catch (Exception ex)
            {
                purgeResult = "Error in PurgeAllDatabases::ProcessRequest: " + ex.Message;
            }
            context.Response.Write(purgeResult);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}