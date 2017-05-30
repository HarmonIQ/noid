using System;
using System.Collections.Generic;
using System.Linq;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Model;

namespace NoID
{
    public class PatientProfile
    {
        private string _languageSelected = "";
        private Patient pt = new Patient();

        public string LanguageHash
        {
            get { return _languageSelected; }
        }

    }
}
