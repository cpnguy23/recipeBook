function submitForm(element) {
    var form = document.getElementById("submitButton");

}

function onTxtFocused(txt) {
    txt.style.backgroundColor = "#ffd1dc";
}

function onTxtBlurred(txt) {
    txt.style.backgroundColor = "white";
}

//testing if this function works

function onKgsInput(txtBox) {
    let lbl = document.getElementById("lblPounds");
    let kgs = txtBox.value;
    lblPounds.innerText = (kgs * 2.20462);
    lblPounds.innerText += " lbs";
}

