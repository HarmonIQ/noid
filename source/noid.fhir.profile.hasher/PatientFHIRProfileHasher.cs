// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using NoID.Cryptographic.Hash;

namespace NoID.FHIR.Profile.Hasher
{
    public class PatientFHIRProfileHasher : PatientFHIRProfile
    {
        // Argon2 Hash parameters. 
        // Currently a protocol level setting for now but could be a setting defined per healthcare and/or match hub.
        // TODO: increase the difficulty of the hash parameters after the prototype.
        const int ARGON2_TIME_COST = 1;
        const int ARGON2_MEMORY_COST = 250;
        const int ARGON2_PARALLEL_LANES = 4;
        HashWriter.ArgonParams argonParams = new HashWriter.ArgonParams(ARGON2_TIME_COST, ARGON2_MEMORY_COST, ARGON2_PARALLEL_LANES);

        //TODO: load and use saltList from matching hubs
        public string hashSalt = "C560325F";

        public PatientFHIRProfileHasher(string organizationName, string noidStatus) : base(organizationName, noidStatus)
        {

        }

        public string PatientCertificateIDHash
        {
            get { return HashWriter.Hash(LocalNoID, hashSalt, argonParams); }
        }

        public string LanguageHash
        {
            get { return HashWriter.Hash(Language, hashSalt, argonParams); }
        }

        public string FirstNameHash
        {
            get { return HashWriter.Hash(FirstName, hashSalt, argonParams); }
        }

        public string LastNameHash
        {
            get { return HashWriter.Hash(LastName, hashSalt, argonParams); }
        }

        public string MiddleNameHash
        {
            get { return HashWriter.Hash(MiddleName, hashSalt, argonParams); }
        }

        public string GenderHash
        {
            get { return HashWriter.Hash(Gender.ToString(), hashSalt, argonParams); }
        }

        public string BirthDateHash
        {
            get { return HashWriter.Hash(BirthDate, hashSalt, argonParams); }
        }

        public string StreetAddressHash
        {
            get { return HashWriter.Hash(StreetAddress, hashSalt, argonParams); }
        }

        public string StreetAddress2Hash
        {
            get { return HashWriter.Hash(StreetAddress2, hashSalt, argonParams); }
        }

        public string CityHash
        {
            get { return HashWriter.Hash(City, hashSalt, argonParams); }
        }

        public string StateHash
        {
            get { return HashWriter.Hash(State, hashSalt, argonParams); }
        }
        public string PostalCodeHash
        {
            get { return HashWriter.Hash(PostalCode, hashSalt, argonParams); }
        }

        public string CountryHash
        {
            get { return HashWriter.Hash(Country, hashSalt, argonParams); }
        }

        public string PhoneHomeHash
        {
            get { return HashWriter.Hash(PhoneHome, hashSalt, argonParams); }
        }
        public string PhoneCellHash
        {
            get { return HashWriter.Hash(PhoneCell, hashSalt, argonParams); }
        }
        public string PhoneWorkHash
        {
            get { return HashWriter.Hash(PhoneWork, hashSalt, argonParams); }
        }

        public string EmailAddressHash
        {
            get { return HashWriter.Hash(EmailAddress, hashSalt, argonParams); }
        }

        public string MultipleBirthHash
        {
            get { return HashWriter.Hash(MultipleBirth.ToString(), hashSalt, argonParams); }
        }
    }
}
