using System;
using System.Collections.Generic;
using System.Linq;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Model;
using NoID.Cryptographic.Hash;

namespace NoID
{
    public class PatientProfile
    {
        private readonly Uri fhirAddress = new Uri("http://localhost/spark/fhir");

        private string _languageSelected = "";
        private string _firstName = "";
        private string _lastName = "";
        private string _middleName = "";

        private Patient pt = new Patient();
        //TODO: load and use saltList from matching hubs
        private string tempSalt = "C560325F-6617-4FE9-BF30-04E67D591637";
        private List<string> saltList;

        HashWriter.ArgonParams argonParams = new HashWriter.ArgonParams(2, 8192, 4);

        public string Language_Hash
        {
            get { return HashWriter.Hash(_languageSelected, tempSalt, argonParams); }
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
    }
}
