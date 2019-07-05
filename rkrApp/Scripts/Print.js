$(document).ready(function () {
    $('#btnExport').click(function () {
        $("#example5").table2excel({
            exclude: ".rows1",
            name: "Report",
            filename: "Report"
        });
    });

});


