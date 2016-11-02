using System;
using System.Linq;
using System.Windows.Forms;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Model;
using System.Collections.Generic;
using Liphsoft.Crypto.Argon2;

namespace noid.test.webclient
{
    public partial class windowTestFHIR : Form
    {
        private readonly string SALT = "C560325F-6617-4FE9-BF30-04E67D591637";
        private readonly Uri fhirAddress = new Uri("http://localhost/spark/fhir");
        public windowTestFHIR()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var endpoint = fhirAddress;
            var client = new FhirClient(endpoint);
            Patient pt = new Patient();
            Identifier id = new Identifier();
            PasswordHasher PH = new PasswordHasher(2, 8192, 4); //time cost = 2, memory cost = 8192, lanes = 4

            id.Value = Convert.ToBase64String(PH.HashRaw(textMRN.Text, SALT));

            pt.Identifier  = new List<Identifier> { id };
            HumanName ptName = new HumanName();
            ptName.Given = new string[] {textFirstName.Text};
            ptName.Family = new string[] { textLastName.Text };
            pt.Name = new List<HumanName> { ptName };

            var newpatient = client.Create(pt);
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            var endpoint = fhirAddress;
            var client = new FhirClient(endpoint);

            var query = new string[] { "name=" + textFirstName.Text };
            var bundle = client.Search("Patient", query);

            labelRecordsFound.Text = "Records Found:" + bundle.Entry.Count();
        }
    }
}
