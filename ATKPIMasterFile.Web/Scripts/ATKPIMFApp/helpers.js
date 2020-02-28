atkpimfApp.fileUpload = function (parameters) {
    var defaultOptions = {
        acceptFileTypes: /(\.|\/)(gif|jpe?g|png)$/i,
        maxFileSize: 10 * 1024 * 1024,
        beforeFormAppend: undefined,
        url: undefined,
        done: undefined,
        fail: undefined,
        event: undefined,
        formData: undefined,
        progress: undefined,
        showProgressFileSize: 0 * 1024 * 1024,
        doneSuccess: undefined,
        reader: undefined,
        readerSuccess: undefined
    };
    var options = defaultOptions;
    for (var key in parameters) {
        if (parameters.hasOwnProperty(key)) {
            if (parameters[key] !== undefined) {
                options[key] = parameters[key];
            }
        }
    }
    console.log(parameters);
    var fileInput = document.getElementById("fileUpload");
    if (fileInput) 
        document.body.removeChild(fileInput);

    fileInput = document.createElement("input");
    fileInput.id = "fileUpload";
    fileInput.name = "file";
    fileInput.type = "file";
    fileInput.accept = "image/jpeg, image/jpg, image/png, image/gif";
    document.body.appendChild(fileInput);


    fileInput.addEventListener("change", function () {
        var file = this.files[0];
        var content = new FormData();
        var regexp = /(\.|\/)(gif|jpe?g|png)$/i;
        if (!file) { return; }
        if (file.size > options.maxFileSize) { return; }
        if (!file.type.match(regexp)) { return; }
        if (options.beforeFormAppend) {
            options.beforeFormAppend(this);
        }
        if (options.reader && options.readerSuccess) {
            options.reader(file, options.readerSuccess);
        }

        if (options.formData) {
            content.append("sk", options.formData);
        }

        content.append("file", file);

        var ajax = new XMLHttpRequest();

        ajax.upload.addEventListener("progress", function (oEvent) {
            if (oEvent.lengthComputable) {
                var percentComplete = Math.round((oEvent.loaded / oEvent.total) * 100);
                if (options.progress && file.size > options.showProgressFileSize) {
                    options.progress(percentComplete);
                }

            } else {

            }
        }, false);

        ajax.addEventListener('load', function () {
            //alert(ajax.status);!!!!
            console.log(options);
            //if (ajax.status == 200) {
                
                if (options.done && options.doneSuccess) {
                    options.done(this, options.doneSuccess)
                } else if (options.done) {
                    options.done(this);
                }
            //} else {
            //    if (options.fail) {
            //        alert('fail');
            //        options.fail();
            //    }
            //}

        }, false);

        ajax.upload.addEventListener("error", function () {
            if (options.fail) {
                alert('fail2');
                options.fail();
            }
        }, false);

        if (options.url) {
            ajax.open("POST", options.url, true);
        }
        ajax.withCredentials = true;

        ajax.send(content);
    }, false);


    fileInput.click();

    if (options.event) {
        if (options.event.stopPropagation) {
            options.event.stopPropagation();
        } else {
            options.event.cancelBubble = true; // IE model
        }

    }
};

atkpimfApp.UploadPhoto = function (url, done, event, callback) {
    
    atkpimfApp.fileUpload({
        url: url,
        done: done,
        event: event,
        beforeFormAppend: callback,//callback,!!!!
        fail: function () {
            console.log(data);
            alert("Unable_upload_photo");//(atkpimfApp.data.localization["Unable_upload_photo"]);
        }
    });
};

atkpimfApp.uploadImage = function (sk, successCb, progress) {
    atkpimfApp.fileUpload({
        url: atkpimfApp.data.urlTmpl.InterestItemUploadPhoto,
        formData: sk,
        done: function (ajax, successCb) {
            var response = JSON.parse(ajax.response);
            var imageData = response.CustomData;
            var onloadedCb = function () {
                var scrollTop = 0;
                var imageContainer = $('#uploaded-image-container');
                var imageWrapper = $('<div class="modal-photo-add-edit-img-pos" onmouseout="atkpimfApp.hover.hide(this)" data-remove=""></div>');
                var closeButton = $('<i class="close-button modal-photo-add-edit-close-button" data-hover=""></i>');
                var image = $('<img id="img-' + imageData.key + '" />')
                    .addClass('modal-photo-add-edit-img-multiple')
                    .attr('src', imageData.url)
                    .attr('data-key', imageData.key);

                imageWrapper.on('mouseover', function () {
                    var imgCount = imageContainer.find('img').length;

                    if (imgCount > 1) {
                        atkpimfApp.hover.show(this);
                    }
                });

                closeButton.on('click', function () {
                    atkpimfApp.photoEdit.removePhoto(this);

                    var imgCount = imageContainer.find('img').length;

                    if (imgCount < 10) {
                        $('.modal-photo-add-edit-more-photos-button-disabled')
                            .switchClass('modal-photo-add-edit-more-photos-button-disabled', 'modal-photo-add-edit-more-photos-button')
                            .removeAttr('disabled');
                    }
                });

                imageContainer.find('img').each(function () {
                    scrollTop += $(this).height();
                });

                image.appendTo(imageWrapper);
                closeButton.appendTo(imageWrapper);
                imageWrapper.appendTo(imageContainer);

                var imageCount = imageContainer.find('img').length;

                if (imageCount >= 10) {
                    $('.modal-photo-add-edit-more-photos-button')
                        .switchClass('modal-photo-add-edit-more-photos-button', 'modal-photo-add-edit-more-photos-button-disabled')
                        .attr('disabled', 'disabled');
                }

                //setTimeout(function () {
                //    imageContainer.scrollTop(scrollTop);
                //}, 200);
            };

            if (successCb) {
                successCb(onloadedCb);
            }
            else {
                onloadedCb();
            }
        },
        doneSuccess: successCb,
        progress: progress,
        fail: function () {
            alert(atkpimfApp.data.localization["Unable_upload_photo"]);
        }
    });
};


atkpimfApp.uploadImageForTextPost = function (successCb) {
    var syncKey = (new Date()).getTime();
    var imgId = atkpimfApp.viewModel.MyUser().UserId + '-' + syncKey;
    atkpimfApp.fileUpload({
        url: atkpimfApp.data.urlTmpl.InterestItemUploadPhoto,
        formData: { sk: syncKey, forTextPost: true },
        fail: function () {
            alert(atkpimfApp.data.localization["Unable_upload_photo"]);
        },
        reader: function (file, success) {
            var reader = new FileReader();
            reader.onload = function (e1) {
                success(imgId, e1.target.result);
            };
            reader.readAsDataURL(file);
        },
        readerSuccess: successCb,
        done: function (ajax) {
            var response = JSON.parse(ajax.response);
            var imageData = response.CustomData;
            $('#' + imgId).attr('src', imageData.url);
        },
    });


};

atkpimfApp.startImageUpload = function (editUrl, targetContainerId, progressFunc) {
    var syncKey = (new Date()).getTime();

    editUrl = atkpimfApp.addPramToLink(editUrl, "sk", syncKey);
    atkpimfApp.uploadImage(syncKey, function (onloadedCb) {
        if (targetContainerId) {
            atkpimfApp.updateElementById(editUrl, targetContainerId, onloadedCb);
        }
        else {
            atkpimfApp.modal.showByUrl(editUrl, null, null, null, onloadedCb);
        }
    }, progressFunc);
};

atkpimfApp.closedHintTime = function (element, hintType) {
    element.parentNode.style.display = "none";
    localStorage[hintType + "-" + "hintClosedTime"] = new Date();
};

atkpimfApp.checkClosedHintTime = function (hintType) {
    var oneDay = 24 * 60 * 60 * 1000;
    var seconds = 10 * 1000;
    var currentTime = new Date();
    var oldTime = Date.parse(localStorage[hintType + "-" + "hintClosedTime"]) || 0;
    var timeDifference = currentTime - oldTime;
    if ((timeDifference > oneDay)) {
        return true;
    }
    else if (oldTime != null) {
        return false;
    }

    return null;
};

atkpimfApp.remove = function (elementId) {
    var element = document.getElementById(elementId);
    element.parentNode.removeChild(element);
};

atkpimfApp.buyRequestQueueAdd = function (name, item) {
    localStorage.setItem(name, item);
};

atkpimfApp.buyRequestQueueGet = function (name) {
    var item = localStorage.getItem(name);

    if (item != null) {
        localStorage.removeItem(name);
    }

    return item;
};


//var __nativeST__ = window.setTimeout, __nativeSI__ = window.setInterval;

//window.setTimeout = function (vCallback, nDelay /*, argumentToPass1, argumentToPass2, etc. */) {
//    var oThis = this, aArgs = Array.prototype.slice.call(arguments, 2);
//    return __nativeST__(vCallback instanceof Function ? function () {
//        vCallback.apply(oThis, aArgs);
//    } : vCallback, nDelay);
//};

//window.setInterval = function (vCallback, nDelay /*, argumentToPass1, argumentToPass2, etc. */) {
//    var oThis = this, aArgs = Array.prototype.slice.call(arguments, 2);
//    return __nativeSI__(vCallback instanceof Function ? function () {
//        vCallback.apply(oThis, aArgs);
//    } : vCallback, nDelay);
//};

atkpimfApp.tooltip = {
    tipId: "atapp-tooltip",
    defaultOffsetFromCursorY: 15,
    offsetFromCursorY: undefined,
    offsetFromCursorX: undefined,
    opacity: 0,
    element: undefined,
    tip: undefined,
    
    init: function () {
        document.addEventListener("mouseover", this, false);
        
    },
    handleEvent: function (e) {
        switch (e.type) {
            case "mouseover":
                this.initTip(e);
                break;
            
        }
    },
    initTip: function (event) {
        var element = event.target;
        var title = "";
        var markup = "";
        var dataTitle = element.dataset && element.dataset.title ? element.dataset.title : undefined;
        var offsetFromCursorX = element.dataset && element.dataset.titleOffsetX && element.dataset.titleOffsetX != "" ? element.dataset.titleOffsetX : undefined;
        var offsetFromCursorY = element.dataset && element.dataset.titleOffsetY && element.dataset.titleOffsetY != "" ? element.dataset.titleOffsetY : undefined;

        if (this.element && this.element == element ||
            this.tip == element ||
            this.element && this.element == element.parentNode ||
            this.element && this.element == element.parentNode.parentNode) {
            return;
        }

        if (this.element && this.element != element ||
            this.element && this.element != element.parentNode ||
            this.element && this.element != element.parentNode.parentNode) {
             this.hideTip();
             return;
        }

       

        if (!element.title && dataTitle === undefined) {
            return;
        }
        
        if (offsetFromCursorX !== undefined) {
            this.offsetFromCursorX = offsetFromCursorX;
        }

        if (offsetFromCursorY !== undefined) {
            this.offsetFromCursorY = offsetFromCursorY;
        }

        if (element.title) {
            title = element.title;
            element.dataset.title = title;
            element.removeAttribute("title");
        }
        if (element.dataset.title) {
            title = element.dataset.title;
        }
        
        markup = element.dataset.markup;
        
        this.element = element;
     
        this.createTip(event, title, markup);
    },
    createTip: function (event, title, markup) {
        var brRegExp = /&lt;br&gt;/g;
        var brTag = "<br>";
        this.opacity = 0;
        var element = this.element;
        var tip = document.getElementById(this.tipId);
        if (!tip) {
            tip = document.createElement("div");
            tip.setAttribute("id", this.tipId);
            document.body.appendChild(tip);
        }
        if (markup) {
            tip.setAttribute("class", markup);
        }
        title = title.replace(brRegExp, brTag);
        tip.innerHTML = title;
        tip.style.opacity = this.opacity;
        tip.style.left = "auto";
        this.tip = tip;
       
        
        
        element = this.element;
        //element.onmousemove = this.moveTip(event);
        this.moveTip(event);
        this.showTip();
        
    },
    hideTip: function () {
        if (this.element) {
            
            this.element = undefined;
        }
        
        this.offsetFromCursorY = undefined;
        if (this.tip) {
            this.tip.style.opacity = "auto";
            this.tip.style.top = "auto";
            this.tip.style.left = "-9000px";
        }
        

    },
    _round: function(value, decimals) {
        return Number(Math.round(value +'e'+ decimals) + 'e-'+decimals);
    },

    showTip: function () {
        var tip = this.tip;
        if (this.opacity < 1) {
            this.opacity = this._round(this.opacity + 0.1, 1);
            tip.style.opacity = this.opacity;
            tip.style.filter = 'alpha(opacity=' + this.opacity * 100 + ')';
           // setTimeout.call(this, this.showTip, 5);
        }
        
        this.tip = tip;
    },
    moveTip: function (event) {
        var tip = this.tip;

        var clientX =  event.clientX;
        var clientY =  event.clientY;
        
        var curX = clientX + document.documentElement.scrollLeft;
        var curY = clientY + document.documentElement.scrollTop;
        var winWidth = window.innerWidth - 20;
        var winHeight = window.innerHeight - 20;
        this.offsetFromCursorY; 
        var rightEdge= winWidth - clientX;
        var bottomEdge = winHeight - clientY - this.offsetFromCursorY;
 
        if (rightEdge < tip.offsetWidth) {
            tip.style.left = curX - tip.offsetWidth + "px";
        }
        else {
            tip.style.left = curX + "px";
        } 
 
        if (this.offsetFromCursorY) {
            tip.style.top = this.offsetFromCursorY + "px"; 
        }   
        else if ( this.offsetFromCursorY == undefined  && bottomEdge < tip.offsetHeight) {
            tip.style.top = curY - tip.offsetHeight - this.defaultOffsetFromCursorY + "px";
        }
        else if (this.offsetFromCursorY == undefined) {
            tip.style.top = curY + this.defaultOffsetFromCursorY + "px";
        }

        this.tip = tip;
        
    }
};

atkpimfApp.tooltip.init();
