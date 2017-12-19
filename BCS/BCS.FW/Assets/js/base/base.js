$.fn.hasAttr = function (name) {
    return this.attr(name) !== undefined;
};

(function ($, window, document, undefined) {
    
    function getInternetExplorerVersion()
        // Returns the version of Internet Explorer or a -1
        // (indicating the use of another browser).
    {
        var rv = -1; // Default value assumes failure.
        var ua = navigator.userAgent;

        // If user agent string contains "MSIE x.y", assume
        // Internet Explorer and use "x.y" to determine the
        // version.

        var re = new RegExp("MSIE ([0-9]{1,}[\.0-9]{0,})");
        if (re.exec(ua) != null)
            rv = parseFloat(RegExp.$1);
        return rv;

    }

    $(document).ready(function () {
        var version = getInternetExplorerVersion();
        if (version != -1 && version < 8) {
            alert("Your browser version is not supported. For better view, please upgrade IE version 8 above.");
        }
        $("body").removeClass("hide");
        
        // select all text when clicked
        $(':text').focus(function () {
            $(this).one('mouseup', function (event) {
                event.preventDefault();
            }).select();
        });
    });
})(jQuery, window, document);

//add new property for Js Common
if (!String.prototype.format) {
    String.prototype.format = function () {
        var args = arguments;
        return this.replace(/{(\d+)}/g, function (match, number) {
            return typeof args[number] != 'undefined'
              ? args[number]
              : match
            ;
        });
    };
}

if (!String.prototype.trim) {
    //code for trim
    String.prototype.trim = function () { return this.replace(/^\s+|\s+$/g, ''); };
    String.prototype.ltrim = function () { return this.replace(/^\s+/, ''); };
    String.prototype.rtrim = function () { return this.replace(/\s+$/, ''); };
    String.prototype.fulltrim = function () { return this.replace(/(?:(?:^|\n)\s+|\s+(?:$|\n))/g, '').replace(/\s+/g, ' '); };
}
if (!Date.prototype.addMinutes) {
    Date.prototype.addHours = function (h) {
        var copiedDate = new Date(this.getTime());
        copiedDate.setMinutes(copiedDate.getMinutes() + h);
        return copiedDate;
    }
    Date.prototype.subMinutes = function (h) {
        var copiedDate = new Date(this.getTime());
        copiedDate.setMinutes(copiedDate.getMinutes() - h);
        return copiedDate;
    }
    String.prototype.substringEclipse = function (maxLength) {
        return this.length > maxLength ? this.substr(this, maxLength) + '...' : this;
    }
}

//ucfirst for string function
String.prototype.capitalize = function () {
    return this.charAt(0).toUpperCase() + this.slice(1);
};

Object.prototype.hasOwnProperty = function (property) {
    return typeof this[property] !== 'undefined';
};


function SubStr(input, count) {
    try {
        var flg = true;
        var arr = input.split(' ');
        var strSub = "";
        $.each(arr, function () {
            if ((strSub.length + this.length) < count) {
                strSub += this;
                strSub += " ";
            } else {
                if (flg) {
                    strSub += "...";
                    flg = false;
                }
            }
        });
    }
    catch (e) {
    }
    return strSub;
}

function ListAllGridAdditionalData() {
    return {
        keyword: $("#inputSearchGrid").val()
    };
}

//.runHashURL();

/* Left Box */
$(function () {
    if ($("#center .main").find(".k-grid").length > 0 && $("#center .main").find(".k-grid").hasClass("hide")) {
        $("#loading").removeClass("hide");
    }
});

var fixKendoGrid = function(obj) {
    if (obj.dataSource.data().length == 0) {

        $(obj.wrapper)
            .find(".k-grid-content").first()
            .find(".k-grid-content-empty")
            .remove();


        var msgDisplay = $("<div>").addClass("k-grid-content-empty").html("Nothing to show here.");
        $(obj.wrapper)
            .find(".k-grid-content").first()
            .append(msgDisplay);

        $(obj.wrapper)
            .find("ul.k-pager-numbers").first()
            .find("li").first()
            .children("span").html("1");

        $(obj.wrapper)
            .find(".k-grid-pager").first()
            .addClass("hide");
    } else {
        $(obj.wrapper)
            .find(".k-grid-content").first()
            .find(".k-grid-content-empty")
            .remove();
        $(obj.wrapper)
            .find(".k-grid-pager").first()
            .removeClass("hide");
    }
};

$.unserialize = function (serializedString) {
    qryString = decodeURI(serializedString);
    qryString = qryString.substring(1);
    var qryStringArr = qryString.split('&');
    var jsonobj = {}, paramvalue = '';
    for (i = 0; i < qryStringArr.length; i++) {
        paramvalue = qryStringArr[i].split('=');
        jsonobj[paramvalue[0]] = paramvalue[1];
    }
    return jsonobj;
}
/* Ext hidden field array*/
jQuery.fn.extend({
    addToHiddenArray: function (value) {
        return this.filter(":input").val(function (i, v) {
            var arr = v.split(',');
            arr.push(value);
            return arr.join(',');
        }).end();
    },
    removeFromHiddenArray: function (value) {
        return this.filter(":input").val(function (i, v) {
            return $.grep(v.split(','), function (val) {
                return val != value;
            }).join(',');
        }).end();
    },
    getHiddenArray: function () {
        return $.grep(this.val().split(','), function (val) {
            return val != null && val.length > 0;
        });
    },
    inHiddenArray: function (value) {
        var index = -1;
        $.grep(this.val().split(','), function (val, i) {
            if (val == value)
                index = i;
            return val == value;
        });
        return index;
    }
});
Array.prototype.getArrayDiff = function (a) {
    return this.filter(function (i) { return a.indexOf(i) < 0; });
};
var showGrid = function (e) {
    //if ($('#btnSaveOrder').html()!=undefined)
    //    $('#btnSaveOrder').attr('disabled', 'disabled');
    //var obj = $(e.sender.wrapper);

    fixKendoGrid.apply(this, [e.sender]);

    //obj.find(".k-grid-content").css('overflow', 'hidden');
    //obj.find(".k-pager-numbers").remove();
    //obj.find(".k-grid-header").addClass("k-grid-header-fixed");
    ////filter by alphabeta
    //if (obj.find("ul.left-filter-by-charactor").length > 0) {
    //    obj.find("ul.left-filter-by-charactor").removeClass('hide');
    //    obj.find("th.k-header > .k-header-column-menu").removeClass("k-highlight");
    //    obj.find("ul.left-filter-by-charactor li.title").removeClass("key-filter-active");
    //    var ft = e.sender.dataSource.filter();
    //    if (ft != null) {
    //        var fields = $.gridState.lookdeepFieldName(ft.filters);
    //        $.each(fields, function(k, item) {
    //            if (item.field == "KeyFilter") {
    //                var keyFilter = obj.find("ul.left-filter-by-charactor li.title");
    //                $.each(keyFilter, function(i, key) {
    //                    var $key = $(key);
    //                    if ($key.text().toLowerCase() == item.value) {
    //                        $key.addClass("key-filter-active");
    //                        return false;
    //                    }
    //                });
    //            } else {
    //                obj.find("th.k-header[data-field='" + item.field + "']").first().children(".k-header-column-menu").addClass("k-highlight");
    //            }
    //        });
    //    }
    //} else { //mark highlight fields in filter in KeyFilter does not exist
    //    obj.find("th.k-header > .k-header-column-menu").removeClass("k-highlight");
    //    var ft = e.sender.dataSource.filter();
    //    if (ft != null) {
    //        var fields = $.gridState.lookdeepFieldName(ft.filters);
    //        $.each(fields, function (k, item) {
    //            if (item.field != "KeyFilter") {
    //                obj.find("th.k-header[data-field='" + item.field + "']").first().children(".k-header-column-menu").addClass("k-highlight");
    //            }
    //        });
    //    }
    //}
    
    //if (obj.is(":visible")) {
    //} else {
    //    obj.removeClass("hide");
    //    $("#loading").addClass("hide");
    //}

    ////var hash = window.location.hash;
    ////var temp = $.unserialize(hash);
    ////if (!obj.attr("reload") && temp && temp.name == "kendoGrid" && temp.id == $(obj).attr("id")) {
    ////    obj.addClass("hide");
    ////    obj.attr("reload", true);
    ////    e.sender.dataSource.page(temp.page);
    ////} else {
    ////    obj.removeClass("hide");
    ////}
    
    //if (!obj.hasClass("fixed")) {
    //    var gridHeight = 0;
    //    if (obj.attr('gridMargin') > 0) {
    //        gridHeight = $("body").height() - $('#header').outerHeight(true) - obj.attr('gridMargin');
    //    } else {
    //        gridHeight = $("body").height() - (obj.find('.k-grid-toolbar').outerHeight(true) + obj.find('.k-grid-header').outerHeight(true) + 178);    
    //    }
    //    obj.find(".k-grid-content").height(gridHeight);
    //}
    ////obj.find(".k-grid-content").niceScroll();
    
    //if (!obj.attr("keyboard")) {
    //    obj.attr("keyboard", true);
    //    obj.attr("tabindex", 0);

    //    obj.keydown(function (ke) {
    //        var kGrid, curRow, newRow;

    //        kGrid = $(this).closest('.k-grid').data("kendoGrid");

    //        curRow = kGrid.select();
    //        if (!curRow.length)
    //            return;

    //        var kContent = $(obj).children(".k-grid-content");
    //        var contentHeight = kContent.height();

    //        if (ke.which == 38) {
    //            newRow = curRow.prev();
    //            var row = $(curRow).closest("tr");

    //            $(kContent).getNiceScroll().each(function (key, val) {
    //                var self = this;
    //                var top = self.getScrollTop();
    //                var rowTop = row.position().top;

    //                if (rowTop < 0) {
    //                    self.setScrollTop(top + (rowTop - row.height()));
    //                } else if (rowTop > contentHeight) {
    //                    self.setScrollTop(top + (rowTop - contentHeight));
    //                } else if (rowTop <= row.height() && top > 0) {
    //                    self.setScrollTop(top - row.height());
    //                }
    //            });
    //        } else if (ke.which == 40) {
    //            newRow = curRow.next();
    //            var row = $(curRow).closest("tr");

    //            $(kContent).getNiceScroll().each(function (key, val) {
    //                var self = this;
    //                var top = self.getScrollTop();
    //                var rowTop = row.position().top;

    //                if (rowTop < 0) {
    //                    self.setScrollTop(top + (rowTop + row.height()));
    //                } else if (rowTop > contentHeight) {
    //                    self.setScrollTop(top + (rowTop - contentHeight + 2 * row.height()));
    //                } else if (rowTop >= contentHeight - 2 * row.height()) {
    //                    self.setScrollTop(top + row.height());
    //                }
    //            });
    //        } else {
    //            return;
    //        }

    //        if (!newRow.length)
    //            return;

    //        kGrid.select(newRow);
    //    });
    //}
    
    ////check grid is Account data
    //var gridId = obj.attr('id');
    //if (gridId != undefined && gridId.toLowerCase().indexOf('account') > -1) {
    //    var data = this.dataSource.data();
    //    var table = this.tbody;
    //    $.each(data, function (i, row) {
    //        //apply rule here
    //        if (row.ActiveAccount != undefined && row.ActiveAccount == false) {
    //            $(table.children()[i]).addClass("in-active");
    //        }
    //        else if (row.CreditHold == true) {
    //            $(table.children()[i]).addClass("credit-hold");
    //        }
    //    });
    //    $('.help-text').removeClass('hide');
    //}

    //if (window.setaJs.hashUrlIsEnable()) {
    //    // Reload to Previous page
    //    if (this.dataSource.view().length == 0) {
    //        var currentPage = this.dataSource.page();
    //        if (currentPage > 1) {
    //            this.dataSource.page(currentPage - 1);
    //        }
    //    }
        
    //    //http://www.kendoui.com/code-library/mvc/grid/enable-shareable-urls-of-ajax-bound-grid-via-the-browser-history-api.aspx
    //    var grid = $('#' + $(obj).attr("id")).data('kendoGrid');

    //    var baseUri = window.setaJs.oGlobal.RootURL.substr(-1) + grid.dataSource.transport.options.read.url;

    //    //$('#' + $(obj).attr("id")).data('kendoGrid');
    //    // ask the parameterMap to create the request object for you
    //    var requestObject = (new kendo.data.transports["aspnetmvc-server"]({ prefix: "" }))
    //    .options.parameterMap({
    //        page: grid.dataSource.page(),
    //        sort: grid.dataSource.sort(),
    //        filter: grid.dataSource.filter()
    //    });

    //    if (baseUri.toLowerCase().indexOf("grid-page") == -1) { // not found
    //        // Get its 'href' attribute - the URL where it would navigate to
    //        var href = baseUri + "?Grid-page=~&Grid-pageSize=~&Grid-filter=~&Grid-sort=~";

    //        // Update the 'page' parameter with the grid's current page
    //        href = href.replace(/page=([^&]*)/, 'page=' + requestObject.page || '~');


    //        // Update the 'sort' parameter with the grid's current sort descriptor
    //        href = href.replace(/sort=([^&]*)/, 'sort=' + requestObject.sort || '~');

    //        // Update the 'pageSize' parameter with the grid's current pageSize
    //        href = href.replace(/pageSize=([^&]*)/, 'pageSize=' + grid.dataSource._pageSize);

    //        //update filter descriptor with the filters applied

    //        href = href.replace(/filter=([^&]*)/, 'filter=' + (requestObject.filter || '~'));



    //        window.setaJs.HashURL.historyUrl.pushState({}, "URL Rewrite Example", href);
    //        //window.setaJs.HashURL.pushState({}, "URL Rewrite Example", href);
    //    }

    //    //re event state hash url
    //    window.setaJs.HashURL.pushStateHashURL();
    //}
    //if (obj.attr("hiddenId") != null && obj.attr("checkAllId") != null && obj.attr("checkClass") != null) {
    //    window.setaJs.modules.common.method.selectionListViewDataBound(this, obj.attr("hiddenId"), obj.attr("checkAllId"), obj.attr("checkClass"));
    //}
};
var showGridfixArrowKey = function (e) {
    var obj = $(e.sender.wrapper);

    fixKendoGrid.apply(this, [e.sender]);

    obj.find(".k-grid-content").css('overflow', 'hidden');
    obj.find(".k-grid-header").addClass("k-grid-header-fixed");
    //filter by alphabeta
    if ($("ul.left-filter-by-charactor").length > 0) {
        obj.append($("ul.left-filter-by-charactor").removeClass('hide'));
        obj.find("th.k-header > .k-header-column-menu").removeClass("k-highlight");
        obj.find("ul.left-filter-by-charactor li.title").removeClass("key-filter-active");
        var ft = e.sender.dataSource.filter();
        if (ft != null) {
            var fields = $.gridState.lookdeepFieldName(ft.filters);
            $.each(fields, function (k, item) {
                if (item.field == "KeyFilter") {
                    var keyFilter = obj.find("ul.left-filter-by-charactor li.title");
                    $.each(keyFilter, function (i, key) {
                        var $key = $(key);
                        if ($key.text().toLowerCase() == item.value) {
                            $key.addClass("key-filter-active");
                            return false;
                        }
                    });
                } else {
                    obj.find("th.k-header[data-field='" + item.field + "']").first().children(".k-header-column-menu").addClass("k-highlight");
                }
            });
        }
    } else { //mark highlight fields in filter in KeyFilter does not exist
        obj.find("th.k-header > .k-header-column-menu").removeClass("k-highlight");
        var ft = e.sender.dataSource.filter();
        if (ft != null) {
            var fields = $.gridState.lookdeepFieldName(ft.filters);
            $.each(fields, function (k, item) {
                if (item.field != "KeyFilter") {
                    obj.find("th.k-header[data-field='" + item.field + "']").first().children(".k-header-column-menu").addClass("k-highlight");
                }
            });
        }
    }
    
    if (obj.is(":visible")) {
    } else {
        obj.removeClass("hide");
        $("#loading").addClass("hide");
    }

    if (!obj.hasClass("fixed")) {
        var gridHeight = $("body").height() - (obj.find('.k-grid-toolbar').outerHeight(true) + obj.find('.k-grid-header').outerHeight(true) + 178);
        obj.find(".k-grid-content").height(gridHeight);
    }
    //obj.find(".k-grid-content").niceScroll();

    if (!obj.attr("keyboard")) {
        obj.attr("keyboard", true);
        obj.attr("tabindex", 0);

        obj.keydown(function (ke) {
            var kGrid, curRow, newRow;

            kGrid = $(this).closest('.k-grid').data("kendoGrid");

            curRow = kGrid.select();
            if (!curRow.length)
                return;

            var kContent = $(obj).children(".k-grid-content");
            var contentHeight = kContent.height();

            if (ke.which == 38) {
                newRow = curRow.prev();
                var row = $(curRow).closest("tr");

                $(kContent).getNiceScroll().each(function (key, val) {
                    var self = this;
                    var top = self.getScrollTop();
                    var rowTop = row.position().top;

                    if (rowTop < 0) {
                        self.setScrollTop(top + (rowTop - row.height()));
                    } else if (rowTop > contentHeight) {
                        self.setScrollTop(top + (rowTop - contentHeight));
                    } else if (rowTop <= row.height() && top > 0) {
                        self.setScrollTop(top - row.height());
                    }
                });
            } else if (ke.which == 40) {
                newRow = curRow.next();
                var row = $(curRow).closest("tr");

                $(kContent).getNiceScroll().each(function (key, val) {
                    var self = this;
                    var top = self.getScrollTop();
                    var rowTop = row.position().top;

                    if (rowTop < 0) {
                        self.setScrollTop(top + (rowTop + row.height()));
                    } else if (rowTop > contentHeight) {
                        self.setScrollTop(top + (rowTop - contentHeight + 2 * row.height()));
                    } else if (rowTop >= contentHeight - 2 * row.height()) {
                        self.setScrollTop(top + row.height());
                    }
                });
            } else {
                return;
            }

            if (!newRow.length)
                return;

            kGrid.select(newRow);
        });
    }

};

var RecordDetailRefesh = function (moduleId, objectId) {
    var _moduleId = { AccountId: 2, ContactId: 1, ServiceTicketId: 3, CampaignId: 5, OpportunityId: 4 };
    switch (parseInt(moduleId)) {
        case _moduleId.AccountId:
            {
                accountController.onLoadRecordDetails(objectId);
            } break;
        case _moduleId.ContactId:
            {
                contactController.onLoadRecordDetails(objectId);
            } break;
        case _moduleId.ServiceTicketId:
            {
                serviceticketController.onLoadRecordDetails(objectId);
            } break;
        case _moduleId.CampaignId:
            {
                campaignController.onLoadRecordDetails(objectId);
            } break;
        case _moduleId.OpportunityId:
            {
                opportunityController.onLoadRecordDetails(objectId);
            } break;
        default:
    }
};
var initCheckBox = function (panel) {
    $(panel).iCheck({
        checkboxClass: 'icheckbox_flat-green',
        radioClass: 'iradio_flat-green'
    });
};

var initRadioButton = function (panel) {
    $(panel).iCheck({
        checkboxClass: 'icheckbox_flat-green',
        radioClass: 'iradio_flat-green'
    });
};

/* Fix kendo Comboxbox */
var kendoComboboxFix = {
    changeAllowEmpty: function (e) {
        var valid = kendoComboboxFix.change(e, this.value());
        if (!valid) {
            e.sender.value(null);
        }
    },
    dataBoundAllowEmpty: function (e) {
        var valid = kendoComboboxFix.dataBound(e, this.value());
        if (!valid) {
            e.sender.value(null);
        }
    },
    change: function (e) {
        var value;
        if (arguments.length == 1) {
            value = this.value();
        } else {
            value = arguments[1];
        }

        var valid = false;
        var dataSource = e.sender.dataSource.data();
        if (dataSource.length == 0) {
            e.sender.value(null);
            return;
        }

        if (value !== "") {
            $.each(dataSource, function (k, v) {
                if (v.Text == value || v.Value == value) {
                    e.sender.value(v.Value);
                    valid = true;
                    return false;
                }
            });
        }

        if (!valid) {
            $.each(dataSource, function (k, v) {
                if ((typeof (v.Selected) == "boolean" && v.Selected == true) || (typeof (v.Selected) == "string" && v.Selected.toLowerCase() == "true")) {
                    e.sender.value(v.Value);
                    valid = true;
                    return false;
                }
            });
        }

        if (!valid) {
            e.sender.value(dataSource[0].Value);
        }
        return valid;
    },
    dataBound: function (e) {
        var value;
        if (arguments.length == 1) {
            value = this.value();
        } else {
            value = arguments[1];
        }
        var valid = false;
        var dataSource = e.sender.dataSource.data();

        if (dataSource.length == 0) {
            e.sender.value(null);
            return;
        }

        if (value !== "") {
            $.each(dataSource, function (k, v) {
                if (v.Text == value || v.Value == value) {
                    e.sender.value(v.Value);
                    valid = true;
                    return false;
                }
            });
        }
        if (!valid) {
            $.each(dataSource, function (k, v) {
                if ((typeof (v.Selected) == "boolean" && v.Selected == true) || (typeof (v.Selected) == "string" && v.Selected.toLowerCase() == "true")) {
                    e.sender.value(v.Value);
                    valid = true;
                    return false;
                }
            });
        }

        if (!valid) {
            e.sender.value(dataSource[0].Value);
        }
        return valid;
    },

    setDefaultValue: function (e) {
        var value = this.value();
        var valid = false;

        if (e.sender.dataSource.data().length == 0) {
            e.sender.value(null);
            return;
        }

        $.each(e.sender.dataSource.data(), function (k, v) {
            if ((typeof (v.Selected) == "boolean" && v.Selected == true) || (typeof (v.Selected) == "string" && v.Selected.toLowerCase() == "true")) {
                e.sender.value(v.Value);
                valid = true;
                return false;
            }
        });

        if (!valid) {
            e.sender.value(e.sender.dataSource.data()[0].Value);
        }
    },

    scrollFixHeader: function (e) {
        if (e.sender.header !== undefined) {
            $(e.sender.ul).addClass('combobox-header');
            var header = $(e.sender.header);
            header.addClass('text-ellipsis').attr('title', header.children('span').html());
            $(e.sender.list).append(header);
            //var listHeight = parseInt($(e.sender.list).height());
            //if ($(e.sender.list)[0].style.height != 'auto') {
            //    $(e.sender.ul).css('max-height', listHeight - parseInt($(e.sender.header).height()));
            //} else {
            //    $(e.sender.ul).css('max-height', '');
            //}
            if (typeof header != "undefined") {
                //e.sender.ul.removeClass("k-list");
                e.sender.ul.css("height", "90%");
            }
        }
    },
};

(function ($, window, document, undefined) {
    var scroll = function () {
    };
    scroll.prototype = {
        resize: function () {
            setTimeout(function () {
                $.nicescroll.each(function () {
                    this.resize();
                });
            }, 100);
        },
        init: function () {
        }
    };

    $.scroll = new scroll();
    //$(document).ready(function () {
    //    $.scroll.init();
    //});

})(jQuery, window, document);

(function ($, window, document) {
    var windowState = function () {
    };

    windowState.prototype = {
        toggleMainTriggers: [],
        addToggleMainTrigger: function (func) {
            var that = this;
            if (typeof (func) == "function") {
                that.toggleMainTriggers.push(func);
            }
        },
        init: function () {
            var that = this;

            $(document).delegate(".crm-toggle-child", "click", function (e) {
                e.preventDefault();
                e.stopPropagation();
                var parentKey = $(this).attr("parentkey");
                var parentToggleClass = $(this).attr("parenttoggleclass");

                var objs = $(document).find("[follow='" + parentKey + "']");
                var state = objs.first().hasClass(parentToggleClass);

                if (state) {
                    $(objs).removeClass(parentToggleClass);
                    $.ajax({
                        url: "/State/WindowState",
                        type: "GET",
                        data: {
                            key: parentKey,
                            state: (state ? 1 : 2)
                        },
                        dataType: "json",
                        success: function (response) {
                            if (response.status) {

                            }
                        }
                    });

                    var key = $(this).attr("key");
                    var toggleClass = $(this).attr("toggleclass");

                    objs = $(document).find("[follow='" + key + "']");
                    state = objs.first().hasClass(toggleClass);

                    if (state) {
                        $(objs).removeClass(toggleClass);
                        $.ajax({
                            url: "/State/WindowState",
                            type: "GET",
                            data: {
                                key: key,
                                state: (state ? 1 : 2)
                            },
                            dataType: "json",
                            success: function (response) {
                                if (response.status) {

                                }
                            }
                        });
                    }

                } else {
                    var key = $(this).attr("key");
                    var toggleClass = $(this).attr("toggleclass");

                    objs = $(document).find("[follow='" + key + "']");
                    state = objs.first().hasClass(toggleClass);

                    if (state) {
                        $(objs).removeClass(toggleClass);
                    } else {
                        $(objs).addClass(toggleClass);
                    }

                    $.ajax({
                        url: "/State/WindowState",
                        type: "GET",
                        data: {
                            key: key,
                            state: (state ? 1 : 2)
                        },
                        dataType: "json",
                        success: function (response) {
                            if (response.status) {

                            }
                        }
                    });
                }

                //$.scroll.resize();
            });

            $(document).delegate(".crm-toggle", "click", function (e) {
                e.preventDefault();
                e.stopPropagation();
                var key = $(this).attr("key");
                var toggleClass = $(this).attr("toggleClass");

                var objs = $(document).find("[follow='" + key + "']");
                var state = objs.first().hasClass(toggleClass);

                if (state) {
                    $(objs).removeClass(toggleClass);
                } else {
                    $(objs).addClass(toggleClass);
                }

                $.ajax({
                    url: "/State/WindowState",
                    type: "GET",
                    data: {
                        key: key,
                        state: (state ? 1 : 2)
                    },
                    dataType: "json",
                    success: function (response) {
                        if (response.status) {

                        }
                    }
                });

                $.each(that.toggleMainTriggers, function (k, func) {
                    func();
                });

                //$.scroll.resize();
            });

        }
    };

    var gridState = function () {
    };

    var gridKeyFilter = "";
    gridState.prototype = {
        init: function () {
            var that = this;
            $(document).find("[gridstate='true']").each(function () {
                var grid = $(this).data("kendoGrid");
                if (grid == undefined) return;
                grid.bind("columnReorder", function (e) {
                    that.reorder.apply(that, [e]);
                });
                grid.bind("columnResize", function (e) {
                    that.resize.apply(that, [e]);
                });
                grid.bind("columnHide", function (e) {
                    that.hide.apply(that, [e]);
                });
                grid.bind("columnShow", function (e) {
                    that.show.apply(that, [e]);
                });
                grid.bind("dataBinding", function (e) {
                    //var ft = e.sender.dataSource.filter();
                    //var wrapper = $(e.sender.wrapper);
                    //wrapper.append($("ul.left-filter-by-charactor").removeClass('hide'));
                    /*wrapper.find("th.k-header > .k-header-column-menu").removeClass("k-highlight");
                    wrapper.find("ul.left-filter-by-charactor li.title").removeClass("key-filter-active");
                    if (ft != null) {
                        //$.each(ft.filters, function (k, filter) {
                        //    if (filter.field == "KeyFilter") {
                        //        var keyFilter = wrapper.find("ul.left-filter-by-charactor li.title");
                        //        $.each(keyFilter, function (k, key) {
                        //            var $key = $(key);
                        //            if ($key.text().toLowerCase() == filter.value) {
                        //                $key.addClass("key-filter-active");
                        //                return false;
                        //            }
                        //        });
                        //    } else {
                        //        wrapper.find("th.k-header[data-field='" + filter.field + "']").first().children(".k-header-column-menu").addClass("k-highlight");
                        //    }
                        //});
                        var fields = that.lookdeepFieldName(ft.filters);//.unique();
                        $.each(fields, function (k, name) {
                            if (name == "KeyFilter") {
                                var keyFilter = wrapper.find("ul.left-filter-by-charactor li.title");
                                $.each(keyFilter, function (i, key) {
                                    console.log($(key));
                                    var $key = $(key);
                                    if ($key.text().toLowerCase() == gridKeyFilter) {
                                        $key.addClass("key-filter-active");
                                        return false;
                                    }
                                });
                            } else {
                                wrapper.find("th.k-header[data-field='" + name + "']").first().children(".k-header-column-menu").addClass("k-highlight");
                            }
                        });
                    }*/
                    var currentSortField = $(e.sender.wrapper).attr("currentsortfield");
                    var currentSortDir = $(e.sender.wrapper).attr("currentsortdir");
                    if (typeof currentSortField !== 'undefined' && currentSortField !== false) {
                        var sorts = grid.dataSource.sort();
                        if (sorts == null || sorts.length == 0) {
                            if (currentSortField == undefined || currentSortField == "") {

                            } else {
                                that.change(e.sender);
                                $(e.sender.wrapper).attr("currentsortfield", "");
                                $(e.sender.wrapper).attr("currentsortdir", "");
                            }
                        } else {
                            if (currentSortField == sorts[0].field && currentSortDir == sorts[0].dir) {

                            } else {
                                that.change(e.sender);
                                $(e.sender.wrapper).attr("currentsortfield", sorts[0].field);
                                $(e.sender.wrapper).attr("currentsortdir", sorts[0].dir);
                            }
                        }
                    }
                    
                });
            });
        },
        lookdeepFieldName: function (obj) {
            var that = this;
            var A = [], tem;
            for (var p in obj) {
                if (obj.hasOwnProperty(p)) {
                    tem = obj[p];
                    if (tem && typeof tem == 'object') {
                        if (tem.field != undefined) {
                            var item = new Object();
                            item.field = tem.field;
                            if (tem.field == "KeyFilter")
                                //gridKeyFilter = tem.value;
                                item.value = tem.value;
                            A[A.length] = item;
                        } else {
                            A = A.concat(arguments.callee(tem));
                        }
                    }
                }
            }
            return A;
        },
        columnsToObject: function (columns) {            
            var that = this;
            var data = [];
            $.each(columns, function (k, column) {
                if (column.hasOwnProperty("attributes") && column.attributes.hasOwnProperty("state")) {
                    var item = that.getObjectFromColumn(k, column);
                    if (item != null)
                        data.push(item);
                }
            });
            return data;
        },
        getObjectFromColumn: function (k, column) {
            if (typeof (column) == "object" && column.hasOwnProperty("field") && column.hasOwnProperty("width")) {
                var obj = {
                    Field: column.field,
                    Width: typeof (column.width) == "number" ? column.width : 0,
                    Visiable: true,
                    Position: k,
                    IncludeInMenu: true
                };

                if (column.hasOwnProperty("hidden"))
                    obj.Visiable = !column.hidden;
                if (column.hasOwnProperty("menu"))
                    obj.IncludeInMenu = column.menu;
                return obj;
            }
            return null;
        },
        pushData: function (key, columns, sort, layoutId) {
            $.ajax({
                url: "/State/GridState",
                type: "POST",
                cache: false,
                async: true,
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify({
                    key: key,
                    columns: columns,
                    sort: sort,
                    layoutId: layoutId
                }),
                success: function (response) {

                }
            });
        },
        change: function (grid) {
            if (grid.notGridState) return;
            var that = this;
            var key = $(grid.wrapper).attr("key");
            var layoutId = $(grid.wrapper).attr("layoutId");           
            setTimeout(function () {
                var columns = that.columnsToObject(grid.columns);
                var sort = null;

                var sorts = grid.dataSource.sort();
                if (sorts != null && sorts.length > 0) {
                    sort = {
                        Field: sorts[0].field,
                        Dir: sorts[0].dir
                    };
                }
                that.pushData(key, columns, sort, layoutId);
            }, 100);
        },
        // e.column Object, e.newIndex Number, e.oldIndex Number, e.sender kendo.ui.Grid
        reorder: function (e) {
            var that = this;
            that.change(e.sender);
        },
        // e.column Object, e.newWidth Number, e.oldWidth Number, e.sender kendo.ui.Grid
        resize: function (e) {
            var that = this;
            that.change(e.sender);
        },
        // e.column Object, e.sender kendo.ui.Grid
        hide: function (e) {
            var that = this;
            that.change(e.sender);
        },
        // e.column Object, e.sender kendo.ui.Grid
        show: function (e) {
            var that = this;
            that.change(e.sender);
        }
    };

    $.windowState = new windowState();
    $.gridState = new gridState();

    $(document).ready(function () {
        $.windowState.init();
        $.gridState.init();

        // Add nicescroll for page search
        $('#search-wrapper').height($("body").height() - $('#header').outerHeight(true));
        //$('#search-wrapper').niceScroll();
    });

})(jQuery, window, document);

(function ($, window, document, setaJs, undefined) {
    var timeZone = function () { };
    timeZone.prototype = {
        currentTimeZone: null,
        init: function () {
            var that = this;
            var tz = $("body").attr("timezoneoffset");
            if (tz != undefined) {
                that.currentTimeZone = parseInt(tz);
                var localTimeZone = that.getLocalTimeZone();
                var currentTimeZone = that.currentTimeZone;
                if (Math.abs(localTimeZone - that.currentTimeZone) >= 30) {
                    var hasSetTimezone = $("body").attr("timezoneHasSet");
                    if (hasSetTimezone == undefined || hasSetTimezone == 'False') {
                        $("#timezone").removeClass("hide");
                    }
                    $("#timezone").delegate("a.btn-timezone-yes", "click", function (e) {
                        e.preventDefault();
                        var btn = this;
                        var name = that.getLocalTimeName(localTimeZone);

                        window.setaJs.msgConfirm({
                            text: "Your default time zone has been set to " + name + ".",
                            buttons: [{
                                text: 'No',
                                onClick: function ($noty) {
                                    $noty.close();
                                }
                            }, {
                                text: 'Yes',
                                onClick: function ($noty) {
                                    $noty.close();
                                    $.ajax({
                                        url: $(btn).attr("href"),
                                        type: "POST",
                                        dataType: "json",
                                        data: {
                                            offset: localTimeZone
                                        },
                                        success: function (response) {
                                            if (response.status)
                                                window.location.reload(false);
                                            else
                                                $("#timezone").addClass("hide");
                                        }
                                    });
                                }
                            }]
                        });
                    });

                    $("#timezone").delegate("a.btn-timezone-no", "click", function (e) {
                        e.preventDefault();
                        var btn = this;
                        $.ajax({
                            url: $(btn).attr("href"),
                            type: "POST",
                            dataType: "json",
                            data: {
                                offset: currentTimeZone
                            },
                            success: function (response) {
                                $("#timezone").addClass("hide");
                            }
                        });
                    });
                }
            }
        },
        getLocalTimeName: function (localTimeZone) {
            var name = "";
            var max = Number.MAX_VALUE;
            $.each(setaJs.oGlobal.TimezoneList, function (k, v) {
                var curr = Math.abs(v.offset - localTimeZone);
                if (curr < max) {
                    name = v.name;
                    max = curr;
                }
            });
            return name;
        },
        getLocalTimeZone: function () {
            var offset = new Date().getTimezoneOffset();
            return parseInt(offset);
        }
    };

    //$.timeZone = new timeZone();

    //$(document).ready(function () {
    //    $.timeZone.init();
    //});
})(jQuery, window, document, setaJs);

function kendoNullToString(value) {
    return (value == null) ? '' : value;
}
//format number: c2: currency, 2 precision| n2: number, 2 precision
//format datetime: yyyy/MM/dd hh:mm:ss tt
function kendoToStringFormat(value, format) {
    return value == null ? '' : kendo.toString(value, format); //using kendo format
}

function kendoSearchFocus(e) {
    var ft = e.sender.dataSource.filter();
    $(e.sender.wrapper).find("th.k-header > .k-header-column-menu").removeClass("k-highlight");
    if (ft != null) {
        $.each(ft.filters, function (k, filter) {
            $(e.sender.wrapper).find("th.k-header[data-field='" + filter.field + "']").first().children(".k-header-column-menu").addClass("k-highlight");
        });
    }
}

var IsHashUrl = {
    mainUrl: location.href,
    mygrid: new Array(),
    arrayCookies: new Array(),
    Init: function (girdArray) {
        if (typeof (girdArray) != "undefined") IsHashUrl.mygrid = girdArray.split(',');
        IsHashUrl.LoadPushState();
        if (IsHashUrl.mygrid.length > 0) {
            for (var i = 0; i < IsHashUrl.mygrid.length; i++) {
                $(IsHashUrl.mygrid[i]).data('kendoGrid').bind("dataBound", function () {
                    var requestObject = (new kendo.data.transports["aspnetmvc-server"]({ prefix: "" }))
                        .options.parameterMap({
                            page: this.dataSource.page(),
                            sort: this.dataSource.sort(),
                            filter: this.dataSource.filter(),
                            group: this.dataSource.group()
                        });
                    if (requestObject.page == '') {
                        requestObject.page = 1;
                    }
                    var getId = this.wrapper.attr('id');
                    var entity = new Object();
                    entity.setPage = requestObject.page;
                    entity.setSize = this.dataSource._pageSize;
                    entity.setFilter = requestObject.filter;
                    entity.setsort = requestObject.sort;
                    entity.setgroup = requestObject.group;
                    entity.Url = this.dataSource.transport.options.read.url;
                    entity.mainUrl = IsHashUrl.mainUrl;
                    IsHashUrl.RemoveItem(IsHashUrl.arrayCookies, getId);
                    IsHashUrl.arrayCookies.push({ name: getId, value: entity });
                    $.cookie("objStore", JSON.stringify(IsHashUrl.arrayCookies));
                });
            }
        }
    },
    LoadPushState: function () {
        if ($.cookie("objStore") != null) {
            IsHashUrl.arrayCookies = $.parseJSON($.cookie("objStore"));
            if (this.mygrid.length > 0 && IsHashUrl.arrayCookies.length > 0) {
                for (var i = 0; i < this.mygrid.length; i++) {
                    var grid = $(this.mygrid[i]).data('kendoGrid');
                    $(grid.wrapper).removeClass("hide");
                    for (var x = 0; x < IsHashUrl.arrayCookies.length; x++) {
                        if (IsHashUrl.arrayCookies[x].name == this.mygrid[i].replace("#", "")) {
                            var obj = new Object();
                            obj = IsHashUrl.arrayCookies[x].value;
                            grid.dataSource.query({
                                page: obj.setPage,
                                pageSize: obj.setSize,
                                //sort: IsHashUrl.SortAndFilter(obj.setsort, 1),
                                sort: grid.dataSource.sort(),
                                filter: IsHashUrl.SortAndFilter(obj.setFilter, 0)
                            });
                        }
                    }
                }
            }
        } else {
            if (this.mygrid.length > 0) {
                IsHashUrl.arrayCookies.length = 0;
                for (var j = 0; j < this.mygrid.length; j++) {
                    var dgrid = $(this.mygrid[j]).data('kendoGrid');
                    $(dgrid.wrapper).removeClass("hide");
                    dgrid.dataSource.read();
                }
            }
        }
    },
    SortAndFilter: function (strquery, type) {
        var temp = new Array();
        if (type == 0) {
            var returnArray = new Array();
            if (strquery != '') {
                temp = strquery.split("~and~");
                if (temp.length > 0) {
                    for (var i = 0; i < temp.length; i++) {
                        if (temp[i] != '') {
                            var getObj = new Array();
                            getObj = temp[i].split("~");
                            if (getObj.length === 3) {
                                var objfilter = new Object();
                                objfilter.field = getObj[0].trim();
                                objfilter.operator = getObj[1].trim();
                                objfilter.value = getObj[2].trim().substring(1, getObj[2].length - 1);
                                objfilter.value = decodeURIComponent(objfilter.value);
                                if (objfilter.field == "KeyFilter") {
                                    objfilter.value = objfilter.value.replace("%", "").trim();
                                }
                                returnArray.push(objfilter);
                            }
                        }
                    }
                }
            }
            return returnArray;
        } else {
            var objsort = new Object();
            objsort.field = "";
            objsort.dir = "";
            if (strquery != '') {
                temp = strquery.split("-");
                if (temp.length === 2) {
                    objsort.field = temp[0].trim();
                    objsort.dir = temp[1].trim();
                }
            }
            return objsort;
        }
    },
    RemoveItem: function (array, setName) {
        for (var i in array) {
            if (array[i].name == setName) {
                array.splice(i, 1);
            }
        }
    },
};


// Dropdown tooltip
(function($) {
    $.fn.addTooltip = function () {
        //return this.each(function () {
        //    var that = $(this);
        //    var originalElements = that.find('input,select');
        //    for (var i = 0; i < originalElements.length; i++) {
        //        var kendoElement = $(originalElements[i]).data("kendoDropDownList");
        //        if (kendoElement != undefined) {
        //            kendoElement.wrapper.attr('title', kendoElement.text());
        //            kendoElement.bind('change', function(e) {
        //                this.wrapper.attr('title', this.text());
        //            });
        //         }
        //    }       
        //});
    };
    $(function() {
        $('.k-widget.k-dropdown').addTooltip();
    });
})(jQuery);

//Array.prototype.unique = function () {
//    var a = this.concat();
//    for (var i = 0; i < a.length; ++i) {
//        for (var j = i + 1; j < a.length; ++j) {
//            if (a[i] === a[j])
//                a.splice(j--, 1);
//        }
//    }
//    return a;
//};

//custom datepicker when change date reset hours
function datetimePickerCustom (element) {
    var viewType = '';
    var valueCurrent = new Date();
    element.kendoDateTimePicker({
        open: function (e) {
            viewType = e.view;
            var parents = $(e.sender.element).closest('div.k-animation-container');
            parents.find('div.k-list-container.k-popup.k-group.k-reset[data-role="popup"]').css('height', '200px');
        },
        change: function () {
            if (viewType == 'date') {
                var resetHours = new Date((this.value()).setHours(0));
                valueCurrent = resetHours.setMinutes(0);
                this.value(new Date(valueCurrent));
            }
        }
    });
}

function getCurrentMenu (controller) {
    var menus = controller.menuColumn;
    var currentField = controller.currentField;
    if (menus && menus.dataField) {
        var indexOfCurrentField = menus.dataField.indexOf(currentField);
        return menus.menu[indexOfCurrentField];
    }
    return null;
}

function handleGridColumnMenuInit(controller, e) {
    // Add Column Menu to menuColumn
    controller.menuColumn.dataField.push(e.sender.element.attr('id') + "." + e.field);
    controller.menuColumn.menu.push(e.container);
}

function onColumnMenuClick(e) {
    if (!e || !e.data)
        return;
    
    var controller = e.data.controller;
    if (!controller)
        return;
    
    controller.currentField = e.data.key + "." + e.currentTarget.parentElement.attributes["data-field"].value;
    showHideGridColumnMenuItem(controller);
}

function showHideGridColumnMenuItem (controller) {
    var menu = getCurrentMenu(controller);
    if (!menu)
        return;

    var status = controller.typeStatus;
    var statusCount = controller.privateColumn.length;
    // Show only Column Menu Item based on Opportunity Status, others are hidden
    for (var i = 0; i < statusCount; i++) {
        if (i != status - 1) {
            hideColumnMenuItem(menu, controller.privateColumn[i]);
        }
    }
    showColumnMenuItem(menu, controller.privateColumn[status - 1]);
    showColumnMenuItem(menu, controller.commonColumns);
}

function showHideGridColumn (grid, controller) {

    var status = controller.typeStatus;
    var statusCount = controller.privateColumn.length;
    // Show only Column based on Opportunity Status, others are hidden
    for (var i = 0; i < statusCount; i++) {
        if (i != status - 1) {
            hideColumn(grid, controller.privateColumn[i]);
        }
    }
    showColumn(grid, controller.privateColumn[status - 1]);
    showColumn(grid, controller.commonColumns);
}

function showColumnMenuItem (container, currentColumns) {
    for (var i = 0; i < currentColumns.length; i++) {
        var item = container.find(":checkbox[data-field='" + currentColumns[i] + "']").parents("li .k-item");
        item.show();
    }
}

function hideColumnMenuItem (container, currentColumns) {
    for (var i = 0; i < currentColumns.length; i++) {
        var item = container.find(":checkbox[data-field='" + currentColumns[i] + "']").parents("li .k-item");
        item.hide();
    }
}

function showColumn (kendoGrid, currentColumns) {
    for (var i = 0; i < currentColumns.length; i++) {
        kendoGrid.showColumn(currentColumns[i]);
    }
}

function hideColumn (kendoGrid, currentColumns) {
    for (var i = 0; i < currentColumns.length; i++) {
        kendoGrid.hideColumn(currentColumns[i]);
    }
}