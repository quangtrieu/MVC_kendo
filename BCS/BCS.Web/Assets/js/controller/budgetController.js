var counter = 1;
var reValue = "re-value";
$.itemDeleteList = [];
$.isTaxFlag = false;
$.isChangeDataInPage = false;
$.isCalculateProfitLoss = false;

function renderNumber(data) {
    return counter++;
}

function updateLinkRestCode(code) {
    var budgetId = $("#BudgetId").val();
    if (budgetId == 0) {
        warningPopup("Warning", "Cannot change link to your rest code, budget is empty.")
        return;
    }

    // call method update link rest code to budget.
    showProcessing();
    $.ajax({
        type: "POST",
        url: "/Budget/UpdateLinkRestCode",
        data: { restCode: code, budgetId: budgetId },
        success: function (data) {
            if (data.Status) {
                var url = window.location.pathname + "?id=" + $.urlParam('id');
                warningPopup("Information", data.Message, url);
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

function getCategoryIsTaxFlg() {
    return {
        id: parseFloat($("#CategorySettingId").val()),
        budgetId: parseFloat($('#BudgetId').val()),
        isTaxFlag: $.isTaxFlag,
    };
}

function editCategorySetting(id, name, isTax) {
    // set is tax flag
    $.isTaxFlag = isTax ? true : false;
    name += $.isTaxFlag ? " (IS TAX)" : "";
    var title = "Manage " + name + " Categories";
    var width = $(document).width() > 1000 ? 1000 : $(document).width() * 0.85;

    var popupWindow = setaJs.initPopupWindow({
        title: title,
        height: 800,
        width: width,
        id: "editCategorySetting"
    });

    popupWindow.refresh({
        url: "/Budget/ViewEditCategorySetting?id=" + id,
        async: false
    }).center().open();
    popupWindow.center().open();

    if ($.isTaxFlag) {
        var headerName = $('#editCategorySetting .category-header-name').children().html();
        $('#editCategorySetting .category-header-name').children().html("Manage Payroll Expenses (IS TAX) Categories")
    }

    var $form = $('#editCategorySetting');

    $form.find("#act-accept-no").on("click", function () {
        if (popupWindow) popupWindow.close();
    });
    $form.find("#act-accept-yes").on("click", function () {
        showProcessing();

        var grid = $("#CategorySettingModelGrid").data("kendoGrid");
        if ($form.find(".field-validation-error").length > 0) {
            var input = $form.find(".field-validation-error").prev();
            grid.select($(input).closest('tr'));
            var scrollTop = $(input).offset().top;
            $('.k-grid-content').scrollTop(scrollTop);
            $(input).focus();
            return;
        }

        var parentCategoryId = parseInt($("#CategorySettingId").val());
        var budgetId = parseInt($("#BudgetId").val());

        var index = 1;
        var listOfObjects = [];
        var checkDuplicateFlag = false;
        var categoryNameDuplicate = "", messageCheck = "";
        for (var item in grid.dataSource._data) {
            if (grid.dataSource._data[item].CategorySettingId != null) {
                var categories = grid.dataSource._data[item];
                // parent category id is new category
                if (categories.CategorySettingId == 0) {
                    categories.ParentCategoryId = parentCategoryId;
                    categories.BudgetId = budgetId;
                }

                if (categories.CategoryName == null || $.trim(categories.CategoryName).length == 0) {
                    $($($("#CategorySettingModelGrid tr")[index]).find('td')[1]).click();
                    $($($("#CategorySettingModelGrid tr")[index]).find('input[name="CategoryName"]')).blur();
                    return;
                }

                // set new sort order by index
                categories.SortOrder = index++;

                var categorySetting = {
                    CategorySettingId: categories.CategorySettingId,
                    BudgetId: categories.BudgetId,
                    ParentCategoryId: categories.ParentCategoryId,
                    SalesCategoryRefId: categories.SalesCategoryRefId,
                    CategoryName: categories.CategoryName,
                    SortOrder: categories.SortOrder,
                    IsSelected: categories.IsSelected,
                    IsPrimeCost: categories.IsPrimeCost,
                    IsTaxCost: $.isTaxFlag,
                    IsPercentage: categories.IsPercentage,
                    DeletedFlg: categories.DeletedFlg,
                    CreatedUserId: categories.CreatedUserId,
                    CreatedDate: categories.CreatedDate
                };

                // check category name is duplicate in db
                if (categoryNameDuplicate.indexOf("[" + categorySetting.CategoryName.toLowerCase() + "];") != -1) {
                    checkDuplicateFlag = true;
                    messageCheck = messageCheck + '<span class="error">This row number #' + categorySetting.SortOrder + ' category name "' + categorySetting.CategoryName + '" is duplicate.</span><br/>';
                } else {
                    categoryNameDuplicate = categoryNameDuplicate + "[" + categorySetting.CategoryName.toLowerCase() + "];";

                    // add category setting to list
                    listOfObjects.push(categorySetting);
                }
            }
        }

        if (checkDuplicateFlag) {
            $(".categorySettingErrorMessage").empty().append(messageCheck);
            return;
        }

        $.merge(listOfObjects, $.itemDeleteList);

        // call action update/add categories.
        $.ajax(
        {
            url: '/Budget/SaveCategorySetting',
            type: "POST",
            data: JSON.stringify({ listObj: listOfObjects, budgetId: budgetId, parentCategoryName: name }),
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            async: false,
            success: function (response) {
                if (response.Status) {
                    window.location = window.location.pathname + "?id=" + $.urlParam("id");
                }
                else
                    warningPopup("Warning", "Save falied.");
            }
        });
    });
}

function openLoadCategoryTemplate() {
    var popupWindow = setaJs.initPopupWindow({
        title: "Load category from template",
        height: 800,
        width: 1000,
        id: "LoadCategoryTemplate"
    });

    popupWindow.refresh({
        url: "/Budget/ViewTemplateCategory",
        async: false
    }).center().open();

    var $form = $('#LoadCategoryTemplate');

    $form.find("#act-accept-no").on("click", function () {
        if (popupWindow) popupWindow.close();
    });
    $form.find("#act-accept-yes").on("click", function () {
        if (popupWindow) popupWindow.close();

        showProcessing(60000);

        // call action reset all categories setting default
        setTimeout(function () {
            var defaultSectionNameList = "Sales,COGS,Payroll Expenses,Operation Expenses,";
            var sectionList = [];
            var categoryBySection = [];
            var sectionName = "";
            var sortOrder = 0;
            var categorySize = $('#TemplateCategoryGrid tbody tr').length;
            $('#TemplateCategoryGrid tbody tr').each(function (i, item) {
                if ($(this).hasClass('k-grouping-row')) {
                    // and section name to section list
                    if (sectionName != "") {
                        sectionList.push({
                            SectionName: sectionName,
                            CategoryBySection: categoryBySection,
                        });
                        categoryBySection = [];
                    }

                    // add section name
                    sectionName = $(this).find('.sectionGroup').text();
                    sortOrder = 1;

                    // remove section name is exists
                    defaultSectionNameList = defaultSectionNameList.replace(sectionName + ',', "");
                } else {
                    categoryBySection.push({
                        CategoryName: $(this).find('td:nth(1)').text(),
                        SortOrder: sortOrder++,
                    });
                }

                // add section lasted row
                if (categorySize == i + 1) {
                    sectionList.push({
                        SectionName: sectionName,
                        CategoryBySection: categoryBySection,
                    });
                }
            });

            // check section is not exist gird
            $.each(defaultSectionNameList.split(','), function (i, sectionName) {
                if (sectionName != "") {
                    sectionList.push({
                        SectionName: sectionName,
                        CategoryBySection: [],
                    });
                }
            });

            $.ajax(
            {
                url: '/Budget/SaveCategorySettingByTemplate',
                type: "POST",
                data: JSON.stringify({ sectionList: sectionList, budgetId: $('#BudgetId').val() }),
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                async: false,
                success: function (response) {
                    hideProcessing();

                    if (response.Status)
                        warningPopup("Information", response.Message, window.location.pathname + "?id=" + $.urlParam("id"));
                    else
                        warningPopup("Warning", "Save falied.");
                }
            });
        }, 500);
    });
}

var loadDataFromTemplate = function (fileName) {
    //getting json data from kendo Grid
    $.ajax({
        type: "GET"
        , url: "/Budget/GetDataFromTemplate"
        , data: { fileName: JSON.stringify(fileName) }
        , success: function (result) {
            if (result.Data.length == 0) {
                return;
            } else {
                $.each(result.Data, function (i, item) {
                    var sectionName = $.trim(item.Section);
                    if (sectionName == "Sales") {
                        item.Section = "<span class='hide'>0001</span><span class='sectionGroup'>Sales</span>";
                    } else if (sectionName == "COGS") {
                        item.Section = "<span class='hide'>0002</span><span class='sectionGroup'>COGS</span>";
                    } else if (sectionName == "Payroll Expense" || sectionName == "Payroll Expenses") {
                        item.Section = "<span class='hide'>0003</span><span class='sectionGroup'>Payroll Expenses</span>";
                    } else if (sectionName == "Payroll Expense (Tax)" || sectionName == "Payroll Expenses (Tax)") {
                        item.Section = "<span class='hide'>0004</span><span class='sectionGroup'>Payroll Expenses (Tax)</span>";
                    } else if (sectionName == "Operation Expense" || sectionName == "Operation Expenses") {
                        item.Section = "<span class='hide'>0005</span><span class='sectionGroup'>Operation Expenses</span>";
                    }
                });

                $("#TemplateCategoryGrid").empty().kendoGrid({
                    scrollable: true,
                    resizable: true,
                    reorderable: false,
                    sortable: true,
                    pageable: false,
                    dataSource: {
                        data: result.Data,
                        schema: {
                            model: {
                                fields: {
                                    Category_Name: { type: "string" },
                                    Account_Type: { type: "string" },
                                    Section: { type: "string" }
                                }
                            }
                        },
                        group: {
                            field: "Section", aggregates: [
                               { field: "Section", aggregate: "count" },
                            ],
                        },
                    },
                    columns: [
                        { field: "Category_Name", title: "Category Name" },
                        { field: "Account_Type", title: "Account Type", width: 250 },
                    ],
                    sort: { field: "Category_Name", dir: "desc" }
                });

                // set order by category name
                setTimeout(function () {
                    $('#TemplateCategoryGrid a:first').trigger('click');

                    // remove sort order flag in grid
                    $('span.k-i-arrow-n').remove();
                    $("#TemplateCategoryGrid .k-link").on('click', function () {
                        return false;
                    });
                })
            }
        }
    });
}

function addNewCategories() {
    var grid = $("#CategorySettingModelGrid").data("kendoGrid");
    $.each(grid.dataSource._data, function (i, categories) {
        if (categories.CategoryName == null || $.trim(categories.CategoryName).length == 0) {
            grid.select($("#CategorySettingModelGrid tr:nth(" + (i + 1) + ")"));
            grid.editCell($("#CategorySettingModelGrid tr:nth(" + (i + 1) + ") td:nth(1)"));
            $("#CategorySettingModelGrid tr:nth(" + (i + 1) + ") td:nth(1)").focus();
            return;
        }
    });

    counter = 1;
    grid.addRow();

    // set selected is percentage checked default.
    var row = $("#CategorySettingModelGrid").find('tr:last');
    var dataItemThis = grid.dataItem(row);
    dataItemThis.IsPercentage = true;
    $(row).find('input[type="checkbox"][name="IsPercentage"]').attr("checked", "checked");

    // set selected is prime cost
    if (grid.columns[3].title == "Is Prime Cost") {
        dataItemThis.IsPrimeCost = true;
        $(row).find('input[type="checkbox"][name="IsPrimeCost"]').attr("checked", "checked");
    }
}

function deleteCategoriesById(item) {
    // get a reference to the grid widget
    var grid = $("#CategorySettingModelGrid").data("kendoGrid");

    // returns the data item for row
    var row = $(item).closest('tr');
    var dataItemThis = grid.dataItem(row);

    var options = {
        id: "ConfirmationDeleteCategoryItem",
        typeStatus: true,
        title: "Confirmation",
        confirmText: "Are you sure you want to delete category " + encodeHtmlEntity(dataItemThis.CategoryName) + "?",
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
        if (dataItemThis.CategorySettingId > 0) {
            // set item to delete list
            dataItemThis.DeletedFlg = true;
            $.itemDeleteList.push(dataItemThis);
        }

        // remove this data item in grid.
        grid.dataSource.remove(dataItemThis);

        // reset index
        counter = 1;
        grid.refresh();

        if (popupWindow) popupWindow.close();
    });
}

function resetDefaultCategory() {
    var options = {
        id: "ConfirmationRestoreCategory",
        typeStatus: true,
        title: "Confirmation",
        confirmText: "Are you sure to restore Categories Settings to Default Settings?<br/>NOTE: By clicking Yes button, all current data will be lost.",
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

        // call action restore categories.
        $.ajax(
        {
            url: '/Budget/RestoreCategories',
            type: "GET",
            dataType: "json",
            data: { budgetId: $("#BudgetId").val() },
            success: function (response) {
                if (response.Status)
                    window.location = '/Budget/CategorySetting?id=' + $("#BudgetId").val();
                else
                    warningPopup("Warning", response.Message);
            }
        });
    });
}

// process in screen budget length
function callSaveBudgetLength() {
    $('.validation-item-BudgetLengthStart').empty();
    $('.validation-item-FiscalYearStartOn').empty();

    var validationFlag = true;
    var startDate = $.datepicker.formatDate('dd M yy', $("#BudgetLengthStart").data('kendoDatePicker').value());
    if (startDate.length == 0) {
        $('.validation-item-BudgetLengthStart').html("The Start field is required.");
        validationFlag = false;
    }

    var fiscalYearStartOn;
    if ($("#FiscalYearStartOn").length != 0) {
        fiscalYearStartOn = $.datepicker.formatDate('dd M yy', $("#FiscalYearStartOn").data('kendoDatePicker').value());
        if (fiscalYearStartOn.length == 0) {
            $('.validation-item-FiscalYearStartOn').html("The Fiscal Year Start On field is required.");
            validationFlag = false;
        }
    } else {
        fiscalYearStartOn = startDate;
    }

    // validation data incorrec
    if (validationFlag == false) return;

    showProcessing();

    // call save change budget
    SaveChangeBudget(false);
}

function SaveChangeBudget(confirm) {
    var budgetId = parseInt($("#BudgetId").val());
    var budgetType = parseFloat($("#BudgetLengthType").val());
    var budgetMonthLenth = parseFloat($("#BudgetMonthLenth").val());
    var startDate = $.datepicker.formatDate('dd M yy', $("#BudgetLengthStart").data('kendoDatePicker').value());
    var fiscalYearStartOn;
    if ($("#FiscalYearStartOn").length != 0) {
        fiscalYearStartOn = $.datepicker.formatDate('dd M yy', $("#FiscalYearStartOn").data('kendoDatePicker').value());
    } else {
        fiscalYearStartOn = startDate;
    }
    $.ajax({
        url: '/Budget/SaveChangeBudget',
        type: "POST",
        data: JSON.stringify({ budgetId: budgetId, budgetType: budgetType, startDate: startDate, fiscalYearStartOn: fiscalYearStartOn, budgetMonthLenth: budgetMonthLenth, confirmDelete: confirm }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        async: false,
        success: function (result) {
            if (result.Status) {
                warningPopup("Budget Length", result.Message);
                return;
            }

            if (result.Confirm) {
                // open confirm message
                var options = {
                    id: "ConfirmationSaveChangeBudget",
                    typeStatus: true,
                    title: "Confirmation",
                    confirmText: result.Message,
                    textYes: "Yes",
                    textNo: "No"
                }
                var popupWindow = window.setaJs.initPopupWindow(options);
                popupWindow.refresh({}).center().open();
                popupWindow.center().open();

                var $form = $('#' + options.id);
                $form.find("#act-accept-no").on("click", function () {
                    if (popupWindow) popupWindow.close();
                    // reload page
                    window.location = window.location.pathname + "?id=" + $.urlParam("id");
                });

                $form.find("#act-accept-yes").on("click", function () {
                    if (popupWindow) popupWindow.close();

                    // call back to method change
                    SaveChangeBudget(true);
                });
            }
            else {
                if (result.Status) {
                    window.location = window.location.pathname + "?id=" + $.urlParam("id");
                } else {
                    warningPopup("Warning", result.Message, window.location.pathname + "?id=" + $.urlParam("id"));
                }
            }
        },
        error: function () {
            warningPopup("Warning", "Process error, Please contact system admin!");
        }
    });
}

function changeBudgetLengthType(data, confirm) {
    showProcessing();
    $.ajax({
        url: "/Budget/ChangeBudgetType",
        type: "POST",
        async: false,
        data: { budgetId: parseInt($("#BudgetId").val()), budgetType: data, confirmDelete: confirm },
        success: function (result) {
            if (result.Confirm) {
                // open confirm message
                var options = {
                    id: "ConfirmationChangeBudgetLengthType",
                    typeStatus: true,
                    title: "Confirmation",
                    confirmText: result.Message,
                    textYes: "Yes",
                    textNo: "No"
                }
                var popupWindow = window.setaJs.initPopupWindow(options);
                popupWindow.refresh({}).center().open();
                popupWindow.center().open();

                var $form = $('#' + options.id);
                $form.find("#act-accept-no").on("click", function () {
                    if (popupWindow) popupWindow.close();
                    // reload page
                    window.location = window.location.pathname + "?id=" + $.urlParam("id");
                });

                $form.find("#act-accept-yes").on("click", function () {
                    if (popupWindow) popupWindow.close();

                    // call back to method change
                    changeBudgetLengthType(data, true);
                });
            }
            else {
                if (result.Status) {
                    window.location = window.location.pathname + "?id=" + $.urlParam("id");
                } else {
                    warningPopup("Warning", result.Message, window.location.pathname + "?id=" + $.urlParam("id"));
                }
            }
        },
        error: function () {
            warningPopup("Warning", "Process error, Please contact system admin!");
        }
    });
}

//Process Input Method by bugget
function callSaveInputMethod() {
    showProcessing();
    var BudgetId = parseInt($("#BudgetId").val());
    var InputMethod = $("#InputMethod").val();
    var ActualNumbersFlg = $('input[type="checkbox"][name="ActualNumbersFlg"]:checked').length > 0 ? true : false;
    var VarianceFlg = $('input[type="checkbox"][name="VarianceFlg"]:checked').length > 0 ? true : false;
    $.ajax(
       {
           url: '/Budget/SaveInputMethodBudget',
           type: "POST",
           data: JSON.stringify({ budgetId: BudgetId, inputMethod: InputMethod, actualNumbersFlg: ActualNumbersFlg, varianceFlg: VarianceFlg }),
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

// function set focus item by format
$.nextScrollTop = 0;
$.nextScrollLeft = 0;
function setFocusOutItem(e) {
    if ($(this).hasAttr('readonly') && (this.id.indexOf('Total__Projection') == -1 || this.id.indexOf('Total__Projection') != 0)) return;

    $(this).val($(this).attr('re-value'));
    $(this).select();

    $(this).unbind('blur');
    $(this).on('blur', function () {
        if ($(this).hasClass('bcs-currency-textbox')) {
            $(this).val($.formatCurrency($(this).attr('re-value')));
        } else {
            $(this).val($.formatPercent($(this).attr('re-value')));
        }
    });

    //var budgetWidth = $('.budget-area:first .budget-item-container').width();
    //var container = $(".budget-item-container");
    //var scrollTo = $(this);
    //var stLeft = scrollTo.offset().left;
    //console.log(scrollTo.offset().left, container.offset().left, container.scrollLeft());
    //if (Math.round(stLeft / budgetWidth) != $.nextScrollLeft) {
    //    $.nextScrollLeft = Math.round(stLeft / budgetWidth);
    //    container.stop().animate({
    //        scrollLeft: scrollTo.offset().left - container.offset().left + container.scrollLeft() - 111,
    //    }, 500);
    //}

    //var ot = parseInt($(this).offset().top),
    //    pt = parseInt($(this).position().top);
    //var budgetHeight = $('.budget-area:first').height();
    //var st = ot - budgetHeight;//108 is height of up div
    //if (st - pt > 0) {
    //    st = st - pt;
    //}
    //if (st < 0) { st = ot - budgetHeight }
    ////console.log(st, ot, pt);
    //if (Math.round(pt / budgetHeight) != $.nextScrollTop) {
    //    $.nextScrollTop = Math.round(pt / budgetHeight);
    //    $('.budget-area:first').stop().animate({
    //        scrollTop: st + $('.budget-area:first').scrollTop()
    //    }, 500);
    //}

    $(this).keypress(function (e) {
        //if the letter is not digit then display error and don't type anything
        if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
            if ((e.which == 46 && this.value.indexOf('.') == -1) || (e.which == 45)) return true;
            return false;
        }
    });

    // set action key next input item
    $(this).keydown(function (event) {
        if (event.keyCode == 13 && event.shiftKey) {
            arrowUp(this.id);
            return false;
        } else if (event.keyCode == 9 && event.shiftKey) {
            arrowLeft(this.id);
            return false;
        } else if (event.keyCode == 13) {// enter
            arrowDown(this.id);
            return false;
        } else if (event.keyCode == 37) {// arrow left
            arrowLeft(this.id);
            return false;
        } else if (event.keyCode == 38) {// arrow up
            arrowUp(this.id);
            return false;
        } else if (event.keyCode == 39) {   // arrow right
            arrowRight(this.id);
            return false;
        } else if (event.keyCode == 40) {  // arrow down
            arrowDown(this.id);
            return false;
        } else if (event.keyCode == 9) {  // arrow tab
            arrowRight(this.id);
            return false;
        }
    });
}

// arrow left
function arrowLeft(inputName) {
    if (inputName == undefined) return;

    var nameArray = inputName.split('_');
    var tabIndex = nameArray[nameArray.length - 3];
    var colIndex = $.formatNumber(nameArray[nameArray.length - 2]);
    var rowIndex = $.formatNumber(nameArray[nameArray.length - 1]);

    var nextItem = null;
    if (inputName.indexOf("Target") != -1) {
        nextItem = $('#' + inputName.substring(0, inputName.lastIndexOf('_')) + '_' + (rowIndex - 1));
        if (nextItem.length == 0) {
            // back to annual sales item
            nextItem = $("#Annual_Sales_" + colIndex);
        }
    } else if (inputName.indexOf('Projection') != -1) {
        if ($('#ActualNumbersFlg').prop('checked')) {
            // back item focus to actual sales
            var sectionName = $('#' + inputName).closest('.data-row-by-category').attr('section-name');
            nextItem = $("#" + sectionName + "ActualSales_" + tabIndex + "_" + (colIndex - 1) + "_" + rowIndex);
        } else {
            // back item focus to projection
            var sectionName = $('#' + inputName).closest('.data-row-by-category').attr('section-name');
            nextItem = $("#" + sectionName + "ProjectionSales_" + tabIndex + "_" + (colIndex - 1) + "_" + rowIndex);
        }

        // prev item and prev section
        if (nextItem.length == 0) {
            var nextSection = getNextSectionByName(sectionName, -1);
            nextItem = $('[id^="' + nextSection + 'ProjectionSales_' + tabIndex + '_"]:last');
        }

        if ($(nextItem).hasAttr('readonly')) {
            nextItem = $(nextItem).next();
        }
    } else {
        // case input item is actual sales
        var sectionName = $('#' + inputName).closest('.data-row-by-category').attr('section-name');
        nextItem = $("#" + sectionName + "ProjectionSales_" + tabIndex + "_" + colIndex + "_" + rowIndex);

        // prev item and prev section
        if (nextItem.length == 0) {
            var nextSection = getNextSectionByName(sectionName, -1);
            nextItem = $('[id^="' + nextSection + 'ProjectionSales_' + tabIndex + '_"]:last');
        }

        if ($(nextItem).hasAttr('readonly')) {
            nextItem = $(nextItem).next();
        }
    }

    if (nextItem.length > 0) {
        $(nextItem).select();
    } else {
        var lastItem = $('#' + inputName).closest('.data-row-by-category').prev().find('.numerictextbox:last');
        if ($('#ActualNumbersFlg').prop('checked')) {
            $(lastItem).select();
        } else {
            if ($(lastItem).attr('id') == undefined) {
                arrowUp(inputName);
            } else {
                arrowLeft($(lastItem).attr('id'));
            }
        }
    }
}

// arrow up
function arrowUp(inputName) {
    if (inputName == undefined) return;

    var nameArray = inputName.split('_');
    var tabIndex = nameArray[nameArray.length - 3];
    var colIndex = $.formatNumber(nameArray[nameArray.length - 2]);
    var rowIndex = $.formatNumber(nameArray[nameArray.length - 1]);

    var nextItem = null;
    if (inputName.indexOf("Target") != -1) {
        nextItem = $('#Annual_Sales_' + colIndex);
    } else if (inputName.indexOf("Projection") != -1) {
        var sectionName = $('#' + inputName).closest('.data-row-by-category').attr('section-name');
        nextItem = $("#" + sectionName + "ProjectionSales_" + tabIndex + "_" + colIndex + "_" + (rowIndex - 1));

        if (nextItem.length == 0 && sectionName == "IsTax_Payroll_Expenses_") {
            nextItem = $('#' + inputName).closest('.data-row-by-category').prev().prev().find('[id^="Payroll_Expenses_ProjectionSales_' + tabIndex + '_' + colIndex + '_"]');
        }

        // prev item and prev section
        if (nextItem.length == 0) {
            var nextSection = getNextSectionByName(sectionName, -1);
            nextItem = $('[id^="' + nextSection + 'ProjectionSales_' + tabIndex + '_' + colIndex + '_"]:last');
        }

        if (nextItem.length == 0) {
            nextItem = $('#Target_Sales_' + tabIndex + '_' + colIndex);
        }

        if (nextItem.hasAttr('readonly')) {
            nextItem = $(nextItem).next();
        }
    } else {
        var sectionName = $('#' + inputName).closest('.data-row-by-category').attr('section-name');
        nextItem = $("#" + sectionName + "ActualSales_" + tabIndex + "_" + colIndex + "_" + (rowIndex - 1));

        if (nextItem.length == 0 && sectionName == "IsTax_Payroll_Expenses_") {
            nextItem = $('#' + inputName).closest('.data-row-by-category').prev().prev().find('[id^="Payroll_Expenses_ActualSales_' + tabIndex + '_' + colIndex + '_"]');
        }

        if (nextItem.length == 0) {
            var nextSection = getNextSectionByName(sectionName, -1);
            nextItem = $('[id^="' + nextSection + 'ActualSales_' + tabIndex + '_' + colIndex + '_"]:last');
        }

        if (nextItem.length == 0) {
            nextItem = $('#Target_Sales_' + tabIndex + '_' + colIndex);
            if (nextItem.hasAttr('readonly')) {
                nextItem = $(nextItem).next();
            }
        }
    }

    if (nextItem.length > 0) {
        $(nextItem).select();
    }
}

// arrow right
function arrowRight(inputName) {
    if (inputName == undefined) return;

    var nameArray = inputName.split('_');
    var tabIndex = nameArray[nameArray.length - 3];
    var colIndex = $.formatNumber(nameArray[nameArray.length - 2]);
    var rowIndex = $.formatNumber(nameArray[nameArray.length - 1]);

    var nextItem = null;
    if (inputName.indexOf("Annual_Sales_") != -1) {
        nextItem = $("#Target_Sales_" + rowIndex + "_0");
        if (nextItem.hasAttr('readonly')) {
            nextItem = $("#Target_Percent_" + rowIndex + "_0");
        }
    } else if (inputName.indexOf("Target") != -1) {
        nextItem = $('#' + inputName.substring(0, inputName.lastIndexOf('_')) + '_' + (rowIndex + 1));
        if (nextItem.length == 0) {
            // next row in section
            var sectionName = $("#SectionViewName").val() == "Sales" ? "" : $("#SectionViewName").val() + "_";
            nextItem = $("#" + sectionName + "ProjectionSales_" + colIndex + "_0_0");
            if (nextItem.hasAttr('readonly')) {
                nextItem = $("#" + sectionName + "ProjectionPercent_" + colIndex + "_0_0");
            }
        }
    } else if (inputName.indexOf('Projection') != -1) {
        if ($('#ActualNumbersFlg').prop('checked')) {
            // next item focus to actual sales
            var sectionName = $('#' + inputName).closest('.data-row-by-category').attr('section-name');
            nextItem = $("#" + sectionName + "ActualSales_" + tabIndex + "_" + colIndex + "_" + rowIndex);
        } else {
            // next item focus to projection
            var sectionName = $('#' + inputName).closest('.data-row-by-category').attr('section-name');
            nextItem = $("#" + sectionName + "ProjectionSales_" + tabIndex + "_" + (colIndex + 1) + "_" + rowIndex);
            if (nextItem.length == 0) {
                // next row
                nextItem = $("#" + sectionName + "ProjectionSales_" + tabIndex + "_" + 0 + "_" + (rowIndex + 1));
            }

            // next item in next section
            if (nextItem.length == 0) {
                var nextSection = getNextSectionByName(sectionName, 1);
                nextItem = $('#' + nextSection + 'ProjectionSales_' + tabIndex + '_' + 0 + '_' + 0);
            }

            if ($(nextItem).hasAttr('readonly')) {
                nextItem = $(nextItem).next();
            }
        }
    } else {
        // case input item is actual sales
        var sectionName = $('#' + inputName).closest('.data-row-by-category').attr('section-name');
        nextItem = $("#" + sectionName + "ProjectionSales_" + tabIndex + "_" + (colIndex + 1) + "_" + rowIndex);
        if (nextItem.length == 0) {
            // next row
            nextItem = $("#" + sectionName + "ProjectionSales_" + tabIndex + "_" + 0 + "_" + (rowIndex + 1));
        }

        // next item in next section
        if (nextItem.length == 0) {
            var nextSection = getNextSectionByName(sectionName, 1);
            nextItem = $('#' + nextSection + 'ProjectionSales_' + tabIndex + '_' + 0 + '_' + 0);
        }

        if ($(nextItem).hasAttr('readonly')) {
            nextItem = $(nextItem).next();
        }
    }

    if (nextItem.length > 0) {
        $(nextItem).select();
    }
}

// arrow down
function arrowDown(inputName) {
    if (inputName == undefined) return;

    var nameArray = inputName.split('_');
    var tabIndex = nameArray[nameArray.length - 3];
    var colIndex = $.formatNumber(nameArray[nameArray.length - 2]);
    var rowIndex = $.formatNumber(nameArray[nameArray.length - 1]);

    var $nextItem = null;
    if (inputName.indexOf("Annual_Sales") != -1) {
        $nextItem = $('#Target_Sales_' + rowIndex + '_0');
        if ($nextItem.hasAttr('readonly')) {
            $nextItem = $('#Target_Percent_' + rowIndex + '_0');
        }
        $nextItem.select();
        return;
    } else if (inputName.indexOf("Target") != -1) {
        var nextSection = getNextSectionByName(sectionName, 1);
        $nextItem = $("#" + nextSection + "ProjectionSales_" + colIndex + "_" + rowIndex + "_0");
        if ($nextItem.hasAttr('readonly')) {
            $nextItem = $("#" + nextSection + "ProjectionPercent_" + colIndex + "_" + rowIndex + "_0");
        }

        if ($nextItem.length == 0) {
            // set next focus next target item
            arrowRight('Target_' + tabIndex + '_' + colIndex + '_' + rowIndex);
        }
    } else if (inputName.indexOf('Projection') != -1) {
        var sectionName = $('#' + inputName).closest('.data-row-by-category').attr('section-name');
        $nextItem = $("#" + sectionName + "ProjectionSales_" + tabIndex + "_" + colIndex + "_" + (rowIndex + 1));

        if ($nextItem.length == 0) {
            var nextSection = getNextSectionByName(sectionName, 1);
            $nextItem = $('#' + nextSection + 'ProjectionSales_' + tabIndex + '_' + colIndex + '_' + 0);
        }

        if ($nextItem.hasAttr('readonly')) {
            $nextItem = $nextItem.next();
        }
    } else {
        var sectionName = $('#' + inputName).closest('.data-row-by-category').attr('section-name');
        $nextItem = $("#" + sectionName + "ActualSales_" + tabIndex + "_" + colIndex + "_" + (rowIndex + 1));

        if ($nextItem.length == 0) {
            var nextSection = getNextSectionByName(sectionName, 1);
            $nextItem = $('#' + nextSection + 'ActualSales_' + tabIndex + '_' + colIndex + '_' + 0);
        }

        if ($nextItem.hasAttr('readonly')) {
            $nextItem = $nextItem.next();
        }
    }

    if ($nextItem.length > 0) {
        $nextItem.select();
    } else {
        // set next focus column to target item
        $nextItem = $('#Target_Sales_' + tabIndex + '_' + colIndex);
        if ($nextItem.hasAttr('readonly')) {
            $nextItem = $nextItem.next();
        }
        arrowRight($nextItem.attr('id'));
    }
}

// function get next section name display block
function getNextSectionByName(name, nextOrBackIndex) {
    var sectionList = [];
    if ($('.active .Area_Category_Sales:first').css('display') == 'block') {
        sectionList.push("");
    }
    if ($('.active .Area_Category_COGS:first').css('display') == 'block') {
        sectionList.push("COGS_");
    }
    if ($('.active .Area_Category_Payroll_Expenses:first').css('display') == 'block') {
        sectionList.push("Payroll_Expenses_");
        sectionList.push("IsTax_Payroll_Expenses_");
    }
    if ($('.active .Area_Category_Operation_Expenses:first').css('display') == 'block') {
        sectionList.push("Operation_Expenses_");
    }
    var indexByName = sectionList.indexOf(name);

    if (indexByName + nextOrBackIndex >= 5)
        return null;

    return sectionList[indexByName + nextOrBackIndex];
}

// START: SECTION SALES
function onChangeProjectionSales(e) {
    if (e.value == "." || isNaN(e.value) || e.value.length == 0) {
        e.value = 0;
    }

    $(e).attr(reValue, e.value);
    SectionSales.reCalcProjectionItem(e.id);

    var nameArray = e.id.split("_");
    var tabIndex = nameArray[nameArray.length - 3];
    var colIndex = nameArray[nameArray.length - 2];

    // call reCalculate profit loss
    ProfitLoss.ReCalcProfitLossByColumn(tabIndex, colIndex);

    // set confirm redirect
    $.isChangeDataInPage = true;
}

function onChangeProjectionPercent(e) {
    if (e.value == "." || isNaN(e.value) || e.value.length == 0) {
        e.value = 0;
    }

    $(e).attr(reValue, e.value);
    SectionSales.reCalcProjectionItem(e.id);

    var nameArray = e.id.split("_");
    var tabIndex = nameArray[nameArray.length - 3];
    var colIndex = nameArray[nameArray.length - 2];

    // call reCalculate profit loss
    ProfitLoss.ReCalcProfitLossByColumn(tabIndex, colIndex);

    // set confirm redirect
    $.isChangeDataInPage = true;
}

function onChangeActualSales(e) {
    if (e.value == "." || isNaN(e.value) || e.value.length == 0) {
        e.value = 0;
    }

    $(e).attr(reValue, e.value);
    SectionSales.reCalcActualPercent(e.id);

    var nameArray = e.id.split("_");
    var tabIndex = nameArray[nameArray.length - 3];
    var colIndex = nameArray[nameArray.length - 2];

    // call reCalculate profit loss
    ProfitLoss.ReCalcProfitLossByColumn(tabIndex, colIndex);

    // set confirm redirect
    $.isChangeDataInPage = true;
}

var SectionSales = {
    reCalcProjectionItem: function (itemName) {
        var nameArray = itemName.split("_");
        var tabIndex = nameArray[nameArray.length - 3];
        var colIndex = nameArray[nameArray.length - 2];
        var rowIndex = nameArray[nameArray.length - 1];

        // target sales by column
        var targetSales = parseFloat($('#Target_Sales_' + tabIndex + '_' + colIndex).attr(reValue));

        var sales = 0, percent = 0;
        if (itemName.indexOf("Sales") != -1) {
            // projection sales data
            sales = parseFloat($('#' + itemName).attr(reValue));

            // reCalculate item projection percent
            percent = (targetSales == 0) ? 0 : sales * 100 / targetSales;
            $('#' + itemName.replace("Sales", "Percent")).val($.formatPercent(percent));
            $('#' + itemName.replace("Sales", "Percent")).attr(reValue, percent);
        } else {
            // projection sales data
            percent = parseFloat($('#' + itemName).attr(reValue));

            // reCalculate item projection sales
            sales = percent * targetSales / 100;
            itemName = itemName.replace("Percent", "Sales");
            $('#' + itemName).val($.formatCurrency(sales));
            $('#' + itemName).attr(reValue, sales);
        }

        // call calculate total column
        SectionSales.reCalcTotalProjection(itemName, targetSales, tabIndex, colIndex);
        SectionSales.reCalcGrandTotalProjection(itemName, tabIndex, rowIndex);

        // call calculate variance row
        SectionSales.reCalcVarianceItem(itemName);

        // call calculate total target
        SectionSales.reCalcTotalTarget(tabIndex, colIndex);

        // call calculate total sales
        SectionSales.reCalcTotalSales(tabIndex);

        // call reCalculate grand total projection percent in section Payroll Expenses by row
        $('[id^="Payroll_Expenses_ProjectionSales_' + tabIndex + '_' + colIndex + '_"]').each(function () {
            var currentItem = $(this);
            if ($(this).hasAttr('readonly')) {
                currentItem = currentItem.next();
            }

            if ($(currentItem).attr(reValue) != 0)
                SectionPayroll.reCalcPayrollProjectionItem($(currentItem).attr("id"));
        });
        $('[id^="IsTax_Payroll_Expenses_ProjectionSales_' + tabIndex + '_' + colIndex + '_"]').each(function () {
            var currentItem = $(this);
            if ($(this).hasAttr('readonly')) {
                currentItem = currentItem.next();
            }

            if ($(currentItem).attr(reValue) != 0)
                SectionPayroll.reCalcPayrollProjectionItem($(currentItem).attr("id"));
        });

        // call reCalculate grand total projection percent in section Operation Expenses by row
        $('[id^="Operation_Expenses_ProjectionSales_' + tabIndex + '_' + colIndex + '_"]').each(function () {
            var currentItem = $(this);
            if ($(this).hasAttr('readonly')) {
                currentItem = currentItem.next();
            }

            if ($(currentItem).attr(reValue) != 0)
                SectionOperation.reCalcOperationProjectionItem($(currentItem).attr("id"))
        });
    },
    reCalcTotalProjection: function (itemName, targetSales, tabIndex, colIndex) {
        var $SalesTotalColumnItem = $('#Total__ProjectionSales_' + tabIndex + '_' + colIndex);

        var salesTotal = 0;
        $('[id^="' + itemName.substring(0, itemName.lastIndexOf("_")) + '_"]').each(function () {
            salesTotal += parseFloat($(this).attr(reValue));
        });

        // set total sales by column
        $SalesTotalColumnItem.val($.formatCurrency(salesTotal));
        $SalesTotalColumnItem.attr(reValue, salesTotal);

        // set total percent by column
        var percentTotal = (targetSales == 0) ? 0 : salesTotal * 100 / targetSales;
        $SalesTotalColumnItem.next().val($.formatPercent(percentTotal));
        $SalesTotalColumnItem.next().attr(reValue, percentTotal);

        // call reCalculate actual percent
        SectionSales.reCalcTotalActualPercent('Total__ProjectionSales_' + tabIndex + '_' + colIndex);
    },

    reCalcActualPercent: function (itemName) {
        var nameArray = itemName.split("_");
        var tabIndex = nameArray[nameArray.length - 3];
        var colIndex = nameArray[nameArray.length - 2];
        var rowIndex = nameArray[nameArray.length - 1];

        var totalActual = 0;
        $('[id^="ActualSales_' + tabIndex + '_' + colIndex + '_"]').each(function () {
            totalActual += parseFloat($(this).attr(reValue));
        });

        // set actual percent by row
        $('[id^="ActualSales_' + tabIndex + '_' + colIndex + '_"]').each(function () {
            var actualSales = parseFloat($(this).attr(reValue));
            var actualPercent = (totalActual == 0) ? 0 : actualSales * 100 / totalActual;

            // next item actual percent
            $(this).next().val($.formatPercent(actualPercent));
            $(this).next().attr(reValue, actualPercent);
        });

        // call calculate variance row
        SectionSales.reCalcVarianceItem(itemName);

        // call calculate grand total row
        SectionSales.reCalcGrandTotalActual(itemName, tabIndex, rowIndex);

        // set total actual by column
        var $totalActualItemItem = $('#Total__ActualSales_' + tabIndex + '_' + colIndex);
        $totalActualItemItem.val($.formatCurrency(totalActual));
        $totalActualItemItem.attr(reValue, totalActual);

        // set total actual percent by column
        SectionSales.reCalcTotalActualPercent('Total__ActualSales_' + tabIndex + '_' + colIndex);

        // get sales category id by row
        var salesCategoryId = parseInt($('#' + itemName).closest('.data-row-by-category').attr('category-setting-id'));

        // get cogs category ref cogs
        $('.active .Area_Category_COGS:first .category-name-data').each(function () {
            var $item = $(this);
            var refId = parseInt($item.attr('sales-category-ref-id'));
            if (refId == 0 || refId == salesCategoryId) {
                var cogsRowIndex = $item.index();

                // call recalculate reference value COGS by row: Actual
                SectionCOGS.reCalcCogsActualPercent("COGS_ActualSales_" + tabIndex + '_' + colIndex + '_' + cogsRowIndex);

                // call recalculate reference value COGS by row: Budgeted
                var $itemCogs = $('#COGS_ProjectionSales_' + tabIndex + '_' + colIndex + '_' + cogsRowIndex);
                if ($itemCogs.hasAttr('readonly')) {
                    $itemCogs = $itemCogs.next();
                }
                SectionCOGS.reCalcCogsProjectionItem($itemCogs.attr('id'));
            }
        });

        // call recalculate data in section Payroll
        $('[id^="Payroll_Expenses_ActualSales_' + tabIndex + '_' + colIndex + '_"]').each(function (i, payrollItem) {
            if (parseFloat($(payrollItem).attr(reValue)) != 0)
            SectionPayroll.reCalcPayrollActualPercent(payrollItem.id);
        });

        // call recalculate data in section Operation
        $('[id^="Operation_Expenses_ActualSales_' + tabIndex + '_' + colIndex + '_"]').each(function (i, operationItem) {
            if (parseFloat($(operationItem).attr(reValue)) != 0)
                SectionOperation.reCalcOperationActualPercent(operationItem.id);
        });
    },
    reCalcTotalActualPercent: function (itemName) {
        var actualSales = 0, projectionSales = 0, actualPercentName = "";
        if (itemName.indexOf("Projection") != -1) {
            actualSales = parseFloat($('#' + itemName.replace("Projection", "Actual")).attr(reValue));
            projectionSales = parseFloat($('#' + itemName).attr(reValue));
            actualPercentName = itemName.replace("ProjectionSales", "ActualPercent");
        } else {
            actualSales = parseFloat($('#' + itemName).attr(reValue));
            projectionSales = parseFloat($('#' + itemName.replace("Actual", "Projection")).attr(reValue));
            actualPercentName = itemName.replace("ActualSales", "ActualPercent");
        }

        var actualPercent = (projectionSales == 0) ? 0 : actualSales * 100 / projectionSales;
        $('#' + actualPercentName).val($.formatPercent(actualPercent));
        $('#' + actualPercentName).attr(reValue, actualPercent);

        // call calculate total variance by column
        SectionSales.reCalcVarianceItem(itemName);
    },

    reCalcGrandTotalProjection: function (itemName, tabIndex, rowIndex) {
        var dataRow = $('#' + itemName).closest('.data-row-by-category');
        var projectionSales = 0;
        $(dataRow).find('[id*="ProjectionSales"]').each(function () {
            projectionSales += parseFloat($(this).attr(reValue));
        });

        // set to item grand total projection sales
        var $grandTotalRowProjectionSalesItem = $('#GrandTotalRow_ProjectionSales_' + tabIndex + '_' + rowIndex);
        $grandTotalRowProjectionSalesItem.val($.formatCurrency(projectionSales));
        $grandTotalRowProjectionSalesItem.attr(reValue, projectionSales);

        // call calculate actual percent
        var itemSalesName = 'GrandTotalRow_ProjectionSales_' + tabIndex + '_' + rowIndex;
        SectionSales.reCalcTotalActualPercent(itemSalesName);

        // set header grand total projection sales
        var projectionSalesTotal = 0;
        $('[id^="GrandTotalRow_ProjectionSales_' + tabIndex + '_"]').each(function () {
            projectionSalesTotal += parseFloat($(this).attr(reValue));
        });
        var $grandTotalProjectionSalesItem = $('#GrandTotal__ProjectionSales_' + tabIndex);
        $grandTotalProjectionSalesItem.val($.formatCurrency(projectionSalesTotal));
        $grandTotalProjectionSalesItem.attr(reValue, projectionSalesTotal);

        // set grand total projection percent
        $('[id^="GrandTotalRow_ProjectionSales_' + tabIndex + '_"]').each(function () {
            // get grand total row projection sales
            projectionSales = parseFloat($(this).attr(reValue));

            // next item grand total row projection percent
            var percent = (projectionSalesTotal == 0) ? 0 : (projectionSales * 100 / projectionSalesTotal);
            $(this).next().val($.formatPercent(percent));
            $(this).next().attr(reValue, percent);
        });

        // next item grand total projection percent
        $grandTotalProjectionSalesItem.next().val($.formatPercent(projectionSalesTotal != 0 ? 100 : 0));

        // call calculate grand total acutal percent
        SectionSales.reCalcTotalActualPercent('GrandTotal__ProjectionSales_' + tabIndex);
    },
    reCalcGrandTotalActual: function (itemName, tabIndex, rowIndex) {
        var dataRow = $('#' + itemName).closest('.data-row-by-category');
        var actualSales = 0;
        $(dataRow).find('[id*="ActualSales"]').each(function () {
            actualSales += parseFloat($(this).attr(reValue));
        });

        // set grand total actual sales
        var $actualSalesItem = $('#GrandTotalRow_ActualSales_' + tabIndex + '_' + rowIndex);
        $actualSalesItem.val($.formatCurrency(actualSales));
        $actualSalesItem.attr(reValue, actualSales);

        // call calculate actual percent
        var actualSalesItemName = 'GrandTotalRow_ActualSales_' + tabIndex + '_' + rowIndex;
        SectionSales.reCalcTotalActualPercent(actualSalesItemName);

        // set header grand total actual sales
        var actualSalesTotal = 0;
        $('[id^="GrandTotalRow_ActualSales_' + tabIndex + '_"]').each(function () {
            actualSalesTotal += parseFloat($(this).attr(reValue));
        });
        var grandTotalActualSalesName = 'GrandTotal__ActualSales_' + tabIndex;
        $('#' + grandTotalActualSalesName).val($.formatCurrency(actualSalesTotal));
        $('#' + grandTotalActualSalesName).attr(reValue, actualSalesTotal);

        // call calculate grand total acutal percent
        SectionSales.reCalcTotalActualPercent(grandTotalActualSalesName);
    },

    reCalcTotalTarget: function (tabIndex, colIndex) {
        // get anual sales by tab index
        var annualSales = parseFloat($('#Annual_Sales_' + tabIndex).attr(reValue));
        var targetSales = parseFloat($('#Target_Sales_' + tabIndex + "_" + colIndex).attr(reValue));
        var targetPercent = parseFloat($('#Target_Percent_' + tabIndex + "_" + colIndex).attr(reValue));

        // Total Projection Sales
        var totalProjectionSales = parseFloat($('#Total__ProjectionSales_' + tabIndex + "_" + colIndex).attr(reValue));

        // Target Sales Variance
        var targetSalesVariance = totalProjectionSales - targetSales;
        var $varianceTargetSalesItem = $('#Variance_Target_Sales_' + tabIndex + "_" + colIndex);
        $varianceTargetSalesItem.val($.formatCurrency(targetSalesVariance));
        $varianceTargetSalesItem.attr(reValue, targetSalesVariance);

        // Target Percent Variance
        var targetPercentVariance = (targetSales == 0) ? 0 : (targetSalesVariance * 100 / targetSales);
        var $varianceTargetPercent = $('#Variance_Target_Percent_' + tabIndex + "_" + colIndex);
        $varianceTargetPercent.val($.formatPercent(targetPercentVariance));
        $varianceTargetPercent.attr(reValue, targetPercentVariance);

        // set color to item variance sales and reference item variance percent & actual percent & actual sales
        $.setColorToItem($varianceTargetSalesItem, $varianceTargetPercent);

        // call calculate sum total sales target
        SectionSales.reCalcTotalSalesTarget(tabIndex);
    },
    reCalcTotalSalesTarget: function (tabIndex) {
        // set value to item total target sales
        var totalTargetSales = 0;
        $('[id^="Target_Sales_' + tabIndex + '_"]').each(function (e) {
            totalTargetSales += parseFloat($(this).attr(reValue));
        });

        var $totalSalesTargetItem = $('#TotalSalesTarget_' + tabIndex);
        $totalSalesTargetItem.val($.formatCurrency(totalTargetSales));
        $totalSalesTargetItem.attr(reValue, totalTargetSales);

        // get anual sales by tab index
        var annualSales = parseFloat($('#Annual_Sales_' + tabIndex).attr(reValue));

        // set value to item variance total target sales
        var totalTargetSalesVariance = totalTargetSales - annualSales;
        var $totalSalesTargetVarianceItem = $('#TotalSalesTargetVariance_' + tabIndex);
        $totalSalesTargetVarianceItem.val($.formatCurrency(totalTargetSalesVariance));
        $totalSalesTargetVarianceItem.attr(reValue, totalTargetSalesVariance)

        // set color to target sales variance
        $.setColorToItem($totalSalesTargetVarianceItem);
    },
    reCalcTotalSales: function (tabIndex) {
        // set value to item total sales
        var totalSales = 0;
        $('[id^="Total__ProjectionSales_' + tabIndex + '_"]').each(function (e) {
            totalSales += parseFloat($(this).attr(reValue));
        });
        $('#TotalSales_' + tabIndex).val($.formatCurrency(totalSales));

        // get anual sales by tab index
        var annualSales = parseFloat($('#Annual_Sales_' + tabIndex).attr(reValue));

        // set value to item total sales variance
        var totalSalesVariance = totalSales - annualSales;
        var $totalSalesVarianceItem = $('#TotalSalesVariance_' + tabIndex);
        $totalSalesVarianceItem.val($.formatCurrency(totalSalesVariance));
        $totalSalesVarianceItem.attr(reValue, totalSalesVariance)

        // set color to sales variance
        $.setColorToItem($totalSalesVarianceItem);
    },

    reCalcVarianceItem: function (itemName) {
        var actualSales = 0, projectionSales = 0, varianceItemName = '';
        if (itemName.indexOf("Projection") != -1) {
            actualSales = parseFloat($('#' + itemName.replace("Projection", "Actual")).attr(reValue));
            projectionSales = parseFloat($('#' + itemName).attr(reValue));
            varianceItemName = itemName.replace("ProjectionSales", "VarianceSales");
        } else {
            actualSales = parseFloat($('#' + itemName).attr(reValue));
            projectionSales = parseFloat($('#' + itemName.replace("Actual", "Projection")).attr(reValue));
            varianceItemName = itemName.replace("ActualSales", "VarianceSales");
        }

        // set variance sales to item
        var varianceSales = actualSales - projectionSales;
        var $varianceSales = $('#' + varianceItemName);
        $varianceSales.val($.formatCurrency(varianceSales));
        $varianceSales.attr(reValue, varianceSales);

        // set variance percent to item
        var variancePercent = (projectionSales == 0) ? 0 : (varianceSales * 100 / projectionSales);
        var $variancePercent = $('#' + varianceItemName.replace("Sales", "Percent"));
        $variancePercent.val($.formatPercent(variancePercent));
        $variancePercent.attr(reValue, variancePercent);

        // get actual percent item
        var $actualPercent = $('#' + varianceItemName.replace("VarianceSales", "ActualPercent"));

        // get actual sales item by total row
        var $actualSales = null;
        if (itemName.indexOf('Total__') != -1) {
            $actualSales = $('#' + varianceItemName.replace("VarianceSales", "ActualSales"));
        }

        // set color to item variance sales and reference item variance percent & actual percent & actual sales
        $.setColorToItem($varianceSales, $variancePercent, $actualPercent, $actualSales);
    },
};
//END: SECTION SALES

// START: SECTION COGS
function onChangeCogsProjectionSales(e) {
    if (e.value == "." || isNaN(e.value) || e.value.length == 0) {
        e.value = 0;
    }

    $(e).attr(reValue, e.value);
    SectionCOGS.reCalcCogsProjectionItem(e.id);

    var nameArray = e.id.split("_");
    var tabIndex = nameArray[nameArray.length - 3];
    var colIndex = nameArray[nameArray.length - 2];

    // call reCalculate profit loss
    ProfitLoss.ReCalcProfitLossByColumn(tabIndex, colIndex);

    // set confirm redirect
    $.isChangeDataInPage = true;
}

function onChangeCogsProjectionPercent(e) {
    if (e.value == "." || isNaN(e.value) || e.value.length == 0) {
        e.value = 0;
    }

    $(e).attr(reValue, e.value);
    SectionCOGS.reCalcCogsProjectionItem(e.id);

    var nameArray = e.id.split("_");
    var tabIndex = nameArray[nameArray.length - 3];
    var colIndex = nameArray[nameArray.length - 2];

    // call reCalculate profit loss
    ProfitLoss.ReCalcProfitLossByColumn(tabIndex, colIndex);

    // set confirm redirect
    $.isChangeDataInPage = true;
}

function onChangeCogsActualSales(e) {
    if (e.value == "." || isNaN(e.value) || e.value.length == 0) {
        e.value = 0;
    }
    $(e).attr(reValue, e.value);
    SectionCOGS.reCalcCogsActualPercent(e.id);

    var nameArray = e.id.split("_");
    var tabIndex = nameArray[nameArray.length - 3];
    var colIndex = nameArray[nameArray.length - 2];

    // call reCalculate profit loss
    ProfitLoss.ReCalcProfitLossByColumn(tabIndex, colIndex);

    // set confirm redirect
    $.isChangeDataInPage = true;
}

var SectionCOGS = {
    reCalcCogsProjectionItem: function (itemName) {
        var nameArray = itemName.split("_");
        var tabIndex = nameArray[nameArray.length - 3];
        var colIndex = nameArray[nameArray.length - 2];
        var rowIndex = nameArray[nameArray.length - 1];

        var actualSalesBySectionSales = 0;

        // get data row by category setting id
        var $currentItem = $('#' + itemName);
        var $dataRowByCategory = $currentItem.closest('.data-row-by-category');

        // get sales category ref cogs category
        var rowIndexBySales = 0;
        var salesCategoryRefId = $dataRowByCategory.attr('sales-category-ref-id');
        if (salesCategoryRefId > 0) {
            // get data row by category sales ref id
            var $dataRowByCategoryRef = $('#tabIndex_' + tabIndex + ' .data-row-by-category[category-setting-id="' + salesCategoryRefId + '"]');
            rowIndexBySales = $dataRowByCategoryRef.index();

            // actual sales by category ref
            actualSalesBySectionSales = parseFloat($('#ActualSales_' + tabIndex + '_' + colIndex + '_' + rowIndexBySales).attr(reValue));
        } else {
            // total actual sales by category ref
            actualSalesBySectionSales = parseFloat($('#Total__ActualSales_' + tabIndex + '_' + colIndex).attr(reValue));
        }

        var sales = 0, percent = 0;
        if (itemName.indexOf("Sales") != -1) {
            // projection sales data
            sales = parseFloat($('#' + itemName).attr(reValue));

            // reCalculate item projection percent
            percent = (actualSalesBySectionSales == 0) ? 0 : sales * 100 / actualSalesBySectionSales;
            $('#' + itemName.replace("Sales", "Percent")).val($.formatPercent(percent));
            $('#' + itemName.replace("Sales", "Percent")).attr(reValue, percent);
        } else {
            // projection sales data
            percent = parseFloat($('#' + itemName).attr(reValue));

            // reCalculate item projection sales
            sales = percent * actualSalesBySectionSales / 100;
            itemName = itemName.replace("Percent", "Sales");
            $('#' + itemName).val($.formatCurrency(sales));
            $('#' + itemName).attr(reValue, sales);
        }

        // call reCalculate actual percent
        SectionCOGS.reCalcCogsTotalActualPercent('COGS_Total__ProjectionSales_' + tabIndex + '_' + colIndex);

        // call calculate variance row
        SectionCOGS.reCalcCogsVarianceItem(itemName);

        // call calculate total column
        SectionCOGS.reCalcCogsTotalProjection(itemName, tabIndex, colIndex);

        // get grand total projection sales by section sales and row
        var totalBySectionSales = (salesCategoryRefId != 0) ? parseFloat($('#GrandTotalRow_ActualSales_' + tabIndex + '_' + rowIndexBySales).attr(reValue)) : parseFloat($('#GrandTotal__ActualSales_' + tabIndex).attr(reValue));
        SectionCOGS.reCalcCogsGrandTotalProjection(itemName, tabIndex, totalBySectionSales);
    },
    reCalcCogsTotalProjection: function (itemName, tabIndex, colIndex) {
        // total actual of sales section
        var totalActualSalesOfSales = parseFloat($('#Total__ActualSales_' + tabIndex + '_' + colIndex).attr(reValue));

        // sum all projection sales
        var salesTotal = 0;
        $('[id^="COGS_ProjectionSales_' + tabIndex + '_' + colIndex + '_"]').each(function () {
            salesTotal += parseFloat($(this).attr(reValue));
        });

        // set total sales by column
        var $salesTotalColumnItem = $('#COGS_Total__ProjectionSales_' + tabIndex + '_' + colIndex);
        $salesTotalColumnItem.val($.formatCurrency(salesTotal));
        $salesTotalColumnItem.attr(reValue, salesTotal);

        // set total percent by column
        var percentTotal = (totalActualSalesOfSales == 0) ? 0 : salesTotal * 100 / totalActualSalesOfSales;
        var $percentTotalColumnItem = $('#COGS_Total__ProjectionPercent_' + tabIndex + '_' + colIndex);
        $percentTotalColumnItem.val($.formatPercent(percentTotal));
        $percentTotalColumnItem.attr(reValue, percentTotal);

        // call reCalculate actual percent
        SectionCOGS.reCalcCogsTotalActualPercent('COGS_Total__ProjectionSales_' + tabIndex + '_' + colIndex);

        //// call calculate variance row
        //SectionCOGS.reCalcCogsVarianceItem(itemName);
    },

    reCalcCogsActualPercent: function (itemName) {
        var nameArray = itemName.split("_");
        var tabIndex = nameArray[nameArray.length - 3];
        var colIndex = nameArray[nameArray.length - 2];
        var rowIndex = nameArray[nameArray.length - 1];

        // actual sales input
        var $actualSalesItem = $("#" + itemName);
        var actualSales = parseFloat($actualSalesItem.attr(reValue));

        // get data row by category
        var $dataRowByCateogry = $actualSalesItem.closest('.data-row-by-category');

        // get actual sales by category and column
        var actualSalesBySectionSales = 0;

        // get current category setting id
        var cogsCategoryId = $dataRowByCateogry.attr('category-setting-id');

        // get sales category ref id
        var rowIndexBySales = 0;
        var salesCategoryRefId = $dataRowByCateogry.attr('sales-category-ref-id');
        if (salesCategoryRefId > 0) {
            // get data row by category sales ref id
            var $dataRowByCategoryRef = $('#tabIndex_' + tabIndex + ' .data-row-by-category[category-setting-id="' + salesCategoryRefId + '"]');
            rowIndexBySales = $dataRowByCategoryRef.index();

            // actual sales by category ref
            actualSalesBySectionSales = parseFloat($('#ActualSales_' + tabIndex + '_' + colIndex + '_' + rowIndexBySales).attr(reValue));
        } else {
            // actual sales by category ref
            actualSalesBySectionSales = parseFloat($('#Total__ActualSales_' + tabIndex + '_' + colIndex).attr(reValue));
        }

        // set actual percent by row
        var percent = (actualSalesBySectionSales == 0) ? 0 : (actualSales * 100 / actualSalesBySectionSales);
        $actualSalesItem.next().val($.formatPercent(percent));
        $actualSalesItem.next().attr(reValue, percent);

        // call calculate variance row
        SectionCOGS.reCalcCogsVarianceItem(itemName);

        // get total actual sales by column
        var totalActual = 0;
        $('[id^="' + itemName.substring(0, itemName.lastIndexOf("_")) + '_"]').each(function () {
            totalActual += parseFloat($(this).attr(reValue));
        });

        // set total actual by column
        var totalActualSalesName = itemName.substring(0, itemName.length - 2).replace("ActualSales", "Total__ActualSales");
        $('#' + totalActualSalesName).val($.formatCurrency(totalActual));
        $('#' + totalActualSalesName).attr(reValue, totalActual);

        // set total actual percent by column
        SectionCOGS.reCalcCogsTotalActualPercent(totalActualSalesName);

        // call calculate grand total row
        SectionCOGS.reCalcCogsGrandTotalActual(itemName, tabIndex, colIndex, rowIndex);
    },
    reCalcCogsTotalActualPercent: function (itemName) {
        var actualSales = 0, actualSalesBySectionSales = 0, actualPercentName = "";
        if (itemName.indexOf("Projection") != -1) {
            actualSales = parseFloat($('#' + itemName.replace("Projection", "Actual")).attr(reValue));
            actualSalesBySectionSales = parseFloat($('#' + itemName.replace("Projection", "Actual").replace("COGS_", "")).attr(reValue));
            actualPercentName = itemName.replace("ProjectionSales", "ActualPercent");
        } else {
            actualSales = parseFloat($('#' + itemName).attr(reValue));
            actualSalesBySectionSales = parseFloat($('#' + itemName.replace("COGS_", "")).attr(reValue));
            actualPercentName = itemName.replace("ActualSales", "ActualPercent");
        }

        var actualPercent = (actualSalesBySectionSales == 0) ? 0 : actualSales * 100 / actualSalesBySectionSales;
        $('#' + actualPercentName).val($.formatPercent(actualPercent));
        $('#' + actualPercentName).attr(reValue, actualPercent);

        // call calculate total variance by column
        SectionCOGS.reCalcCogsVarianceItem(itemName);
    },

    reCalcCogsGrandTotalProjection: function (itemName, tabIndex, totalBySectionSales) {
        var dataRow = $('#' + itemName).closest('.data-row-by-category');
        var categorySettingId = $(dataRow).attr("category-setting-id");
        var projectionSales = 0;
        $(dataRow).find('[id*="ProjectionSales"]').each(function () {
            projectionSales += parseFloat($(this).attr(reValue));
        });

        // set to item grand total projection sales
        var $currentDiv = $(dataRow).closest('.tab-area-by-section');
        var $grandTotalProjectionSalesItem = $currentDiv.find('.grand-total-category-by-row input[category-setting-id="' + categorySettingId + '"]');
        $grandTotalProjectionSalesItem.val($.formatCurrency(projectionSales));
        $grandTotalProjectionSalesItem.attr(reValue, projectionSales);

        // set grand total projection percent by row
        var projectionPercent = (totalBySectionSales == 0) ? 0 : (projectionSales * 100 / totalBySectionSales);
        $grandTotalProjectionSalesItem.next().val($.formatPercent(projectionPercent));
        $grandTotalProjectionSalesItem.next().attr(reValue, projectionPercent);

        // call calculate actual percent
        var itemSalesName = $grandTotalProjectionSalesItem.attr("id");
        SectionCOGS.reCalcCogsTotalActualPercent(itemSalesName);

        // call reCalculate variance grand total by row
        SectionCOGS.reCalcCogsVarianceItem(itemSalesName);

        // set header grand total projection sales
        var projectionSalesTotal = 0;
        $('[id^="' + itemSalesName.substring(0, itemSalesName.lastIndexOf('_')) + '_"]').each(function () {
            projectionSalesTotal += parseFloat($(this).attr(reValue));
        });
        var $projectionSalesTotalItem = $('#' + itemSalesName.substring(0, itemSalesName.lastIndexOf('_')).replace("Row", "_"));
        $projectionSalesTotalItem.val($.formatCurrency(projectionSalesTotal));
        $projectionSalesTotalItem.attr(reValue, projectionSalesTotal);

        // set header grand total projection percent
        totalBySectionSales = parseFloat($('#GrandTotal__ActualSales_' + tabIndex).attr(reValue));
        projectionPercent = (totalBySectionSales == 0) ? 0 : (projectionSalesTotal * 100 / totalBySectionSales);
        $projectionSalesTotalItem.next().val($.formatPercent(projectionPercent));
        $projectionSalesTotalItem.next().attr(reValue, projectionPercent);

        // call reCalculate variance grand total by header
        SectionCOGS.reCalcCogsVarianceItem($projectionSalesTotalItem.attr("id"));
    },
    reCalcCogsGrandTotalActual: function (itemName, tabIndex, colIndex, rowIndex) {
        var $dataRow = $('#' + itemName).closest('.data-row-by-category');
        var categorySettingId = $dataRow.attr("category-setting-id");
        var actualSales = 0;
        $dataRow.find('[id*="ActualSales"]').each(function () {
            actualSales += parseFloat($(this).attr(reValue));
        });

        // get item grand total actual by row
        var $actualSalesItem = $('#COGS_GrandTotalRow_ActualSales_' + tabIndex + "_" + rowIndex);
        $actualSalesItem.val($.formatCurrency(actualSales));
        $actualSalesItem.attr(reValue, actualSales);

        // get sales category id ref
        var actualSalesBySectionSales = 0;
        var salesCategoryRefId = $dataRow.attr('sales-category-ref-id');
        if (salesCategoryRefId > 0) {
            // get data row by category sales ref id
            var $dataRowByCategoryRef = $('#tabIndex_' + tabIndex + ' .data-row-by-category[category-setting-id="' + salesCategoryRefId + '"]');

            // actual sales by category ref
            actualSalesBySectionSales = parseFloat($('#GrandTotalRow_ActualSales_' + tabIndex + '_' + $dataRowByCategoryRef.index()).attr(reValue));
        } else {
            // total actual sales by category ref
            actualSalesBySectionSales = parseFloat($('#GrandTotal__ActualSales_' + tabIndex).attr(reValue));
        }

        // call calculate actual percent
        var actualPercent = (actualSalesBySectionSales == 0) ? 0 : (actualSales * 100 / actualSalesBySectionSales);
        var $actualPercentItem = $('#COGS_GrandTotalRow_ActualPercent_' + tabIndex + "_" + rowIndex);
        $actualPercentItem.val($.formatPercent(actualPercent));
        $actualPercentItem.attr(reValue, actualPercent);

        // call reCalculate variance grand total by row
        var actualSalesItemName = 'COGS_GrandTotalRow_ActualSales_' + tabIndex + "_" + rowIndex;
        SectionCOGS.reCalcCogsVarianceItem(actualSalesItemName);

        // set header grand total actual sales
        var actualSalesTotal = 0;
        $('[id^="COGS_GrandTotalRow_ActualSales_' + tabIndex + '_"]').each(function () {
            actualSalesTotal += parseFloat($(this).attr(reValue));
        });
        var $grandTotalActualSalesItem = $('#COGS_GrandTotal__ActualSales_' + tabIndex);
        $grandTotalActualSalesItem.val($.formatCurrency(actualSalesTotal));
        $grandTotalActualSalesItem.attr(reValue, actualSalesTotal);

        // get grand total projection sales by COGS
        var grandTotalActualSalesBySectionSales = parseFloat($('#GrandTotal__ActualSales_' + tabIndex).attr(reValue));

        // set header grand total actual percent
        actualPercent = (grandTotalActualSalesBySectionSales == 0) ? 0 : (actualSalesTotal * 100 / grandTotalActualSalesBySectionSales);
        var $grandTotalActualPercentItem = $('#COGS_GrandTotal__ActualPercent_' + tabIndex);
        $grandTotalActualPercentItem.val($.formatPercent(actualPercent));
        $grandTotalActualPercentItem.attr(reValue, actualPercent);

        // call reCalculate variance grand total by header
        SectionCOGS.reCalcCogsVarianceItem('COGS_GrandTotal__ActualSales_' + tabIndex);
    },

    reCalcCogsVarianceItem: function (itemName) {
        var varianceItemName = '';
        var actualSales = 0, projectionSales = 0;
        var actualPercent = 0, projectionPercent = 0;
        if (itemName.indexOf("Projection") != -1) {
            actualSales = parseFloat($('#' + itemName.replace("Projection", "Actual")).attr(reValue));
            projectionSales = parseFloat($('#' + itemName).attr(reValue));
            varianceItemName = itemName.replace("ProjectionSales", "VarianceSales");

            // get projection & actual percent
            projectionPercent = parseFloat($('#' + itemName.replace('Sales', 'Percent')).attr(reValue));
            actualPercent = parseFloat($('#' + itemName.replace('ProjectionSales', 'ActualPercent')).attr(reValue));
        } else {
            actualSales = parseFloat($('#' + itemName).attr(reValue));
            projectionSales = parseFloat($('#' + itemName.replace("Actual", "Projection")).attr(reValue));
            varianceItemName = itemName.replace("ActualSales", "VarianceSales");

            // get projection & actual percent
            actualPercent = parseFloat($('#' + itemName.replace('Sales', 'Percent')).attr(reValue));
            projectionPercent = parseFloat($('#' + itemName.replace('ActualSales', 'ProjectionPercent')).attr(reValue));
        }

        // set variance sales to item
        var varianceSales = projectionSales - actualSales;
        var $varianceSales = $('#' + varianceItemName);
        $varianceSales.val($.formatCurrency(varianceSales));
        $varianceSales.attr(reValue, varianceSales);

        // set variance percent to item
        var variancePercent = projectionPercent - actualPercent;
        var $variancePercent = $('#' + varianceItemName.replace("Sales", "Percent"));
        $variancePercent.val($.formatPercent(variancePercent));
        $variancePercent.attr(reValue, variancePercent);

        // get actual percent item
        var $actualPercent = $('#' + varianceItemName.replace("VarianceSales", "ActualPercent"));

        // get actual sales item by total row
        var $actualSales = null;
        if (itemName.indexOf('_Total__') != -1) {
            $actualSales = $('#' + varianceItemName.replace("VarianceSales", "ActualSales"));
        }

        // set color to item variance sales and reference item variance percent & actual percent & actual sales
        $.setColorToItem($varianceSales, $variancePercent, $actualPercent, $actualSales);
    },
};
//END: SECTION COGS

// START: SECTION PAYROLL IS NOT TAX
function onChangePayrollProjectionSales(e) {
    if (e.value == "." || isNaN(e.value) || e.value.length == 0) {
        e.value = 0;
    }

    $(e).attr(reValue, e.value);
    SectionPayroll.reCalcPayrollProjectionItem(e.id);

    // call reCalculate payroll is tax by row
    $('[id^="IsTax_' + e.id.substring(0, e.id.lastIndexOf('_')) + '_"]').each(function () {
        if (parseFloat(this.value) != 0)
            SectionPayroll.reCalcPayrollProjectionItem(this.id);
    });

    var nameArray = e.id.split("_");
    var tabIndex = nameArray[nameArray.length - 3];
    var colIndex = nameArray[nameArray.length - 2];

    // call reCalculate profit loss
    ProfitLoss.ReCalcProfitLossByColumn(tabIndex, colIndex);

    // set confirm redirect
    $.isChangeDataInPage = true;
}

function onChangePayrollProjectionPercent(e) {
    if (e.value == "." || isNaN(e.value) || e.value.length == 0) {
        e.value = 0;
    }

    $(e).attr(reValue, e.value);
    SectionPayroll.reCalcPayrollProjectionItem(e.id);

    // call reCalculate payroll is tax by row
    $('[id^="IsTax_' + e.id.substring(0, e.id.lastIndexOf('_')) + '_"]').each(function () {
        if (parseFloat(this.value) != 0)
            SectionPayroll.reCalcPayrollProjectionItem(this.id);
    });

    var nameArray = e.id.split("_");
    var tabIndex = nameArray[nameArray.length - 3];
    var colIndex = nameArray[nameArray.length - 2];

    // call reCalculate profit loss
    ProfitLoss.ReCalcProfitLossByColumn(tabIndex, colIndex);

    // set confirm redirect
    $.isChangeDataInPage = true;
}

function onChangePayrollActualSales(e) {
    if (e.value == "." || isNaN(e.value) || e.value.length == 0) {
        e.value = 0;
    }

    $(e).attr(reValue, e.value);
    SectionPayroll.reCalcPayrollActualPercent(e.id);

    var nameArray = e.id.split("_");
    var tabIndex = nameArray[nameArray.length - 3];
    var colIndex = nameArray[nameArray.length - 2];

    // call reCalculate profit loss
    ProfitLoss.ReCalcProfitLossByColumn(tabIndex, colIndex);

    // set confirm redirect
    $.isChangeDataInPage = true;
}
// END: SECTION PAYROLL IS NOT TAX

// START: SECTION PAYROLL IS TAX
function onChangePayrollIsTaxProjectionSales(e) {
    if (e.value == "." || isNaN(e.value) || e.value.length == 0) {
        e.value = 0;
    }

    $(e).attr(reValue, e.value);
    SectionPayroll.reCalcPayrollProjectionItem(e.id);

    var nameArray = e.id.split("_");
    var tabIndex = nameArray[nameArray.length - 3];
    var colIndex = nameArray[nameArray.length - 2];

    // call reCalculate profit loss
    ProfitLoss.ReCalcProfitLossByColumn(tabIndex, colIndex);

    // set confirm redirect
    $.isChangeDataInPage = true;
}

function onChangePayrollIsTaxProjectionPercent(e) {
    if (e.value == "." || isNaN(e.value) || e.value.length == 0) {
        e.value = 0;
    }

    $(e).attr(reValue, e.value);
    SectionPayroll.reCalcPayrollProjectionItem(e.id);

    var nameArray = e.id.split("_");
    var tabIndex = nameArray[nameArray.length - 3];
    var colIndex = nameArray[nameArray.length - 2];

    // call reCalculate profit loss
    ProfitLoss.ReCalcProfitLossByColumn(tabIndex, colIndex);

    // set confirm redirect
    $.isChangeDataInPage = true;
}

function onChangePayrollIsTaxActualSales(e) {
    if (e.value == "." || isNaN(e.value) || e.value.length == 0) {
        e.value = 0;
    }

    $(e).attr(reValue, e.value);
    SectionPayroll.reCalcPayrollIsTaxActualPercent(e.id);

    var nameArray = e.id.split("_");
    var tabIndex = nameArray[nameArray.length - 3];
    var colIndex = nameArray[nameArray.length - 2];

    // call reCalculate profit loss
    ProfitLoss.ReCalcProfitLossByColumn(tabIndex, colIndex);

    // set confirm redirect
    $.isChangeDataInPage = true;
}

var SectionPayroll = {
    reCalcPayrollProjectionItem: function (itemName) {
        var nameArray = itemName.split("_");
        var tabIndex = nameArray[nameArray.length - 3];
        var colIndex = nameArray[nameArray.length - 2];
        var rowIndex = nameArray[nameArray.length - 1];

        // target sales by column
        var targetSales = parseFloat($('#Target_Sales_' + tabIndex + '_' + colIndex).attr(reValue));
        if (itemName.indexOf("IsTax") != -1) {
            targetSales = parseFloat($('#Payroll_Expenses_Total__ProjectionSales_' + tabIndex + '_' + colIndex).attr(reValue));
        }

        var sales = 0, percent = 0;
        if (itemName.indexOf("Sales") != -1) {
            // projection sales data
            sales = parseFloat($('#' + itemName).attr(reValue));

            // reCalculate item projection percent
            percent = (targetSales == 0) ? 0 : sales * 100 / targetSales;
            var $percentItem = $('#' + itemName.replace("Sales", "Percent"));
            $percentItem.val($.formatPercent(percent));
            $percentItem.attr(reValue, percent);
        } else {
            // projection sales data
            percent = parseFloat($('#' + itemName).attr(reValue));

            // reCalculate item projection sales
            sales = percent * targetSales / 100;
            itemName = itemName.replace("Percent", "Sales");
            var $salesItem = $('#' + itemName);
            $salesItem.val($.formatCurrency(sales));
            $salesItem.attr(reValue, sales);
        }

        // call calculate total column
        SectionPayroll.reCalcPayrollTotalProjection(itemName, targetSales);
        SectionPayroll.reCalcPayrollGrandTotalProjection(itemName, tabIndex, rowIndex);

        // call calculate variance row
        SectionPayroll.reCalcPayrollVarianceItem(itemName);

        // call calculate total all by section payroll
        SectionPayroll.reCalcPayrollAllTotal(tabIndex, colIndex);
        SectionPayroll.reCalcPayrollAllGrandTotal(tabIndex);
    },
    reCalcPayrollTotalProjection: function (itemName, targetSales) {
        var salesTotalName = itemName.substring(0, itemName.lastIndexOf("_")).replace("ProjectionSales", "Total__ProjectionSales");

        var salesTotal = 0;
        $('[id^="' + itemName.substring(0, itemName.lastIndexOf("_")) + '_"]').each(function () {
            salesTotal += parseFloat($(this).attr(reValue));
        });

        // set total sales by column
        var $salesTotalItem = $('#' + salesTotalName);
        $salesTotalItem.val($.formatCurrency(salesTotal));
        $salesTotalItem.attr(reValue, salesTotal);

        // set total percent by column
        var percentTotal = (targetSales == 0) ? 0 : salesTotal * 100 / targetSales;
        $salesTotalItem.next().val($.formatPercent(percentTotal));
        $salesTotalItem.next().attr(reValue, percentTotal);

        // call reCalculate actual percent
        SectionPayroll.reCalcPayrollTotalActualPercent(salesTotalName);
    },

    reCalcPayrollActualPercent: function (itemName) {
        var nameArray = itemName.split("_");
        var tabIndex = nameArray[nameArray.length - 3];
        var colIndex = nameArray[nameArray.length - 2];
        var rowIndex = nameArray[nameArray.length - 1];

        // get actual sales by row
        var $actualSalesItem = $('#' + itemName);

        // get actual sales value by row
        var actualSales = parseFloat($actualSalesItem.attr(reValue));

        // get projection sales value by row
        var projectionSales = parseFloat($('#Payroll_Expenses_ProjectionSales_' + tabIndex + '_' + colIndex + '_' + rowIndex).attr(reValue));

        // set actual percent to row
        var actualPercent = (projectionSales == 0) ? 0 : (actualSales * 100.00 / projectionSales);
        $actualSalesItem.next().val($.formatPercent(actualPercent));
        $actualSalesItem.next().attr(reValue, actualPercent);

        // call calculate total actual header
        SectionPayroll.reCalcPayrollActualHeader(tabIndex, colIndex);

        // call calculate variance row
        SectionPayroll.reCalcPayrollVarianceItem(itemName);

        // call calculate grand total row
        SectionPayroll.reCalcPayrollGrandTotalActual(itemName);

        // call calculate total all by section payroll
        SectionPayroll.reCalcPayrollAllTotal(tabIndex, colIndex);
        SectionPayroll.reCalcPayrollAllGrandTotal(tabIndex);
    },
    reCalcPayrollActualHeader: function (tabIndex, colIndex) {
        var actualSales = 0;
        $('[id^="Payroll_Expenses_ActualSales_' + tabIndex + '_' + colIndex + '_"]').each(function (i, item) {
            actualSales += parseFloat($(item).attr(reValue));
        });

        // set total payroll actual sales
        var $totalPayrollActualSalesItem = $('#Payroll_Expenses_Total__ActualSales_' + tabIndex + '_' + colIndex);
        $totalPayrollActualSalesItem.val($.formatCurrency(actualSales));
        $totalPayrollActualSalesItem.attr(reValue, actualSales);

        // get total payroll projection sales
        var projectionSales = parseFloat($('#Payroll_Expenses_Total__ProjectionSales_' + tabIndex + '_' + colIndex).attr(reValue));

        // set total payroll actual percent
        var actualPercent = (projectionSales == 0) ? 0 : (actualSales * 100.00 / projectionSales);
        $totalPayrollActualSalesItem.next().val($.formatPercent(actualPercent));
        $totalPayrollActualSalesItem.next().attr(reValue, actualPercent);

        // call calculate total variance by column
        SectionPayroll.reCalcPayrollVarianceItem('Payroll_Expenses_Total__ProjectionSales_' + tabIndex + '_' + colIndex);

        // call calculate total all by section payroll
        SectionPayroll.reCalcPayrollAllTotal(tabIndex, colIndex);
        SectionPayroll.reCalcPayrollAllGrandTotal(tabIndex);
    },

    reCalcPayrollTotalActualPercent: function (itemName) {
        var actualSales = 0, projectionSales = 0, actualPercentName = "";
        if (itemName.indexOf("Projection") != -1) {
            actualSales = parseFloat($('#' + itemName.replace("Projection", "Actual")).attr(reValue));
            projectionSales = parseFloat($('#' + itemName).attr(reValue));
            actualPercentName = itemName.replace("ProjectionSales", "ActualPercent");
        } else {
            actualSales = parseFloat($('#' + itemName).attr(reValue));
            projectionSales = parseFloat($('#' + itemName.replace("Actual", "Projection")).attr(reValue));
            actualPercentName = itemName.replace("ActualSales", "ActualPercent");
        }

        var actualPercent = (projectionSales == 0) ? 0 : actualSales * 100 / projectionSales;
        var $actualPercentItem = $('#' + actualPercentName);
        $actualPercentItem.val($.formatPercent(actualPercent));
        $actualPercentItem.attr(reValue, actualPercent);

        // call calculate total variance by column
        SectionPayroll.reCalcPayrollVarianceItem(itemName);
    },

    reCalcPayrollIsTaxActualPercent: function (itemName) {
        var nameArray = itemName.split("_");
        var tabIndex = nameArray[nameArray.length - 3];
        var colIndex = nameArray[nameArray.length - 2];
        var rowIndex = nameArray[nameArray.length - 1];

        // get current actual sales item
        var $actualSalesItem = $('#' + itemName);

        // get actual sales value
        var actualSales = parseFloat($actualSalesItem.attr(reValue));

        // get projection sales by row
        var projectionSales = parseFloat($('#IsTax_Payroll_Expenses_ProjectionSales_' + tabIndex + '_' + colIndex + '_' + rowIndex).attr(reValue));

        // calculate actual percent
        var actualPercent = (projectionSales == 0) ? 0 : (actualSales * 100.00 / projectionSales);

        // set actual percent by row
        $actualSalesItem.next().val($.formatPercent(actualPercent));
        $actualSalesItem.next().attr(reValue, actualPercent);

        // call calculate total variance by column
        SectionPayroll.reCalcPayrollVarianceItem(itemName);

        // call calculate grand total row
        SectionPayroll.reCalcPayrollGrandTotalActual(itemName);

        // call calculate total payroll is tax actual header
        SectionPayroll.reCalcPayrollIsTaxActualHeader(tabIndex, colIndex);
    },
    reCalcPayrollIsTaxActualHeader: function (tabIndex, colIndex) {
        var actualSales = 0;
        $('[id^="IsTax_Payroll_Expenses_ActualSales_' + tabIndex + '_' + colIndex + '_"]').each(function (i, item) {
            actualSales += parseFloat($(item).attr(reValue));
        });

        // set total payroll is tax actual sales
        var $totalPayrollIsTaxActualSalesItem = $('#IsTax_Payroll_Expenses_Total__ActualSales_' + tabIndex + '_' + colIndex);
        $totalPayrollIsTaxActualSalesItem.val($.formatCurrency(actualSales));
        $totalPayrollIsTaxActualSalesItem.attr(reValue, actualSales);

        // get total payroll is tax projection sales
        var projectionSales = parseFloat($('#IsTax_Payroll_Expenses_Total__ProjectionSales_' + tabIndex + '_' + colIndex).attr(reValue));

        // set total payroll is tax actual percent
        var actualPercent = (projectionSales == 0) ? 0 : (actualSales * 100.00 / projectionSales);
        $totalPayrollIsTaxActualSalesItem.next().val($.formatPercent(actualPercent));
        $totalPayrollIsTaxActualSalesItem.next().attr(reValue, actualPercent);

        // call calculate total variance by column
        SectionPayroll.reCalcPayrollVarianceItem('IsTax_Payroll_Expenses_Total__ProjectionSales_' + tabIndex + '_' + colIndex);

        // call calculate total all by section payroll
        SectionPayroll.reCalcPayrollAllTotal(tabIndex, colIndex);
        SectionPayroll.reCalcPayrollAllGrandTotal(tabIndex);
    },

    reCalcPayrollGrandTotalProjection: function (itemName, tabIndex, rowIndex) {
        var dataRow = $('#' + itemName).closest('.data-row-by-category');
        var categorySettingId = $(dataRow).attr("category-setting-id");
        var projectionSales = 0;
        $(dataRow).find('[id*="ProjectionSales"]').each(function () {
            projectionSales += parseFloat($(this).attr(reValue));
        });

        // set to item grand total projection sales
        var $currentDiv = $(dataRow).closest('.tab-area-by-section');
        var $grandTotalProjectionSalesItem = $currentDiv.find('.grand-total-category-by-row input[category-setting-id="' + categorySettingId + '"]');
        $grandTotalProjectionSalesItem.val($.formatCurrency(projectionSales));
        $grandTotalProjectionSalesItem.attr(reValue, projectionSales);

        // get header grand total projection sales by section sales
        var totalBySectionSales = parseFloat($('#GrandTotal__ProjectionSales_' + tabIndex).attr(reValue));

        // set grand total projection percent by row
        var projectionPercent = (totalBySectionSales == 0) ? 0 : (projectionSales * 100 / totalBySectionSales);
        $grandTotalProjectionSalesItem.next().val($.formatPercent(projectionPercent));
        $grandTotalProjectionSalesItem.next().attr(reValue, projectionPercent);

        // call calculate actual percent
        var itemSalesName = $grandTotalProjectionSalesItem.attr("id");
        SectionPayroll.reCalcPayrollTotalActualPercent(itemSalesName);

        // set header grand total projection sales
        var projectionSalesTotal = 0;
        $('[id^="' + itemSalesName.substring(0, itemSalesName.lastIndexOf('_')) + '_"]').each(function () {
            projectionSalesTotal += parseFloat($(this).attr(reValue));
        });
        var grandTotalColumnProjectionSalesItemName = itemSalesName.substring(0, itemSalesName.lastIndexOf('_')).replace("Row", "_");
        var $grandTotalColumnProjectionSalesItem = $('#' + grandTotalColumnProjectionSalesItemName);
        $grandTotalColumnProjectionSalesItem.val($.formatCurrency(projectionSalesTotal));
        $grandTotalColumnProjectionSalesItem.attr(reValue, projectionSalesTotal);

        // set grand total projection percent
        projectionPercent = (totalBySectionSales == 0) ? 0 : (projectionSalesTotal * 100 / totalBySectionSales);
        $grandTotalColumnProjectionSalesItem.next().val($.formatPercent(projectionPercent));
        $grandTotalColumnProjectionSalesItem.next().attr(reValue, projectionPercent);

        // call calculate grand total acutal percent
        SectionPayroll.reCalcPayrollTotalActualPercent(grandTotalColumnProjectionSalesItemName);
    },
    reCalcPayrollGrandTotalActual: function (itemName) {
        var dataRow = $('#' + itemName).closest('.data-row-by-category');
        var categorySettingId = $(dataRow).attr("category-setting-id");
        var actualSales = 0;
        $(dataRow).find('[id*="ActualSales"]').each(function () {
            actualSales += parseFloat($(this).attr(reValue));
        });

        // set grand total actual sales
        var currentDiv = $(dataRow).closest('.tab-area-by-section');
        var grandTotalRow = $(currentDiv).find('.grand-total-category-by-row input[category-setting-id="' + categorySettingId + '"]').closest('.grand-total-category-by-row');
        var $actualSalesItem = $(grandTotalRow).find('[id*="ActualSales"]');
        $actualSalesItem.val($.formatCurrency(actualSales));
        $actualSalesItem.attr(reValue, actualSales);

        // call calculate actual percent
        var actualSalesItemName = $actualSalesItem.attr("id");
        SectionPayroll.reCalcPayrollTotalActualPercent(actualSalesItemName);

        // set header grand total actual sales
        var actualSalesTotal = 0;
        $('[id^="' + actualSalesItemName.substring(0, actualSalesItemName.lastIndexOf('_')) + '_"]').each(function () {
            actualSalesTotal += parseFloat($(this).attr(reValue));
        });
        var grandTotalColumnActualSalesItemName = actualSalesItemName.substring(0, actualSalesItemName.lastIndexOf('_')).replace("Row", "_");
        var $grandTotalColumnActualSalesItem = $('#' + grandTotalColumnActualSalesItemName);
        $grandTotalColumnActualSalesItem.val($.formatCurrency(actualSalesTotal));
        $grandTotalColumnActualSalesItem.attr(reValue, actualSalesTotal);

        // call calculate grand total acutal percent
        SectionPayroll.reCalcPayrollTotalActualPercent(grandTotalColumnActualSalesItemName);
    },

    reCalcPayrollAllTotal: function (tabIndex, colIndex) {
        // set total all projection sales
        var totalProjectionSales = parseFloat($('#Payroll_Expenses_Total__ProjectionSales_' + tabIndex + "_" + colIndex).attr(reValue))
                                 + parseFloat($('#IsTax_Payroll_Expenses_Total__ProjectionSales_' + tabIndex + "_" + colIndex).attr(reValue));
        $('#All_Payroll_Expenses_Total__ProjectionSales_' + tabIndex + "_" + colIndex).val($.formatCurrency(totalProjectionSales));
        $('#All_Payroll_Expenses_Total__ProjectionSales_' + tabIndex + "_" + colIndex).attr(reValue, totalProjectionSales);

        // set total all Projection Percent
        var targetSales = parseFloat($('#Target_Sales_' + tabIndex + '_' + colIndex).attr(reValue));
        var totalProjectionPercent = (targetSales == 0) ? 0 : (totalProjectionSales * 100 / targetSales);
        $('#All_Payroll_Expenses_Total__ProjectionPercent_' + tabIndex + "_" + colIndex).val($.formatPercent(totalProjectionPercent));
        $('#All_Payroll_Expenses_Total__ProjectionPercent_' + tabIndex + "_" + colIndex).attr(reValue, totalProjectionPercent);

        // set total all actual sales
        var totalActualSales = parseFloat($('#Payroll_Expenses_Total__ActualSales_' + tabIndex + "_" + colIndex).attr(reValue))
                             + parseFloat($('#IsTax_Payroll_Expenses_Total__ActualSales_' + tabIndex + "_" + colIndex).attr(reValue));
        var $actualSales = $('#All_Payroll_Expenses_Total__ActualSales_' + tabIndex + "_" + colIndex);
        $actualSales.val($.formatCurrency(totalActualSales));
        $actualSales.attr(reValue, totalActualSales);

        // set total all actual Percent
        var totalActualBySales = parseFloat($('#Total__ActualSales_' + tabIndex + '_' + colIndex).attr(reValue));
        var totalActualPercent = (totalProjectionSales == 0) ? 0 : (totalActualSales * 100 / totalProjectionSales);
        var $actualPercent = $('#All_Payroll_Expenses_Total__ActualPercent_' + tabIndex + "_" + colIndex);
        $actualPercent.val($.formatPercent(totalActualPercent));
        $actualPercent.attr(reValue, totalActualPercent);

        // Total Variance Sales
        var totalVarianceSales = totalProjectionSales - totalActualSales;
        var newStyleColor = (totalVarianceSales > 0) ? 'green' : 'red';
        var $varianceSales = $('#All_Payroll_Expenses_Total__VarianceSales_' + tabIndex + "_" + colIndex);
        $varianceSales.val($.formatCurrency(totalVarianceSales));
        $varianceSales.attr(reValue, totalVarianceSales);

        // Total Variance Percent
        var totalVariancePercent = (totalProjectionSales == 0) ? 0 : (totalVarianceSales * 100 / totalProjectionSales);
        var $variancePercent = $('#All_Payroll_Expenses_Total__VariancePercent_' + tabIndex + "_" + colIndex);
        $variancePercent.val($.formatPercent(totalVariancePercent));
        $variancePercent.attr(reValue, totalVariancePercent);

        // set color to item variance sales and reference item variance percent & actual percent & actual sales
        $.setColorToItem($varianceSales, $variancePercent, $actualPercent, $actualSales);
    },
    reCalcPayrollAllGrandTotal: function (tabIndex) {
        // set total all projection sales
        var totalProjectionSales = parseFloat($('#Payroll_Expenses_GrandTotal__ProjectionSales_' + tabIndex).attr(reValue))
                                 + parseFloat($('#IsTax_Payroll_Expenses_GrandTotal__ProjectionSales_' + tabIndex).attr(reValue));
        $('#All_Payroll_Expenses_GrandTotal__ProjectionSales_' + tabIndex).val($.formatCurrency(totalProjectionSales));
        $('#All_Payroll_Expenses_GrandTotal__ProjectionSales_' + tabIndex).attr(reValue, totalProjectionSales);

        // set total all Projection Percent
        var grandTotalProjectionSales = parseFloat($('#GrandTotal__ProjectionSales_' + tabIndex).attr(reValue));
        var totalProjectionPercent = (grandTotalProjectionSales == 0) ? 0 : (totalProjectionSales * 100 / grandTotalProjectionSales);
        $('#All_Payroll_Expenses_GrandTotal__ProjectionPercent_' + tabIndex).val($.formatPercent(totalProjectionPercent));
        $('#All_Payroll_Expenses_GrandTotal__ProjectionPercent_' + tabIndex).attr(reValue, (totalProjectionPercent));

        // set total all actual sales
        var totalActualSales = parseFloat($('#Payroll_Expenses_GrandTotal__ActualSales_' + tabIndex).attr(reValue))
                             + parseFloat($('#IsTax_Payroll_Expenses_GrandTotal__ActualSales_' + tabIndex).attr(reValue));
        $('#All_Payroll_Expenses_GrandTotal__ActualSales_' + tabIndex).val($.formatCurrency(totalActualSales));
        $('#All_Payroll_Expenses_GrandTotal__ActualSales_' + tabIndex).attr(reValue, totalActualSales);

        // set total all actual Percent
        var totalActualPercent = (totalProjectionSales == 0) ? 0 : (totalActualSales * 100 / totalProjectionSales);
        $('#All_Payroll_Expenses_GrandTotal__ActualPercent_' + tabIndex).val($.formatPercent(totalActualPercent));
        $('#All_Payroll_Expenses_GrandTotal__ActualPercent_' + tabIndex).attr(reValue, totalActualPercent);

        // Total Variance Sales
        var totalVarianceSales = totalProjectionSales - totalActualSales;
        var newStyleColor = (totalVarianceSales > 0) ? 'color: green' : 'color: red';
        $('#All_Payroll_Expenses_GrandTotal__VarianceSales_' + tabIndex).attr('style', newStyleColor);
        $('#All_Payroll_Expenses_GrandTotal__VarianceSales_' + tabIndex).val($.formatCurrency(totalVarianceSales));

        // Total Variance Percent
        var totalVariancePercent = (totalProjectionSales == 0) ? 0 : (totalVarianceSales * 100 / totalProjectionSales);
        $('#All_Payroll_Expenses_GrandTotal__VariancePercent_' + tabIndex).attr('style', newStyleColor);
        $('#All_Payroll_Expenses_GrandTotal__VariancePercent_' + tabIndex).val($.formatPercent(totalVariancePercent));
    },

    reCalcPayrollVarianceItem: function (itemName) {
        var actualSales = 0, projectionSales = 0, varianceItemName = '';
        if (itemName.indexOf("Projection") != -1) {
            actualSales = parseFloat($('#' + itemName.replace("Projection", "Actual")).attr(reValue));
            projectionSales = parseFloat($('#' + itemName).attr(reValue));
            varianceItemName = itemName.replace("ProjectionSales", "VarianceSales");
        } else {
            actualSales = parseFloat($('#' + itemName).attr(reValue));
            projectionSales = parseFloat($('#' + itemName.replace("Actual", "Projection")).attr(reValue));
            varianceItemName = itemName.replace("ActualSales", "VarianceSales");
        }

        // set variance sales to item
        var varianceSales = projectionSales - actualSales;
        var $varianceSales = $('#' + varianceItemName);
        $varianceSales.val($.formatCurrency(varianceSales));
        $varianceSales.attr(reValue, varianceSales);

        // set variance percent to item
        var variancePercent = (projectionSales == 0) ? 0 : (varianceSales * 100 / projectionSales);
        var $variancePercent = $('#' + varianceItemName.replace("Sales", "Percent"));
        $variancePercent.val($.formatPercent(variancePercent));
        $variancePercent.attr(reValue, variancePercent);

        // get actual percent item
        var $actualPercent = $('#' + varianceItemName.replace("VarianceSales", "ActualPercent"));

        // get actual sales item by total row
        var $actualSales = null;
        if (itemName.indexOf('_Total__') != -1) {
            $actualSales = $('#' + varianceItemName.replace("VarianceSales", "ActualSales"));
        }

        // set color to item variance sales and reference item variance percent & actual percent & actual sales
        $.setColorToItem($varianceSales, $variancePercent, $actualPercent, $actualSales);
    },
};
// END: SECTION PAYROLL IS TAX

// START: SECTION OPERATION
function onChangeOperationProjectionSales(e) {
    if (e.value == "." || isNaN(e.value) || e.value.length == 0) {
        e.value = 0;
    }

    $(e).attr(reValue, e.value);
    SectionOperation.reCalcOperationProjectionItem(e.id);

    var nameArray = e.id.split("_");
    var tabIndex = nameArray[nameArray.length - 3];
    var colIndex = nameArray[nameArray.length - 2];

    // call reCalculate profit loss
    ProfitLoss.ReCalcProfitLossByColumn(tabIndex, colIndex);

    // set confirm redirect
    $.isChangeDataInPage = true;
}

function onChangeOperationProjectionPercent(e) {
    if (e.value == "." || isNaN(e.value) || e.value.length == 0) {
        e.value = 0;
    }

    $(e).attr(reValue, e.value);
    SectionOperation.reCalcOperationProjectionItem(e.id);

    var nameArray = e.id.split("_");
    var tabIndex = nameArray[nameArray.length - 3];
    var colIndex = nameArray[nameArray.length - 2];

    // call reCalculate profit loss
    ProfitLoss.ReCalcProfitLossByColumn(tabIndex, colIndex);

    // set confirm redirect
    $.isChangeDataInPage = true;
}

function onChangeOperationActualSales(e) {
    if (e.value == "." || isNaN(e.value) || e.value.length == 0) {
        e.value = 0;
    }

    $(e).attr(reValue, e.value);
    SectionOperation.reCalcOperationActualPercent(e.id);

    var nameArray = e.id.split("_");
    var tabIndex = nameArray[nameArray.length - 3];
    var colIndex = nameArray[nameArray.length - 2];

    // call reCalculate profit loss
    ProfitLoss.ReCalcProfitLossByColumn(tabIndex, colIndex);

    // set confirm redirect
    $.isChangeDataInPage = true;
}

var SectionOperation = {
    reCalcOperationProjectionItem: function (itemName) {
        var nameArray = itemName.split("_");
        var tabIndex = nameArray[nameArray.length - 3];
        var colIndex = nameArray[nameArray.length - 2];
        var rowIndex = nameArray[nameArray.length - 1];

        // target sales by column
        var targetSales = parseFloat($('#Target_Sales_' + tabIndex + '_' + colIndex).attr(reValue));

        var sales = 0, percent = 0;
        if (itemName.indexOf("Sales") != -1) {
            // projection sales data
            sales = parseFloat($('#' + itemName).attr(reValue));

            // reCalculate item projection percent
            percent = (targetSales == 0) ? 0 : sales * 100 / targetSales;
            $('#' + itemName.replace("Sales", "Percent")).val($.formatPercent(percent));
            $('#' + itemName.replace("Sales", "Percent")).attr(reValue, percent);
        } else {
            // projection sales data
            percent = parseFloat($('#' + itemName).attr(reValue));

            // reCalculate item projection sales
            sales = percent * targetSales / 100;
            itemName = itemName.replace("Percent", "Sales");
            $('#' + itemName).val($.formatCurrency(sales));
            $('#' + itemName).attr(reValue, sales);
        }

        // call calculate total column
        SectionOperation.reCalcOperationTotalProjection(itemName, targetSales);
        SectionOperation.reCalcOperationGrandTotalProjection(itemName, tabIndex, rowIndex);

        // call calculate variance row
        SectionOperation.reCalcOperationVarianceItem(itemName);
    },
    reCalcOperationTotalProjection: function (itemName, targetSales) {
        var salesTotalName = itemName.substring(0, itemName.lastIndexOf("_")).replace("ProjectionSales", "Total__ProjectionSales");

        var salesTotal = 0;
        $('[id^="' + itemName.substring(0, itemName.lastIndexOf("_"))).each(function () {
            salesTotal += parseFloat($(this).attr(reValue));
        });

        // set total sales by column
        $('#' + salesTotalName).val($.formatCurrency(salesTotal));
        $('#' + salesTotalName).attr(reValue, salesTotal);

        // set total percent by column
        var percentTotal = (targetSales == 0) ? 0 : salesTotal * 100 / targetSales;
        $('#' + salesTotalName.replace("Sales", "Percent")).val($.formatPercent(percentTotal));
        $('#' + salesTotalName.replace("Sales", "Percent")).attr(reValue, percentTotal);

        // call reCalculate actual percent
        SectionOperation.reCalcOperationTotalActualPercent(salesTotalName);
    },

    reCalcOperationActualPercent: function (itemName) {
        var nameArray = itemName.split("_");
        var tabIndex = nameArray[nameArray.length - 3];
        var colIndex = nameArray[nameArray.length - 2];
        var rowIndex = nameArray[nameArray.length - 1];

        // get projection sales by row
        var projectionSales = parseFloat($('#Operation_Expenses_ProjectionSales_' + tabIndex + '_' + colIndex + '_' + rowIndex).attr(reValue));

        // get actual sales by item name
        var actualSales = parseFloat($("#" + itemName).attr(reValue));

        // set actual percent by row
        var actualPercent = (projectionSales == 0) ? 0 : (actualSales * 100 / projectionSales);
        $("#" + itemName.replace("Sales", "Percent")).val($.formatPercent(actualPercent));
        $("#" + itemName.replace("Sales", "Percent")).attr(reValue, actualPercent);

        // call calculate variance row
        SectionOperation.reCalcOperationVarianceItem(itemName);

        // call calculate grand total row
        SectionOperation.reCalcOperationGrandTotalActual(itemName, tabIndex, rowIndex);

        // get total actual by column
        var totalActual = 0;
        $('[id^="Operation_Expenses_ActualSales_' + tabIndex + '_' + colIndex + '_"]').each(function () {
            totalActual += parseFloat($(this).attr(reValue));
        });

        // set total actual by column
        var $totalActualSalesItemName = $('#Operation_Expenses_Total__ActualSales_' + tabIndex + '_' + colIndex);
        $totalActualSalesItemName.val($.formatCurrency(totalActual));
        $totalActualSalesItemName.attr(reValue, totalActual);

        // set total actual percent by column
        SectionOperation.reCalcOperationTotalActualPercent('Operation_Expenses_Total__ActualSales_' + tabIndex + '_' + colIndex);
    },
    reCalcOperationTotalActualPercent: function (itemName) {
        var actualSales = 0, projectionSales = 0, $actualPercentItem;
        if (itemName.indexOf("Projection") != -1) {
            actualSales = parseFloat($('#' + itemName.replace("Projection", "Actual")).attr(reValue));
            projectionSales = parseFloat($('#' + itemName).attr(reValue));
            $actualPercentItem = $('#' + itemName.replace("ProjectionSales", "ActualPercent"));
        } else {
            actualSales = parseFloat($('#' + itemName).attr(reValue));
            projectionSales = parseFloat($('#' + itemName.replace("Actual", "Projection")).attr(reValue));
            $actualPercentItem = $('#' + itemName.replace("ActualSales", "ActualPercent"));
        }

        var actualPercent = (projectionSales == 0) ? 0 : actualSales * 100 / projectionSales;
        $actualPercentItem.val($.formatPercent(actualPercent));
        $actualPercentItem.attr(reValue, actualPercent);

        // call calculate total variance by column
        SectionOperation.reCalcOperationVarianceItem(itemName);
    },

    reCalcOperationGrandTotalProjection: function (itemName, tabIndex, rowIndex) {
        var dataRow = $('#' + itemName).closest('.data-row-by-category');
        var categorySettingId = $(dataRow).attr("category-setting-id");
        var projectionSales = 0;
        $(dataRow).find('[id*="ProjectionSales"]').each(function () {
            projectionSales += parseFloat($(this).attr(reValue));
        });

        // set to item grand total projection sales
        var currentDiv = $(dataRow).closest('.tab-area-by-section');
        var $grandTotalProjectionSalesItem = $('#Operation_Expenses_GrandTotalRow_ProjectionSales_' + tabIndex + '_' + rowIndex);
        $grandTotalProjectionSalesItem.val($.formatCurrency(projectionSales));
        $grandTotalProjectionSalesItem.attr(reValue, projectionSales);

        // get header grand total projection sales by section sales
        var totalBySectionSales = parseFloat($('#GrandTotal__ProjectionSales_' + tabIndex).attr(reValue));

        // set grand total projection percent by row
        var projectionPercent = (totalBySectionSales == 0) ? 0 : (projectionSales * 100 / totalBySectionSales);
        $grandTotalProjectionSalesItem.next().val($.formatPercent(projectionPercent));
        $grandTotalProjectionSalesItem.next().attr(reValue, projectionPercent);

        // call calculate actual percent
        var itemSalesName = $grandTotalProjectionSalesItem.attr("id");
        SectionOperation.reCalcOperationTotalActualPercent(itemSalesName);

        // set header grand total projection sales
        var projectionSalesTotal = 0;
        $('[id^="' + itemSalesName.substring(0, itemSalesName.lastIndexOf('_')) + '_"]').each(function () {
            projectionSalesTotal += parseFloat($(this).attr(reValue));
        });
        var grandTotalColumnProjectionSalesItemName = itemSalesName.substring(0, itemSalesName.lastIndexOf('_')).replace("Row", "_");
        var $grandTotalColumnProjectionSalesItem = $('#' + grandTotalColumnProjectionSalesItemName);
        $grandTotalColumnProjectionSalesItem.val($.formatCurrency(projectionSalesTotal));
        $grandTotalColumnProjectionSalesItem.attr(reValue, projectionSalesTotal);

        // set grand total projection percent
        projectionPercent = (totalBySectionSales == 0) ? 0 : (projectionSalesTotal * 100 / totalBySectionSales);
        $grandTotalColumnProjectionSalesItem.next().val($.formatPercent(projectionPercent));
        $grandTotalColumnProjectionSalesItem.next().attr(reValue, projectionPercent);

        // call calculate grand total acutal percent
        SectionOperation.reCalcOperationTotalActualPercent(grandTotalColumnProjectionSalesItemName);
    },
    reCalcOperationGrandTotalActual: function (itemName, tabIndex, rowIndex) {
        var dataRow = $('#' + itemName).closest('.data-row-by-category');
        var categorySettingId = $(dataRow).attr("category-setting-id");
        var actualSales = 0;
        $(dataRow).find('[id*="ActualSales"]').each(function () {
            actualSales += parseFloat($(this).attr(reValue));
        });

        // set grand total actual sales
        var $grandTotalActualSalesItem = $('#Operation_Expenses_GrandTotalRow_ActualSales_' + tabIndex + '_' + rowIndex);
        $grandTotalActualSalesItem.val($.formatCurrency(actualSales));
        $grandTotalActualSalesItem.attr(reValue, actualSales);

        // call calculate actual percent
        var actualSalesItemName = 'Operation_Expenses_GrandTotalRow_ActualSales_' + tabIndex + '_' + rowIndex;
        SectionOperation.reCalcOperationTotalActualPercent(actualSalesItemName);

        // set header grand total actual sales
        var actualSalesTotal = 0;
        $('[id^="Operation_Expenses_GrandTotalRow_ActualSales_' + tabIndex + '_"]').each(function () {
            actualSalesTotal += parseFloat($(this).attr(reValue));
        });
        var grandTotalColumnActualSalesItemName = actualSalesItemName.substring(0, actualSalesItemName.lastIndexOf('_')).replace("Row", "_");
        var $grandTotalColumnActualSalesItem = $('#' + grandTotalColumnActualSalesItemName);
        $grandTotalColumnActualSalesItem.val($.formatCurrency(actualSalesTotal));
        $grandTotalColumnActualSalesItem.attr(reValue, actualSalesTotal);

        // call calculate grand total acutal percent
        SectionOperation.reCalcOperationTotalActualPercent(grandTotalColumnActualSalesItemName);
    },

    reCalcOperationVarianceItem: function (itemName) {
        var actualSales = 0, projectionSales = 0, varianceItemName = '';
        if (itemName.indexOf("Projection") != -1) {
            actualSales = parseFloat($('#' + itemName.replace("Projection", "Actual")).attr(reValue));
            projectionSales = parseFloat($('#' + itemName).attr(reValue));
            varianceItemName = itemName.replace("ProjectionSales", "VarianceSales");
        } else {
            actualSales = parseFloat($('#' + itemName).attr(reValue));
            projectionSales = parseFloat($('#' + itemName.replace("Actual", "Projection")).attr(reValue));
            varianceItemName = itemName.replace("ActualSales", "VarianceSales");
        }

        // set variance sales to item
        var varianceSales = projectionSales - actualSales;
        var $varianceSales = $('#' + varianceItemName);
        $varianceSales.val($.formatCurrency(varianceSales));
        $varianceSales.attr(reValue, varianceSales);

        // set variance percent to item
        var variancePercent = (projectionSales == 0) ? 0 : (varianceSales * 100 / projectionSales);
        var $variancePercent = $('#' + varianceItemName.replace("Sales", "Percent"));
        $variancePercent.val($.formatPercent(variancePercent));
        $variancePercent.attr(reValue, variancePercent);

        // get actual percent item
        var $actualPercent = $('#' + varianceItemName.replace("VarianceSales", "ActualPercent"));

        // get actual sales item by total row
        var $actualSales = null;
        if (itemName.indexOf('_Total__') != -1) {
            $actualSales = $('#' + varianceItemName.replace("VarianceSales", "ActualSales"));
        }

        // set color to item variance sales and reference item variance percent & actual percent & actual sales
        $.setColorToItem($varianceSales, $variancePercent, $actualPercent, $actualSales);
    },
};
//END: SECTION OPERATION

// on change annual sales
function onChangeAnnualSales(e) {
    if (e.value == "." || isNaN(e.value) || e.value.length == 0) {
        e.value = 0;
    }

    $(e).attr(reValue, e.value);

    var nameArray = e.id.split("_");
    var tabIndex = nameArray[nameArray.length - 1];

    // call onchange target
    showProcessing(1500);
    setTimeout(function () {
        var reCalculateTotalTargerFlg = true;
        if ($('#InputMethod').val() == $('#inputMethodDollar').val()) {
            $('[id^="Target_Sales_' + tabIndex + '"]').each(function () {
                if (parseFloat($(this).attr(reValue)) != 0) {
                    Target.reCalcTarget($(this).attr("id"));
                    reCalculateTotalTargerFlg = false;
                }
            });
        } else {
            $('[id^="Target_Percent_' + tabIndex + '"]').each(function () {
                if (parseFloat($(this).attr(reValue)) != 0) {
                    Target.reCalcTarget($(this).attr("id"));
                    reCalculateTotalTargerFlg = false;
                }
            });
        }

        // check flag reCalculate data
        if (reCalculateTotalTargerFlg) {
            // call reCalculate target sales
            SectionSales.reCalcTotalSalesTarget(tabIndex);

            // call calculate total sales
            SectionSales.reCalcTotalSales(tabIndex);
        }

        // Call reCalculate profit loss
        ProfitLoss.ReCalcProfitLossByRow(tabIndex);
    }, 500);

    // set confirm redirect
    $.isChangeDataInPage = true;
}

// on change target sales
function onChangeTargetSales(e) {
    if (e.value == "." || isNaN(e.value) || e.value.length == 0) {
        e.value = 0;
    }

    $(e).attr(reValue, e.value);
    Target.reCalcTarget(e.id);

    var nameArray = e.id.split("_");
    var tabIndex = nameArray[nameArray.length - 2];
    var colIndex = nameArray[nameArray.length - 1];

    // call reCalculate profit loss
    ProfitLoss.ReCalcProfitLossByColumn(tabIndex, colIndex);

    // set confirm redirect
    $.isChangeDataInPage = true;
}

// on change target percent
function onChangeTargetPercent(e) {
    if (e.value == "." || isNaN(e.value) || e.value.length == 0) {
        e.value = 0;
    }

    $(e).attr(reValue, e.value);
    Target.reCalcTarget(e.id);

    var nameArray = e.id.split("_");
    var tabIndex = nameArray[nameArray.length - 2];
    var colIndex = nameArray[nameArray.length - 1];

    // call reCalculate profit loss
    ProfitLoss.ReCalcProfitLossByColumn(tabIndex, colIndex);

    // set confirm redirect
    $.isChangeDataInPage = true;
}

var Target = {
    reCalcTarget: function (itemName) {
        showProcessing(1500);
        setTimeout(function () {
            var nameArray = itemName.split("_");
            var tabIndex = nameArray[nameArray.length - 2];
            var colIndex = nameArray[nameArray.length - 1];

            // get anual sales by tab index
            var annualSales = $('#Annual_Sales_' + tabIndex).attr(reValue);

            var targetSales = 0, targetPercent = 0;
            if (itemName.indexOf("Sales") != -1) {
                targetSales = $('#' + itemName).attr(reValue);
                targetPercent = (annualSales == 0) ? 0 : targetSales * 100 / annualSales;
                $('#Target_Percent_' + tabIndex + '_' + colIndex).val($.formatPercent(targetPercent));
                $('#Target_Percent_' + tabIndex + '_' + colIndex).attr(reValue, targetPercent);
            } else {
                targetPercent = $('#' + itemName).attr(reValue);
                targetSales = targetPercent * annualSales / 100;
                $('#Target_Sales_' + tabIndex + '_' + colIndex).val($.formatCurrency(targetSales));
                $('#Target_Sales_' + tabIndex + '_' + colIndex).attr(reValue, targetSales);
            }

            // call onchange item projection
            var reCalculateTotalTargerFlg = true;
            $('[id^="ProjectionSales_' + tabIndex + '_' + colIndex + '_"]').each(function () {
                var currentItem = $(this);
                if ($(this).hasAttr('readonly')) {
                    currentItem = currentItem.next();
                }

                if ($(currentItem).attr(reValue) != 0) {
                    SectionSales.reCalcProjectionItem($(currentItem).attr("id"));
                    reCalculateTotalTargerFlg = false;
                }
            });

            // check flag reCalculate data
            if (reCalculateTotalTargerFlg) {
                // call reCalculate target sales & total sales
                SectionSales.reCalcTotalTarget(tabIndex, colIndex);

                // call calculate total sales
                SectionSales.reCalcTotalSales(tabIndex);
            }

            // call onchange item projection
            $('[id^="Payroll_Expenses_ProjectionSales_' + tabIndex + '_' + colIndex + '_"]').each(function () {
                var currentItem = $(this);
                if ($(this).hasAttr('readonly')) {
                    currentItem = currentItem.next();
                }

                if ($(currentItem).attr(reValue) != 0)
                    SectionPayroll.reCalcPayrollProjectionItem($(currentItem).attr("id"));
            });

            // call onchange item projection
            $('[id^="IsTax_Payroll_Expenses_ProjectionSales_' + tabIndex + '_' + colIndex + '_"]').each(function () {
                var currentItem = $(this);
                if ($(this).hasAttr('readonly')) {
                    currentItem = currentItem.next();
                }

                if ($(currentItem).attr(reValue) != 0)
                    SectionPayroll.reCalcPayrollProjectionItem($(currentItem).attr("id"));
            });

            // call onchange item projection
            $('[id^="Operation_Expenses_ProjectionSales_' + tabIndex + '_' + colIndex + '_"]').each(function () {
                var currentItem = $(this);
                if ($(this).hasAttr('readonly')) {
                    currentItem = currentItem.next();
                }

                if ($(currentItem).attr(reValue) != 0)
                    SectionOperation.reCalcOperationProjectionItem($(currentItem).attr("id"));
            });
        }, 500);
    }
};

var ProfitLoss = {
    FlagCalculateGrandTotal: false,
    FlagRuningTotalProfit: false,

    // function calculate profit loss by column index
    ReCalcProfitLossByColumn: function (tabIndex, colIndex) {
        // call function calculate profit loss
        $.isCalculateProfitLoss = true;
        setTimeout(function () {
            if ($.isCalculateProfitLoss) {
                console.log('reCalculate profit loss by column');

                // calculate profit loss item
                ProfitLoss.CalcProfitLossItem(tabIndex, colIndex);
            }

            // reset call calculate profit loss
            $.isCalculateProfitLoss = false;
        }, 5000);
    },

    // function calculate profit loss all column by row
    ReCalcProfitLossByRow: function (tabIndex) {
        // call function calculate profit loss
        $.isCalculateProfitLoss = true;
        setTimeout(function () {
            if ($.isCalculateProfitLoss) {
                console.log('reCalculate profit loss by row');

                // get all column by active tab
                $('.active:last .header-name').each(function (colIndex, item) {
                    // calculate profit loss item
                    ProfitLoss.CalcProfitLossItem(tabIndex, colIndex);
                });
            }

            // reset call calculate profit loss
            $.isCalculateProfitLoss = false;
        }, 5000);
    },

    // function calculate profit loss all item by column
    CalcProfitLossItem: function (tabIndex, colIndex) {
        // get data Sales Total by tab index and col index
        var projectionSalesBySales = $.formatNumber($('#Total__ProjectionSales_' + tabIndex + '_' + colIndex).attr(reValue));
        var actualSalesBySales = $.formatNumber($('#Total__ActualSales_' + tabIndex + '_' + colIndex).attr(reValue));
        var varianceSalesBySales = actualSalesBySales - projectionSalesBySales;
        var salesItem = {
            ProjectionSales: projectionSalesBySales,
            ProjectionPercent: $.formatNumber($('#Total__ProjectionPercent_' + tabIndex + '_' + colIndex).attr(reValue)),
            ActualSales: actualSalesBySales,
            ActualPercent: actualSalesBySales == 0 ? 0 : 100,
            VarianceSales: varianceSalesBySales,
            VariancePercent: (projectionSalesBySales == 0) ? 0 : varianceSalesBySales * 100 / projectionSalesBySales,
        };

        // set data to Sales: row index is 0
        ProfitLoss.SetDataToProfitLossItem(tabIndex, colIndex, 0, salesItem);

        // get data COGS Total By tab index and col index
        var projectionSalesByCOGS = $.formatNumber($('#COGS_Total__ProjectionSales_' + tabIndex + '_' + colIndex).attr(reValue));
        var actualSalesByCOGS = $.formatNumber($('#COGS_Total__ActualSales_' + tabIndex + '_' + colIndex).attr(reValue));
        var varianceSalesByCOGS = actualSalesByCOGS - projectionSalesByCOGS;
        var cogsItem = {
            ProjectionSales: projectionSalesByCOGS,
            ProjectionPercent: $.formatNumber($('#COGS_Total__ProjectionPercent_' + tabIndex + '_' + colIndex).attr(reValue)),
            ActualSales: actualSalesByCOGS,
            ActualPercent: $.formatNumber($('#COGS_Total__ActualPercent_' + tabIndex + '_' + colIndex).attr(reValue)),
            VarianceSales: varianceSalesByCOGS,
            VariancePercent: (projectionSalesByCOGS == 0) ? 0 : varianceSalesByCOGS * 100 / projectionSalesByCOGS,
        };

        // set data to COGS: row index is 1
        ProfitLoss.SetDataToProfitLossItem(tabIndex, colIndex, 1, cogsItem);

        // get Target value by tab & column index
        var targetSales = $.formatNumber($('#Target_Sales_' + tabIndex + '_' + colIndex).attr(reValue));

        // get gross profit to row index is 2
        var grossSales = projectionSalesBySales - projectionSalesByCOGS;
        var grossActual = actualSalesBySales - actualSalesByCOGS;
        var grossVariance = grossActual - grossSales;
        var grossItem = {
            ProjectionSales: grossSales,
            ProjectionPercent: targetSales == 0 ? 0 : grossSales * 100 / targetSales,
            ActualSales: grossActual,
            ActualPercent: actualSalesBySales == 0 ? 0 : grossActual * 100 / actualSalesBySales,
            VarianceSales: grossVariance,
            VariancePercent: grossSales == 0 ? 0 : grossVariance * 100 / grossSales,
        };

        // set gross profit to row index is 2
        ProfitLoss.SetDataToProfitLossItem(tabIndex, colIndex, 2, grossItem);

        // get data to Payroll Expenses to row index is 3
        var projectionSalesByPayroll = $.formatNumber($('#All_Payroll_Expenses_Total__ProjectionSales_' + tabIndex + '_' + colIndex).attr(reValue));
        var actualSalesByPayroll = $.formatNumber($('#All_Payroll_Expenses_Total__ActualSales_' + tabIndex + '_' + colIndex).attr(reValue));
        var varianceSalesByPayroll = actualSalesByPayroll - projectionSalesByPayroll;
        var payrollItem = {
            ProjectionSales: projectionSalesByPayroll,
            ProjectionPercent: $.formatNumber($('#All_Payroll_Expenses_Total__ProjectionPercent_' + tabIndex + '_' + colIndex).attr(reValue)),
            ActualSales: actualSalesByPayroll,
            ActualPercent: actualSalesBySales == 0 ? 0 : actualSalesByPayroll * 100 / actualSalesBySales,
            VarianceSales: varianceSalesByPayroll,
            VariancePercent: (projectionSalesByPayroll == 0) ? 0 : varianceSalesByPayroll * 100 / projectionSalesByPayroll,
        };

        // set Payroll Expenses to row index is 3
        ProfitLoss.SetDataToProfitLossItem(tabIndex, colIndex, 3, payrollItem);

        // get operating profit to row index is 4
        var operatingSales = grossSales - projectionSalesByPayroll;
        var operatingActual = grossActual - actualSalesByPayroll;
        var operatingVariance = operatingActual - operatingSales;
        var operatingItem = {
            ProjectionSales: operatingSales,
            ProjectionPercent: targetSales == 0 ? 0 : operatingSales * 100 / targetSales,
            ActualSales: operatingActual,
            ActualPercent: actualSalesBySales == 0 ? 0 : operatingActual * 100 / actualSalesBySales,
            VarianceSales: operatingVariance,
            VariancePercent: (operatingSales == 0) ? 0 : operatingVariance * 100 / operatingSales,
        };

        // set operating profit to row index is 4
        ProfitLoss.SetDataToProfitLossItem(tabIndex, colIndex, 4, operatingItem);

        // get prime cost to row index is 5
        var primeCostData = ProfitLoss.GetTotalPrimeCostByColumn(tabIndex, colIndex);
        var primeCostSales = projectionSalesByCOGS + primeCostData.ProjectionSales;
        var primeCostActual = actualSalesByCOGS + primeCostData.ActualSales;
        var primeCostVariance = primeCostActual - primeCostSales;
        var primeCostItem = {
            ProjectionSales: primeCostSales,
            ProjectionPercent: targetSales == 0 ? 0 : primeCostSales * 100 / targetSales,
            ActualSales: primeCostActual,
            ActualPercent: actualSalesBySales == 0 ? 0 : primeCostActual * 100 / actualSalesBySales,
            VarianceSales: primeCostVariance,
            VariancePercent: (primeCostSales == 0) ? 0 : primeCostVariance * 100 / primeCostSales,
        };

        // set prime cost to row index is 5
        ProfitLoss.SetDataToProfitLossItem(tabIndex, colIndex, 5, primeCostItem);

        // get operation expenses value to row index is 6
        var projectionSalesByOperation = $.formatNumber($('#Operation_Expenses_Total__ProjectionSales_' + tabIndex + '_' + colIndex).attr(reValue));
        var actualSalesByOperation = $.formatNumber($('#Operation_Expenses_Total__ActualSales_' + tabIndex + '_' + colIndex).attr(reValue));
        var varianceSalesByOperation = actualSalesByOperation - projectionSalesByOperation;
        var operationItem = {
            ProjectionSales: projectionSalesByOperation,
            ProjectionPercent: $.formatNumber($('#Operation_Expenses_Total__ProjectionPercent_' + tabIndex + '_' + colIndex).attr(reValue)),
            ActualSales: actualSalesByOperation,
            ActualPercent: actualSalesBySales == 0 ? 0 : actualSalesByOperation * 100 / actualSalesBySales,
            VarianceSales: varianceSalesByOperation,
            VariancePercent: (projectionSalesByOperation == 0) ? 0 : varianceSalesByOperation * 100 / projectionSalesByOperation,
        };

        // set operation expenses value to row index is 6
        ProfitLoss.SetDataToProfitLossItem(tabIndex, colIndex, 6, operationItem);

        // get net profit loss to row index is 7
        var netProfitSales = operatingSales - projectionSalesByOperation;
        var netProfitActual = operatingActual - actualSalesByOperation;
        var netProfitVariance = netProfitActual - netProfitSales;
        var netProfitItem = {
            ProjectionSales: netProfitSales,
            ProjectionPercent: targetSales == 0 ? 0 : netProfitSales * 100 / targetSales,
            ActualSales: netProfitActual,
            ActualPercent: actualSalesBySales == 0 ? 0 : netProfitActual * 100 / actualSalesBySales,
            VarianceSales: netProfitVariance,
            VariancePercent: (netProfitSales == 0) ? 0 : netProfitVariance * 100 / netProfitSales,
        };

        // set operation value to row index is 7
        ProfitLoss.SetDataToProfitLossItem(tabIndex, colIndex, 7, netProfitItem);
        ProfitLoss.SetDataToProfitLossItemHeader(tabIndex, colIndex, netProfitItem)

        // get net profit runing total to row index is 8
        ProfitLoss.FlagRuningTotalProfit = true;
        setTimeout(function () {
            if (ProfitLoss.FlagRuningTotalProfit) {
                var netProfitRuningSales = 0;
                var netProfitRuningActual = 0;
                var netProfitRuningVariance = 0;
                // calculate row profit loss runing
                var headerColumnLenght = $('.active:last .header-name').length;
                for (var i = 0; i <= headerColumnLenght; i++) {
                    netProfitRuningSales += $.formatNumber($('#Profit_Loss_ProjectionSales_' + tabIndex + '_' + i + '_' + 7).attr(reValue));
                    netProfitRuningActual += $.formatNumber($('#Profit_Loss_ActualSales_' + tabIndex + '_' + i + '_' + 7).attr(reValue));
                    netProfitRuningVariance = netProfitRuningActual - netProfitRuningSales;

                    var targetSalesRuning = $.formatNumber($('#Target_Sales_' + tabIndex + '_' + i).attr(reValue));
                    var actualSalesBySalesRuning = $.formatNumber($('#Total__ActualSales_' + tabIndex + '_' + i).attr(reValue));
                    var netProfitRuningItem = {
                        ProjectionSales: netProfitRuningSales,
                        ProjectionPercent: targetSalesRuning == 0 ? 0 : netProfitRuningSales * 100 / targetSalesRuning,
                        ActualSales: netProfitRuningActual,
                        ActualPercent: actualSalesBySalesRuning == 0 ? 0 : netProfitRuningActual * 100 / actualSalesBySalesRuning,
                        VarianceSales: netProfitRuningVariance,
                        VariancePercent: (netProfitRuningSales == 0) ? 0 : netProfitRuningVariance * 100 / netProfitRuningSales,
                    };

                    // set operation value to row index is 8
                    ProfitLoss.SetDataToProfitLossItem(tabIndex, i, 8, netProfitRuningItem);
                }
            }

            // reset flag calculate runing total
            ProfitLoss.FlagRuningTotalProfit = false;
        }, 2000);

        // get Breakeven Point value to row index is 9
        var bepData = ProfitLoss.GetFixCostAndVariableCost(tabIndex, colIndex);
        var bepSales = targetSales == 0 ? 0 : bepData.FixCost / (1 - bepData.VariableCost / targetSales);
        var bepItem = {
            ProjectionSales: bepSales,
            ProjectionPercent: targetSales == 0 ? 0 : bepSales * 100 / targetSales,
            ActualSales: 0,
            ActualPercent: 0,
            VarianceSales: 0,
            VariancePercent: 0,
        };

        // set Breakeven Point value to row index is 9
        ProfitLoss.SetDataToProfitLossItem(tabIndex, colIndex, 9, bepItem);

        // call calculate grand total row
        ProfitLoss.FlagCalculateGrandTotal = true;
        setTimeout(function () {
            if (ProfitLoss.FlagCalculateGrandTotal) {
                ProfitLoss.CalcProfitLossGrandTotal(tabIndex);
            }
            ProfitLoss.FlagCalculateGrandTotal = false;
        }, 2000);
    },

    // function common set data item to profit loss item by column & row
    SetDataToProfitLossItem: function (tabIndex, colIndex, rowIndex, dataItem) {
        // set data Sales Total to row Sales Of Profit Loss section
        var $projectionSales = $('#Profit_Loss_ProjectionSales_' + tabIndex + '_' + colIndex + '_' + rowIndex);
        $projectionSales.val($.formatCurrency(dataItem.ProjectionSales));
        $projectionSales.attr(reValue, dataItem.ProjectionSales);

        var $projectionPercent = $('#Profit_Loss_ProjectionPercent_' + tabIndex + '_' + colIndex + '_' + rowIndex);
        $projectionPercent.val($.formatPercent(dataItem.ProjectionPercent));
        $projectionPercent.attr(reValue, dataItem.ProjectionPercent);

        var $actualSales = $('#Profit_Loss_ActualSales_' + tabIndex + '_' + colIndex + '_' + rowIndex);
        $actualSales.val($.formatCurrency(dataItem.ActualSales));
        $actualSales.attr(reValue, dataItem.ActualSales);

        var $actualPercent = $('#Profit_Loss_ActualPercent_' + tabIndex + '_' + colIndex + '_' + rowIndex);
        $actualPercent.val($.formatPercent(dataItem.ActualPercent));
        $actualPercent.attr(reValue, dataItem.ActualPercent);

        var $varianceSales = $('#Profit_Loss_VarianceSales_' + tabIndex + '_' + colIndex + '_' + rowIndex);
        $varianceSales.val($.formatCurrency(dataItem.VarianceSales));
        $varianceSales.attr(reValue, dataItem.VarianceSales);

        var $variancePercent = $('#Profit_Loss_VariancePercent_' + tabIndex + '_' + colIndex + '_' + rowIndex);
        $variancePercent.val($.formatPercent(dataItem.VariancePercent));
        $variancePercent.attr(reValue, dataItem.VariancePercent);

        // set color to item variance sales and reference item variance percent & actual percent
        $.setColorToItem($varianceSales, $variancePercent, $actualPercent);
    },
    SetDataToProfitLossItemHeader: function (tabIndex, colIndex, dataItem) {
        // set data Sales Total to row Sales Of Profit Loss section
        var $projectionSales = $('#Profit_Loss_Total__ProjectionSales_' + tabIndex + '_' + colIndex);
        $projectionSales.val($.formatCurrency(dataItem.ProjectionSales));
        $projectionSales.attr(reValue, dataItem.ProjectionSales);

        var $projectionPercent = $('#Profit_Loss_Total__ProjectionPercent_' + tabIndex + '_' + colIndex);
        $projectionPercent.val($.formatPercent(dataItem.ProjectionPercent));
        $projectionPercent.attr(reValue, dataItem.ProjectionPercent);

        var $actualSales = $('#Profit_Loss_Total__ActualSales_' + tabIndex + '_' + colIndex);
        $actualSales.val($.formatCurrency(dataItem.ActualSales));
        $actualSales.attr(reValue, dataItem.ActualSales);

        var $actualPercent = $('#Profit_Loss_Total__ActualPercent_' + tabIndex + '_' + colIndex);
        $actualPercent.val($.formatPercent(dataItem.ActualPercent));
        $actualPercent.attr(reValue, dataItem.ActualPercent);

        var $varianceSales = $('#Profit_Loss_Total__VarianceSales_' + tabIndex + '_' + colIndex);
        $varianceSales.val($.formatCurrency(dataItem.VarianceSales));
        $varianceSales.attr(reValue, dataItem.VarianceSales);

        var $variancePercent = $('#Profit_Loss_Total__VariancePercent_' + tabIndex + '_' + colIndex);
        $variancePercent.val($.formatPercent(dataItem.VariancePercent));
        $variancePercent.attr(reValue, dataItem.VariancePercent);

        // set color to item variance sales and reference item variance percent & actual sales & actual percent
        $.setColorToItem($varianceSales, $variancePercent, $actualPercent, $actualSales);
    },

    // function calculate profit loss grand total by tab
    CalcProfitLossGrandTotal: function (tabIndex) {
        console.log('call calculate grand total row');

        // get data Grand Sales Total by tab index
        var projectionSalesBySales = $.formatNumber($('#GrandTotal__ProjectionSales_' + tabIndex).attr(reValue));
        var actualSalesBySales = $.formatNumber($('#GrandTotal__ActualSales_' + tabIndex).attr(reValue));
        var varianceSalesBySales = actualSalesBySales - projectionSalesBySales;
        var salesItem = {
            ProjectionSales: projectionSalesBySales,
            ProjectionPercent: projectionSalesBySales == 0 ? 0 : 100,
            ActualSales: actualSalesBySales,
            ActualPercent: (projectionSalesBySales == 0) ? 0 : actualSalesBySales * 100 / projectionSalesBySales,
            VarianceSales: varianceSalesBySales,
            VariancePercent: (projectionSalesBySales == 0) ? 0 : varianceSalesBySales * 100 / projectionSalesBySales,
        };

        // set data to Sales: row index is 0
        ProfitLoss.SetDataToProfitLossGrandTotal(tabIndex, 0, salesItem);

        // get data Grand Sales Total by tab index
        var projectionSalesByCOGS = $.formatNumber($('#COGS_GrandTotal__ProjectionSales_' + tabIndex).attr(reValue));
        var actualSalesByCOGS = $.formatNumber($('#COGS_GrandTotal__ActualSales_' + tabIndex).attr(reValue));
        var varianceSalesByCOGS = actualSalesByCOGS - projectionSalesByCOGS;
        var salesItem = {
            ProjectionSales: projectionSalesByCOGS,
            ProjectionPercent: $.formatNumber($('#COGS_GrandTotal__ProjectionPercent_' + tabIndex).attr(reValue)),
            ActualSales: actualSalesByCOGS,
            ActualPercent: $.formatNumber($('#COGS_GrandTotal__ActualPercent_' + tabIndex).attr(reValue)),
            VarianceSales: varianceSalesByCOGS,
            VariancePercent: (projectionSalesByCOGS == 0) ? 0 : varianceSalesByCOGS * 100 / projectionSalesByCOGS,
        };

        // set data to COGS: row index is 1
        ProfitLoss.SetDataToProfitLossGrandTotal(tabIndex, 1, salesItem);

        // get data to gross profit
        var grossGrandTotalSales = projectionSalesBySales - projectionSalesByCOGS;
        var grossGrandTotalActual = actualSalesBySales - actualSalesByCOGS;
        var grossGrandTotalVariance = grossGrandTotalActual - grossGrandTotalSales;
        var grossItem = {
            ProjectionSales: grossGrandTotalSales,
            ProjectionPercent: projectionSalesBySales == 0 ? 0 : grossGrandTotalSales * 100 / projectionSalesBySales,
            ActualSales: grossGrandTotalActual,
            ActualPercent: grossGrandTotalSales == 0 ? 0 : grossGrandTotalActual * 100 / grossGrandTotalSales,
            VarianceSales: grossGrandTotalVariance,
            VariancePercent: (grossGrandTotalSales == 0) ? 0 : grossGrandTotalVariance * 100 / grossGrandTotalSales,
        };

        // set data to COGS: row index is 2
        ProfitLoss.SetDataToProfitLossGrandTotal(tabIndex, 2, grossItem);

        // get data to Payroll
        var payrollGrandTotalSales = $.formatNumber($('#All_Payroll_Expenses_GrandTotal__ProjectionSales_' + tabIndex).attr(reValue));
        var payrollGrandTotalActual = $.formatNumber($('#All_Payroll_Expenses_GrandTotal__ActualSales_' + tabIndex).attr(reValue));
        var payrollGrandTtoalVariance = payrollGrandTotalActual - payrollGrandTotalSales;
        var payrollItem = {
            ProjectionSales: payrollGrandTotalSales,
            ProjectionPercent: projectionSalesBySales == 0 ? 0 : payrollGrandTotalSales * 100 / projectionSalesBySales,
            ActualSales: payrollGrandTotalActual,
            ActualPercent: payrollGrandTotalSales == 0 ? 0 : payrollGrandTotalActual * 100 / payrollGrandTotalSales,
            VarianceSales: payrollGrandTtoalVariance,
            VariancePercent: (payrollGrandTotalSales == 0) ? 0 : payrollGrandTtoalVariance * 100 / payrollGrandTotalSales,
        };

        // set data to Payroll: row index is 3
        ProfitLoss.SetDataToProfitLossGrandTotal(tabIndex, 3, payrollItem);

        // get data to operating profit: row index is 4
        var operatingGrandTotalSales = projectionSalesBySales - projectionSalesByCOGS - payrollGrandTotalSales;
        var operatingGrandTotalActual = actualSalesBySales - actualSalesByCOGS - payrollGrandTotalActual;
        var operatingGrandTotalVariance = operatingGrandTotalActual - operatingGrandTotalSales;
        var operatingItem = {
            ProjectionSales: operatingGrandTotalSales,
            ProjectionPercent: projectionSalesBySales == 0 ? 0 : operatingGrandTotalSales * 100 / projectionSalesBySales,
            ActualSales: operatingGrandTotalActual,
            ActualPercent: operatingGrandTotalSales == 0 ? 0 : operatingGrandTotalActual * 100 / operatingGrandTotalSales,
            VarianceSales: operatingGrandTotalVariance,
            VariancePercent: (operatingGrandTotalSales == 0) ? 0 : operatingGrandTotalVariance * 100 / operatingGrandTotalSales,
        };

        // set data to Payroll: row index is 4
        ProfitLoss.SetDataToProfitLossGrandTotal(tabIndex, 4, operatingItem);

        // get data to Prime Cost: row index is 5
        var primeCostData = ProfitLoss.GetGrandTotalPrimeCost();
        var primeCostGrandTotalSales = projectionSalesByCOGS + primeCostData.ProjectionSales;
        var primeCostGrandTotalActual = actualSalesByCOGS + primeCostData.ActualSales;
        var primeCostGrandTtoalVariance = primeCostGrandTotalActual - primeCostGrandTotalSales;
        var primeCostItem = {
            ProjectionSales: primeCostGrandTotalSales,
            ProjectionPercent: projectionSalesBySales == 0 ? 0 : primeCostGrandTotalSales * 100 / projectionSalesBySales,
            ActualSales: primeCostGrandTotalActual,
            ActualPercent: primeCostGrandTotalSales == 0 ? 0 : primeCostGrandTotalActual * 100 / primeCostGrandTotalSales,
            VarianceSales: primeCostGrandTtoalVariance,
            VariancePercent: (primeCostGrandTotalSales == 0) ? 0 : primeCostGrandTtoalVariance * 100 / primeCostGrandTotalSales,
        };

        // set data to Payroll: row index is 5
        ProfitLoss.SetDataToProfitLossGrandTotal(tabIndex, 5, primeCostItem);

        // set data to Operation: row index is 6
        var operationGrandTotalSales = $.formatNumber($('#Operation_Expenses_GrandTotal__ProjectionSales_' + tabIndex).attr(reValue));
        var operationGrandTotalActual = $.formatNumber($('#Operation_Expenses_GrandTotal__ActualSales_' + tabIndex).attr(reValue));
        var operationGrandTtoalVariance = operationGrandTotalActual - operationGrandTotalSales;
        var operationItem = {
            ProjectionSales: operationGrandTotalSales,
            ProjectionPercent: projectionSalesBySales == 0 ? 0 : operationGrandTotalSales * 100 / projectionSalesBySales,
            ActualSales: operationGrandTotalActual,
            ActualPercent: actualSalesBySales == 0 ? 0 : operationGrandTotalActual * 100 / actualSalesBySales,
            VarianceSales: operationGrandTtoalVariance,
            VariancePercent: (operationGrandTotalSales == 0) ? 0 : operationGrandTtoalVariance * 100 / operationGrandTotalSales,
        };

        // set data to Operation: row index is 6
        ProfitLoss.SetDataToProfitLossGrandTotal(tabIndex, 6, operationItem);

        // get data to Net Profit: row index is 7
        var netProfitGrandTotalSales = operatingGrandTotalSales - operationGrandTotalSales;
        var netProfitGrandTotalActual = operatingGrandTotalActual - operationGrandTotalActual;
        var netProfitGrandTtoalVariance = netProfitGrandTotalActual - netProfitGrandTotalSales;
        var netProfitItem = {
            ProjectionSales: netProfitGrandTotalSales,
            ProjectionPercent: projectionSalesBySales == 0 ? 0 : netProfitGrandTotalSales * 100 / projectionSalesBySales,
            ActualSales: netProfitGrandTotalActual,
            ActualPercent: netProfitGrandTotalSales == 0 ? 0 : netProfitGrandTotalActual * 100 / netProfitGrandTotalSales,
            VarianceSales: netProfitGrandTtoalVariance,
            VariancePercent: (netProfitGrandTotalSales == 0) ? 0 : netProfitGrandTtoalVariance * 100 / netProfitGrandTotalSales,
        };

        // set data to Net Profit: row index is 7
        ProfitLoss.SetDataToProfitLossGrandTotal(tabIndex, 7, netProfitItem);
        ProfitLoss.SetDataToProfitLossGrandTotalHeader(tabIndex, netProfitItem);

        // set data to Net Profit Runing Total: row index is 8
        ProfitLoss.SetDataToProfitLossGrandTotal(tabIndex, 8, netProfitItem);

        // get data to Net Profit Runing Total: row index is 9
        var bepProjectionSales = 0;
        $('.active:last .header-name').each(function (i, v) {
            bepProjectionSales += $.formatNumber($('#Profit_Loss_ProjectionSales_' + tabIndex + '_' + i + '_9').attr(reValue));
        });
        var bepItem = {
            ProjectionSales: bepProjectionSales,
            ProjectionPercent: projectionSalesBySales == 0 ? 0 : bepProjectionSales * 100 / projectionSalesBySales,
            ActualSales: 0,
            ActualPercent: 0,
            VarianceSales: 0,
            VariancePercent: 0,
        };

        // set data to Net Profit Runing Total: row index is 9
        ProfitLoss.SetDataToProfitLossGrandTotal(tabIndex, 9, bepItem);
    },

    // function common set data item to grand total profit loss
    SetDataToProfitLossGrandTotal: function (tabIndex, rowIndex, dataItem) {
        // set data Sales Total to row Sales Of Profit Loss section
        var $projectionSales = $('#Profit_Loss_GrandTotalRow_ProjectionSales_' + tabIndex + '_' + rowIndex);
        $projectionSales.val($.formatCurrency(dataItem.ProjectionSales));
        $projectionSales.attr(reValue, dataItem.ProjectionSales);

        var $projectionPercent = $('#Profit_Loss_GrandTotalRow_ProjectionPercent_' + tabIndex + '_' + rowIndex);
        $projectionPercent.val($.formatPercent(dataItem.ProjectionPercent));
        $projectionPercent.attr(reValue, dataItem.ProjectionPercent);

        var $actualSales = $('#Profit_Loss_GrandTotalRow_ActualSales_' + tabIndex + '_' + rowIndex);
        $actualSales.val($.formatCurrency(dataItem.ActualSales));
        $actualSales.attr(reValue, dataItem.ActualSales);

        var $actualPercent = $('#Profit_Loss_GrandTotalRow_ActualPercent_' + tabIndex + '_' + rowIndex);
        $actualPercent.val($.formatPercent(dataItem.ActualPercent));
        $actualPercent.attr(reValue, dataItem.ActualPercent);

        var $varianceSales = $('#Profit_Loss_GrandTotalRow_VarianceSales_' + tabIndex + '_' + rowIndex);
        $varianceSales.val($.formatCurrency(dataItem.VarianceSales));
        $varianceSales.attr(reValue, dataItem.VarianceSales);

        var $variancePercent = $('#Profit_Loss_GrandTotalRow_VariancePercent_' + tabIndex + '_' + rowIndex);
        $variancePercent.val($.formatPercent(dataItem.VariancePercent));
        $variancePercent.attr(reValue, dataItem.VariancePercent);

        // set color to item variance sales and reference item variance percent & actual percent
        $.setColorToItem($varianceSales, $variancePercent, $actualPercent);
    },
    SetDataToProfitLossGrandTotalHeader: function (tabIndex, dataItem) {
        // set data Sales Total to row Sales Of Profit Loss section
        var $projectionSales = $('#Profit_Loss_GrandTotal__ProjectionSales_' + tabIndex);
        $projectionSales.val($.formatCurrency(dataItem.ProjectionSales));
        $projectionSales.attr(reValue, dataItem.ProjectionSales);

        var $projectionPercent = $('#Profit_Loss_GrandTotal__ProjectionPercent_' + tabIndex);
        $projectionPercent.val($.formatPercent(dataItem.ProjectionPercent));
        $projectionPercent.attr(reValue, dataItem.ProjectionPercent);

        var $actualSales = $('#Profit_Loss_GrandTotal__ActualSales_' + tabIndex);
        $actualSales.val($.formatCurrency(dataItem.ActualSales));
        $actualSales.attr(reValue, dataItem.ActualSales);

        var $actualPercent = $('#Profit_Loss_GrandTotal__ActualPercent_' + tabIndex);
        $actualPercent.val($.formatPercent(dataItem.ActualPercent));
        $actualPercent.attr(reValue, dataItem.ActualPercent);

        var $varianceSales = $('#Profit_Loss_GrandTotal__VarianceSales_' + tabIndex);
        $varianceSales.val($.formatCurrency(dataItem.VarianceSales));
        $varianceSales.attr(reValue, dataItem.VarianceSales);

        var $variancePercent = $('#Profit_Loss_GrandTotal__VariancePercent_' + tabIndex);
        $variancePercent.val($.formatPercent(dataItem.VariancePercent));
        $variancePercent.attr(reValue, dataItem.VariancePercent);

        // set color to item variance sales and reference item variance percent & actual sales & actual percent
        $.setColorToItem($varianceSales, $variancePercent, $actualPercent, $actualSales);
    },

    // function get prime cost by column
    GetTotalPrimeCostByColumn: function (tabIndex, colIndex) {
        var sales = 0;
        var actual = 0;
        var $currentTab = $('.active:last');
        $currentTab.find('input[name="IsPrimeCost"][value="true"]').each(function () {
            var categorySettingId = $(this).parent().attr('category-setting-id');

            // get data row by category
            var $dataRowByCategory = $currentTab.find('div[category-setting-id="' + categorySettingId + '"]:last');

            // get projection sales
            var $projectionSalesItem = $dataRowByCategory.find('input[id*="_ProjectionSales_' + tabIndex + '_' + colIndex + '_"]');
            sales += $.formatNumber($projectionSalesItem.attr('re-value'));

            // get actual sales
            var $actualSalesItem = $dataRowByCategory.find('input[id*="_ActualSales_' + tabIndex + '_' + colIndex + '_"]');
            actual += $.formatNumber($actualSalesItem.attr('re-value'));
        });

        // return prime cost item
        return {
            ProjectionSales: sales,
            ActualSales: actual,
        };
    },

    // function get grand prime cost
    GetGrandTotalPrimeCost: function () {
        var sales = 0;
        var actual = 0;
        var $currentTab = $('.active:last');
        $currentTab.find('input[name="IsPrimeCost"][value="true"]').each(function () {
            var categorySettingId = $(this).parent().attr('category-setting-id');

            // get projection sales
            var $projectionSalesItem = $currentTab.find('input[category-setting-id="' + categorySettingId + '"]');
            sales += $.formatNumber($projectionSalesItem.attr('re-value'));

            // get actual sales
            var $actualSalesItem = $('#' + $projectionSalesItem.attr('id').replace('Projection', 'Actual'));
            actual += $.formatNumber($actualSalesItem.attr('re-value'));
        });

        // return prime cost item
        return {
            ProjectionSales: sales,
            ActualSales: actual,
        };
    },

    // function get fix cost & variable cost by column
    GetFixCostAndVariableCost: function (tabIndex, colIndex) {
        var variableCost = 0;
        var fixCost = 0;

        $('[id^=COGS_IsPercentage_' + tabIndex + '_' + colIndex + '_]').each(function (i, item) {
            var $IsPercentageItem = $(item);
            // is percentage true, sum sales to variable cost
            if (JSON.parse($IsPercentageItem.val().toLowerCase())) {
                variableCost += $.formatNumber($IsPercentageItem.next().attr(reValue));
            } else {
                fixCost += $.formatNumber($IsPercentageItem.next().attr(reValue));
            }
        });

        $('[id^=Payroll_Expenses_IsPercentage_' + tabIndex + '_' + colIndex + '_]').each(function (i, item) {
            var $IsPercentageItem = $(item);
            // is percentage true, sum sales to variable cost
            if (JSON.parse($IsPercentageItem.val().toLowerCase())) {
                variableCost += $.formatNumber($IsPercentageItem.next().attr(reValue));
            } else {
                fixCost += $.formatNumber($IsPercentageItem.next().attr(reValue));
            }
        });

        $('[id^=Operation_Expenses_IsPercentage_' + tabIndex + '_' + colIndex + '_]').each(function (i, item) {
            var $IsPercentageItem = $(item);
            // is percentage true, sum sales to variable cost
            if (JSON.parse($IsPercentageItem.val().toLowerCase())) {
                variableCost += $.formatNumber($IsPercentageItem.next().attr(reValue));
            } else {
                fixCost += $.formatNumber($IsPercentageItem.next().attr(reValue));
            }
        });

        // return fix cost and variable cost
        return {
            VariableCost: variableCost,
            FixCost: fixCost,
        };
    }
}

$.setColorToItem = function (item, ref1, ref2, ref3) {
    var newColor = '';
    if (item.val() == '$0.00' || item.val() == '($0.00)' || item.val() == '0.00') {
        newColor = 'black';
    } else {
        if (item.attr(reValue) > 0) {
            newColor = 'green';
        } else {
            newColor = 'red';
        }
    }

    // set color to item
    item.css('color', newColor);

    // set color to item reference
    if (ref1)
        ref1.css('color', newColor);
    if (ref2)
        ref2.css('color', newColor);
    if(ref3)
        ref3.css('color', newColor);
}

// reCalculate data row after change ref category setting id on COGS section
$.reCalculateAfterChangeCategoryRefId = function (cogsCategoryId, salesCategoryId) {
    // get all data row by category
    $('.data-row-by-category[category-setting-id="' + cogsCategoryId + '"]').each(function () {
        var $dataRow = $(this);
        $dataRow.find('.numerictextbox').each(function () {
            var itemName = $(this).attr('id');
            if (itemName.indexOf("Actual") != -1) {
                // call change actual sales
                SectionCOGS.reCalcCogsActualPercent(itemName);
            } else {
                // call change projection item
                SectionCOGS.reCalcCogsProjectionItem(itemName);
            }
        });
    });
}

// common read data in tab
function readBudgetItemByTab(tab) {
    // get annual sales by tab
    var annualSales = $.formatNumberSave($(tab).find('[id^="Annual_Sales_"]').attr(reValue));

    // get header column
    var headerColumnList = [];
    $(tab).find('.header-row').each(function () {
        $(this).find('span[name^="HeaderName_"]').each(function () {
            headerColumnList.push($(this).html());
        });
    });

    // get target column
    var targetColumnList = [];
    $(tab).find('.target-row').each(function () {
        $(this).find('[id^="Target_Sales_"]').each(function () {
            targetColumnList.push({
                TargetSales: $.formatNumberSave($(this).attr(reValue)),
                TargetPercent: $.formatNumberSave($(this).next().attr(reValue)),
            });
        });
    });

    // get projection & actual
    var budgetItemModelList = [];
    $(tab).find('.data-row-by-category').each(function () {
        var budgetItemDetailList = [];
        $(this).find('.projection-data').each(function () {
            var budgetItemDetail = {
                IsPercentage: $(this).find('[id*="IsPercentage"]').val(),
                ProjectionSales: $.formatNumberSave($(this).find('[id*="ProjectionSales"]').attr(reValue)),
                ProjectionPercent: $.formatNumberSave($(this).find('[id*="ProjectionPercent"]').attr(reValue)),
                ActualSales: $.formatNumberSave($(this).next().find('[id*="ActualSales"]').attr(reValue)),
                ActualPercent: $.formatNumberSave($(this).next().find('[id*="ActualPercent"]').attr(reValue)),
            };
            // add budget item detail to budget item detail list
            budgetItemDetailList.push(budgetItemDetail);
        });

        var categorySettingId = $(this).attr('category-setting-id');
        var sectionName = $(this).attr("section-name");
        var isTaxCost = false;
        if (sectionName == "IsTax_Payroll_Expenses_") {
            isTaxCost = true;
        }

        // get isPrimeCost
        var isPrimeCost = false;
        if (sectionName.indexOf("Payroll_Expenses") != -1) {
            isPrimeCost = $(tab).find('div[category-setting-id="' + categorySettingId + '"]:first > input[name="IsPrimeCost"]').val();
        }

        var budgetItemModel = {
            BudgetItemId: $(this).attr('budget-item-id'),
            CategorySettingId: categorySettingId,
            CategoryName: $(this).attr('category-name'),
            ParentCategoryId: $(this).attr('parent-category-id'),
            ParentCategoryName: $(this).attr('parent-category-name'),
            SalesCategoryRefId: $(this).attr('sales-category-ref-id'),
            BudgetItemList: budgetItemDetailList,
            IsTaxCost: isTaxCost,
            IsPrimeCost: isPrimeCost,
        };

        // add budget item model to budget item model list
        budgetItemModelList.push(budgetItemModel);
    });

    // return budget item list in tab
    return {
        BudgetTabId: $(tab).attr("budgetTabId"),
        TabIndex: $(tab).attr("budget-tab-index"),
        TabName: $(tab).attr("budget-tab-name"),
        AnnualSales: annualSales,
        HeaderColumnList: headerColumnList,
        TargetColumnList: targetColumnList,
        BudgetItemModelList: budgetItemModelList,
    };
}

// common save change budget
function SaveBudgetDetails(url) {
    if ($.isChangeDataInPage == false) {
        warningPopup("Warning", "Budget Details has no change.");
        return;
    }

    if ($("#BudgetId").val() == 0) {
        warningPopup("Warning", "Cannot save change data, budget is empty.")
        return;
    }

    showProcessing(60000);
    setTimeout(function () {
        var budgetTabModelList = [];
        $('.tab-area-by-section').each(function () {
            budgetTabModelList.push(readBudgetItemByTab(this));
        });

        var BudgetId = parseFloat($("#BudgetId").val());
        var InputMethod = parseFloat($('#InputMethod').val());
        var ActualNumbersFlg = $('input[name="ActualNumbersFlg"]').prop('checked');
        var VarianceFlg = $('input[name="VarianceFlg"]').prop('checked');

        // call action save data
        $.ajax({
            url: '/Budget/SaveBudgetDetails',
            type: "POST",
            data: JSON.stringify({ budgetTabModelList: budgetTabModelList, budgetId: BudgetId, inputMethod: InputMethod, actualNumbersFlg: ActualNumbersFlg, varianceFlg: VarianceFlg }),
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            async: false,
            success: function (response) {
                if (response.Status) {
                    // set no show confirm ridirect page
                    $.isChangeDataInPage = false;

                    // cancel popup
                    if (url.length > 0) {
                        window.location = url;
                    } else {
                        warningPopup("Save Change Budget", response.Message);
                        hideProcessing();
                    }
                } else {
                    warningPopup("Warning", "Save Change Budget falied.");
                }
            },
            error: function (e) {
                console.log(e);
            }
        });
    }, 500);
}

// common show confirm before redirect page
function confirmBeforeRedirectPage(url) {
    if ($.isChangeDataInPage == false) {
        window.location = url;
    } else {
        var options = {
            id: "ConfirmBeforeRedirect",
            typeStatus: true,
            title: "Confirmation",
            confirmText: "Do you want to save your changes?",
            textYes: "Save",
            textNo: "Cancel"
        }
        var popupWindow = window.setaJs.initPopupWindow(options);
        popupWindow.refresh({}).center().open();
        popupWindow.center().open();

        var $form = $('#' + options.id);
        $('#ConfirmBeforeRedirect .confirm-footer-popHD button:first').after('<button class="btn btn-primary" id="act-accept-dont-save">Don&prime;t Save</button>');

        $form.find("#act-accept-no").on("click", function () {
            if (popupWindow) popupWindow.close();
        });

        $form.find("#act-accept-dont-save").on("click", function () {
            if (popupWindow) popupWindow.close();

            // set no show confirm ridirect page
            $.isChangeDataInPage = false;
            window.location = url;
        });

        $form.find("#act-accept-yes").on("click", function () {
            if (popupWindow) popupWindow.close();

            // call save change budget
            SaveBudgetDetails(url);
        });
    }
}

// common save data to session, after redirect to review page
function saveDataToSessionAfterRedirectToReviewPage() {
    var BudgetId = parseFloat($("#BudgetId").val());
    var InputMethod = parseFloat($('#InputMethod').val());
    var ActualNumbersFlg = $('input[name="ActualNumbersFlg"]').prop('checked');
    var VarianceFlg = $('input[name="VarianceFlg"]').prop('checked');

    var budgetTabModelList = [];
    $('.tab-area-by-section').each(function () {
        budgetTabModelList.push(readBudgetItemByTab(this));
    });

    // call copy item to server
    $.ajax({
        url: '/Budget/CopyItemBudget',
        type: "POST",
        data: JSON.stringify({ budgetTabModelList: budgetTabModelList, budgetId: BudgetId, inputMethod: InputMethod, actualNumbersFlg: ActualNumbersFlg, varianceFlg: VarianceFlg }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        async: false,
        success: function (response) {
            if (response.Status) {
                // set no show confirm ridirect page
                $.isChangeDataInPage = false;

                // redirect to review page
                redirectToReview();
            } else {
                warningPopup("Warning", "Change Budget falied.");
            }
        },
        error: function (e) {
            console.log(e);
        }
    });
}

// show or hide Trendline
function showOrHiddenTrendline() {
    if ($('.active .lineChart').css('display') == 'none') {
        $('.active .lineChart').css('display', 'block');

        // create trend line chart.
        createTrendlineChart();

        // call resize windown
        $(window).trigger('resize');
    }
    else {
        $('.active .lineChart').css('display', 'none');
    }
}

// function refresh trend line
function refreshTrendline() {
    if ($('.lineChart').is(':visible')) {
        createTrendlineChart();
    }
}

// show trendline chart
function createTrendlineChart() {
    var monthList = [];
    var seriesArray = [];

    // get tab active
    var tabActive = $('.active:last');
    var tabIndex = $(tabActive).attr('budget-tab-index');
    $(tabActive).find('input[name="checkbox-category-line-' + tabIndex + '"]:checked').each(function () {
        var name = "";
        var budgetItemDetailList = [];

        // get current checkbox
        var $currentCheckbox = $(this);

        // get category name
        var categoryName = $currentCheckbox.next().html();

        // get parent name
        var parentName = $currentCheckbox.parent().parent().attr('parent-category-name');

        var section = $(this).attr('section-name');
        if ($(this).attr('is-parent-category') == "true") {
            $(tabActive).find('[id^="' + section + 'Total__ProjectionSales_' + tabIndex + '"]').each(function () {
                budgetItemDetailList.push(parseFloat($(this).attr(reValue)));
            });
        } else {
            categoryName += '(' + parentName + ')';
            var dataByCategory = $(tabActive).find('.data-row-by-category[category-setting-id="' + $(this).val() + '"]');
            $(dataByCategory).find('[id^="' + section + 'ProjectionSales_' + tabIndex + '"]').each(function () {
                budgetItemDetailList.push(parseFloat($(this).attr(reValue)));
            });
        }

        seriesArray.push({
            type: "line",
            data: budgetItemDetailList,
            name: categoryName,
        });
    });

    if (seriesArray.length > 0) {
        $('span[name^="HeaderName_' + tabIndex + '"]').each(function () {
            monthList.push($(this).html());
        });
    }

    $(tabActive).find(".lineChart").kendoChart({
        title: { text: "Trendline" },
        seriesColors: $.colorArray,
        series: seriesArray,
        legend: {
            position: "bottom"
        },
        valueAxis: {
            labels: {
                format: "{0:C}",
                skip: 2,
                step: 2
            }
        },
        categoryAxis: {
            categories: monthList,
            labels: {
                rotation: 310
            }
        },
        tooltip: {
            visible: true,
            template: "#= series.name #: #= kendo.format('{0:C}',value) #"
        }
    });
}

//show or hide Annual Graphs
function showOrHiddenAnnualGraphs() {
    if ($('.active .annualGraphs').css('display') == 'none') {
        $('.active .annualGraphs').css('display', 'block');

        // create trend line chart.
        createSalesChart();
        createCogsChart();
        createPayrollChart();
        createOperationChart();
        createProfilLossChart();

        // call resize windown
        $(window).trigger('resize');
    }
    else {
        $('.active .annualGraphs').css('display', 'none');
    }
}

// function refresh annual graphs
function refreshAnnualGraphs() {
    if ($('.annualGraphs').is(':visible')) {
        createSalesChart();
        createCogsChart();
        createPayrollChart();
        createOperationChart();
        createProfilLossChart();
    }
}

// proccess sales chart
function createSalesChart() {
    // get tab active sales
    var tabIndex = $('.active:last').attr('budget-tab-index');
    var seriesArray = [];
    $('.active:last .Area_Category_Sales .grand-total-category-by-row').find('[id*="ProjectionSales_' + tabIndex + '"]').each(function () {
        seriesArray.push({
            category: $(this).attr('category-name'),
            value: parseFloat($(this).attr(reValue)),
        });
    });

    var templateStr =
        '<div>' +
            '<div style="float:left; width: 75px; font-size: 15pt; margin-top: 30px"><b>#= kendo.format("{0:P}", percentage) #</b></div>' +
            '<div style="float:left; width: 170px; text-align: left">' +
                '<p style="font-size: 13pt"><b>#= category #</b></p>' +
                '<p>Amount: #= kendo.format("{0:C}",value) #</p>' +
                '<p>Percentage of Sales: #= kendo.format("{0:P}", percentage) #</p>' +
            '</div>' +
        '</div>';

    $(".active:last .salesChart").kendoChart({
        title: {
            position: "bottom",
            text: "Sales"
        },
        legend: {
            visible: false
        },
        seriesDefaults: {
            type: "donut",
            startAngle: 150,
        },
        series: [{
            data: seriesArray,
        }],
        tooltip: {
            visible: true,
            //background: "transparent",
            template: templateStr
        }
    });
}

// proccess cogs chart
function createCogsChart() {
    // get tab active sales
    var tabIndex = $('.active:last').attr('budget-tab-index');
    var seriesCogsArray = [];

    var grandTotalProjectionBySectionSales = 0;
    $('.active:last .Area_Category_Sales .grand-total-category-by-row').find('[id*="ProjectionSales_' + tabIndex + '"]').each(function () {
        grandTotalProjectionBySectionSales += parseFloat(Math.abs($(this).attr(reValue)));
    });

    $('.active:last .Area_Category_COGS .grand-total-category-by-row').find('[id*="ProjectionSales_' + tabIndex + '"]').each(function () {
        var percentBySectionSales = $('.active:last .Area_Category_Sales .grand-total-category-by-row').find('[id*="ProjectionSales_' + tabIndex + '"][category-name="' + $(this).attr('category-name') + '"]').attr(reValue);
        seriesCogsArray.push({
            category: $(this).attr('category-name'),
            value: parseFloat($(this).attr(reValue)),
            percentBySales: grandTotalProjectionBySectionSales == 0 ? 0 : $.formatPercent(Math.abs(percentBySectionSales) * 100 / grandTotalProjectionBySectionSales),
        });
    });

    var templateStr =
       '<div>' +
           '<div style="float:left; width: 75px; font-size: 15pt; margin-top: 30px"><b>#= kendo.format("{0:P}", percentage) #</b></div>' +
           '<div style="float:left; width: 170px; text-align: left">' +
               '<p style="font-size: 13pt"><b>#= category #</b></p>' +
               '<p>Amount: #= kendo.format("{0:C}",value) #</p>' +
               '<p>Percentage of Sales: #= dataItem.percentBySales #</p>' +
               '<p>Percentage of COGS: #= kendo.format("{0:P}", percentage) #</p>' +
           '</div>' +
       '</div>';

    $(".active:last .cogsChart").kendoChart({
        title: {
            position: "bottom",
            text: "COGS"
        },
        legend: {
            visible: false
        },
        seriesDefaults: {
            type: "donut",
            startAngle: 150
        },
        series: [{
            data: seriesCogsArray,
        }],
        tooltip: {
            visible: true,
            //background: "transparent",
            template: templateStr
        }
    });
}

// proccess Payroll Expenses chart
function createPayrollChart() {
    // get tab active sales
    var tabIndex = $('.active:last').attr('budget-tab-index');
    var seriesPayrollArray = [];

    $('.active:last .Area_Category_Payroll_Expenses .grand-total-category-by-row').find('[id*="ProjectionSales_' + tabIndex + '"]').each(function () {
        seriesPayrollArray.push({
            category: $(this).attr('category-name'),
            value: parseFloat($(this).attr(reValue)),
            percentBySales: $(this).next().val(),
        });
    });

    var templateStr =
        '<div>' +
            '<div style="float:left; width: 75px; font-size: 15pt; margin-top: 30px"><b>#= kendo.format("{0:P}", percentage) #</b></div>' +
            '<div style="float:left; width: 225px; text-align: left">' +
                '<p style="font-size: 13pt"><b>#= category #</b></p>' +
                '<p>Amount: #= kendo.format("{0:C}", value) #</p>' +
                '<p>Percentage of Sales: #= dataItem.percentBySales #</p>' +
                '<p>Percentage of Payroll Expenses: #= kendo.format("{0:P}", percentage) #</p>' +
            '</div>' +
        '</div>';

    $(".active:last .payrollChart").kendoChart({
        title: {
            position: "bottom",
            text: "Payroll Expenses"
        },
        legend: {
            visible: false
        },
        seriesDefaults: {
            type: "donut",
            startAngle: 150
        },
        series: [{
            data: seriesPayrollArray,
        }],
        tooltip: {
            visible: true,
            //background: "transparent",
            template: templateStr
        }
    });
}

// proccess Operation Expenses chart
function createOperationChart() {
    // get tab active sales
    var tabIndex = $('.active:last').attr('budget-tab-index');
    var seriesOperationArray = [];

    $('.active:last .Area_Category_Operation_Expenses .grand-total-category-by-row').find('[id*="ProjectionSales_' + tabIndex + '"]').each(function () {
        seriesOperationArray.push({
            category: $(this).attr('category-name'),
            value: parseFloat($(this).attr(reValue)),
            percentBySales: $(this).next().val(),
        });
    });

    var templateStr =
        '<div>' +
            '<div style="float:left; width: 75px; font-size: 15pt; margin-top: 30px"><b>#= kendo.format("{0:P}", percentage) #</b></div>' +
            '<div style="float:left; width: 245px; text-align: left">' +
                '<p style="font-size: 13pt"><b>#= category #</b></p>' +
                '<p>Amount: #= kendo.format("{0:C}", value) #</p>' +
                '<p>Percentage of Sales: #= dataItem.percentBySales #</p>' +
                '<p>Percentage of Operating Expenses: #= kendo.format("{0:P}", percentage) #</p>' +
            '</div>' +
        '</div>';

    $(".active:last .operatingChart").kendoChart({
        title: {
            position: "bottom",
            text: "Operating Expense"
        },
        legend: {
            visible: false
        },
        seriesDefaults: {
            type: "donut",
            startAngle: 150
        },
        series: [{
            data: seriesOperationArray,
        }],
        tooltip: {
            visible: true,
            //background: "transparent",
            template: templateStr
        }
    });
}

// process profil loss chart
function createProfilLossChart() {

    var tabIndex = $('.active:last').attr('budget-tab-index');
    var seriesArray = [];

    $('.active:last .Area_Category_Profit_Loss .grand-total-category-by-row').find('[id*="ProjectionSales_' + tabIndex + '"]').each(function () {
        seriesArray.push({
            category: $(this).attr('category-name'),
            value: parseFloat($(this).attr(reValue)),
        });
    });

    var grandTotalSales = parseFloat($('#GrandTotal__ProjectionSales_' + tabIndex).attr(reValue));
    var templateStr =
       '<div>' +
           '<div style="float:left; width: 75px; font-size: 15pt; margin-top: 30px"><b>#= kendo.format("{0:P}", percentage) #</b></div>' +
           '<div style="float:left; width: 200px; text-align: left">' +
               '<p style="font-size: 13pt"><b>#= category #</b></p>' +
               '<p>Amount: #= kendo.format("{0:C}", value) #</p>' +
               '<p>Percentage of Sales: #= kendo.format("{0:P}", value/' + grandTotalSales + ') #</p>' +
               '<p>Percentage of Profit/ Loss: #= kendo.format("{0:P}", percentage) #</p>' +
           '</div>' +
       '</div>';

    $(".active:last .profilLossChart").kendoChart({
        title: {
            position: "bottom",
            text: "Profit/ Loss"
        },
        legend: {
            visible: false
        },
        seriesDefaults: {
            type: "donut",
            startAngle: 150
        },
        series: [{
            data: seriesArray,
        }],
        tooltip: {
            visible: true,
            template: templateStr
        }
    });
}

// open import actual number
function openImportActualNumber() {
    var id = parseInt($("#BudgetId").val());
    var budgetTabId = parseInt($('.active:last').attr('budgetTabId'));
    return window.location = "/Budget/ImportActualNumber?id=" + id + "&budgetTabId=" + budgetTabId + "&headerName=&redirectPage=BudgetDetails";
}

// open variance reporting
function openVarianceReporting(categorySettingIdArray, headerColumnIndexArray, isAllSection) {
    // add budget item model list to budget tab
    var budgetTabModelList = [];
    budgetTabModelList.push(readBudgetItemByTab($('.active:last')));

    var budgetId = parseInt($("#BudgetId").val());
    var budgetName = $("#BudgetNameDisplay").val();
    var inputMethod = parseInt($("#InputMethod").val());
    var sectionName = $("#SectionViewName").val();

    var popupWindow = setaJs.initPopupWindow({
        id: "showVariance",
        title: "Variance report",
        height: 800,
        width: $(document).width() * 0.85,
    });

    popupWindow.refresh({
        type: "POST",
        url: "/Budget/ShowVarianceReport",
        data: JSON.stringify({
            id: budgetId,
            budgetTabId: $('.active:last').attr('budgettabid'),
            sectionName: sectionName,
            isAllSection: isAllSection,
            isVarianceReport: true,
            budgetTabModelList: budgetTabModelList,
            categorySettingIdList: categorySettingIdArray,
            headerColumnIndexList: headerColumnIndexArray,
        }),
        contentType: "application/json; charset=utf-8",
        async: false
    }).center().open();

    // set resize budget data area in variance report by rezise
    popupWindow.bind("resize", window_resize);
    setTimeout(function () {
        window_resize();
    },500)
    var $form = $('#showVariance');

    // set year to variance report form
    $form.find(".budget-tab-year").html(' ' + $('.active:first > a').html().replace('Period budget - ', ''));

    // set sales total to variance report form
    $form.find(".sales-total").html(' ' + $('.active:last').find('[id^="TotalSales_"]').val());

    // set sales target total to variance report form
    $form.find(".sales-target-total").html(' ' + $('.active:last').find('[id^="TotalSalesTarget_"]').val());

    // set action cancel button
    $form.find("#act-accept-no").on("click", function () {
        if (popupWindow) popupWindow.close();
    });

    // set action export button
    $form.find("#act-accept-yes-export").unbind("click");
    $form.find("#act-accept-yes-export").on("click", function () {
        showProcessing(60000);
        // call action export excel
        $.ajax(
        {
            url: '/Budget/ExportVarianceReport',
            type: "POST",
            data: JSON.stringify({
                id: budgetId,
                inputMethod: inputMethod,
                isAllSection: isAllSection,
                isVarianceReport: true,
                budgetTabModelList: budgetTabModelList,
                categorySettingIdList: categorySettingIdArray,
                headerColumnIndexList: headerColumnIndexArray,
            }),
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            async: false,
            success: function (response) {
                if (response.status) {
                    var host = "http://" + document.location.host;
                    var path = response.Url;
                    path = path.replace('~', '');
                    $.fileDownload(host + path);
                    hideProcessing();
                }
            }
        });
    });

    // set action print button
    $('#act-accept-yes-print').printPreview();
}

// set resize budget data area in variance report by rezise
function window_resize() {
    $('#showVariance .budget-data-area').css('height', ($.formatNumber($('#showVariance').css('height')) - 147) + 'px');
}

// open variance setting reporting
function openVarianceReportSettings() {
    var tabId = $('.active:last').attr('budgettabid');
    var budgetId = parseInt($("#BudgetId").val());
    var popupWindow = setaJs.initPopupWindow({
        id: "varianceReportSettings",
        title: "Variance report settings",
        height: 800,
        width: $(document).width() * 0.85,
    });

    popupWindow.refresh({
        type: "POST",
        url: "/budget/ShowVarianceReportSetting",
        data: JSON.stringify({ id: budgetId, tabId: tabId }),
        contentType: "application/json; charset=utf-8",
        async: false
    }).center().open();

    // set resize budget data area in variance report by rezise
    popupWindow.bind("resize", window_resize_report);

    var $form = $('#varianceReportSettings');
    $form.find(".budget-tab-year").html($('.active:first > a').html().replace('Period budget - ', ''));
    $form.find("#act-accept-no").on("click", function () {
        if (popupWindow) popupWindow.close();
    });

    // generate variance settings
    $form.find("#act-accept-yes-genarate").unbind("click");
    $form.find("#act-accept-yes-genarate").on("click", function () {
        showProcessing(1500);
        setTimeout(function () {
            // validation start end column
            var startColumn = $("#headerColumnStart").data("kendoDropDownList");
            var endColumn = $("#headerColumnEnd").data("kendoDropDownList");
            if (startColumn.selectedIndex > endColumn.selectedIndex) {
                warningPopup("Warning", "End cannot be before Start.");
                return;
            }

            // set header column list
            var headerColumnIndexArray = [];
            $('.active .header-name').each(function (i, v) {
                if (i >= startColumn.selectedIndex && i <= endColumn.selectedIndex) {
                    headerColumnIndexArray.push({
                        HeaderIndex: i,
                        HeaderName: $(v).attr('header-month-year'),
                    });
                }
            });

            // set category setting id list
            var categorySettingIdArray = [];
            $('#varianceReportSettings input[type="checkbox"]:checked').each(function () {
                categorySettingIdArray.push($(this).val());
            })

            // call action export excel
            if (categorySettingIdArray.length > 0) {
                showProcessing();
                var isAllSection = false;
                openVarianceReporting(categorySettingIdArray, headerColumnIndexArray, isAllSection);
            }
            else {
                warningPopup("Warning", "Please select at least a category!");
            }
        }, 500);
    });

    // call resize width header in variance report setting
    resizeHeaderInVarianceReportSetting($form);
}

function resizeHeaderInVarianceReportSetting(form) {
    setTimeout(function () {
        var headerDiv = $(form).find('.header-variance-report-setting:first');
        var tableDiv = $(form).find('.header-variance-report-setting:last');
        if ($(tableDiv).offset().top - $(window).scrollTop() < $(tableDiv).height()) {
            var newWidth = $(tableDiv).width() - 18;
            $(headerDiv).width(newWidth);
        }
    }, 500);
}

function convertMonthNameToNumber(month) {
    return new Date(Date.parse("1 " + month)).getMonth() + 1;
}

function startChange() {
    var endPicker = $("#end").data("kendoDatePicker"),
        startDate = this.value();

    if (startDate) {
      //  startDate = new Date(startDate);
       // startDate.setDate(startDate.getMonth() + 1);
        endPicker.min(startDate);
    }
}

function endChange() {
    var startPicker = $("#start").data("kendoDatePicker"),
        endDate = this.value();

    if (endDate) {
       // endDate = new Date(endDate);
      //  endDate.setDate(endDate.getDate() - 1);
        startPicker.max(endDate);
    }
}

// set resize budget data area in variance report by rezise
function window_resize_report() {
    $('#varianceReportSettings .category-name-column').css('height', this.element.height() - 80);
}

// function show or hidden div by section
function showOrHideDiv(item) {
    var $currentItem = $(item);
    var $currentTab = $currentItem.closest('.tab-area-by-section');

    var sectionName = $currentItem.attr('section-name');
    var currentClass = $currentItem.children().attr('class');
    if ($currentItem.children().hasClass('fa-angle-double-right')) {
        $currentItem.children().attr('class', 'fa fa-angle-double-down');
        $currentTab.find('.Area_Category_' + sectionName).css('display', 'block');
    } else {
        $currentItem.children().attr('class', 'fa fa-angle-double-right');
        $currentTab.find('.Area_Category_' + sectionName).css('display', 'none');
    }
}

// function show or hidden actual column and variance column
function showHideActualAndVariance(actualFlg, varianceFlg) {
    var widthByHeader = 496;
    if (actualFlg) {
        if($('.tab-area-by-section .actual-div:first').css('display') == 'none')
            $('.tab-area-by-section .actual-div').css('display', 'block');
    } else {
        if ($('.tab-area-by-section .actual-div:first').css('display') == 'block')
            $('.tab-area-by-section .actual-div').css('display', 'none');
        widthByHeader -= 165;
    }

    if (varianceFlg) {
        if ($('.tab-area-by-section .variance-div:first').css('display') == 'none')
            $('.tab-area-by-section .variance-div').show();
    } else {
        if ($('.tab-area-by-section .variance-div:first').css('display') == 'block')
            $('.tab-area-by-section .variance-div').hide();
        widthByHeader -= 165;
    }

    $('.tab-area-by-section').each(function () {
        // reCalculate budget area
        reCalculateWidthBudgetArea(this, widthByHeader);
    });

    $('.tab-area-by-section .header-name').each(function () {
        $(this).css('width', (widthByHeader - 1) + 'px');
    });
}

// common calculate width budget area
function reCalculateWidthBudgetArea(tab, widthByHeader) {
    var $currentActiveTab = $('.active .budget-area');
    var tableWidth = $currentActiveTab.width() - 18;

    var $currentTab = $(tab);
    var newWidth = $currentTab.find('.header-name').length * widthByHeader;

    // set new width to budget center area
    $currentTab.find('.budget-item-container > .budget-item-container').css('width', newWidth + 'px');

    var grandTotalPanelWidth = $('.grand-total-panel').css('display') == 'none' ? 0 : widthByHeader + 3;

    // set new width to grand total area
    $currentTab.find('.budget-detail-item:last').css('width', grandTotalPanelWidth + 'px');

    // calculate percent all column in grid view
    var firstColumnWidth = 185;
    var lastColumnWidth = grandTotalPanelWidth;
    var seconrdColumnWidth = tableWidth - (firstColumnWidth + lastColumnWidth);

    // set new width to category column area
    $currentTab.find('.header-area > div:first').css('width', (firstColumnWidth + 1) + 'px');
    $currentTab.find('.budget-area > div:first').css('width', (firstColumnWidth + 1) + 'px');
    $currentTab.find('.footer-area > div:first').css('width', (firstColumnWidth + 1) + 'px');

    // set new width to budget column area
    $currentTab.find('.header-area > div:nth-child(2)').css('width', seconrdColumnWidth + 'px');
    $currentTab.find('.budget-area > div:nth-child(2)').css('width', seconrdColumnWidth + 'px');
    $currentTab.find('.footer-area > div:nth-child(2)').css('width', seconrdColumnWidth + 'px');

    // set new width to grand column
    $currentTab.find('.budget-area > div:last').css('width', lastColumnWidth + 'px');
    $currentTab.find('.header-area > div:last').css('width', lastColumnWidth + 'px');
    $currentTab.find('.footer-area > div:last').css('width', lastColumnWidth + 'px');

    //setTimeout(function () {
    //    // reset width header and footer grid view
    //    var afterTableWidth = $currentActiveTab.width() - 3 - ($currentActiveTab.hasScrollBar() ? 15 : 0);
    //    if (afterTableWidth != tableWidth) {
    //        reCalculateWidthBudgetArea(tab, widthByHeader)
    //    }
    //}, 500);
}

// check box uncheck box sales
function setFlagCheckBox(item) {
    var className = $(item).attr('id');
    $('.' + className).prop('checked', $(item).prop('checked'));
}

function setFlagChildrenCheckBox(item) {
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

// function show hidden grand total panel
function showOrHiddenGrandTotal(item) {
    var $currentItem = $(item);
    if ($currentItem.children().hasClass('fa') == false) {
        $currentItem = $currentItem.next();
    }

    if ($currentItem.children().hasClass('fa-angle-double-down')) {
        $currentItem.children().attr('class', 'fa fa-angle-double-up');
        $('.grand-total-panel').css('display', 'none');
        $currentItem.prev().children().html('Show Grand Total');
    } else {
        $currentItem.children().attr('class', 'fa fa-angle-double-down');
        $('.grand-total-panel').css('display', 'block');
        $currentItem.prev().children().html('Hide Grand Total');
    }

    var actualFlg = $('input[name="ActualNumbersFlg"]').prop('checked');
    var varianceFlg = $('input[name="VarianceFlg"]').prop('checked');
    showHideActualAndVariance(actualFlg, varianceFlg);
}

// function show hide full screen
function showFullScreen(item) {
    var $currentItem = $(item);
    if ($currentItem.children().hasClass('fa') == false) {
        $currentItem = $currentItem.next();
    }

    var currentClass = $currentItem.children().attr('class');
    if ($currentItem.children().hasClass('fa-arrows-alt')) {
        // hide all menu, header and footer
        $('.page-header').css('display', 'none');
        $('.menu-first').css('display', 'none');
        $('.menu-seconrd').css('display', 'none');
        $('.page-footer').css('display', 'none');
        $('.content').css('margin-top', -46);

        $currentItem.children().attr('class', 'fa fa-compress');
        $currentItem.prev().children().html('Normal Screen');
    } else {
        // show all menu, header and footer
        $('.page-header').css('display', 'block')
        $('.menu-first').css('display', 'block');
        $('.menu-seconrd').css('display', 'block');
        $('.page-footer').css('display', 'block');
        $('.content').css('margin-top', 0);

        $currentItem.children().attr('class', 'fa fa-arrows-alt');
        $currentItem.prev().children().html('Full Screen');
    }

    $(window).trigger('resize');
}

// sync data budget category seting from SSP
var syncDataBudgetCategorySettingFromSSP = function (restCode) {
    var restName = $('#restName').val();
    var budgetId = parseInt($("#BudgetId").val());
    var options = {
        id: "ConfirmationSyncCategorySetting",
        typeStatus: true,
        title: "Confirmation",
        confirmText: "Are you sure to sync Budget Categories Settings of Restaurant " + restName + " (" + restCode + ")" + " from SSP System?",
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

        // call action sync data budget categories setting.
        $.ajax(
        {
            url: '/budget/SyncBudgetCategorySettings',
            type: "POST",
            dataType: "json",
            data: JSON.stringify({
                id: budgetId
            }),
            contentType: "application/json; charset=utf-8",
            async: false,
            success: function (response) {
                if (response.Status) {
                    warningPopup("Information", response.Message, window.location.pathname + "?id=" + budgetId);
                }
                else
                    warningPopup("Warning", response.Message);
            }
        });
    });
}

// function export budget report
function exportBudgetReport() {
    // show processing
    showProcessing(3000);

    setTimeout(function () {
        var budgetId = parseInt($("#BudgetId").val());
        var inputMethod = parseInt($("#InputMethod").val());

        // set header column list
        var headerColumnIndexArray = [];
        $('.active .header-name').each(function (i, v) {
            headerColumnIndexArray.push({
                HeaderIndex: i,
                HeaderName: $(v).attr('header-month-year'),
            });
        });

        // set category setting id list
        var categorySettingIdArray = [];
        var sectionList = ["Sales", "COGS", "Payroll_Expenses", "Operation_Expenses"];
        $(sectionList).each(function (i, sectionName) {
            $('.active .Area_Category_' + sectionName).find('input[type="checkbox"]').each(function () {
                categorySettingIdArray.push($(this).val());
            });
        });

        // add budget item model list to budget tab
        var budgetTabModelList = [];
        budgetTabModelList.push(readBudgetItemByTab($('.active:last')));

        // call action export excel
        $.ajax(
        {
            url: '/Budget/ExportVarianceReport',
            type: "POST",
            data: JSON.stringify({
                id: budgetId,
                inputMethod: inputMethod,
                isAllSection: true,
                isVarianceReport: false,
                budgetTabModelList: budgetTabModelList,
                categorySettingIdList: categorySettingIdArray,
                headerColumnIndexList: headerColumnIndexArray,
            }),
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            async: false,
            success: function (response) {
                if (response.status) {
                    var host = "http://" + document.location.host;
                    var path = response.Url;
                    path = path.replace('~', '');
                    $.fileDownload(host + path);

                    // hide processing
                    hideProcessing();
                }
            }
        });
    }, 500);
}