function populateReviewPage() {
    var firstName = document.getElementById("FirstName").value;
    var middleName = document.getElementById("MiddleName").value;
    var lastName = document.getElementById("LastName").value;
    var birthYear = document.getElementById('BirthYear');
    var selectedBirthYear = birthYear.options[birthYear.selectedIndex].text;
    var birthMonth = document.getElementById('BirthMonth');
    var selectedBirthMonth = birthMonth.options[birthMonth.selectedIndex].text;
    var birthDay = document.getElementById('BirthDay');
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
    var multipleBirthPregnancy = document.getElementById('FromMultipleBirthPregnancy');
    var selectedMultipleBirthPregnancy = multipleBirthPregnancy.options[multipleBirthPregnancy.selectedIndex].innerText;
    var genderChanged = document.getElementById('GenderChanged');
    var selectedGenderChanged = genderChanged.options[genderChanged.selectedIndex].innerText;
    
    //begin populate reveiw page
    document.getElementById('revName').innerText = firstName + " " + (middleName.length > 0 ? middleName + " " + lastName : lastName);
    document.getElementById('revDOB').innerText = selectedBirthMonth + " " + selectedBirthDay + ", " + selectedBirthYear;
    document.getElementById('revGender').innerText = gender;
    document.getElementById('revStreetAddress').innerText = streetAddress;
    //if street address2 not blank set innerhtml and include line break after
    document.getElementById('revStreetAddress2').innerHTML = (streetAddress2.length > 0 ? streetAddress2 + "<br />" : "");
    document.getElementById('revCityState').innerText = city + ", " + state;
    document.getElementById('revPostalCode').innerText = postalCode;
    document.getElementById('revPhoneNumber').innerText = phoneCell;
    document.getElementById('revEmail').innerText = emailAddress;
    document.getElementById('revTwin').innerText = selectedMultipleBirthPregnancy;
    document.getElementById('revGenderChanged').innerText = selectedGenderChanged;
  
};
function setLateralitySite(selectedElementID) {
    switch (selectedElementID) {
        case 'selectLeftLittle':
            document.getElementById('spnLeftHandFinger').innerText = "Left Little Finger";
            document.getElementById('spnLeftHandFinger2').innerText = "Left Little Finger";
            document.getElementById('scanStatusMessageLeft').innerHTML = "<h4>Please scan your:<br /><h3>Left Little Finger</h3>If this is correct, please place your finger on the scanner as shown in the image to the right.<br /><br />If this is not correct,<br />please select another finger.</h4>";
            savelateralityCaptureSite("Left", "LittleFinger");
            break;
        case 'selectLeftRing':
            document.getElementById('spnLeftHandFinger').innerText = "Left Ring Finger";
            document.getElementById('spnLeftHandFinger2').innerText = "Left Ring Finger";
            document.getElementById('scanStatusMessageLeft').innerHTML = "<h4>Please scan your:<br /><h3>Left Ring Finger</h3>If this is correct, please place your finger on the scanner as shown in the image to the right.<br /><br />If this is not correct,<br />please select another finger.</h4>";
            savelateralityCaptureSite("Left", "RingFinger");
            break;
        case 'selectLeftMiddle':
            document.getElementById('spnLeftHandFinger').innerText = "Left Middle Finger";
            document.getElementById('spnLeftHandFinger2').innerText = "Left Middle Finger";
            document.getElementById('scanStatusMessageLeft').innerHTML = "<h4>Please scan your:<br /><h3>Left Middle Finger</h3>If this is correct, please place your finger on the scanner as shown in the image to the right.<br /><br />If this is not correct,<br />please select another finger.</h4>";
            savelateralityCaptureSite("Left", "MiddleFinger");
            break;
        case 'selectLeftIndex':           
            document.getElementById('spnLeftHandFinger').innerText = "Left Index Finger";
            document.getElementById('spnLeftHandFinger2').innerText = "Left Index Finger";
            document.getElementById('scanStatusMessageLeft').innerHTML = "<h4>Please scan your:<br /><h3>Left Index Finger</h3>If this is correct, please place your finger on the scanner as shown in the image to the right.<br /><br />If this is not correct,<br />please select another finger.</h4>";
            savelateralityCaptureSite("Left", "IndexFinger");
            break;
        case 'selectLeftThumb':
            document.getElementById('spnLeftHandFinger').innerText = "Left Thumb";
            document.getElementById('spnLeftHandFinger2').innerText = "Left Thumb";
            document.getElementById('scanStatusMessageLeft').innerHTML = "<h4>Please scan your:<br /><h3>Left Thumb</h3>If this is correct, please place your finger on the scanner as shown in the image to the right.<br /><br />If this is not correct,<br />please select another finger.</h4>";
            savelateralityCaptureSite("Left", "Thumb");
            break;            
        case 'selectRightLittle':
            document.getElementById('spnRightHandFinger').innerText = "Right Little Finger";
            document.getElementById('spnRightHandFinger2').innerText = "Right Little Finger";
            document.getElementById('scanStatusMessageRight').innerHTML = "<h4>Please scan your:<br /><h3>Right Little Finger</h3>If this is correct, please place your finger on the scanner as shown in the image to the right.<br /><br />If this is not correct,<br />please select another finger.</h4>";
            savelateralityCaptureSite("Right", "LittleFinger");
            break;
        case 'selectRightRing':
            document.getElementById('spnRightHandFinger').innerText = "Right Ring Finger";
            document.getElementById('spnRightHandFinger2').innerText = "Right Ring Finger";
            document.getElementById('scanStatusMessageRight').innerHTML = "<h4>Please scan your:<br /><h3>Right Ring Finger</h3>If this is correct, please place your finger on the scanner as shown in the image to the right.<br /><br />If this is not correct,<br />please select another finger.</h4>";
            savelateralityCaptureSite("Right", "RingFinger");
            break;
        case 'selectRightMiddle':
            document.getElementById('spnRightHandFinger').innerText = "Right Middle Finger";
            document.getElementById('spnRightHandFinger2').innerText = "Right Middle Finger";
            document.getElementById('scanStatusMessageRight').innerHTML = "<h4>Please scan your:<br /><h3>Right Middle Finger</h3>If this is correct, please place your finger on the scanner as shown in the image to the right.<br /><br />If this is not correct,<br />please select another finger.</h4>";
            savelateralityCaptureSite("Right", "MiddleFinger");
            break;
        case 'selectRightIndex':           
            document.getElementById('spnRightHandFinger').innerText = "Right Index Finger";
            document.getElementById('spnRightHandFinger2').innerText = "Right Index Finger";
            document.getElementById('scanStatusMessageRight').innerHTML = "<h4>Please scan your:<br /><h3>Right Index Finger</h3>If this is correct, please place your finger on the scanner as shown in the image to the right.<br /><br />If this is not correct,<br />please select another finger.</h4>";
            savelateralityCaptureSite("Right", "IndexFinger");
            break;
        case 'selectRightThumb':
            document.getElementById('spnRightHandFinger').innerText = "Right Thumb";
            document.getElementById('spnRightHandFinger2').innerText = "Right Thumb";
            document.getElementById('scanStatusMessageRight').innerHTML = "<h4>Please scan your:<br /><h3>Right Thumb</h3>If this is correct, please place your finger on the scanner as shown in the image to the right.<br /><br />If this is not correct,<br />please select another finger.</h4>";
            savelateralityCaptureSite("Right", "Thumb");
            break;
                    
    };
};
function savelateralityCaptureSite(laterality, captureSite) {
    NoIDBridge.postLateralityCaptureSite(laterality, captureSite);
    if (NoIDBridge.errorDescription != '') {
       //error, show user message
        alert("postLateralityCaptureSite Error " + NoIDBridge.errorDescription);
    }
};
function showComplete(whichStep) {
    switch (whichStep) {
        case 'Left':
            document.getElementById('checkLeft').setAttribute('class', 'fa fa-check-square-o fa-5x fa-fw pull-right complete');
            document.getElementById('scanStatusMessageLeft').innerHTML = "<h4>Success!<br />Please click next to scan a finger<br />from your right hand</h4>";
            document.getElementById('leftFingerNextButton').disabled = false;
            document.getElementById('rightFingerprintBackButton').disabled = true;
            break;
        case 'Right':
            document.getElementById('checkRight').setAttribute('class', 'fa fa-check-square-o fa-5x fa-fw pull-right complete');
            document.getElementById('scanStatusMessageRight').innerHTML = "<h4>Success!<br />Please click next.</h4>";
            document.getElementById('rightFingerNextButton').disabled = false;
            document.getElementById('demographics1').disabled = true;
            break;
    };
};
function showFail(whichStep) {
    switch (whichStep) {
        case 'Left':
            //document.getElementById('checkLeft').setAttribute('class', 'fa fa-times fa-5x fa-fw pull-right incomplete');
            document.getElementById('checkLeft').setAttribute('class', 'fa fa-spinner fa-spin fa-5x fa-fw pull-right incomplete');
            document.getElementById('scanStatusMessageLeft').innerHTML = "<h4>Fingerprint scan attempt<br />was not successful.<br />Please try again</h4>";
            break;
        case 'Right':
            //document.getElementById('checkRight').setAttribute('class', 'fa fa-times fa-5x fa-fw pull-right complete');
            document.getElementById('checkRight').setAttribute('class', 'fa fa-spinner fa-spin fa-5x fa-fw pull-left incomplete');
            document.getElementById('scanStatusMessageRight').innerHTML = "<h4>Fingerprint scan attempt<br />was not successful.<br />Please try again</h4>";
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
// mark schroeder 20170627 testing
function moveToRightHandScan() {    
    alert("We were unable to get a fingerprint from your left hand. Let's try your right hand next. Please close this window and we will try the right hand.");
    document.getElementById('leftFingerNextButton').disabled = false;
    document.getElementById('leftFingerNextButton').click();
};
/*function savePatientTest() {
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
    
   
};*/
