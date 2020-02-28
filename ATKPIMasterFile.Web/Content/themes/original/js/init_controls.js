/*
atkpimfApp.fn.activeButton = function (el) {
    $(el).css({ "line-height": "36px", "padding-bottom": "19px" });
};
*/

jQuery.fn.extend({
    insertAtCaret: function (myValue) {
        return this.each(function (i) {
            if (document.selection) {
                //For browsers like Internet Explorer
                this.focus();
                var sel = document.selection.createRange();
                sel.text = myValue;
                this.focus();
            }
            else if (this.selectionStart || this.selectionStart == '0') {
                //For browsers like Firefox and Webkit based
                var startPos = this.selectionStart;
                var endPos = this.selectionEnd;
                var scrollTop = this.scrollTop;
                this.value = this.value.substring(0, startPos) + myValue + this.value.substring(endPos, this.value.length);
                this.focus();
                this.selectionStart = startPos + myValue.length;
                this.selectionEnd = startPos + myValue.length;
                this.scrollTop = scrollTop;
            } else {
                this.value += myValue;
                this.focus();
            }
        });
    }
});

atkpimfApp.fn.hasScrollBar = function () {
    return this.get(0).scrollHeight > this.height();
};

atkpimfApp.fn.initTextarea = function (selector) {
    $("#" + selector).each(function (index) { $(this).val($(this).attr("title")); });
    $("#" + selector).focus(function () { if ($(this).val() == $(this).attr("title")) { $(this).css({ "color": "#000" }).val(""); }; }).focusout(function () { if ($(this).val().length == 0) { $(this).val($(this).attr("title")).css({ "color": "#bebebe" }); } else { $(this).css({ "color": "#121212" }); }; });
};

atkpimfApp.fn.initSelect = function (selector, options) {

    return $(selector).coreUISelect(options || {});
};

atkpimfApp.fn.initSelectAddEditPhoto = function (selector) {

    $(selector).coreUISelect(
        {
            //onChange: function(a, b, c) {
            //    var isPersonal = $('.modal-photo-add-edit-content-wrapper option:selected').attr('personal') == '1';

            //    if (isPersonal) {
            //        $('.modal-photo-add-edit-more-photos-button').hide();

            //        var imgsToDelete = $('.modal-photo-add-edit-img-wrapper .modal-photo-add-edit-img-pos')
            //            .find('.close-button')
            //            .toArray()
            //            .slice(1);

            //        for (var index = 0; index < imgsToDelete.length; index++) {
            //            $(imgsToDelete[index]).trigger('click');
            //        }
            //    }
            //    else {
            //        $('.modal-photo-add-edit-more-photos-button').show();
            //    }
            //}
        },
        {
            select: {
                container: '<div class="b-core-ui-select modal-photo-add-edit-dropdown"></div>',
                value: '<span class="b-core-ui-select__value"></span>',
                button: '<span class="b-core-ui-select__button"></span>'
            },
            dropdown: {
                container: '<div id="modal-photo-add-edit-dropdown-select" class="b-core-ui-select__dropdown modal-photo-add-edit-dropdown-select"></div>',
                wrapper: '<div class="modal-photo-add-edit-dropdown-wrap"></div>',
                list: '<ul class="b-core-ui-select__dropdown__list"></ul>',
                optionLabel: '<li class="b-core-ui-select__dropdown__label"></li>',
                item: '<li class="b-core-ui-select__dropdown__item"></li>'
            }
        });


};

atkpimfApp.fn.initInput = function (selector) {
    selector.each(function () { $(this).val($(this).val() + " " + $(this).attr("ei")); });
    selector.focusin(function () { $(this).val($(this).val().replace(/[^0-9]/g, '')); });
    selector.focusout(function () { $(this).val().replace(/[^0-9]/g, '') != "" ? ($(this).val($(this).val() + " " + $(this).attr("ei"))) : "11" });
};


atkpimfApp.fn.initRadio = function () {
    $(".shared__radiobutton").mousedown(
    function () {
        $(this).parent("div").parent("div").find(".shared__radiobutton").css("background-position", "0 0");
        $(this).children("input").attr("checked", "checked");
        $(this).css("background-position", "0 -20px");
    });

    $(".shared__radiobutton input").each(function () {
        if ($(this).attr("checked")) {
            $(this).parent(".shared__radiobutton").mousedown();
        }
    });
};

atkpimfApp.fn.initRadioGroup = function (groupName, onChangeCallback) {
    $('input[name=' + groupName + ']').parent('.shared__radiobutton').on('mousedown', function () {
        $('input[name=' + groupName + ']').removeAttr('checked')
            .parent('.shared__radiobutton')
            .css('background-position', '0 0');
        $(this).find('input')
            .attr('checked', 'checked')
            .parent('.shared__radiobutton')
            .css('background-position', '0 -20px');

        onChangeCallback = onChangeCallback || function () { };

        onChangeCallback($(this).find('input').val(), $(this).find('input'));
    });

    $('input[name=' + groupName + ']').each(function () {
        if ($(this).attr('checked')) {
            $(this).parent('.shared__radiobutton')
                .css('background-position', '0 -20px');
        }
    });
};

atkpimfApp.fn.buttonGroupMouseover = function (el) {
    //if (atkpimfApp.fn.isMobile() === false) {
    //    $(el).find(".two-button-group").removeClass("invisible");
    //}
    
};

atkpimfApp.fn.buttonGroupMouseout = function (el) {
    var buttons = $(el).find(".two-button-group");
    var mainPopup = $(el).find("#main__popup");
    if (mainPopup.length == 0 || mainPopup.css("display") == "none") {
        buttons.addClass("invisible");
    }
    else {
        buttons.removeClass("invisible");
    }
};


atkpimfApp.fn.initCurrentLink = function () {
    $("[data-href]").each(function () {
        var el = $(this);


        if (el.attr("data-href") == document.location.pathname)
            el.attr("class", el.attr("data-selected"))
        else
            el.attr("class", el.attr("data-normal"));
    });

    //var curr = $("[data-href='" + document.location.pathname + "']");
    //curr.attr("class", curr.attr("data-selected"));

    //$("[data-href]").removeClass("menu-holder__menu-item-selected").addClass("menu-holder__menu-item");
    //$("[data-href='" + document.location.pathname + "']").removeClass("menu-holder__menu-item").addClass("menu-holder__menu-item-selected");
};


atkpimfApp.fn.initTypeahead = function (selector, limit, searchUrl, minLength, callback) {
    var tagSelector = $(selector);
    tagSelector.typeahead({
        limit: limit,
        remote: searchUrl + "?query=%QUERY",
        minLength:minLength ? minLength : 0, //когда 0, тогда при инициализации подтягивается дефотный список городов
        //close: function () { alert(1); }

    }).bind("typeahead:closed", callback);

};



//atkpimfApp.fn.initUserDetailsAddButton = function (profileAddButtonSelector, profileAddButtonBlockInnerSelector) {
//    var profileAddButton = $(profileAddButtonSelector);
//    $(profileAddButtonBlockInnerSelector).hover(
//        function () { profileAddButton.addClass("profile__add-button-hover") },
//        function () { profileAddButton.removeClass("profile__add-button-hover") }
//    );
//};


atkpimfApp.fn.initUserDetailsUpdateStatus = function (userStatusSelector, url) {
    var userStatus = $(userStatusSelector);
    var lastText = userStatus.text();

    userStatus.on("focusout", function (e) {
        var content = userStatus.text();
        if (lastText != content) {
            $.ajax({
                url: url,
                type: 'POST',
                data: {
                    value: content
                },
                success: function () {
                    lastText = content;
                }
            });
        }
    });

    userStatus.keypress(function (e) {
        var characterlimit = 500;
        var current = $(this).text().length;
        if (e.keyCode == 13)
            userStatus.blur().next().focus();
        if ((current >= characterlimit && (e.keyCode != 8)) || e.keyCode == 13)
            e.preventDefault();

    });

    userStatus.on("focus", function () {
        var range = document.createRange();
        range.selectNodeContents(this);
        var sel = window.getSelection();
        sel.removeAllRanges();
        sel.addRange(range);
    });
};

atkpimfApp.fn.socialShare = function (socialNetwork, e, element) {
    var windowOptions = 'toolbar=0,resizable=0,status=1,width=450,height=430';
    var modal = document.getElementById("modal-outer");

    if (modal == null || typeof (modal) == 'undefined') {
        window.open(element.dataset.shareUrl, "", windowOptions);
    }   
    else {
        element.href = element.dataset.shareUrl;
        element.target = "_blank"; 
    }
    atkpimfApp.analytics.trackEvent(atkpimfApp.analytics.eventType.share, { 'network': socialNetwork });

};
