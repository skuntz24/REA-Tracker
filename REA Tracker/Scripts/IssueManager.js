function ValidateNewIssueForm()
{
    var valid = true;
    var ErrorString = "";
    var NewName = document.getElementById("NewName").value;
    var NewDescription = document.getElementById("NewDescription").value;

    if(NewName.length == 0)
    {
        valid = false;
        ErrorString += "-New Name cannot be empty when creating a new Issue.\n"
    }

    if (valid == false) {
        alert("The following errors where found:\n\n" + ErrorString);
    }
    return valid;
}

function ValidateUpdateIssueForm()
{
    var valid = true;
    var ErrorString = "";
    var inputs = new Array();
    inputs = document.querySelectorAll("input[type=text]");
    for( var i = 0; i<inputs.length; i++ )
    {
        if (inputs[i].getAttribute("name") != null &&
            inputs[i].getAttribute("name").indexOf(".TypeName") > -1 &&
            inputs[i].value.length == 0
            )
        {
            valid = false;
            ErrorString = "-Name of current issue types cannot be empty.\n";
        }
    }
    if (valid == false) {
        alert("The following errors where found:\n\n" + ErrorString);
    }
    return valid;
}