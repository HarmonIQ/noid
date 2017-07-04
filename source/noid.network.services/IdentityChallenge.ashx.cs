
using System;
using System.Web;
using System.Web.Configuration;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using NoID.Utilities;
using NoID.Database.Wrappers;

namespace NoID.Network.Services
{
    /// <summary>
    /// Summary description for IdentityChallenge
    /// </summary>
    public class IdentityChallenge : IHttpHandler
    {
        private readonly Uri sparkEndpointAddress = new Uri(StringUtilities.RemoveTrailingBackSlash(WebConfigurationManager.AppSettings["SparkEndpointAddress"].ToString()));
        private static readonly string NoIDMongoDBAddress = WebConfigurationManager.AppSettings["NoIDMongoDBAddress"].ToString();
        private static readonly string SparkMongoDBAddress = WebConfigurationManager.AppSettings["SparkMongoDBAddress"].ToString();
        private static readonly string OrganizationName = WebConfigurationManager.AppSettings["OrganizationName"].ToString();
        private static readonly string DomainName = WebConfigurationManager.AppSettings["DomainName"].ToString();
        private static readonly string NodeSalt = WebConfigurationManager.AppSettings["NodeSalt"].ToString();

        private string _localNoID;
        private string _confirmFieldName;
        private string _confirmReponse;
        private string _computerName;
        private string _clinicArea;

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            try
            {
                foreach (String key in context.Request.QueryString.AllKeys)
                {
                    switch (key.ToLower())
                    {
                        case "localnoid":
                            _localNoID = context.Request.QueryString[key];
                            break;
                        case "fieldname":
                            _confirmFieldName = context.Request.QueryString[key];
                            break;
                        case "confirmreponse":
                            _confirmReponse = context.Request.QueryString[key];
                            break;
                        case "computername":
                            _computerName = context.Request.QueryString[key];
                            break;
                        case "clinicarea":
                            _clinicArea = context.Request.QueryString[key];
                            break;
                    }
                }
                MongoDBWrapper dbwrapper = new MongoDBWrapper(NoIDMongoDBAddress, SparkMongoDBAddress);
                FhirClient client = new FhirClient(sparkEndpointAddress);
                string sparkReference = dbwrapper.GetSparkID(_localNoID);
                string sparkAddress = sparkEndpointAddress.ToString() + "/Patient/" + sparkReference;
                Patient pendingPatient = (Patient)client.Get(sparkAddress);
                if (pendingPatient != null)
                {
                    if (_confirmFieldName == "birthdate")
                    {
                        if (pendingPatient.BirthDate != null && _confirmReponse == pendingPatient.BirthDate)
                        {
                            SessionQueue seq = Utilities.PatientToSessionQueue(pendingPatient, sparkReference, _localNoID, "return", "pending");
                            seq.SubmitDate = DateTime.UtcNow;
                            seq._id = StringUtilities.SHA256(DomainName + Guid.NewGuid().ToString() + NodeSalt);
                            seq.SessionComputerName = _computerName;
                            seq.ClinicArea = _clinicArea;
                            dbwrapper.AddPendingPatient(seq);
                            context.Response.Write("yes");
                        }
                        else
                        {
                            context.Response.Write("no");
                        }
                    }
                    else if (_confirmFieldName == "lastname")
                    {
                        //TODO: implement lastname, use metaphone or just accept exact matches?
                        context.Response.Write("Error occurred.  " + _confirmFieldName + " is not implemented yet!");
                    }
                    else if (_confirmFieldName == "firstname")
                    {
                        //TODO: implement firstname, use root or just accept exact matches?
                        context.Response.Write("Error occurred.  " + _confirmFieldName + " is not implemented yet!");
                    }
                    else if (_confirmFieldName == "failedchallenge")
                    {
                        SessionQueue seq = Utilities.PatientToSessionQueue(pendingPatient, sparkReference, _localNoID, "return**", "pending");
                        seq.SubmitDate = DateTime.UtcNow;
                        seq._id = StringUtilities.SHA256(DomainName + Guid.NewGuid().ToString() + NodeSalt);
                        seq.SessionComputerName = _computerName;
                        seq.ClinicArea = _clinicArea;
                        dbwrapper.AddPendingPatient(seq);
                        context.Response.Write("yes");
                    }
                }
            }
            catch (Exception ex)
            {
                context.Response.Write("no. Error occured for LocalNoID = " + _localNoID + ".  UpdatePendingStatus::ProcessRequest: " + ex.Message);
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