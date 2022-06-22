function resetForm(form) {
    // clearing inputs
    document.getElementById("TitleText").value = '';
    document.getElementById("SCRNumber").value = '';
    document.getElementById("SCRNumber2").value = '';

    var inputs = form.getElementsByTagName('input');
    if (typeof inputs != "undefined")
    {
        for (var i = 0; i < inputs.length; i++) {
            switch (inputs[i].type) {
                // case 'hidden':
                case 'text':
                    inputs[i].value = '';
                    break;
                case 'radio':
                case 'checkbox':
                    inputs[i].checked = false;
            }
        }
    }
    
    // clearing selects
    var selects = form.getElementsByTagName('select');
    for (var i = 0; i < selects.length; i++) {
        selects[i].selectedIndex = 0;
    }
    // clearing textarea
    var text = form.getElementsByTagName('textarea');
    for (var i = 0; i < text.length; i++) {
        text[i].innerHTML = '';
        text[i].value ='';
    }
    var RightSelectList = document.getElementById("RightSelectTag");
    var LeftSelectList = document.getElementById("LeftSelectTag");
    var RightSelectListSortBy = document.getElementById("RightSelectTagSort");
    var LeftSelectListSortBy = document.getElementById("LeftSelectTagSort");
    var RightSelectListProduct = document.getElementById("RightSelectTagProducts");
    var LeftSelectListProduct = document.getElementById("LeftSelectTagProducts");
    selectAll(RightSelectList);
    selectAll(RightSelectListSortBy);
    selectAll(RightSelectListProduct);
    SearchSelectMoveRows(RightSelectList, LeftSelectList);
    SearchSelectMoveRows(RightSelectListSortBy, LeftSelectListSortBy);
    SearchSelectMoveRows(RightSelectListProduct, LeftSelectListProduct);
    ChangeReported();
    document.getElementById("MainContent").innerHTML = "";

    return false;
}
function selectAll( selectList )
{
    for (var i = 0; i < selectList.options.length;i++ )
    {
        selectList.options[i].selected = true;
    }
}
function SCRNumberReveal()
{
    var compare = document.getElementById("SCRComparator");
    var selected = compare.options[compare.selectedIndex].value;
    var scr2 = document.getElementById("SCRNumber2");
    if (selected == 4) {
        scr2.style.backgroundColor = "white";
        scr2.readOnly = false;
    }
    else
    {
        scr2.value = "";
        scr2.style.backgroundColor = "#C4C4A7";
        scr2.readOnly = true;
    }
}
function LegacySCRNumberReveal()
{//ToDo: Dave we need to redo this when we get to refactoring
    var compare = document.getElementById("LegacySCRComparator");
    var selected = compare.options[compare.selectedIndex].value;
    var scr2 = document.getElementById("LegacySCRNumber2");
    if (selected == 4) {
        scr2.style.backgroundColor = "white";
        scr2.readOnly = false;
    }
    else {
        scr2.value = "";
        scr2.style.backgroundColor = "#C4C4A7";
        scr2.readOnly = true;
    }
}
function SearchSelectMoveRows(SS1, SS2) {
    var SelID = '';
    var SelText = '';
    // Move rows from SS1 to SS2 from bottom to top
    for (i = 0; i < SS1.options.length; i++) {
        //gets the indext of a selected item
        if (SS1.options[i].selected == true) {
            //gets the value and Text
            SelID = SS1.options[i].value;
            SelText = SS1.options[i].text;
            if (SS2 == document.getElementById("RightSelectTagSort")) {
                SelText = "↑" + SelText;
            }
            if (SS2 == document.getElementById("LeftSelectTagSort")) {
                SelText = SelText.replace("↑", "");
                SelText = SelText.replace("↓", "");
                SelID = SelID.replace(" DESC", " ASC");
               
            }
            //creates a new option
            var newRow = new Option(SelText, SelID);
            if (SS1.options[i].hasAttribute("ondblclick")) {
                if (SS1 == document.getElementById("RightSelectTagProducts")) {
                    newRow.setAttribute("ondblclick", "javascript: SearchSelectMoveRows(form.LeftSelectTagProducts, form.RightSelectTagProducts)");
                }
                else
                {
                    newRow.setAttribute("ondblclick",   "javascript: SearchSelectMoveRows(form.RightSelectTagProducts, form.LeftSelectTagProducts)");
                }
            }
            //places the new object in the new box's index
            SS2.options[SS2.length] = newRow;
            //delete the old object
        }
    }
    for (i = SS1.options.length - 1; i >= 0; i--) {
        if (SS1.options[i].selected == true) {
            SS1.options[i] = null;
        }
    }
    //Sort the list
    //SelectSort(SS2);
}
function SelectSort(SelList) {
    var ID = '';
    var Text = '';
    for (x = 0; x < SelList.length - 1; x++) {
        for (y = x + 1; y < SelList.length; y++) {
            if (SelList[x].parentNode.label == SelList[y].parentNode.label
                && SelList[x].text > SelList[y].text) {
                // Swap rows
                ID = SelList[x].value;
                Text = SelList[x].text;
                SelList[x].value = SelList[y].value;
                SelList[x].text = SelList[y].text;
                SelList[y].value = ID;
                SelList[y].text = Text;
            }
        }
    }
}
function MoveUpDisplay()
{
    var selected = $("#RightSelectTag").find(":selected");
    var before = selected.prev();
    if (before.length > 0)
        selected.detach().insertBefore(before);
}
function MoveDownDisplay() {
    var selected = $("#RightSelectTag").find(":selected");
    var next = selected.next();
    if (next.length > 0)
        selected.detach().insertAfter(next);
}
function MoveUpSort() {
    var selected = $("#RightSelectTagSort").find(":selected");
    var before = selected.prev();
    if (before.length > 0)
        selected.detach().insertBefore(before);
}
function MoveDownSort()
{
    var selected = $("#RightSelectTagSort").find(":selected");
    var next = selected.next();
    if (next.length > 0)
        selected.detach().insertAfter(next);
}
function autoFillHiddenString(HiddenString, SelectList)
{
    HiddenString.value = "";
    if( SelectList != null )
    {
        for (var i = 0; i < SelectList.length;i++ )
        {
            if (SelectList[i].value != null && SelectList[i].value != "")
            {
                if (i != 0)
                {
                    HiddenString.value += ", ";
                }
                HiddenString.value += SelectList[i].value;
            }
        }
    }
}

function Alternate()
{
    var SelectList = document.getElementById("RightSelectTagSort");
    for (var i = 0; i < SelectList.length; i++)
    {
        if( SelectList[i].selected == true)
        {
            if( SelectList[i].value.indexOf(" ASC")!=-1)
            {
                SelectList[i].value = SelectList[i].value.replace(" ASC", " DESC");
                SelectList[i].text = SelectList[i].text.replace("↑", "↓");
            }
            else if (SelectList[i].value.indexOf(" DESC")!=-1)
            {
                SelectList[i].value = SelectList[i].value.replace(" DESC", " ASC");
                SelectList[i].text = SelectList[i].text.replace("↓", "↑");
            }
        }
    }
}
function IssueType(IssueTypeName) {
    var Output = "";
    if (IssueTypeName == "3" || IssueTypeName == 3) {
        Output = "<div style='text-align:center;'><i class='fa fa-tasks fa-2x'></i></div>";
    }
    else if (IssueTypeName == "2" || IssueTypeName == 2) {
        Output = "<div style='text-align:center;'><i class='fa fa-plus-circle fa-2x'></i></div>";
    }
    else if (IssueTypeName == "1" || IssueTypeName == 1) {
        Output = "<div style='text-align:center;'><i class='fa fa-bug fa-2x'></i></div>";
    }
    else {
        Output = IssueTypeName;
    }
    return Output;
}
function PriorityFunc(PriorityID) {
    var output = "";
    if (PriorityID == "4") {
        output = "<div style='text-align:center;'><i class=' glyphicon glyphicon-fire fa-2x qvi-color-critical'></i></div>";
    }
    else if (PriorityID == "3") {
        output = "<div style='text-align:center;'><i class=' fa fa-fire fa-2x qvi-color-high'></i></div>";
    }
    else if (PriorityID == "2") {
        output = "<div style='text-align:center;'><i class=' fa fa-exclamation-triangle fa-2x qvi-color-medium'></i></div>";
    }
    else if (PriorityID == "1") {
        output = "<div style='text-align:center;'><i class=' fa fa-info-circle fa-2x qvi-color-low'></i></div>";
    }
    return output;
}
function populatePrint() {
    var Print = "<tr>";
    var table = document.getElementById("Table");
    if(table.rows.length != 0)
    {
        for (var i = 0; i < table.rows[0].cells.length; i++) {
            Print += "<th>" + table.rows[0].cells[i].headers + "</th>";
        }
        Print += "</tr>";
        document.getElementById('grid1print').innerHTML = (Print + $("#Table").html());
    }
}
function PrintMe() {
    populatePrint();
    window.print();
    //workaround for Chrome bug - https://code.google.com/p/chromium/issues/detail?id=141633
    if (window.stop) {
        location.reload(); //triggering unload (e.g. reloading the page) makes the print dialog appear
        window.stop(); //immediately stop reloading
    }
    else if(location.reload)
    {
        window.stop();
    }
    return false;
}
function ChangeReported()
{
    var Reported = document.getElementById("CustomerReported")
    var SelectedReported = Reported.options[Reported.selectedIndex].value;
    var Company = document.getElementById("CustomerCompany");
    var Location = document.getElementById("CustomerLocation");
    var Contact = document.getElementById("CustomerContact");
    //var SyteLine = document.getElementById("CustomerSyteLine");
    //var MachineSN = document.getElementById("CustomerMachineSN");
    if (SelectedReported == 1)
    {
        Company.style.backgroundColor = "white";
        Location.style.backgroundColor = "white";
        Contact.style.backgroundColor = "white";
        //SyteLine.style.backgroundColor = "white";
        //MachineSN.style.backgroundColor = "white";
        Company.readOnly = false;
        Location.readOnly = false;
        Contact.readOnly = false;
        //SyteLine.readOnly = false;
        // MachineSN.readOnly = false;
    }
    else
    {
        Company.value = "";
        Location.value = "";
        Contact.value = "";
        //SyteLine.value = "";
        //MachineSN.value = "";
        Company.style.backgroundColor = "#C4C4A7";
        Location.style.backgroundColor = "#C4C4A7";
        Contact.style.backgroundColor = "#C4C4A7";
        //SyteLine.style.backgroundColor = "#C4C4A7";
        //MachineSN.style.backgroundColor = "#C4C4A7";
        Company.readOnly = true;
        Location.readOnly = true;
        Contact.readOnly = true;
        //SyteLine.readOnly = true;
        //MachineSN.readOnly = true;
    }
}
function validate(isSave)
{
    var AlertMessage = "";
    //Check if exisits
    if($('#MaxRows').length)
    {
        if (!isNormalInteger(document.getElementById("MaxRows").value))
        {
            AlertMessage += '-The Max Rows area must be a integer.\n';
        }
    }
    if (
        (document.getElementById("NameOfReport") != null &&
        (document.getElementById("NameOfReport").value == null ||
        document.getElementById("NameOfReport").value.trim() == ""))&&
        true == isSave
        ) {
        AlertMessage += '-The Title of the report cannot be left blank.\n';
    }
    //Static Feilds
    // Ints only
    var SCRNum = document.getElementById("SCRNumber").value;
    if (!isNormalInteger(SCRNum))
    {
        AlertMessage += '-The first SCR Number has to be a integer.\n';
    }
    if (document.getElementById("SCRNumber2").getAttribute('disabled') == false)
    {
        var SCRNUm2 = document.getElementById("SCRNumber2").value;
        if (!isNormalInteger(SCRNum2)) {
            AlertMessage += '-The second SCR Number has to be a postive integer.\n';
        }
    }
    var Rank = document.getElementById("Rank").value.trim();
    if (isStringEmpty(Rank) == false) {
        if (isNumberInt(Rank) == false) {
            AlertMessage += '- The "Rank" can only be a postive number.\n'
        }
        else if (parseInt(Rank) <= 0) {
            AlertMessage += '- The "Rank" can only be a postive number.\n'
        }
    }
    var Size = document.getElementById("Size").value.trim();
    if (isStringEmpty(Size) == false) {
        if (isNumberInt(Size) == false) {
            AlertMessage += '- The "Size" can only be a postive number.\n'
        }
        else if (parseInt(Size) <= 0) {
            AlertMessage += '- The "Size" can only be a postive number.\n'
        }
    }

    //Versions
    var VFMajor = document.getElementById("VersionFound.Major").value;
    if (!isNormalInteger(VFMajor))
    {
        AlertMessage += '-The Major in Version Found must be a postive integer.\n';
    }
    var VFMinor = document.getElementById("VersionFound.Minor").value;
    if (!isNormalInteger(VFMinor)) {
        AlertMessage += '-The Minor in Version Found must be a postive integer.\n';
    }
    var VFBuild = document.getElementById("VersionFound.Build").value;
    if (!isNormalInteger(VFBuild)) {
        AlertMessage += '-The Build in Version Found must be a postive integer.\n';
    }
        
    var VPMajor = document.getElementById("VersionPlanned.Major").value;
    if (!isNormalInteger(VPMajor)) {
        AlertMessage += '-The Major in Version Planned must be a postive integer.\n';
    }
    var VPMinor = document.getElementById("VersionPlanned.Minor").value;
    if (!isNormalInteger(VPMinor)) {
        AlertMessage += '-The Minor in Version Planned must be a postive integer.\n';
    }
    var VPBuild = document.getElementById("VersionPlanned.Build").value;
    if (!isNormalInteger(VPBuild)) {
        AlertMessage += '-The Build in Version Planned must be a postive integer.\n';
    }
    var VRMajor = document.getElementById("VersionResolved.Major").value;
    if (!isNormalInteger(VRMajor)) {
        AlertMessage += '-The Major in Version Resolved must be a postive integer.\n';
    }
    var VRMinor = document.getElementById("VersionResolved.Minor").value;
    if (!isNormalInteger(VRMinor)) {
        AlertMessage += '-The Minor in Version Resolved must be a postive integer.\n';
    }
    var VRBuild = document.getElementById("VersionResolved.Build").value;
    if (!isNormalInteger(VRBuild)) {
        AlertMessage += '-The Build in Version Resolved must be a postive integer.\n';
    }
    var VCMajor = document.getElementById("VersionClosed.Major").value;
    if (!isNormalInteger(VCMajor)) {
        AlertMessage += '-The Major in Version Closed must be a postive integer.\n';
    }
    var VCMinor = document.getElementById("VersionClosed.Minor").value;
    if (!isNormalInteger(VCMinor)) {
        AlertMessage += '-The Minor in Version Closed must be a postive integer.\n';
    }
    var VCBuild = document.getElementById("VersionClosed.Build").value;
    if (!isNormalInteger(VCBuild)) {
        AlertMessage += '-The Build in Version Closed must be a postive integer.\n';
    }

    //Dates
    var SubmittedStartYear = document.getElementById("SubmittedStartDate.Year").value;
    if (!isNormalInteger(SubmittedStartYear)) {
        AlertMessage += '-The Year in Submitted Start must be a postive integer.\n';
    }
    var SubmittedStartDay = document.getElementById("SubmittedStartDate.Day").value;
    if (!isNormalInteger(SubmittedStartDay)) {
        AlertMessage += '-The Day in Submitted Start must be a postive integer.\n';
    }
    var SubmittedStartMonth = document.getElementById("SubmittedStartDate.Month").value;
    if (!isNormalInteger(SubmittedStartMonth)) {
        AlertMessage += '-The Month in Submitted Start must be a postive integer.\n';
    }

    var SubmittedEndYear = document.getElementById("SubmittedEndDate.Year").value;
    if (!isNormalInteger(SubmittedEndYear)) {
        AlertMessage += '-The Year in Submitted End must be a postive integer.\n';
    }
    var SubmittedEndDay = document.getElementById("SubmittedEndDate.Day").value;
    if (!isNormalInteger(SubmittedEndDay)) {
        AlertMessage += '-The Day in Submitted End must be a postive integer.\n';
    }
    var SubmittedEndMonth = document.getElementById("SubmittedEndDate.Month").value;
    if (!isNormalInteger(SubmittedEndMonth)) {
        AlertMessage += '-The Month in Submitted End must be a postive integer.\n';
    }

    var AssignedStartYear = document.getElementById("AssignedToStartDate.Year").value;
    if (!isNormalInteger(AssignedStartYear)) {
        AlertMessage += '-The Year in Assigned to Start must be a postive integer.\n';
    }
    var AssignedStartDay = document.getElementById("AssignedToStartDate.Day").value;
    if (!isNormalInteger(AssignedStartDay)) {
        AlertMessage += '-The Day in Assigned to Start must be a postive integer.\n';
    }
    var AssignedStartMonth = document.getElementById("AssignedToStartDate.Month").value;
    if (!isNormalInteger(AssignedStartMonth)) {
        AlertMessage += '-The Month in Assigned to Start must be a postive integer.\n';
    }

    var AssignedEndYear = document.getElementById("AssignedToEndDate.Year").value;
    if (!isNormalInteger(AssignedEndYear)) {
        AlertMessage += '-The Year in Assigned to End must be a postive integer.\n';
    }
    var AssignedEndDay = document.getElementById("AssignedToEndDate.Day").value;
    if (!isNormalInteger(AssignedEndDay))
    {
        AlertMessage += '-The Day in Assigned to End must be a postive integer.\n';
    }
    var AssignedEndMonth = document.getElementById("AssignedToEndDate.Month").value;
    if (!isNormalInteger(AssignedEndMonth)) {
        AlertMessage += '-The Month in Assigned to End must be a postive integer.\n';
    }

    var ResolvedStartYear = document.getElementById("ResolvedStartDate.Year").value;
    if (!isNormalInteger(ResolvedStartYear)) {
        AlertMessage += '-The Year in Resolved on Start must be a postive integer.\n';
    }
    var ResolvedStartDay = document.getElementById("ResolvedStartDate.Day").value;
    if (!isNormalInteger(ResolvedStartDay)) {
        AlertMessage += '-The Day in Resolved on Startmust be a postive integer.\n';
    }
    var ResovledStartMonth = document.getElementById("ResolvedStartDate.Month").value;
    if (!isNormalInteger(ResovledStartMonth)) {
        AlertMessage += '-The Month in Resolved on Start must be a number.\n';
    }

    var ResolvedEndYear = document.getElementById("ResolvedEndDate.Year").value;
    if (!isNormalInteger(ResolvedEndYear)) {
        AlertMessage += '-The Year in Resolved on End must be a postive integer.\n';
    }
    var ResolvedEndDay = document.getElementById("ResolvedEndDate.Day").value;
    if (!isNormalInteger(ResolvedEndDay)) {
        AlertMessage += '-The Day in Resolved on End must be a postive integer.\n';
    }
    var ResolvedEndMonth = document.getElementById("ResolvedEndDate.Month").value;
    if (!isNormalInteger(ResolvedEndMonth)) {
        AlertMessage += '-The Month in Resolved on End must be a postive integer.\n';
    }

    var ClosedStartYear = document.getElementById("ClosedStartDate.Year").value;
    if (!isNormalInteger(ClosedStartYear)) {
        AlertMessage += '-The Year in Closed on Start must be a postive integer.\n';
    }
    var ClosedStartDay = document.getElementById("ClosedStartDate.Day").value;
    if (!isNormalInteger(ClosedStartDay)) {
        AlertMessage += '-The Day in Closed on Start must be a postive integer.\n';
    }
    var ClosedStartMonth = document.getElementById("ClosedStartDate.Month").value;
    if (!isNormalInteger(ClosedStartMonth)) {
        AlertMessage += '-The Month in Closed on Start must be a postive integer.\n';
    }

    var ClosedEndYear = document.getElementById("ClosedEndDate.Year").value;
    if (!isNormalInteger(ClosedEndYear)) {
        AlertMessage += '-The Year in Closed on End must be a postive integer.\n';
    }
    var ClosedEndDay = document.getElementById("ClosedEndDate.Day").value;
    if (!isNormalInteger(ClosedEndDay)) {
        AlertMessage += '-The Day in Closed on End must be a postive integer.\n';
    }
    var ClosedEndMonth = document.getElementById("ClosedEndDate.Month").value;
    if (!isNormalInteger(ClosedEndMonth)) {
        AlertMessage += '-The Month in Closed on End must be a postive integer.\n';
    }
    var ModStartYear = document.getElementById("ModifiedAfter.Year").value;
    if (!isNormalInteger(ModStartYear)) {
        AlertMessage += '-The Year in Closed on After must be a postive integer.\n';
    }
    var ModStartDay = document.getElementById("ModifiedAfter.Day").value;
    if (!isNormalInteger(ModStartDay)) {
        AlertMessage += '-The Day in Modified on After must be a postive integer.\n';
    }
    var ModStartMonth = document.getElementById("ModifiedAfter.Month").value;
    if (!isNormalInteger(ModStartMonth)) {
        AlertMessage += '-The Month in Modified After must be a postive integer.\n';
    }
    var ModEndYear = document.getElementById("ModifiedBefore.Year").value;
    if (!isNormalInteger(ModEndYear)) {
        AlertMessage += '-The Year in Modified Before must be a postive integer.\n';
    }
    var ModEndDay = document.getElementById("ModifiedBefore.Day").value;
    if (!isNormalInteger(ModEndDay)) {
        AlertMessage += '-The Day in Modified Before must be a postive integer.\n';
    }
    var ModEndMonth = document.getElementById("ModifiedBefore.Month").value;
    if (!isNormalInteger(ModEndMonth)) {
        AlertMessage += '-The Month in Modified Before must be a postive integer.\n';
    }
    if(!AlertMessage)
    {
        return true;
    }
    alert(AlertMessage);
    return false;
}
function isNormalInteger(str) {
    var n = ~~Number(str);
    return (str == "") ||(parseFloat(n) === n >>> 0);
}

function isStringEmpty(str) {
    return (!str || /^\s*$/.test(str)) || (!str || 0 === str.length);
}

function isNumberInt(inputString) {
    return ((!isNaN(parseInt(inputString))) ? true : false) &&
        (inputString % 1 === 0)
    ;
}

