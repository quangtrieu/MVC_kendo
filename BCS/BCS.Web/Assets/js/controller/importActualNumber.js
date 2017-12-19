$(document).ready(function () {
    onChangeFunction();
})

//Process Input Method by bugget
function callSaveInputMethod() {
    showProcessing();

    var budgetId = parseInt($("#BudgetId").val());
    var inputMethod = $("#InputMethod").val();
    var actualNumbersFlg = $('input[type="checkbox"][name="ActualNumbersFlg.Value"]:checked').length > 0 ? true : false;
    var varianceFlg = $('input[type="checkbox"][name="VarianceFlg.Value"]:checked').length > 0 ? true : false;
    $.ajax({
        url: '/Budget/SaveInputMethodBudget',
        type: "POST",
        data: JSON.stringify({ budgetId: budgetId, inputMethod: inputMethod, actualNumbersFlg: actualNumbersFlg, varianceFlg: varianceFlg }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        async: false,
        success: function (response) {
            if (response.Status) {
                warningPopup("Input Method", response.Message);
            } else {
                warningPopup("Warning", "Input Method update falied.");
            }
        }
    });
}

// process upload file
function onUpload(e) {
    $.each(e.files, function (index, value) {
        var ok = value.extension == ".XLS"
                 || value.extension == ".xls"
                 || value.extension == ".XLSX"
                 || value.extension == ".xlsx";
        if (this.size / 1024 / 1024 > 5) {
            e.preventDefault();
            warningPopup("Warning", "The file size cannot greater than 5MB.");
        }
        if (!ok) {
            e.preventDefault();
            warningPopup("Warning", "Your selected file is invalid. Please select a .XLS/.XLSX file");
        } else {
            return;
        }
    });
}

var fileImportActual = "";
function onSuccess(result) {
    filePath_uploadInportActual = "";
    var ddl = $("#sheetName").data("kendoDropDownList");
    ddl.setDataSource(result.response.data)
    ddl.refresh();
    $("#fileName").val(result.response.fileName);

    if (!result.response.Status) {
        warningPopup("Warning", "This file is empty. Please select another file");
    }
}

function callReadExcelFile() {
    showProcessing();
    // check upload file
    fileImportActual = $("#fileName").val();
    if (typeof fileImportActual === "undefined" || fileImportActual.length <= 0) {
        warningPopup("Warning", "Please select a file before clicking continue!");
        return;
    }
    //alert(fileImportActual);
    var sheetname = $("#sheetName").val();
    if (sheetname === "undefined" || sheetname == "") {
        warningPopup("Warning", "Please select a sheet name before clicking continue!");
        return;
    }
    var firstValue = '';
    var secondValue = '';
    var functionCalculate = '';

    // set parameter to cookie
    $.cookie("fileName", fileImportActual, { expires: 1 });
    $.cookie("headerName", $("#HeaderName").val(), { expires: 1 });
    $.cookie("budgetTabId", $("#BudgetTabId").val(), { expires: 1 });
    $.cookie("sheetIndex", sheetname, { expires: 1 });
    $.cookie("firstValue", firstValue, { expires: 1 });
    $.cookie("secondValue", secondValue, { expires: 1 });
    $.cookie("functionCalculate", functionCalculate, { expires: 1 });
    $.cookie("RedirectPage", $("#RedirectPage").val(), { expires: 1 });

    window.location.href = "/Budget/Mapping?id=" + $("#BudgetId").val();
}

function onChangeFunction() {
    var calfunction = $("#Addfunction").val();
    var columnName = $("#firstValue").val();
    var addColumnName = $("#secondValue").val();

    // calculate merged value
    $('span[name^="mergedValue_"]').each(function (index, itemMerged) {
        var firstValue = 0, secondValue = 0;
        var columnArray = $("span[name='ColumnName_" + (index + 1) + "']");
        if (columnArray.length == 0) return;
        if (columnArray.length == 1) {
            firstValue = formatOtherNumber($(columnArray[0]).html());
            $(itemMerged).html($.formatCurrency(firstValue));
        }
        else {
            firstValue = formatOtherNumber($(columnArray[0]).html());
            secondValue = formatOtherNumber($(columnArray[1]).html());
            if (columnName == addColumnName) {
                warningPopup("Warning", "Please select another column. Same columns will not be calculated.");
                return;
            }
            calculateMergedValue(calfunction, itemMerged, firstValue, secondValue);
        }
    });
}

function calculateMergedValue(calfunction, itemMerged, firstValue, secondValue) {
    if (calfunction == "MUTIPLE") {
        $(itemMerged).html($.formatCurrency(firstValue * secondValue));
    }
    else if (calfunction == "DIVIDE") {
        $(itemMerged).html($.formatCurrency(secondValue == 0 ? 0 : firstValue / secondValue));
    }
    else if (calfunction == "SUM") {
        $(itemMerged).html($.formatCurrency(firstValue + secondValue));
    }
    else if (calfunction == "MINUS") {
        $(itemMerged).html($.formatCurrency(firstValue - secondValue));
    }
    else if (calfunction == "AVG") {
        $(itemMerged).html($.formatCurrency((firstValue + secondValue) / 2));
    }
    else if (calfunction == "MIN") {
        $(itemMerged).html($.formatCurrency(Math.min(firstValue, secondValue)));
    }
    else if (calfunction == "MAX") {
        $(itemMerged).html($.formatCurrency(Math.max(firstValue, secondValue)));
    }
    else if (calfunction == "") {
        $(itemMerged).html($.formatCurrency(0));
    }
}

function callFinishImportFile() {
    var budgetId = parseInt($("#BudgetId").val());
    var budgetTabId = parseInt($("#BudgetTabId").val());
    var headerName = $("#SelectTargetMonthsAndPeriodic").val();

    if ($("#SelectTargetMonthsAndPeriodic").val() == "") {
        warningPopup("Warning", "Please select a target month or periodic before click button finish!");
        return;
    }
    var firstValue = $("#firstValue").val();
    var secondValue = $("#secondValue").val();
    var functiondCalculate = $("#Addfunction").val();
    var listItem = [];
    if ((firstValue === "undefined" || firstValue == "") && (secondValue === "undefined" || secondValue == "")) {
        warningPopup("Warning", "Please select a column value before click button finish!");
        return;
    }
    if (secondValue != "") {
        if ((functiondCalculate == "" || functiondCalculate === "undefined") && firstValue != "") {
            warningPopup("Warning", "Please select a function calculate before click button finish!");
            return;
        }
    }

    var MyRows = $('table#tbexcel').find('tbody').find('tr');
    for (var i = 0; i < MyRows.length; i++) {
        var $dataRow = $(MyRows[i]);
        var mergedValue = $dataRow.find('[name="mergedValue_' + (i + 1) + '"]').text();
        var MappingBudgetModel =
        {
            MergedValue: mergedValue.indexOf('(') == 0 ? 0 - formatOtherNumber(mergedValue) : formatOtherNumber(mergedValue),
            Section: $("#BudgetSection" + (i + 1)).val(),
            CategorySettingId: $("#BudgetCategorySetting" + (i + 1)).val()
        };
        listItem.push(MappingBudgetModel);
    }
    $.ajax(
       {
           url: '/Budget/SaveMappingBudget',
           type: "POST",
           data: JSON.stringify({ id: budgetId, budgetTabId: budgetTabId, headerName: headerName, listItem: listItem }),
           dataType: "json",
           contentType: "application/json; charset=utf-8",
           async: false,
           success: function (response) {
               if (response.Status) {
                   warningPopup("Budget mapping", response.Message);
                   setTimeout(function () {
                       if ($("#RedirectPage").val() == "Dashboard") {
                           return window.location = "/Dashboard/Index";
                       }
                       else {
                           return window.location = "/Budget/ViewTagetAndSales?id=" + budgetId;
                       }
                   }, 2000);

               } else {
                   warningPopup("Warning", "Please select at least a Section and a corresponding Category.");
               }
           }
       });
}

function onChangeColumn() {
    var budgetTabId = parseInt($("#BudgetTabId").val());
    var headerName = $("#SelectTargetMonthsAndPeriodic").val();

    // set parameter to cookie
    $.cookie("fileName", $("#FileName").val(), { expires: 1 });
    $.cookie("headerName", $("#SelectTargetMonthsAndPeriodic").val(), { expires: 1 });
    $.cookie("budgetTabId", $("#BudgetTabId").val(), { expires: 1 });
    $.cookie("sheetIndex", $("#SheetIndex").val(), { expires: 1 });
    $.cookie("firstValue", $("#firstValue").val(), { expires: 1 });
    $.cookie("secondValue", $("#secondValue").val(), { expires: 1 });
    $.cookie("functionCalculate", $("#Addfunction").val(), { expires: 1 });
    $.cookie("RedirectPage", $("#RedirectPage").val(), { expires: 1 });

    window.location.href = "/Budget/Mapping?id=" + $("#BudgetId").val();
}

function onChangeTargetMonthsAndPeriodic() {
    var headerName = $("#SelectTargetMonthsAndPeriodic").val();
}

function returnEditBudget() {
    var id = parseInt($("#BudgetId").val());
    var redirectPage = $("#RedirectPage").val();
    if (redirectPage == "BudgetDetails") {
        return window.location = "/Budget/ViewTagetAndSales?id=" + id;
    } else if (redirectPage == "Dashboard") {
        return window.location = "/Dashboard/Index";
    }
}

function CallCanCel() {
    var id = parseInt($("#BudgetId").val());
    var budgetTabId = parseInt($("#BudgetTabId").val());
    var headerName = $("#HeaderName").val();
    return window.location = "/Budget/ImportActualNumber?id=" + id + "&budgetTabId=" + budgetTabId + "&headerName=" + headerName + "&redirectPage=" + $("#RedirectPage").val();
}

function callStartSync() {
    // Sync SSP DB
}

function formatOtherNumber(value) {
    return (value.indexOf('(') != -1) ? 0 - Number(value.replace(/[^0-9\.]+/g, "")) : Number(value.replace(/[^0-9\.]+/g, ""));
}

function budgetCategorySettingChange(e) {
    var currentItem = this;
    var rowIndex = 0;
    $('input[id^="BudgetCategorySetting"]').each(function (i, item) {
        if ($(item).attr('id') != $(currentItem.element).attr('id') && $(item).val() == $(currentItem.element).val()) {
            rowIndex = i + 1;
        }
    });

    if (rowIndex != 0) {
        warningPopup('Warning', 'The No.' + rowIndex + ' has Category is duplicated.');
    }
}

// check box uncheck box sales
function setFlagHeaderCheckBox(item) {
    var className = $(item).attr('id');
    $('.' + className).prop('checked', $(item).prop('checked'));
}

function setFlagHeaderChildrenCheckBox(item) {
    // check all by section
    var unCheckFlag = [], checkedFlag = [];
    var className = $(item).attr('class');
    $('.' + className).each(function () {
        if ($(this).prop('checked')) {
            checkedFlag.push(this);
        } else {
            unCheckFlag.push(this);
        }
    });

    if (checkedFlag.length > 0 && unCheckFlag.length == 0) {
        $('#' + className).prop('checked', true);
    } else {
        $('#' + className).prop('checked', false);
    }
}

//sync data budget sales and cogs from SSP
var SyncBudgetActualFromSSP = function () {
    // select months or period of budget sync data
    var tabIdArray = [];
    $('.scroll input[type="checkbox"][id*="tabIndex_"]').each(function () {
        var tabId = $(this).val();

        var data = []
        // get all checked children
        $('.' + this.id + ':checked').each(function () {
            data.push($(this).attr('row-index'));
        });

        if (data.length > 0) {
            tabIdArray.push({
                TabId: tabId,
                HeaderIndex: data,
            });
        }
    })

    // check if not exists checked column
    var budgetLengthType = parseInt($("#BudgetLengthType").val());
    if (tabIdArray.length == 0) {
        warningPopup("Warning", 'Please select at least a ' + (budgetLengthType == 1 ? 'month' : 'period') + ' before clicking start Import Actual number button!');
        return;
    }

    var restCode = $("#RestCode").val();
    var restName = $("#RestName").val();
    var options = {
        id: "ConfirmationSyncBudgetSalesAndCogs",
        typeStatus: true,
        title: "Confirmation",
        confirmText: "Are you sure to Import Actual number from " + restName + " (" + restCode + ")" + "?",
        textYes: "Yes",
        textNo: "No"
    }
    var popupWindow = window.setaJs.initPopupWindow(options);
    popupWindow.center().open();

    var $form = $('#' + options.id);
    $form.find("#act-accept-no").on("click", function () {
        if (popupWindow) popupWindow.close();
    });

    $form.find("#act-accept-yes").on("click", function () {
        if (popupWindow) popupWindow.close();

        // call action sync data ssp
        showProcessing(3000);
        setTimeout(function () {
            var budgetId = parseInt($("#BudgetId").val());
            $.ajax({
                url: '/budget/SyncBudgetActualFromSSP',
                type: "POST",
                dataType: "json",
                data: JSON.stringify({
                    id: budgetId,
                    headerOjectList: tabIdArray,
                    budgetType: budgetLengthType
                }),
                contentType: "application/json; charset=utf-8",
                async: false,
                success: function (response) {
                    if (response.Status) {
                        warningPopup("Information", response.Message, "/Budget/ViewTagetAndSales?id=" + budgetId);
                    }
                    else
                        warningPopup("Warning", response.Message);
                }
            });
        }, 500);
    });
}