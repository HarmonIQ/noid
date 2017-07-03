// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Configuration;
using System.Collections.Generic;
using NoID.FHIR.Profile;
using NoID.Security;
using NoID.Network.Transport;
using NoID.Utilities;

namespace NoID.Browser
{
    /// <summary>Binds JavaScript in the CEF browser with this .NET framework class.
    /// <para> Used to handle the Provider UI and briges calls between CEF and .NET.</para>
    /// <para> Uses JavaScript naming because CEF renames them anyways.</para>
    /// <para> Use CEFBridge as a base class and contains</para>
    /// <para> organizationName, endPoint, serviceName, errorDescription, alertFunction</para>
    /// <seealso cref="CEFBridge"/>
    /// </summary>

    class ProviderBridge : CEFBridge
    {
        private static readonly string IdentityChallengeUri = ConfigurationManager.AppSettings["IdentityChallengeUri"].ToString();
        private static readonly string UpdatePendingStatusUri = ConfigurationManager.AppSettings["UpdatePendingStatusUri"].ToString();
        private static readonly string DevicePhysicalLocation = ConfigurationManager.AppSettings["DevicePhysicalLocation"].ToString();
        private static readonly string ClinicArea = ConfigurationManager.AppSettings["ClinicArea"].ToString();
        private static readonly Uri PendingPatientsUri = new Uri(ConfigurationManager.AppSettings["PendingPatientsUri"].ToString());
        private static readonly string NoIDServiceName = ConfigurationManager.AppSettings["NoIDServiceName"].ToString();
        
        string _patientApprovalTable = "";
		int _patientApprovalTableRowCount = 0;
		string _approveDenySession = "";
		string _approveDenyAction = "";
        Uri _endPoint = null;

        public delegate void ProviderEventHandler(object sender, string javaScriptToExecute);
        public event ProviderEventHandler JavaScriptAsync = delegate { };

        private IList<PatientProfile> _patients;

        public void ExecuteJavaScriptAsync(string javaScriptToExecute)
        {
            if (JavaScriptAsync != null)
                JavaScriptAsync(this, javaScriptToExecute);
        }

        public ProviderBridge(string organizationName, string serviceName) : base(organizationName, serviceName)
        {
            _patients = GetCheckinList();
			_patientApprovalTable = CreatePatientApprovalQueue();
		}

        public Uri endPoint
        {
            get { return _endPoint; }
            set { _endPoint = value; }
        }

        private static IList<PatientProfile> GetCheckinList()
        {
            IList<PatientProfile> PatientProfiles = null;
            Authentication auth;
            if (Utilities.Auth == null)
            {
                auth = SecurityUtilities.GetAuthentication(NoIDServiceName);
            }
            else
            {
                auth = Utilities.Auth;
            }
            HttpsClient client = new HttpsClient();
            PatientProfiles = client.RequestPendingQueue(PendingPatientsUri, auth);
            return PatientProfiles;
        }

        ~ProviderBridge() { }

		public string getPatientDetailsProviderView(string sessionID)
		{
			//string htmlTable = "";
			try
			{
				foreach (PatientProfile x in _patients)
				{
					if (x.SessionID.ToString() == sessionID)
					{
						string name = "";
						string patientAddress1 = "";
						string patientAddress2 = "";
						string patientCity = "";
						string patientState = "";
						string patientPostalCode = "";
						string dob = "";
						string gender = "";
						string phone = "";
						string email = "";
						string biometricStatus = "";					

						name = HandleNullString(x.FirstName) + (x.MiddleName.Length > 0 ? (" " + HandleNullString(x.MiddleName)) : "") + " " + HandleNullString(x.LastName);
						patientAddress1 = HandleNullString(x.StreetAddress);
						patientAddress2 = HandleNullString(x.StreetAddress2);
						patientCity = HandleNullString(x.City);
						patientState = HandleNullString(x.State);
						patientPostalCode = HandleNullString(x.PostalCode);
						dob = HandleNullString(x.BirthDate);
						gender = HandleNullString(x.Gender);
						phone = HandleNullString(x.PhoneCell);
						email = HandleNullString(x.EmailAddress);
						biometricStatus = HandleNullString(x.BiometricsCaptured);

      //                  htmlTable += "<table class='table' style='padding: 0; margin: 0; border-collapse: collapse;'><thead><tr>" +
						//			"<th style='width: 300px; padding:0; margin:0;text-align: center;'>Patient Details</th></tr></thead><tbody>";

						//htmlTable += "<tr>"
						//	+ "<td style='width: 300px; padding: 0; margin: 0; text-align: left; '><strong>Name: </strong>" + name + " </td>"
						//	+ "</tr><tr>"
						//	+ "<td style='width: 300px; padding: 0; margin: 0; text-align: left;'><strong>DOB: </strong>" + dob + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<strong>Gender: </strong>" + gender + "</td>"
						//	+ "</tr><tr>"
						//	+ "<td style='width: 300px; padding: 0; margin: 0; text-align: left;'><strong>Phone: </strong>" + phone + "</td>"
						//	+ "</tr><tr>"
						//	+ " <td style='width: 300px; padding: 0; margin: 0; text-align: left;'><strong>Email: </strong>" + email + "</td>"
						//	+ "</tr><tr>"
						//	+ "<td style='width: 300px; padding: 0; margin: 0; text-align: left;'><strong>Address: </strong><br />" + patientAddress1 + "<br />" + patientAddress2 + "<br />" + patientCity + ", " + patientState + " " + patientPostalCode + "</td>"
						//	+ "</tr><tr>"
						//	+ "<td style='width: 300px; padding:0; margin:0;text-align: left;'><strong>Biometric Status:</strong><br />" + biometricStatus + "</td>"
						//+ "</tr>"
						//;
						//htmlTable += "</tbody></table>";
                        ExecuteJavaScriptAsync("populatetPatientDetailsProviderView('" + sessionID + "', '" + name + "', '" + dob + "', '" + gender + "', '" + phone + "', '" + email + "', '" + patientAddress1 + "', '" + patientAddress2 + "', '" + patientCity + "', '" + patientState + "', '" + patientPostalCode + "', '" + biometricStatus + "');");
                    }
                }				
			}
			catch (Exception ex)
			{
				errorDescription = ex.Message;
				return "error";
			}
			return "";
		}

        string HandleNullString(string convert)
        {
            if (convert == null)
            {
                return "";
            }
            return convert;
        }

		public bool postApproveOrDeny(string sessionID, string action)
		{
			try
			{
				_approveDenySession = sessionID;
				_approveDenyAction = action;
                string response = UpdateStatusAction(sessionID, action);
                if (response.ToLower().Contains("error") == true)
                {
                    errorDescription = response;
                    return false;
                }
            }
			catch (Exception ex)
			{
				errorDescription = ex.Message;
				return false;
			}
			return true;
		}
		public bool postNewEditedDemographics
			(
			string sessionID,
			string name,
			string dob,
			string gender,
			string phone,
			string email,
			string patientAddress1,
			string patientAddress2,
			string patientCity,
			string patientState, 
			string patientPostalCode
		)
		{
			try
			{
				//stub to change demographics from provider view page
				//need to set document.getElementById('editDemographics').style.visibility = "visible";
				// which will enable edit button. click edit button to make field editable and click submit button to call this function
				// do demographic update below
				return true;
			}
			catch (Exception ex)
			{
				errorDescription = ex.Message;
				return false;
			}			
		}

        string UpdateStatusAction(string sessionID, string action)
        {
            string result = "";
            try
            {
                //action = approve, deny or hold.
                string computerName = SecurityUtilities.GetComputerName();
                string userName = SecurityUtilities.GetUserName();
                Authentication auth;
                if (Utilities.Auth == null)
                {
                    auth = SecurityUtilities.GetAuthentication(serviceName);
                }
                else
                {
                    auth = Utilities.Auth;
                }
                HttpsClient client = new HttpsClient();
                Uri fhirAddress = new Uri(UpdatePendingStatusUri);
                result = client.UpdatePendingStatus(fhirAddress, auth, sessionID, action, computerName, userName);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return result;
        }

		public string CreatePatientApprovalQueue()
		{
			string htmlTable = "";
			int rowCount = 0;
			try
			{
				//_captureSite = FHIRUtilities.StringToCaptureSite(captureSite);
				//_laterality = FHIRUtilities.StringToLaterality(laterality);
				if (_patients != null)
				{
					htmlTable += "<table class='table table-striped  table-bordered'>"
						 + "<thead><tr><th style='width: 170px' title='Time patient submitted NoID Enrollment or Identity request'>In Time</th><th style='width: 110px' title='New NoID Patient Type or Return NoID Patient Type'>Type</th><th style='width: 100px' title='Patient First Name'>First Name</th>"
						 + "<th style='width: 130px' title='Patient Last Name'>Last Name</th><th style='width: 100px' title='Patient Date of Birth'>DOB</th><th style='width: 85px' title='Approve Patient'>Approve</th><th style='width: 80px' title='Deny Patient'>Deny</th></tr></thead><tbody>";
					foreach (PatientProfile x in _patients)
					{
						htmlTable += "<tr>"
										+ "<td style='width: 170px'>" + Convert.ToDateTime(x.CheckinDateTime.ToString().Replace("-Z", "")).ToLocalTime() + " </td>"
										+ "<td style='width: 110px' title='" + x.NoIDType.ToString() + "'><div style='white-space:nowrap; text-overflow:ellipsis; overflow:hidden;'>" + x.NoIDStatus.ToString() + "</div></td>"
										+ "<td style='width: 100px' title='" + x.FirstName.ToString() + "'><div style='white-space:nowrap; text-overflow:ellipsis; overflow:hidden;' onclick='showtPatientDetailsProviderView(" + (char)34 + x.SessionID.ToString() + (char)34 + ");'><u>" + x.FirstName.ToString() + "</u></div></td>"
										+ "<td style='width: 130px' title='" + x.LastName.ToString() + "'><div style='white-space:nowrap; text-overflow:ellipsis; overflow:hidden;' onclick='showtPatientDetailsProviderView(" + (char)34 + x.SessionID.ToString() + (char)34 + ");'><u>" + x.LastName.ToString() + "</u></div></td>"
										+ "<td style='width: 100px'>" + x.BirthDate.ToString() + "</td>"
										+ "<td style='width: 85px'>" + "<a href='javascript:void(0);' onclick='approvePatient(" + (char)34 + x.SessionID.ToString() + (char)34 + "," + (char)34 + "Approve" + (char)34 + ")' title='Click Approve to approve " + x.FirstName.ToString() + " " + x.LastName.ToString() + "'>Approve</a></td>"
										+ "<td style='width: 80px'>" + "<a href='javascript:void(0);' onclick='approvePatient(" + (char)34 + x.SessionID.ToString() + (char)34 + "," + (char)34 + "Deny" + (char)34 + ")' title='Click Deny to deny " + x.FirstName.ToString() + " " + x.LastName.ToString() + "'>Deny</a></td>"
									+ "</tr>"
									;
						rowCount++;
					}
					htmlTable += "</tbody></table>";
					_patientApprovalTableRowCount = rowCount;
					errorDescription = "";
                }
				else
				{
					htmlTable = "No patients in queue.";
				}
			}
			catch (Exception ex)
			{
				errorDescription = ex.Message;
				return "Error in ProviderBridge::CreatePatientApprovalQueue: " + errorDescription;
			}
			return htmlTable;
		}
		
        public IList<PatientProfile> patients
        {
            get { return _patients; }
        }

		public string patientApprovalTable
		{
			get { return _patientApprovalTable; }
			set { _patientApprovalTable = value; }
		}

		public int patientApprovalTableRowCount
		{
			get { return _patientApprovalTableRowCount; }
			set { _patientApprovalTableRowCount = value; }
		}
		public string approveDenySession
		{
			get { return _approveDenySession; }
			set { _approveDenySession = value; }
		}
		public string approveDenyAction
		{
			get { return _approveDenyAction; }
			set { _approveDenyAction = value; }
		}
	}
}
