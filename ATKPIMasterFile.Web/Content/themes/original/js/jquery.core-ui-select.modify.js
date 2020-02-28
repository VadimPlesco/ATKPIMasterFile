

(function ($) {


    var defaultOption = {
        appendInputBarOverlay: false,
        appendInputBarFull: false,
        appendInputBarSmall: false,
        appendToBody: false,
        jScrollPane: null,
        onInit: null,
        onFocus: null,
        onBlur: null,
        onOpen: null,
        onClose: null,
        onChange: null,
        onDestroy: null
    };

    var allSelects = [];

    function CoreUISelect(__elem, __options, __templates) {

        this.domSelect = __elem;
        this.settings = __options || defaultOption;
        this.isSelectShow = false;
        this.isSelectFocus = false;
        this.isJScrollPane = this.isJScrollPane();
        this.domJustCreatedOptionLength = 0;
        // templates
        this.templates = __templates ||
        {
            select: {
                container: '<div class="b-core-ui-select"></div>',
                value: '<span class="b-core-ui-select__value"></span>',
                button: '<span class="b-core-ui-select__button"></span>'
            },
            dropdown: {
                container: '<div class="b-core-ui-select__dropdown"></div>',
                wrapper: '<div class="b-core-ui-select__dropdown__wrap"></div>',
                list: '<ul class="b-core-ui-select__dropdown__list"></ul>',
                optionLabel: '<li class="b-core-ui-select__dropdown__label"></li>',
                item: '<li class="b-core-ui-select__dropdown__item"></li>'
            }
        }

        this.init(this.settings);
       
    }

    CoreUISelect.prototype.init = function() {
        this.settings.rememberLastSelected = this.domSelect.attr("remember-last-selected");
        this.settings.createFolderInside = this.domSelect.attr("create-folder-inside");
        this.settings.appendInputBarFull = this.domSelect.attr("input-bar-full");
        this.settings.appendInputBarSmall = this.domSelect.attr("input-bar-small");
        this.settings.appendInputBarOverlay = this.domSelect.attr("input-bar-overlay");

        if (this.settings.rememberLastSelected) {
            try {
                var $select = this;
                $("#" + this.domSelect.attr("id")).children("option").each(function(index) {
                    if ($(this).val() == localStorage["last-selected-value-of-" + $select.domSelect.attr("id")]) {
                        $("#" + $select.domSelect.attr("id")).val(localStorage["last-selected-value-of-" + $select.domSelect.attr("id")]).attr('selected', true);
                    }
                });
            } catch(e) {
                console.log(e);
            }
        }

        this.buildUI();
        this.hideDomSelect();
        if (this.domSelect.is(':disabled')) {
            this.select.addClass('disabled');
            return this;
        }
        if (this.isJScrollPane) this.buildJScrollPane();
        this.bindUIEvents();
        this.settings.onInit && this.settings.onInit.apply(this, [this.domSelect, 'init']);

        if (typeof this.settings.onChange === 'function') {
            this.settings.onChange();
        }
    };

    CoreUISelect.prototype.buildUI = function() {

        // Build select container
        this.select = $(this.templates.select.container)
            .insertBefore(this.domSelect);

        this.selectValue = $(this.templates.select.value)
            .appendTo(this.select);

        // TODO Add custom states for button
        this.selectButton = $(this.templates.select.button)
            .appendTo(this.select);

        // Build dropdown container
        this.dropdown = $(this.templates.dropdown.container);
        this.dropdownWrapper = $(this.templates.dropdown.wrapper).appendTo(this.dropdown);

        this.settings.appendToBody ? this.dropdown.appendTo($('body')) : this.dropdown.insertBefore(this.domSelect);


        // Append input bar (!! kpa)
        this.settings.createFolderInside ? this.dropdown.prepend('<div class="modal__ui-select__input-bar"><div onclick="' + this.settings.createFolderInside + '" class="shared__button shared__button-no-icon button-gold-to-gold button-create shared__width_90">' + atkpimfApp.data.localization.coreUISelectCreateFolder + '</div><div class="shared__ajax-preloader shared__right-float shared__display-none"></div>') : null;
        this.settings.appendInputBarFull ? this.dropdown.prepend('<div class="modal__ui-select__input-bar"><input class="modal-content__input-control modal-content__input-control-full core-ui-select-input"/><div class="shared__button shared__button-no-icon button-gold-to-gold button-create">' + atkpimfApp.data.localization.coreUISelectCreateFolder + '</div><div class="shared__ajax-preloader shared__right-float shared__display-none"></div>') : null;
        this.settings.appendInputBarSmall ? this.dropdown.prepend('<div class="modal__ui-select__input-bar"><input class="modal-content__input-control modal-content__input-control-small core-ui-select-input"/><div class="shared__button shared__button-no-icon button-gold-to-gold button-create-small">' + atkpimfApp.data.localization.coreUISelectCreate + '</div><div class="shared__ajax-preloader shared__right-float shared__display-none"></div>') : null;
        this.settings.appendInputBarOverlay ? this.dropdown.prepend('<div class="coreui-input-block-overlay"><button type="button" class="shared__button shared__button-no-icon button-gold-to-gold coreui-input-block-button button-create-overlay">' + atkpimfApp.data.localization.coreUISelectCreate + '</button><div class="coreui-input-overlay-container"><input placeholder="' + atkpimfApp.data.localization.coreUISelectCreateFolderTip + '" class="coreui-input-overlay"/></div><div class="shared__ajax-preloader shared__right-float shared__display-none"></div></div>') : null;

        // Build dropdown
        var self = this;
        this.dropdownList = $(this.templates.dropdown.list).appendTo(this.dropdownWrapper);
        this.domSelect.find('option').attr({ "todo": "app" });

        if (this.domSelect.find('option').length > 0) {
            this.domSelect.find('option').each($.proxy(this, 'addItems'));
            this.domSelect.children('option').each(function(index) {
                if ($(this).attr('personal') == 1) {
                    self.dropdownList.children(".b-core-ui-select__dropdown__item").eq(index).addClass("personal").append($("<span>", { 'class': "shared__icon shared__icon-user-folder-small" }));
                }
            });
        }
        else {
            var fakeFolders = eval(atkpimfApp.data.localization["FakeFolders"]);
            var self = this;
            this.domSelect.append("<option value='modal-input-create-folder'>" + atkpimfApp.data.localization["Name_your_folder"] + "</option>").find('option').each($.proxy(this, 'addItems'));

            this.dropdown.css("max-height", "800px");

            this.dropdown.find("ul").append("<li class='b-core-ui-select__dropdown__item-title'>" + atkpimfApp.data.localization["Suggestions_folder_name"] + "</li>");
            $.each(fakeFolders, function(i, val) {
                self.dropdown.find("ul").append("<li class='b-core-ui-select__dropdown__item-fake' value='" + val + "'>" + val + "</li>");
            });

            $(".b-core-ui-select__dropdown__item-fake").click(function() {
                self.hideDropdown();
                self.dropdown.find("li.b-core-ui-select__dropdown__item-title").remove();
                self.dropdown.find("li.b-core-ui-select__dropdown__item-fake").remove();
                self.inputBar.val($(this).attr("value"));
                self.createButton.click();
            });

        }
        var createButtonSelector = null;

        this.inputBar = this.dropdown.find("input");
        if (this.settings.appendInputBarFull) {
            createButtonSelector = "div.button-create";
        }
        if (this.settings.appendInputBarSmall) {
            createButtonSelector = "div.button-create-small";
        }
        if (this.settings.appendInputBarOverlay) {
            createButtonSelector = ".button-create-overlay";
        }

        this.createButton = this.dropdown.find(createButtonSelector);

        // Build dropdown
        this.dropdownItem = this.dropdown.find('.' + $(this.templates.dropdown.item).attr('class'));

        // Add classes for dropdown


        this.dropdownItem.filter(':first-child').addClass('first');
        this.dropdownItem.filter(':last-child').addClass('last');

        this.addOptionGroup();

        // Add placeholder value by selected option (kpa)


        if ((this.domSelect.find('option:first').val() == "modal-input-create-folder") && (this.settings.appendInputBarFull || this.settings.appendInputBarSmall)) {
            this.select.css("color", "#bebebe");
            this.setSelectValue(this.domSelect.find('option:first').html());
            this.dropdownList.children(".b-core-ui-select__dropdown__item:first").remove();
        } else {
            this.setSelectValue(this.getSelectedItem().html());
        }


        this.updateDropdownPosition();

        // Set current item form option:selected
        this.currentItemOfDomSelect = this.currentItemOfDomSelect || this.domSelect.find('option:selected');

    };

    CoreUISelect.prototype.hideDomSelect = function() {

        this.domSelect.addClass('b-core-ui-select__select_state_hide');
        this.domSelect.css({
            'top': this.select.position().top,
            'left': this.select.position().left
        });
    };

    CoreUISelect.prototype.showDomSelect = function() {
        this.domSelect.css({
            'top': 'auto',
            'left': 'auto'
        });
        this.domSelect.removeClass('b-core-ui-select__select_state_hide');
    };

    CoreUISelect.prototype.bindUIEvents = function() {
        // Bind plugin elements
        this.domSelect.on('focus', $.proxy(this, 'onFocus'));
        this.domSelect.on('blur', $.proxy(this, 'onBlur'));
        this.domSelect.on('change', $.proxy(this, 'onChange'));

        this.inputBar.on('keypress', $.proxy(this, 'onInputBarKeyPress'));

        this.select.on('click', $.proxy(this, 'onSelectClick'));
        this.createButton.on('click', $.proxy(this, 'onCreateButtonClick'));
        this.dropdownItem.on('click', $.proxy(this, 'onDropdownItemClick'));
    };

    CoreUISelect.prototype.getCurrentIndexOfItem = function(__item) {
        return this.domSelect.find('option').index($(this.domSelect).find('option:selected'));
    };

    CoreUISelect.prototype.scrollToCurrentDropdownItem = function(__item) {
        if (this.dropdownWrapper.data('jsp')) {
            this.dropdownWrapper.data('jsp').scrollToElement(__item);
            return this;
        }
    };

    CoreUISelect.prototype.buildJScrollPane = function() {
        this.dropdownWrapper.wrap($('<div class="j-scroll-pane"></div>'));
    };

    CoreUISelect.prototype.isJScrollPane = function() {
        if (this.settings.jScrollPane) {
            if ($.fn.jScrollPane) return true;
            else throw new Error('jScrollPane no found');
        }
    };

    CoreUISelect.prototype.initJScrollPane = function() {
        this.dropdownWrapper.jScrollPane(this.settings.jScrollPane);
    };

    CoreUISelect.prototype.showDropdown = function() {

        this.select.find(".b-core-ui-select__value").children(".shared__icon-lock-small").hide();


        this.settings.onOpen && this.settings.onOpen.apply(this, [this.domSelect, 'open']);
        if (!this.isSelectShow) {
            this.isSelectShow = true;

            this.dropdown.addClass('show').removeClass('hide');
            this.dropdown.scrollTop(0);

            if (this.isJScrollPane) this.initJScrollPane();
            this.updateDropdownPosition();
        }

        if (this.settings.appendInputBarFull || this.settings.appendInputBarSmall) {
            this.dropdown.css({ "border-top": "1px solid #e6e6e6" });
            this.inputBar.val("").focus();
        }
    };

    CoreUISelect.prototype.hideDropdown = function() {
        this.select.find(".b-core-ui-select__value").children("span.shared__icon-lock-small").css("display", "inline-block");

        if (this.isSelectShow) {
            this.isSelectShow = false;
            //this.select.removeClass('open');
            this.dropdown.removeClass('show').addClass('hide');
            this.settings.onClose && this.settings.onClose.apply(this, [this.domSelect, 'close']);
        }
    };

    CoreUISelect.prototype.hideAllDropdown = function() {
        for (var i in allSelects) {
            if (allSelects[i].hasOwnProperty(i)) {
                allSelects.dropdown.isSelectShow = false;
                allSelects.dropdown.domSelect.blur();
                allSelects.dropdown.addClass('hide').removeClass('show');
            }
        }
    };

    CoreUISelect.prototype.changeDropdownData = function(event) {
        if ((this.isSelectShow || this.isSelectFocus)) {
            this.currentItemOfDomSelect = this.domSelect.find('option:selected');
            this.dropdownItem.removeClass("selected");
            this.dropdownItem.eq(this.getCurrentIndexOfItem()).addClass("selected");
            this.scrollToCurrentDropdownItem(this.dropdownItem.eq(this.getCurrentIndexOfItem()));
            this.setSelectValue(this.currentItemOfDomSelect.html());

        }
    };

    CoreUISelect.prototype.onDomSelectChange = function(_is) {
        this.domSelect.on('change', $.proxy(this, 'onChange'));
        dispatchEvent(this.domSelect.get(0), 'change');
        if (!_is) this.settings.onChange && this.settings.onChange.apply(this, [this.domSelect, 'change']);
    };

    CoreUISelect.prototype.addListenerByServicesKey = function(event) {
        if (this.isSelectShow) {
            switch (event.which) {
            case 9:
// TAB
            case 13:
// ESQ
            case 27:
// ENTER
                this.hideDropdown();
                break;
            }
        }
    };

    CoreUISelect.prototype.onSelectClick = function() {
        if (!this.isSelectShow && !this.selectButton.hasClass('loading')) this.showDropdown();
        else this.hideDropdown();
        return false;
    };

    CoreUISelect.prototype.onInputBarKeyPress = function(event) {
        if (event.which == 0) {
            this.hideDropdown();
        }
        if (event.which == 13) {
            if (this.inputBar.val()) {
                this.createButton.hide();
                $(".shared__ajax-preloader").css({ "width": this.settings.appendInputBarFull ? 140 : 105 }).show();
                atkpimfApp.fn.createRequest(this, this.inputBar.val());
                event.preventDefault();
            }
            return false;
        }
    };

    $.fn.successVisual = function (select, text, value, personal) {
        if (select.settings.rememberLastSelected) {
            localStorage["last-selected-index-of-" + select.domSelect.attr("id")] = 0;
            localStorage["last-selected-text-of-" + select.domSelect.attr("id")] = text;
            localStorage["last-selected-value-of-" + select.domSelect.attr("id")] = value;
        }

        select.createButton.show();
        $(".shared__ajax-preloader").hide();
        select.domSelect.append("<option class='just-created-option' value='" + value + "' personal='" + personal + "'>" + text + "</option>");
        select.domJustCreatedOption = select.domSelect.find("option.just-created-option");
        select.domJustCreatedOption.attr({ "todo": "pre" });
        select.domJustCreatedOption.each($.proxy(select, 'addItems')).removeAttr('class');
        select.domJustCreatedOptionLength = select.domJustCreatedOptionLength + 1;
        select.dropdownList.children(".b-core-ui-select__dropdown__item").removeClass("first");
        select.dropdownList.children(".b-core-ui-select__dropdown__item").removeClass("last");
        select.justCreatedOption = select.dropdownList.find("li.b-core-ui-select__dropdown__item:first");
        select.justCreatedOption.addClass("first");
        if (personal) {
            select.justCreatedOption.addClass("personal").css({ 'background-color': '#FFFFA7' }).append('<span class="shared__icon shared__icon-user-folder-small"></span>');
        }
        select.dropdownList.find("li.b-core-ui-select__dropdown__item:last").addClass("last");


        $(select.justCreatedOption).click($.proxy(function (event) {
            select.select.css("color", "#121212");
            select.dropdownList.find(".b-core-ui-select__dropdown__item.selected").removeClass("selected");
            $(event.target).addClass('selected');
           
            select.domSelect.find("option").removeAttr("selected");
            select.setSelectValue($(event.target).html());
            select.domSelect.val(value);
            select.hideDropdown();
        }, select));

        select.justCreatedOption.click();
    };

    CoreUISelect.prototype.onCreateButtonClick = function() {
        if (this.inputBar.val()) {
            this.hideDropdown();
            this.createButton.hide();
            this.dropdown.find("li.b-core-ui-select__dropdown__item-title").remove();
            this.dropdown.find("li.b-core-ui-select__dropdown__item-fake").remove();
            $(".shared__ajax-preloader").css({ "width": this.settings.appendInputBarFull ? 140 : 105 }).show();
            atkpimfApp.fn.createRequest(this, this.inputBar.val());
        }
    };

    CoreUISelect.prototype.onFocus = function() {
        this.isDocumentMouseDown = false;
        this.isSelectFocus = true;
        this.select.addClass('focus');
        this.settings.onFocus && this.settings.onFocus.apply(this, [this.domSelect, 'focus']);
    };

    CoreUISelect.prototype.onBlur = function() {
        if (!this.isDocumentMouseDown) {
            this.isSelectFocus = false;
            this.select.removeClass('focus');
            this.settings.onBlur && this.settings.onBlur.apply(this, [this.domSelect, 'blur']);
        }
    };

    CoreUISelect.prototype.onChange = function() {
        this.settings.onChange && this.settings.onChange.apply(this, [this.domSelect, 'change']);
    };

    CoreUISelect.prototype.onDropdownItemClick = function(event) {


        this.dropdownList.find(".b-core-ui-select__dropdown__item").removeClass("selected");
        var item = $(event.currentTarget);
        if (!(item.hasClass('disabled') || item.hasClass('selected'))) {
            this.domSelect.off('change', $.proxy(this, 'onChange'));
            var index = this.dropdown.find('.' + $(this.templates.dropdown.item).attr('class')).index(item) - this.domJustCreatedOptionLength;

            this.dropdownItem.removeClass('selected');
            this.dropdownItem.eq(index).addClass('selected');
            this.domSelect.find('option').removeAttr('selected');
            this.domSelect.find('option').eq(index).prop('selected', true);
            this.setSelectValue(this.getSelectedItem().html());
            this.onDomSelectChange(true);
            if (this.settings.rememberLastSelected) {
                localStorage["last-selected-index-of-" + this.domSelect.attr("id")] = index;
                localStorage["last-selected-text-of-" + this.domSelect.attr("id")] = item.text();
                localStorage["last-selected-value-of-" + this.domSelect.attr("id")] = $("#" + this.domSelect.attr("id")).val();
            }
        }
        this.hideDropdown();
        return false;
    };

    CoreUISelect.prototype.onDocumentMouseDown = function(event) {
        this.isDocumentMouseDown = true;
        if ($(event.target).closest(this.select).length == 0 && $(event.target).closest(this.dropdown).length == 0) {
            if ($(event.target).closest('option').length == 0) { // Hack for Opera
                this.isDocumentMouseDown = false;
                this.hideDropdown();
            }
        }
        return false;
    };

    CoreUISelect.prototype.updateDropdownPosition = function() {
        if (this.isSelectShow) {
            if (this.settings.appendToBody) {
                this.dropdown.css({
                    'position': 'absolute',
                    'top': this.select.offset().top + this.select.outerHeight(true),
                    'left': this.select.offset().left,
                    'z-index': '9999'
                });
            } else {
                this.dropdown.css({
                    'position': 'absolute',
                    'top': this.select.position().top + this.select.outerHeight(true) - 3,
                    'left': this.select.position().left,
                    'z-index': '9999',
                });

                //console.log($(this.select).css("width"));

                this.dropdown.width(parseInt($(this.select).css("width")) + 20);

            }
        }
    };

    CoreUISelect.prototype.setSelectValue = function(_text) {
        this.selectValue.html(_text);
    };

    CoreUISelect.prototype.isOptionGroup = function() {
        return this.domSelect.find('optgroup').length > 0;
    };

    CoreUISelect.prototype.addOptionGroup = function() {
        var optionGroup = this.domSelect.find('optgroup');
        for (var i = 0; i < optionGroup.length; i++) {
            var index = this.domSelect.find("option").index($(optionGroup[i]).find('option:first-child'))
            $(this.templates.dropdown.optionLabel)
                .text($(optionGroup[i]).attr('label'))
                .insertBefore(this.dropdownItem.eq(index));
        }
    };

    CoreUISelect.prototype.addItems = function (index, el) {
        var el = $(el);
        var elText = el.text();
        //var elValue = el.val();
        var item = $(this.templates.dropdown.item).text(elText);
        if (el.attr("disabled")) item.addClass('disabled');
        if (el.prop("selected")) {
            this.domSelect.find('option').removeAttr('selected');
            item.addClass('selected');
            el.prop('selected', true);
        }
        if (el.attr("todo") == "app" && !this.settings.appendInputBarOverlay) {
            item.appendTo(this.dropdownList)
        }
        else if (!this.settings.appendInputBarOverlay) {
            item.prependTo(this.dropdownList);
        }
        if (el.attr("todo") == "app" && this.settings.appendInputBarOverlay && !item.hasClass('selected')) {
            item.appendTo(this.dropdownList)
        }
        else if (this.settings.appendInputBarOverlay) {
            item.prependTo(this.dropdownList);
            el.prependTo(this.domSelect);
            //console.info(elValue);
        }

    }

    CoreUISelect.prototype.getSelectedItem = function () {
        return this.dropdown.find('.selected').eq(0);
    }

    CoreUISelect.prototype.update = function () {
        this.destroy();
        this.init();
    }

    CoreUISelect.prototype.destroy = function () {
        // Unbind plugin elements
        this.domSelect.off('focus', $.proxy(this, 'onFocus'));
        this.domSelect.off('blur', $.proxy(this, 'onBlur'));
        this.domSelect.off('change', $.proxy(this, 'onChange'));
        this.select.off('click', $.proxy(this, 'onSelectClick'));
        this.createButton.off('click', $.proxy(this, 'onCreateButtonClick'));
        this.dropdownItem.off('click', $.proxy(this, 'onDropdownItemClick'));
        // Remove select container
        this.select.remove();
        this.dropdown.remove();
        this.showDomSelect();
        this.settings.onDestroy && this.settings.onDestroy.apply(this, [this.domSelect, 'destroy']);
    }


    $.fn.coreUISelect = function (__options, __templates) {
        return this.each(function () {
            var select = $(this).data('coreUISelect');
            if (__options == 'destroy' && !select) return;
            if (select) {
                __options = (typeof __options == "string" && select[__options]) ? __options : 'update';
                select[__options].apply(select);
                if (__options == 'destroy') {
                    $(this).removeData('coreUISelect');
                    for (var i = 0; i < allSelects.length; i++) {
                        if (allSelects[i] == select) {
                            allSelects.splice(i, 1);
                            break;
                        }
                    }
                }
            } else {
                select = new CoreUISelect($(this), __options, __templates);
                allSelects.push(select);
                $(this).data('coreUISelect', select);
            }

        });
    };

    function dispatchEvent(obj, evt, doc) {
        var doc = doc || document;
        if (obj !== undefined || obj !== null) {
            if (doc.createEvent) {
                var evObj = doc.createEvent('MouseEvents');
                evObj.initEvent(evt, true, false);
                obj.dispatchEvent(evObj);
            } else if (doc.createEventObject) {
                var evObj = doc.createEventObject();
                obj.fireEvent('on' + evt, evObj);
            }
        }
    }

    $(document).on('mousedown', function (event) {
        for (var i = 0; i < allSelects.length; i++) {
            allSelects[i].onDocumentMouseDown(event);
        }
    });


    $(window).on('resize', function (event) {
        for (var i = 0; i < allSelects.length; i++) {
            allSelects[i].updateDropdownPosition(event);
        }
    });

})(jQuery);