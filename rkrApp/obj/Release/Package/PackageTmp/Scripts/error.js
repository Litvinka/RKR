$(document).ready(function () {
    $("#id_topic").prop('disabled', true);
    $("#Topics_id_section").change(function () {
        $("#id_topic").prop('disabled', true);
        if ($("#Topics_id_section").val() > 0) {
            $.ajax({
                url: '/Results/SearchTopics',
                type: 'POST',
                data: JSON.stringify({ 'param': $("#Topics_id_section").val() }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    data = data.replace(/\]/g, "");
                    data = data.replace(/\[/g, "");
                    var arr = data.toString().split('\",\"');
                    arr[0] = arr[0].replace(/\"/, "");
                    arr[arr.length - 1] = arr[arr.length - 1].replace(/\"/, "");
                    $('#id_topic').attr("disabled", false);
                    document.getElementById('id_topic').innerHTML = '';
                    $('#id_topic').append(new Option(arr[i + 1]));
                    for (var i = 0; i < arr.length; ++i) {
                        $('#id_topic').append(new Option(arr[i + 1], arr[i]));
                        ++i;
                    }
                }
            });
        }
    });
});