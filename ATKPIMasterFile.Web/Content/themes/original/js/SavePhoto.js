var kinkyApp = kinkyApp || {};
kinkyApp.savePhoto = {};
kinkyApp.savePhoto = (function () {
    var self = {};
    self.checkInterestLevel = function (textarea, input, tags) {
        var textarea = $(textarea);
        var tags = $(tags);
        var input = $(input);
        var statusElement = $("#modal-photo-add-edit-rate-text");
        var helpElement = $("#modal-photo-add-edit-rate-help");
        var barElement = $("#modal-photo-add-edit-rate-bar-progress");
        var tagRemoveElement = $(".tm-tag-remove");
        var rateBarText = $("#modal-photo-add-edit-rate-bar-text");

        var status = ["low", "medium", "high", "maximum"]; // status categories, for css purposes
        var statusState; // rating status state

        var tagPercentWeight = 0.05; // each tag adds up 5%
        var targetLength = (tags.length>0)?70:150; // number of symbols to target
        var lowLevel = 0.5; // 50%
        var mediumLevel = 0.8; // 100%
        var topLevel = 1; // 100%
        var tagsNumberTarget = (1 - mediumLevel) / tagPercentWeight;
        var tagsNumberMaxTarget = (1 - topLevel) / tagPercentWeight;

        var levelPoints = function () {
            var length = textarea.val().replace(/\s/g, "").length;

            var inputlength = (($(input).length>0)?(input.val().replace(/\s/g, "").length):0);

            
            /*
            if (input.length > 0) {
                var inputlength = input.val().replace(/\s/g, "").length;
                var width = (inputlength / targetLength).toFixed(2);
            } else {
                var width = (length / targetLength).toFixed(2); // status bar width
            }
            */
            
            if (tags.length == 0) {
                //var width = (length/1.3 / targetLength + inputlength/3.5 / targetLength).toFixed(2); // status bar width
                var width = (length / targetLength + inputlength / 3.5 / targetLength).toFixed(2); // status bar width
            } else {
                var width = ((length / targetLength).toFixed(2) < 0.1 ? 0.1 : (length / targetLength).toFixed(2));

                //console.log(width)
            }
            

            //var width = (length / targetLength).toFixed(2);

            //console.log(width);
            //if (tags.length > 0) { var tagslength = (length / targetLength).toFixed(2); }

            
            //var width = ((tags.length == 0) ? inputlength : tagslength); // status bar width
            //console.log("tagslength" + tagslength);
//            console.log("inputlength" + inputlength);

            var widthPercentString = (width * 100).toString() + "%"; // status bar width string
            var tagsNumber = $(".tm-tag").length;

            if (length > (targetLength - 1)) {
                statusState = status[2];
                statusElement.text(kinkyApp.data.localization.Post_rating_level_high);
                if (tags.length > 0) {
                    helpElement.text(kinkyApp.data.localization.Post_rating_level_more_tags);
                }

                tagsNumber = (tagsNumber >= tagsNumberMaxTarget) ? tagsNumberMaxTarget : tagsNumber;

                width = (width < topLevel) ? width * 100 : width * 100 + tagPercentWeight * 100 * tagsNumber;
                width = (width < 100) ? width : 100;

                if (tagsNumber == tagsNumberMaxTarget) {
                    helpElement.text("");

                    if (width == 100) {
                        statusElement.text(kinkyApp.data.localization.Post_rating_level_max);
                        statusState = status[3];
                    }
                }
                barElement.width(width.toString() + "%");
            }

           

            if (length <= (targetLength - 1)) {
                statusState = status[2];
                statusElement.text(kinkyApp.data.localization.Post_rating_level_high);
                if (tags.length > 0) {
                    helpElement.text(kinkyApp.data.localization.Post_rating_level_more_tags);
                }

                tagsNumber = (tagsNumber >= tagsNumberTarget) ? tagsNumberTarget : tagsNumber;
                width = (width < mediumLevel) ? width * 100 : width * 100 + tagPercentWeight * 100 * tagsNumber;
                width = (width < 100) ? width : 100;

                if ((tagsNumber == tagsNumberTarget) && (width < topLevel)) {
                    helpElement.text("");
                    if (width == 100) {
                        statusElement.text(kinkyApp.data.localization.Post_rating_level_maxi);
                        statusState = status[3];
                    }
                }
                barElement.width(width.toString() + "%");
            }

            if (length <= targetLength * mediumLevel) {
                statusState = status[1];
                statusElement.text(kinkyApp.data.localization.Post_rating_level_medium);
                barElement.width(widthPercentString);
                helpElement.text(kinkyApp.data.localization.Post_rating_level_more_text);
            }

            if (length <= targetLength * lowLevel) {
                statusState = status[0];
                statusElement.text(kinkyApp.data.localization.Post_rating_level_low);
                barElement.width(widthPercentString);
                helpElement.text(kinkyApp.data.localization.Post_rating_level_more_text);
            }

            if (width >= 60) {
                rateBarText.hide();
            }
            else {
                rateBarText.show();
            }

            statusElement.attr("data-form-interest-rating", statusState);
            barElement.attr("data-form-bar-rating", statusState);
        };
        levelPoints();

        textarea.on("keyup", function () {
            levelPoints();

        });

        tags.on("typeahead:selected", function (e) {
            levelPoints();

        });
        tags.on("keyup", function (e) {
            if (e.which == 13) {
                levelPoints();
            }
        });

        input.on("keyup", function (e) {
            levelPoints();
        });

        $(document).on("click", tagRemoveElement, function (e) {
            levelPoints();
        });
    };

    return self;
})();