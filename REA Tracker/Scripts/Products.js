
function validateInformationForm() {
    var ErrorString = "";
    var valid = true;
    var ProductName = document.getElementById("CurrentProductName").value;
    var BillingCode = document.getElementById("BillingCode").value;
    if (BillingCode.length == 0)
    {
        valid = false;
        ErrorString += "-Billing Code is required.\n";
    }
    else if (!isInt(BillingCode)) {
        valid = false;
        ErrorString += "-Billing Code must be a integer.\n";
    }
    if(ProductName.length==0)
    {
        valid = false;
        ErrorString += "-Product name cannot be empty.\n";
    }
    if(valid==false)
    {
        alert("The following errors were found:\n\n"+ErrorString);
    }
    return valid;
}

function validateNewModuleForm()
{
    var ErrorString = "";
    var valid = true;
    var NewModuleName = document.getElementById("NewModuleName").value;

    if (NewModuleName.length == 0)
    {
        valid = false;
        ErrorString += "-New Modules cannot have blank names.\n";
    }
    if (!valid) {
        alert("The following errors where found:\n\n" + ErrorString)
    }
    return valid;
}

function validateUpdateModuleForm()
{
    var ErrorString = "";
    var valid = true;

    var inputs = new Array();
    inputs = document.querySelectorAll("input[type=text]");
    var AtleastOneIsNull = false;
    for (var i = 0; i < inputs.length; i++) {
        if (inputs[i].type == 'text' &&
            inputs[i].getAttribute("name") != null &&
            inputs[i].getAttribute("name").indexOf("ProductModulesName") > -1 &&
            inputs[i].value.length == 0
            ) {
            AtleastOneIsNull = true;
        }
    }
    if (AtleastOneIsNull) {
        valid = false;
        ErrorString += "The name field in existing modules cannot be left blank.\n";
    }

    if(!valid)
    {
        alert("The following errors where found:\n\n"+ErrorString)
    }
    return valid;
}

function validateNewProductForm()
{
    var ErrorString = "";
    var valid = true;
    var Name = document.getElementById("CurrentProductName").value;
    var BillingCode = document.getElementById("BillingCode").value;
    if ( Name.length == 0 )
    {
        valid = false;
        ErrorString += "-The new product must have a name.\n";
    }
    if (BillingCode.length == 0) {
        valid = false;
        ErrorString += "-The new product must have a Billing Code.\n";
    }
    if( valid == false )
    {
        alert("The following errors where found:\n\n" + ErrorString);
    }
    return valid; 
}

function isInt(value) {
    return !isNaN(value) && parseInt(Number(value)) == value;
}