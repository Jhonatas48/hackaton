// Mexe com o foco do envoltório do textbox (pq eu n consigo modificar o input diretamente ent preciso estilizar a div)
$(document).ready(function () {
    $('.textbox').on('focus', function () {
        $(this).parent().parent('.env').addClass('env-margin');
    });

    $('.textbox').on('blur', function () {
        $(this).parent().parent('.env').removeClass('env-margin');
    });

    $('.env').on('click', function () {
        $(this).children().children('.textbox').focus();
    });
});