var counter = 1;
$.itemDeleteList = [];

$(document).ready(function () {
    // reCalculate height main content
    $('#mainContent').height($(window).height() - 100);
    $('#mainContent').css('overflow', 'auto');
    $(window).on('resize', function () {
        $('#mainContent').height($(window).height() - 100);
    });
});

function renderNumber(data) {
    return counter++;
}

function checkIsPrimeCost() {
    $("input[type=hidden][name=isPrimeCost]").each(function (i, v) {
        $(this).closest('div').addClass('error')
    })
}

var isTaxFlag = false
function getCategoryIsTaxFlg() {
    return {
        id: $.formatNumber($("#CategoryId").val()),
        isTaxFlag: isTaxFlag,
    };
}

function editCategories(id, name, isTax) {
    // set is tax flag
    isTaxFlag = isTax ? true : false;
    name += isTaxFlag ? " (IS TAX)" : "";
    var title = "Manage " + name + " Categories";
    var width = $(document).width() > 1000 ? 1000 : $(document).width() * 0.85;

    var popupWindow = setaJs.initPopupWindow({
        title: title,
        height: 800,
        width: width,
        id: "editCategories"
    });

    popupWindow.refresh({
        url: "/CategoriesSetting/ViewEditCategories?id=" + id,
        async: false
    }).center().open();
    popupWindow.center().open();

    if (isTaxFlag) {
        var headerName = $('#editCategories .category-header-name').children().html();
        $('#editCategories .category-header-name').children().html("Manage Payroll Expenses (IS TAX) Categories")
    }

    var $form = $('#editCategories');
    $form.find("#act-accept-no").on("click", function () {
        if (popupWindow) popupWindow.close();
    });

    $form.find("#act-accept-yes").unbind("click");
    $form.find("#act-accept-yes").on("click", function () {
        showProcessing();
        var grid = $("#CategoryModelGrid").data("kendoGrid");
        if ($form.find(".field-validation-error").length > 0) {
            var input = $form.find(".field-validation-error").prev();
            grid.select($(input).closest('tr'));
            var scrollTop = $(input).offset().top;
            $('.k-grid-content').scrollTop(scrollTop);
            $(input).focus();

            return;
        }

        var parentCategoryId = parseInt($("#CategoryId").val());
        var parentCategoryName = $("#CategoryName").val();

        var index = 1;
        var listOfObjects = [];
        var checkDuplicateFlag = false;
        var categoryNameDuplicate = "", messageCheck = "";
        for (var item in grid.dataSource._data) {
            var categories = grid.dataSource._data[item];
            if (categories.CategoryId != null) {
                // parent category id is new category
                if (categories.CategoryId == 0) {
                    categories.ParentCategoryId = parentCategoryId;
                }

                if (categories.CategoryName == null || $.trim(categories.CategoryName).length == 0) {
                    $($($("#CategoryModelGrid tr")[index]).find('td')[1]).click();
                    $($($("#CategoryModelGrid tr")[index]).find('input[name="CategoryName"]')).blur();
                    return;
                }

                categories.SortOrder = index++;

                // check category name is duplicate in db
                if (categoryNameDuplicate.indexOf("[" + categories.CategoryName.toLowerCase() + "];") != -1) {
                    checkDuplicateFlag = true;
                    messageCheck = messageCheck + '<span class="error">This row number #' + categories.SortOrder + ' category name "' + $.htmlEscape(categories.CategoryName) + '" is duplicate.</span><br/>';
                } else {
                    categoryNameDuplicate = categoryNameDuplicate + "[" + categories.CategoryName.toLowerCase() + "];";

                    listOfObjects.push({
                        CategoryId: categories.CategoryId,
                        CategoryName: categories.CategoryName,
                        SortOrder: categories.SortOrder,
                        IsSelected: categories.IsSelected,
                        IsPrimeCost: categories.IsPrimeCost,
                        IsTaxCost: isTaxFlag,
                        IsPercentage: categories.IsPercentage,
                        ParentCategoryId: categories.ParentCategoryId,
                        DeletedFlg: categories.DeletedFlg,
                        CreatedUserId: categories.CreatedUserId,
                        CreatedDate: categories.CreatedDate
                    });
                }
            }
        }

        if (checkDuplicateFlag) {
            $(".categoryErrorMessage").empty().append(messageCheck);
            return;
        }

        $.merge(listOfObjects, $.itemDeleteList);

        // call action update/add categories.
        $.ajax(
        {
            url: '/CategoriesSetting/SaveCategories',
            type: "POST",
            data: JSON.stringify({ listObj: listOfObjects, parentCategoryName: parentCategoryName }),
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            async: false,
            success: function (response) {
                if (response.Status)
                    window.location = window.location.pathname;
                else
                    warningPopup("Warning", "Save falied.");
            }
        });
    });
}

function addNewCategories() {
    var grid = $("#CategoryModelGrid").data("kendoGrid");
    $.each(grid.dataSource._data, function (i, categories) {
        if (categories.CategoryName == null || $.trim(categories.CategoryName).length == 0) {
            grid.select($("#CategoryModelGrid tr:nth(" + (i + 1) + ")"));
            grid.editCell($("#CategoryModelGrid tr:nth(" + (i + 1) + ") td:nth(1)"));
            $("#CategoryModelGrid tr:nth(" + (i + 1) + ") td:nth(1)").focus();
            return;
        }
    });

    counter = 1;
    grid.addRow();

    // set checked default.
    var row = $("#CategoryModelGrid").find('tr:last');
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
    var grid = $("#CategoryModelGrid").data("kendoGrid");

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
    popupWindow.center().open();

    var $form = $('#' + options.id);
    $form.find("#act-accept-no").on("click", function () {
        if (popupWindow) popupWindow.close();
    });

    $form.find("#act-accept-yes").on("click", function () {
        if (dataItemThis.CategoryId > 0) {
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

function openLoadCategoryTemplate() {
    var popupWindow = setaJs.initPopupWindow({
        title: "Load category from template",
        height: 800,
        width: 1000,
        id: "LoadCategoryTemplate"
    });

    popupWindow.refresh({
        url: "/CategoriesSetting/ViewTemplateCategory",
        async: false
    }).center().open();

    var $form = $('#LoadCategoryTemplate');

    $form.find("#act-accept-no").on("click", function () {
        if (popupWindow) popupWindow.close();
    });
    $form.find("#act-accept-yes").on("click", function () {
        if (popupWindow) popupWindow.close();

        showProcessing();

        // call action reset all categories setting default
        setTimeout(function () {
            var defaultSectionNameList = "Sales,COGS,Payroll Expenses,Operation Expenses,";
            var sectionList = [];
            var categoryBySection = [];
            var sectionName = "";
            var sortOrder = 0;

            // get data in grid
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
                url: '/CategoriesSetting/SaveCategorySettingByTemplate',
                type: "POST",
                data: JSON.stringify({ sectionList: sectionList }),
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                async: false,
                success: function (response) {
                    if (response.Status)
                        warningPopup("Information", response.Message, window.location.pathname);
                    else
                        warningPopup("Warning", "Save falied.");
                }
            });
        });
    });
}

var loadDataFromTemplate = function(fileName) {
    //getting json data from kendo Grid
    $.ajax({
        type: "GET"
        , url: "/CategoriesSetting/GetDataFromTemplate"
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

var resetDefaultCategory = function () {
    var options = {
        id: "ConfirmationRestoreCategory",
        typeStatus: true,
        title: "Confirmation",
        confirmText: "Are you sure to restore Default Categories Settings to Sample Settings?",
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
            url: '/CategoriesSetting/RestoreCategories',
            type: "POST",
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            async: false,
            success: function (response) {
                if (response.Status)
                    window.location = window.location.pathname;
                else
                    warningPopup("Warning", "Restore Default Categories Settings falied.");
            }
        });
    });
}

//sync data category default from SSP
var syncDataDefaultCategoryFromSSP = function (restCode) {
    var restName = $('#restName').val();
    var options = {
        id: "ConfirmationSyncCategory",
        typeStatus: true,
        title: "Confirmation",
        confirmText: "Are you sure to sync Categories Settings of Restaurant " + restName + " (" + restCode + ")" + " from SSP System?",
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

        // call action sync data default categories setting.
        $.ajax(
        {
            url: '/CategoriesSetting/syncCategorySettings',
            type: "POST",
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            async: false,
            success: function (response) {
                if (response.Status) {
                    warningPopup("Information", response.Message, window.location.pathname);
                }
                else
                    warningPopup("Warning", response.Message);
            }
        });
    });
}