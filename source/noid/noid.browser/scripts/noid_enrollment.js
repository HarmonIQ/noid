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