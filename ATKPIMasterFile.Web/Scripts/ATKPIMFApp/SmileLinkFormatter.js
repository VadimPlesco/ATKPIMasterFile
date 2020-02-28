atkpimfApp.SmileLinkFormatter = function() {


    var tagsToReplace = {
        '&': '&amp;',
        '<': '&lt;',
        '>': '&gt;'
    };

    function escape(text) {
        return text.replace(/[&<>]/g, function(tag) {
            return tagsToReplace[tag] || tag;
        });
    }

    function urldecode(str) {
        try {
            // percent sign regex except those in encoded url (%.{0,2}(?=\s))|(%$)|(%.{2}(?!(%|\/|-)))
            return decodeURIComponent((str + '').replace('%', '[percent-sign]')).replace('[percent-sign]', '%');
        }
        catch(e) {
            return str;
        }
    }

    function replaceEmoticons(text) {
        var emoticons = { ';-)': 'wink.png', ';)': 'wink.png', ':-)': 'smile.png', ':)': 'smile.png', ':-D': 'laughing.png', ':D': 'laughing.png', ':-[': 'confused.png', ':[': 'confused.png', 'B-)': 'cool.png', 'B)': 'cool.png', ':-o': 'gasp.png', '|-o': 'footinmouth.png', '*smile*': 'smile.png', '*minismile*': 'minismile.png', '*wink*': 'wink.png', '*content*': 'content.png', '*cool*': 'cool.png', '*laughing*': 'laughing.png', '*grin*': 'grin.png', '*sarcastic*': 'sarcastic.png', '*crazy*': 'crazy.png', '*hearteyes*': 'hearteyes.png', '*kiss*': 'kiss.png', '*innocent*': 'innocent.png', '*yuck*': 'yuck.png', '*yum*': 'yum.png', '*confused*': 'confused.png', '*slant*': 'slant.png', '*ambivalent*': 'ambivalent.png', '*frown*': 'frown.png', '*minifrown*': 'minifrown.png', '*embarrassed*': 'embarrassed.png', '*footinmouth*': 'footinmouth.png', '*angry*': 'angry.png', '*naughty*': 'naughty.png', '*cry*': 'cry.png', '*gasp*': 'gasp.png', '*moneymouth*': 'moneymouth.png', '*nerd*': 'nerd.png', '*notamused*': 'notamused.png', '*sealed*': 'sealed.png', '*sick*': 'sick.png', '*heart*': 'heart.png', '*thumbsup*': 'thumbsup.png', '*thumbsdown*': 'thumbsdown.png' }, url = atkpimfApp.data.contentCdnPath + "Content/themes/dzen/images/smiles/", patterns = [],
            metachars = /[[\]{}()*+?.\\|^$\-,&#\s]/g;

        // build a regex pattern for each defined property
        for (var i in emoticons) {
            if (emoticons.hasOwnProperty(i)) { // escape metacharacters
                patterns.push('(' + i.replace(metachars, "\\$&") + ')');
            }
        }

        // build the regular expression and replace
        return text.replace(new RegExp(patterns.join('|'), 'g'), function(match) {
            return typeof emoticons[match] != 'undefined' ?
                '<div class="shared__chat-smile-container"><img class="shared__chat-smile" src="' + url + emoticons[match] + '"/></div>' :
                match;
        });
    }

    this.formatMessage = function(text) {
        if (text == null) return '';

        text = escape(text);

        /* url, email */
        var urlPattern = /\b(?:https?|ftp):\/\/[a-zа-я0-9-+&@#\/%?=~_|!:,.;]*[a-zа-я0-9-+&@#\/%=~_|]/gim;
        var pseudoUrlPattern = /(^|[^\/])(www\.[\S]+(\b|$))/gim;
        var emailAddressPattern = /\w+@[a-zA-Zа-яА-Я_]+?(?:\.[a-zA-Zа-яА-Я]{2,6})+/gim;
        text = urldecode(
            text.replace(urlPattern, '<a rel="nofollow" target="_blank" class="white-white" href="$&">$&</a>')
                .replace(pseudoUrlPattern, '$1<a rel="nofollow" target="_blank" class="white-white" href="http://$2">$2</a>')
                .replace(emailAddressPattern, '<a rel="nofollow" target="_blank" class="white-white" href="mailto:$&">$&</a>')
        );

        text = replaceEmoticons(text);

        return replaceEmoticons(text);
    };
};

atkpimfApp.smileLinkFormatter = new atkpimfApp.SmileLinkFormatter();
