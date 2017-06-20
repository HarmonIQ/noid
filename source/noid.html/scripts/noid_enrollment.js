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