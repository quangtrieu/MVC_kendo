var AJAX = {
    effect: false,
    effect_inline: null,
    ajaxResult: null,
    ajaxData: {},
    ajaxUrl: null,
    ajaxCallback: null,
    ajaxAsync: false,
    ajaxCache: false,
    ajaxType: 'POST',
    ajaxParseJSON: true,
    inline_text: null,

    request: function ()
    {
        jQuery.ajax({
            type: AJAX.ajaxType,
            data: AJAX.ajaxData,
            url: AJAX.ajaxUrl,
            async: AJAX.ajaxAsync,
            cache: AJAX.ajaxCache,
            beforeSend: function ()
            {
                if (AJAX.effect)
                {
                    var h = document.documentElement.scrollHeight;
                    jQuery('#model-loading').css({ "height": h + "px", "background-position": "center center !important" });
                    jQuery('#model-loading').fadeIn();
                    jQuery('#model-loading').animate({ opacity: 0.5 }, 100, null);
                } else if (AJAX.effect_inline !== null)
                {
                    AJAX.inline_text = jQuery(AJAX.effect_inline).html();
                    jQuery(AJAX.effect_inline).html('');
                    jQuery('.inline_loading').removeClass('inline_loading');
                    jQuery(AJAX.effect_inline).addClass('inline_loading');
                }
            },
            error: function (request, error)
            {
                AJAX.ajaxType = 'POST';
                if (AJAX.effect_inline !== null)
                {
                    jQuery(AJAX.effect_inline).removeClass('inline_loading');
                }
                jQuery('#model-loading').fadeOut();
            },
            success: function (result)
            {
                try
                {
                    AJAX.ajaxType = 'POST';
                    if (AJAX.effect_inline !== null)
                    {
                        jQuery(AJAX.effect_inline).removeClass('inline_loading');
                        jQuery(AJAX.effect_inline).html(AJAX.inline_text);
                        AJAX.effect_inline = null;
                        AJAX.inline_text = null;
                    }
                    jQuery('#model-loading').fadeOut('500');
                    if (AJAX.ajaxParseJSON)
                    {
                        AJAX.ajaxResult = jQuery.parseJSON(result);
                        AJAX.ajaxData = {};
                    } else
                    {
                        AJAX.ajaxResult = result;
                        AJAX.ajaxData = {};
                        AJAX.ajaxParseJSON = true;
                    }
                    AJAX.ajaxCallback();
                } catch (e) { }
            }
        });
    }
}