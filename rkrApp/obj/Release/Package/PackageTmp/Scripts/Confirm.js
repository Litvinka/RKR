//Удаление шифра
$(function ($) {
    $('.delete_chiper').click(function (e) {
        e.preventDefault();
        var href = (e.target.tagName == "SPAN") ? e.target.parentNode.href : e.target.href;
        $('#confirm').modal({
            closeHTML: "<a href='#' class='modal-close'>x</a>",
            position: ["20%",],
            overlayId: 'confirm-overlay',
            containerId: 'confirm-container',
            onShow: function (dialog) {
                var modal = this;
                $('.yes', dialog.data[0]).click(function () {
                    $.ajax({
                        url: href,
                        type: 'GET',
                        success: function () {
                            location.reload();
                            modal.close();
                        }
                    });
                    
                });
            }
        });
	});
});


//Удаление пароля
$(function ($) {
    $('#delete_password').click(function (e) {
        e.preventDefault();
        var href = e.target.href;
        $('#confirm').modal({
            closeHTML: "<a href='#' class='modal-close'>x</a>",
            position: ["20%",],
            overlayId: 'confirm-overlay',
            containerId: 'confirm-container',
            onShow: function (dialog) {
                var modal = this;
                $('.yes', dialog.data[0]).click(function () {
                    $.ajax({
                        url: href,
                        type: 'GET',
                        success: function () {
                            location.reload();
                            modal.close();
                        }
                    });
                    
                });
            }
        });
	});
});


//Удаление скана
$(function ($) {
    $('.delete_blank').click(function (e) {
        e.preventDefault();
        var href = (e.target.tagName == "SPAN") ? e.target.parentNode.href : e.target.href;
        $('#confirm').modal({
            closeHTML: "<a href='#' class='modal-close'>x</a>",
            position: ["20%",],
            overlayId: 'confirm-overlay',
            containerId: 'confirm-container',
            onShow: function (dialog) {
                var modal = this;
                $('.yes', dialog.data[0]).click(function () {
                    $.ajax({
                        url: href,
                        type: 'GET',
                        success: function () {
                            location.reload();
                            modal.close();
                        }
                    });

                });
            }
        });
    });
});


//Удаление пользователя
$(function ($) {
    $('.delete_user').click(function (e) {
        e.preventDefault();
        var href = (e.target.tagName == "SPAN") ? e.target.parentNode.href : e.target.href;
        $('#confirm').modal({
            closeHTML: "<a href='#' class='modal-close'>x</a>",
            position: ["20%",],
            overlayId: 'confirm-overlay',
            containerId: 'confirm-container',
            onShow: function (dialog) {
                var modal = this;
                $('.yes', dialog.data[0]).click(function () {
                    $.ajax({
                        url: href,
                        type: 'GET',
                        success: function () {
                            location.reload();
                            modal.close();
                        }
                    });

                });
            }
        });
    });
});


//Удаление ошибки
$(function ($) {
    $('.DeleteError').click(function (e) {
        e.preventDefault();
        var href = (e.target.tagName == "SPAN") ? e.target.parentNode.href : e.target.href;
        $('#confirm').modal({
            closeHTML: "<a href='#' class='modal-close'>x</a>",
            position: ["20%",],
            overlayId: 'confirm-overlay',
            containerId: 'confirm-container',
            onShow: function (dialog) {
                var modal = this;
                $('.yes', dialog.data[0]).click(function () {
                    $.ajax({
                        url: href,
                        type: 'GET',
                        success: function () {
                            location.reload();
                            modal.close();
                        }
                    });

                });
            }
        });
    });
});
//Удаление результата
$(function ($) {
    $('.DeleteResult').click(function (e) {
        e.preventDefault();
        var href = (e.target.tagName == "SPAN") ? e.target.parentNode.href : e.target.href;
        $('#confirm').modal({
            closeHTML: "<a href='#' class='modal-close'>x</a>",
            position: ["20%",],
            overlayId: 'confirm-overlay',
            containerId: 'confirm-container',
            onShow: function (dialog) {
                var modal = this;
                $('.yes', dialog.data[0]).click(function () {
                    $.ajax({
                        url: href,
                        type: 'GET',
                        success: function () {
                            location.reload();
                            modal.close();
                        }
                    });

                });
            }
        });
    });
});
//Удаление студента
$(function ($) {
    $('.DeleteStudent').click(function (e) {
        e.preventDefault();
        var href = (e.target.tagName == "SPAN") ? e.target.parentNode.href : e.target.href;
        $('#confirm').modal({
            closeHTML: "<a href='#' class='modal-close'>x</a>",
            position: ["20%",],
            overlayId: 'confirm-overlay',
            containerId: 'confirm-container',
            onShow: function (dialog) {
                var modal = this;
                $('.yes', dialog.data[0]).click(function () {
                    $.ajax({
                        url: href,
                        type: 'GET',
                        success: function () {
                            location.reload();
                            modal.close();
                        }
                    });

                });
            }
        });
    });
});
//Удаление класса
$(function ($) {
    $('.DeleteClass').click(function (e) {
        e.preventDefault();
        var href = (e.target.tagName == "SPAN") ? e.target.parentNode.href : e.target.href;
        $('#confirm').modal({
            closeHTML: "<a href='#' class='modal-close'>x</a>",
            position: ["20%",],
            overlayId: 'confirm-overlay',
            containerId: 'confirm-container',
            onShow: function (dialog) {
                var modal = this;
                $('.yes', dialog.data[0]).click(function () {
                    $.ajax({
                        url: href,
                        type: 'GET',
                        success: function () {
                            location.reload();
                            modal.close();
                        }
                    });

                });
            }
        });
    });
});










