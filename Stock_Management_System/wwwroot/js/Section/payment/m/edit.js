$(document).ready(function () {
    // Initialize Select2 for .select elements
    $('.select').select2({
        minimumResultsForSearch: Infinity
    });

    $('#payingAmount').on('input', function () {
        var totalRemain = parseFloat($('#currentRemainAmount').val()) || 0;
        var paying = parseFloat($(this).val()) || 0;

        // Check if the paying amount is more than the current remain amount
        if (paying > totalRemain) {
            $(this).val(totalRemain); // Set value to max allowable if it's over
            paying = totalRemain;
        }

        // Calculate and display the new remaining amount
        var newRemaining = totalRemain - paying;
        $('#currentRemainAmount').val(newRemaining.toFixed(2));
    });

    $('#selectpaymentmethod').change(function () {
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

    $('#bankSelect').select2({
        minimumResultsForSearch: -1,
        templateResult: formatBankOption,
        templateSelection: formatBankOption
    });

    $('#PreviousbankSelect').select2({
        minimumResultsForSearch: -1,
        templateResult: formatBankOption,
        templateSelection: formatBankOption
    });

    $('#payingAmount').trigger('input');

    $('#submitBtn').on('click', function () {
        if (validateForm()) {
            Swal.fire({
                title: 'Confirm Submission',
                text: "Are you sure you want to submit this form?",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Yes, submit it!'
            }).then((result) => {
                if (result.isConfirmed) {
                    document.getElementById('PaymentForm').submit();
                }
            });
        }
    });
});

function validateForm() {
    let isValid = true;
    let errors = [];

    var paymentMethod = $('#selectpaymentmethod').val();
    var payingAmount = parseFloat($('#payingAmount').val()) || 0;
    var bankSelected = $('#bankSelect').val();
    var bankAccountNumber = $('#bankAccountNumber').val();
    var cheqNo = $('#cheqNoInput').val();
    var rtgsNo = $('#rtgsNoInput').val();

    if (!paymentMethod) {
        errors.push('Please select a payment method.');
        isValid = false;
    }

    if (payingAmount <= 0) {
        errors.push('Paying amount must be greater than zero.');
        isValid = false;
    }

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

function formatBankOption(bank) {
    if (!bank.id || bank.text === '-- Select Bank --') return bank.text;
    var $bank = $('<span class="img-flag-wrapper"><img src="' + $(bank.element).data('icon') + '" class="img-flag" /> ' + bank.text + '</span>');
    return $bank;
}
