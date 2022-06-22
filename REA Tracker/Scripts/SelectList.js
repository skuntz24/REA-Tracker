//------------------SelectList dropdown for Edit Users------------------------
function SelectMoveRows(SS1, SS2) {
    var SelID = '';
    var SelText = '';
    // Move rows from SS1 to SS2 from bottom to top
    for (i = SS1.options.length - 1; i >= 0; i--) {
        //gets the indext of a selected item
        if (SS1.options[i].selected == true) {
            //gets the value and Text
            SelID = SS1.options[i].value;
            SelText = SS1.options[i].text;
            //creates a new option
            var newRow = new Option(SelText, SelID);
            //places the new object in the new box's index
            //SS2.options[SS2.length] = newRow;
            if (SS1.options[i].parentNode.label == 'Active') {
                //add it to the index of the corosponding label
                var group = SS2.getElementsByTagName('optgroup');
                group[0].appendChild(newRow);
            }
            else {
                //add it to the index of the corosponding label
                var group = SS2.getElementsByTagName('optgroup');
                group[1].appendChild(newRow);
            }
            //delete the old object
            SS1.options[i] = null;
        }
    }
    //Sort the list
    SelectSort(SS2);
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
//=============================Autofill Hidden Field===========================
function autoFillHiddenFullAccess(fullaccess, noaccess, access) {
    //if (true == fullaccess.checked) {
    //    document.getElementById("hiddenString").value = "";
    //    if (access != null && noaccess != null) {
    //        for (var i = 0; i < access.length; i++) {
    //            if(i !=0)
    //            {
    //                document.getElementById("hiddenString").value += " ";
    //            }
    //            document.getElementById("hiddenString").value += access[i].value;
    //        }
    //        for (var i = 0; i < noaccess.length; i++) {
    //            if (document.getElementById("hiddenString").value != null && document.getElementById("hiddenString").value != "") 
    //            {
    //                document.getElementById("hiddenString").value += " ";
    //            }
    //            document.getElementById("hiddenString").value += noaccess[i].value;
    //        }
    //    }
    //}
    //else
    //{
        autoFillHidden(access);
    //}
}

//------------------CLears Acess field (giveAccess) and (noAccess)-------------
function clearAccessList() {
    for (var i = 0; i < document.getElementById("giveAccess").length ; i++) {
        document.getElementById("giveAccess")[i].selected = true;
    }
    SelectMoveRows(document.getElementById("giveAccess"), document.getElementById("NoAccess"));
}

function autoFillHidden(access) {
    document.getElementById("hiddenString").value = "";
    if (access != null) {
        for (var i = 0; i < access.length; i++) {
            if( i != 0 )
            {
                document.getElementById("hiddenString").value += " ";
            }
            document.getElementById("hiddenString").value += access[i].value;
        }
    }
}

function popUp(URL) {
    day = new Date();
    id = day.getTime();
    eval("page" + id + " = window.open(URL, '" + id + "', 'toolbar=0,scrollbars=0,location=0,statusbar=0,menubar=0,resizable=0,width=640,height=480,left = 600,top = 480');");
}