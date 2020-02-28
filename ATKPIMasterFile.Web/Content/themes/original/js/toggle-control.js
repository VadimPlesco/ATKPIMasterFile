
atkpimfApp.fn.initToggle = function(el, image, readonly) {
    $(el).children('.mac-toggle__switcher').css({ 'background-image': 'url(/Content/themes/dzen/images/' + image + '.png)' });
    $(el).each(function(index) {
        if ($(this).children('input').attr('value') == 'False') {
            $(this).removeClass('on').addClass('off');
        }
        else {
            $(this).removeClass('off').addClass('on');
        }
    });

    if (!readonly) {
        $(el).one('click', function() {
            $(this).children('input').attr('value', ($(this).children('input').attr('value') == 'False' ? 'True' : 'False'));
            atkpimfApp.fn.initToggle(el, image, readonly);
        });
    }
}