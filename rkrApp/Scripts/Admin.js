$(function () {
    $("#all_users_site").dataTable({
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

    $("#all_school_site").dataTable({
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

    $("#all_region_site").dataTable({
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

})


$(document).ready(function () {
    if (window.location.pathname.indexOf("Users/Edit") >= 0) {
        getRole();
        $("#district_dst").prop('disabled', false);
        $("#school_dst").prop('disabled', false);
        $("#id_district").prop('disabled', false);
        $("#school").prop('disabled', false);
        $("#district_watcher").prop('disabled', false);
        $("#school_watcher").prop('disabled', false);
        $('#letter').fadeIn();
    }


    $("#id_role").change(function(){
        getRole();
    });

    function getRole() {
        var role = $("#id_role").val();
        if (role == 3) {
            $("#region").css("display", "none");
            $("#zavuch").fadeIn();
            $("#watcher").css("display", "none");
            $("#areaCheck").css("display", "none");
            $("#districtCheck").css("display", "none");

            $("#id_district").prop('disabled', true);
            $("#school").prop('disabled', true);
            $("#id_area").change(function () {
                $("#id_district").prop('disabled', true);
                $("#school").prop('disabled', true);
                if ($("#id_area").val() > 0) {
                    $.ajax({
                        url: '/Users/AllDistrict',
                        type: 'POST',
                        data: JSON.stringify({ 'param': $("#id_area").val() }),
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            data = data.replace(/\]/g, "");
                            data = data.replace(/\[/g, "");
                            var arr = data.toString().split('\",\"');
                            arr[0] = arr[0].replace(/\"/, "");
                            arr[arr.length - 1] = arr[arr.length - 1].replace(/\"/, "");
                            $('#id_district').attr("disabled", false);
                            document.getElementById('id_district').innerHTML = '';
                            $('#id_district').append(new Option(arr[i + 1]));
                            for (var i = 0; i < arr.length; ++i) {
                                $('#id_district').append(new Option(arr[i + 1], arr[i]));
                                ++i;
                            }
                        }
                    });
                }
            });

            $("#id_district").change(function () {
                $("#school").prop('disabled', true);
                if ($("#id_district").val() > 0) {
                    $.ajax({
                        url: '/Users/AllSchool',
                        type: 'POST',
                        data: JSON.stringify({ 'param': $("#id_district").val(), 'role': $("#id_role").val() }),
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            data = data.replace(/\]/g, "");
                            data = data.replace(/\[/g, "");
                            data = data.replace(/\"/g, "");
                            data = data.replace(/\\/g, "\"");
                            var arr = data.toString().split(',');
                            $('#school').attr("disabled", false);
                            document.getElementById('school').innerHTML = '';
                            $('#school').append(new Option(arr[i + 1]));
                            for (var i = 0; i < arr.length; ++i) {
                                $('#school').append(new Option(arr[i + 1], arr[i]));
                                ++i;
                            }
                        }
                    });
                }
            });

        }
        else if (role == 4) {
            $("#region").css("display", "none");
            $("#zavuch").css("display", "none");
            $("#watcher").fadeIn();
            $("#areaCheck").css("display", "none");
            $("#districtCheck").css("display", "none");

            $("#district_watcher").prop('disabled', true);
            $("#school_watcher").prop('disabled', true);
            $('#letter').fadeOut();
            $("#area_watcher").change(function () {
                $("#district_watcher").prop('disabled', true);
                $("#school_watcher").prop('disabled', true);
                if ($("#area_watcher").val() > 0) {
                    $.ajax({
                        url: '/Users/AllDistrict',
                        type: 'POST',
                        data: JSON.stringify({ 'param': $("#area_watcher").val() }),
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            data = data.replace(/\]/g, "");
                            data = data.replace(/\[/g, "");
                            var arr = data.toString().split('\",\"');
                            arr[0] = arr[0].replace(/\"/, "");
                            arr[arr.length - 1] = arr[arr.length - 1].replace(/\"/, "");
                            $('#district_watcher').attr("disabled", false);
                            document.getElementById('district_watcher').innerHTML = '';
                            $('#district_watcher').append(new Option(arr[i + 1]));
                            for (var i = 0; i < arr.length; ++i) {
                                $('#district_watcher').append(new Option(arr[i + 1], arr[i]));
                                ++i;
                            }
                        }
                    });
                }
            });

            $("#district_watcher").change(function () {
                $("#school_watcher").prop('disabled', true);
                if ($("#district_watcher").val() > 0) {
                    $.ajax({
                        url: '/Users/AllSchool',
                        type: 'POST',
                        data: JSON.stringify({ 'param': $("#district_watcher").val(), 'role': $("#id_role").val() }),
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            data = data.replace(/\]/g, "");
                            data = data.replace(/\[/g, "");
                            data = data.replace(/\"/g, "");
                            data = data.replace(/\\/g, "\"");
                            var arr = data.toString().split(',');
                            $('#school_watcher').attr("disabled", false);
                            document.getElementById('school_watcher').innerHTML = '';
                            $('#school_watcher').append(new Option(arr[i + 1]));
                            for (var i = 0; i < arr.length; ++i) {
                                $('#school_watcher').append(new Option(arr[i + 1], arr[i]));
                                ++i;
                            }
                        }
                    });
                }
            });
            $("#subject_watcher").change(function () {
                if ($("#subject_watcher").val() > 0) {
                    document.getElementById('letter').innerHTML = '';
                    $.ajax({
                        url: '/Users/AllLetter',
                        type: 'POST',
                        data: JSON.stringify({ 'param': $("#subject_watcher").val(), 'param2': $("#school_watcher").val() }),
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            data = data.replace(/\]/g, "");
                            data = data.replace(/\[/g, "");
                            if (data != "") {
                                data = data.replace(/\"/g, "");
                                var arr = data.toString().split(',');
                                $('#letter').fadeIn();
                                document.getElementById('letter').innerHTML = '';
                                for (var i = 0; i < arr.length; ++i) {
                                    var checkbox = document.createElement("input");
                                    checkbox.setAttribute("type", "checkbox");
                                    checkbox.setAttribute("name", "letter_class");
                                    checkbox.setAttribute("id", "letter" + arr[i]);
                                    checkbox.setAttribute("value", arr[i]);
                                    var label = document.createElement('label')
                                    label.htmlFor = "letter" + arr[i];
                                    label.appendChild(document.createTextNode(" " + arr[i + 1]));
                                    document.getElementById('letter').appendChild(checkbox);
                                    document.getElementById('letter').appendChild(label);
                                    ++i;
                                }
                            }
                        }
                    });
                }
            });

        }
        else if (role == 5) {
            $("#region").css("display", "none");
            $("#zavuch").css("display", "none");
            $("#watcher").css("display", "none");
            $("#areaCheck").css("display", "none");
            $("#districtCheck").fadeIn();

            $("#district_dst").prop('disabled', true);
            $("#area_dst").change(function () {
                $("#district_dst").prop('disabled', true);
                $("#school_dst").prop('disabled', true);
                if ($("#area_dst").val() > 0) {
                    $.ajax({
                        url: '/Users/AllDistrict',
                        type: 'POST',
                        data: JSON.stringify({ 'param': $("#area_dst").val() }),
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            data = data.replace(/\]/g, "");
                            data = data.replace(/\[/g, "");
                            var arr = data.toString().split('\",\"');
                            arr[0] = arr[0].replace(/\"/, "");
                            arr[arr.length - 1] = arr[arr.length - 1].replace(/\"/, "");
                            $('#district_dst').attr("disabled", false);
                            document.getElementById('district_dst').innerHTML = '';
                            $('#district_dst').append(new Option(arr[i + 1]));
                            for (var i = 0; i < arr.length; ++i) {
                                $('#district_dst').append(new Option(arr[i + 1], arr[i]));
                                ++i;
                            }
                        }
                    });
                }
            });

        }
        else if (role == 6 || role == 8) {
            $("#region").css("display", "none");
            $("#zavuch").css("display", "none");
            $("#watcher").css("display", "none");
            $("#areaCheck").fadeIn();
            $("#districtCheck").css("display", "none");
        }
        else if (role == 7) {
            $("#zavuch").css("display", "none");
            $("#watcher").css("display", "none");
            $("#areaCheck").css("display", "none");
            $("#region").fadeIn();
            $("#districtCheck").css("display", "none");
        }
        else {
            $("#region").css("display", "none");
            $("#zavuch").css("display", "none");
            $("#watcher").css("display", "none");
            $("#areaCheck").css("display", "none");
            $("#districtCheck").css("display", "none");
        }
    }




});