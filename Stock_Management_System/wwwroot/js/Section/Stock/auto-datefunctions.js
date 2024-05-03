function setDateDefaultValue() {
    var today = new Date();
    var day = String(today.getDate()).padStart(2, '0');
    var month = String(today.getMonth() + 1).padStart(2, '0'); // January is 0!
    var year = today.getFullYear();
    var formattedDate = year + '-' + month + '-' + day;

    var datePicker = document.getElementById('datepicker');
    if (datePicker) {
        datePicker.value = formattedDate;
    }
}

document.addEventListener('DOMContentLoaded', setDateDefaultValue);
window.onload = setDateDefaultValue;
