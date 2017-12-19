$(document).ready(function() {
    // Private function
    // 1. set function in parent form call iframes
    $('#btnSignUp').on('click', function () {
        warningPopup("Login Smart System Pro", '<iframe width="505" height="265" scrolling="no" src="' + $('#bcsWebServiceUrl').val() + '"></iframe>');
    });

    $('#btnContactUs').on('click', function () {
        warningPopup('Warning', 'Coming soon..');
    });

    if (window.addEventListener) {
        window.addEventListener("message", receive, false);
    }
    else {
        if (window.attachEvent) {
            window.attachEvent("onmessage", receive, false);
        }
    }
});

function receive(event) {
    var data = event.data;
    if (typeof (window[data.func]) == "function") {
        window[data.func].call(null, data.params[0]);
    }
}

function receiveSendTokenId(tokenId) {
    if (tokenId != null && tokenId != '') {
        // check user exists bcs
        $.ajax({
            type: "POST",
            url: "/register/CheckTokenIdExist",
            data: { tokenId: tokenId },
            success: function (data) {
                if (data.Message) {
                    $("#warningPopup").find('iframe').parent().html(data.Message)
                }
                else {
                    showProcessing();
                    var systemId = 2; // user from ssp system value is 2.
                    window.location = '/Register?id=' + tokenId + '&systemId=' + systemId;
                }
            },
            error: function (e) {
                console.log(e);
            }
        });
    }
    
}
