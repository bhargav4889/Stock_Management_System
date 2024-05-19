$(document).ready(function () {
    function isValidPassword(password) {
        // Validate password strength (example: at least 6 characters)
        return password.length >= 6;
    }

    $('#resetPasswordForm').submit(function (e) {
        e.preventDefault();

        var password = $('#Password').val();
        var confirmPassword = $('#ConfirmPassword').val();

        if (!isValidPassword(password)) {
            toastr.error('Password must be at least 6 characters.', {
                closeButton: true,
                progressBar: true,
                positionClass: 'toast-bottom-right',
                preventDuplicates: true,
                timeOut: '5000'
            });
            return;
        }

        if (password !== confirmPassword) {
            toastr.error('Passwords do not match.', {
                closeButton: true,
                progressBar: true,
                positionClass: 'toast-top-right',
                preventDuplicates: true,
                timeOut: '5000'
            });
            return;
        }

        Swal.fire({
            title: 'Are you sure?',
            text: "Do you want to update your password?",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, update it!'
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: '/Auth/ChangeRequestPassword',
                    type: 'POST',
                    data: $('#resetPasswordForm').serialize(),
                    success: function (response) {
                        if (response.success) {
                            toastr.success(response.message, {
                                closeButton: true,
                                progressBar: true,
                                positionClass: 'toast-bottom-right',
                                preventDuplicates: true,
                                timeOut: '5000'
                            });
                            window.location.href = response.redirectUrl; // Redirect to the provided URL
                        } else {
                            toastr.error(response.message, {
                                closeButton: true,
                                progressBar: true,
                                positionClass: 'toast-bottom-right',
                                preventDuplicates: true,
                                timeOut: '5000'
                            });
                        }
                    },
                    error: function () {
                        toastr.error('Something went wrong. Please try again.', {
                            closeButton: true,
                            progressBar: true,
                            positionClass: 'toast-bottom-right',
                            preventDuplicates: true,
                            timeOut: '5000'
                        });
                    }
                });

            }
        });
    });
});