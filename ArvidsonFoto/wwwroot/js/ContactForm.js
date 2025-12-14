// Bootstrap 5 Form Validation
$(() => {
    // Hide validation summary initially
    $('#validationSummary').addClass('d-none');

    // Bootstrap 5 form validation
    const forms = document.querySelectorAll('.needs-validation');

    // Add Bootstrap validation classes on blur
    Array.from(forms).forEach(form => {
        const inputs = form.querySelectorAll('input, textarea');
        
        inputs.forEach(input => {
            // Real-time validation on input
            input.addEventListener('blur', () => {
                if (input.checkValidity()) {
                    input.classList.remove('is-invalid');
                    input.classList.add('is-valid');
                } else {
                    input.classList.remove('is-valid');
                    input.classList.add('is-invalid');
                }
            });

            // Show validation on keyup for better UX
            input.addEventListener('keyup', () => {
                if (input.value.length > 0) {
                    const feedbackId = input.getAttribute('aria-describedby');
                    if (feedbackId) {
                        const feedback = document.getElementById(feedbackId);
                        if (feedback && feedback.textContent.trim().length > 0) {
                            if (input.checkValidity()) {
                                input.classList.remove('is-invalid');
                                input.classList.add('is-valid');
                            } else {
                                input.classList.remove('is-valid');
                                input.classList.add('is-invalid');
                            }
                        }
                    }
                }
            });
        });

        form.addEventListener('submit', event => {
            if (!form.checkValidity()) {
                event.preventDefault();
                event.stopPropagation();
            }

            form.classList.add('was-validated');
            
            // Show validation summary if form is invalid
            if (!form.checkValidity()) {
                $('#validationSummary').removeClass('d-none');
            }
        }, false);
    });

    // Show validation feedback when submit button is clicked
    $('#SubmitButton').on('click', () => {
        $('#validationSummary').removeClass('d-none');
    });
});