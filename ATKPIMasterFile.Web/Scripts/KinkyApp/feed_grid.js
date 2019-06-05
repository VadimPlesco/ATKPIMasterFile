kinkyApp.FeedGridModel = function (wrapperMaxHeight) {
    var colCount = 0;
    var colWidth = 0;
    var margin = 0;
    var windowWidth = 0;
    var blocks = [];

    this.setupBlocks = function (feedwrapperId, selector) {
        windowWidth = $('#' + feedwrapperId).width();
        colWidth = $(selector).outerWidth();
        if (colWidth == null)
            return;
        blocks = [];
        colCount = Math.floor(windowWidth / (colWidth + margin * 2));
        for (var i = 0; i < colCount; i++) {
            blocks.push(margin);
        }
        positionBlocks(feedwrapperId, selector);
    };

    function positionBlocks(feedwrapperId, selector) {
        $('#' + feedwrapperId + ' ' + selector).each(function () {
            var min = Math.min.apply(Math, blocks);
            var index = $.inArray(min, blocks);
            var leftPos = margin + (index * (colWidth + margin));
            $(this).css({
                'left': leftPos + 'px',
                'top': min + 'px'
            });
            blocks[index] = min + $(this).outerHeight() + margin;
        });
        if (wrapperMaxHeight)
            $('#' + feedwrapperId).height(Math.min(wrapperMaxHeight, Math.max.apply(Math, blocks)));
        else
            $('#' + feedwrapperId).height(Math.max.apply(Math, blocks) + 10);
    }
};