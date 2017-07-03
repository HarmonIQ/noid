


using System;
using System.Linq;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using NoID.Database.Wrappers;

namespace NoID.Network.Services
{
    public static class Utilities
    {

        public static SessionQueue PatientToSessionQueue(Patient pt, string sparkReference, string localNoID, string status, string approval)
        {
            SessionQueue seq = null;
            try
            {
                if (pt != null)
                {
                    seq = new SessionQueue();
                    if (pt.Identifier.Count > 0)
                    {
                        foreach (Identifier id in pt.Identifier)
                        {
                            if (id.System.ToString().ToLower().Contains("session") == true)
                            {
                                seq._id = id.Value.ToString();
                            }
                        }
                    }
                    if (seq._id == null)
                    {
                        seq._id = sparkReference;
                    }
                    if (seq._id.Length == 0)
                    {
                        seq._id = sparkReference;
                    }
                    seq.LocalReference = localNoID;
                    seq.SparkReference = sparkReference;
                    seq.PatientStatus = status; //"new"
                    seq.ApprovalStatus = approval;// "pending";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return seq;
        }
    }
}