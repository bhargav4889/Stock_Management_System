



function confirmDelete(Name) {


    Swal.fire({
        title: 'Are you sure?',
        text: ` Are You Want This ${Name}'s Delete Something !`, // Using template literal
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Delete'
    }).then((result) => {
        if (result.isConfirmed) {
            document.getElementById('deleteform').submit();
        }
    });
}








document.addEventListener('DOMContentLoaded', function () {

    var errorMessage = "";
    errorMessage = document.getElementById('toastrErrorMessage').value;


    if (errorMessage) {
        toastr.error(errorMessage, "Alert !!", {
            closeButton: true,
            progressBar: true,
            positionClass: 'toast-top-right',
            preventDuplicates: true,
            showDuration: '300',
            hideDuration: '1000',
            timeOut: '2000',
            extendedTimeOut: '1000',
            showEasing: 'swing',
            hideEasing: 'linear',
            showMethod: 'fadeIn',
            hideMethod: 'fadeOut'
        });
    }
});
