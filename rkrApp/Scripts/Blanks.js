$(document).ready(function () {
    $("img.img-responsive.cipher").click(function() {
        var img = $(this);
        var src = img.attr("src");
        $("body").append("<div class='popup'>" + "<div class='popup_bg'></div>"+ "<img src='"+ src + "' class='popup_img' />" + "</div>");
        $(".popup").fadeIn(800);
        $(".popup_bg").click(function(){  
            $(".popup").fadeOut(800);
            setTimeout(function() {  
                $(".popup").remove();
            }, 800);
        });
    });


    $("a.img-responsive_a").click(function (e) {
        e.preventDefault();
        var img = $(this);
        var src = img.attr("href");
        $("body").append("<div class='popup'>" + "<div class='popup_bg'></div>" + "<img src='" + src + "' class='popup_img' />" + "</div>");
        $(".popup").fadeIn(800);
        $(".popup_bg").click(function () {
            $(".popup").fadeOut(800);
            setTimeout(function () {
                $(".popup").remove();
            }, 800);
        });
    });


    $("#table_blanks").dataTable({
        language: {
            "processing": "Подождите...",
            "search": "Поиск:",
            "lengthMenu": "Показать _MENU_ записей",
            "info": "Записи с _START_ до _END_ из _TOTAL_ записей",
            "infoEmpty": "Записи с 0 до 0 из 0 записей",
            "infoFiltered": "(отфильтровано из _MAX_ записей)",
            "infoPostFix": "",
            "loadingRecords": "Загрузка записей...",
            "zeroRecords": "Записи отсутствуют.",
            "emptyTable": "В таблице отсутствуют данные",
            "paginate": {
                "first": "Первая",
                "previous": "Предыдущая",
                "next": "Следующая",
                "last": "Последняя"
            },
            "aria": {
                "sortAscending": ": активировать для сортировки столбца по возрастанию",
                "sortDescending": ": активировать для сортировки столбца по убыванию"
            }
        }
    });

    $("#table_ciphers").dataTable({
        language: {
            "processing": "Подождите...",
            "search": "Поиск:",
            "lengthMenu": "Показать _MENU_ записей",
            "info": "Записи с _START_ до _END_ из _TOTAL_ записей",
            "infoEmpty": "Записи с 0 до 0 из 0 записей",
            "infoFiltered": "(отфильтровано из _MAX_ записей)",
            "infoPostFix": "",
            "loadingRecords": "Загрузка записей...",
            "zeroRecords": "Записи отсутствуют.",
            "emptyTable": "В таблице отсутствуют данные",
            "paginate": {
                "first": "Первая",
                "previous": "Предыдущая",
                "next": "Следующая",
                "last": "Последняя"
            },
            "aria": {
                "sortAscending": ": активировать для сортировки столбца по возрастанию",
                "sortDescending": ": активировать для сортировки столбца по убыванию"
            }
        }
    });


    $(".scan").click(function (e) {
        var img = $(".scan #scan_img");
        var src = img.attr("src");
        $("body").append("<div class='popup'>" + "<div class='popup_bg'></div>" + "<img src='" + src + "' class='popup_img' />" + "</div>");
        $(".popup").fadeIn(800);
        $(".popup_bg").click(function () {
            $(".popup").fadeOut(800);
            setTimeout(function () {
                $(".popup").remove();
            }, 800);
        });
    });


    $("#Students_cipher").change(function () {
        $(".scan").css("display", "none");
        $("#scan_img").attr("src", "");
        if ($("#Students_cipher").val() > 0) {
            $.ajax({
                url: '/Results/GetScan',
                type: 'POST',
                data: JSON.stringify({ 'param': $("#Students_cipher").val() }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    data = data.replace(/\"/g, "");
                    $(".scan").fadeIn();
                    $("#scan_img").attr("src", data);
                }
            });
        }
    });


}); 


