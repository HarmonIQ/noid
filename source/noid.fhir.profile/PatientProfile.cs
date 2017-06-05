using System;
using System.Collections.Generic;
using System.Linq;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Model;
using NoID.Cryptographic.Hash;

namespace NoID.Message
{
    public class PatientProfile
    {
        private readonly Uri fhirAddress = new Uri("http://localhost/spark/fhir");

        private string _language = "";
        private string _firstName = "";
        private string _lastName = "";
        private string _middleName = "";
        private string _gender = "";
        private string _birthDate = "";
        private string _streetAddress1 = "";
        private string _streetAddress2 = "";
        private string _city = "";
        private string _state = "";
        private string _postalCode = "";
        private string _phoneHome = "";
        private string _phoneCell = "";
        private string _phoneWork = "";
        private string _emailAddress = "";
        private bool   _twinIndicator = false;


        private Patient pt = new Patient();

        //TODO: load and use saltList from matching hubs
        private string tempSalt = "C560325F-6617-4FE9-BF30-04E67D591637";
        private List<string> saltList;

        HashWriter.ArgonParams argonParams = new HashWriter.ArgonParams(2, 8192, 4);

        public string Language_Hash
        {
            get { return HashWriter.Hash(_language, tempSalt, argonParams); }
        }

        public string FirstName_Hash
        {
            get { return HashWriter.Hash(_firstName, tempSalt, argonParams); }
        }
 
        public string LastName_Hash
        {
            get { return HashWriter.Hash(_lastName, tempSalt, argonParams); }
        }

        public string MiddleName_Hash
        {
            get { return HashWriter.Hash(_middleName, tempSalt, argonParams); }
        }

        public bool SendEnrollmentProfile()
        {
            var endpoint = fhirAddress;
            var client = new FhirClient(endpoint);
            Patient pt = new Patient();
            Identifier id = new Identifier();

            pt.Identifier = new List<Identifier> { id };
            HumanName ptName = new HumanName();
            ptName.Given = new string[] { FirstName_Hash };
            //ptName.Family = new string[] { LastName_Hash };
            pt.Name = new List<HumanName> { ptName };

            var newpatient = client.Create(pt);

            return false;

        }

        public string TestEnrollmentSave()
        {
            var endpoint = fhirAddress;
            var client = new FhirClient(endpoint);

            var query = new string[] { "name=" + FirstName_Hash };
            var bundle = client.Search("Patient", query);

            return "Records Found:" + bundle.Entry.Count();
        }
    }
}
