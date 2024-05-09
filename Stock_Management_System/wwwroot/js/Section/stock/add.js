
document.getElementById('selectgrain').addEventListener('change', function () {
    var selectedOption = this.options[this.selectedIndex];
    var selectedText = selectedOption.text;
    document.querySelector('input[name="ProductName"]').value = selectedText;
});
document.getElementById('selectvehicletype').addEventListener('change', function () {
    var selectedOption = this.options[this.selectedIndex];
    var selectedText = selectedOption.text; // This will be "ઘઉં" for the given example
    document.querySelector('input[name="VehicleName"]').value = selectedText;
});

function setDateDefaultValue() {
    var today = new Date();
    var day = String(today.getDate()).padStart(2, '0');
    var month = String(today.getMonth() + 1).padStart(2, '0'); // January is 0!
    var year = today.getFullYear();
    var formattedDate = year + '-' + month + '-' + day;
    // Ensure the datepicker element exists and then set its value
    var datePicker = document.getElementById('datepicker');
    if (datePicker) {
        datePicker.value = formattedDate;
    }
}
// Run the function after the page has loaded to ensure the element is available
document.addEventListener('DOMContentLoaded', function () {
    setDateDefaultValue();
});
window.onload = function () {
    setDateDefaultValue();
    CalculateMethod();
};

function CalculateMethod() {
    var bags = document.getElementById('bags').value.trim();
    var bagsperkg = document.getElementById('bagsperkg').value.trim();
    var weightInput = document.getElementById('weight');
    var productprice = document.getElementById('productprice').value.trim();
    var totalprice = 0;
    // Check if both bags and bagsperkg are provided for multiplication
    var weight = (bags !== '' && bagsperkg !== '') ? bags * bagsperkg : parseFloat(weightInput.value) || 0;
    weightInput.value = weight === 0 ? '' : weight;
    var selectGrain = document.getElementById("selectgrain");
    var selectedValue = selectGrain.value;
    switch (selectedValue) {
        case "1":
        case "2":
        case "4":
        case "5":
        case "6":
        case "7":
        case "8":
            totalprice = weight * productprice / 20;
            break;
        case "3":
            totalprice = weight * productprice / 400;
            break;
        // Add more cases if needed
    }
    document.getElementById('totalprice').value = totalprice === 0 ? '' : totalprice.toFixed(2);
}
// Attach the event listener to the dropdown
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
// Attach the event listener to the input field for product price
document.getElementById("productprice").addEventListener("input", CalculateMethod);

function CheckData() {
    var date = document.getElementById("datepicker").value;
    var stockgrade = document.getElementById("stockgrade").value;
    var grainname = document.getElementById("selectgrain").value;
    var customername = document.getElementById("Customer").value;
    var bags = document.getElementById("bags").value;
    var bagperkg = document.getElementById("bagsperkg").value;
    var weight = document.getElementById("weight").value;
    var productprice = document.getElementById("productprice").value;
    var totalprice = document.getElementById("totalprice").value;
    var vehicletype = document.getElementById("selectvehicletype").value;
    var vehicleno = document.getElementById("vehicleno").value;
    var drivername = document.getElementById("drivername").value;
    // Removed check for 'tolatname'
    if (!grainname || !customername || !weight || !productprice || !totalprice || !vehicletype || !vehicleno || !drivername) {
        toastr.error('Please fill out all required fields.', {
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
        });
        return false; // Prevent form submission
    } else {
        // If all fields are filled, use SweetAlert2 to confirm with the user before submission
        confirmStockAddition("/Stock/InsertStockAndCustomerDetails", customername);
        return false; // Prevent form submission until confirmation and AJAX call are completed
    }
}

function confirmStockAddition(addUrl, customerName) {
    Swal.fire({
        title: 'Are You Sure You Want To Add This Stock?',
        text: `${customerName}'s Stock Add?`,
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, add it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: addUrl, // Make sure addUrl is defined and not undefined
                type: 'POST',
                data: $("#AddStockForm").serialize(), // Ensure this form ID matches your form
                success: function (response) {
                    // Assuming the server sends a JSON response with a redirectUrl field
                    sessionStorage.setItem('AddStatus', 'Stock added successfully!');
                    window.location.href = response.redirectUrl; // Use the redirect URL from the response
                },
                error: function () {
                    sessionStorage.setItem('AddStatus', 'Error adding stock. Please try again.');
                    window.location.href = response.redirectUrl; // Use the redirect URL from the response
                }
            });
        }
    });
}

function formatVehicleNo(input) {
    // Remove non-alphanumeric characters and convert to uppercase
    let formattedInput = input.value.replace(/[^a-zA-Z0-9]/g, '').toUpperCase();
    // Ensure the input does not exceed the expected length
    formattedInput = formattedInput.substr(0, 14);
    // Insert hyphens at specific positions
    formattedInput = formattedInput.replace(/(\w{2})(\d{2})(\w{2})(\d{4})/, '$1-$2-$3-$4');
    // Update the input field with the formatted value
    input.value = formattedInput;
    // Validate the input format
    const regex = /^([A-Z]{2})-(\d{2})-([A-Z]{2})-(\d{4})$/;
    const errorMsg = document.getElementById('error-msg');
    errorMsg.textContent = regex.test(formattedInput) ? '' : 'Invalid format. Please follow the format: GJ-12-AB-1234';
}

$(document).ready(function () {
    var isSuggestionSelected = false; // Flag to indicate if a suggestion has been selected.
    // Function to show or hide new customer fields and a message.
    function toggleNewCustomerFields(show) {
        $("#newCustomerMessage").toggle(show);
        $("#newcustomertype").toggle(show);
        $("#newcustomercity").toggle(show);
        $("#newcustomerphoneno").toggle(show);
    }
    // Initially hide the new customer message and fields.
    toggleNewCustomerFields(false);
    $("#Customer").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: "/Stock/GetBuyerCustomerData", // Correct URL to your controller action.
                type: "POST",
                dataType: "json",
                data: {
                    CustomerName: request.term
                },
                success: function (data) {
                    response($.map(data, function (item) {
                        return {
                            label: item.customerName + ' (' + item.customerType + ') - ' + item.customerAddress,
                            value: item.customerName,
                            id: item.customerId, // Customer ID.
                            type: item.customerType // Customer Type.
                        };
                    }));
                }
            });
        },
        minLength: 1, // Minimum length before searching.
        response: function (event, ui) {
            var exactMatch = ui.content.some(item => item.value.toLowerCase() === $("#Customer").val().toLowerCase());
            toggleNewCustomerFields(!exactMatch && !isSuggestionSelected);
        },
        select: function (event, ui) {
            isSuggestionSelected = true;
            event.preventDefault(); // Prevent default to avoid placing label in the input.
            $("#Customer").val(ui.item.value); // Set the input to the customer name.
            $("#CustomerId").val(ui.item.id); // Update hidden field with selected customer ID.
            $("#cttype").val(ui.item.type).trigger('change'); // Set the customer type and trigger change for any attached handlers.
            toggleNewCustomerFields(false); // Hide new customer fields as a selection has been made.
        }
    });
    // Reset the flag and potentially hide new customer fields if the input is cleared or changed.
    $("#Customer").on('input', function () {
        var enteredValue = $(this).val().trim();
        if (!enteredValue) {
            toggleNewCustomerFields(false);
            isSuggestionSelected = false;
        } else {
            isSuggestionSelected = false;
            // Since we are still typing, check if any item matches exactly from the last search.
            $("#Customer").autocomplete("search", enteredValue);
        }
    });
    // Optionally, reset the form state when the page is refreshed or navigated away.
    $(window).on('beforeunload', function () {
        toggleNewCustomerFields(false);
    });
});
