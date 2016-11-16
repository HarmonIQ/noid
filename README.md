### NoID Patient Identity and Record Location Management Protocol

#### **NoID Version 0.16.11.15*
### Project Status: ![Backlog](https://badge.waffle.io/harmoniq/noid.png?label=Ready&title=Backlog) [Waffle Board](https://waffle.io/HarmonIQ/noid "Waffle.io Project Management Board")

### About NoID
A global patient identifier is dangerous. A GPI, if compromised, could provide access to Protected Health Information. We believe that a better approach is to create a hashed patient resource, based on the current HL7 FHIR protocol, which contains patient demographic and biometric data, and which in its entirety acts as the means for patient identification. The methodology to achieve this goal uses current block chain and public ledger technology to manage participants, encryption and hashing to protect communication and patient data, HL7 FHIR protocols for interoperability, and hardware such as smartphones to gather and validate biometric data. This is the NoID protocol.
* NoID provides simple and fast patient enrollment
* Enrollment occurs at Healthcare Organization Nodes 
* Nodes communicate with Patient Hubs which provide identification verification
* All communication between Nodes and Hubs is secure 
* Protocol is not dependent on age, sex, nationality, etc
* NoID Profile is shared data set used to store hashed patient data. NoID Profile is based on HL7 FHIR Patient Resource
* Biometrics are used to minimize errors in enrollment
* Data is meaningless outside of NoID. However, if a NoID profile was compromised, it can be changed by applying a modified hash algorithm
* The setup of NoID functionally at a node is highly automated 
* The protocol handles exceptions to typical biometric gathering by providing alternative pathways to populate NoID Profiles 
* Hubs provid 
64
or  specifications for fingerprint minutiae data.e a secure web interface which allows for a patient to gather metadata audit information. This web interface also provides alternative contact information (fax, phone, etc) for the patient’s nodes. No PHI is available here
* Adds 3-6 minutes to existing patient registration processes (biometric gathering)
* NoID provides fast, accurate identification
* Reduces transcription errors by mandating biometric input devices and recommending OCR technology 
* A single biometric is enough to positively identify an existing patient
* If a match is found, the Patient Hub returns a FHIR location resource to the calling node with pointers to other nodes which have a copy of that profile. The calling node then requests a FHIR Patient Resource to update their systems. Any new data added to the NoID profile triggers a cascade of node-to-node updates to sync profiles.
* Should remove 30 seconds from standard facility registration 
* All queries should complete in under 10 seconds.
* False positive rates would be extremely low: 1/10,000,000,000. Expected false negative rates are higher at 1/1,000,000. This is due to the variability of data inputs.
* NoID provides security and fraud management
* Protects data by 3 key factors. 1. centrally stored profiles are abstracted from their raw data 2. nodes ability to communicate across the NoID protocol is dictated by the node’s collateral or NoID coins. 3. communication is fully encrypted
* Uses TCP for network communication and package management
* FHIR provides a means to check for well-formed and complete data
* NoID provides support for privacy and anonymity
* HIPAA and HITECH compliant through encryption and hashing
* Provides patient ability to set privacy settings such as node access
* NoID is scalable 
* Software is free, open source, and available via MIT license
* Uses commodity hardware
* Supports all major platforms (Android, iOS, Windows, Linux, etc)
* Uses peer-2-peer (P2P) decentralized cryptocurrency architecture 
* Profiles are completely unique and can scale to handle the US population
* Due to use of biometrics, there is potential interface capabilities with other systems such as India’s “Aahaar” national id system.
* NoID Adoptability is high
* Offers a safe, reliable, scalable, and inexpensive means to uniquely identify all patients in the US
* State and federal agencies such as Medicare, Medicaid, and the V.A. will need to register as NoID HeatlhOrg nodes
* Patient participation will be driven by healthcare providers
* NoID will follow standard implementation using SDLC

#### Message and Schema Standard
HL7 FHIR
#### Blockchain and RPC API (C++/C#/JSON)
Duality Binary Blockchain system constisting of Dynamic and Sequence.
#### Embedded Browser (C++/C#)
Chromium Embedded Framework or CEF
* Windows .NET using CEFSharp (C#)
* MacOS, Linux, CEF using Qt Framework (C++).  U++ might be a good alternative.
#### FHIR Server (C#) 
Using FHIR Spark with MongoDB
#### Core Matching (C#) etc will use
Metaphone 2 or 3 or phonetics 
Standard first name database
Levinstien Distance (only for non-hashed matching)
Fingerprint Minutiae Data Format: ISO/IEC 19794-2:2005 (Want to move to ANSI INSITS 378-2004)
#### Demographic Hash Algorithm (C/C++/C#) 
Argon2i (Password Hash Winner 2015).  Slow hashing on CPU & GPU.  No ASICS
#### UI(HTML5, CSS3, JavaScript, maybe AngularJS)
Bootstrap, JQuery, etc..  TBD
### UI Data Layer (C# web services)
ASP.NET Web Handler Pages or WebSockets.
#### Data Layer.
Clustered Database (persistence): MongoDB or Couchbase Server
Embedded Database (cache/memory): DBreeze for match engine
#### App Store
Health Services Platform Consortium App Store.
