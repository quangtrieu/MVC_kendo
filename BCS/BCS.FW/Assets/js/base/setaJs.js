function InitDimensionMasterPage() {
    var heightBody = $("body").height();
    $(".content-detail").height(heightBody - $('.record-header').outerHeight(true) - $('#header').outerHeight(true));
    //$(".container-fluid.content-detail").niceScroll().resize();
    
    $('.create-update').height(heightBody - $('#header').outerHeight(true) - $('.create-update-panel').outerHeight(true));
    $('#scheduleCrm').height(heightBody - $('#header').outerHeight(true));
    //$('#add-profile').height(heightBody - $('.create-update-panel').outerHeight(true) - $('#header').outerHeight(true) - );
    $("#center-box").height(heightBody - $('.create-update-panel').outerHeight(true) - $('#header').outerHeight(true));
    $('#field-settings').height(heightBody - 80);
    

    $("#nav .nav-action").height(heightBody - $("#nav .logo").outerHeight() - $("#nav .user").outerHeight());
    //var widthContent = $('body').outerWidth() - $('#nav').outerWidth();
    //$(".wp-main > .content").css('width', widthContent + 'px');
}

// Common seta Js
$.extend(true, setaJs, {
    init: function () {
        var t = this;
        InitDimensionMasterPage();
        
        //init tooltip
        $(document).ready(function () {
            /*$(document.body).tooltip();*/
            /*$('[data-toggle="tooltip"]').tooltip();*/
        });

        //$("#btnChooseCompany").click(function (e) {
        //    e.preventDefault();
        //    $(this).parent().parent().children("ul").children('li').fadeToggle();
        //});
        $(window).resize(function (e) {
            InitDimensionMasterPage();
            var grid = $('.k-grid');
            if (!grid.hasClass('fixed')) {
                var gridContent = $(".k-grid:not(.fixed) .k-grid-content");
                var gridHeight = 0;
                if (grid.attr('gridMargin') > 0) {
                    gridHeight = $("body").height() - $('#header').outerHeight(true) - grid.attr('gridMargin');
                } else {
                    gridHeight = $("body").height() - (gridContent.siblings('.k-grid-toolbar').outerHeight(true) + gridContent.siblings('.k-grid-header').outerHeight(true) + 178);
                }
                gridContent.height(gridHeight);
            }
            //$.scroll.resize();
        });
        //init height for list grid


        $("#main").on("click", function (e) {
            if ($(e.target).hasClass("k-grid") || $(e.target).parents(".k-grid").size()) {
                var kgrid = $(e.target).parents(".k-grid").first();
                var kendoGrid = $(kgrid).data("kendoGrid");

                var itemClick = $(e.target).closest("tr");
                if (itemClick.hasClass("k-state-selected")) {
                    if (itemClick.hasClass("crm-selected")) {
                        $(kgrid).find("tr.crm-selected").removeClass("crm-selected");
                        kendoGrid.clearSelection();
                    } else {
                        $(kgrid).find("tr.crm-selected").removeClass("crm-selected");
                        itemClick.addClass("crm-selected");
                    }
                } else {
                    kendoGrid.clearSelection();
                    $(kgrid).find("tr.crm-selected").removeClass("crm-selected");
                }
            } else {
                $("#main").find(".k-grid").each(function (k, item) {
                    var kendoGrid = $(item).data("kendoGrid");
                    kendoGrid.clearSelection();
                    $(item).find("tr.crm-selected").removeClass("crm-selected");
                });
            }
        });

        $(".a-link-action").click(function (e) {
            e.preventDefault();
            var link = $(this).attr("url");
            if (!link.match(/^http([s]?):\/\/.*/)) {
                link = 'http://' + link;
            }
            window.open(link, '_blank');
        });

        //Handler for document
        $(document)
            //Modal close alert
            .on("click", "div.noty_modal", function () {
                $.noty.closeAll();
            })
            //Modal Ajax for body - http://api.jquery.com/category/ajax/global-ajax-event-handlers/
            .on({
                // When ajaxStart is fired, add 'loading' to body class
                ajaxStart: function (event) {
                    $("#loading-ajax").removeClass("hide");
                    //hide a div after 10 seconds
                    setTimeout(function () {
                        $("#loading-ajax").addClass("hide");
                    }, 1000);
                },
                // When ajaxStop is fired, rmeove 'loading' from body class
                ajaxStop: function () {
                    $("#loading-ajax").addClass("hide");
                    //$.scroll.resize();
                },
                ajaxError: function (event, request, settings) {
                    $("#loading-ajax").addClass("hide");
                    //your error handling code here
                },
                ajaxSend: function (event, request, settings) {
                    var urls = [];
                    urls.push("/State/WindowState");
                    urls.push("/State/GridState");
                    urls.push("/Task/GetNotification");
                    urls.push("/Task/UpdateStatusReminder");
                    urls.push("/Calendar/Tooltip");
                    urls.push("/Contact/GetMoreEmail");
                    urls.push("/Contact/GetMorePhone");
                    urls.push("/Account/GetMoreActive");
                    urls.push("/Account/GetMorePhone");
                    $.each(urls, function (k, url) {
                        if (settings.url.substr(0, url.length) == url) {
                            $("#loading-ajax").addClass("hide");
                            return false;
                        }
                    });

                },
                ajaxComplete: function (event, request, settings) {

                }
            }).on("click", ".comingsoon", function (e) {
                e.preventDefault();
                t.msgWarning({ text: "Coming soon" });
            });

        $.ajaxSetup({
            statusCode: {
                401: function () {
                    t.msgWarning({
                        text: t.l('Common_Sessiontimeout'),
                        callback: {
                            onClose: function () {
                                //<div class="url-common" data-url-login="@Url.Action("Login", "Common")"></div>
                                window.location.href = $('.url-common').attr('data-url-login');
                            }
                        }
                    });
                }
            },
            // Disable caching of AJAX responses
            cache: false
        });

        //Menu main
        $('#nav-main a:not([class^="crm-toggle"])').on('click', function () {
            $('#nav-main a').each(function () {
                $(this).removeClass('selected');
            });
            $(this).addClass('selected');
        });

        //Window Collapse state
        $(".crm-collapse").on("click", function (e) {
            e.preventDefault();
            window.setaJs.WindowCollapseState.toggle.apply(this, []);
        });
        //fluent object
        
        // BichTV: Dropdown menu action
        $(function () {
            $(".menu-action-option").click(function (e) {
                e.stopPropagation();
            });
        });
        
        return this;
    },
    initController: function (param) {
        var parts = param.split(".");
        for (var i = 0, len = parts.length, obj = window; i < len; ++i) {
            obj = obj[parts[i]];
        }
        if (typeof (obj) == 'object' && obj.hasOwnProperty('init')) {
            obj.init();
        }
    },
    _openWindow: function (template, viewModel) {
        // Create a placeholder element.
        var window = $(document.createElement('div'));

        // Apply template to the placeholder element, and bind the viewmodel.
        var templateHtml = $(template).html();
        window.html(kendo.template(templateHtml)(viewModel));
        kendo.bind(window, viewModel);

        // Add window placeholder to the body.
        $('body').append(window);

        // Turn placeholder into a Window widget.
        window.kendoWindow({
            width: 600,
            title: "",
            resizable: false,
            modal: true,
            draggable: true,
            close: function () {
                // When the window is closed, remove the element from the document.
                window.parents(".k-window").remove();
            }
        });

        // Center and show the Window.
        window.data("kendoWindow").center();
        window.data("kendoWindow").open();
    },
    initPopupWindow: function(options) {
        var defaultOptions = {
            isGrid: true,
            id: 'popup',
            minWidth: 400,
            minHeight: 100,
            modal: true,
            draggable: true,
            resizable: true,
            title: 'Popup title',
            typeStatus: false,
            confirmText: '',
            contentHtml:'',
            textYes: '',
            textNo:''
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
                deactivate: options.deactivate
            });
            popupWindow = popup.data('kendoWindow');
        } else {
            popupWindow.title(options.title);
        }
        if (options.typeStatus) {
            var htmlHD = '';
            htmlHD += '<div class="confirm-content-popHD">';
            htmlHD += '<h3>' + options.confirmText + '</h3>';
            if (options.contentHtml != '') {
                htmlHD += options.contentHtml;
            }
            htmlHD += '</div>';
            htmlHD += '<div class="confirm-footer-popHD">';
            if (jQuery.trim(options.textNo).length > 0)
                htmlHD += '<button class="btn btn-primary" id="act-accept-no"><i class="fa fa-remove"></i>' + options.textNo + '</button>';
            if (jQuery.trim(options.textYes).length > 0)
                htmlHD += '<button class="btn btn-primary" id="act-accept-yes"><i class="fa fa-save"></i>' + options.textYes + '</button>';
            htmlHD += '</div>';
            popupWindow.content(htmlHD);
        }
        
        return popupWindow;
    },
    initAlertWindow: function (options) {
    var defaultOptions = {
        isGrid: true,
        id: 'popup',
        minWidth: 100,
        minHeight: 100,
        width: 600,
        modal: true,
        draggable: true,
        resizable: true,
        title: 'Popup title',
        typeStatus: false,
        confirmText: '',
        textYes: '',
        textNo:''
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
        htmlHD += '<div class="confirm-content-popHD">';
        htmlHD += '<h3>'+options.confirmText+'</h3>';
        htmlHD += '</div>';
        htmlHD += '<div class="confirm-footer-popHD">';
        if (options.textNo != "")
            htmlHD += '<button class="btn btn-primary" id="alt-accept-no">' + options.textNo + '</button>';
        htmlHD += '<button class="btn btn-primary" id="act-accept-yes">' + options.textYes + '</button>';
        htmlHD += '</div>';
        popupWindow.content(htmlHD);
    }
        
    return popupWindow;
    },
    actionFormWindow: function (formId, popupWindow, grid, options, data) {
        var oGrid = grid.data("kendoGrid");
        var $form = $(formId);
        //handler button
        $form.find("#act-accept-no").on("click", function () {
            if (popupWindow) popupWindow.close();
        });
        $form.find("#act-accept-yes").on("click", function () {
            if (data == undefined) {
                data = {};
            } else {
                var dateChange = jQuery("#datepickerEndDateEmployee").val();
                var dateEndPos = jQuery(".enddatePosition #datepickerEndDate").val();
                if (dateChange != undefined) {
                    data.EndDateEmp = dateChange;
                }
                if (dateEndPos != undefined) {
                    data.EndDate = dateEndPos;
                }
            }
            
            $.ajax(
             {
                 url: options.url,
                 type: "POST",
                 dataType: "json",
                 data:data,
                 async: false,
                 success: function (response) {
                     popupWindow.close();
                     $(options.idAction).find('tbody tr').remove();
                     $(options.idAction).find('.fa.fa-edit.btn-address-user').addClass('fa-plus').removeClass('fa-edit');
                     if (grid) {
                         var total = oGrid.dataSource.total();
                         var page = oGrid.dataSource.page();
                         var pageSize = oGrid.dataSource.pageSize();
                         if (total / pageSize > 1 && total % pageSize == 1) {
                             oGrid.dataSource.page(page - 1);
                         }
                         if (typeof grid.reload == "undefined" || (typeof gridui.reload != "undefined" && grid.reload == true)) {
                             oGrid.dataSource.read();
                             oGrid.refresh();
                         }
                     }
                 }
             });
        });
    },

    initFormDialog: function (formId, popupWindow) {
        var $form = $(formId);
        //handler button
        $form.find(".k-grid-cancel").on("click", function () {
            if (popupWindow) popupWindow.close();
        });
        $form.find(".k-grid-update").on("click", function () {

            $('.loading-process').remove();
            $(formId + ' .modal-footer').prepend('<a href="#" class="loading-process"></a>');
            if (!$form.valid()) {
                $('.loading-process').remove();
            }
            $form.submit();
        });
        //focus first text box
        setaJs.focusFirstInputForm($form);
    },

    initFormWindow: function (formId, popupWindow, grid, callback, errorCallback) {
        var empCheck = 0;
        var $form = $(formId);
        //handler button
        $form.find(".k-grid-cancel").on("click", function () {
            if (popupWindow) popupWindow.close();
        });
        
        var event = "";
        var submit = function (e) {
            $('.loading-process').remove();
            $(formId + ' .modal-footer').prepend('<a href="#" class="loading-process"></a>');
            if (!$form.valid()) {
                $('.loading-process').remove();
            }
            else {
                event = e;
                $form.find(".k-grid-update").unbind("click");
                $form.submit();
            }
        }
        $form.find(".k-grid-update").on("click", function (e) {
            empCheck = 1;
            submit(e)
        });
        //handler post form
        $.validator.unobtrusive.parse($form);
        var validator = $form.data("validator");
        validator.settings.ignore = ':hidden, [readonly=readonly]';
        validator.settings.submitHandler = function (form) {
            var postData = $(form).serialize();
            var formURL = $(form).attr("action");
            $.ajax(
            {
                url: formURL,
                type: "POST",
                async: false,
                data: postData,
                success: function (result, textStatus, jqXHR) {
                    $('.loading-process').remove();
                    if (result.Success == true) {
                        if (popupWindow) popupWindow.close();
                        if (callback && typeof (callback) == 'function') {
                            callback(result);
                        } else {
                            if (grid) {
                                var gridData = grid.data("kendoGrid");
                                //alert(gridData);
                                if (gridData && formId == '#frmAddOrEditEmployees' && Employees.Status != undefined && Employees.Status != 1) {
                                    gridData.dataSource.read({ "staticType": 1 });
                                } else if ((gridData && formId != '#frmAddOrEditEmployees') || (gridData && formId == '#frmAddOrEditEmployees' && Employees.Status != undefined && Employees.Status == 1)) {
                                    gridData.dataSource.read();
                                }
                            }
                        }
                    } else {
                        $form.find(".k-grid-update").on("click", function (e) {
                            submit(e);
                        });
                        $('.loading-process').remove();
                        validator.showErrors(result.Errors);
                        jQuery('.form-group .k-widget').each(function (key, item) {
                            if (typeof jQuery(this).find('span > label.error').html() != 'undefined') {
                                jQuery(jQuery(this).find('span > label.error')).appendTo(jQuery(this));
                            }
                        });
                        if (result.TypeShowErr != typeof undefined && parseInt(result.TypeShowErr) == 1) {
                            jQuery('input#' + result.FieldID).addClass('input-validation-error').removeClass("valid");
                            jQuery('#' + $form.attr('id') + ' span[data-valmsg-for="' + result.FieldID + '"]').addClass('field-validation-error').removeClass('field-validation-valid').html('<span for="' + result.FieldID + '" class="">' + result.Message + '</span>');
                        }
                        if (result.TypeShowErr != typeof undefined && parseInt(result.TypeShowErr) == 2) {
                            jQuery('.show-error-date-esixt').text(result.Message);
                        }
                        if (result.TypeShowErr != typeof undefined && parseInt(result.TypeShowErr) == 3) {
                            var htmlError = "";
                            htmlError += "<ul class='list-show-error-block'>";
                            jQuery(result.Data).each(function (key, item) {
                                if (key == 0)
                                    htmlError += '<li>The "' + kendo.toString(kendo.parseDate(item.BlockRequestDate), "MM/dd/yyyy") + '" is blocked so that you cannot make a leave request for that date*.</li>';
                            });
                            htmlError += "</ul>";
                            jQuery('.show-error-date-esixt').html(htmlError);
                        }
                        if (result.TypeShowErr != typeof undefined && parseInt(result.TypeShowErr) == 4) {
                            htmlError = "<ul class='list-show-error-block'>";
                            jQuery(result.Data).each(function (key, item) {
                                if (key == 0)
                                    htmlError +='<li>You can\'t create a request on "' + kendo.toString(kendo.parseDate(item), "MM/dd/yyyy") + '" anymore. The date is getting request limitation.</li>';
                            });
                            htmlError += "</ul>";
                            jQuery('.show-error-date-esixt').html(htmlError);
                        }

                        if (result.TypeShowErr != typeof undefined && parseInt(result.TypeShowErr) == 6) {
                            jQuery('input#' + result.FieldID).addClass('input-validation-error').removeClass("valid");
                            jQuery('#' + $form.attr('id') + ' span[data-valmsg-for="' + result.FieldID + '"]').addClass('field-validation-error').removeClass('field-validation-valid').html('<span for="' + result.FieldID + '" class="">' + result.Message + '</span>');
                        }

						// remove show poup msg error
                        //display message if available
                        //if (result.Message!= typeof undefined && result.Message != null && result.Message != '')
                        //    window.setaJs.msgError({ text: result.Message });
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    //if fails
                    $('.loading-process').remove();
                    $form.find(".k-grid-update").on("click", function (e) {
                        submit(e);
                    });
                    if (errorCallback != undefined) {
                        errorCallback(jqXHR, textStatus, errorThrown);
                    }
                }
            });
        };
        //focus first text box
        setaJs.focusFirstInputForm($form);
    },
    focusFirstInputForm : function($form) {
        setTimeout(function () {
            var txtInput = $form.find('input[type=text]:first');
            if (txtInput.length == 0) return;
            var strLength = txtInput.val().length + 1;
            txtInput.focus();
            txtInput[0].setSelectionRange(strLength, strLength);
        }, 500);
    },
    setaPopup: function (title, url, data, options, beforeLoad) {
        var defaultOptions = {
            //minWidth: "500px",
            //minHeight: "500px",
            modal: true,
            draggable: true,
            resizable: true,
            activate: null,
        };
        options = $.extend(defaultOptions, options);
        var positonleft = ($(window).width() - parseInt(options.width)) / 2;
        var positontop = ($(document).height() - $(window).height()) + ($(window).height() / 2 - 400);
        
        //var obj = this;
        var win = $("<div class='popup-content'>").kendoWindow({
            width: options.width,
            height: options.height,
            minWidth: options.minWidth,
            minHeight: options.minHeight,
            draggable: options.draggable,
            modal: options.modal,
            resizable: options.resizable,
            title: title,
            activate: options.activate,
            content: {
                url: url,
                cache: false,
                dataType: "html",
                type: "GET",
                data: data
            },
            //position: { top: positontop, left: positonleft },
            visible: false,
            close: function () {
                //$(win).fadeOut(200, function () {
                    $(win).data("kendoWindow").destroy();
                //});
            },
            refresh: beforeLoad
        });
        $(win).delegate(".btn-cancel", "click", function (e) {
            e.preventDefault();
            $(win).data("kendoWindow").close();
        });
        return win;
    },
    setaPopupClose: function (win) {
        $(win).data("kendoWindow").close();
    },
    setaGridDel: function(record, ui) {
        var oGrid = $(ui.grid).data("kendoGrid");
        var row = $(record).closest("tr");
        var dataItem = oGrid.dataItem(row);
        var id = dataItem[ui.recId];
        var confirmTitle = (ui.confirmTile == undefined || ui.confirmTile == null) ? "Delete Confirmation" : ui.confirmTile;
        var options = {
            id: ui.confirmDelId,
            url: ui.urlDel,
            typeStatus: true,
            title: confirmTitle,
            confirmText: ui.confirmDel,
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
            $.ajax(
             {
                 url: options.url,
                 type: "POST",
                 dataType: "json",
                 data: {
                     id: id
                 },
                 async: false,
                 success: function (response) {
                     popupWindow.close();
                     if (response.Status == 1) {
                         // check items of page when delete item of current page = 1 then load pre page
                         var total = oGrid.dataSource.total();
                         var page = oGrid.dataSource.page();
                         var pageSize = oGrid.dataSource.pageSize();
                         if (total / pageSize > 1 && total % pageSize == 1) {
                             oGrid.dataSource.page(page - 1);
                         }
                         if (typeof ui.reload == "undefined" || (typeof ui.reload != "undefined" && ui.reload == true)) {
                             oGrid.dataSource.read();
                             oGrid.refresh();
                         }

                     }
                 }
             });
        });
        //window.setaJs.msgConfirm({
        //    text: ui.confirmDel,
        //    titleHeader: confirmTitle,
        //    buttons: [
        //        {
        //            text: 'Yes',
        //            onClick: function ($noty) {
        //                $noty.close();
        //                $.ajax({
        //                    url: ui.urlDel,
        //                    type: "POST",
        //                    dataType: "json",
        //                    async: false,
        //                    data: {
        //                        id: id
        //                    },
        //                    success: function (response) {
        //                        if (response.Status == 1) {
        //                            // check items of page when delete item of current page = 1 then load pre page
        //                            var total = oGrid.dataSource.total();
        //                            var page = oGrid.dataSource.page();
        //                            var pageSize = oGrid.dataSource.pageSize();
        //                            if (total / pageSize > 1 && total % pageSize == 1) {
        //                                oGrid.dataSource.page(page - 1);
        //                            }
        //                            if (typeof ui.reload == "undefined" || (typeof ui.reload != "undefined" && ui.reload == true)) {
        //                                oGrid.dataSource.read();
        //                                oGrid.refresh();
        //                            }
                                   
        //                        } else {
        //                            window.setaJs.msgError(response.ErrorMsg);
        //                        }
        //                    },
        //                    error: function (xhr, ajaxOptions, thrownError) {
        //                        //console.log(ajaxOptions);
        //                        //console.log(thrownError);
        //                        //console.log(xhr);
        //                    }
        //                });
        //            }
        //        },
        //        {
        //        text: 'No',
        //        onClick: function ($noty) {
        //            $noty.close();
        //            }
        //        }
        //    ]
        //});
    },
    checkPrototype: function () {
        for (var prop in this) {
            log(prop);
        }
    },
    loadExtTemplate: function (path) {
        //http://docs.kendoui.com/howto/load-templates-external-files
        //Use jQuery Ajax to fetch the template file
        var tmplLoader = $.get(path)
            .success(function (result) {
                //On success, Add templates to DOM (assumes file only has template definitions)
                $("body").append(result);
            })
            .error(function (result) {
                alert("Error Loading Templates -- TODO: Better Error Handling");
            });

        tmplLoader.complete(function () {
            //Publish an event that indicates when a template is done loading
            $(host).trigger("TEMPLATE_LOADED", [path]);
        });
    },
    //--------------Message alert--------------------
    alert: function (options) {
        var titleHeader = "";
        if (typeof options.titleHeader != undefined) {
            titleHeader = options.titleHeader;
        }
        var defaultOptions = {
            layout: 'topRight',
            theme: 'defaultTheme',
            type: 'warning',
            text: '',
            dismissQueue: true,
            //template: '<div class="noty_message"><div class="noty-header">' + titleHeader + '</div><div class="noty_text"></div><div class="noty_close"></div></div>',
            animation: {
                open: { height: 'toggle' },
                close: { height: 'toggle' },
                easing: 'swing',
                speed: 500
            },
            timeout: 10000, //1s
            force: false,
            modal: false,
            maxVisible: 5, // max item display
            closeWith: ['click'], /// ['click', 'button', 'hover']
            //callback: {
            //    onShow: function () {
            //    },
            //    afterShow: function () {
            //        var that = this;
            //        $.each(that.options.buttons, function (i, v) {
            //            if (v.focus != undefined && v.focus == true) {
            //                $(that.$buttons).find("button")[i].focus();
            //                return false;
            //            }
            //        });
            //    },
            //    onClose: function () {
            //    },
            //    afterClose: function () {
            //    },
            //    onCloseClick: function () {
            //    }
            //},
            buttons: false
        };
        /* merge options into defaultOptions, recursively */
        $.extend(true, defaultOptions, options);


        if (defaultOptions.type == 'success') {
            defaultOptions.callback.onClose.call();
            this.log(defaultOptions.text);
        } else {
            if (defaultOptions.type == 'showsuccess') {
                defaultOptions.type = 'success';
            }

            return noty(defaultOptions);
        }
        //return noty(defaultOptions);
    },
    msgAlert: function (options) {
        var settings = $.extend({}, { type: 'alert' }, options);
        this.alert(settings);
    },
    msgSuccess: function (options) {
        var settings = $.extend({}, { type: 'success' }, options);
        this.alert(settings);
    },
    msgError: function (options, input) {
        var settings = {
            type: 'error',
            buttons: [{
                //addClass: 'btn btn-primary',
                addClass: 'btn btn-grey btn-crm btn-ok',
                text: 'OK',
                onClick: function ($noty) {
                    // this = button element
                    // $noty = $noty element
                    if (input != undefined) {
                        if(input.attr("type") == 'text')
                            input.focus();
                        else
                            input.siblings("input:visible").focus();
                        
                        // return false;
                    }
                    $noty.close();
                },
                focus: true
            }],
            modal: true,
            titleHeader: 'Error'
        };
        $.extend(true, settings, options);
        this.alert(settings);
    },
    msgWarning: function (options) {
        var settings = {
            type: 'warning',
            buttons: [{
                //addClass: 'btn btn-primary',
                addClass: 'btn btn-grey btn-crm btn-ok',
                text: 'OK',
                onClick: function ($noty) {
                    // this = button element
                    // $noty = $noty element
                    $noty.close();
                },
                focus: true
            }],
            modal: true,
            titleHeader: 'Warning'
        };
        $.extend(true, settings, options);
        this.alert(settings);
    },
    msgInfor: function (options) {
        var settings = $.extend({}, { type: 'information' }, options);
        this.alert(settings);
    },
    msgShowSuccess: function (options) {
        var settings = {
            type: 'showsuccess',
            titleHeader: 'Success'
        };
        $.extend(true, settings, options);
        this.alert(settings);
    },
    msgConfirm: function (options) {
        var settings = {
            type: 'confirm',
            buttons: [{
                //addClass: 'btn btn-primary',
                addClass: 'btn btn-primary btn-crm btn-ok',
                text: 'Yes',
                onClick: function ($noty) {
                    // this = button element
                    // $noty = $noty element
                    $noty.close();
                },
                focus: false

            }, {
                //addClass: 'btn btn-danger',
                addClass: 'btn btn-primary btn-crm btn-cancel',
                text: 'No',
                onClick: function ($noty) {
                    $noty.close();
                },
                focus: true
            }],
            modal: true,
            //confirmDel: 'Confirmation'
        };
        $.extend(true, settings, options);
        this.alert(settings);
    }, msgWarningWithAbort: function (options) {
        var settings = {
            type: 'confirm',
            buttons: [{
                addClass: 'btn btn-primary btn-cancel',
                text: 'Abort',
                onClick: function ($noty) {
                    $noty.close();
                }
            }],
            modal: true
        };
        $.extend(true, settings, options);
        this.alert(settings);
    },
    //--------------End Message alert--------------------
    log: function (text) {
        if (typeof window.console != 'undefined') {

        }
    },
    l: function (name) {
        try {
            var lang = "en";
            if (arguments.length > 1) {
                lang = arguments[1];
            }
            return (eval("setaJs.localize.{0}.{1}".format(lang, name)));
        } catch (err) {
            //alert(err.message);
            return "Miss key language: " + name;
        }
    },
    ajax: function (options) {
        var defaultOptions = {
            type: 'POST',
            contentType: 'application/json',
            dataType: 'json',
            //async: false,
            cache: false,
            success: function (result) {
            }
        };
        $.extend(true, defaultOptions, options);
        $.ajax(defaultOptions);
    },
    AddAntiForgeryToken: function (data, elForm) {
        //data.__RequestVerificationToken = $(elForm + '#__AjaxAntiForgeryForm input[name=__RequestVerificationToken]').val();
        if (typeof elForm !== 'undefined') {
            elForm += " ";
        }
        data.__RequestVerificationToken = $(elForm + 'input[name=__RequestVerificationToken]').val();
        return JSON.stringify(data);
    },
    formatPhoneNumber: function (phoneNumber) {
        var piece1 = phoneNumber.substring(0, 3); //123
        var piece2 = phoneNumber.substring(3, 6); //456
        var piece3 = phoneNumber.substring(6); //7890

        //should return (123)456-7890
        return kendo.format("({0})-{1}-{2}", piece1, piece2, piece3);
    }, ValidateDateTime: function (dateTime) {
        //only format mm/DD/YYYY H:MM PM
        var isValidDate = false;
        try {
            var arr1 = dateTime.split('/');
            var year = 0;
            var month = 0;
            var day = 0;
            var hour = 0;
            var minute = 0;
            var sec = 0;
            if (arr1.length == 3) {
                var arr2 = arr1[2].split(' ');
                if (arr2.length == 3) {
                    var arr3 = arr2[1].split(':');
                    year = parseInt(arr2[0], 10);
                    month = parseInt(arr1[0], 10);
                    day = parseInt(arr1[1], 10);
                    hour = parseInt(arr3[0], 10);
                    minute = parseInt(arr3[1], 10);
                    //sec = parseInt(arr3[0],10);
                    sec = 0;

                    var ampm = arr2[2];
                    if (isNaN(year) || isNaN(month) || isNaN(day) || isNaN(hour) || isNaN(minute) || !(ampm == 'AM' || ampm == 'PM'))
                        return false;

                    var isValidTime = false;
                    if (hour >= 0 && hour <= 12 && minute >= 0 && minute <= 59 && sec >= 0 && sec <= 59)
                        isValidTime = true;
                    else if (hour == 12 && minute == 0 && sec == 0)
                        isValidTime = true;

                    if (isValidTime) {
                        var isLeapYear = false;
                        if (year % 4 == 0)
                            isLeapYear = true;

                        if ((month == 4 || month == 6 || month == 9 || month == 11) && (day >= 0 && day <= 30))
                            isValidDate = true;
                        else if ((month != 2) && (day >= 0 && day <= 31))
                            isValidDate = true;

                        if (!isValidDate) {
                            if (isLeapYear) {
                                if (month == 2 && (day >= 0 && day <= 29))
                                    isValidDate = true;
                            } else {
                                if (month == 2 && (day >= 0 && day <= 28))
                                    isValidDate = true;
                            }
                        }
                    }
                    if (year <= 0 || (month <= 0 || month > 12) || day <= 0) {
                        isValidDate = false;
                    }
                }

            }
        } catch (err) {
            isValidDate = false;
        }
        return isValidDate;

    }, ValidateDate: function (date) {
        //only format mm/DD/YYYY
        var isValidDate = false;
        try {
            var arr1 = date.split('/');
            var year = 0;
            var month = 0;
            var day = 0;
            if (arr1.length == 3) {
                year = parseInt(arr1[2], 10);
                month = parseInt(arr1[0], 10);
                day = parseInt(arr1[1], 10);

                if (isNaN(year) || isNaN(month) || isNaN(day))
                    return false;

                var isLeapYear = false;
                if (year % 4 == 0)
                    isLeapYear = true;

                if ((month == 4 || month == 6 || month == 9 || month == 11) && (day >= 0 && day <= 30))
                    isValidDate = true;
                else if ((month != 2) && (day >= 0 && day <= 31))
                    isValidDate = true;

                if (!isValidDate) {
                    if (isLeapYear) {
                        if (month == 2 && (day >= 0 && day <= 29))
                            isValidDate = true;
                    } else {
                        if (month == 2 && (day >= 0 && day <= 28))
                            isValidDate = true;
                    }
                }

                if (year <= 0 || (month <= 0 || month > 12) || day <= 0) {
                    isValidDate = false;
                }

            }
        } catch (err) {
            isValidDate = false;
        }
        return isValidDate;
    }, ValidateTime: function (time) {
        //only format H:MM PM
        var isValidTime = false;
        try {
            var hour = 0;
            var minute = 0;
            var sec = 0;
            var arr1 = time.split(' ');
            if (arr1.length == 2) {
                var arr2 = arr1[0].split(':');
                hour = parseInt(arr2[0], 10);
                minute = parseInt(arr2[1], 10);
                //sec = parseInt(arr3[0],10);
                sec = 0;
                var ampm = arr1[1];
                if (isNaN(hour) || isNaN(minute) || !(ampm == 'AM' || ampm == 'PM'))
                    return false;

                if (hour >= 0 && hour <= 12 && minute >= 0 && minute <= 59 && sec >= 0 && sec <= 59)
                    isValidTime = true;
                else if (hour == 12 && minute == 0 && sec == 0)
                    isValidTime = true;

            }
        } catch (err) {
            isValidTime = false;
        }
        return isValidTime;
    },
    //validate date with format
    isDate: function (txtDate, formatDate) {
        if (formatDate == undefined) formatDate = 'mm/dd/yyyy';
        formatDate = formatDate.toLowerCase();
        var currVal = txtDate;
        if (currVal == '')
            return false;

        var rxDatePattern = /^(\d{1,2})(\/|-)(\d{1,2})(\/|-)(\d{4})$/; //Declare Regex
        var dtArray = currVal.match(rxDatePattern); // is format OK?

        if (dtArray == null)
            return false;

        //Checks for format.
        switch (formatDate) {
            case "mm/dd/yyyy":
                dtMonth = dtArray[1];
                dtDay = dtArray[3];
                dtYear = dtArray[5];
                break;
            case "dd/mm/yyyy":
                dtMonth = dtArray[3];
                dtDay = dtArray[1];
                dtYear = dtArray[5];
                break;
            case "yyyy/mm/dd":
                dtMonth = dtArray[3];
                dtDay = dtArray[5];
                dtYear = dtArray[1];
                break;
        }
        if (dtMonth < 1 || dtMonth > 12)
            return false;
        else if (dtDay < 1 || dtDay > 31)
            return false;
        else if ((dtMonth == 4 || dtMonth == 6 || dtMonth == 9 || dtMonth == 11) && dtDay == 31)
            return false;
        else if (dtMonth == 2) {
            var isleap = (dtYear % 4 == 0 && (dtYear % 100 != 0 || dtYear % 400 == 0));
            if (dtDay > 29 || (dtDay == 29 && !isleap))
                return false;
        }
        return true;
    },
    onOpenTimePicker: function (e) {
        var timePicker = e.sender;
        var ul = timePicker.timeView.ul;
        ul.children('li').removeClass('k-state-hover');
        var value = timePicker.value();
        if (value) {
            var minute = value.getMinutes();
            var hour = value.getHours();
            if (minute !== 0 && minute !== 30) {
                var interval = setInterval(function () {
                    if (ul) {
                        minute = minute < 30 ? 30 : 0;
                        hour = minute == 0 ? hour + 1 : hour;
                        var index = hour * 2 + (minute == 0 ? 0 : 1);
                        if (index >= 48) index = 0;
                        var indexLiRound = ul.find('li').eq(index);
                        indexLiRound.addClass('k-state-hover');
                        var scrollTo = index * indexLiRound.outerHeight(true) - 80;
                        ul[0].scrollTop = scrollTo;
                        clearInterval(interval);
                    }
                }, 100);
            }
        }
    },
    ValidateDecimal: function (value, key) {
        // using for validating inline edit Money valua
        var msg = "";
        if (!($.isNumeric(value))) {
            msg = window.setaJs.l("Validate_Invalid").format(window.setaJs.l(key));
            return msg;
        } else {
            var vla = parseFloat(value);
            if (vla == undefined || vla.toString() !== value.toString()) {
                return window.setaJs.l("Message_NumberLarger").format(window.setaJs.l(key));
            }
            if (vla < 0) {
                msg = window.setaJs.l("Validate_BiggerThan").format(window.setaJs.l(key), 0);
                return msg;
            }
        }
        return msg;
    },
    ValidateInteger: function (value, key) {
        // using for validating inline edit Integer valua
        var msg = "";
        if (!($.isNumeric(value))) {
            msg = window.setaJs.l("Validate_Invalid").format(window.setaJs.l(key));
            return msg;
        } else {
            var vla = parseInt(value);
            if (vla == undefined || vla.toString() !== value.toString()) {
                return window.setaJs.l("Message_NumberLarger").format(window.setaJs.l(key));
            }
            if (vla < 0) {
                msg = window.setaJs.l("Validate_BiggerThan").format(window.setaJs.l(key), 0);
                return msg;
            }
        }
        return msg;
    },
    ValidateEmail: function (email) {
        var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        return re.test(email);
    },
    FindTopmostElement: function (elm) {
        //Find top most kendo window opened: .k-window:visible
        var idx = 0;
        var that = $(elm);
        if (that.length <= 1)
            return that;
        that.each(function (k, v) {
            var value = $(v).css('zIndex');
            if ($.inArray(value, ['inherit', 'auto']) == -1 && parseInt(value) > idx)
                idx = k - 1;
        });
        return that.eq(idx);
    },
    HandleEnterKeyPress: function () {
        $("body").keydown(function (e) {
            if (e.which == 13) {
                //catch enter on focus kendo container on opened or textarea, a, button nothing to do
                if ($('.k-animation-container:visible').length > 0 || $("*:focus").is("textarea, button, a, .k-item")) return;

                var topWindow = window.setaJs.FindTopmostElement('.k-window:visible');
                if (topWindow.length == 1) {
                    var submitButton = topWindow.find('.form-submit-button:visible:enabled, .btn-save:visible:enabled, .btn-create:visible:enabled, .btn-edit:visible:enabled').first();
                    if (submitButton.length == 1 && submitButton.is(':focus') == false) {

                        submitButton.click();
                    }
                }

                //var submitButton = $(".form-submit-button:visible:enabled").last();
                //if (submitButton.length == 1) {
                //    if (submitButton.is(":focus") == false)
                //        submitButton.click();
                //} else {

                //    return;
                //    submitButton = $(".k-window .btn-save:visible:enabled, .k-window .btn-create:visible:enabled").last(); //.btn-crm-submit:enabled,
                //    if (submitButton.length == 1 && submitButton.is(":focus") == false) {
                //        submitButton.click();
                //    }
                //} //else nothing to do
            }
        });
    },
    SubStr: function (input, count) {
        try {
            var flg = true;
            var arr = input.split(' ');
            var strSub = "";
            $.each(arr, function () {
                if ((strSub.length + this.length) < count) {
                    strSub += this
                    strSub += " ";
                } else {
                    if (flg) {
                        strSub += "...";
                        flg = false;
                    }
                }
            });
        } catch (e) {
        }
        return strSub;
    },
    setValueDefaultObject: function (obj) {
        if (typeof obj == "object") {
            for (var i in obj) {
                switch (typeof obj[i]) {
                    case "object":
                        if (obj[i] == null) {
                            obj[i] = "";
                        }
                        break;
                    case "string":
                        if (obj[i].length == 0) {
                            obj[i] = "";
                        }
                        break;
                    case "number":
                        if (obj[i].length == 0) {
                            obj[i] = 0;
                        }
                        break;
                }
            }
        }
        return obj;
    },
    //Url
    getUrlVars: function () {
        //http://jquery-howto.blogspot.com/2009/09/get-url-parameters-values-with-jquery.html
        var vars = [], hash;
        var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
        for (var i = 0; i < hashes.length; i++) {
            hash = hashes[i].split('=');
            vars.push(hash[0]);
            vars[hash[0]] = hash[1];
        }
        return vars;
    },
    getUrlVar: function (name) {
        return this.getUrlVars()[name];
    },
    autoRedirect: function () {
        //using call if have  returnUrl window.setaJs.autoRedirect() or window.setaJs.autoRedirect("param_url")
        var url = window.setaJs.getUrlVar('returnUrl');
        if (arguments.length == 1) {
            url = arguments[0];
        }
        if (url != 'undefined') {
            window.location.href = url;
        }
    },
    goBack: function () {
        if (setaJs.getReturnUrl() != undefined) {
            window.location.href = unescape(setaJs.getReturnUrl());
        }
        else if (window.location.href.indexOf('returnList=True') != -1) {
            //window.history.go(-2); //Go back list
            window.location.href = setaJs.oGlobal.RootURL + setaJs.oGlobal.controller + '/Index';
        }
        else {
            if (window.history.length == 1)
                window.location.href = document.referrer;
            else 
                window.history.go(-1);
        }
        return false;
    },
    goForward: function () {
        window.history.go(+1);
        return false;
    },
    getReturnUrl: function () {
        return setaJs.getUrlVar('returnUrl');
    },
    //End Url
    generateFilterCharacter: function (elGrid, param) {
        var t = this;
        if (typeof (param) == 'object') {
            param.unshift('#');
            param.unshift('all');
            var $keyFilter = $('.left-filter-by-charactor');
            for (var i = 0; i < param.length; i++) {
                var elAppend = $("<li class='title'>{0}</li>".format((param[i]).toUpperCase()))
                    .on('click', function () {
                        t.filterCharacter(elGrid, $(this).text().toLowerCase());
                    });
                $keyFilter.append(elAppend);
            }
            $(elGrid).append($keyFilter);

        }
    },
    filterCharacter: function (elGrid, value) {
        var grid = $(elGrid).data("kendoGrid");
        var initialFilter = { logic: "and", filters: [] };
        if (value != '' && value != 'all') {
            if ($('#ModuleLayout').length > 0) {
                var moduleLayout = $('#ModuleLayout').data('kendoComboBox').value();
                var filterModule = {
                    logic: "and", filters: [
                        { field: "KeyFilter", operator: "eq", value: value },
                        { field: "ModuleID", operator: "eq", value: moduleLayout },
                        { field: "keyword", operator: "contains", value: $("#search-layout-input").val() }
                    ]
                };
                if (moduleLayout == 0) {
                    filterModule = {
                        logic: "and", filters: [
                             { field: "KeyFilter", operator: "eq", value: value },
                             { field: "keyword", operator: "contains", value: $("#search-layout-input").val() }
                        ]
                    };
                }
                initialFilter.filters.push(filterModule);
                grid.dataSource.filter(initialFilter);
            } else {
                grid.dataSource.filter({ field: "KeyFilter", operator: "eq", value: value });
            }
        } else {
            if ($('#ModuleLayout').length > 0) {
                if ($('#ModuleLayout').data('kendoComboBox') != undefined) {
                    var moduleLayout = $('#ModuleLayout').data('kendoComboBox').value();                  
                    var filterModule = {
                        logic: "and", filters: [
                              { field: "ModuleID", operator: "eq", value: moduleLayout },
                             { field: "keyword", operator: "contains", value: $("#search-layout-input").val() }
                        ]
                    };
                    if (moduleLayout == 0) {
                        filterModule = {
                            logic: "and", filters: [
                                 { field: "keyword", operator: "contains", value: $("#search-layout-input").val() }
                            ]
                        };
                    }
                    initialFilter.filters.push(filterModule);
                    grid.dataSource.filter(initialFilter);
                }
            } else {
                $("#searchFlag").val(0);
                $("#inputSearchGrid").val('');
                //$("#inputSearchGrid").animate({ "width": "0px" }, "slow");
                //setTimeout(function () {
                //    $("#inputSearchGrid").addClass("hidden");
                //}, 600);
                grid.dataSource.filter([]);
            }
        }
    },
    //function for quick search on all main module
    quickSearchListView: function ($btnSearch, $inputSearch, $searchFlag, $gridSearch) {
        var searchFlag = '0';
        $btnSearch.on("click", function () {
            searchFlag = $searchFlag.val();
            if (searchFlag == '0') {
                $inputSearch.removeClass("hidden");
                //$inputSearch.animate({ "width": "350px" }, "slow");
                setTimeout(function () {
                    $inputSearch.focus();
                }, 600);
                $searchFlag.val(1);
            }
            //else {
            if ($searchFlag.val() != '') {
                $gridSearch.data("kendoGrid").dataSource.page(1);
            }

            if ($inputSearch.val() == '') {
                $inputSearch.removeClass("hidden");
                $searchFlag.val(0);
            }
            //}
        });
        $inputSearch.keypress(function (e) {
            if (e.which == 13) {
                $gridSearch.data("kendoGrid").dataSource.page(1);
            }
        });
    },
    kendoComboboxHasData: function (cbb) {
        /// validate item not found in combobox
        var selectedIndex = cbb.data("kendoComboBox").select();
        if (selectedIndex >= 0) {
            return true;
        }
        return false;
    },
    kendoComboBoxOnChange: function () {
        // Using for kendoCombox sugesstion input
        // If data has not Input value, reset value of combobox
        var selectedIndex = this.select();
        if (selectedIndex < 0) {
            this.value(null);
            this.dataSource.filter([]);
        }
    },
    kendoComboBoxOnDataBound: function () {
        if (!this.firstload && this.select() < 0) {
            this.firstload = true;
            this.value(null);
        }
    },
    kendoComboBoxOnChangeInvalid: function (cb) {
        // Using for kendoCombox sugesstion input
        // If data has not Input value, reset value of combobox
        var selectedIndex = cb.select();
        if (selectedIndex < 0) {
            cb.value(null);
        }
    },
    kendoNumberBoxOnChange: function (e) {
        if (this.value() == null) {
            this.value(0);
        }
    },
    kendoNumberBoxOnChangeMaxValue: function (e) {
        var value = this.value();
        if (value == null) {
            this.value(0);
        }
        else {
            if (value < this.min() || value >= this.max()) { 
                this.value(this.oldvalue);
            }
        }
    },
    kendoDatetimePickerOnChange: function (e) {
        var date = Date.parse(this.value());
    },
    getDateFromString: function (str) { // format date : mm/dd/yyyy
        var parts = str.split('/');
        var date = new Date(parseInt(parts[2], 10), // year
            parseInt(parts[0], 10) - 1, // month, starts with 0
            parseInt(parts[1], 10));
        return date;
    },
    getDateTimeFromString: function (str) { // format date : mm/dd/yyyy hh:mm
        var dateRegex = /^([0]\d|[1][0-2])\/([0-2]\d|[3][0-1])\/([2][01]|[1][6-9])\d{2}(\s([0-1]\d|[2][0-3])(\:[0-5]\d){1,2})?$/;
        if (str.match(dateRegex) != null)
            return new Date(str);
        return null;

        //var dateSp = str.split(' ')[0];
        //var timeSp = str.split(' ')[1];
        //var parts = dateSp.split('/');
        //var times = timeSp.split(':');
        //var date = new Date(parseInt(parts[2], 10), // year
        //    parseInt(parts[0], 10) - 1, // month, starts with 0
        //    parseInt(parts[1], 10),
        //    parseInt(times[0], 10),
        //    parseInt(times[1], 10)
        //);
        //return date;
    },
    searchSpotLight: function (selector) {
        $(selector).kendoAutoComplete({
            minLength: 0,
            //height: 400,
            //delay: 200,
            placeholder: window.setaJs.l("Btn_Search") + "...",
            filter: "contains",
            dataTextField: "Name",
            dataSource: new kendo.data.DataSource({
                serverFiltering: true,
                transport: {
                    read: {
                        url: '/Search/BasicSearch_Get',
                        type: 'GET',
                        dataType: 'json'
                    },
                    parameterMap: function (e) {
                        return {
                            keyword: encodeURIComponent($(selector).val().replace(/^\s+|\s+$/g, "")),
                            module: '',
                            pageIndex: 1
                        };
                    }
                }
            }),
            template: kendo.template($("#BasicSearchItem").html()),
            open: function (e) {
                this.list.width(315);
            },
            select: function (e) {
                var selectlink = e.item.find("a").attr('href');
                if (selectlink != null && selectlink != 'undefined') {
                    window.setaJs.gotoDetail(selectlink);
                } else {
                    var pageIndex = e.item.find("a").attr('more-index');
                    var module = e.item.find("a").attr('module');
                    window.setaJs.searchSpotLightViewMore(selector, module, pageIndex);
                }
                var oldText = $(selector).val();
                if (oldText != null && oldText.length > 0) {
                    this.value($(selector).value());
                }
            },
            dataBound: function (e) {
                window.setaJs.searchSpotLightMore(selector, "Contact");
                window.setaJs.searchSpotLightMore(selector, "Account");
                window.setaJs.searchSpotLightMore(selector, "ServiceTicket");
                window.setaJs.searchSpotLightMore(selector, "Opportunity");
                window.setaJs.searchSpotLightMore(selector, "Campaign");
                window.setaJs.searchSpotLightMore(selector, "ServiceContract");
                $("#txt-search-query_listbox a").each(function (k, v) {
                    $(this).attr('href', v.href.replace('{keySearch}', encodeURIComponent($(selector).val().replace(/^\s+|\s+$/g, ""))));
                });
            },
            close: function (e) {
                return false;
            }
        });
    },
    searchSpotLightMore: function (selector, moduleName) {
        $(".more-result-" + moduleName).on('click', function (evg) {
            evg.preventDefault();
            var pageIndex = $(this).attr("more-index");
            window.setaJs.searchSpotLightViewMore(selector, moduleName, pageIndex);
        });
    },
    searchSpotLightViewMore: function (selector, moduleName, pageIndex) {
        var tmpPage = parseInt(pageIndex) + 1;
        var listItem = $("div[class^='row spotlight-row more-" + moduleName + "-" + pageIndex + "']").closest('li');
        var startIndex = $("#txt-search-query_listbox li").index(listItem);
        $.ajax({
            url: "/Search/BasicSearch_Get",
            type: "GET",
            cache: false,
            data: {
                keyword: encodeURIComponent($("#txt-search-query").val().replace(/^\s+|\s+$/g, "")),
                module: moduleName,
                pageIndex: pageIndex
            },
            dataType: "json",
            success: function (response) {
                if (response) {
                    var ds = $(selector).data("kendoAutoComplete").dataSource;
                    if (response != null && response.length > 0) {
                        var moreItem = ds.at(startIndex);
                        ds.remove(moreItem);
                        for (var i = 0; i < response.length; i++) {
                            ds.insert(startIndex + i, response[i]);
                        }
                    }
                    var mores = $("a.more-result-" + moduleName);
                    if (mores != null && mores.length > 0) {
                        $.each(mores, function (key, more) {
                            if ($(more).attr('more-index') != tmpPage)
                                $(more).closest("li").removeClass("hide");
                        });
                    }
                }
            },
            error: function () {
            }
        });
    },
    searchBasic: function (btnSelector, txtSelector) {
        $(btnSelector).on('click', function (evg) {
            evg.preventDefault();
            window.setaJs.searchBasicClick(txtSelector);
        });
        $(txtSelector).on('keydown', function (evg) {
            if (evg.which == 13) {
                window.setaJs.searchBasicClick(txtSelector);
            }
            //Up Down Key
            if (evg.which == 38 || evg.which == 40) {
                var kendoAuto = $(this).data("kendoAutoComplete");
                var wapper = kendoAuto.list.closest(".k-animation-container");
                if (!$(wapper).is(":visible")) {
                    kendoAuto.search($(this).val());
                    evg.preventDefault();
                }
            }
        });
    },
    searchAdvance: function (selector) {
        $(selector).on('click', function (evg) {
            var returnUrl = escape(location.pathname + location.search);
            evg.preventDefault();
            var module = window.setaJs.oGlobal.CurrentModule;
            // delete cookies
            var currentSearchId = window.setaJs.getCRMCookie("currentSearchId");
            if (currentSearchId != "") {
                window.setaJs.deleteCRMCookie("currentSearchId");
            }
            var cSelected = window.setaJs.getCRMCookie("currentSelected");
            if (cSelected != "") {
                window.setaJs.deleteCRMCookie("currentSelected");
            }

            window.setaJs.msgWarning({ text: "Under Construction!", modal: true });
            return true;

            if (module != null) {
                $(window).attr("location", "/Search/AdvancedSearch?module=" + module + "&returnUrl=" + returnUrl);
            } else {
                $(window).attr("location", "/Search/AdvancedSearch?module=0" + "&returnUrl=" + returnUrl);
                //window.setaJs.msgWarning({ text: window.setaJs.l("AdvanceSearch_NoModule"), modal: true });
            }
        });
    },
    searchBasicClick: function (txtSelector) {
        var searchText = $(txtSelector).val().replace(/^\s+|\s+$/g, "");
        if (searchText != null && searchText.length > 0) {
            $(window).attr("location", "/Search/SearchResult?keyword=" + encodeURIComponent(searchText));
        }
    },
    gotoDetail: function (link) {
        if (link != null && link.length > 0) {
            $(window).attr("location", link);
        }
    },
    searchGridDataBound: function (e) {
        var total = 0;
        total = e.sender.dataSource.total();
        showGrid.apply(this, [e]);
        //if (total <= 0) {
        //    $(e.sender.wrapper).addClass("hide");
        //} else {
        //    $(e.sender.wrapper).removeClass("hide");
        //}
        $(e.sender.wrapper).prev().find("span").first().html("(" + total + ")");

        // using for advance search
        if (e.sender._data.length > 0) {
            $(e.sender.table).closest('div.search-result-grid').addClass('hasdata').removeClass('nodata').show();
        } else {
            $(e.sender.table).closest('div.search-result-grid').addClass('nodata').removeClass('hasdata').hide();
            var divContain = $("#advancedsearchresult").first();
            if (divContain != undefined) {
                var totalGrid = $("#advancedsearchresult").attr("totalgrid");
                var resultGrid = $('div.search-result-grid.nodata');
                if (totalGrid != undefined && resultGrid != undefined && resultGrid.length == totalGrid) {
                    $("div.query-box-search-result").first().append("<span class='span-search-no-result'>" + window.setaJs.l("Data_NotFound") + "</span>");
                }
            }
        }
    },
    getCRMCookie: function (cname) {
        var name = cname + "=";
        var ca = document.cookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i].trim();
            if (c.indexOf(name) == 0) return c.substring(name.length, c.length);
        }
        return "";
    },
    setCRMCookie: function (cname, cvalue, exdays) {
        var d = new Date();
        d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
        var expires = "expires=" + d.toGMTString();
        document.cookie = cname + "=" + cvalue + "; " + expires + "; path=/";
    },
    deleteCRMCookie: function (cname) {
        var currentSearchId = window.setaJs.getCRMCookie(cname);
        if (currentSearchId != "") {
            document.cookie = cname + "=; expires=Thu, 01 Jan 1970 00:00:00 GMT; path=/";
        }
    },
    convertToRealNumberFormat: function (number, format) {
        var twoDigit = number.substring(number.indexOf('.') + 1, number.length);
        var intNumber = Number(number.substring(0, number.indexOf('.')));
        var intNumberFormat = kendoToStringFormat(intNumber, format);
        var result = intNumberFormat.substring(0, intNumberFormat.indexOf('.') + 1) + twoDigit;
        return result;
    },
    validateExceptSpecialCharacter: function(text) {
        //return /[\<\>\!\@\#\$\%\^\&\*\(\)\'\"\;\`]/.test(text);
        return /[\<\>\!\@\#\$\%\^\&\(\)\'\"\;\`]/.test(text);
    },
    find_duplicates: function(arr) {
        var len = arr.length,
        out = [],
        counts = {};

        for (var i = 0; i < len; i++) {
            var item = arr[i];
            counts[item] = counts[item] >= 1 ? counts[item] + 1 : 1;
        }

        for (var item in counts) {
            if (counts[item] > 1)
                out.push(item);
        }

        return out;
    },
    /*****************Hash URL*****************/
    HashURL: {
        historyUrl: null,
        scriptDictionary: [],
        //https://github.com/browserstate/history.js/
        //http://stackoverflow.com/questions/13553037/jquery-history-js-example
        initHashURL: function (params) {
            var that = this;
            // Prepare
            //var History = window.History; // Note: We are using a capital H instead of a lower h
            window.setaJs.HashURL.historyUrl = window.History; // Note: We are using a capital H instead of a lower h
            //if (!History.enabled) {
            if (!window.setaJs.HashURL.historyUrl.enabled) {
                // History.js is disabled for this browser.
                // This is because we can optionally choose to support HTML4 browsers or not.
                return false;
            }

            // Bind to StateChange Event
            //History.Adapter.bind(window, 'statechange', function () { // Note: We are using statechange instead of popstate
            window.setaJs.HashURL.historyUrl.Adapter.bind(window, 'statechange', function () { // Note: We are using statechange instead of popstate
                //var State = History.getState();
                var state = that.historyUrl.getState();
                //$('#content').load(State.url);
                window.setaJs.log("link :" + state.url);
                $.ajax({
                    url: state.url,
                    data: { hashtable: true },
                    success: function (response) {
                        $('#main-content-site').html(response);
                        window.setaJs.log("finish content load ajax");
                        //window.setaJs.log(window.setaJs.HashURL.queueScript);
                        var totalJsFile = window.setaJs.HashURL.queueScript.length;
                        if (totalJsFile > 0) {
                            //http://blog.sebarmeli.com/2010/12/06/best-way-to-loop-through-an-array-in-javascript/
                            for (var i = 0; i < totalJsFile; i++) {
                                var callback = window.setaJs.HashURL.queueScript[i];
                                callback.apply();

                                //Remove callback this when run finish
                                window.setaJs.HashURL.queueScript.splice(0, 1);
                            }
                            //window.setaJs.HashURL.queueScript.forEach(function (callback) {
                            //    //http://stackoverflow.com/questions/1986896/what-is-the-difference-between-call-and-apply
                            //    //window.setaJs.log(callback);
                            //    callback.apply();

                            //    //Remove callback this when run finish
                            //    window.setaJs.HashURL.queueScript.splice(0, 1);
                            //});
                        }
                        //window.setaJs.log(window.setaJs.HashURL.queueScript);

                        //init again new element
                        //window.setaJs.log("reinit again new element when finish load data ajax");
                    }
                });
            });
            window.setaJs.HashURL.pushStateHashURL();
        },
        pushStateHashURL: function () {
            if (window.setaJs.hashUrlIsEnable()) {
                window.setaJs.log("init or reinit pushState Hash Url");
                // Capture all the links to push their url to the history stack and trigger the StateChange Event
                $("a:not([href^='http'])").click(function (evt) {
                    evt.preventDefault();
                    window.setaJs.HashURL.historyUrl.pushState(null, $(this).text(), $(this).attr('href'));
                });
            }
        },
        pushScript: function (srcJs, callbackInit) {
            if (window.setaJs.hashUrlIsEnable()) {
                var urlJs = window.setaJs.oGlobal.RootURL + srcJs;
                if ($.inArray(urlJs, window.setaJs.HashURL.scriptDictionary) == -1) { // not found
                    window.setaJs.log("pushScript : " + urlJs);
                    window.setaJs.HashURL.scriptDictionary.push(urlJs);
                }
                if (typeof callbackInit != "undefined") {
                    window.setaJs.log("Init controller : " + urlJs);
                    window.setaJs.JsScript.loadResource(urlJs, callbackInit);
                }
            }
        },
        queueScript: []
    },
    runHashURL: function () {
        if (this.hashUrlIsEnable())
            this.HashURL.initHashURL();
    },
    hashUrlIsEnable: function () {
        if (window.setaJs.oGlobal.UseHashURL.toLowerCase() == "true") {
            return true;
        }
        return false;
    },
    //Js
    JsScript: {
        loadResource: function (pathJs, callbackInit) {
            //http://unixpapa.com/js/dyna.html

            var head = document.getElementsByTagName('head')[0];
            var script = document.createElement('script');
            script.type = 'text/javascript';
            if (typeof callbackInit != "undefined") {
                script.onreadystatechange = function () {
                    //if (this.readyState == 'complete') helper();
                    if (this.readyState == 'complete') {
                        window.setaJs.initController(callbackInit);
                    }
                };
            }

            //script.onload = helper;
            //script.src = 'helper.js';
            script.src = pathJs;
            head.appendChild(script);
        }
    },
    //File
    FileSystem: {
        getExtension: function (file) {
            var extension = file.substr((file.lastIndexOf('.') + 1));
            return extension;
        }
    },
    //View more Popover
    ViewMorePopover: function (element, title, content, placement, container) {
       
        if (placement == undefined) placement = function (tip, source) {
            var $element, above, actualHeight, actualWidth, below, boundBottom, boundLeft, boundRight, boundTop, elementAbove, elementBelow, elementLeft, elementRight, isWithinBounds, left, pos, right;
            isWithinBounds = function (elementPosition) {
                return boundTop < elementPosition.top && boundLeft < elementPosition.left && boundRight > (elementPosition.left + actualWidth) && boundBottom > (elementPosition.top + actualHeight);
            };
            $element = $(source);
            pos = $.extend({}, $element.offset(), {
                width: element.offsetWidth,
                height: element.offsetHeight
            });
            actualWidth = 280;
            actualHeight = 180;
            boundTop = $(document).scrollTop();
            boundLeft = $(document).scrollLeft();
            boundRight = boundLeft + $(window).width();
            boundBottom = boundTop + $(window).height();
            elementAbove = {
                top: pos.top - actualHeight,
                left: pos.left + pos.width / 2 - actualWidth / 2
            };
            elementBelow = {
                top: pos.top + pos.height,
                left: pos.left + pos.width / 2 - actualWidth / 2
            };
            elementLeft = {
                top: pos.top + pos.height / 2 - actualHeight / 2,
                left: pos.left - actualWidth
            };
            elementRight = {
                top: pos.top + pos.height / 2 - actualHeight / 2,
                left: pos.left + pos.width
            };
            above = isWithinBounds(elementAbove);
            below = isWithinBounds(elementBelow);
            left = isWithinBounds(elementLeft);
            right = isWithinBounds(elementRight);
            if (right) return 'right';
            else if (left) return 'left';
            else if (above) return 'top';
            else if (below) return 'bottom';
            else return 'left';
        };
        if (container == undefined) container = 'body';
        var popup = $(element);
        popup.popover({
            html: true,
            trigger: 'manual',
            placement: placement,
            title: title,
            container: container,
            content: content
        }).on("click", function () { //change trigger from mouseenter to click -- by CuongTD            
            var _this = this;
            //$('.popover-custom').parent('.popover-content').css({ "max-height": "140px" });
            $(this).popover("show");
            $('.popover-custom').find("ul").addClass("list-unstyled");
            $(".popover").on("mouseleave", function () {
                $(_this).popover('hide');
            });
        }).on("mouseleave", function () {
            var _this = this;
            setTimeout(function () {
                if (!$(".popover:hover").length) {
                    $(_this).popover("hide");
                }
            }, 100);
        }).addClass('init-popover');
        //if ($('html').hasClass('k-ie')) {
        //    $('.popover').remove();
        //    popup.popover("show");
        //    $('.popover-custom').parent('.popover-content').css({ "max-height": "140px" }).niceScroll();
        //    setTimeout(function () {
        //        popup.popover("hide");
        //    }, 2000);
        //} else {
        //    popup.popover("show");
        //    setTimeout(function () {
        //        $('.popover-custom').parent('.popover-content').css({ "max-height": "140px" }).niceScroll();
        //    }, 50);
        //}
    },
    WindowCollapseState: {
        toggle: function () {
            var obj = this;
            var state = 1;
            if ($(obj).hasClass("collapsed")) {
                window.setaJs.WindowCollapseState.collapse.apply(obj, []);
                state = 1;
            } else {
                window.setaJs.WindowCollapseState.expand.apply(obj, []);
                state = 2;
            }
            var $obj = $(obj);
            var isAsync = true;
            if ($obj.attr("stateSync")) {
                isAsync = false;
            }
            $.ajax({
                url: "/State/WindowState",
                type: "GET",
                async: isAsync,
                data: {
                    key: $obj.attr("key"),
                    state: state,
                    value: $obj.attr("value") == 'undefined' ? "" : $obj.attr("value")
                },
                dataType: "json",
                success: function (response) {
                    if (response.status) {

                    }
                }
            });
        },
        expand: function () {
            var obj = this;
            $(obj).removeClass("collapsed");
        },
        collapse: function () {
            var obj = this;
            $(obj).addClass("collapsed");
        }
    },

    //new project
    GenerateId: function () {
        return (new Date()).getTime() * -1;
    },
    genUniqueId: function() {
        
    },

    // move toolbar to footer paging area
    ToolBar: function (idObj) {
        $(idObj + " > .k-grid-pager").prepend($(idObj).find(".k-grid-toolbar"));
    },
    Hierarchy: function (obj, fieldObj, idObj) {
        jQuery(obj).click(function () {
            var _this = this, fieldID;
            fieldID = parseInt(jQuery(_this).closest('tr.k-master-row').find(fieldObj).text());
            setTimeout(function () {
                window.setaJs.ToolBar(idObj + fieldID);

            }, 100);
        });
    },
    ExportExcelPDF: function (obj) {
        jQuery(obj).click(function() {
            var _this = this, url;
            url = jQuery(_this).attr('href');
            window.location.href = url;
        });
    },
    loadContent: function (event) {
        event.preventDefault();

        //show loading screen
        $("#loading_ajax").removeClass('hide');

        //load content
        var link = $(event.target).attr('href');
        $.ajax({
            url: link,
            type: "GET",
            success: function (response) {
                $("#mainContent").html(response);
                $("#loading_ajax").addClass('hide');
                $("#navLabel").text(navLabel);
                document.title = navLabel;
                $(".page-sidebar-menu li").removeClass("active");
                $(event.target).parent().addClass("active");
                $(event.target).parent().parent().parent().addClass("active");
            },
            error: function (err) {
                $("#loading_ajax").addClass('hide');
            }
        });
    },
    getParameterByName: function (name) {
        name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
        var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
            results = regex.exec(location.search);
        return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
    },
    getLocalStorage: function (name) {
        //console.log('get local storage');

        if (name == '') return false;
        var checkVal = localStorage.getItem(name);
        if (checkVal == 'undefined' || checkVal == '') return false;
        data = JSON.parse(localStorage.getItem(name));
        return data;
    },
    setLocalStorage: function (name, value) {
        //console.log('set local storage');

        if (name != '' && value != '') {
            localStorage.setItem(name, JSON.stringify(value));
            return true;
        }
        return false;
    },
    removeLocalStorage: function (name) {
        localStorage.setItem(name, 'null');
    },
    createControlPointer: function (data) {
        var jsonPointer = {};
        for (var ind in data) {
            jsonPointer[data[ind].ItemID] = { jsonVar: data[ind] };
        }
        return jsonPointer;
    },
    createControlPointerOrder: function (data) {
        var jsonPointer = {};
        for (var ind in data) {
            jsonPointer[data[ind].ProductID] = { jsonVar: data[ind] };
        }
        return jsonPointer;
    },
    checkNetConnection: function () {
        var xhr = new XMLHttpRequest();
        var file = "/Assets/img/avatar3_small.png";
        var r = Math.round(Math.random() * 10000);
        xhr.open('HEAD', file + "?subins=" + r, false);
        try {
            xhr.send();
            if (xhr.status >= 200 && xhr.status < 304) {
                return true;
            } else {
                return false;
            }
        } catch (e) {
            return false;
        }
    }
});

//Init setaJs
window.setaJs.init();