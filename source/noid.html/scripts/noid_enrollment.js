function showComplete(whichStep) {
    switch (whichStep) {
        case 'left':
            document.getElementById('checkLeft').setAttribute('class', 'fa fa-check-square-o fa-5x fa-fw pull-right complete');
            break;
        case 'right':
            document.getElementById('checkRight').setAttribute('class', 'fa fa-check-square-o fa-5x fa-fw pull-right complete');
            break;
    };
};
function showFail(whichStep) {
    switch (whichStep) {
        case 'left':
            document.getElementById('checkLeft2').setAttribute('class', 'fa fa-times fa-5x fa-fw pull-right incomplete');
            break;
        case 'right':
            document.getElementById('checkRight2').setAttribute('class', 'fa fa-times fa-5x fa-fw pull-right complete');
            break;
    };
};
function wait(whichStep, result) {
    switch (result) {
        case 'success':
            setTimeout(function () {
                showComplete(whichStep);
            }, 5000);
            break;
        case 'fail':
            setTimeout(function () {
                showFail(whichStep);
            }, 5000);
            break;
    };
};
function showNewExistingModal() {    
 setTimeout(function () {
    $('#newReturnPatientModal').modal('show');
  }, 750);    
};
function showExceptions() {
    document.getElementById('exceptions').setAttribute('class', 'tab-pane active');
    document.getElementById('step2').setAttribute('class', 'tab-pane');
};
function hideExceptions() {    
    document.getElementById('optionsRadios1').checked = true;
    document.getElementById('exceptions').setAttribute('class', 'tab-pane');
    document.getElementById('step2').setAttribute('class', 'tab-pane active');
};
function showFailure() {
    document.getElementById('step5').setAttribute('class', 'tab-pane active');
    document.getElementById('step4').setAttribute('class', 'tab-pane');
};
function confirmFailure() {
    document.getElementById('step6').setAttribute('class', 'tab-pane active');
    document.getElementById('step5').setAttribute('class', 'tab-pane');
};
function showPatient() {
    var x = document.getElementById("patientSelect").selectedIndex;
    //alert("Patient " + x.innerText);
    var y = document.getElementsByTagName("option")[x].value;
    setPatient(document.getElementsByTagName("option")[x].innerText);
    var pts = document.getElementsByClassName('patients');
    for (var j = 0; j < pts.length; j++) {
            pts[j].setAttribute('class', 'patients hidden');
    };
    document.getElementById(y).setAttribute('class', 'patients col-md-9');
};
function setPatient(patName) {
    document.getElementById('patientInfo').innerHTML = " <label>Name:</label>" + patName;
    document.getElementById('patientConfirm').innerText = patName + " has been positively identified and linked to your local NoID system.";
};
function reset() {
    document.getElementById('checkLeft').setAttribute('class', 'fa fa-spinner fa-spin fa-5x fa-fw pull-right incomplete');
    wait('left', 'fail');
};
function startTime() {
    var today = new Date();
    var h = today.getHours();
    var m = today.getMinutes();
    var s = today.getSeconds();
    var day = today.getDate();
    var month = today.getMonth() + 1;
    var year = today.getFullYear();
    m = checkTime(m);
    s = checkTime(s);
    day = checkTime(day);
    month = checkTime(month);
    document.getElementById('currentDateTime').innerHTML =
      //  today ;
    month + "/" + day + "/" + year + " " + h + ":" + m + ":" + s;
    var t = setTimeout(startTime, 500);
};
function checkTime(i) {
    if (i < 10) {i = "0" + i};  // add zero in front of numbers < 10
    return i;
};
function savePatientTest() {
    var language = document.getElementById('selectedLanguage');
    var languageSelected = language.options[language.selectedIndex].text;
    var firstName = document.getElementById("FirstName").value;
    var middleName = document.getElementById("MiddleName").value;
    var lastName = document.getElementById("LastName").value;
    var birthYear =  document.getElementById('BirthYear');
    var selectedBirthYear = birthYear.options[birthYear.selectedIndex].text;
	var birthMonth =  document.getElementById('BirthMonth');
    var selectedBirthMonth = birthMonth.options[birthMonth.selectedIndex].text;
	var birthDay =  document.getElementById('BirthDay');
    var selectedBirthDay = birthDay.options[birthDay.selectedIndex].text;
	if (document.getElementById("GenderMale").checked) {
		var gender = document.getElementById("GenderMale").value;
	};
	if (document.getElementById("GenderFemale").checked) {
		var gender = document.getElementById("GenderFemale").value;
	};
	var streetAddress = document.getElementById("StreetAddress").value;
	var streetAddress2 = document.getElementById("StreetAddress2").value;
	var city = document.getElementById("City").value;
	var state = document.getElementById("State").value;
	var postalCode = document.getElementById("ZipCode").value;
	var phoneCell = document.getElementById("PhoneNumber").value;
	var emailAddress = document.getElementById("EmailAddress").value;
	//may not need below for profile, but will need eventually
	var newOrReturnPatient = document.getElementById('NewOrReturnPatient');
    var selectedNewOrReturnPatient = newOrReturnPatient.options[newOrReturnPatient.selectedIndex].value;
	var multipleBirthPregnancy = document.getElementById('FromMultipleBirthPregnancy');
    var selectedMultipleBirthPregnancy = multipleBirthPregnancy.options[multipleBirthPregnancy.selectedIndex].value;
	var genderChanged = document.getElementById('GenderChanged');
    var selectedGenderChanged = genderChanged.options[genderChanged.selectedIndex].value;
	var patientHub = document.getElementById('PatientHub');
	var selectedPatientHub = patientHub.options[patientHub.selectedIndex].value;
	var portalPassword = document.getElementById("PortalPassword").value;
	
	    alert("Language = " + languageSelected 
        + "\nFirst Name = " + firstName 
        + "\nMiddle Name = " +  middleName 
        + "\nLast Name = " + lastName
        + "\nBirth Year = " + selectedBirthYear
		+ "\nBirth Month = " + selectedBirthMonth
		+ "\nBirth Day = " + selectedBirthDay
		+ "\nGender = " + gender
		+ "\nStreetAddress = " + streetAddress
		+ "\nStreetAddress2 = " + streetAddress2
		+ "\nCity = " + city
		+ "\nState = " + state
		+ "\nPostalCode = " + postalCode
		+ "\nPhoneCell = " + phoneCell
		+ "\nEmail = " + emailAddress
		+ "\nNew or Return Patient = " + selectedNewOrReturnPatient
		+ "\nFrom Multiple Birth Pregnancy = " + selectedMultipleBirthPregnancy
		+ "\nGender Changed = " + selectedGenderChanged
		+ "\nPatient Hub = " + selectedPatientHub
		+ "\nPortal Password = " + portalPassword);
    
    /*
    string language,
                string firstName,
                string middleName,
                string lastName,
                string gender,
                string birthYear,
                string birthMonth,save
                string streetAddress,
				string streetAddress2,
                string city,
                string state,
                string postalCode,
                string emailAddress,
                string phoneCell
    */
};
