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
function wait(whichStep) {
    setTimeout(function () {
        showComplete(whichStep);
    }, 5000);

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