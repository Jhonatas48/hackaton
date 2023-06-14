$(document).ready(function () {
    $(".home-btn").hover(
        function () {
            $(this).children(".home").addClass("home-orange");
        }
    );
    $(".home-btn").mouseleave(
        function () {
            $(this).children(".home").removeClass("home-orange");
        }
    );
});