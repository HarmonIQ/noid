// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Configuration;
using System.Collections.Generic;
using NoID.FHIR.Profile;

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
        string _patientApprovalTable = "";
		int _patientApprovalTableRowCount = 0;
		string _approveDenySession = "";
		string _approveDenyAction = "";
        

		private IList<PatientProfile> _patients;

        public ProviderBridge(string organizationName, string serviceName) : base(organizationName, PendingPatientsUri, serviceName)
        {
            _patients = TestPatientList.GetTestPatients(organizationName);

			_patientApprovalTable = CreatePatientApprovalQueue();

		}

		 ~ProviderBridge() { }

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
					 + "<thead><tr><th style='width: 175px'>In Time</th><th style='width: 100px'>NoID Type</th><th style='width: 100px'>First Name</th>"
					 + "<th style='width: 110px'>Last Name</th><th style='width: 100px'>DOB</th><th style='width: 100px'>Approve</th><th style='width: 80px'>Deny</th></tr></thead><tbody>";
				foreach (PatientProfile x in _patients)
				{
					htmlTable += "<tr>"
									+ "<td style='width: 175px'>" + Convert.ToDateTime(x.CheckinDateTime.ToString().Replace("-Z", "")).ToLocalTime() + " </td>"
									+ "<td style='width: 100px'>" + x.NoIDStatus.ToString() + "</td>"
									+ "<td style='width: 100px'>" + x.FirstName.ToString() + "</td>"
									+ "<td style='width: 110px'>" + x.LastName.ToString() + "</td>"
									+ "<td style='width: 100px'>" + x.BirthDate.ToString() + "</td>"
									+ "<td style='width: 100px'>" + "<a href='javascript:void(0);' onclick='approvePatient(" + (char)34 + x.SessionID.ToString() + (char)34 + "," + (char)34 + "Approve" + (char)34 + ")'>Approve</a></td>"
									+ "<td style='width: 80px'>" + "<a href='javascript:void(0);' onclick='approvePatient(" + (char)34 + x.SessionID.ToString() + (char)34 + "," + (char)34 + "Deny" + (char)34 + ")'>Deny</a></td>"
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
