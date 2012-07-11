$(function() {
    BindUI();
});

function BindUI() {
    $(".popup-link").click(function(e) {
        e.preventDefault();
        var href = $(this).attr('href');
        $.get(href, function(data) {
            $('#PopUp').html(data);
        });
    });
}

function Wait(show) {
    if (show) {
        $(".loader")
            .height($(document).height())
            .fadeIn(25);
    } else {
        $(".loader").hide();
    }
}

/***********************************************************
        Ajax Link / AJAX Form
************************************************************/
$.fn.ajaxlink = function (method, onComplete) {
    return this.each(function () {
        var href = $(this).attr('href');
        $(this).click(function (e) {
            e.preventDefault();
            Wait(true);
            $.ajax({
                url: href,
                type: method,
                success: function (data) {
                    onComplete(data);
                    Wait(false);
                }
            });
        });
    });
};

$.fn.ajaxform = function (onComplete) {
    return this.each(function () {
        $(this).submit(function (e) {
            e.preventDefault();
            Wait(true);
            var href = $(this).attr('action');
            var postData = $(this).serialize();

            $.post(href, postData, function (data) {
                onComplete(data);
                Wait(false);
            });
        });
    });
};