//kinkyApp.fn.oldBrowserDetect = function () {

//    var animation = false;
//    var updateBrowserHtml = '<div id="noscript-update-browser-container" class="update-browser-container">' +
//        '<div class="update-browser-inner-top">' +
//          '<div class="update-browser-text update-browser-text-first">' + kinkyApp.data.localization.Browser_not_pull_site + '</div>' +
//          '<div class="update-browser-text">' + kinkyApp.data.localization.Download_modern_browser + ':' + '</div>' +
//    '</div>' +
//    '<div class="update-browser-inner-bottom">' +
//    '<div class="update-browser-logos-container">' +
//    '<a target="_blank" rel="nofollow" href="http://www.google.ru/intl/ru/chrome/" class="update-browser-link" title="' + kinkyApp.data.localization.Go_download + ' Chrome">' +
//    '<i class="update-browser-logos update-browser-logo-chrome"></i>' +
//    '</a>' +
//    '<a target="_blank" rel="nofollow" href="http://www.mozilla.org/ru/firefox/new/" class="update-browser-link" title="' + kinkyApp.data.localization.Go_download + ' Mozilla Firefox">' +
//    ' <i class="update-browser-logos update-browser-logo-firefox"></i>' +
//    ' </a>' +

//    '<a target="_blank" rel="nofollow" href="http://www.opera.com/ru/computer/next" class="update-browser-link" title="' + kinkyApp.data.localization.Go_download + ' Opera">' +
//    '  <i class="update-browser-logos update-browser-logo-opera"></i>' +
//    ' </a>' +

//    ' </div></div></div>';








//    var elm = document.body || document.documentElement,
//    animationstring = 'animation',
//    keyframeprefix = '',
//    domPrefixes = 'Webkit Moz O ms Khtml'.split(' '),
//    pfx = '';


//    if (elm.style.animationName) { animation = true; }

//    if (animation === false) {
//        for (var i = 0; i < domPrefixes.length; i++) {
//            if (elm.style[domPrefixes[i] + 'AnimationName'] !== undefined) {
//                pfx = domPrefixes[i];
//                animationstring = pfx + 'Animation';
//                keyframeprefix = '-' + pfx.toLowerCase() + '-';
//                animation = true;
//                break;
//            }
//        }
//    }


//    if (animation == false) {
//        document.getElementById("update-browser-container").innerHTML = updateBrowserHtml;
//    }

//};


//kinkyApp.fn.downloadBrowserLink = function (browser) {
//    var os;
//    var lang = navigator.language;
//    if (navigator.appVersion.indexOf("Win") != -1) os = "win";
//    if (navigator.appVersion.indexOf("Mac") != -1) os = "osx";
//    if (navigator.appVersion.indexOf("Linux") != -1) os = "linux";


//    if (browser == firefox) {
//        window.location = "https://download.mozilla.org/?product=firefox-stub&os=" + os + "&lang=" + lang;
//    }
//};