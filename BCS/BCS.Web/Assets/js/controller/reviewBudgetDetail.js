$(document).ready(function (e) {
    var sectionList = ["Sales", "COGS", "Payroll_Expenses", "Operation_Expenses"];

    var editFlag = JSON.parse($('#EditFlag').val().toLowerCase());
    if (editFlag) {
        $(window).on('beforeunload', function () {
            if ($.isChangeDataInPage) {
                return 'Do you want to save your changes';
            }
        });

        // set action focus input text
        $(".bcs-currency-textbox").unbind('focus');
        $(".bcs-currency-textbox").on("focus", setFocusItemReview);
        $(".bcs-percent-textbox").unbind('focus');
        $(".bcs-percent-textbox").on("focus", setFocusItemReview);

        // loop all section, add action double click projection item and actual sales.
        
        $(sectionList).each(function (i, sectionName) {
            $('.Area_Category_' + sectionName).find('input[id*="Projection"]').on('dblclick', function () {
                var $item = $(this);
                $item.addClass('numerictextbox');
                $item.removeClass('readonly');
                $item.removeAttr('readonly');
                setTimeout(function () {
                    setFocusOutItemReview($item);
                });

                var $itemOther;
                if ($item.attr('id').indexOf('Sales') != -1) {
                    $itemOther = $item.next();
                    $item.prev().val('false');
                } else {
                    $itemOther = $item.prev();
                    $itemOther.prev().val('true');
                }

                $itemOther.removeClass('numerictextbox');
                $itemOther.addClass('readonly');
                $itemOther.attr('readonly', 'readonly');
            });
            $('.Area_Category_' + sectionName).find('input[id*="ActualSales"]').on('dblclick', function () {
                var $item = $(this);
                $item.addClass('numerictextbox');
                $item.removeClass('readonly');
                $item.removeAttr('readonly');
                setTimeout(function () {
                    setFocusOutItemReview($item);
                });
            });
        });

        // set action double click item target
        $('[id^="Target_"]').on('dblclick', function () {
            var $item = $(this);
            if ($item.hasAttr('readonly')) {
                var data = this.id.indexOf("Sales") != -1 ? 2 : 1;
                $("#InputMethod").data("kendoDropDownList").value(data);
                $("#InputMethod").data("kendoDropDownList").trigger("change");
            }
            setTimeout(function () {
                setFocusOutItemReview($item);
            });
        });

        // set action double click item target
        $('[id^="Annual_Sales_"]').on('dblclick', function () {
            var $item = $(this);
            if ($item.hasAttr('readonly')) {
                var $item = $(this);
                $item.addClass('numerictextbox');
                $item.removeClass('readonly');
                $item.removeAttr('readonly');
                setTimeout(function () {
                    setFocusOutItemReview($item);
                });
            }
        });

    } else {
        $('.menu-seconrd').hide();
        $('.btn-primary').hide();
        $('.fa-edit').hide();
    }

    // loop all section, hidden row detail and add action double click projection item and actual sales.
    $(sectionList).each(function (i, sectionName) {
        $('.Area_Category_' + sectionName).hide();
    });

    // set action click checkbox checked show line chart trendline
    $('input[name^="checkbox-category-line-"]').on("change", function () {
        createTrendlineChart();
    });

    var subCatContainer = $(".budget-item-container");
    subCatContainer.scroll(function (e) {
        subCatContainer.scrollLeft($(this).scrollLeft());
    });

    setTimeout(function () {
        // set active the first tab
        $(".nav-tabs li:first > a").click();

        // set action show hidden section on double click category name total
        $('.category-total').dblclick(function () {
            $(this).find('a').trigger('click');
        });

        // call show hide column actual & variance
        $(window).trigger('resize');
    }, 500);
    $(window).on('resize', callShowHideActualAndVariance);
});

function callShowHideActualAndVariance() {
    var actualFlg = $.parseJSON($('#ActualNumbersFlg').val().toLowerCase());
    var varianceFlg = $.parseJSON($('#VarianceFlg').val().toLowerCase());
    showHideActualAndVariance(actualFlg, varianceFlg);
}

// function set focus item by format
function setFocusItemReview(e) {
    var $item = $(this);
    if ($item.hasAttr('readonly') || $item.hasClass('readonly') || $item.attr('id').indexOf('Variance') != -1 || $item.attr('id').indexOf('Total') != -1) return;

    $item.val($(this).attr('re-value'));
    setTimeout(function () {
        $item.select();
    });

    $item.unbind('blur');
    $item.on('blur', function () {
        if ($item.hasClass('bcs-currency-textbox')) {
            $item.val($.formatCurrency($(this).attr('re-value')));
        } else {
            $item.val($.formatPercent($(this).attr('re-value')));
        }
    });

    $item.keypress(function (e) {
        //if the letter is not digit then display error and don't type anything
        if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
            if ((e.which == 46 && this.value.indexOf('.') == -1) || (e.which == 45)) return true;
            return false;
        }
    });

    // set action key next input item
    $item.keydown(function (event) {
        if (event.keyCode == 13 && event.shiftKey) {
            $(this).trigger('blur');
            arrowUp(this.id);
            return false;
        } else if (event.keyCode == 9 && event.shiftKey) {
            $(this).trigger('blur');
            arrowLeft(this.id);
            return false;
        } else if (event.keyCode == 13) {// enter
            $(this).trigger('blur');
            arrowDown(this.id);
            return false;
        } else if (event.keyCode == 37) {// arrow left
            $(this).trigger('blur');
            arrowLeft(this.id);
            return false;
        } else if (event.keyCode == 38) {// arrow up
            $(this).trigger('blur');
            arrowUp(this.id);
            return false;
        } else if (event.keyCode == 39) {   // arrow right
            $(this).trigger('blur');
            arrowRight(this.id);
            return false;
        } else if (event.keyCode == 40) {  // arrow down
            $(this).trigger('blur');
            arrowDown(this.id);
            return false;
        } else if (event.keyCode == 9) {  // arrow tab
            $(this).trigger('blur');
            arrowRight(this.id);
            return false;
        }
    });
}

// function set focus and blur item
function setFocusOutItemReview($item) {
    $item.trigger('focus');
    $item.on('blur', function (e) {
        e.preventDefault();

        if ($(this).hasClass('bcs-currency-textbox')) {
            $(this).val($.formatCurrency($(this).attr('re-value')));
        } else {
            $(this).val($.formatPercent($(this).attr('re-value')));
        }

        if ($.isChangeDataInPage && $('.btn-save-change').is(":visible") == false) {
            $('.btn-save-change').html('Save Change & Reload');
            $('.btn-save-change').css('display', 'inline-block');
            $('.btn-save-change').css('width', 180);
        }
    });
}

// redirect page edit by section
function redirectPage(item) {
    window.location = "/Budget/ViewTagetAndSales?id=" + $.urlParam("id") + "&section=" + $(item).attr("section-name") + "&budgetTabIndex=" + $('.active:last').attr('budget-tab-index');
}