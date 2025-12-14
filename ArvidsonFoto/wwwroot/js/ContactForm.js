// Bootstrap 5 Form Validation
$(() => {
    // Hide validation summary initially
    $('#validationSummary').addClass('d-none');

    // Bootstrap 5 form validation
    const forms = document.querySelectorAll('.needs-validation');

    // Validate input and apply Bootstrap classes
    const validateInput = (input) => {
        if (input.checkValidity()) {
            input.classList.remove('is-invalid');
            input.classList.add('is-valid');
        } else {
            input.classList.remove('is-valid');
            input.classList.add('is-invalid');
        }
    };

    // Add Bootstrap validation classes on blur and keyup
    Array.from(forms).forEach(form => {
        const inputs = form.querySelectorAll('input, textarea');
        
        inputs.forEach(input => {
            // Real-time validation on blur
            input.addEventListener('blur', () => {
                validateInput(input);
            });

            // Show validation on keyup for better UX (only if user has started typing)
            input.addEventListener('keyup', () => {
                if (input.value.length > 0) {
                    validateInput(input);
                }
            });
        });

        form.addEventListener('submit', event => {
            const isValid = form.checkValidity();
            
            if (!isValid) {
                event.preventDefault();
                event.stopPropagation();
                // Show validation summary only if form is invalid
                $('#validationSummary').removeClass('d-none');
            }

            form.classList.add('was-validated');
        }, false);
    });
});