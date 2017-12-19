$.CurrentUserId = parseInt($('.CurrentUserId').val());

var counter = 1;
function renderNumber(data) {
    return counter++;
}

// renderNumberBySuper
var counterBySuper = 1;
function renderNumberBySuper() {
    return counterBySuper++;
}

// renderNumberByTech
var counterByTech = 1;
function renderNumberByTech() {
    return counterByTech++;
}

// renderNumberByStandalone
var counterByStandalone = 1;
function renderNumberByStandalone() {
    return counterByStandalone++;
}

// renderNumberByAssociated
var counterByAssociated = 1;
function renderNumberByAssociated() {
    return counterByAssociated++;
}

// function add new user
function addNewUser(gridId, userId) {
    var title = userId > 0 ? "Edit User" : "Add New User";
    var options = {
        id: 'popupEditUser',
        title: title,
        width: 680,
    };
    var popupWindow = window.setaJs.initPopupWindow(options);
    popupWindow.refresh({
        url: '/User/ViewEditUser',
        data: { userId: userId },
        async: false
    });
    popupWindow.center().open();

    //init form
    var $form = $('#formEditUser');
    window.setaJs.focusFirstInputForm($form);

    // add information to new user
    if (userId == 0) {
        $form.find('#SystemId').val(1);
        $form.find('#RoleId').val(gridId == "SuperUserGrid" ? 1 : (gridId == "TechUserGrid" ? 2 : 3));
        $form.find('#Active').val(true);
        $form.find('#DeletedFlg').val(false);
    }

    //handler button
    $form.find(".btn-accept-no").on("click", function () {
        if (popupWindow) popupWindow.close();
    });

    //handler button
    $form.find(".btn-accept-yes").on("click", function () {
        // clear all error message before submit
        $form.find('.error').empty();

        // check not exists error in form
        if (validationFormEditUser($form, userId)) {
            $.ajax({
                type: "POST",
                datatype: "json",
                url: '/User/EditUser',
                data: $form.serialize(),
                success: function (response) {
                    if (response.DuplicateEmail || response.DuplicateUserName) {
                        // set message show on popup
                        if (response.DuplicateEmail)
                            $form.find(".email-message").empty().append("This e-mail address already exists.");

                        // set message show on popup
                        if (response.DuplicateUserName)
                            $form.find(".user-name-message").empty().append("This User Name already exists.");

                        return;
                    }

                    if (response.Status) {
                        // call refresh grid
                        $('#' + gridId).data('kendoGrid').dataSource.read();

                        if (popupWindow) popupWindow.close();
                    }
                    else {
                        warningPopup("Warning", response.Message);
                    }
                }
            });
        }
    });
}

function validationFormEditUser($form, userId) {
    var validationFlg = true;

    // check validation user name
    if ($form.find("#txtUserName").val().length == 0) {
        $form.find(".user-name-message").empty().text("The User Name field is required.");
        validationFlg = false;
    } else if ($form.find("#txtUserName").val().length < 3 || $form.find("#txtUserName").val().length > 255) {
        $form.find(".user-name-message").empty().text("User Name length must be between 3 to 255 characters.");
        validationFlg = false;
    }

    if (userId == 0) {
        // check validation password
        if ($form.find("#txtPassword").val().length == 0) {
            $form.find(".password-message").empty().text("The Password field is required.");
            validationFlg = false;
        } else if ($form.find("#txtPassword").val().length < 6 || $form.find("#txtPassword").val().length > 255) {
            $form.find(".password-message").empty().text("Password length must be between 6 to 255 characters.");
            validationFlg = false;
        }

        // check validation confirm password
        if ($form.find("#txtConfirmPassword").val().length == 0) {
            $form.find(".confirm-password-message").empty().text("The Confirm Password field is required.");
            validationFlg = false;
        } else if ($form.find("#txtConfirmPassword").val().length < 6 || $form.find("#txtConfirmPassword").val().length > 255) {
            $form.find(".confirm-password-message").empty().text("Confirm Password length must be between 6 to 255 characters.");
            validationFlg = false;
        } else if ($("#txtConfirmPassword").val() != $("#txtPassword").val()) {
            $(".confirm-password-message").empty().append("Confirm Password does not match with Password.");
            validationFlg = false;
        }
    } else {
        if ($form.find("#txtPassword").val().length > 0) {
            if ($form.find("#txtPassword").val().length < 6 || $form.find("#txtPassword").val().length > 255) {
                $form.find(".password-message").empty().text("Password length must be between 6 to 255 characters.");
                validationFlg = false;
            }

            if ($form.find("#txtConfirmPassword").val().length < 6 || $form.find("#txtConfirmPassword").val().length > 255) {
                $form.find(".confirm-password-message").empty().text("Confirm Password length must be between 6 to 255 characters.");
                validationFlg = false;
            } else if ($("#txtConfirmPassword").val() != $("#txtPassword").val()) {
                $(".confirm-password-message").empty().append("Confirm Password does not match with Password.");
                validationFlg = false;
            }
        }
    }

    // check validation full name
    if ($form.find("#txtFullName").val().length == 0) {
        $form.find(".full-name-message").empty().append("The Full Name field is required.");
        validationFlg = false;
    } else if ($form.find("#txtFullName").val().length < 3 || $form.find("#txtFullName").val().length > 255) {
        $form.find(".full-name-message").empty().append("Full Name length must be between 3 to 255 characters.");
        validationFlg = false;
    }

    // check validation email
    if ($form.find("#txtEmail").val().length == 0) {
        $form.find(".email-message").empty().append("The Email field is required.");
        validationFlg = false;
    } else if (!validateEmail($form.find("#txtEmail").val())) {
        $form.find(".email-message").empty().append("The Email field is not a valid e-mail address.");
        validationFlg = false;
    }

    // check phone number when input
    if ($form.find("#txtPhone").val().length > 0 && ($form.find("#txtPhone").val().length < 6 || $form.find("#txtPhone").val().length > 30)) {
        $form.find(".phone-message").empty().append("Phone length must be between 6 to 30 characters.");
        validationFlg = false;
    }

    return validationFlg;
}

// function draw button active and delete by row
function getActionByRow(gridId, UserId, Active) {
    if ($.CurrentUserId == UserId) {
        return '<label class="btn-crm btn-crm-action" title="Edit User" onclick="addNewUser(\'' + gridId + '\', ' + UserId + ')"><i class="fa fa-edit"></i></label>';
    }

    var str = '';
    if (Active) {
        str += '<label class="btn-crm btn-crm-action" title="In-Active User" onclick="changeActiveById(' + gridId + ', ' + UserId + ', false)"><i class="fa fa-lock"></i></label>';
    } else {
        str += '<label class="btn-crm btn-crm-action" title="Active User" onclick="changeActiveById(' + gridId + ', ' + UserId + ', true)"><i class="fa fa-unlock"></i></label>';
    }
    str += '<label class="btn-crm btn-crm-action" title="Edit User" onclick="addNewUser(\'' + gridId + '\', ' + UserId + ')"><i class="fa fa-edit"></i></label>';
    str += '<label class="btn-crm btn-crm-action" title="Delete User" onclick="deleteUserById(' + gridId + ', ' + UserId + ')"><i class="fa fa-trash"></i></label>';
    return str;
}

// function draw button active and delete by row
function getActionByRowAssociatedUser(gridId, UserId, Active) {
    var str = '';
    if (Active) {
        str += '<label class="btn-crm btn-crm-action" title="In-Active User" onclick="changeActiveById(' + gridId + ', ' + UserId + ', false)"><i class="fa fa-lock"></i></label>';
    } else {
        str += '<label class="btn-crm btn-crm-action" title="Active User" onclick="changeActiveById(' + gridId + ', ' + UserId + ', true)"><i class="fa fa-unlock"></i></label>';
    }
    str += '<label class="btn-crm btn-crm-action" title="Edit User" onclick="addNewUser(\'' + gridId + '\', ' + UserId + ')"><i class="fa fa-edit"></i></label>';
    return str;
}

// function call delete user by id
function deleteUserById(gridId, id) {
    var options = {
        id: "confirmDelteUserPopup",
        typeStatus: true,
        title: "Confirm",
        confirmText: "Are you sure you want to delete this User?",
        textYes: "Yes",
        textNo: "No"
    }
    var popupWindow = window.setaJs.initPopupWindow(options);
    popupWindow.refresh({}).center().open();
    popupWindow.center().open();

    var $form = $('#' + options.id);
    $form.find("#act-accept-no").on("click", function () {
        if (popupWindow) popupWindow.close();
    });
    $form.find("#act-accept-yes").on("click", function () {
        showProcessing();
        $.ajax({
            url: '/User/DeleteByUserId',
            type: "GET",
            async: false,
            data: { userId: id },
            success: function (result) {
                if (result.Status) {
                    // call refresh grid
                    $(gridId).data('kendoGrid').dataSource.read();

                    if (popupWindow) popupWindow.close();
                } else {
                    warningPopup("Warning", "Process delete failure.");
                }
            },
            error: function () {
                warningPopup("Warning", "Process error, Please contact system admin!");
            }
        });
    });
}

// function call reset status active/inactive by id
function changeActiveById(gridId, id, status) {
    $.ajax({
        url: '/User/ChangeActiveStatusByUserId',
        type: "GET",
        async: false,
        data: { userId: id, activeFlg: status },
        success: function (result) {
            if (result.Status) {
                // call refresh grid
                $(gridId).data('kendoGrid').dataSource.read();
            } else {
                warningPopup("Warning", "Process change status failure.");
            }
        },
        error: function () {
            warningPopup("Warning", "Process error, Please contact system admin!");
        }
    });
}
