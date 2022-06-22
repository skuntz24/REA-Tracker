function ReportSelectMoveRows(SS1, SS2) {
    var SelID = '';
    var SelText = '';
    // Move rows from SS1 to SS2 from bottom to top
    for (i = 0; i < SS1.options.length; i++) {
        //gets the indext of a selected item
        if (SS1.options[i].selected == true) {
            //gets the value and Text
            SelID = SS1.options[i].value;
            SelText = SS1.options[i].text;
            if (!(SS1.id.toLowerCase() == 'rightselecttagproducts'
                && SelID == '-1')
                ) {
                //creates a new option
                var newRow = new Option(SelText, SelID);
                //places the new object in the new box's index
                SS2.options[SS2.length] = newRow;
                //delete the old object
            }
        }
    }
    for (i = SS1.options.length - 1; i >= 0; i--) {
        if (SS1.options[i].selected == true &&
            !(SS1.id.toLowerCase() == 'leftselecttagproducts' && SelID == '-1')
            ) {
            SS1.options[i] = null;
        }
    }
    //Sort the list
    //SelectSort(SS2);
}
function MoveUpDisplay() {
    var selected = $("#RightSelectTagProducts").find(":selected");
    var before = selected.prev();
    if (before.length > 0)
        selected.detach().insertBefore(before);
}
function MoveDownDisplay() {
    var selected = $("#RightSelectTagProducts").find(":selected");
    var next = selected.next();
    if (next.length > 0)
        selected.detach().insertAfter(next);
}
function autoFillHiddenString(HiddenString, SelectList) {
    HiddenString.value = "";
    if (SelectList != null) {
        for (var i = 0; i < SelectList.length; i++) {
            if (SelectList[i].value != null && SelectList[i].value != "") {
                if (i != 0) {
                    HiddenString.value += ", ";
                }
                HiddenString.value += SelectList[i].value;
            }
        }
    }
}