

$(function () {
    function formatBankOption(bank) {
        if (!bank.id || bank.text === '-- Select Bank --') return bank.text;
        var iconUrl = $(bank.element).data('icon');
        var $bank = $('<span><img src="' + iconUrl + '" class="img-flag" /> ' + bank.text + '</span>');
        return $bank;
    }

    $('#bankSelect').select2({
        minimumResultsForSearch: -1,
        templateResult: formatBankOption,
        templateSelection: formatBankOption
    });
});

// Reset button event
$('button[type="reset"]')(function () {
    setTimeout(function () { // Timeout for ensuring the reset has completed
        $('#bankSelect').val("").trigger('change'); // Resets Select2
    }, 1);
});



function CheckData() {
    var bankId = document.getElementById('bankSelect').value;
    var accountNo = document.getElementById('BankAccountNo').value;
    var holderName = document.getElementById('AccountHolderName').value;
    var ifsccode = document.getElementById('IFSC_CODE').value;

    var missingFields = [];

    if (bankId === '') missingFields.push('Bank');
    if (accountNo === '') missingFields.push('Account No');
    if (holderName === '') missingFields.push('Account Holder Name');
    if (ifsccode === '') missingFields.push('IFSC Code');

    if (missingFields.length > 0) {
        toastr.error(`Please fill out the following required fields: ${missingFields.join(', ')}`, {
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
        return false;
    } else {
        confirmAddBankInformation('/Information/InsertBankInformation');
        return false; // Prevent form submission until confirmation and AJAX call are completed
    }
}

function confirmAddBankInformation(redirectUrl) {
    Swal.fire({
        title: 'Confirm Add Bank Information',
        text: 'Are you sure you want to add this bank information?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, add it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: redirectUrl,
                type: 'POST',
                data: $("#AddBankInformationForm").serialize(), // Ensure this form ID matches your form
                success: function (response) {
                    sessionStorage.setItem('AddBankStatus', 'Bank information added successfully!');
                    window.location.href = response.redirectUrl; // Redirect using the URL from the response
                },
                error: function () {
                    sessionStorage.setItem('ErrorMsg', 'Somthing Went Wrong !');
                    window.location.reload(); // Reload the current page on error
                }
            });
        }
    });
}


function confirmInformationDataReset() {
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
