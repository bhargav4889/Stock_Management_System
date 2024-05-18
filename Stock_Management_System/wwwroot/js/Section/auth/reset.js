function isValidEmail(email) {
    const regex = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$/;
    return regex.test(email);
}


function requestPasswordReset() {
    var email = document.getElementById('email').value;
    if (!isValidEmail(email)) {
        toastr.error('Please enter a valid email address.', {
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
        return;
    }

    Swal.fire({
        title: 'Are you sure?',
        text: "A password reset link will be sent to your email if it's registered in our system.",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, send it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/Auth/ResetRequestPassword',
                type: 'GET',
                data: { email: email },
                success: function (response) {
                    if (response.success) {
                        sessionStorage.setItem('resetStatus', 'Password reset link sent successfully!');
                        window.location.href = response.redirectUrl;
                    } else {
                        toastr.error('Email is not registered.', {
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
                    }
                },
                error: function (xhr) {
                    toastr.error('There was an error processing your request. Please try again.', {
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
                }
            });
        }
    });
}


