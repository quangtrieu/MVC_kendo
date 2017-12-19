$(document).ready(function () {
    if (window.addEventListener) {
        window.addEventListener("message", receive, false);
    }
    else {
        if (window.attachEvent) {
            window.attachEvent("onmessage", receive, false);
        }
    }

    // set action change default rest code
    $('input[type="radio"][name="DefaultRestCode"]').on("change", function (e) {
        updateDefaultRestCode($(this).val());
    });

    // set checked value is not exists link rest
    if ($('input[type="radio"][checked="checked"]').length == 0) {
        $('input[type="radio"]:first').attr('checked', 'checked');
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
        showProcessing();

        // call action add reference rest code
        $.ajax({
            type: "POST",
            url: "/LinkRestaurant/AddReferenceRestCode",
            data: { tokenId: tokenId },
            success: function (data) {
                if (data.Status) {
                    $("#warningPopup").find('iframe').parent().html(data.Message);
                    $("#warningPopup").find("#act-accept-no").on("click", function () {
                        window.location = window.location.pathname;
                    });
                }
                else {
                    $("#warningPopup").find('iframe').parent().html(data.Message);
                }
            },
            error: function (e) {
                console.log(e);
            }
        });
    }
}

function addReferenceRestaurant() {
    warningPopup("Login Smart System Pro", '<iframe width="505" height="265" scrolling="no" src="' + $('#bcsWebServiceUrl').val() + '"></iframe>');
}

function updateDefaultRestCode(val) {
    // call method update default rest code.
    showProcessing();
    $.ajax({
        type: "POST",
        url: "/LinkRestaurant/UpdateDefaultRestCode",
        data: { restCode: val },
        success: function (data) {
            if (data.Status) {
                warningPopup("Information", data.Message);
            }
            else {
                warningPopup("Warning", data.Message);
            }
        },
        error: function (e) {
            console.log(e);
        }
    });
}
