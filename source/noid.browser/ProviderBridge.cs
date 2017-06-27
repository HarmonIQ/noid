// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
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
		string _patientApprovalTable = "";
		int _patientApprovalTableRowCount = 0;
		string _approveDenySession = "";
		string _approveDenyAction = "";

		private IList<PatientProfile> _patients;

        public ProviderBridge(string organizationName, Uri endPoint, string serviceName) : base(organizationName, endPoint, serviceName)
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
				htmlTable += "<table class='table table-striped  table-bordered'><thead><tr><th>In Time</th><th>NoID Type</th><th>First Name</th>" +
							"<th>Last Name</th><th>DOB</th><th>Approve</th><th>Deny</th></tr></thead><tbody>";
				foreach (PatientProfile x in _patients)
				{
					htmlTable += "<tr>"
									+ "<td>" + Convert.ToDateTime(x.CheckinDateTime.ToString().Replace("-Z", "")).ToLocalTime() + " </td>"
									+ "<td>" + x.NoIDStatus.ToString() + "</td>"
									+ "<td>" + x.FirstName.ToString() + "</td>"
									+ "<td>" + x.LastName.ToString() + "</td>"
									+ "<td>" + x.BirthDate.ToString() + "</td>"
									+ "<td>" + "<a href='javascript:void(0);' onclick='approvePatient(" + (char)34 + x.SessionID.ToString() + (char)34 + "," + (char)34 + "Approve" + (char)34 + ")'>Approve</a></td>"
									+ "<td>" + "<a href='javascript:void(0);' onclick='approvePatient(" + (char)34 + x.SessionID.ToString() + (char)34 + "," + (char)34 + "Deny" + (char)34 + ")'>Deny</a></td>"
								+ "</tr>";
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
