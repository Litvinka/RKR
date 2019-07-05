$(document).ready(function () {
    if ($("#letters").length && $("#students").length) {
        $("#students").prop('disabled', true);

        $("#letters").change(function () {
            $("#students").prop('disabled', true);
            if ($("#letters").val()>0) {
                $.ajax({
                    url: '/Observer/GetStudentClass',
                    type: 'POST',
                    data: JSON.stringify({ 'param': $("#letters").val()}),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        data = data.replace(/\]/g, "");
                        data = data.replace(/\[/g, "");
                        data = data.replace(/\"/g, "");
                        var arr = data.toString().split(',');
                        $('#students').attr("disabled", false);
                        document.getElementById('students').innerHTML = '';
                        $('#students').append(new Option(arr[i + 1]));
                        for (var i = 0; i < arr.length; ++i) {
                            $('#students').append(new Option(arr[i + 1], arr[i]));
                            ++i;
                        }
                    }
                });
            }
        });
    }

    $("#Students_cipher").chosen({ no_results_text: "Ничего не найдено.", placeholder_text_single: "Выберите шифр" });


});
