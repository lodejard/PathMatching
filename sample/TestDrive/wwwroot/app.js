$(function () {

    var updateTimeout;

    function update() {
        clearTimeout(updateTimeout);
        updateTimeout = setTimeout(updateResult, 500);
    }

    function updateResult() {
        $.ajax('api', {
            'type': 'POST',
            'data': $('#screen').serialize(),
            'contentType': 'application/x-www-form-urlencoded; charset=UTF-8',
            'success': function (data, textStatus, xhr) {
                var recorder = xhr.getResponseHeader('recorder');
                $("#recorder").text(recorder.split(',').join('\r\n'));
                $("#result").text(data);
            },
            'error': function (xhr, textStatus, errorThrown) {
                $("#result").text(textStatus);
            }
        });
    }

    $("#include").keyup(update);
    $("#exclude").keyup(update);
    $("#files").keyup(update);
});

