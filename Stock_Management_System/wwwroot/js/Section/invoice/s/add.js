


$(function () {
    $('#selectgrain').select2({
        minimumResultsForSearch: Infinity // Disables the search box
    });

    // Listen for the 'select2:select' event to capture when an option is selected
    $('#selectgrain').on('select2:select', function (e) {
        // Get the text of the selected option
        var selectedText = e.params.data.text;

        // Update the #ProductName hidden input field's value with the selected option's text
        $('#ProductName').val(selectedText);

        // Logging the selected text for verification
    });
});


$(function () {
    $('#selectvehicletype').select2({
        minimumResultsForSearch: Infinity // Disables the search box
    });

    // Listen for the 'select2:select' event to capture when an option is selected
    $('#selectvehicletype').on('select2:select', function (e) {
        // Get the text of the selected option
        var selectedText = e.params.data.text;

        // Update the #ProductName hidden input field's value with the selected option's text
        $('#VehicleName').val(selectedText);

        // Logging the selected text for verification
    });
});



function CalculateMethod() {
    var bags = document.getElementById('bags').value.trim();
    var bagsperkg = document.getElementById('bagsperkg').value.trim();
    var weightInput = document.getElementById('weight');
    var productprice = document.getElementById('productprice').value.trim();
    var sgst = parseFloat(document.getElementById('SGST').value.trim()) || 0;
    var cgst = parseFloat(document.getElementById('CGST').value.trim()) || 0;
    var totalprice = parseFloat(document.getElementById('totalprice').value.trim()) || 0;

    // Check if both bags and bagsperkg are provided for multiplication
    var weight = (bags !== '' && bagsperkg !== '') ? bags * bagsperkg : parseFloat(weightInput.value) || 0;

    weightInput.value = weight === 0 ? '' : weight;

    var selectGrain = document.getElementById("selectgrain");
    var selectedValue = selectGrain.value;

    switch (selectedValue) {
        case "1":
        case "2":
        case "3":
        case "4":
        case "5":
        case "6":
        case "7":
        case "8":
            totalprice = weight * productprice;
            break;
    }

    document.getElementById('beforetaxtotal').value = totalprice.toFixed(2);

    // Check if CGST and SGST are not empty or zero
    if (cgst !== 0 && sgst !== 0) {
        // Calculate CGST and IGST based on percentages
        var cgstAmount = (totalprice * cgst) / 100;
        var sgstAmount = (totalprice * sgst) / 100;

        document.getElementById('CGST_Total').value = cgstAmount;
        document.getElementById('SGST_Total').value = sgstAmount;

        // Add CGST and IGST to the total price
        totalprice += cgstAmount + sgstAmount;
    } else {
        // Set CGST and SGST totals to 0 or empty
        document.getElementById('CGST_Total').value = '';
        document.getElementById('SGST_Total').value = '';
    }

    document.getElementById('totalprice').value = totalprice === 0 ? '' : totalprice.toFixed(2);
}

// Attach the event listener to the dropdown outside the function
document.getElementById("selectgrain").addEventListener("change", CalculateMethod);

// Attach the event listener to the input field for bags
document.getElementById("bags")?.addEventListener("input", function () {
    if (this.value.trim() !== '') {
        CalculateMethod();
    }
});

// Attach the event listener to the input field for bags per kg
document.getElementById("bagsperkg")?.addEventListener("input", function () {
    if (this.value.trim() !== '') {
        CalculateMethod();
    }
});

document.getElementById("SGST")?.addEventListener("input", function () {
    if (this.value.trim() !== '') {
        CalculateMethod();
    }
});

document.getElementById("CGST")?.addEventListener("input", function () {
    if (this.value.trim() !== '') {
        CalculateMethod();
    }
});

// Attach the event listener to the input field for product price
document.getElementById("productprice").addEventListener("input", CalculateMethod);


function CheckData() {
    var grainname = document.getElementById("selectgrain").value;
    var partyName = document.getElementById("Partyname").value.trim();
    var invoiceType = $('#selectinvoice').val();
    var partyAddress = document.getElementById("PartyAddress").value.trim();
    var brandName = document.getElementById("ProductBrandName").value.trim();
    var weight = parseFloat(document.getElementById("weight").value);
    var totalPrice = parseFloat(document.getElementById("totalprice").value);
    var productPrice = parseFloat(document.getElementById("productprice").value);
    var vehicleType = document.getElementById("VehicleName").value;
    var containerNumber = document.getElementById("containerNumber") ? document.getElementById("containerNumber").value : null;
    var vehicleNo = document.getElementById("vehicleno").value.trim();
    var driverName = document.getElementById("drivername").value.trim();

    var toastrSettings = {
        closeButton: true,
        progressBar: true,
        positionClass: 'toast-bottom-right',
        preventDuplicates: true,
        showDuration: '300',
        hideDuration: '1000',
        timeOut: '5000',
        extendedTimeOut: '1000',
        showEasing: 'swing',
        hideEasing: 'linear',
        showMethod: 'fadeIn',
        hideMethod: 'fadeOut'
    };

    // Validate required fields
    var missingFields = [];
    if (!grainname) missingFields.push("Grain Type");
    if (!partyName) missingFields.push("Party Name");
    if (!partyAddress) missingFields.push("Party Address");
    if (!brandName) missingFields.push("Brand Name");
    if (isNaN(weight) || weight <= 0) missingFields.push("Weight");
    if (isNaN(totalPrice) || totalPrice <= 0) missingFields.push("Total Price");
    if (isNaN(productPrice) || productPrice <= 0) missingFields.push("Product Price");
    if (!vehicleType) missingFields.push("Vehicle Type");
    if (!vehicleNo) missingFields.push("Vehicle Number");
    if (!driverName) missingFields.push("Driver Name");

   

    if (vehicleType === 'CONTAINER' && (!containerNumber || containerNumber.trim() === '')) {
        missingFields.push("Container Number");
    }

    // Display errors if any fields are missing
    if (missingFields.length > 0) {
        toastr.error(`Please fill out the following required fields: ${missingFields.join(', ')}`, toastrSettings);
        return false;
    }

    // Use SweetAlert2 to confirm with the user before submission
    confirmSaleInvoiceCreation("/Invoice/InsertSaleInvoice", partyName);
    return false; // Prevent form submission until confirmation and AJAX call are completed
}






function confirmSaleInvoiceCreation(redirectUrl, partyName) {
    Swal.fire({
        title: 'Confirm Invoice Creation',
        text: `Create an invoice for ${partyName}?`,
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, create it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: redirectUrl,
                type: 'POST',
                data: $("#SaleInvoiceForm").serialize(),
                success: function (response) {
                    if (response.success) {
                        sessionStorage.setItem('InvoiceStatus', 'Invoice created successfully!');
                        window.location.href = response.redirectUrl; // Use the redirect URL from the response
                    } else {
                        toastr.error('Failed to create invoice: ' + response.message);
                    }
                },
                error: function (xhr, status, error) {
                    toastr.error('Ajax error: ' + error);
                }
            });
        }
    });
}





$(function () {
    // Initialize select2 on #selectvehicletype and disable the search box
    $('#selectvehicletype').select2({
        minimumResultsForSearch: Infinity // Disables the search box
    });

    // Listen for the 'select2:select' event to capture when an option is selected
    $('#selectvehicletype').on('select2:select', function (e) {
        // Get the value and text of the selected option
        var selectedValue = e.params.data.id;
        var selectedText = e.params.data.text;

        // Show or hide the #containerNumberField based on the selected value
        if (selectedText === 'CONTAINER') {
            $('#containerNumberField').show();

        } else {
            $('#containerNumberField').hide();

        }

        // Update the #VehicleName hidden input field's value with the selected option's text
        $('#VehicleName').val(selectedText);

    });
});



function formatVehicleNo(input) {
    // Remove non-alphanumeric characters and convert to uppercase
    let formattedInput = input.value.replace(/[^a-zA-Z0-9]/g, '').toUpperCase();
    // Ensure the input does not exceed the expected length
    formattedInput = formattedInput.substr(0, 14);

    // Determine the appropriate format based on the number of letters after the first four characters
    if (/^[A-Z]{2}\d{2}[A-Z]{1}\d{4}$/.test(formattedInput)) {
        // Format: GJ-12-A-1234
        formattedInput = formattedInput.replace(/(\w{2})(\d{2})(\w{1})(\d{4})/, '$1-$2-$3-$4');
    } else if (/^[A-Z]{2}\d{2}[A-Z]{2}\d{4}$/.test(formattedInput)) {
        // Format: GJ-12-AB-1234
        formattedInput = formattedInput.replace(/(\w{2})(\d{2})(\w{2})(\d{4})/, '$1-$2-$3-$4');
    }

    // Update the input field with the formatted value
    input.value = formattedInput;

    // Validate the input format
    const regex = /^([A-Z]{2})-(\d{2})-([A-Z]{1,2})-(\d{4})$/;
    const errorMsg = document.getElementById('error-msg');
    errorMsg.textContent = regex.test(formattedInput) ? '' : 'Invalid format. Please follow the formats: GJ-12-A-1234 or GJ-12-AB-1234';
}



$('#SaleInvoiceForm').submit(function (event) {
    event.preventDefault();  // Prevent the default form submission
    
});


function confirmSaleInvoiceDataReset() {
    Swal.fire({
        title: 'Are you sure Want Reset All values ?',
        text: "You won't be able to revert this!",
        icon: 'info',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, clear it!'
    }).then((result) => {
        if (result.isConfirmed) {
            window.location.reload();
        }
    });
}

document.addEventListener('DOMContentLoaded', function () {
    setDateDefaultValue();
});

function setDateDefaultValue() {
    var today = new Date();
    var day = String(today.getDate()).padStart(2, '0');
    var month = String(today.getMonth() + 1).padStart(2, '0');
    var year = today.getFullYear();
    var formattedDate = year + '-' + month + '-' + day;

    var datePicker = document.getElementById('datepicker');
    if (datePicker) {
        datePicker.value = formattedDate;
    }
}












