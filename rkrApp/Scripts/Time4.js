//$(document).ready(function () {
//    var date = new Date();

//    var role = 0;
//    var subject = 0;
//    $.ajax({
//        url: '/Home/GetUserRole',
//        type: 'POST',
//        contentType: "application/json; charset=utf-8",
//        dataType: "json",
//        success: function (data) {
//            role = data;
//            if (role == 4 || role == 5 || role == 6) {
//                $.ajax({
//                    url: '/Home/GetUserSubject',
//                    type: 'POST',
//                    contentType: "application/json; charset=utf-8",
//                    dataType: "json",
//                    success: function (data) {
//                        subject = data;
//                        r();
//                    }
//                });
//            }
//            else {
//                r();
//            }
//        }
//    });

    
//    function r() {
//        //if (date.getDate() == 26 && date.getHours()>=9) {
//        //    $(".school_btn").css("display", "none");
//        //}
//        $(".school_btn").css("display", "none");
//        $(".watcher_btn").css("display", "none");
//        $(".result_btn").css("display", "none");
//        //if (date.getDate() == 28) {
//        //    if (role == 4) {
//        //        $(".watcher_btn").css("display", "none");
//        //    }
//        //    if (role == 5 && subject == 2) {
//        //        $(".result_btn").css("display", "none");
//        //    }
//        //}
//        //if (date.getDate() == 1) {
//        //    //if (role == 5) {
//        //    //    $(".result_btn").css("display", "none");
//        //    //}
//        //    if (role == 5) {
//        //        $(".result_btn").css("display", "none");
//        //    }
//        //    //if (role == 6 && subject == 2) {
//        //    //    $(".result_btn").css("display", "none");
//        //    //}
//        //}
//        //if (date.getDate() >= 2 && date.getDate() <= 20) {
//        //    if (role == 5 || role==6) {
//        //        $(".result_btn").css("display", "none");
//        //    }
//        //}
//        //if (role == 8 && date.getDate() > 6 && date.getDate() <= 20) {
//        //    $(".result_btn").css("display", "none");
//        //}
//    }

//});