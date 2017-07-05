﻿// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Web;
using System.Web.Configuration;
using NoID.Database.Wrappers;

namespace NoID.Network.Services
{
    /// <summary>
    /// Summary description for UpdatePendingStatus
    /// </summary>
    public class UpdatePendingStatus : IHttpHandler
    {
        private static readonly string NoIDMongoDBAddress = WebConfigurationManager.AppSettings["NoIDMongoDBAddress"].ToString();
        private static readonly string SparkMongoDBAddress = WebConfigurationManager.AppSettings["SparkMongoDBAddress"].ToString();

        private string _sessionID = "";
        private string _action = "";
        private string _computerName = "";
        private string _userName = "";

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            try
            {
                foreach (String key in context.Request.QueryString.AllKeys)
                {
                    switch (key)
                    {
                        case "sessionid":
                            _sessionID = context.Request.QueryString[key];
                            break;
                        case "action":
                            _action = context.Request.QueryString[key];
                            break;
                        case "computername":
                            _computerName = context.Request.QueryString[key];
                            break;
                        case "username":
                            _userName = context.Request.QueryString[key];
                            break;
                    }
                }

                MongoDBWrapper dbwrapper = new MongoDBWrapper(NoIDMongoDBAddress, SparkMongoDBAddress);
                if (dbwrapper.UpdateSessionQueueRecord(_sessionID, _action, _userName, _computerName) == false)
                {
                    if (dbwrapper.Exceptions.Count > 0)
                    {
                        string errorMessage = dbwrapper.Exceptions[0].Message;
                        context.Response.Write("UpdatePendingStatus::ProcessRequest Error: " + errorMessage);
                    }
                    else
                    {
                        context.Response.Write("UpdatePendingStatus::ProcessRequest Error: Could not find sessionID " + _sessionID + ".");
                    }
                }
                else
                {
                    context.Response.Write("Successfully updated the pending status.");
                }
            }
            catch (Exception ex)
            {
                context.Response.Write("UpdatePendingStatus::ProcessRequest Error: " + ex.Message);
            }
            context.Response.End();
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