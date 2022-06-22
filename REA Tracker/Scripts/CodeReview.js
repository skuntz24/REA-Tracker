function isStringEmpty(str) {
    return (!str || /^\s*$/.test(str)) || (!str || 0 === str.length);
}

//Create Code Review
function openCodeReviewCreate() {
    $('#createCodeReview').modal('show');
}

function updateCodeReview(x) {
    var title = document.getElementById("CodeReviewTitle-" + x).innerText;
    document.getElementById("EditCodeReviewTitle").value = title;
    document.getElementById("EditCodeLocation").value = document.getElementById("CodeLocation-" + x).innerText;
    document.getElementById("EditCodeReviewNotes").innerText = document.getElementById("CodeReviewNotes-" + x).innerText;
    document.getElementById("fileNamesTwo").innerText = document.getElementById("CodeReviewFiles-" + x).innerText;
    var Id = document.getElementById("CodeReviewId-" + x).innerHTML;
    document.getElementById("EditCodeReviewId").innerText = Id;
    //document.getElementById("#EditCodeReviewTitle").innerText = ;

    $('#EditCodeReview').modal('show');
}

function alphabetical(a, b) {
    return a.localeCompare(b);
}

function cleanFileNames() {
    //get the list of filenames (one per row)
    var splitted = $('#fileNames').val().split("\n");

    //trim all the filenames down to at most 2 directories
    var len = splitted.length;
    for (i = 0; i < len; i++) {
        if (splitted[i].indexOf('@@') != -1) {
            splitted[i] = splitted[i].substring(0, splitted[i].indexOf('@@'));
        }
        splitted[i] = splitted[i].split('\\');
        if (splitted[i].length >= 3) {
            splitted[i] = splitted[i].slice(splitted[i].length - 3).join("\\");
        }
        else {
            splitted[i] = splitted[i].join("\\");
        }
    }
    //sort them
    splitted.sort(alphabetical);

    //get rid of doubles
    splitted = splitted.join('\n');    
    var uniqueList = splitted.split('\n').filter(function (item, i, allItems) {
        return i == allItems.indexOf(item);
    }).join('\n');
    //console.log(uniqueList); 
    $("#fileNames").val(uniqueList.trim());
}

function cleanFileNames(textBox) {
    //get the list of filenames (one per row)
    var splitted = textBox.val().split("\n");

    //trim all the filenames down to at most 2 directories
    var len = splitted.length;
    for (i = 0; i < len; i++) {
        if (splitted[i].indexOf('@@') != -1) {
            splitted[i] = splitted[i].substring(0, splitted[i].indexOf('@@'));
        }
        splitted[i] = splitted[i].split('\\');
        if (splitted[i].length >= 3) {
            splitted[i] = splitted[i].slice(splitted[i].length - 3).join("\\");
        }
        else {
            splitted[i] = splitted[i].join("\\");
        }
        splitted[i] = splitted[i].trim();
    }
    //sort them
    splitted.sort(alphabetical);

    //get rid of doubles
    splitted = splitted.join('\n');
    var uniqueList = splitted.split('\n').filter(function (item, i, allItems) {
        return i == allItems.indexOf(item);
    }).join('\n');
    //console.log(uniqueList);
    textBox.val(uniqueList.trim());
}

function validateComment(textBox)
{
    var isValid = false;
    if (isStringEmpty(textBox.val()) == false)
    {
        isValid = true;
    }
    else
    {
        alert('The comment field cannot be blank.');
    }
    return isValid;
}