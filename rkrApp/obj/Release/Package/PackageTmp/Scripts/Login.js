$(document).ready(function(){
    $("#new_pass").submit(function (e) {
        if ($("#pass").val() != $("#pass2").val()) {
            e.preventDefault();
            $("#new_pass .error").text("Пароли не совпадают");
        }
        else {
            $("#new_pass .error").text("");
        }
    });
});