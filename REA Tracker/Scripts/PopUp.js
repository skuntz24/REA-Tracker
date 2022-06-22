function popUp(URL) {
    day = new Date();
    id = day.getTime();

    var strWindowFeatures = 'toolbar=no,scrollbars=no,statusbar=no,location=no,menubar=no,resizable=no,width=640,height=480,left = 600,top = 480'
    if (navigator.userAgent.indexOf("Mozilla") != -1) {
        window.open(URL, id, strWindowFeatures).moveTo(0, 0);
    }
    else {
        dom.disable_window_open_feature.location;
        dom.disable_window_open_feature.status;
        var popup = window.open(URL, "Name", "_blank");
        popup.focus();
    }
}
