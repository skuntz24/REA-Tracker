function validateForm()
{
    var ErrorString = "";
    var valid = true;
    var Build = document.getElementById("Build").value;
    var Major = document.getElementById("Major").value;
    var Minor = document.getElementById("Minor").value;

    if (Major.length == 0 || Minor.length == 0) {
        valid = false;
        ErrorString += "-There are no SCRs set to delivered and assigned to the release coordinator.\n";
    }
    else {
        if (Build.length == 0) {
            valid = false;
            ErrorString += "-The Build cannot be left blank.\n";
        }
        else if (!isNumberFloat(Build)) {
            valid = false;
            ErrorString += "-The Build can only be numbers.\n";
        }
        var checkboxes = new Array();
        checkboxes = document.getElementsByTagName('input');
        var selectedAtleastOne = false;
        for (var i = 0; i < checkboxes.length; i++) {
            if (checkboxes[i].type == 'checkbox' &&
                checkboxes[i].getAttribute("name").indexOf(".Selected") > -1 &&
                checkboxes[i].checked != false
                ) {
                selectedAtleastOne = true;
            }
        }
        if (!selectedAtleastOne) {
            valid = false;
            ErrorString += "-Please select an SCR.\n";
        }
    }
    var allselected = true;
    $("select").each(function () {
        if (parseInt($(this).val()) <0)
        {
            allselected = false;
        }
    });
    if(allselected == false)
    {
        valid = false;
        ErrorString += "-You must select one of each sub-build.\n";
    }
    if (valid == false) {
        alert("The following errors were found:\n\n" + ErrorString);
    }
    return valid;
}

function checkAll() {
    var checkboxes = new Array();
    checkboxes = document.getElementsByTagName('input');

    for (var i = 0; i < checkboxes.length; i++) {
        if (checkboxes[i].type == 'checkbox' &&
            (checkboxes[i].getAttribute("name").indexOf(".Selected") > -1)
            ) {
            //for some reason firefox can have a checked box without having a check
            checkboxes[i].checked = true;
        }
    }
}

function isNumberFloat(inputString) {
    return (!isNaN(parseFloat(inputString))) ? true : false;
}