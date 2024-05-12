$(document).ready(function () {
   
    
        $('.select').select2({
            minimumResultsForSearch: Infinity
        });

        // Event listener for paying amount changes
        $('#payingAmount').on('input', function () {
            var total = parseFloat($('#totalPrice').val()) || 0;
            var paying = parseFloat($(this).val()) || 0;

            // If the paying amount exceeds the total, reset it to the total
            if (paying > total) {
                $(this).val(total);
                paying = total;
            }

            // Update remaining amount
            var remaining = total - paying;
            $('#remainAmount').val(remaining.toFixed(2));
        });

        // Payment method change handler
        $('#selectpaymentmethod').change(function () {
            // Hide all method-specific inputs initially
            $('#bankSelection, #AccountNo, #CheqNoPart, #RTGSPart').hide();

            var paymentMethod = $(this).val();
            if (paymentMethod === "BANK") {
                $('#bankSelection, #AccountNo').show();

            } else if (paymentMethod === "CHEQ") {
                $('#bankSelection,#AccountNo, #CheqNoPart').show();
            } else if (paymentMethod === "RTGS") {
                $('#bankSelection,#AccountNo, #RTGSPart').show();
            }
        });

        // Formatting function for bank selection with icons
        function formatBankOption(bank) {
            if (!bank.id || bank.text === '-- Select Bank --') return bank.text; // No icon for the placeholder
            var $bank = $('<span class="img-flag-wrapper"><img src="' + $(bank.element).data('icon') + '" class="img-flag" /> ' + bank.text + '</span>');
            return $bank;
        }

        $('#bankSelect').select2({
            minimumResultsForSearch: -1,
            templateResult: formatBankOption,
            templateSelection: formatBankOption
        });

        // Trigger initial update in case of pre-filled values
        $('#payingAmount').trigger('input');



    // Toastr configuration for error messages
    toastr.options = {
        closeButton: true,
        debug: false,
        progressBar: true,
        positionClass: 'toast-top-right',
        timeOut: '5000',
        extendedTimeOut: '1000',
        showEasing: 'swing',
        hideEasing: 'linear',
        showMethod: 'fadeIn',
        hideMethod: 'fadeOut'
    };



    function checkFormChanges() {
        let isChanged = false;
        $('#PaymentForm input, #PaymentForm select').each(function () {
            // Check if the input or select element has a value different from the default
            if ($(this).val() !== "") {
                isChanged = true;
            }
        });

        if (isChanged) {
            $('#submitBtn').prop('disabled', false);  // Enable the submit button if any change is detected
        } else {
            $('#submitBtn').prop('disabled', true);   // Keep the submit button disabled if no changes are detected
        }
    }

    // Attach change event handlers to all inputs and selects within the form
    $('#PaymentForm').on('change input', 'input, select', checkFormChanges);

    // You might want to call checkFormChanges initially if the form can have pre-filled values that need the button enabled
    checkFormChanges();


    // Event listener for submit button
    document.getElementById('submitBtn').addEventListener('click', function () {
        var customerName = $('#customername').val(); // Get customer name from the input field

        // Validate the form before proceeding
        if (validateForm()) {
            // Only show the SweetAlert if all validations pass
            Swal.fire({
                title: 'Confirm Submission',
                text: `${customerName}'s payment entry to be created?`,
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Yes, create it!'
            }).then((result) => {
                if (result.isConfirmed) {
                    document.getElementById('PaymentForm').submit();
                }
            });
        } else {
            // Optionally, alert the user to correct the errors
            toastr.error("Please Fill All Requried Fields");
        }
    });


});



// Validate form inputs
// Validate form inputs
function validateForm() {
    let isValid = true;
    let errors = [];

    var paymentMethod = $('#selectpaymentmethod').val();
    var payingAmount = parseFloat($('#payingAmount').val()) || 0;
    var bankSelected = $('#bankSelect').val();
    var bankAccountNumber = $('#bankAccountNumber').val();
    var cheqNo = $('#cheqNoInput').val();
    var rtgsNo = $('#rtgsNoInput').val();

    // Check if a payment method has been selected
    if (!paymentMethod) {
        errors.push('Please select a payment method.');
        isValid = false;
    }

    // Check for valid paying amount (assuming zero is not valid unless you decide otherwise)
    if (payingAmount <= 0) {
        errors.push('Paying amount must be greater than zero.');
        isValid = false;
    }

    // Specific checks based on the payment method selected
    if (['BANK', 'CHEQ', 'RTGS'].includes(paymentMethod)) {
        if (!bankSelected) {
            errors.push('Bank selection is required.');
            isValid = false;
        }
        if (!bankAccountNumber) {
            errors.push('Bank account number is required.');
            isValid = false;
        }
    }

    if (paymentMethod === 'CHEQ' && (!cheqNo || cheqNo.length !== 6)) {
        errors.push('A valid 6-digit CHEQ NO. is required.');
        isValid = false;
    }

    if (paymentMethod === 'RTGS' && (!rtgsNo || rtgsNo.length !== 8)) {
        errors.push('A valid 8-digit RTGS NO. is required.');
        isValid = false;
    }

    errors.forEach(error => toastr.error(error));
    return isValid;
}


