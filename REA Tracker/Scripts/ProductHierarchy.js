function validateNewForm()
{

    var valid = true;
    var ErrorString = "";

    var NewParent = document.getElementById("NewParentProductID");
    var NewParentName = NewParent.options[NewParent.selectedIndex].text;
    var NewChild    = document.getElementById("NewChildProductID");
    var NewChildName = NewChild.options[NewChild.selectedIndex].text;


    if (NewParent.options[NewParent.selectedIndex].text == NewChild.options[NewChild.selectedIndex].text)
    {
        valid = false;
        ErrorString += "-The child and parent cannot be the same.\n"
    }

    rows = document.getElementById('ProductRelationTableHtml').getElementsByTagName('tr');
    for (i = 2; i < rows.length; i++) {
        var parent = rows[i].getElementsByTagName('td')[1].innerHTML;
        var child = rows[i].getElementsByTagName('td')[2].innerHTML;
        if ((parent.indexOf(NewParentName)>-1) && (child.indexOf(NewChildName)>-1)) {
            valid = false;
            ErrorString += '-The relation already exists.\n';
        }
        else if (
            (parent.indexOf(NewChildName) > -1) && (child.indexOf(NewParentName) > -1)
            ) {
            valid = false;
            ErrorString += '-The inverse relation exists, will cause a infinite loop.\n';
        }
    }

    if (valid == false) {
        alert("The following errors where found:\n\n" + ErrorString);
    }
    return valid;
}

function validateDeleteform()
{

    var valid = true;
    var ErrorString = "";
    var inputs = new Array();
    inputs = document.querySelectorAll("input[type=checkbox]");
    for( var i = 0; i<inputs.length; i++ )
    {

    }
    if (valid == false) {
        alert("The following errors where found:\n\n" + ErrorString);
    }
    return valid;

}