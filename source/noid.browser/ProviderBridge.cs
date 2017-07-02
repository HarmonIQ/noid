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
        private static readonly Uri PendingPatientsUri = new Uri(ConfigurationManager.AppSettings["PendingPatientsUri"].ToString());
        private static readonly string NoIDServiceName = ConfigurationManager.AppSettings["NoIDServiceName"].ToString();
        string _patientApprovalTable = "";
		int _patientApprovalTableRowCount = 0;
		string _approveDenySession = "";
		string _approveDenyAction = "";
        Uri _endPoint = null;

		private IList<PatientProfile> _patients;

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
            Authentication auth = SecurityUtilities.GetAuthentication(NoIDServiceName);
            HttpsClient client = new HttpsClient();
            PatientProfiles = client.RequestPendingQueue(PendingPatientsUri, auth);
            return PatientProfiles;
        }

        ~ProviderBridge() { }

		public bool getPatientDetailsProviderView(string sessionID)
		{
			try
			{
				errorDescription = "getPaitentDetailsProviderView with sessionID: " + sessionID;
			}
			catch (Exception ex)
			{
				errorDescription = ex.Message;
				return false;
			}
			return true;
		}
		public bool postApproveOrDeny(string sessionID, string action)
		{
			try
			{
				_approveDenySession = sessionID;
				_approveDenyAction = action;
			}
			catch (Exception ex)
			{
				errorDescription = ex.Message;
				return false;
			}
			return true;
		}

		public string CreatePatientApprovalQueue()
		{
			string htmlTable = "";
			int rowCount = 0;
			try
			{
				//_captureSite = FHIRUtilities.StringToCaptureSite(captureSite);
				//_laterality = FHIRUtilities.StringToLaterality(laterality);
				htmlTable += "<table class='table table-striped  table-bordered'>"
					 + "<thead><tr><th style='width: 170px' title='Time patient submitted NoID Enrollment or Identity request'>In Time</th><th style='width: 110px' title='New NoID Patient Type or Return NoID Patient Type'>Type</th><th style='width: 100px' title='Patient First Name'>First Name</th>"
					 + "<th style='width: 130px' title='Patient Last Name'>Last Name</th><th style='width: 100px' title='Patient Date of Birth'>DOB</th><th style='width: 85px' title='Approve Patient'>Approve</th><th style='width: 80px' title='Deny Patient'>Deny</th></tr></thead><tbody>";
				foreach (PatientProfile x in _patients)
				{
					htmlTable += "<tr>"
									+ "<td style='width: 170px'>" + Convert.ToDateTime(x.CheckinDateTime.ToString().Replace("-Z", "")).ToLocalTime() + " </td>"
									+ "<td style='width: 110px' title='" + x.NoIDStatus.ToString() + "'><div style='white-space:nowrap; text-overflow:ellipsis; overflow:hidden;'>" + x.NoIDStatus.ToString() + "</div></td>"
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
			catch (Exception ex)
			{
				errorDescription = ex.Message;
				return "Error";
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
