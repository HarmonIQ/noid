// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Collections.Generic;
using Hl7.Fhir.Model;
using NoID.Database.Wrappers;
using NoID.Utilities;

namespace NoID.Network.Services
{
    /// <summary>
    /// Summary description for AltMatchByDemographics
    /// </summary>

    public class AltMatchByDemographics : IHttpHandler
    {
        private readonly string NoIDMongoDBAddress = WebConfigurationManager.AppSettings["NoIDMongoDBAddress"].ToString();
        private readonly string SparkMongoDBAddress = WebConfigurationManager.AppSettings["SparkMongoDBAddress"].ToString();

        private string _responseText;
        private Patient _patient = null;
        private Exception _exception;

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            try
            {
                Stream httpStream = context.Request.InputStream;
                StreamReader httpStreamReader = new StreamReader(httpStream);
                Resource newResource = FHIRUtilities.StreamToFHIR(httpStreamReader);
                _patient = (Patient)newResource;

                //find all patient without fingerprints.  should be a small sample.
                //if found, return localid 
                //if not found, return "no match found"
                MongoDBWrapper dbwrapper = new MongoDBWrapper(NoIDMongoDBAddress, SparkMongoDBAddress);
                AlternateSearch altSearch = GetAlternateFromPatient(_patient);
                string localNoID = dbwrapper.AlternateSearch(altSearch);
                if (localNoID.ToLower().Contains("noid://") == false)
                {
                    dbwrapper.AddAlternateSearch(altSearch);
                    _responseText = "no match found";
                }
                else
                {
                    _responseText = localNoID;
                }
            }
            catch (Exception ex)
            {
                _responseText = "Error in AltMatchByDemographics::ProcessRequest: " + ex.Message;
                LogUtilities.LogEvent(_responseText);
            }
            context.Response.Write(_responseText);
            context.Response.End();
        }

        AlternateSearch GetAlternateFromPatient(Patient pt)
        {
            AlternateSearch alt = new AlternateSearch();
            try
            {
                // Gets the demographics from the patient FHIR resource class
                alt.LastName = pt.Name[0].Family.ToString();
                List<string> givenNames = pt.Name[0].Given.ToList();
                alt.FirstName = givenNames[0].ToString();
                if (givenNames.Count > 1)
                {
                    alt.MiddleName = givenNames[1].ToString();
                }
                alt.Gender = pt.Gender.ToString().Substring(0, 1).ToUpper();
                alt.BirthDate = pt.BirthDate;

                foreach (var id in pt.Identifier)
                {
                    if (id.System.ToLower().Contains("biometric") == true)
                    {
                        Extension extExceptionQA = id.Extension[0];
                        foreach (var ext in extExceptionQA.Extension)
                        {
                            if (ext.Url.ToLower().Contains("reason") == true)
                            {
                                alt.ExceptionReason = ext.Value.ToString();
                            }
                            else if (ext.Url.ToLower().Contains("question 1") == true)
                            {
                                alt.Question1 = ext.Value.ToString();
                            }
                            else if (ext.Url.ToLower().Contains("answer 1") == true)
                            {
                                alt.Answer1 = ext.Value.ToString();
                            }
                            else if (ext.Url.ToLower().Contains("question 2") == true)
                            {
                                alt.Question2 = ext.Value.ToString();
                            }
                            else if (ext.Url.ToLower().Contains("answer 2") == true)
                            {
                                alt.Answer1 = ext.Value.ToString();
                            }
                        }
                    }
                }

                // Gets the address information from the patient FHIR resource class
                if (pt.Address.Count > 0)
                {
                    List<string> addressLines = pt.Address[0].Line.ToList();
                    alt.StreetAddress = addressLines[0].ToString();
                    if (addressLines.Count > 1)
                    {
                        alt.StreetAddress2 = addressLines[1].ToString();
                    }

                    alt.City = pt.Address[0].City.ToString();
                    alt.State = pt.Address[0].State.ToString();
                    alt.PostalCode = pt.Address[0].PostalCode.ToString();
                    alt.Country = pt.Address[0].Country.ToString();
                }
                // Gets the contact information from the patient FHIR resource class
                if (pt.Contact.Count > 0)
                {
                    foreach (var contact in pt.Contact)
                    {
                        foreach (var telecom in contact.Telecom)
                        {
                            if (telecom.Use.ToString().ToLower() == "home")
                            {
                                if (telecom.System.ToString().ToLower() == "email")
                                {
                                    alt.Email = telecom.Value.ToString();
                                }
                                else if (telecom.System.ToString().ToLower() == "phone")
                                {
                                    alt.Phone = telecom.Value.ToString();
                                }
                            }
                            else if (telecom.Use.ToString().ToLower() == "work")
                            {
                                alt.Phone = telecom.Value.ToString();
                            }
                            else if (telecom.Use.ToString().ToLower() == "mobile")
                            {
                                alt.Phone = telecom.Value.ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return alt;
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