/*
Unfortunately these have to be hardcoded here
*/
var StatusEnum = {
    Submitted:  1,
    Approved:   2,
    Deferred:   3,
    Rejected:   4,
    InProcess:  5,
    Fixed:      6,
    Delivered:  7,
    Testing:    8,
    Closed:     9,
    OnHold:     10,
    Built:      11,
};

function enableField(field, enable)
{
    if (field != null) {
        if (enable) {
            field.readOnly = false;
            field.style.background = "white";
        }
        else {
            field.readOnly = true;
            //field.style.background = "#C8C8C8";
        }
    }
    else
    {
        alert("The field that was passed was NULL.")
    }
}
function enhancement() {
    var Issuelist = document.getElementById("IssueID");
    var selected = parseInt(Issuelist.options[Issuelist.selectedIndex].value);
    //bugs
    var details = document.getElementById("Details");
    //enhancement
    var problem = document.getElementById("Problem");
    var benifit = document.getElementById("Benefit");
    var solution = document.getElementById("Solution");

    //plannedwork
    var background = document.getElementById("Background");
    var story = document.getElementById("Story");
    var acceptance = document.getElementById("Acceptance");


    switch(selected)
    {
        case 1: //Issue Type is PROBLEM
            {
                enableField(details, true);
                enableField(problem, false);
                enableField(solution, false);
                enableField(benifit, false);
                enableField(background, false);
                enableField(story, false);
                enableField(acceptance, false);
                break;
            }
        case 2: //Issue Type is ENHANCEMENT
            {
                enableField(details, false);
                enableField(problem, true);
                enableField(solution, true);
                enableField(benifit, true);
                enableField(background, false);
                enableField(story, false);
                enableField(acceptance, false);
                break;
            }
        case 3: //Issue Type is PLANNED WORK
            {
                enableField(details, false);
                enableField(problem, false);
                enableField(solution, false);
                enableField(benifit, false);
                enableField(background, true);
                enableField(story, true);
                enableField(acceptance, true);
                break;
            }
    }
}

function toggleEnhancementAccordian()
{
    var Issuelist = document.getElementById("IssueID");
    var selected = parseInt(Issuelist.options[Issuelist.selectedIndex].value);
    var enhancement = document.getElementById("Enhancements");
    var plannedwork = document.getElementById("PlannedWork");
    switch (selected) {
        case 1:
            {
                enhancement.style.display = 'none';
                plannedwork.style.display = 'none';
                break;
            }
        case 2:
            {
                document.getElementById("mainTitle").innerHTML = "<a style='color: white'>Enhancement Details</a>";
                document.getElementById("problemName").innerHTML = "Problem / Oppurtunity";
                //document.getElementById("textAreaProblem").required = true;
                document.getElementById("solutionName").innerHTML = "Proposed Solution";
                document.getElementById("benefitsName").innerHTML = "Benefit";
                //document.getElementById("textAreaBenefits").required = true;
                enhancement.style.display = 'block';
                //plannedwork.style.display = 'none';
                break;
            }
        case 3:
            {
                document.getElementById("mainTitle").innerHTML = "<a style='color: white'>Planned Work</a>";
                document.getElementById("problemName").innerHTML = "Background / Rational";
                document.getElementById("textAreaProblem").required = false;
                document.getElementById("solutionName").innerHTML = "Story";
                document.getElementById("benefitsName").innerHTML = "Acceptance Criteria";
                document.getElementById("textAreaBenefits").required = false;
                //enhancement.style.display = 'none';
                //plannedwork.style.display = 'block';
                break;
            }
    }
}

//$(document).ready(function () {
//    $('*[data-autocomplete-url]')
//        .each(function () {
//            $(this).autocomplete({
//                source: $(this).data("GetCompanyNames-url")
//            });
//        });
//});

//this is function that is called when SCR is being editted and the issue type is changed
function toggleEnhancement(value) {
    value = parseInt(value);
    var enhancement = document.getElementById("Enhancements");
    var plannedwork = document.getElementById("PlannedWork");
    
    switch(value)
    {
        case 1:
            {
                enhancement.style.display = 'none';
                plannedwork.style.display = 'none';
                break;
            }
        case 2:
            {
                document.getElementById("mainTitle").innerHTML = "<a style='color: white'>Enhancement Details</a>";
                document.getElementById("problemName").innerHTML = "Problem / Oppurtunity";
                //document.getElementById("textAreaProblem").required = true;
                document.getElementById("solutionName").innerHTML = "Proposed Solution";
                document.getElementById("benefitsName").innerHTML = "Benefit";
                //document.getElementById("textAreaBenefits").required = true;
                enhancement.style.display = 'block';
                //plannedwork.style.display = 'none';
                break;
            }
        case 3:
            {
                document.getElementById("mainTitle").innerHTML = "<a style='color: white'>Planned Work</a>";
                document.getElementById("problemName").innerHTML = "Background / Rational";
                //document.getElementById("textAreaProblem").required = false;
                document.getElementById("solutionName").innerHTML = "Story";
                document.getElementById("benefitsName").innerHTML = "Acceptance Criteria";
                //document.getElementById("textAreaBenefits").required = false;
                enhancement.style.display = 'block';
                //plannedwork.style.display = 'block';
                break;
            }
    }

    var issueValue = document.getElementById("issueType");
    var oldIssueType = document.getElementById("OldIssueType");
    var assignedToSelectList = document.getElementById("assignedToId");
    var SCCB = document.getElementById("SCCB");

    if (OldIssueType.value !=2 && issueValue.value == 2)
    {
        if (assignedToSelectList.length == null) {
            assignedToSelectList.value = SCCB.value;
        }
        else {
            for (i = 0; i < assignedToSelectList.length; i++) {
                if (assignedToSelectList.options[i].value == SCCB.value) {
                    assignedToSelectList.value = SCCB.value;
                    break;
                }
            }
        }
    }
}

function customer()
{
    var CustomerName = document.getElementById("ContactPerson");
    var CompanyName = document.getElementById("CompanyName");
    var CompanyLocation = document.getElementById("Location");
    var CustomerBugSelectList = document.getElementById("CustomerReported");
    var isCustomer = CustomerBugSelectList.options[CustomerBugSelectList.selectedIndex].value;
    if (isCustomer == "true")
    {
        CustomerName.readOnly = false;
        CustomerName.style.background = "white";
        CompanyName.readOnly = false;
        CompanyName.style.background = "white";
        CompanyLocation.readOnly = false;
        CompanyLocation.style.background = "white";
    }
    else
    {
        CustomerName.readOnly = true;
        CustomerName.style.background = "#C8C8C8";
        //CustomerName.value = '\0';
        CompanyName.readOnly = true;
        CompanyName.style.background = "#C8C8C8";
        //CompanyName.value = '\0';
        CompanyLocation.readOnly = true;
        CompanyLocation.style.background = "#C8C8C8";
        //CompanyLocation.value = '\0';
    }
}
//To-Do change name to above
function customerEdit() {
    var CustomerName = document.getElementById("customerName");
    var CompanyName = document.getElementById("customerCompany");
    var CompanyLocation = document.getElementById("customerLocation");
    var CustomerBugSelectList = document.getElementById("customerBug");
    var CustomerErrorMessage = document.getElementById("CustomerNameErrorMessage");
    var syteline = document.getElementById("Syteline");
    var isCustomer = CustomerBugSelectList.options[CustomerBugSelectList.selectedIndex].value;

    if (isCustomer == "true") {
        CustomerName.readOnly = false;
        CustomerName.style.background = "white";
        CustomerName.removeAttribute("disabled");
        CompanyName.readOnly = false;
        CompanyName.style.background = "white";
        CompanyLocation.readOnly = false;
        CompanyLocation.style.background = "white";
        //syteline.readOnly = false;
        //syteline.style.background = "white";
        CustomerErrorMessage.style.display = 'block';   
    }
    else {
        CustomerName.readOnly = true;
        CustomerName.style.background = "#C8C8C8";
        CustomerName.setAttribute("disabled", "disabled");
        resetValidation();
        CustomerErrorMessage.style.display = 'none';
        //CustomerName.value = '\0';
        CompanyName.readOnly = true;
        CompanyName.style.background = "#C8C8C8";
        //CompanyName.value = '\0';
        CompanyLocation.readOnly = true;
        CompanyLocation.style.background = "#C8C8C8";
        //CompanyLocation.value = '\0';
        //syteline.readOnly = true;
        //syteline.style.background = "#C8C8C8";
    }
}

function setActualReleaseVersion(enable)
{
    var status = document.getElementById("statusId");
    alert("setActualReleaseVersion");
    if (!enable) {
        document.getElementById("versionReleasedMajor").style.background = "#C8C8C8";
        document.getElementById("versionReleasedMajor").readOnly = true;

        document.getElementById("versionReleasedMinor").style.background = "#C8C8C8";
        document.getElementById("versionReleasedMinor").readOnly = true;

        document.getElementById("versionReleasedBuild").style.background = "#C8C8C8";
        document.getElementById("versionReleasedBuild").readOnly = true;

        document.getElementById("versionReleasedMisc").style.background = "#C8C8C8";
        document.getElementById("versionReleasedMisc").readOnly = true;
    }
    else
    {
        document.getElementById("versionReleasedMajor").style.background = "white";
        document.getElementById("versionReleasedMajor").readOnly = false;

        document.getElementById("versionReleasedMinor").style.background = "white";
        document.getElementById("versionReleasedMinor").readOnly = false;

        document.getElementById("versionReleasedBuild").style.background = "white";
        document.getElementById("versionReleasedBuild").readOnly = false;

        document.getElementById("versionReleasedMisc").style.background = "white";
        document.getElementById("versionReleasedMisc").readOnly = false;
    }
}

function toggleStatus(value)
{
    //New inputs
        //Closed Reason
    var closedStatus    = document.getElementById("ClosedStatus");
        //Closed Version
    var closedStatus2   = document.getElementById("ClosedStatus2");
    var statusbar       = document.getElementById("statusBar");
    var status          = document.getElementById("statusId");

    //Old Input Values
    var oldStatus       = document.getElementById("OldStatus");
    var oldResolvedBy   = document.getElementById("OldResolved_By");
    var oldAssignedTo   = document.getElementById("OldAssigned_To");
    var oldClosedby     = document.getElementById("OldClosed_by");
    var Validator       = document.getElementById("Validator");
    var SCCB            = document.getElementById("SCCB");
    var DefaultTester   = document.getElementById("DefaultTester");
    var ReleaseCoordinator
                        = document.getElementById("Rel_Coord");
    var CurrUser        = document.getElementById("User_id");
    //SelectLists
    var resolvedBySelectList    = document.getElementById("resolvedByID");
    var ClosedBySelectList      = document.getElementById("closedByID");
    var assignedToSelectList    = document.getElementById("assignedToId");
    var resolvedByBar = document.getElementById("resolved");

    //disable the release version fields
    if (status.value <= StatusEnum.InProcess)
    {
        //document.getElementById("canEditversionReleased").value = false;
        //document.getElementById("VersionReleased").style.background = "#C8C8C8";
        //document.getElementById("VersionReleased").readOnly = true;
        document.getElementById("versionReleasedMajor").style.background = "#C8C8C8";
        document.getElementById("versionReleasedMajor").readOnly = true;

        document.getElementById("versionReleasedMinor").style.background = "#C8C8C8";
        document.getElementById("versionReleasedMinor").readOnly = true;

        document.getElementById("versionReleasedBuild").style.background = "#C8C8C8";
        document.getElementById("versionReleasedBuild").readOnly = true;

        document.getElementById("versionReleasedMisc").style.background = "#C8C8C8";
        document.getElementById("versionReleasedMisc").readOnly = true;
    }
    else if (status.value >= StatusEnum.Fixed)
    {
        //document.getElementById("VersionReleased").style.background = "white";
        //document.getElementById("VersionReleased").readOnly = false;
        document.getElementById("versionReleasedMajor").style.background = "white";
        document.getElementById("versionReleasedMajor").readOnly = false;

        document.getElementById("versionReleasedMinor").style.background = "white";
        document.getElementById("versionReleasedMinor").readOnly = false;

        document.getElementById("versionReleasedBuild").style.background = "white";
        document.getElementById("versionReleasedBuild").readOnly = false;

        document.getElementById("versionReleasedMisc").style.background = "white";
        document.getElementById("versionReleasedMisc").readOnly = false;

    }

    //if the status was changed to closed pop on the window
    if (value == StatusEnum.Closed) {
        statusbar.style.width = '137px';
        statusbar.style.cssFloat = 'left';
        closedStatus2.style.display = 'block';
        closedStatus.style.cssFloat = 'right';
        closedStatus.style.display = 'block';

        document.getElementById('versionClosedMajor').value = document.getElementById('versionReleasedMajor').value;
        document.getElementById('versionClosedMinor').value = document.getElementById('versionReleasedMinor').value;
        document.getElementById('versionClosedBuild').value = document.getElementById('versionReleasedBuild').value;

    }
    else {
        statusbar.style.width = '274px';
        closedStatus.style.display = 'none';
        closedStatus2.style.display = 'none';
    }
    //changeStatus() in EditRecord.js
    if (oldStatus.value >= StatusEnum.Fixed && status.value == StatusEnum.InProcess)
    {
        var notresolved = document.getElementById("notresolved");
        if (notresolved != null) {
            document.getElementById("notresolved").style.display = 'block';
        }
        if (resolvedByBar != null) {
            resolvedByBar.style.display = 'none';
        }
        // SCR went from Fixed/Delivered/Testing/Closed back to In Process
        // Try to assign the SCR back to the person who resolved it. Otherwise, validator.
        if (assignedToSelectList.length == null) {
            if (oldResolvedBy.value == null) {
                assignedToSelectList.value = Validator.value;
            }
            else {
                assignedToSelectList.value = oldResolvedBy.value;
            }
        }
        else{
            // It's a combo box
            for (i = 0; i < assignedToSelectList.length; i++) {
                if (assignedToSelectList.options[i].value == oldResolvedBy.value) {
                    assignedToSelectList.value = oldResolvedBy.value;
                    break;
                }
            }
        }
        if (oldStatus.value == StatusEnum.Closed)
        {
            document.getElementById("closedByID").value = null;
        }
        //Clear the actual version released
        //document.getElementById("VersionReleased").value = "";
        document.getElementById("versionReleasedMajor").value = "";
        document.getElementById("versionReleasedMinor").value = "";
        document.getElementById("versionReleasedBuild").value = "";
        document.getElementById("versionReleasedMisc").value = "";
    }
    
    else if (oldStatus.value <= 4 && status.value == 5)
    {
        if (assignedToSelectList.length == null){
            assignedToSelectList.value = CurrUser.value;
        }
        else{
            for (i = 0; i < assignedToSelectList.length; i++) {
                if (assignedToSelectList.options[i].value == CurrUser.value) {
                    assignedToSelectList.value = CurrUser.value;
                    break;
                }
            }
        }
    }

    else if (oldStatus.value <7 && status.value == 7)
    { 
        // In Process to Delivered
        //Set the resolved by to the person who set it to delivered        
        if (oldStatus.value < 6)
        {//if it wasn't fixed before changed the resolved by field
            resolvedBySelectList.value = CurrUser.value;
        }
        
        for (i = 0; i < assignedToSelectList.length; i++) {
            if (assignedToSelectList.options[i].value == CurrUser.value) {
                assignedToSelectList.value = CurrUser.value;
                break;
            }
        }
        if(assignedToSelectList.length == null)
        {
            assignedToSelectList.value = ReleaseCoordinator.value;
        }
        else{
            for (i = 0; i < assignedToSelectList.length; i++) {
                if (assignedToSelectList.options[i].value == ReleaseCoordinator.value) {
                    assignedToSelectList.value = ReleaseCoordinator.value;
                    break;
                }
            }
        }
    }
    else if (((oldStatus.value <= 5) &&
        (((status.value == 6)) || (oldStatus.value != 6 && status.value == 7) || (status.value == 9))))
    {
     //In process to (fixed)||(delivered but wasn't fixes)||(closed)   
        if (oldStatus.value != 9 && status.value == 9)
        {// If the old status was not closed but is now closed
            ClosedBySelectList.value = CurrUser.value;
            resolvedBySelectList.value = CurrUser.value;

        }
        document.getElementById("notresolved").style.display = 'none';
        resolvedBySelectList.value = CurrUser.value;
        resolvedByBar.style.display = 'block';
    }
    // Now that the issue is 'closed', make sure the issue was resolved. If not,
    // make the resolved person the person who closed the SCR.	
    else if (value == 9 && oldStatus.value != 9) {
        ClosedBySelectList.value = CurrUser.value;
        if (resolvedBySelectList.value == -1 || resolvedBySelectList.value == null) {
            resolvedBySelectList.value = CurrUser.value;
        }
    }
    else if (oldStatus.value != 7 && value == 7) {
        for (i = 0; i < assignedToSelectList.length; i++) {
            if (assignedToSelectList.options[i].value == ReleaseCoordinator.value){
                assignedToSelectList.value = ReleaseCoordinator.value;
                break;
            }
        }
    }
    else
    {
        //Fix stuff? this was in old code
    }

}

function resetValidation()
{    
    //Removes validation from input-fields
    $('input#customerName.input-validation-error').addClass('input-validation-valid');
    $('input#customerName.input-validation-error').removeClass('input-validation-error');
    //Removes validation message after input-fields
    $('input#customerName.field-validation-error').addClass('field-validation-valid');
    $('input#customerName.field-validation-error').removeClass('field-validation-error');
    //Removes validation summary 
    $('input#customerName.validation-summary-errors').addClass('validation-summary-valid');
    $('input#customerName.validation-summary-errors').removeClass('validation-summary-errors');
}

//submit button for hours and release checking if it is fixed or not

function isStringEmpty(str)
{
    return (!str || /^\s*$/.test(str)) || (!str || 0 === str.length);
}

//This is the function that is called when the user clicks "Save" when editing an SCR.
function ValidateEditForm()
{
    //Error Message Fields
    var error_message = 'The following errors were detected in your SCR:\n\n';
    var result = true;
    
    //double checking the have something to compare
    if (document.getElementById("statusId") != null && document.getElementById("OldStatus") != null)
    {
        var returnValue = true;

        //value is the current status of the SCR (i.e. fixed, in process, etc...)
        var value = document.getElementById("statusId").options[document.getElementById("statusId").selectedIndex].value;
        var oldValue = document.getElementById("OldStatus").value;
        if (value == StatusEnum.Fixed || value == StatusEnum.Delivered || value == StatusNum.Testing || value == StatuEnum.Closed || value == StatusEnum.Built) //the new status was changed to FIXED or higher
        {
            //required Fields
            var ActualHoursFixed = document.getElementById("actualHourstoFix");
            var VRMajor = document.getElementById("versionReleasedMajor");
            var VRMinor = document.getElementById("versionReleasedMinor");
            
            if ((document.getElementById("actualHourstoFix").type != 'hidden')&&
                (ActualHoursFixed.value == null || ActualHoursFixed.value.trim() == "")
                ) {
                document.getElementById("ActualHoursFixError").textContent = "Actual Hours Fixed Required.";
                error_message += "- Actual Hours Fixed Required.\n";
                returnValue = false;
                result = false;
            }
            if (
                (VRMajor.value == null || VRMajor.value.trim() == "") ||
                (VRMinor.value == null || VRMinor.value.trim() == "")
               ) {
                document.getElementById("VersionReleaseError").textContent = "Release Version Required.";
                error_message += "- Release Version Required.\n";
                returnValue = false;
                result = false;
            }
        }

        if (value == StatusEnum.Closed) //the new status is CLOSED
        {
            //required Fields
            var ActualHoursTested = document.getElementById("actualHourstoTest");
            if (ActualHoursTested == null || ActualHoursTested.value.trim() == "") {
                document.getElementById("ActualHoursTestError").textContent = "Actual Hours Tested Required.";
                error_message += "- Actual Hours Tested Required.\n";
                returnValue = false;
                result = false;
            }
        }

        var details = document.getElementById("Details");
        if (((value == StatusEnum.Fixed && value != oldValue) || (value == StatusEnum.Closed && value != oldValue) || (value == StatusEnum.OnHold && value != oldValue))
            && (details.value == null || details.value.trim() == ""))
        {
            var detailsError = document.getElementById("DetailsErrorMessage");
            detailsError.textContent = "Please enter in a Resolution.";
            error_message += "- Resolution Required.\n";
            returnValue = false;
            result = false;
        }

        //the new status was put back earlier than fixed, so either IN PROCESS or SUBMITTED FROM an previous value of greater than FIXED
        /*
        if (value < 6) 
        {
            //document.getElementById("VersionReleased").value = "";
            document.getElementById("versionReleasedMajor").value = "";
            document.getElementById("versionReleasedMinor").value = "";
            document.getElementById("versionReleasedBuild").value = "";
            document.getElementById("versionReleasedMisc").value = "";
        }
        */

        if (document.getElementById("customerBug").value == "false")
        {
            if (isStringEmpty(document.getElementById("customerName").value) == false) {
                result = false;
                error_message += '- You cannot have a customer name if this is not\n';
                error_message += ' a bug reported by a customer.\n';
            }
            else
            {
                document.getElementById("customerName").value = "";
            }
            document.getElementById("customerLocation").value = "";
            document.getElementById("customerCompany").value = "";
        }
        else if (document.getElementById("customerBug").value == "true")
        {
            if (!document.getElementById("customerName").value || 0 === document.getElementById("customerName").value.length)
            {
                result = false;
                error_message += '- You must enter a customer name if this was a\n';
                error_message += ' customer reported bug.\n';
            }
        }

        //ERROR MESSAGE POP UP, valid entered information        
        if (!document.getElementById("Title").value || 0 == document.getElementById("Title").value.length)
        {
            result = false;
            error_message += '- The title cannot be blank.\n';
        }
        
        if (document.getElementById("Syteline").value.length > 0 && document.getElementById("Syteline").value.length < 10) {
            result = false;
            error_message += '- Syteline must consist of 10 characters.\n'
        }
        
        if (document.getElementById("plannedHourstoSpecify").type != 'hidden') {
            if (isStringEmpty(document.getElementById("plannedHourstoSpecify").value.trim()) == false) {
                if (isNumberFloat(document.getElementById("plannedHourstoSpecify").value.trim()) == false) {
                    result = false;
                    error_message += '- The "Planned Hours to Specify" can only be a postive  number.\n'
                }
                else if (parseFloat(document.getElementById("plannedHourstoSpecify").value.trim()) < 0) {
                    result = false;
                    error_message += '- The "Planned Hours to Specify" can only be a postive number.\n'
                }
            }
        }
        
        if (document.getElementById("plannedHourstoFix").type != 'hidden') {
            if (isStringEmpty(document.getElementById("plannedHourstoFix").value.trim()) == false) {
                if (isNumberFloat(document.getElementById("plannedHourstoFix").value.trim()) == false) {
                    result = false;
                    error_message += '- The "Planned Hours to Fix" can only be a postive number.\n';
                }
                else if (parseFloat(document.getElementById("plannedHourstoFix").value.trim()) < 0) {
                    result = false;
                    error_message += '- The "Planned Hours to Fix" can only be a postive  number.\n';
                }
            }
        }
        
        if (document.getElementById("plannedHourstoTest").type != 'hidden') {
            if (isStringEmpty(document.getElementById("plannedHourstoTest").value.trim()) == false) {
                if (isNumberFloat(document.getElementById("plannedHourstoTest").value.trim()) == false) {
                    result = false;
                    error_message += '- The "Planned Hours to Test" can only be a postive number.\n';
                }
                else if (parseFloat(document.getElementById("plannedHourstoTest").value.trim()) < 0) {
                    result = false;
                    error_message += '- The "Planned Hours to Test" can only be a postive number.\n';
                }
            }
        }
        
        if (document.getElementById("actualHourstoSpecify").type != 'hidden') {
            if (isStringEmpty(document.getElementById("actualHourstoSpecify").value.trim()) == false) {
                if (isNumberFloat(document.getElementById("actualHourstoSpecify").value.trim()) == false) {
                    result = false;
                    error_message += '- The "Actual Hours to Specify" can only be a postive  number.\n'
                }
                else if (parseFloat(document.getElementById("actualHourstoSpecify").value.trim()) < 0) {
                    result = false;
                    error_message += '- The "Actual Hours to Specify" can only be a postive  number.\n'
                }
            }
        }
        
        if (document.getElementById("actualHourstoFix").type != 'hidden') {
            if (isStringEmpty(document.getElementById("actualHourstoFix").value.trim()) == false) {
                if (isNumberFloat(document.getElementById("actualHourstoFix").value.trim()) == false) {
                    result = false;
                    error_message += '- The "Actual Hours to Fix" can only be a postive  number.\n';
                }
                else if (parseFloat(document.getElementById("actualHourstoFix").value.trim()) < 0) {
                    result = false;
                    error_message += '- The "Actual Hours to Fix" can only be a postive  number.\n';
                }
            }
        }
        
        if (document.getElementById("actualHourstoTest").type != 'hidden') {
            if (isStringEmpty(document.getElementById("actualHourstoTest").value.trim()) == false) {
                if (isNumberFloat(parseFloat(document.getElementById("actualHourstoTest").value.trim())) == false) {
                    result = false;
                    error_message += '- The "Actual Hours to Test" can only be a postive number.\n';
                }
                else if (parseFloat(document.getElementById("actualHourstoTest").value.trim()) < 0) {
                    result = false;
                    error_message += '- The "Actual Hours to Test" can only be a postive number.\n';
                }
            }
        }
        
        if (document.getElementById("assignedToId").type != 'hidden') {
            //only display the error message if the status is NOT closed
            if ((document.getElementById("assignedToId").value == -1) && (document.getElementById("statusId").value != 9)) {
                result = false;
                error_message += '- The "Assigned To" field must have a known user.\n';
            }
        }
        
        if (document.getElementById("versionPlannedMajor") != null)
        {
            if (document.getElementById("versionPlannedMajor").type != 'hidden')
            {
                // All the fields are either hidden or not.
                // regex check
                var vpmajor = document.getElementById("versionPlannedMajor").value;
                var vpminor = document.getElementById("versionPlannedMinor").value;
                var vpbuild = document.getElementById("versionPlannedBuild").value;

                if (vpmajor != "") {
                    if (isNumberInt(vpmajor) == false) {
                        result = false;
                        error_message += '- The planned release major verison can only be an integer.\n';
                    }
                }
                if (vpminor != "") {
                    if (isNumberInt(vpminor) == false) {
                        result = false;
                        error_message += '- The planned release minor version can only be an integer.\n';
                    }
                }
                if (vpbuild.length > 0) {
                    if (isNumberInt(vpbuild) == false) {
                        result = false;
                        error_message += '- The planned release build number can only be an integer.\n';
                    }
                }
            }
        }
        //some times the element is a drop list, and some times it is just a string
        var Issuelist = document.getElementById("issueType");
        var selected = -1;
        if (Issuelist.type == 'hidden')
        {
            //no control, just the value
            selected = Issuelist.value;
        }
        else
        {
            //has drop list control
            selected = parseInt(Issuelist.options[Issuelist.selectedIndex].value);
        }            
        
        if (value != StatusEnum.Closed) //NOT closing the SCR
        {
            if (selected == 2) //Enhancement
            {
                if (document.getElementById("textAreaSolution").type != 'hidden') {
                    var problem = document.getElementById("textAreaProblem").value.trim();
                    if (problem.length == 0) {
                        result = false;
                        error_message += '- The Problem / Opportunity field must be filled in.\n';
                    }
                    else if (problem.length < 10) {
                        result = false;
                        error_message += '- Please be more descriptive in Problem / Opportunity field.\n';
                    }

                    var solution = document.getElementById("textAreaSolution").value.trim();
                    if (solution.length == 0) {
                        result = false;
                        error_message += '- The Proposed Solution field must be filled in.\n';
                    }
                    else if (solution.length < 10) {
                        result = false;
                        error_message += '- Please be more descriptive in Proposed Solution field.\n';
                    }

                    var benefit = document.getElementById("textAreaBenefits").value.trim();
                    if (benefit.length == 0) {
                        result = false;
                        error_message += '- The Benefit field must be filled in.\n';
                    }
                    else if (benefit.length < 10) {
                        result = false;
                        error_message += '- Please be more descriptive in Benefit field.\n';
                    }

                    if ((problem == solution) || (problem == benefit) || (solution == benefit)) {
                        result = false;
                        error_message += '- There seems to be a some cut & paste action going on.\n';
                    }
                }
            }
            else if (selected == 3) //Planned Work
            {
                if (document.getElementById("textAreaSolution").type != 'hidden') {
                    var story = document.getElementById("textAreaSolution").value.trim();
                    if (story.length == 0) {
                        result = false;
                        error_message += '- The Story field must be filled in.\n';
                    }
                    else if (story.length < 10) {
                        result = false;
                        error_message += '- Please be more descriptive in Story field.\n';
                    }

                }
            }
        }
        // All the fields are either hidden or not, so only need to test the first field
        if (document.getElementById("versionReleasedMajor").type != 'hidden')
        {
            if (isStringEmpty(document.getElementById("versionReleasedMajor").value) == false) {
                var vrmajor = document.getElementById("versionReleasedMajor").value.trim();
                var vrminor = document.getElementById("versionReleasedMinor").value.trim();
                var vrbuild = document.getElementById("versionReleasedBuild").value.trim();

                if ((vrmajor.length > 0) && isNumberInt(vrmajor) == false) {
                    result = false;
                    error_message += '- The actual release major verison can only be an integer.\n';
                }
                else if(vrmajor.length==0 ) {
                    result = false;
                    error_message += '- The actual release major version is required.\n';
                }
                if ((vrminor.length>0) && isNumberInt(vrminor) == false) {
                    result = false;
                    error_message += '- The actual release minor version can only be an integer.\n';
                }
                else if(vrminor.length==0) {
                    result = false;
                    error_message += '- The actual release minor version is required.\n';
                }
                if ((vrbuild.length>0) && isNumberInt(vrbuild) == false) {
                    result = false;
                    error_message += '- The actual release build number can only be an integer.\n';
                }
            }
        }

        //status has changed from when it was opened
        if (document.getElementById("OldStatus").value != document.getElementById("statusId").value)
        {
            //changing the status to fixed/delivered/testing/on hold/close
            if ((oldValue <= StatusEnum.InProcess) && (value >= StatusEnum.Fixed) && (value < StatusEnum.OnHold))
            {
                if (isStringEmpty(document.getElementById("Details").value) == true) {
                    result = false;
                    error_message += '- You must enter a resolution for this SCR.\n';
                }
            }

            if (value == StatusEnum.Closed) //closing the SCR
            {
                var closedreason = document.getElementById("closedReasonId");

                if (closedreason.options[closedreason.selectedIndex] == 0)
                {
                    result = false;
                    error_message += '- You must selected a closed reason.\n';
                }
                if (isStringEmpty(document.getElementById("versionClosedMajor").value) == false)
                {
                    var vcmajor = document.getElementById("versionClosedMajor").value;
                    var vcminor = document.getElementById("versionClosedMinor").value;
                    var vcbuild = document.getElementById("versionClosedBuild").value;
                    if (vcmajor == null || isStringEmpty(vcmajor)) {
                        result = false;
                        error_message += '- The major version of "Closed Version" field cannot be blank.\n';
                    }
                    else if (isNumberInt(vcmajor) == false) {
                        result = false;
                        error_message += '- The major version of "Closed Version" field can contain only numbers.\n';
                    }
                    if (vcminor == null || isStringEmpty(vcminor)) {
                        result = false;
                        error_message += '- The minor version of "Closed Version" field cannot be blank.\n';
                    }
                    else if (isNumberInt(vcminor) == false) {
                        result = false;
                        error_message += '- The minor version of "Closed Version" field can contain only numbers.\n';
                    }
                    if (vcbuild == null || isStringEmpty(vcbuild)) {
                        result = false;
                        error_message += '- The build version of "Closed Version" field cannot be blank.\n';
                    }
                    else if (isNumberInt(vcbuild) == false) {
                        result = false;
                        error_message += '- The build number of "Closed Version" field can contain only numbers.\n';
                    }
                }
                else
                {
                    result = false;
                    error_message += '- Closed Version is Required.\n';
                }
                if (document.getElementById("actualHourstoTest").type != 'hidden') {
                    if (isStringEmpty(document.getElementById("actualHourstoTest").value) == true) {
                        result = false;
                        error_message += '- The "Actual Hours to Test" field cannot be blank.\n';
                    }
                }
                if (document.getElementById("AccessType").value == 4) {
                    var build = document.getElementById("versionClosedBuild").value;
                    
                    if (isStringEmpty(build) == true) {
                        result = false;
                        error_message += '- The build version of "Closed Version" field cannot be blank.\n';
                    }
                }
            }

            //check if translate to new variables
            if (((oldValue <= StatusEnum.InProcess) && (value >= StatusEnum.Fixed)) ||
                 ((oldValue >= StatusEnum.Fixed) && (value >= StatusEnum.Fixed)))
            {
                // Check to see if things are in place
                if (document.getElementById("actualHourstoFix").type != 'hidden') {
                    if (isStringEmpty(document.getElementById("actualHourstoFix").value.trim()) == true) {
                        result = false;
                        error_message += '- The "Actual Hours to Fix" field cannot be blank.\n';
                    }
                }
            }
            else if (((oldValue <= StatusEnum.InProcess) && (value <= StatusEnum.InProcess)) ||
                        ((oldValue.value >= StatusEnum.Fixed) && (value <= StatusEnum.InProcess)))
            {
                // Must be blank.
                if (document.getElementById("versionReleasedMajor").type != 'hidden') {
                    if (isStringEmpty(document.getElementById("versionReleasedMajor").value.trim()) == false) {
                        result = false;
                        error_message += '- The major version of "Actual Release" can only be entered if the SCR is fixed.\n';
                    }
                }
            }
        }
        // If the user is a member of QC, they need the final build.
        if (document.getElementById("versionReleasedMajor").type != 'hidden' && value >= StatusEnum.Fixed)
        {
            //var VR = document.getElementById("VersionReleased").value.split('.');
            var VRmajor = document.getElementById("versionReleasedMajor");
            var VRminor = document.getElementById("versionReleasedMinor");
            if (VRmajor == null || VRminor == null || isStringEmpty(VRmajor) == true || isStringEmpty(VRminor) == true ) {
                result = false;
                error_message += '- The major and minor version of "Actual Release Version" cannot be blank.\n';
            }
        }
        if (document.getElementById("Rank").type != 'hidden')
        {
            if (isStringEmpty(document.getElementById("Rank").value.trim()) == false) {
                if (isNumberInt(document.getElementById("Rank").value.trim()) == false) {
                    result = false;
                    error_message += '- The "Rank" can only be a postive number.\n'
                }
                else if (parseInt(document.getElementById("Rank").value.trim()) < 0) {
                    result = false;
                    error_message += '- The "Rank" can only be a postive number.\n'
                }
            }
        }
        if (document.getElementById("Size").type != 'hidden')
        {
            if (isStringEmpty(document.getElementById("Size").value.trim()) == false) {
                if (isNumberInt(document.getElementById("Size").value.trim()) == false) {
                    result = false;
                    error_message += '- The "Size" can only be a postive number.\n'
                }
                else if (parseInt(document.getElementById("Size").value.trim()) < 0) {
                    result = false;
                    error_message += '- The "Size" can only be a postive number.\n'
                }
            }
        }

        if (!IsValidUpload())
        {
            result = false;
            error_message += '- Attachment was never uploaded.\n'
        }

        if (result == false)
        {
            alert(error_message);
        }
        return returnValue && result;
    }
    return false;
}

function OnChangeVersionPlanned()
{
    var vpmajor = document.getElementById("versionPlannedMajor").value.trim();
    var vpminor = document.getElementById("versionPlannedMinor").value.trim();
    var vpbuild = document.getElementById("versionPlannedBuild").value.trim();

    var error = document.getElementById("VersionPlannedError");
    
    var valid = true;

    if (vpmajor != "") {
        if (isNumberInt(vpmajor) == false || vpmajor <0) {
            valid = false;
        }
    }
    if (vpminor != "") {
        if (isNumberInt(vpminor) == false || vpminor < 0) {
            valid = false;
        }
    }
    if (vpbuild != "") {
        if (isNumberInt(vpbuild) == false || vpbuild< 0) {
            valid = false;
        }
    }

    if (!valid) {
        error.textContent = "Planned Version can only be non-negative integers, or blank";
    } else
    {
        error.textContent = '';
    }
    return valid;
}

function OnChangeVersionReleased() {
    var vrmajor = document.getElementById("versionReleasedMajor").value.trim();
    var vrminor = document.getElementById("versionReleasedMinor").value.trim();
    var vrbuild = document.getElementById("versionReleasedBuild").value.trim();

    var error = document.getElementById("VersionReleaseError");

    var valid = true;

    if (vrmajor != "") {
        if (isNumberInt(vrmajor) == false || vrmajor < 0) {
            valid = false;
        }
    }
    if (vrminor != "") {
        if (isNumberInt(vrminor) == false || vrminor < 0) {
            valid = false;
        }
    }
    if (vrbuild != "") {
        if (isNumberInt(vrbuild) == false || vrbuild < 0) {
            valid = false;
        }
    }

    if (!valid) {
        error.textContent = "Released Version can only be non-negative integers";
    } else {
        error.textContent = '';
    }
    return valid;
}

function OnChangeVersionClosed() {
    var vrmajor = document.getElementById("versionClosedMajor").value.trim();
    var vrminor = document.getElementById("versionClosedMinor").value.trim();
    var vrbuild = document.getElementById("versionClosedBuild").value.trim();

    var error = document.getElementById("VersionClosedError");

    var valid = true;

    if (vrmajor != "") {
        if (isNumberInt(vrmajor) == false || vrmajor < 0) {
            valid = false;
        }
    }
    if (vrminor != "") {
        if (isNumberInt(vrminor) == false || vrminor < 0) {
            valid = false;
        }
    }
    if (vrbuild != "") {
        if (isNumberInt(vrbuild) == false || vrbuild < 0) {
            valid = false;
        }
    }

    if (!valid) {
        error.textContent = "Closed Version can only be non-negative integers";
    } else {
        error.textContent = '';
    }
    return valid;
}




    // isNumberInt - checked to see if the value entered is a legal int
    function isNumberInt(inputString) {
        return ((!isNaN(parseInt(inputString))) ? true : false) &&
            (inputString % 1 === 0)
        ;
    }

    // isNumberFlaot - checks to see if the number is a float
    function isNumberFloat(inputString) {
        return (!isNaN(parseFloat(inputString))) ? true : false;
    }

    function ValidateDouble(inputElement, error)
    {
        var value = document.getElementById("statusId").options[statusId.selectedIndex].value;
        if (value >= 6) {
            //required Fields
            var ActualHoursFixed = document.getElementById("actualHourstoFix");
            var VersionReleaseMajor = document.getElementById("versionReleasedMajor");
            var VersionReleaseMinor = document.getElementById("versionReleasedMinor");
            if ((document.getElementById("actualHourstoFix").type != 'hidden')&&
                (ActualHoursFixed.value == null || ActualHoursFixed.value.trim() == "")) {
                document.getElementById("ActualHoursFixError").textContent = "Actual Hours Fixed Required.";
            }
            else {
                document.getElementById("ActualHoursFixError").textContent = "";
            }
            if (VersionReleaseMinor.value == null || VersionReleaseMinor.value.trim() == "" ||
                VersionReleaseMajor.value == null || VersionReleaseMajor.value.trim() == ""
                ) {
                document.getElementById("VersionReleaseError").textContent = "Release Version Required.";
            }
            else {
                document.getElementById("VersionReleaseError").textContent = "";
            }
        }
        else
        {
            document.getElementById("VersionReleaseError").textContent = "";
            document.getElementById("ActualHoursFixError").textContent = "";
        }
        if (value == 9) {
            //required Fields
            var ActualHoursTested = document.getElementById("actualHourstoTest");
            if (ActualHoursTested == null || ActualHoursTested.value.trim() == "") {
                document.getElementById("ActualHoursTestError").textContent = "Actual Hours Tested Required.";
            }
            else {
                document.getElementById("ActualHoursTestError").textContent = "";
            }
        }
        else
        {
            document.getElementById("ActualHoursTestError").textContent = "";
        }
    }

    //Upload button 
    function ValidateEditUpload()
    {
        var file = document.getElementById("File_0_");
        var fileValue = file.value;
        var description = document.getElementById("FileDescription");
        var errorMsg = document.getElementById("UploadErrorMessage");
        var uploadButton = document.getElementById("UploadBtn");
        //var MAX_SIZE_BYTES = (100 * 1000000); //100 MB maximum size

        if(fileValue == "")
        {
            //File missing
            errorMsg.textContent = "File is missing.";
            uploadButton.disabled = true;
        }
        /*
        else if(file.size > MAX_SIZE_BYTES)
        {
            //File is too Large
            alert("check");
            errorMsg.textContent = "File is too large. File limit is ~100 MB";
            uploadButton.disabled = true;
        }*/
        else if (description.value == null || description.value.trim() == "") {
            //Description is empty
            errorMsg.textContent = "File description is missing.";
            uploadButton.disabled = true;
        }
        else
        {
            errorMsg.textContent = "";
            uploadButton.removeAttribute("disabled");
            uploadButton.disabled = false;
        }

    }

    function IsValidUpload()
    {
        var isValid = true;
        var file = document.getElementById("File_0_");
        var fileValue = file.value;
        if (fileValue != "")
        {
            //There is file in the edit box, which means the user didnt hit upload
            isValid = false;
        }
        return isValid;
    }
    //add and corrolate in EDIT
    function ValidateRelated()
    {
        var isValid = false;
        var input = document.getElementById("RelatedInputID").value.trim();
        var addButton = document.getElementById("AddButton");
        var corrolateButton = document.getElementById("CorrolateButton");
        
        if (input.length > 0)
        {
            if (!isNaN(input))
            {
                addButton.removeAttribute("disabled");
                corrolateButton.removeAttribute("disabled");
                document.getElementById("RelatedInputID").value = input; //pu the trimmed value back in so the validation passes
                isValid = true;
            }
        }

        if(!isValid)
        {
            addButton.disabled = true;
            corrolateButton.disabled = true;
        }
        return isValid;
    }
