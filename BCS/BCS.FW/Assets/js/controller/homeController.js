function linkRestaurant() {
    window.location = "/LinkRestaurant/ConfigRestaurant"
}

function linkedToYourRestaurant() {
    window.location = "/Budget/ViewLinkToYourRestaurant?id=" + $.urlParam("id");
}

function redirectToCategorySetting(id) {
    var budgetId = id == null ? $.urlParam("id") : id;
    window.location = "/Budget/CategorySetting?id=" + budgetId;
}

function redirectToBudgetLength() {
    window.location = "/Budget/BudgetLength?id=" + $.urlParam("id");
}

function redirectToInputMethod() {
    window.location = "/Budget/InputMethod?id=" + $.urlParam("id");
}

function redirectToGoodJobCategorySetting() {
    window.location = "/Budget/ViewGoodJobCategorySetting?id=" + $.urlParam("id");
}

function redirectToBudgetDetails() {
    window.location = "/Budget/ViewTagetAndSales?id=" + $.urlParam("id");
}

function redirectToReview() {
    window.location = "/Budget/ReviewBudgetDetail?id=" + $.urlParam("id");
}

function redirectToEditBudget() {
    window.location = "/Budget/ViewEditBudget?id=" + $.urlParam("id");
}

function redirectToTargetAndSales() {
    $.showOrHideSection('Sales');
    $('#SectionViewName').val('Sales')
}

function redirectToCogs() {
    $.showOrHideSection('COGS');
    $('#SectionViewName').val('COGS')
}

function redirectToPayrollExpenses() {
    $.showOrHideSection('Payroll_Expenses');
    $('#SectionViewName').val('Payroll_Expenses')
}

function redirectToOperationExpenses() {
    $.showOrHideSection('Operation_Expenses');
    $('#SectionViewName').val('Operation_Expenses')
}

function newBudget() {
    var options = {
        id: "warningPopup",
        typeStatus: true,
        title: "New Budget",
        confirmText: "<div class='newBudgetInput'><input class='bcs-textbox' placeholder='New Budget' name='BudgetName' maxlength='255'/><br/><span class='error'></span></div>",
        textYes: "SAVE",
        textNo: "CANCEL"
    }
    var popupWindow = window.setaJs.initPopupWindow(options);
    popupWindow.refresh({}).center().open();

    var $form = $('#' + options.id);
    $form.find('input[name="BudgetName"]').focus();

    $form.find("#act-accept-no").on("click", function () {
        if (popupWindow) popupWindow.close();
    });
    $form.find("#act-accept-yes").on("click", function () {
        var budgetName = $.trim($("input[name='BudgetName']").val());
        if (budgetName.length == 0) {
            $(".newBudgetInput .error").empty().append("The Budget Name field is required.");
            return;
        }

        $.ajax({
            url: '/Budget/NewBudget',
            type: "GET",
            async: false,
            data: { budgetName: budgetName },
            success: function (result) {
                if (result.Status) {
                    if (popupWindow) popupWindow.close();
                    // redirect to screen new budget
                    window.location = "/Budget/CategorySetting?id=" + result.BudgetId;
                } else {
                    warningPopup("Warning", "Process add new budget failure.");
                }
            },
            error: function () {
                warningPopup("Warning", "Process error, Please contact system admin!");
            }
        });
    });
}

function ChangeBudgetName(budgetId, budgetName) {
    var options = {
        id: "warningPopup",
        typeStatus: true,
        title: "Edit Budget Name",
        confirmText: '<div class="newBudgetInput"><input class="bcs-textbox" placeholder="Edit Budget" name="BudgetName" maxlength="255" value="' + budgetName + '"/><br/><span class="error"></span></div>',
        textYes: "SAVE",
        textNo: "CANCEL"
    }
    var popupWindow = window.setaJs.initPopupWindow(options);
    popupWindow.refresh({}).center().open();
    popupWindow.center().open();

    var $form = $('#' + options.id);
    $form.find("#act-accept-no").on("click", function () {
        if (popupWindow) popupWindow.close();
    });
    $form.find("#act-accept-yes").on("click", function () {
        var budgetName = $.trim($form.find("input[name='BudgetName']").val());
        if (budgetName.length == 0) {
            $(".newBudgetInput .error").empty().append("The Budget Name field is required.");
            return;
        }

        $.ajax({
            url: '/Budget/ChangeBudgetName',
            type: "GET",
            async: false,
            data: { id: budgetId, budgetName: budgetName },
            success: function (result) {
                if (result.Status) {
                    if (popupWindow) popupWindow.close();
                    warningPopup("Information", result.Message, window.location.pathname);
                } else {
                    warningPopup("Warning", result.Message);
                }
            },
            error: function () {
                warningPopup("Warning", "Process error, Please contact system admin!");
            }
        });
    });
}

// edit user profile
function editUserProfile() {
    var options = {
        id: 'popupUserProfile',
        title: 'My Profile',
        width: 680
    };
    var popupWindow = window.setaJs.initPopupWindow(options);
    popupWindow.refresh({
        url: '/User/MyProfile',
        async: false
    });
    popupWindow.center().open();

    //init form
    var $form = $('#frmSaveMyProfile');

    window.setaJs.focusFirstInputForm($form);

    //handler button
    $form.find(".btn-accept-no").on("click", function () {
        if (popupWindow) popupWindow.close();
    });

    //handler button
    $form.find(".k-grid-update").on("click", function () {
        // clear all error message before submit
        $('.error').empty();

        if ($("#frmSaveMyProfile").valid()) {
            // check valid
            if ($form.find("#txtFullName").val().length == 0) {
                $form.find(".fullNameInput .error").empty().append("</br> The Full Name field is required.");
                return;
            }
            if ($form.find("#txtFullName").val().length < 3 || $form.find("#txtFullName").val().length > 255) {
                $form.find(".fullNameInput .error").empty().append("</br> Full Name length must be between 3 to 255 characters.");
                return;
            }

            if ($form.find("#txtEmail").val().length == 0) {
                $form.find(".EmailInput .error").empty().append("</br> The Email field is required.");
                return;
            }
            if (!validateEmail($form.find("#txtEmail").val())) {
                $form.find(".EmailInput .error").empty().append("</br> The Email field is not a valid e-mail address.");
                return;
            }
            // check phone number when input
            if ($form.find("#txtPhone").val().length > 0) {
                if ($form.find("#txtPhone").val().length < 6 || $form.find("#txtPhone").val().length > 30) {
                    $form.find(".PhoneInput .error").empty().append("</br> Phone length must be between 6 to 30 characters.");
                    return;
                }
            }

            $.ajax({
                type: "POST",
                datatype: "json",
                url: '/User/SaveMyProfile',
                data: $form.serialize(),
                success: function (response) {
                    if (response.StatusCheckDuplicateEmail) {
                        // set message show on popup
                        $(".EmailInput .error").empty().append("</br> This e-mail address already exists in this Budget Creator.");
                        return;
                    }

                    if (response.Status) {
                        warningPopup("Information", response.Message, window.location.pathname + "" + window.location.search);
                    }
                    else
                        warningPopup("Warning", response.Message);
                }
            });
        }
    });
}

// valid mail
function validateEmail(email) {
    var emailReg = new RegExp(/^(("[\w-\s]+")|([\w-]+(?:\.[\w-]+)*)|("[\w-\s]+")([\w-]+(?:\.[\w-]+)*))(@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$)|(@\[?((25[0-5]\.|2[0-4][0-9]\.|1[0-9]{2}\.|[0-9]{1,2}\.))((25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\.){2}(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\]?$)/i);
    var valid = emailReg.test(email);

    if (valid) {
        return true;
    } else {
        return false;
    }
}

// valid phone number
function checkPhoneNumber(phone) {
    var phoneRegEx = /^[0-9]{0,30}$/;
    var valid = phoneRegEx.test(phone);
    if (valid) {
        return true;
    } else {
        return false;
    }
}

function showHiddenHelp(flag, helpSettingDataId) {
    showProcessing();
    $.ajax({
        type: "POST",
        url: "/Home/ShowOrHiddenHelp",
        data: { flag: flag, helpSettingDataId: helpSettingDataId },
        success: function (data) {
            if (data.Status) {
                window.location = window.location.pathname + "?id=" + $.urlParam('id');
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

function callNextOrBack(isNext) {
    switch (window.location.pathname)
    {
        case "/Budget/CategorySetting":
            if (isNext) {
                redirectToBudgetLength();
            } else {
                window.location = "/Dashboard/Index";
            }
            break;
        case "/Budget/BudgetLength":
            if (isNext) {
                linkedToYourRestaurant();
            } else {
                redirectToCategorySetting();
            }
            break;
        case "/Budget/ViewLinkToYourRestaurant":
            if (isNext) {
                redirectToBudgetDetails();
            } else {
                redirectToBudgetLength();
            }
            break;
        case "/Budget/ViewTagetAndSales":
            //var sectionName = $("#SectionViewName").val();
            if (isNext) {
                // call save data from screen to session
                confirmBeforeRedirectPage("/Budget/ReviewBudgetDetail?id=" + $.urlParam("id"));
            } else {
                confirmBeforeRedirectPage("/Budget/ViewLinkToYourRestaurant?id=" + $.urlParam("id"));
            }
            break;
        case "/Budget/ReviewBudgetDetail":
            if (isNext) {
                saveChangeBudget();
            } else {
                //redirectToBudgetDetails();
                confirmBeforeRedirectPage("/Budget/ViewTagetAndSales?id=" + $.urlParam("id"));
            }
            break;
        default:
            break;
    }
}

function showCommingSoon() {
    //Coming soon..
    warningPopup('Warning', 'Coming soon..');
}

function warningPopup(title, message, redirectActionClose) {
    var options = {
        id: "warningPopup",
        typeStatus: true,
        title: title,
        confirmText: message,
        textYes: "",
        textNo: "Close"
    }
    var popupWindow = window.setaJs.initPopupWindow(options);
    popupWindow.refresh({}).center().open();
    popupWindow.center().open();

    var $form = $('#' + options.id);
    $form.find("#act-accept-no").on("click", function () {
        if (popupWindow) popupWindow.close();
        if (redirectActionClose != null && redirectActionClose.length > 0)
            window.location = redirectActionClose;
    });
}

$.formatNumber = function (total) {
    return total == undefined ? 0 : parseFloat(total);
}

$.formatCurrency = function (total) {
    var strResult = '';
    if (total < 0) {
        strResult = ('($') + parseFloat(Math.round((0 - total) * 100) / 100, 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString() + ')';
    } else {
        strResult = ('$') + parseFloat(Math.round(total * 100) / 100, 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString();
    }

    return strResult;
}

$.formatPercent = function (total) {
    var strResult = '';
    if (total < 0) {
        strResult = parseFloat((0 - total), 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString();
        if (strResult != "0.00") {
            strResult = '-' + strResult;
        }
    } else {
        strResult = parseFloat(total, 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString();
    }

    return strResult + ('%');
}

$.formatNumberSave = function (value) {
    return parseFloat(value).toFixed(5);
}

$.showOrHideSection = function (sectionName) {
    showProcessing();

    $('div[class^="Area_Category_"]').each(function () {
        var name = $(this).attr('class');
        if (name.indexOf(sectionName) != -1) {
            $(this).show();
        } else {
            $(this).hide();
        }
    });

    // set focus annual sales item
    $('.active:last input[id^="Annual_Sales_"]').focus();

    // remove class hightlight
    $('[class^="menu-budget-input-"]').removeClass('highlight-secornd');

    switch (sectionName) {
        case "Sales":
            $('#headerLaber').html("TARGET & SALES");
            changeTitle("Target & Sales");
            $('.menu-budget-input-sales').addClass('highlight-secornd');
            break;
        case "COGS":
            $('#headerLaber').html("COGS");
            changeTitle("COGS");
            $('.menu-budget-input-cogs').addClass('highlight-secornd');
            break;
        case "Payroll_Expenses":
            $('#headerLaber').html("PAYROLL EXPENSES");
            changeTitle("Payroll Expenses");
            $('.menu-budget-input-payroll').addClass('highlight-secornd');
            break;
        case "Operation_Expenses":
            $('#headerLaber').html("OPERATION EXPENSES");
            changeTitle("Operation Expenses");
            $('.menu-budget-input-operation').addClass('highlight-secornd');
            break;
    }
}

$.htmlEscape = function (str) {
    return str
        .replace(/&/g, '&amp;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#39;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;');
}

$.htmlUnescape = function (str) {
    return str
        .replace(/&quot;/g, '"')
        .replace(/&#39;/g, "'")
        .replace(/&lt;/g, '<')
        .replace(/&gt;/g, '>')
        .replace(/&amp;/g, '&');
}

var encodeHtmlEntity = function (str) {
    var buf = [];
    for (var i = str.length - 1; i >= 0; i--) {
        buf.unshift(['&#', str[i].charCodeAt(), ';'].join(''));
    }
    return buf.join('');
};

// change user password
function changeUserPassword() {
    var options = {
        id: 'popupChangeUserPassword',
        title: 'Change Password',
        width: 680
    };
    var popupWindow = window.setaJs.initPopupWindow(options);
    popupWindow.refresh({
        url: '/User/ChangePassword',
        async: false
    });
    popupWindow.center().open();

    //init form
    var $form = $('#frmChangePassword');
    $("#txtOldPassword").focus();
    window.setaJs.focusFirstInputForm($form);

    //handler button
    $form.find(".btn-accept-no").on("click", function () {
        if (popupWindow) popupWindow.close();
    });

    //handler button
    $form.find(".k-grid-update").on("click", function () {
        // clear all error message before submit
        $('.error').empty();
       
        var txtOldPassword = $("#txtOldPassword").val();
        var txtPassword = $("#txtPassword").val();
        var txtConfirmPassword = $("#txtConfirmPassword").val();
        var oldPassword = $("#Password").val();
        var crurrentPass = $.MD5(txtOldPassword);
        // check valid
        if (txtOldPassword.length == 0) {
            $(".olPasswordInput .error").empty().append("</br> The Current Password field is required.");
            $("#txtOldPassword").focus();
            return;
        }
        if (txtPassword.length == 0) {
            $(".passwordInput .error").empty().append("</br> The New Password field is required.");
            $("#txtPassword").focus();
            return;
        }
        if (txtConfirmPassword.length == 0) {
            $(".confirmPassword .error").empty().append("</br> The Confirm Password field is required.");
            $("#txtConfirmPassword").focus();
            return;
        }

        if ((txtPassword.length < 6 || txtPassword.length > 20) && (txtOldPassword.length < 6 || txtOldPassword.length > 20) && (txtConfirmPassword.length < 6 || txtConfirmPassword.length > 20)) {
            $(".olPasswordInput .error").empty().append("Current Password length must be between 6 to 20 characters.");
            $(".passwordInput .error").empty().append("New Password length must be between 6 to 20 characters.");
            $(".confirmPassword .error").empty().append("Confirm Password length must be between 6 to 20 characters.");
            return;
        }
        if ((txtPassword.length < 6 || txtPassword.length > 20) && (txtOldPassword.length < 6 || txtOldPassword.length > 20)) {
            $(".olPasswordInput .error").empty().append("Current Password length must be between 6 to 20 characters.");
            $(".passwordInput .error").empty().append("New Password length must be between 6 to 20 characters.");
            return;
        }
        if (txtPassword.length > 0 && txtConfirmPassword.length > 0) {
            if ((txtPassword.length < 6 || txtPassword.length > 20) && (txtConfirmPassword.length < 6 || txtConfirmPassword.length > 20)) {
                $(".passwordInput .error").empty().append("New Password length must be between 6 to 20 characters.");
                $(".confirmPassword .error").empty().append("Confirm Password length must be between 6 to 20 characters.");
                return;
            }
        }

        if (txtPassword.length < 6 || txtPassword.length > 20) {
            $(".passwordInput .error").empty().append("</br> New Password length must be between 6 to 20 characters.");
            return;
        }
       
        if (txtConfirmPassword.length < 6 || txtConfirmPassword.length > 20) {
            $(".confirmPassword .error").empty().append("</br> Confirm Password length must be between 6 to 20 characters.");
            return;
        }
        if (txtPassword == txtOldPassword) {
            $(".passwordInput .error").empty().append("</br> New Password should not be same as Current Password.");
            return;
        }
        if (txtPassword.localeCompare(txtConfirmPassword)) {
            $(".confirmPassword .error").empty().append("</br> Confirm Password does not match with New Password.");
            return;
        }
        //check newpassword vs old password
        if (crurrentPass != oldPassword) {
            $(".olPasswordInput .error").empty().append("</br> The Current Password you have entered is incorrect.");
            return;
        }

        var obj = {
            UserId: $('#UserId').val(),
            OldPassword: txtOldPassword,
            Password: txtPassword,
            ConfirmPassword: txtConfirmPassword,
            CheckOldPassword: $('#CheckOldPassword').val(),
            CheckPassword: $('#CheckPassword').val(),
        };

        // cal action save change password
        $.ajax({
            type: "POST",
            datatype: "json",
            url: '/User/SaveChangePassword',
            data: { model: obj },
            success: function (response) {
                if (response.Status) {
                    warningPopup("Information", response.Message, "/Security/Logout");
                }
                else {
                    warningPopup("Warning", response.Message);
                }
            }
        });
    });
}

$.reCalcHeightBody = function () {
    var windowHeight = $(window).height();
    var heightMenuAndFooter = $('.page-footer').css('display') == 'none' ? 0 : 42;
    heightMenuAndFooter += $('.page-header').css('display') == 'none' ? 0 : 50;
    heightMenuAndFooter += $('.menu-first').css('display') == 'none' ? 0 : $('.menu-first').height();
    heightMenuAndFooter += $('.menu-seconrd').css('display') == 'none' ? 0 : $('.menu-seconrd').height();
    heightMenuAndFooter += $('.menu-bottom').css('display') == 'none' ? 0 : $('.menu-bottom').height();

    $('#mainContent').height(windowHeight - heightMenuAndFooter);

    var helpMenu = 0;//$('.Hide-HelpSettingTargetAndSales').length == 0 ? 0 : $('.Hide-HelpSettingTargetAndSales').height() - 15;
    var budgetAreaHeight = windowHeight - heightMenuAndFooter - helpMenu - 300;
    $('.budget-area').height(budgetAreaHeight);
}

$.onShowDialogSwitchUser = function () {
    var options = {
        id: 'switchUser',
        title: 'Switch User',
        resizable: false,
        width: 680,
    };
    var popupWindow = window.setaJs.initPopupWindow(options);
    popupWindow.refresh({
        url: '/User/ViewSwitchUser',
        async: false
    }).center().open();
    popupWindow.center().open();

    //init form
    var $form = $('#switchUser');

    // close this popup
    $form.find("#act-accept-no").on("click", function () {
        if (popupWindow) popupWindow.close();
    });

    //handler button
    $form.find("#act-accept-yes").on("click", function () {
        // change swith user by user id
        var userId = $('#switchUserComboBox').val();
        if (userId.length == 0) {
            $form.find('#switchUserPopupMessage').html("Please select one user before clicking Switch button.");
            return;
        }

        // call action swith user to support
        $.ajax({
            type: "POST",
            datatype: "json",
            url: '/User/SwithUserById',
            data: { userId: userId },
            success: function (response) {
                if (response.Status) {
                    // close this popup
                    if (popupWindow) popupWindow.close();

                    // reload page
                    window.location = "/Dashboard/Index";
                }
                else {
                    warningPopup("Warning", response.Message);
                }
            }
        });
    });
}

$.backToHomeSwitchUser = function () {
    // call action back to home switch user
    $.ajax({
        type: "POST",
        datatype: "json",
        url: '/User/BackToSwithUser',
        //data: { userId: userId },
        success: function (response) {
            if (response.Status) {
                // reload page
                window.location = "/Home/Index";
            }
            else {
                warningPopup("Warning", response.Message);
            }
        }
    });
}

$.colorArray = [
    "#8b8378",
    "#76eec6",
    "#c1cdcd",
    "#0000ff",
    "#8a2be2",
    "#a52a2a",
    "#76ee00",
    "#ff7f24",
    "#ee6a50",
    "#6495ed",
    "#006400",
    "#a2cd5a",
    "#68228b",
    "#483d8b",
    "#1e90ff",
    "#ffd700",
    "#1f1f1f",
    "#8470ff",
    "#698b22",
    "#27408b",
    "#9acd32"
];


// MD5 (Message-Digest Algorithm)
$.MD5 = function (string) {

    function RotateLeft(lValue, iShiftBits) {
        return (lValue << iShiftBits) | (lValue >>> (32 - iShiftBits));
    }

    function AddUnsigned(lX, lY) {
        var lX4, lY4, lX8, lY8, lResult;
        lX8 = (lX & 0x80000000);
        lY8 = (lY & 0x80000000);
        lX4 = (lX & 0x40000000);
        lY4 = (lY & 0x40000000);
        lResult = (lX & 0x3FFFFFFF) + (lY & 0x3FFFFFFF);
        if (lX4 & lY4) {
            return (lResult ^ 0x80000000 ^ lX8 ^ lY8);
        }
        if (lX4 | lY4) {
            if (lResult & 0x40000000) {
                return (lResult ^ 0xC0000000 ^ lX8 ^ lY8);
            } else {
                return (lResult ^ 0x40000000 ^ lX8 ^ lY8);
            }
        } else {
            return (lResult ^ lX8 ^ lY8);
        }
    }

    function F(x, y, z) { return (x & y) | ((~x) & z); }
    function G(x, y, z) { return (x & z) | (y & (~z)); }
    function H(x, y, z) { return (x ^ y ^ z); }
    function I(x, y, z) { return (y ^ (x | (~z))); }

    function FF(a, b, c, d, x, s, ac) {
        a = AddUnsigned(a, AddUnsigned(AddUnsigned(F(b, c, d), x), ac));
        return AddUnsigned(RotateLeft(a, s), b);
    };

    function GG(a, b, c, d, x, s, ac) {
        a = AddUnsigned(a, AddUnsigned(AddUnsigned(G(b, c, d), x), ac));
        return AddUnsigned(RotateLeft(a, s), b);
    };

    function HH(a, b, c, d, x, s, ac) {
        a = AddUnsigned(a, AddUnsigned(AddUnsigned(H(b, c, d), x), ac));
        return AddUnsigned(RotateLeft(a, s), b);
    };

    function II(a, b, c, d, x, s, ac) {
        a = AddUnsigned(a, AddUnsigned(AddUnsigned(I(b, c, d), x), ac));
        return AddUnsigned(RotateLeft(a, s), b);
    };

    function ConvertToWordArray(string) {
        var lWordCount;
        var lMessageLength = string.length;
        var lNumberOfWords_temp1 = lMessageLength + 8;
        var lNumberOfWords_temp2 = (lNumberOfWords_temp1 - (lNumberOfWords_temp1 % 64)) / 64;
        var lNumberOfWords = (lNumberOfWords_temp2 + 1) * 16;
        var lWordArray = Array(lNumberOfWords - 1);
        var lBytePosition = 0;
        var lByteCount = 0;
        while (lByteCount < lMessageLength) {
            lWordCount = (lByteCount - (lByteCount % 4)) / 4;
            lBytePosition = (lByteCount % 4) * 8;
            lWordArray[lWordCount] = (lWordArray[lWordCount] | (string.charCodeAt(lByteCount) << lBytePosition));
            lByteCount++;
        }
        lWordCount = (lByteCount - (lByteCount % 4)) / 4;
        lBytePosition = (lByteCount % 4) * 8;
        lWordArray[lWordCount] = lWordArray[lWordCount] | (0x80 << lBytePosition);
        lWordArray[lNumberOfWords - 2] = lMessageLength << 3;
        lWordArray[lNumberOfWords - 1] = lMessageLength >>> 29;
        return lWordArray;
    };

    function WordToHex(lValue) {
        var WordToHexValue = "", WordToHexValue_temp = "", lByte, lCount;
        for (lCount = 0; lCount <= 3; lCount++) {
            lByte = (lValue >>> (lCount * 8)) & 255;
            WordToHexValue_temp = "0" + lByte.toString(16);
            WordToHexValue = WordToHexValue + WordToHexValue_temp.substr(WordToHexValue_temp.length - 2, 2);
        }
        return WordToHexValue;
    };

    function Utf8Encode(string) {
        string = string.replace(/\r\n/g, "\n");
        var utftext = "";

        for (var n = 0; n < string.length; n++) {

            var c = string.charCodeAt(n);

            if (c < 128) {
                utftext += String.fromCharCode(c);
            }
            else if ((c > 127) && (c < 2048)) {
                utftext += String.fromCharCode((c >> 6) | 192);
                utftext += String.fromCharCode((c & 63) | 128);
            }
            else {
                utftext += String.fromCharCode((c >> 12) | 224);
                utftext += String.fromCharCode(((c >> 6) & 63) | 128);
                utftext += String.fromCharCode((c & 63) | 128);
            }

        }

        return utftext;
    };

    var x = Array();
    var k, AA, BB, CC, DD, a, b, c, d;
    var S11 = 7, S12 = 12, S13 = 17, S14 = 22;
    var S21 = 5, S22 = 9, S23 = 14, S24 = 20;
    var S31 = 4, S32 = 11, S33 = 16, S34 = 23;
    var S41 = 6, S42 = 10, S43 = 15, S44 = 21;

    string = Utf8Encode(string);

    x = ConvertToWordArray(string);

    a = 0x67452301; b = 0xEFCDAB89; c = 0x98BADCFE; d = 0x10325476;

    for (k = 0; k < x.length; k += 16) {
        AA = a; BB = b; CC = c; DD = d;
        a = FF(a, b, c, d, x[k + 0], S11, 0xD76AA478);
        d = FF(d, a, b, c, x[k + 1], S12, 0xE8C7B756);
        c = FF(c, d, a, b, x[k + 2], S13, 0x242070DB);
        b = FF(b, c, d, a, x[k + 3], S14, 0xC1BDCEEE);
        a = FF(a, b, c, d, x[k + 4], S11, 0xF57C0FAF);
        d = FF(d, a, b, c, x[k + 5], S12, 0x4787C62A);
        c = FF(c, d, a, b, x[k + 6], S13, 0xA8304613);
        b = FF(b, c, d, a, x[k + 7], S14, 0xFD469501);
        a = FF(a, b, c, d, x[k + 8], S11, 0x698098D8);
        d = FF(d, a, b, c, x[k + 9], S12, 0x8B44F7AF);
        c = FF(c, d, a, b, x[k + 10], S13, 0xFFFF5BB1);
        b = FF(b, c, d, a, x[k + 11], S14, 0x895CD7BE);
        a = FF(a, b, c, d, x[k + 12], S11, 0x6B901122);
        d = FF(d, a, b, c, x[k + 13], S12, 0xFD987193);
        c = FF(c, d, a, b, x[k + 14], S13, 0xA679438E);
        b = FF(b, c, d, a, x[k + 15], S14, 0x49B40821);
        a = GG(a, b, c, d, x[k + 1], S21, 0xF61E2562);
        d = GG(d, a, b, c, x[k + 6], S22, 0xC040B340);
        c = GG(c, d, a, b, x[k + 11], S23, 0x265E5A51);
        b = GG(b, c, d, a, x[k + 0], S24, 0xE9B6C7AA);
        a = GG(a, b, c, d, x[k + 5], S21, 0xD62F105D);
        d = GG(d, a, b, c, x[k + 10], S22, 0x2441453);
        c = GG(c, d, a, b, x[k + 15], S23, 0xD8A1E681);
        b = GG(b, c, d, a, x[k + 4], S24, 0xE7D3FBC8);
        a = GG(a, b, c, d, x[k + 9], S21, 0x21E1CDE6);
        d = GG(d, a, b, c, x[k + 14], S22, 0xC33707D6);
        c = GG(c, d, a, b, x[k + 3], S23, 0xF4D50D87);
        b = GG(b, c, d, a, x[k + 8], S24, 0x455A14ED);
        a = GG(a, b, c, d, x[k + 13], S21, 0xA9E3E905);
        d = GG(d, a, b, c, x[k + 2], S22, 0xFCEFA3F8);
        c = GG(c, d, a, b, x[k + 7], S23, 0x676F02D9);
        b = GG(b, c, d, a, x[k + 12], S24, 0x8D2A4C8A);
        a = HH(a, b, c, d, x[k + 5], S31, 0xFFFA3942);
        d = HH(d, a, b, c, x[k + 8], S32, 0x8771F681);
        c = HH(c, d, a, b, x[k + 11], S33, 0x6D9D6122);
        b = HH(b, c, d, a, x[k + 14], S34, 0xFDE5380C);
        a = HH(a, b, c, d, x[k + 1], S31, 0xA4BEEA44);
        d = HH(d, a, b, c, x[k + 4], S32, 0x4BDECFA9);
        c = HH(c, d, a, b, x[k + 7], S33, 0xF6BB4B60);
        b = HH(b, c, d, a, x[k + 10], S34, 0xBEBFBC70);
        a = HH(a, b, c, d, x[k + 13], S31, 0x289B7EC6);
        d = HH(d, a, b, c, x[k + 0], S32, 0xEAA127FA);
        c = HH(c, d, a, b, x[k + 3], S33, 0xD4EF3085);
        b = HH(b, c, d, a, x[k + 6], S34, 0x4881D05);
        a = HH(a, b, c, d, x[k + 9], S31, 0xD9D4D039);
        d = HH(d, a, b, c, x[k + 12], S32, 0xE6DB99E5);
        c = HH(c, d, a, b, x[k + 15], S33, 0x1FA27CF8);
        b = HH(b, c, d, a, x[k + 2], S34, 0xC4AC5665);
        a = II(a, b, c, d, x[k + 0], S41, 0xF4292244);
        d = II(d, a, b, c, x[k + 7], S42, 0x432AFF97);
        c = II(c, d, a, b, x[k + 14], S43, 0xAB9423A7);
        b = II(b, c, d, a, x[k + 5], S44, 0xFC93A039);
        a = II(a, b, c, d, x[k + 12], S41, 0x655B59C3);
        d = II(d, a, b, c, x[k + 3], S42, 0x8F0CCC92);
        c = II(c, d, a, b, x[k + 10], S43, 0xFFEFF47D);
        b = II(b, c, d, a, x[k + 1], S44, 0x85845DD1);
        a = II(a, b, c, d, x[k + 8], S41, 0x6FA87E4F);
        d = II(d, a, b, c, x[k + 15], S42, 0xFE2CE6E0);
        c = II(c, d, a, b, x[k + 6], S43, 0xA3014314);
        b = II(b, c, d, a, x[k + 13], S44, 0x4E0811A1);
        a = II(a, b, c, d, x[k + 4], S41, 0xF7537E82);
        d = II(d, a, b, c, x[k + 11], S42, 0xBD3AF235);
        c = II(c, d, a, b, x[k + 2], S43, 0x2AD7D2BB);
        b = II(b, c, d, a, x[k + 9], S44, 0xEB86D391);
        a = AddUnsigned(a, AA);
        b = AddUnsigned(b, BB);
        c = AddUnsigned(c, CC);
        d = AddUnsigned(d, DD);
    }

    var temp = WordToHex(a) + WordToHex(b) + WordToHex(c) + WordToHex(d);

    return temp.toLowerCase();
}
