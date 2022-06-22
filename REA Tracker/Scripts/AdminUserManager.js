//-----------------------DIsable fields----------------------------------------
function FullAccessChange() {
    var fullaccess = document.getElementById("fullAccess");
    if (true == fullaccess.checked) {
        document.getElementById("toLeft").disabled = true;
        document.getElementById("toRight").disabled = true;
        document.getElementById("Clear").disabled = true;
        document.getElementById("NoAccess").disabled = true;
        document.getElementById("giveAccess").disabled = true;
    }
    else
    {
        document.getElementById("toLeft").disabled = false;
        document.getElementById("toRight").disabled = false;
        document.getElementById("Clear").disabled = false;
        document.getElementById("NoAccess").disabled = false;
        document.getElementById("giveAccess").disabled = false;
    }
}

function OtherCompany(company, othercompany)
{
    if (company.options[company.selectedIndex].value != 0) {
        othercompany.style.backgroundColor = "#D3D3D3";
        othercompany.readOnly = true;
        
    }
    else {
        othercompany.readOnly = false;
        othercompany.style.backgroundColor = "white";
    }
}