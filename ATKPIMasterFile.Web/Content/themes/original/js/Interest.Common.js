atkpimfApp.fn.createMyFirstInterest = function (el) {
    $(el).hide();
    $(".b-core-ui-select").show().click();
    setTimeout(function () {
        $(".b-core-ui-select").next(".b-core-ui-select__dropdown").children(".modal__ui-select__input-bar").children("input.core-ui-select-input").focus();
    }, 10);
};