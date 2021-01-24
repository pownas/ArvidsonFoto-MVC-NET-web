$(() => {
    $('#validationSummary').hide();
    $('#validationName').hide();
    $('#validationEmail').hide();
    $('#validationSubject').hide();
    $('#validationMessage').hide();
    $('#validationCode').hide();

    $('#SubmitButton').click(() => {
        $('#validationSummary').show();
    });

    $('#Name').keyup(() => {
        $('#validationName').show();
    });
    $('#Email').keyup(() => {
        $('#validationEmail').show();
    });
    $('#Subject').keyup(() => {
        $('#validationSubject').show();
    });
    $('#Message').keyup(() => {
        $('#validationMessage').show();
    });
    $('#Code').keyup(() => {
        $('#validationCode').show();
    });

});