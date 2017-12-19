$.urlParam = function (name) {
    var results = new RegExp('[\?&]' + name + '=([^&#]*)').exec(window.location.href);
    if (results == null) {
        return null;
    }
    else {
        return results[1] || 0;
    }
}

$.fn.hasScrollBar = function () {
    return this.get(0).scrollHeight > this.height();
}

var STBASE = {
    exp: -1,
    DEFAULTS : {
        delimiter: '&',
        keyValueSeparator: '=',
        startAfter: '?',
    },
    init: function () {
        STBASE.ActionNoti();
    },
    ActiveNav: function () {
        var urlPath = location.pathname;
        var checkR = jQuery('ul.page-sidebar-menu > li.active > a').attr("href");
        var strurlPath = urlPath.replace(/\//g, '');
        if (strurlPath == 'Resources') {
            return false;
        }
        jQuery('ul.page-sidebar-menu > li').removeClass('active');
        var searches = location.search.replace('?', '').split('&');
        var checkExistUrl = 0;
        var activeHref;
        jQuery('ul.page-sidebar-menu').find('a').each(function() {
            var hrefUrl = jQuery(this).attr("href");
            var ahref = hrefUrl.split('?');
            if (ahref.length > 1) {
                var base = ahref[0].split('/')[1];
                var queries = ahref[1].split('&');
                if (base == urlPath.split('/')[1]) {
                    for (var i = 0; i < searches.length; i++) {
                        for (var j = 0; j < queries.length; j++) {
                            if (searches[i] == queries[j]) {
                                checkExistUrl = 1;
                                activeHref = hrefUrl;
                            }
                        }
                    }
                }
            }
            else {
                if (hrefUrl == urlPath) {
                    checkExistUrl = 1;
                    activeHref = hrefUrl;
                }
            }
        });

        if (location.pathname.split("/").length >= 3 && checkExistUrl == 0) {
            urlPath = "/" + location.pathname.split("/")[1];
        }
        
        if (location.pathname == "/") {
            jQuery('ul.page-sidebar-menu .start').addClass('active');
        }
        /****Fix url group product***/
        if (location.pathname == "/GroupProduct") {
            activeHref = location.pathname + location.search;
            activeHref = activeHref.replace("GroupProduct", "Product").replace("?accordionType", "?type");
            jQuery('ul.page-sidebar-menu').find('a[href="' + activeHref + '"]').closest('li').addClass("active");
        }
        /****end***/
        var obj = jQuery('ul.page-sidebar-menu').find('a[href="' + activeHref + '"]');
        if (obj.length > 0) {
            obj.closest('ul.sub-menu').parent('li').addClass('active');
        } else {
            var type = $.urlParam('type');
            type = type != null && type != '' ? type.toLowerCase() : '';
            jQuery('ul.page-sidebar-menu').find('a[data-url="' + type + '"]').closest('ul.sub-menu').parent('li').addClass('active');
            if (type == "" && urlPath != "") {
                jQuery('ul.page-sidebar-menu').find('a[href="' + urlPath + '"]').closest('ul.sub-menu').parent('li').addClass('active');
            }
        }
    },
    ToolBar: function (idObj) {
        $(idObj + " > .k-grid-pager").prepend($(idObj).find(".k-grid-toolbar"));
    },
    Hierarchy: function (obj, fieldObj, idObj, status) {
        jQuery(obj).click(function () {
            var _this = this, fieldID;
            fieldID = parseInt(jQuery(_this).closest('tr.k-master-row').find(fieldObj).text());
            setTimeout(function () {
                STBASE.ToolBar(idObj + fieldID);
                if (status != undefined) STBASE.SortTable(idObj + fieldID);
            }, 100);
        });
    },
    HierarchyOrg: function (obj, fieldObj, idObj, status) {
        jQuery(obj).click(function () {
            var _this = this, fieldID;
            fieldID = parseInt(jQuery(_this).closest('tr.k-master-row').find(fieldObj).text());
            setTimeout(function () {
                var objID = idObj + fieldID;
                //STBASE.ToolBar(idObj + fieldID);
                //console.log($(objID+" > .k-grid-toolbar"));
                $(objID + " > .k-grid-pager").prepend($(objID+" > .k-grid-toolbar"));
                if (status != undefined) STBASE.SortTable(idObj + fieldID);
            }, 100);
        });
    },

    HierarchyWrapper: function (obj, fieldObj, idObj, onChangeSort, status) {
        jQuery(obj).click(function () {
            var _this = this, fieldID;
            fieldID = parseInt(jQuery(_this).closest('tr.k-master-row').find(fieldObj).text());
            setTimeout(function () {
                STBASE.ToolBar(idObj + fieldID);
                if (status != undefined) STBASE.SortTable(idObj + fieldID, onChangeSort);
            }, 100);
        });
    },
    SortTable: function (obj, onChangeSort) {
        if (onChangeSort == undefined) {
            onChangeSort = MLOG.onChangeSort;
        }
        jQuery(obj+" > table > tbody").kendoSortable({
            hint: function (element) {
                return element.clone().addClass("hint");
            },
            placeholder: function (element) {
                return element.clone().addClass("placeholder").text("");
            },
            cursor: "move",
            cursorOffset: {
                top: -10,
                left: -230
            },
            start: STBASE.onStart,
            move: STBASE.onMove,
            end: STBASE.onEnd,
            change: onChangeSort,
            cancel: STBASE.onCancel
        });
    },
    onStart:function(e) {
        var id = e.sender.element.attr("id");
    },

    onMove:function (e) {
        var id = e.sender.element.attr("id"),
            placeholder = e.list.placeholder;
    },

    onEnd:function (e) {
        var id = e.sender.element.attr("id"),
            text = e.item.text(),
            oldIndex = e.oldIndex,
            newIndex = e.newIndex;
    },

    onChange:function (e) {
        var id = e.sender.element.attr("id"),
            text = e.item.text(),
            newIndex = e.newIndex,
            oldIndex = e.oldIndex;
    },

    onCancel:function(e) {
    },

    ExportExcelPDF: function () {
        jQuery('#exportExcel, #exportPdf').click(function () {
            var _this = this, url;
            url = jQuery(_this).attr('href');
            window.location.href = url;
        })
    },
    CheckSideBar: function () {
        jQuery('li.nav-sidebar-li').each(function () {
            var checkLi = jQuery(this).find(".sub-menu > li").length;
            if (checkLi == 0) {
                jQuery(this).remove();
            }
        });
    },
    
    ActionNoti: function () {
        jQuery("#notification").click(function () {
            jQuery(".DataNoti").toggle(100, function () {});
        });
        jQuery(document).click(function (event) {
            if (!jQuery(event.target).is("#notification, #notification *, .DataNoti, .DataNoti *")) {
                jQuery(".DataNoti").hide();
            }
        });
    },
    AlertPopup: function (options) {
        var defaultOptions = {
            isGrid: true,
            isHtml:false,
            id: 'popup',
            minWidth: 100,
            minHeight: 100,
            width: 600,
            modal: true,
            draggable: true,
            resizable: true,
            title: 'Messages',
            typeStatus: true,
            confirmText: '',
            customButton: '',
            textYes: 'OK',
            textNo: '',
            iconHtmlYes: '',
            iconHtmlNo: '',
            idActYes: 'act-acceptAlert-yes',
            idActNo: 'act-acceptAlert-no'
        };
        options = $.extend(defaultOptions, options);
        var popup = $('#' + options.id);
        if (!popup.length) {
            $('body').append('<div id="' + options.id + '"></div>');
            popup = $('#' + options.id);
        }
        var popupWindow = popup.data('kendoWindow');
        if (!popupWindow) {
            popup.kendoWindow({
                minWidth: options.minWidth,
                minHeight: options.minHeight,
                width: options.width,
                title: options.title,
                modal: options.modal,
                resizable: options.resizable,
                draggable: options.draggable,
                deactivate: options.deactivate,
                activate: options.activate
            });
            popupWindow = popup.data('kendoWindow');
        } else {
            popupWindow.title(options.title);
        }
        if (options.typeStatus) {
            var htmlHD = '';
            htmlHD += '<div class="alert-content-popHD">';
            if (options.isHtml)
                    htmlHD += options.confirmText;
                else
                    htmlHD += '<h5>' + options.confirmText + '</h5>';
            htmlHD += '</div>';
            htmlHD += '<div class="alert-footer-popHD">';
            if (options.customButton) {
                htmlHD += options.customButton
            }
            if (options.textYes)
                htmlHD += '<button class="btn btn-primary" id="' + options.idActYes + '">' + options.iconHtmlYes + options.textYes + '</button>';
            if (options.textNo)
                htmlHD += '<button class="btn btn-primary" id="' + options.idActNo + '">' + options.iconHtmlNo + options.textNo + '</button>';
            htmlHD += '</div>';
            popupWindow.content(htmlHD);
        }
        popupWindow.center().open();
        jQuery('#act-acceptAlert-yes, #act-acceptAlert-no').click(function () {
            popupWindow.close();
        });
        return popupWindow;
    },
    expandRow: function (e, grid, dataGrid) {
        var gridData = grid.data("kendoGrid");
        if (dataGrid != undefined) {
            var dataItem = gridData.dataItem(dataGrid);
        } else {
            var dataItem = gridData.dataItem(jQuery(e.currentTarget).closest("tr"));
        }
        if (dataItem != undefined) {
            var uid = dataItem.uid;
            for (var i = 0 in gridData._data) {
                if (gridData._data[i].uid == uid) {
                    STBASE.exp = i - 1;
                }
            }
        } else {
            STBASE.exp = gridData._data.length - 1;
        }
    },
    PanelHeading: function (objGrid) {
        jQuery('.STPanel-heading > h3').click(function () {
            var optData = "";
            if (objGrid == undefined) {
                optData = "";
                optData = jQuery(this).attr("data-content");
                optData = "#" + optData;
                
            } else {
                optData = objGrid;
            }
            jQuery(optData).slideToggle("fast", function () {
                // Animation complete.
            });
        })
    },
    ExtractURL: function (url,opts) {
        if (url.length <= 1) return;

        var delimiter, keyValueSeparator, startAfter, limit;
        var opts = opts || {},
			keyValuePairs = [],
			params = {};

        delimiter = opts.delimiter || STBASE.DEFAULTS.delimiter;
        keyValueSeparator = opts.keyValueSeparator || STBASE.DEFAULTS.keyValueSeparator;
        startAfter = opts.startAfter || STBASE.DEFAULTS.startAfter;
        limit = STBASE.FilterInt(opts.limit) ? opts.limit : undefined;
        var startAfterindex = url.indexOf(startAfter);
        var keyValueSeparatorFirstIndex = url.indexOf(keyValueSeparator);

        if (keyValueSeparatorFirstIndex < 0) return;

        // scope of finding params only applicable to str
        var str = startAfterindex < 0 ? url : url.substring(startAfterindex + 1);

        keyValuePairs = str.split(delimiter, limit);
        var kvPair;
        for (var i = 0, s = keyValuePairs.length; i < s; i++) {
            kvPair = keyValuePairs[i].split(keyValueSeparator, 2);
            // ignore any items after first value found, where key = kvPair[0], value = kvPair[1]
            var value = kvPair[1];
            params[kvPair[0]] = STBASE.FilterInt(value) ? STBASE.FilterInt(value) : value; // return int if value is parsable
        };
        return params;
    },
    CalHeightGrid: function (idGrid, addHeight) {
        if (addHeight == undefined) {
            addHeight = 112;
        }
        jQuery(idGrid).css("overflow", "hidden");
        var heightContent = jQuery('.k-grid-content').height();
        jQuery(idGrid).height(parseInt(heightContent) + parseInt(addHeight))
        jQuery(".page-sidebar-menu .sidebar-toggler").click(function () {
            var heightContent = jQuery('.k-grid-content').height();
            var heightMain = jQuery(idGrid).height();
            setTimeout(function () {
                jQuery('.k-grid-content').height(heightContent);
                jQuery(idGrid).height(heightMain);
            }, 'fast');
        });
    },
    FilterInt:function(value) {
        if(/^(\-|\+)?([0-9]+|Infinity)$/.test(value)){
            return Number(value);
        };
        return NaN;
    },
    isTextSelected: function (input) {
        var is_chrome = navigator.userAgent.indexOf('Chrome') > -1;
        if (is_chrome){
            var startPos = input.selectionStart;
            var endPos = input.selectionEnd;
            var doc = document.selection;

            if (doc && doc.createRange().text.length != 0) {
                return true;
            } else if (!doc && input.value.substring(startPos, endPos).length != 0) {
                return true;
            }
        }
        return false;
    },
    ShowControlRestName: function () {
        jQuery( ".controlRestShow" )
          .mouseout(function() {
              jQuery(this).closest("li").removeClass("open");
              jQuery(this).attr("aria-expanded", false);
          })
          .mouseover(function() {
              jQuery(this).closest("li").addClass("open");
              jQuery(this).attr("aria-expanded", true);
          });
    }
}

jQuery(document).ready(function () {
    STBASE.init();
    STBASE.ExportExcelPDF();
    STBASE.ShowControlRestName();

    jQuery(".page-sidebar-menu .sidebar-toggler").click(function () {
        var checkStatus = jQuery(this).closest("ul").hasClass('page-sidebar-menu-closed');
        localStorage.setItem('sidebarOpt', checkStatus);
    });


});
STBASE.ActiveNav();
STBASE.CheckSideBar();
var sidebarStatus = localStorage.getItem('sidebarOpt');
if (sidebarStatus == 'false') {
    jQuery('body').addClass('page-sidebar-closed');
    jQuery('ul.page-sidebar-menu').addClass('page-sidebar-menu-closed');
}



var isMobile = {
    Android: function () {
        return navigator.userAgent.match(/Android/i);
    },
    BlackBerry: function () {
        return navigator.userAgent.match(/BlackBerry/i);
    },
    iOS: function () {
        return navigator.userAgent.match(/iPhone|iPad|iPod/i);
    },
    Opera: function () {
        return navigator.userAgent.match(/Opera Mini/i);
    },
    Windows: function () {
        return navigator.userAgent.match(/IEMobile/i);
    },
    any: function () {
        return (isMobile.Android() || isMobile.BlackBerry() || isMobile.iOS() || isMobile.Opera() || isMobile.Windows());
    }
};