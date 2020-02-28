(function ($, atkpimfApp) {

    //atkpimfApp.updateElementById = function (url, id, ajaxcallback) {
    //    //alert('1');
    //    atkpimfApp.asyncRequest({
    //        url: url,
    //        id: id,
    //        success: function (data) { //alert(data)
    //            //if (id == "body") {
    //            //    $(window).scrollTop(0);
    //            //    atkpimfApp.modal.hide();
    //            //}
    //            //alert('success');
    //            console.log(data.Html);
    //            $("#" + id).html(data);
    //            //$("#" + id).html('<p>Hello!</p>');
    //            if (ajaxcallback != null)
    //                ajaxcallback(data);
    //        }
    //    });
    //    return false;
    //};

    atkpimfApp.updateElementById = function (url, id, ajaxcallback) {
        
        var loading = $("#loadingElement");

        $.ajax({
            url: url,
            contentType: 'application/html',
            data: id,
            success: function (content) {
               
                $("#" + id).html(content);

                var parsed = $.parseHTML(content);
                result = $(parsed).find("#avatarImg").attr('src');
               
                $('#menuAvatar').css('background-image', 'url(' + result + ')');
                
                if (ajaxcallback != null)
                    ajaxcallback(content);
            },

            beforeSend: function () {
               
                loading.show(0);
            },
            complete: function () {
                loading.hide(0);
               
            },

            error: function (e) { console.log(e); }
        });

        return false;
    };

    atkpimfApp.updateBodyByUrl = function (url, ajaxcallback) {
        atkpimfApp.asyncRequest({
            url: url,
            id: "body",
            success: function (data) {
                atkpimfApp.updateBody(data.Html);
                if (ajaxcallback != null)
                    ajaxcallback(data);
            }
        });
        return false;
    };

    atkpimfApp.updateBody = function (html) {
        $(window).scrollTop(0);
        atkpimfApp.modal.hide();
        $("#body").html(html);
        return false;
    };


    atkpimfApp.addPramToLink = function (link, key, val) {
        return link + (link.indexOf('?') > -1 ? '&' : '?') + key + '=' + val;
    };


    var reload = function (id, url) {
        atkpimfApp.asyncRequest({
            url: url,
            isReload: true,
            id: id,
            success: function (data) {
                $("#" + id).html(data.Html);
            }
        });
    };

    atkpimfApp.banUser = function (url, userId, success) {
        $.ajax({
            url: url,
            type: 'POST',
            data: { userId: userId },
            success: success,
            error: function (parameters) {

            }
        });
    };

    atkpimfApp.asyncRequest = function (options) {

        var loading = $("#loadingElement");
        //alert("123");
        //alert(options.url);
        options.url = atkpimfApp.addPramToLink(options.url, "X-Requested-With", "XMLHttpRequest");

        var success = options.success;
        var complete = options.complete;
        var beforeSend = options.beforeSend;
        $.extend(options, {
            beforeSend: function () {
                if (beforeSend)
                    beforeSend();
                loading.show(0);
            },
            complete: function () {
                loading.hide(0);
                if (complete)
                    complete();
            },
            error: function (xhr, status, error) {
                //alert(error);
                try {
                    var d = eval('[' + xhr.responseText + ']')[0];
                    if (success)
                        success(d);
                }
                catch (e) {
                    alert(e);
                }
            },
            success: function (data) {
                //alert(data.Status);
                ///console.log(data);
                if (data.RedirectUrl) {
                    if (data.Status == atkpimfApp.statusCodes.redirectToBody)
                        atkpimfApp.updateBodyByUrl(data.RedirectUrl);
                    else
                        document.location.href = data.RedirectUrl;
                    return;
                }

                if (data.Status == atkpimfApp.statusCodes.ok || data.Status == null) {
                    if (success)
                        success(data);

                    if (options.id) {
                        if (!options.isReload)
                            setHistory(data, options.id, options.setUrlInAddressBar, options.replaceUrlInAddressBar);

                        if (atkpimfApp.fn.initCurrentLink)
                            atkpimfApp.fn.initCurrentLink();

                        $("#" + options.id + " form").each(function () {
                            $.validator.unobtrusive.parse(this);
                        });

                        if (!options.isReload)
                            atkpimfApp.analytics.trackPageView(data.Url, data.Title, true)/*Track(data.Url, data.Title)*/;
                    }
                }
                else if (data.Status == atkpimfApp.statusCodes.authenticationHasExpired) {
                    atkpimfApp.modal.show(data.Html, true);
                }
                else if (data.Status == atkpimfApp.statusCodes.blocker) {
                    $(window).scrollTop(0);
                    atkpimfApp.modal.showTop(data.Html);
                }
                else if (data.Status == atkpimfApp.statusCodes.blockerBottom) {
                    $(window).scrollTop(0);
                    atkpimfApp.modal.show(data.Html);
                }
                else if (data.Status == atkpimfApp.statusCodes.hideModal) {
                    atkpimfApp.modal.hide();
                    atkpimfApp.modal.hideTop();
                    atkpimfApp.reloadBody();
                }
                else if (data.Status == atkpimfApp.statusCodes.error) {
                    success(data);
                }

            },
        });
        options.headers = { Accept: "application/json; charset=utf-8" };
        if (options.id == "main_modal")
            options.headers.isModal = "true";

        options.data = options.data || {};


        if (options.url.indexOf(':') > -1) {
            options.xhrFields = { withCredentials: true };
            options.crossDomain = true;
        }

        $.ajax(options).done();
    };


    //////////
    //atkpimfApp.asyncRequest = function (element, options) {
    //    var confirm, loading, method, duration;

    //    confirm = element.getAttribute("data-ajax-confirm");
    //    if (confirm && !window.confirm(confirm)) {
    //        return;
    //    }

    //    loading = $(element.getAttribute("data-ajax-loading"));
    //    duration = parseInt(element.getAttribute("data-ajax-loading-duration"), 10) || 0;

    //    $.extend(options, {
    //        type: element.getAttribute("data-ajax-method") || undefined,
    //        url: element.getAttribute("data-ajax-url") || undefined,
    //        cache: !!element.getAttribute("data-ajax-cache"),
    //        beforeSend: function (xhr) {
    //            var result;
    //            asyncOnBeforeSend(xhr, method);
    //            result = getFunction(element.getAttribute("data-ajax-begin"), ["xhr"]).apply(element, arguments);
    //            if (result !== false) {
    //                loading.show(duration);
    //            }
    //            return result;
    //        },
    //        complete: function () {
    //            loading.hide(duration);
    //            getFunction(element.getAttribute("data-ajax-complete"), ["xhr", "status"]).apply(element, arguments);
    //        },
    //        success: function (data, status, xhr) {
    //            asyncOnSuccess(element, data, xhr.getResponseHeader("Content-Type") || "text/html");
    //            getFunction(element.getAttribute("data-ajax-success"), ["data", "status", "xhr"]).apply(element, arguments);
    //        },
    //        error: function () {
    //            getFunction(element.getAttribute("data-ajax-failure"), ["xhr", "status", "error"]).apply(element, arguments);
    //        }
    //    });

    //    options.data.push({ name: "X-Requested-With", value: "XMLHttpRequest" });

    //    method = options.type.toUpperCase();
    //    if (!isMethodProxySafe(method)) {
    //        options.type = "POST";
    //        options.data.push({ name: "X-HTTP-Method-Override", value: method });
    //    }

    //    $.ajax(options);
    //}
    ///////////


    var bodyState = { pageTitle: document.title, url: document.location.href, id: "body", html: null };

    atkpimfApp.reloadBody = function () {
        if (bodyState.url != null) {
            reload('body', bodyState.url);
        }
    };

    $(document).ready(function () {
        if (document.getElementById("body")) {
            bodyState = { pageTitle: document.title, url: document.location.href, id: "body", html: document.getElementById("body").innerHTML };
            window.history.replaceState(bodyState, bodyState.pageTitle, bodyState.url);
        }
    });

    atkpimfApp.replaceState = function () {
        window.history.pushState(bodyState, bodyState.pageTitle, bodyState.url);
        document.title = bodyState.pageTitle;
    };

    function setHistory(data, id, setUrlInAddressBar, replaceUrlInAddressBar) {
        try {
            if (window.history.state && window.history.state.id == "body")
                bodyState = window.history.state;

            if (setUrlInAddressBar || id == "body") {
                window.history.pushState({ html: data.Html, pageTitle: data.Title, url: data.Url, id: id }, data.Title, data.Url);
                if (data.Title)
                    document.title = data.Title;
                else
                    document.title = data.Url;
            }

            if (replaceUrlInAddressBar) {
                window.history.replaceState({ pageTitle: data.Title, url: data.Url, id: "body" }, data.Title, data.Url);
            }

        } catch (error) {
            console.log("Error: " + error);
        }
    }

    window.onpopstate = function (e) {
        //console.log(e.state);
        if (e.state) {
            if (e.state.id == "body") {
                atkpimfApp.modal.hide();
                //$("#" + e.state.id).html(e.state.html)
                reload(e.state.id, e.state.url);
                document.title = e.state.pageTitle;
            }
            else if (e.state.id == "main_modal") {
                atkpimfApp.modal.show(e.state.html);
                document.title = e.state.url;
            }
        }
    };



    var data_click = "unobtrusiveAjaxClick", data_validation = "unobtrusiveValidation";

    function getFunction(code, argNames) {
        var fn = window, parts = (code || "").split(".");
        while (fn && parts.length) {
            fn = fn[parts.shift()];
        }
        if (typeof (fn) === "function") {
            return fn;
        }
        argNames.push(code);
        return Function.constructor.apply(null, argNames);
    }

    function isMethodProxySafe(method) {
        return method === "GET" || method === "POST";
    }

    function asyncOnBeforeSend(xhr, method) {
        if (!isMethodProxySafe(method)) {
            xhr.setRequestHeader("X-HTTP-Method-Override", method);
        }
    }

    function asyncOnSuccess(element, data, contentType) {
        var mode = (element.getAttribute("data-ajax-mode") || "").toUpperCase();
        //alert('asyncOnSuccess');
        //alert(mode); alert(element);
        //console.log(element);
        $(element.getAttribute("data-ajax-update")).each(function (i, update) {
            //alert(update.name);
            //console.log(update);
            var top;
            switch (mode) {
                case "BEFORE":
                    top = update.firstChild;
                    $("<div />").html(data.Html).contents().each(function () {
                        update.insertBefore(this, top);
                    });
                    break;
                case "AFTER":
                    $("<div />").html(data.Html).contents().each(function () {
                        update.appendChild(this);
                    });
                    break;
                default:
                    $(update).html(data);
                    //alert('default');
                    //alert(data);
                    break;
            }
        });
    }


    function asyncRequest(element, options) {
        var confirm, loading, method, duration;
        //alert('asyncRequest');
        confirm = element.getAttribute("data-ajax-confirm");
        if (confirm && !window.confirm(confirm)) {
            return;
        }
        var successHandler = options.success || function () { };
        var errorHandler = options.error || function () { };

        loading = $(element.getAttribute("data-ajax-loading"));
        duration = element.getAttribute("data-ajax-loading-duration") || 0;

        $.extend(options, {
            type: element.getAttribute("data-ajax-method") || undefined,
            url: element.getAttribute("data-ajax-url") || undefined,
            beforeSend: function (xhr) {
                var result;
                asyncOnBeforeSend(xhr, method);
                result = getFunction(element.getAttribute("data-ajax-begin"), ["xhr"]).apply(this, arguments);
                if (result !== false) {
                    loading.show(duration);
                }
                return result;
            },
            complete: function () {
                loading.hide(duration);
                getFunction(element.getAttribute("data-ajax-complete"), ["xhr", "status", "data"]).apply(this, arguments);
            },
            success: function (data, status, xhr) {
                //                debugger;
                //alert('success');
                //alert(data);
                //alert(status);
                
                successHandler(data, status, xhr);
                if (data.RedirectUrl) {
                    //alert(data.RedirectUrl);
                    if (data.Status == atkpimfApp.statusCodes.redirectToBody)
                        atkpimfApp.updateBodyByUrl(data.RedirectUrl);
                    else
                        document.location.href = data.RedirectUrl;
                    return;
                }
                //else if (data.Status == atkpimfApp.statusCodes.blocker) {
                //    $(window).scrollTop(0);
                //    atkpimfApp.modal.showTop(data.Html);
                //    alert('11'); alert(data.Status); alert(atkpimfApp.statusCodes.blocker);
                //    return;
                //} else if (data.Status == atkpimfApp.statusCodes.blockerBottom) {
                //    $(window).scrollTop(0);
                //    atkpimfApp.modal.show(data.Html);
                //    alert('12');
                //    return;
                //} else if (data.Status == atkpimfApp.statusCodes.hideModal) {
                //    atkpimfApp.modal.hide();
                //    atkpimfApp.modal.hideTop();
                //    atkpimfApp.reloadBody();
                //    alert('13');
                //    return;
                //}
                //alert('111');
                asyncOnSuccess(element, data, xhr.getResponseHeader("Content-Type") || "text/html");
                if (element.getAttribute("data-ajax-update") == "#body")
                    setHistory(data, "body");

                //$(element.getAttribute("data-ajax-update")).find("script").each(function (i) {
                //    eval($(this).text());
                //});
                $(element.getAttribute("data-ajax-update") + " form").each(function () {
                    $.validator.unobtrusive.parse(this);
                });

                getFunction(element.getAttribute("data-ajax-success"), ["data", "status", "xhr"]).apply(this, arguments);
            },
            error: function (a, b, c) {
                errorHandler(a, b, c);
                getFunction(element.getAttribute("data-ajax-failure"), ["xhr", "status", "error"]);
            }
        });

        options.headers = { Accept: "application/json; charset=utf-8", isModal: (element.getAttribute("data-ajax-update") == "#main_modal" ? "true" : "false") };

        method = options.type.toUpperCase();
        if (!isMethodProxySafe(method)) {
            options.type = "POST";
            options.data.push({ name: "X-HTTP-Method-Override", value: method });
        }

        $.ajax(options).done();
    }


    function validate(form) {
        var validationInfo = $(form).data(data_validation);
        return !validationInfo || !validationInfo.validate || validationInfo.validate();
    }

    $(document).on("click", "a[data-ajax=true]", function (evt) {
        evt.preventDefault();
        asyncRequest(this, {
            url: this.href,
            type: "GET",
            data: []
        });
    });

    $(document).on("click", "form[data-ajax=true] input[type=image]", function (evt) {
        var name = evt.target.name,
            $target = $(evt.target),
            form = $target.parents("form")[0],
            offset = $target.offset();

        $(form).data(data_click, [
            { name: name + ".x", value: Math.round(evt.pageX - offset.left) },
            { name: name + ".y", value: Math.round(evt.pageY - offset.top) }
        ]);

        setTimeout(function () {
            $(form).removeData(data_click);
        }, 0);
    });

    $(document).on("click", "form[data-ajax=true] :submit", function (evt) {
        var name = evt.target.name,
            form = $(evt.target).parents("form")[0];

        $(form).data(data_click, name ? [{ name: name, value: evt.target.value }] : []);

        setTimeout(function () {
            $(form).removeData(data_click);
        }, 0);
    });

    $(document).on("submit", "form[data-ajax=true]", function (evt) {
        var spinner = $("<span>").addClass("icon button-loading-icon");
        var submitButton = $(this).find(":submit").not(".input-control__delete");
        var btnContent = submitButton.html();
        var clickInfo = $(this).data(data_click) || [];
        evt.preventDefault();
        if (!validate(this)) {
            return;
        }
        //alert("submit");
        asyncRequest(this, {
            url: this.action,
            type: this.method || "GET",
            data: clickInfo.concat($(this).serializeArray()),
            //            success: function (a, b, c) {
            //                submitButton.html(btnContent);
            //                submitButton.removeAttr('disabled');
            ////                submitButton[0].disabled = false;
            //            },
            error: function (a, b, c) {
                submitButton.html(btnContent);
                submitButton.removeProp('disabled');
            }
        });
        //submitButton.empty().append(spinner).prop("disabled", true); My 12.02.18
    });

    $(document).on("click", "form[data-ajax=true] div.submit", function (evt) {
        var form = $(evt.target).parents("form")[0];
        if (!validate(form)) {
            return;
        }

        asyncRequest(form, {
            url: form.action,
            type: form.method || "GET",
            data: $(form).serializeArray()
        });
    });
}(jQuery, atkpimfApp));
