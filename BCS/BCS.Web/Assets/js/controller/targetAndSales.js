$(document).ready(function (e) {
    // hidden next step
    $('.btn-continue').hide();

    // set action to button save change budget
    $('.btn-save-change').unbind('click');
    $('.btn-save-change').on('click', function () {
        SaveBudgetDetails("");
    });

    var subCatContainer = $(".budget-item-container");
    subCatContainer.scroll(function (e) {
        subCatContainer.scrollLeft($(this).scrollLeft());
    });

    // set action click checkbox checked show line chart trendline
    $('input[name^="checkbox-category-line-"]').on("change", function () {
        createTrendlineChart();
    });

    var editFlag = JSON.parse($('#EditFlag').val().toLowerCase());
    // set menu to role edit
    if (editFlag) {
        // set action change input type
        setEnableOrDisableBudgetItem();

        // set edit text box for Target item
        if ($('#InputMethod-hidden').val() == $('#inputMethodDollar').val()) {
            $("[id^='Target_Sales_']").addClass('numerictextbox');
            $("[id^='Target_Sales_']").removeClass('readonly');
            $("[id^='Target_Sales_']").removeAttr('readonly');
        } else {
            $("[id^='Target_Percent_']").addClass('numerictextbox');
            $("[id^='Target_Percent_']").removeClass('readonly');
            $("[id^='Target_Percent_']").removeAttr('readonly');
        }

        // set action focus input text
        $(".bcs-currency-textbox").unbind('focus');
        $(".bcs-currency-textbox").on("focus", setFocusOutItem);
        $(".bcs-percent-textbox").unbind('focus');
        $(".bcs-percent-textbox").on("focus", setFocusOutItem);

        // header context menu
        $("#headerContextMenu").kendoContextMenu({
            orientation: 'horizontal',
            target: ".header-row",
            filter: '.header-name',
            open: function (e) {
                var countMenu = ($('.tab-area-by-section').length == 1 && $('.tab-area-by-section .header-row .header-name').length == 1) ? 'none' : 'block';
                var ubcAllYearDisplay = ($('.tab-area-by-section').length == 1) ? 'none' : 'block';
                var ubcByYearDisplay = ($('.tab-area-by-section .header-row .header-name').length == 1) ? 'none' : 'block';
                $('#headerContextMenu > div:first').css('display', countMenu);
                $("#headerContextMenu").find('a[menu-name="headerMenu_UseBudgetBasisAllYears"]').css('display', ubcAllYearDisplay);
                $("#headerContextMenu").find('a[menu-name="headerMenu_UserBudgetBasis"]').css('display', ubcByYearDisplay);

                var budgetTabId = $.formatNumber($(e.target).attr("budget-tab-id"));
                $("#headerContextMenu").find("input[name='BudgetTabId']").val(budgetTabId);
                var headerName = $(e.target).children().html();
                $("#headerContextMenu").find("input[name='HeaderName']").val(headerName);
                var budgetTabIndex = $.formatNumber($(e.target).attr("budget-tab-index"));
                $("#headerContextMenu").find("input[name='BudgetTabIndex']").val(budgetTabIndex);
                var headerindex = ($(e.target).attr("header-index"));
                $("#headerContextMenu").find("input[name='HeaderIndex']").val(headerindex);
            },
            select: function (e) { }
        });

        // target context menu
        $("#targetContextMenu").kendoContextMenu({
            orientation: 'horizontal',
            target: ".target-row",
            filter: '.target-data',
            open: function (e) {
                $("#targetContextMenu").find("span[name='targetSalesInContextMenu']").html("Total: " + e.target.children[0].value);

                var budgetTabIndex = $.formatNumber($(e.target).attr("budget-tab-index"));
                $("#targetContextMenu").find("input[name='BudgetTabIndex']").val(budgetTabIndex);

                var HeaderIndex = ($(e.target).attr("header-index"));
                $("#targetContextMenu").find("input[name='HeaderIndex']").val(HeaderIndex);
            },
            select: function (e) { }
        });

        // sales context menu
        $("#salesContextMenu").kendoContextMenu({
            orientation: 'horizontal',
            target: ".sales-row",
            filter: '.total-sales-data',
            open: function (e) {
                // get current total sales div
                var $currentDiv = $(e.target);

                // get total sales
                var itemId = e.target.children[0].id;

                var $salesContextMenu = $("#salesContextMenu");
                $salesContextMenu.find("input[name='CopyItemName']").val(itemId);

                var sectionName = $currentDiv.parent().attr('section-name');
                if (sectionName == undefined) {
                    sectionName = "Profit/Loss";

                    // set hidden to use budget
                    $salesContextMenu.find('.sales-menu-use-budget').css('display', 'none');

                    // set section name after use read data by area
                    $salesContextMenu.find("input[name='SectionName']").val("Profit_Loss");
                } else {
                    $salesContextMenu.find('.sales-menu-use-budget').css('display', 'block');

                    // set section name after use read data by area
                    $salesContextMenu.find("input[name='SectionName']").val(sectionName);
                }

                // set section name to context menu
                $salesContextMenu.find("span[name='sectionNameInContextMenu']").html("Category group: " + sectionName.replace('_', ' '));

                // set total sales to context menu
                $salesContextMenu.find("span[name='totalSalesInContextMenu']").html("Total: " + $('#' + itemId).val());

                // set budget tab index to context menu
                var budgetTabIndex = $currentDiv.attr("budget-tab-index");
                $salesContextMenu.find("input[name='BudgetTabIndex']").val(budgetTabIndex);

                var headerIndex = $currentDiv.attr("header-index");
                $salesContextMenu.find("input[name='HeaderIndex']").val(headerIndex);
            },
            select: function (e) { }
        });

        // projection item context menu
        $("#projectionItemContextMenu").kendoContextMenu({
            orientation: 'horizontal',
            target: ".data-row-by-category",
            filter: '.projection-data',
            open: function (e) {
                var countMenu = ($('.tab-area-by-section').length == 1 && $('.tab-area-by-section .header-row .header-name').length == 1) ? 'none' : 'block';
                var ubcAllYearDisplay = ($('.tab-area-by-section').length == 1) ? 'none' : 'block';
                var ubcByYearDisplay = ($('.tab-area-by-section .header-row .header-name').length == 1) ? 'none' : 'block';
                $('#projectionItemContextMenu > div:last').css('display', countMenu);
                $("#projectionItemContextMenu").find('a[menu-name="projectionItem_UseBudgetBasisColumnAllYears"]').css('display', ubcAllYearDisplay);
                $("#projectionItemContextMenu").find('a[menu-name="projectionItem_UseBudgetBasisColumn"]').css('display', ubcByYearDisplay);

                // get current div projection data
                var $currentDiv = $(e.target);

                // get parent div data row by category
                var $dataRowByCategory = $(e.target.parentElement);

                // get parent category name
                var sectionName = $dataRowByCategory.attr('parent-category-name');

                // set parent category to context menu
                $("#projectionItemContextMenu").find("span[name='sectionNameInContextMenu']").html("Category group: " + sectionName);

                var categoryName = $dataRowByCategory.attr('category-name');
                $("#projectionItemContextMenu").find("span[name='categoryNameInContextMenu']").html("Category: " + categoryName);

                var headerName = ($("#budgetLenthTypeMonth").val() == $("#BudgetLengthType").val()) ? "Month : " : "Period : ";
                headerName += $currentDiv.find('input[type="hidden"][name="HeaderName"]').val();
                $("#projectionItemContextMenu").find("span[name='headerNameInContextMenu']").html(headerName);

                var $projectionSalesItem = $currentDiv.find('.bcs-currency-textbox');
                $("#projectionItemContextMenu").find("span[name='projectionSalesInContextMenu']").html("Total: " + $projectionSalesItem.val());

                var copyItem = 0;
                if ($projectionSalesItem.hasClass('readonly')) {
                    copyItem = $projectionSalesItem.attr('id').replace('Sales', 'Percent');
                } else {
                    copyItem = $projectionSalesItem.attr('id');
                }
                $("#projectionItemContextMenu").find("input[name='CopyItemName']").val(copyItem);

                // set categoryid
                var categorySettingId = $dataRowByCategory.attr('category-setting-id');
                $("#projectionItemContextMenu").find("input[name='CategorySettingId']").val(categorySettingId);

                // set budget tab index
                var budgetTabIndex = $dataRowByCategory.attr('budget-tab-index');
                $("#projectionItemContextMenu").find("input[name='BudgetTabIndex']").val(budgetTabIndex);

                var headerIndex = $currentDiv.attr("header-index");
                $("#projectionItemContextMenu").find("input[name='HeaderIndex']").val(headerIndex);

                // set hidden menu use budget if case show Profit loss
                if (sectionName == "Profit/Loss") {
                    $("#projectionItemContextMenu").find('.projection-menu-use-budget').css('display', 'none');
                } else {
                    $("#projectionItemContextMenu").find('.projection-menu-use-budget').css('display', 'block');
                }
            },
            select: function (e) { }
        });

        // projection item context menu confirm change status
        $("#confirmChangeStatusContextMenu").kendoContextMenu({
            orientation: 'horizontal',
            target: ".data-row-by-category",
            filter: '.projection-data',
            open: function (e) { },
            select: function (e) { }
        });

        // category item context menu
        $("#categoryItemContextMenu").kendoContextMenu({
            orientation: 'horizontal',
            target: ".category-name-column",
            filter: '.category-name-data',
            open: function (e) {
                // get menu div
                var $categoryItemContextMenu = $("#categoryItemContextMenu");

                // set parent category setting id
                var $sectionArea = $(e.target.parentElement);
                var parentCategorySettingId = $sectionArea.attr('parent-category-id');
                $categoryItemContextMenu.find("input[name='ParentCategorySettingId']").val(parentCategorySettingId);

                // set parent category name
                var sectionName = $sectionArea.attr('parent-category-name');
                $categoryItemContextMenu.find("input[name='SectionName']").val(sectionName);

                // set header category group name
                $categoryItemContextMenu.find("span[name='sectionNameInContextMenu']").html("Category group: " + sectionName);

                //set category setting name
                var $item = $(e.target);
                var categoryName = $item.attr('category-name');
                $categoryItemContextMenu.find("span[name='categoryNameInContextMenu']").html("Category: " + $.htmlEscape(categoryName));
                $categoryItemContextMenu.find("input[name='CategorySettingName']").val(categoryName);

                // set show or hide menu change prime cost
                if (sectionName == "Payroll Expenses") {
                    $categoryItemContextMenu.find("a[menu-name='categoryMenu_PrimeCost']").css("display", "block");
                } else {
                    $categoryItemContextMenu.find("a[menu-name='categoryMenu_PrimeCost']").css("display", "none");
                }

                // set show or hide menu "Sales Category"
                if (sectionName == "COGS") {
                    $categoryItemContextMenu.find(".sales-category-mapping").css("display", "block");

                    // get sales category ref id
                    var salesCategoryRefId = $item.attr('sales-category-ref-id');
                    $categoryItemContextMenu.find("input[name='SalesCategoryRefId']").val(salesCategoryRefId);

                    // reset color and background color
                    $categoryItemContextMenu.find(".sales-category-mapping").find('a').css("background-color", "");
                    $categoryItemContextMenu.find(".sales-category-mapping").find('a').css("color", "");

                    // find sales category id on menu, after set color active.
                    var $salesCategoryRefItem = $categoryItemContextMenu.find('a[sales-category-id="' + salesCategoryRefId + '"]');
                    $salesCategoryRefItem.css("background-color", "whitesmoke");
                    $salesCategoryRefItem.css("color", "#9C27B0");

                    // add sales category ref name
                    $categoryItemContextMenu.find("span[name='categoryNameInContextMenu']").append('<div style="color: #9C27B0;">Sales Category: ' + $salesCategoryRefItem.find('span').text() + '</div>');
                } else {
                    $categoryItemContextMenu.find(".sales-category-mapping").css("display", "none");
                }

                // hidden edit category for profit loss section
                if (sectionName == "Profit/Loss") {
                    $categoryItemContextMenu.find('a[menu-name="categoryMenu_ShowEditCategory"]').parent().css("display", "none");
                } else {
                    $categoryItemContextMenu.find('a[menu-name="categoryMenu_ShowEditCategory"]').parent().css("display", "block");
                }

                // set budget tab index
                var $tabAreaBySection = $item.closest('.tab-area-by-section');
                var budgetTabIndex = $tabAreaBySection.attr('budget-tab-index');
                $categoryItemContextMenu.find("input[name='BudgetTabIndex']").val(budgetTabIndex);

                // get category setting id focus action
                var categorySettingId = $item.attr('category-setting-id');
                $categoryItemContextMenu.find("span[name='grandTotalProjectionSalesInContextMenu']").html("Grand Total: " + $tabAreaBySection.find('input[id*="GrandTotalRow_ProjectionSales_"][category-setting-id="' + categorySettingId + '"]').val());
                $categoryItemContextMenu.find("input[name='CategorySettingId']").val(categorySettingId);

                // set flag istax
                var flagIxTax = 0;
                if ($item.find('input[section-name="IsTax_Payroll_Expenses_"]').attr('section-name') == "IsTax_Payroll_Expenses_") {
                    flagIxTax = 1;
                }
                $categoryItemContextMenu.find("input[name='IsTaxFlag']").val(flagIxTax);
            },
            select: function (e) { }
        });

        $(window).on('beforeunload', function () {
            if ($.isChangeDataInPage) {
                return 'Do you want to save your changes';
            }
        });
    }

    $("#isShowHelp").on('click', function () {
        showHideHelpOther(false, 5);
        $('.Hide-HelpSettingTargetAndSales').addClass('display-none');
        $('.Show-HelpSettingTargetAndSales').removeClass('display-none');
        $("#isShowHelp").prop('checked', true);

        $(window).trigger('resize');
    });
    $("#isHiddenHelp").on('click', function () {
        showHideHelpOther(true, 5);
        $('.Hide-HelpSettingTargetAndSales').removeClass('display-none');
        $('.Show-HelpSettingTargetAndSales').addClass('display-none');
        $("#isHiddenHelp").prop('checked', false);

        $(window).trigger('resize');
    });

    // disable key [control + z]
    $(window).on('keydown', function (event) {
        if (event.which === 90 && event.ctrlKey) {
            return false;
        }
    });

    setTimeout(function () {
        // get is change data in page flag
        if ($.cookie("isChangeDataInPage") != null) {
            $.isChangeDataInPage = $.cookie("isChangeDataInPage");
            $.cookie("isChangeDataInPage", null, { expires: -1 });
        }

        // set action show hidden section on double click category name total
        $('.category-total').dblclick(function () {
            $(this).find('a').click();
        });

        // action show/hide by section view name
        //$.showOrHideSection($('#SectionViewName').val());
        var sectionList = ["Sales", "COGS", "Payroll_Expenses", "Operation_Expenses", "Profit_Loss"];
        $(sectionList).each(function (i, sectionName) {
            $('.Area_Category_' + sectionName).hide();
        });

        // disable all item
        if (editFlag == false) {
            $('.btn-save-change').hide();
            $('#InputMethod').data('kendoDropDownList').enable(false);
            $('.numerictextbox').each(function () {
                $(this).removeClass('numerictextbox');
                $(this).addClass('readonly');
                $(this).attr('readonly', 'readonly');
            });
        }

        // set active the first tab
        var budgetTabIndex = parseInt($('#BudgetTabIndex').val()) - 1;
        $(".nav-tabs li > a:nth(" + budgetTabIndex + ")").click();

        // set show or hide column actual & variance
        setTimeout(function () {
            $(window).trigger('resize');

            // set focus annual sales
            $('.active:last input[id^="Annual_Sales_"]').focus();

            // show area by section name
            var sectionName = $('#SectionViewName').val();
            var $item = $('.active:last a[section-name="' + sectionName + '"]');
            if ($item.children().hasClass('fa-angle-double-right')) {
                $item.children().attr('class', 'fa fa-angle-double-down');
                $('.active:last .Area_Category_' + sectionName).show();
            }
        }, 500);
    }, 500);
});

function showHideHelpOther(flag, helpSettingDataId) {
    $.ajax({
        type: "POST",
        url: "/Home/ShowOrHiddenHelp",
        data: { flag: flag, helpSettingDataId: helpSettingDataId },
        success: function (data) {
            if (data.Status) {
                console.log('done');
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

// function set enable or disable budget item
function setEnableOrDisableBudgetItem() {
    $('[id^="Target_"]').on('dblclick', function () {
        var $targetItem = $(this);
        if ($targetItem.hasClass('readonly')) {
            var data = this.id.indexOf("Sales") != -1 ? 2 : 1;
            $("#InputMethod").data("kendoDropDownList").value(data);
            $("#InputMethod").data("kendoDropDownList").trigger("change");

            // set focus after change
            setTimeout(function () {
                $targetItem.trigger("focus");
            });
        }
    });

    $('.projection-data input').on('dblclick', function (e) {
        var $currentItem = $(this);
        var currentItemName = this.id;
        if ($currentItem.hasClass('numerictextbox') || currentItemName.indexOf('Profit_Loss_') == 0) return;

        // get item type
        var itemId = this.id;
        var itemType = (itemId.indexOf("Sales") != -1) ? "Sales" : "Percent";

        // get data row by category
        var dataRow = $currentItem.closest('.data-row-by-category');
        $("#confirmChangeStatusContextMenu").find("input[name='CategorySettingId']").val($(dataRow).attr('category-setting-id'));

        var confirmFlag = false;
        $(dataRow).find('[id*="ProjectionSales"]').each(function () {
            if ($.formatNumber(this.value) > 0 || $.formatNumber($(this).next().val()) > 0) {
                confirmFlag = true;
                return;
            }
        });

        // check data in row last than by zero
        if (confirmFlag) {
            // show popup menu confirm
            var contextMenu = $("#confirmChangeStatusContextMenu").data("kendoContextMenu");
            contextMenu.open(e.clientX, e.clientY);

            $("#confirmChangeStatusContextMenu").find('#act-accept-no').on('click', function () {
                contextMenu.close(100, 100);
            });
            $("#confirmChangeStatusContextMenu").find('#act-accept-yes').unbind('click');
            $("#confirmChangeStatusContextMenu").find('#act-accept-yes').on('click', function () {
                showProcessing();
                contextMenu.close(100, 100);
                var checkedRadioValue = $.formatNumber($("#confirmChangeStatusContextMenu").find('input[type="radio"][name="radio-change-status"]:checked').val());
                var categorySettingId = $.formatNumber($("#confirmChangeStatusContextMenu").find('input[name="CategorySettingId"]').val());

                // case set all
                if (checkedRadioValue == 1) {
                    // call set enable or disable all item in row
                    $('.active:last .data-row-by-category[category-setting-id="' + categorySettingId + '"]').find('[id*="Projection' + itemType + '"]').each(function (i, v) {
                        setEnableOrDisableByItem(this, itemType);
                    });
                } else if (checkedRadioValue == 2) {
                    // call set enable or disable all item in row(cancel this item)
                    $('.active:last .data-row-by-category[category-setting-id="' + categorySettingId + '"]').find('[id*="Projection' + itemType + '"]').each(function (i, v) {
                        var $otherItem = (itemType == "Sales") ? $(this).next() : $(this).prev();
                        if ($.formatNumber($(this).attr('re-value')) == 0 && $.formatNumber($otherItem.attr('re-value')) == 0) {
                            setEnableOrDisableByItem(this, itemType);
                        }
                    });
                } else if (checkedRadioValue == 3) {
                    // call set enable or disable all item in row(cancel this item)
                    $('.active:last .data-row-by-category[category-setting-id="' + categorySettingId + '"]').find('[id*="Projection' + itemType + '"]').each(function (i, v) {
                        var $otherItem = (itemType == "Sales") ? $(this).next() : $(this).prev();
                        if ($.formatNumber($(this).attr('re-value')) != 0 || $.formatNumber($otherItem.attr('re-value')) != 0) {
                            setEnableOrDisableByItem(this, itemType);
                        }
                    });
                }

                if (currentItemName.indexOf("COGS_") == 0 || currentItemName.indexOf("Payroll_Expenses_") == 0 || currentItemName.indexOf("Operation_Expenses_") == 0) {
                    // Call reCalculate profit loss
                    var tabIndex = $('.active:last').attr('budget-tab-index');
                    setTimeout(function () {
                        ProfitLoss.ReCalcProfitLossByRow(tabIndex);
                    }, 1000);
                }

                // set flag change data on grid view
                $.isChangeDataInPage = true;
            });
        } else {
            $(dataRow).find('[id*="Projection' + itemType + '"]').each(function () {
                setEnableOrDisableByItem(this, itemType);
            });
        }

        setTimeout(function () { $('#' + itemId).focus(); });
    });
}

function setEnableOrDisableByItem(projectionItem, itemType) {
    $(projectionItem).removeAttr('readonly');
    $(projectionItem).removeClass('readonly');
    $(projectionItem).addClass('numerictextbox');

    var otherItem = (itemType == "Sales") ? $(projectionItem).next() : $(projectionItem).prev();
    $(otherItem).attr('readonly', 'readonly')
    $(otherItem).addClass('readonly');
    $(otherItem).removeClass('numerictextbox');

    $(projectionItem).parent().find('[id*="IsPercentage_"]').val(itemType == "Percent");
}

function contextMenuAction(item) {
    // get context menu
    var contextMenu = $(item).closest('div[data-role="contextmenu"]');

    // close the ContextMenu
    contextMenu.data("kendoContextMenu").close(100, 100);

    // Process call Import Actual Number
    var subcontextMenu = $(item).attr('menu-name');
    switch (subcontextMenu) {
        case "headerMenu_UserBudgetBasis":
            UseBudgetBasis(contextMenu, false);
            break;
        case "headerMenu_UseBudgetBasisAllYears":
            UseBudgetBasis(contextMenu, true);
            break;
        case "headerMenu_ShowVarianceReport":
            ShowVarianceReport(contextMenu, false, true);
            break;
        case "headerMenu_ShowVarianceReportYear":
            ShowVarianceReport(contextMenu, true, true);
            break;
        case "headerMenu_ImportActualNumber":
            ImportActualNumber(contextMenu);
            break;
        case "targetMenu_ShowVarianceReport":
            ShowVarianceReport(contextMenu, false, true);
            break;
        case "salesMenu_UseBudgetBasis":
            UseBudgetBasisByColumn(contextMenu, false);
            break;
        case "salesMenu_UseBudgetBasisAllYears":
            UseBudgetBasisByColumn(contextMenu, true);
            break;
        case "salesMenu_ShowVarianceReport":
            ShowVarianceReport(contextMenu, false, false);
            break;
        case "salesMenu_ShowVarianceReportGroupMonths":
            ShowVarianceReport(contextMenu, true, false);
            break;
        case "projectionItem_UseBudgetBasisColumn":
            UseAsBudgetBasisByRow(contextMenu, false);
            break;
        case "projectionItem_UseBudgetBasisColumnAllYears":
            UseAsBudgetBasisByRow(contextMenu, true);
            break;
        case "projectionItem_ShowVarianceReport":
            ShowVarianceReport(contextMenu, false, false);
            break;
        case "categoryMenu_PrimeCost":
            changeIsPrimeCost(contextMenu);
            break;
        case "categoryMenu_TrendLine":
            showTrendLine(contextMenu);
            break;
        case "categoryMenu_ShowVarianceReport":
            ShowVarianceReport(contextMenu, true, false);
            break;
        case "categoryMenu_ShowEditCategory":
            ShowViewEditCategory(contextMenu);
            break;
        case "categoryMenu_ChangeSalesCategoryRefId":
            ChangeSalesCategoryRefId(contextMenu, $(item).attr("sales-category-id"));
            break;
        default:
            console.log("Sorry, ");
    }
}

// Use as budget basis
function UseBudgetBasis(contextMenu, byYear) {
    var months = ($("#budgetLenthTypeMonth").val() == $("#BudgetLengthType").val()) ? "months" : "periodic";
    var year = byYear ? "all years." : "current year.";

    // show confirm message
    var confirmMessage = "<p>This command overwrites all budget information for remaining " + months + " in " + year + "</p>";
    confirmMessage += "<p>Do you want to continue?</p>";
    var options = {
        id: "ConfirmPopup",
        typeStatus: true,
        title: "Warning",
        confirmText: confirmMessage,
        textYes: "Yes",
        textNo: "No"
    }
    var popupWindow = window.setaJs.initPopupWindow(options);
    popupWindow.center().open();

    var $form = $('#' + options.id);
    $form.find("#act-accept-no").on("click", function () {
        if (popupWindow) popupWindow.close();
    });

    $form.find("#act-accept-yes").unbind('click');
    $form.find("#act-accept-yes").on("click", function () {
        if (popupWindow) popupWindow.close();
        showProcessing(10000);

        setTimeout(function () {
            var budgetTabIndex = $.formatNumber($(contextMenu).find('input[name="BudgetTabIndex"]').val());
            var headerIndex = $.formatNumber($(contextMenu).find('input[name="HeaderIndex"]').val());

            // copy to server call reload page
            if ($('.header-name').length >= 52) {
                var budgetTabModelList = [];
                $('.tab-area-by-section').each(function (i, tab) {
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
                        var isPercentageCopy = false;
                        var salesCopy = 0;
                        var percentCopy = 0;
                        $(this).find('.projection-data').each(function (colIndex, item) {
                            if (colIndex <= headerIndex) {
                                isPercentageCopy = $(this).find('[id*="IsPercentage"]').val();
                                salesCopy = $.formatNumberSave($(this).find('[id*="ProjectionSales"]').attr(reValue));
                                percentCopy = $.formatNumberSave($(this).find('[id*="ProjectionPercent"]').attr(reValue));
                            }

                            var budgetItemDetail = {
                                IsPercentage: isPercentageCopy,
                                ProjectionSales: salesCopy,
                                ProjectionPercent: percentCopy,
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
                            BudgetItemList: budgetItemDetailList,
                            IsTaxCost: isTaxCost,
                            IsPrimeCost: isPrimeCost,
                        };

                        // add budget item model to budget item model list
                        budgetItemModelList.push(budgetItemModel);
                    });

                    // budget item list in tab
                    budgetTabModelList.push({
                        BudgetTabId: $(tab).attr("budgetTabId"),
                        TabIndex: $(tab).attr("budget-tab-index"),
                        TabName: $(tab).attr("budget-tab-name"),
                        AnnualSales: annualSales,
                        HeaderColumnList: headerColumnList,
                        TargetColumnList: targetColumnList,
                        BudgetItemModelList: budgetItemModelList,
                    });
                });

                var BudgetId = parseFloat($("#BudgetId").val());
                var InputMethod = parseFloat($('#InputMethod').val());
                var ActualNumbersFlg = $('#ActualNumbersFlg').prop('checked');
                var VarianceFlg = $('#VarianceFlg').prop('checked');

                // call copy item to server
                $.ajax({
                    url: '/Budget/SaveBudgetDetailToSession',
                    type: "POST",
                    data: JSON.stringify({ budgetTabModelList: budgetTabModelList, budgetId: BudgetId, inputMethod: InputMethod, actualNumbersFlg: ActualNumbersFlg, varianceFlg: VarianceFlg }),
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    async: false,
                    success: function (response) {
                        if (response.Status) {
                            // save cookie is confirm rediect page
                            $.cookie("isChangeDataInPage", $.isChangeDataInPage);

                            // set no show confirm ridirect page
                            $.isChangeDataInPage = false;

                            // reload page
                            var sectionName = $("#SectionViewName").val();
                            window.location = window.location.pathname + "?id=" + $.urlParam("id") + '&section=' + sectionName + '&budgetTabIndex=' + budgetTabIndex;
                        } else {
                            warningPopup("Warning", "Change Budget falied.");
                            hideProcessing();
                        }
                    },
                    error: function (e) {
                        console.log(e);
                        hideProcessing();
                    }
                });
            } else {
                $('.active:last .data-row-by-category').each(function () {
                    var itemName = $(this).find('[id^="' + $(this).attr('section-name') + 'ProjectionSales_' + budgetTabIndex + '_' + headerIndex + '_"]').attr('id');
                    var categorySettingId = $(this).attr('category-setting-id');
                    copyBudgetByCategory(itemName, categorySettingId, byYear);
                    setTimeout(function () {
                        hideProcessing();

                        // Call reCalculate profit loss
                        setTimeout(function () {
                            ProfitLoss.ReCalcProfitLossByRow(budgetTabIndex);
                        }, 1000);
                    }, 3000);
                });
            }
        }, 500);
    });
}

// Use as budget basis
function UseBudgetBasisByColumn(contextMenu, byYear) {
    var months = ($("#budgetLenthTypeMonth").val() == $("#BudgetLengthType").val()) ? "months" : "periodic";
    var year = byYear ? "all years." : "current year.";

    // show confirm message
    var confirmMessage = "<p>This command overwrites all budget information for remaining " + months + " in " + year + "</p>";
    confirmMessage += "<p>Do you want to continue?</p>";
    var options = {
        id: "ConfirmPopup",
        typeStatus: true,
        title: "Warning",
        confirmText: confirmMessage,
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
        showProcessing(3000);

        setTimeout(function () {
            var copyItemName = $(contextMenu).find('input[name="CopyItemName"]').val().replace("Total__", "");

            $('[id^="' + copyItemName.replace('All_', '') + '"]').each(function () {
                var itemName = $(this).attr("id");
                if ($(this).hasClass('readonly')) {
                    itemName = $(this).next().attr('id');
                }
                var categorySettingId = $.formatNumber($(this).closest(".data-row-by-category").attr('category-setting-id'));
                copyBudgetByCategory(itemName, categorySettingId, byYear)
            });

            if (copyItemName.indexOf("All") == 0) {
                $('[id^="IsTax_' + copyItemName.replace('All_', '') + '"]').each(function () {
                    var itemName = $(this).attr("id");
                    if ($(this).hasClass('readonly')) {
                        itemName = $(this).next().attr('id');
                    }
                    var categorySettingId = $.formatNumber($(this).closest(".data-row-by-category").attr('category-setting-id'));
                    copyBudgetByCategory(itemName, categorySettingId, byYear)
                });
            }

            // Call reCalculate profit loss
            var tabIndex = $('.active:last').attr('budget-tab-index');
            setTimeout(function () {
                ProfitLoss.ReCalcProfitLossByRow(tabIndex);
            }, 1000);
        }, 500);
    });
}

// call change status prime cost by category setting id
function changeIsPrimeCost(contextMenu) {
    var categorySettingId = $.formatNumber($(contextMenu).find('input[name="CategorySettingId"]').val());

    // call action change status prime cost by category setting id
    $.ajax({
        url: '/Budget/ChangeIsPrimeCost',
        type: "POST",
        data: JSON.stringify({ categorySettingId: categorySettingId }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        async: false,
        success: function (response) {
            // change color category in all tab
            $('.category-name-data[category-setting-id="' + categorySettingId + '"]').each(function () {
                var $currentItem = $(this);
                if ($currentItem.css('color') == 'rgb(255, 0, 0)') {
                    $currentItem.css('color', 'black');
                    $currentItem.find('input[name="IsPrimeCost"]').val('false');
                } else {
                    $currentItem.css('color', 'red');
                    $currentItem.find('input[name="IsPrimeCost"]').val('true');
                }
            });
        },
        error: function (e) {
            warningPopup("Warning", "Process change prime cost error!");
        }
    });
}

// call show trend line
function showTrendLine(contextMenu) {
    var categorySettingId = $.formatNumber($(contextMenu).find('input[name="CategorySettingId"]').val());
    var checkBox = $('.active div[category-setting-id="' + categorySettingId + '"]:first input[type="checkbox"]');
    if ($(checkBox).prop('checked')) {
        $(checkBox).prop('checked', false);
    } else {
        $(checkBox).prop('checked', true);
    }

    // create trend line chart.
    createTrendlineChart();
    if (false == $('.lineChart').is(':visible')) {
        $('.lineChart').show(250);
    }
}

// show variance report
function ShowVarianceReport(contextMenu, isAllColumn, isAllSection) {
    // get header column index list
    var headerColumnIndexArray = [];
    if (isAllColumn) {
        $('.active:last .header-name').each(function (i, v) {
            headerColumnIndexArray.push({
                HeaderIndex: i,
                HeaderName: $(v).attr('header-month-year'),
            });
        });
    } else {
        var headerIndex = $(contextMenu).find('input[name="HeaderIndex"]').val();
        headerColumnIndexArray.push({
            HeaderIndex: headerIndex,
            HeaderName: $('.active:last .header-name:nth(' + headerIndex + ')').attr('header-month-year'),
        });
    }

    // get category setting id list
    var categorySettingIdArray = [];
    var categorySettingId = $.formatNumber($(contextMenu).find('input[name="CategorySettingId"]').val());
    if (categorySettingId > 0) {
        categorySettingIdArray.push(categorySettingId);
    } else if (isAllSection) {
        $('.active:last').find('.category-name-data').each(function () {
            categorySettingIdArray.push($.formatNumber($(this).attr('category-setting-id')));
        });
    } else {
        var sectionName = $(contextMenu).find('input[name="SectionName"]').val();
        $('.active:last .Area_Category_' + sectionName).find('.category-name-data').each(function () {
            categorySettingIdArray.push($.formatNumber($(this).attr('category-setting-id')));
        });
    }

    // call open variance reporting
    openVarianceReporting(categorySettingIdArray, headerColumnIndexArray, isAllSection);
}

// show popup edit category by budget
function ShowViewEditCategory(contextMenu) {
    // TODO:
    var id = $(contextMenu).find('input[name="ParentCategorySettingId"]').val();
    var categoryName = $(contextMenu).find('input[name="CategorySettingName"]').val();
    var sectionName = $(contextMenu).find('input[name="SectionName"]').val();
    var name = $(contextMenu).find('input[name="SectionName"]').val();
    var isTax = '';
    if (parseInt($(contextMenu).find('input[name="IsTaxFlag"]').val()) == 1) {
        isTax = true;
    }
    var budgetTabIndex = $(contextMenu).find('input[name="BudgetTabIndex"]').val();

    $.isTaxFlag = isTax ? true : false;
    name += $.isTaxFlag ? " (IS TAX)" : "";
    var title = "Manage " + name + " Categories";
    var width = $(document).width() > 1000 ? 1000 : $(document).width() * 0.85;

    var popupWindow = setaJs.initPopupWindow({
        id: "editCategorySettingByEditBudget",
        title: title,
        height: 800,
        width: width,
    });

    popupWindow.refresh({
        url: "/Budget/ViewEditCategorySetting?id=" + id,
        async: false
    }).center().open();
    popupWindow.center().open();

    if ($.isTaxFlag) {
        var headerName = $('#editCategorySettingByEditBudget .category-header-name').children().html();
        $('#editCategorySettingByEditBudget .category-header-name').children().html("Manage Payroll Expenses (IS TAX) Categories")
    }

    var $form = $('#editCategorySettingByEditBudget');

    // set focus item in grid by category name
    setTimeout(function () {
        $('#editCategorySettingByEditBudget').find('table >tbody >tr').each(function (index, item) {
            if (categoryName == $.htmlUnescape($($(this).children()[1]).html())) {
                var grid = $("#CategorySettingModelGrid").data("kendoGrid");
                grid.select($("#CategorySettingModelGrid tr:nth(" + (index + 1) + ")"));
                grid.editCell($("#CategorySettingModelGrid tr:nth(" + (index + 1) + ") td:nth(1)"));
                $("#CategorySettingModelGrid tr:nth(" + (index + 1) + ") td:nth(1)").focus();
            }
        });
    }, 500);

    $form.find("#act-accept-no").on("click", function () {
        if (popupWindow) popupWindow.close();
    });

    $form.find("#act-accept-yes").on("click", function () {
        if (popupWindow) popupWindow.close();
        showProcessing();

        var grid = $("#CategorySettingModelGrid").data("kendoGrid");
        if ($form.find(".field-validation-error").length > 0) {
            var $input = $form.find(".field-validation-error").prev();
            grid.select($input.closest('tr'));
            var scrollTop = $input.offset().top;
            $('.k-grid-content').scrollTop(scrollTop);
            $input.focus();
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
                    messageCheck = messageCheck + '<span class="error">This row number #' + categorySetting.SortOrder + ' category name "' + $.htmlEscape(categorySetting.CategoryName) + '" is duplicate.</span><br/>';
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

        // call function get all data by tab
        var budgetTabModelList = [];
        $('.tab-area-by-section').each(function () {
            budgetTabModelList.push(readBudgetItemByTab(this));
        })

        var InputMethod = parseFloat($('#InputMethod').val());
        var ActualNumbersFlg = $('#ActualNumbersFlg').prop('checked');
        var VarianceFlg = $('#VarianceFlg').prop('checked');

        // call action update/add categories.
        $.ajax(
        {
            url: '/Budget/SaveBudgetDetailToSession',
            type: "POST",
            data: JSON.stringify({ budgetTabModelList: budgetTabModelList, budgetId: budgetId, inputMethod: InputMethod, actualNumbersFlg: ActualNumbersFlg, varianceFlg: VarianceFlg }),
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            async: false,
            success: function (response) {
                if (response.Status) {
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
                                // save cookie is confirm rediect page
                                $.cookie("isChangeDataInPage", $.isChangeDataInPage);

                                // set no show confirm ridirect page
                                $.isChangeDataInPage = false;

                                // reload page
                                window.location = window.location.pathname + "?id=" + $.urlParam("id") + '&section=' + sectionName.trim().replace(" ", "_") + '&budgetTabIndex=' + budgetTabIndex;
                            }
                            else {
                                warningPopup("Warning", "Save falied.");
                            }
                        }
                    });
                }
                else {
                    warningPopup("Warning", "Save falied.");
                }
            },
            error: function (e) {
                console.log(e);
            }
        });
    });
}

function ChangeSalesCategoryRefId(contextMenu, salesCategoryId) {
    // get old sales category ref id
    var salesCategoryRefId = $(contextMenu).find("input[name='SalesCategoryRefId']").val();
    if (salesCategoryId == salesCategoryRefId) {
        // return no action
        return;
    }

    // get cogs category setting id
    var cogsCategoryId = $(contextMenu).find("input[name='CategorySettingId']").val();

    // call change sales category ref id
    $.ajax({
        url: '/Budget/ChangeSalesCategoryRefId',
        type: "POST",
        dataType: "json",
        data: JSON.stringify({ id: $("#BudgetId").val(), cogsCategoryId: cogsCategoryId, salesCategoryId: salesCategoryId }),
        contentType: "application/json; charset=utf-8",
        async: false,
        success: function (response) {
            if (response.Status) {
                // change ref id to screen
                $('.Area_Category_COGS').find('div[category-setting-id="' + cogsCategoryId + '"]').attr('sales-category-ref-id', salesCategoryId);

                // call reCalculate data after change sales category ref id
                $.reCalculateAfterChangeCategoryRefId(cogsCategoryId, salesCategoryId);

                // set show confirm ridirect page
                $.isChangeDataInPage = true;
            }
            else {
                warningPopup("Warning", response.Message);
            }
        }
    });
}

// Import Actual Number
function ImportActualNumber(contextMenu) {
    var budgetId = parseInt($("#BudgetId").val());
    var budgetTabId = $(contextMenu).find('input[name="BudgetTabId"]').val();
    var headerName = $(contextMenu).find('input[name="HeaderName"]').val();
    var url = "/Budget/ImportActualNumber";
    window.location.href = url + "?id=" + budgetId + "&budgetTabId=" + budgetTabId + "&headerName=" + headerName + "&redirectPage=BudgetDetails";
}

// Use as budget basis column
function UseAsBudgetBasisByRow(contextMenu, byYear) {
    var months = ($("#budgetLenthTypeMonth").val() == $("#BudgetLengthType").val()) ? "months" : "periodic";
    var year = byYear ? "all years." : "current year.";

    // show confirm message
    var confirmMessage = "<p>This command overwrites all budget information for remaining " + months + " in " + year + "</p>";
    confirmMessage += "<p>Do you want to continue?</p>";
    var options = {
        id: "ConfirmPopup",
        typeStatus: true,
        title: "Warning",
        confirmText: confirmMessage,
        textYes: "Yes",
        textNo: "No"
    }
    var popupWindow = window.setaJs.initPopupWindow(options);
    popupWindow.refresh({}).center().open();

    var $form = $('#' + options.id);
    $form.find("#act-accept-no").on("click", function () {
        if (popupWindow) popupWindow.close();
    });

    $form.find("#act-accept-yes").on("click", function () {
        if (popupWindow) popupWindow.close();

        var categorySettingId = $(contextMenu).find('input[name="CategorySettingId"]').val();
        var copyItemName = $(contextMenu).find('input[name="CopyItemName"]').val();
        copyBudgetByCategory(copyItemName, categorySettingId, byYear);
    });
}

// function reader data copy and pasre all item in row by category setting id
function copyBudgetByCategory(copyItemName, categorySettingId, byYear) {
    var nameArray = copyItemName.split("_");
    var tabIndex = $.formatNumber(nameArray[nameArray.length - 3]);
    var colIndex = $.formatNumber(nameArray[nameArray.length - 2]);

    var copyItem = $("#" + copyItemName);
    var projectionData = $(copyItem).closest('.projection-data');
    var IsPercentage = $.parseJSON($(projectionData).find('[id*="IsPercentage"]').val().toLowerCase());
    var ProjectionSales = $(projectionData).find('[id*="ProjectionSales"]').attr('re-value');
    var ProjectionPercent = $(projectionData).find('[id*="ProjectionPercent"]').attr('re-value');

    var projectionDataArray = $(copyItem).closest('.data-row-by-category').find('.projection-data');
    copyDataByRow(projectionDataArray, colIndex + 1, IsPercentage, ProjectionSales, ProjectionPercent);

    // process copy by year
    if (byYear) {
        // set set column index by copy month
        setTimeout(function () {
            $('.tab-area-by-section').each(function () {
                var budgetTabIndex = $.formatNumber($(this).attr('budget-tab-index'));

                if (budgetTabIndex > tabIndex) {
                    $(this).find('.data-row-by-category[category-setting-id="' + categorySettingId + '"]').each(function () {
                        var projectionDataArray = $(this).find('.projection-data');
                        copyDataByRow(projectionDataArray, 0, IsPercentage, ProjectionSales, ProjectionPercent);
                    });
                }
            });
        }, 500);
    }
}

// parse data to row by category setting id
function copyDataByRow(projectionDataArray, headIndex, IsPercentage, ProjectionSales, ProjectionPercent) {
    for (var i = headIndex; i < projectionDataArray.length; i++) {
        $(projectionDataArray[i]).find('[id*="IsPercentage"]').val(IsPercentage);
        var projectionSalesItem = $(projectionDataArray[i]).find('[id*="ProjectionSales"]');
        var projectionPercentItem = $(projectionDataArray[i]).find('[id*="ProjectionPercent"]');

        // set value to enable item
        if (IsPercentage) {
            setEnableOrDisableByItem(projectionPercentItem, "Percent");
            $(projectionPercentItem).val(ProjectionPercent);
            $(projectionPercentItem).change();
            // set format percent item
            $(projectionPercentItem).val($.formatPercent(ProjectionPercent));
        } else {
            setEnableOrDisableByItem(projectionSalesItem, "Sales");
            $(projectionSalesItem).val(ProjectionSales);
            $(projectionSalesItem).change();
            // set format currency item
            $(projectionSalesItem).val($.formatCurrency(ProjectionSales));
        }
    }
}

function changeTitle(sectionName) {
    var budgetName = $('#BudgetNameDisplay').val();
    $('.page-breadcrumb-sub').children().html(sectionName + ' (Budget Name: ' + budgetName + ')');

    $(document).prop('title', sectionName + ' (' + budgetName + ')');
}