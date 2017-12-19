$(document).ready(function (e) {
    $("#startDate").attr("readonly", true);
    $("#endDate").attr("readonly", true);

    $(".budget-tag-body").mouseenter(function () {
        $(this).find(".menuConfig").removeClass("hidden");
    }).mouseleave(function () {
        $(this).find(".menuConfig").addClass("hidden");
    });

    $('.btn-filter-budget').unbind('click');
    $('.btn-filter-budget').on('click', filterBudget);

    $(".btn-clear-filter-budget").on('click', clearFilterBudget);

    $('.showHideFilter').on('click', function () {
        showOrHideFilter(this)
    });

    $('#labelShowHideFilter').on('click', function () {
        $('.showHideFilter').click();
    });

    // clear all cookie after load page
    setTimeout(function () {
        $.removeCookie("restCode");
        $.removeCookie("createdUser");
        $.removeCookie("startDate");
        $.removeCookie("endDate");
        $.removeCookie("budgetType");
    }, 500);
});

function filterBudget() {
    showProcessing();
    var startDate = $("#startDate").data('kendoDatePicker').value();
    var endDate = $("#endDate").data('kendoDatePicker').value();
    if (endDate != null && startDate > endDate) {
        $('#errorLabel').html('Budget Start To cannot be before Budget Start From.');
        $('#errorLabel').show();
        return;
    } else {
        $('#errorLabel').hide();
    }

    // call action search budget
    var restCode = '';
    if ($("#restCode").length > 0) {
        restCode = JSON.stringify($("#restCode").data('kendoDropDownList').value());
    }
    $.cookie("restCode", restCode);

    var createdUser = '';
    if ($("#createdUser").length > 0) {
        createdUser = JSON.stringify($("#createdUser").data('kendoDropDownList').value());
    }
    $.cookie("createdUser", createdUser);

    var startDate = '';
    if ($("#startDate").data('kendoDatePicker') != null) {
        startDate = JSON.stringify($.datepicker.formatDate('dd M yy', $("#startDate").data('kendoDatePicker').value()));
    }
    $.cookie("startDate", startDate);

    var endDate = '';
    if ($("#startDate").data('kendoDatePicker') != null) {
        endDate = JSON.stringify($.datepicker.formatDate('dd M yy', $("#endDate").data('kendoDatePicker').value()));
    }
    $.cookie("endDate", endDate);

    var budgetType = '';
    if ($("#BudgetType").data('kendoDropDownList').value() != null) {
        budgetType = JSON.stringify($("#BudgetType").data('kendoDropDownList').value());
    }
    $.cookie("budgetType", budgetType);

    // filter
    window.location.href = "/Dashboard/FilterBudget";
}

function clearFilterBudget() {
    window.location = "/Dashboard/Index";
}

function linkedToRestaurant(id, code) {
    window.location = "/Budget/ViewLinkToYourRestaurant?id=" + id;
}

function viewDetail(id, code) {
    window.location = "/Budget/ViewTagetAndSales?id=" + id;
}

function deleteThisBudget(id) {
    var options = {
        id: "warningPopup",
        typeStatus: true,
        title: "Confirm",
        confirmText: "Are you sure you want to delete this Budget?",
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
            url: '/Dashboard/Delete',
            type: "GET",
            async: false,
            data: { Id: id },
            success: function (result) {
                if (result.Status) {
                    if (popupWindow) popupWindow.close();
                    window.location = "/Dashboard/Index";
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

function FilterUserByRestCode() {
    return {
        restCode: $.trim($("#restCode").data('kendoDropDownList').value())
    };
}

function onChangeRestCode() {
    $("#createdUser").data('kendoDropDownList').dataSource.read();
    $("#createdUser").data('kendoDropDownList').refresh();
}

function showOrHideFilter(item) {
    var currentClass = $(item).children().attr('class');
    if (currentClass.indexOf('right') != -1) {
        $(item).children().attr('class', 'fa fa-angle-double-down');
        $('.filter-box-area').css('display', 'block');
        $(item).next().html('Hidden filter budgets');
    } else {
        $(item).children().attr('class', 'fa fa-angle-double-right');
        $('.filter-box-area').css('display', 'none');
        $(item).next().html('Show filter budgets');
    }
}

function importActualNumber(id) {
    // 1. get all tab by budget
    $.ajax({
        url: '/Budget/GetBudgetTabList',
        type: "GET",
        async: false,
        data: { Id: id },
        success: function (result) {
            if (result.length == 1) {
                // redirect to import acutal the first tab
                var budgetTabId = result[0].BudgetTabId;
                return window.location = "/Budget/ImportActualNumber?id=" + id + "&budgetTabId=" + budgetTabId + "&headerName=&redirectPage=Dashboard";
            } else {
                var confirmText = '<div class="col-xs-12" style="margin-top: 15px;"><div class="col-xs-4">Budget Tab</div><div class="col-xs-8"><select name="BudgetTabList">';
                $.each(result, function (i, item) {
                    confirmText += '<option value="' + item.BudgetTabId + '">' + item.TabName + '</option>';
                });
                confirmText += '</select></div></div>';

                // show popup choise budget tab
                var options = {
                    id: "budgetTabListPopup",
                    typeStatus: true,
                    title: "Budget Tab List",
                    confirmText: confirmText,
                    textYes: "Next",
                    textNo: "Close"
                }
                var popupWindow = window.setaJs.initPopupWindow(options);
                popupWindow.refresh({}).center().open();

                var $form = $('#' + options.id);
                $form.find("#act-accept-no").on("click", function () {
                    if (popupWindow) popupWindow.close();
                });
                $form.find("#act-accept-yes").on("click", function () {
                    var budgetTabId = $('select[name="BudgetTabList"]').val();
                    return window.location = "/Budget/ImportActualNumber?id=" + id + "&budgetTabId=" + budgetTabId + "&headerName=&redirectPage=Dashboard";
                });
            }
        },
        error: function () {
            warningPopup("Warning", "Process error, Please contact system admin!");
        }
    });
}

//Clone budget
function CloneBudget(budgetId, budgetName) {
    var options = {
        id: "warningPopup",
        typeStatus: true,
        title: "Clone Budget",
        confirmText: '<div class="newBudgetInput"><input class="bcs-textbox" placeholder="Clone Budget" name="BudgetName" maxlength="255" value="Clone - ' + budgetName + '"/><br/><span class="error"></span></div>',
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
            url: '/Budget/CloneBudget',
            type: "GET",
            async: false,
            data: { id: budgetId, budgetName: budgetName },
            success: function (result) {
                if (result.Status) {
                    if (popupWindow) popupWindow.close();
                    warningPopup("Information", result.Message, "/Budget/ViewTagetAndSales?id=" + result.BudgetId);
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

function RestoreBudget(budgetId) {
    showProcessing();
    $.ajax({
        url: '/Dashboard/RestoreBudgetDeleted',
        type: "GET",
        async: false,
        data: { budgetId: budgetId },
        success: function (result) {
            if (result.Status) {
                window.location = "/Dashboard/ViewRecycleBin";
            } else {
                warningPopup("Warning", "Process delete failure.");
            }
        },
        error: function () {
            warningPopup("Warning", "Process error, Please contact system admin!");
        }
    });
}