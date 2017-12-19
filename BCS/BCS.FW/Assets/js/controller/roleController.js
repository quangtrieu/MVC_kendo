(function (window, $) {
    STBASE.ToolBar('#RoleGrid');
    //Define pluginsádf
    $.fn.roleController = function (options) {
        var defaultOptions = {};
        // Establish our default settings
        var settings = $.extend(defaultOptions, options);
        $.extend(roleController, settings);
        return this.each(function () {
            //code here
        });
    };

    //Private function
    var delRole = function (e) {
        e.preventDefault();
        
        //window.setaJs.setaGridDel(this, ui);
        var _RoleID = parseInt(jQuery(this).closest('tr').find('div.RoleIDField').text());
        
        var _this = this;
        var confirmTitle = "Delete Role";
        var confirmDel = "Are you sure you want to delete this Role?";
        var ui = roleController.ui;
        var options = {
            id: ui.popupAddOrEditId,
            idAction: '#RoleGrid',
            url: '/Role/Delete/' + _RoleID,
            typeStatus: true,
            title: confirmTitle,
            confirmText: confirmDel,
            textYes: "Yes",
            textNo: "No"
        }
        console.log(options);
        var popupWindow = setaJs.initPopupWindow(options);
        popupWindow.refresh({}).center().open();
        setaJs.actionFormWindow("#RoleAddOrEdit", popupWindow, $(ui.grid), options);
    }

    //Global variable - Helpers
    window.roleController = {
        ui: {
            grid: "#RoleGrid",
            inputSearch: "#inputSearchGrid",
            searchFlag: "#searchFlag",
            searchButton: "#btn-grid-search",
            btnAdd: 'a.btn-add',
            popupAddOrEditId: 'RoleAddOrEdit',
            btnShowPermission: 'a.btn-view',
            popupShowPermissionId: 'ShowPermission',
            btnDel: "a.btn-delete",
            urlDel: "/Role/Delete",
            recId: "RoleID",
            confirmTile: "Delete Confirmation",
            confirmDel: "<b>Are you sure you want to delete this Role?</b></br>WARNING: All references to this record will be deleted.",
            btnEdit: 'a.btn-edit',
            btnListForms: 'a.btn-list-forms',
            formId: '#frmAddOrEditRole',
            formPermissionId: '#frmShowPermission',
            //Properties
            RoleID: '#RoleID',
            RoleName: '#RoleName',
            DefaultRole: '#DefaultRole',
            RoleIDCopy: '#RoleIDCopy',
            divSearchToolbar: '#div-search-toolbar',
            //GeneralTab: '#GeneralTab',
        },
        init: function () {
            switch (window.setaJs.oGlobal.action.toLowerCase()) {
                case "create":
                    break;
                case "index":
                    var ui = roleController.ui;

                    //quick search    
                    window.setaJs.quickSearchListView($(ui.searchButton), $(ui.inputSearch), $(ui.searchFlag), $(ui.grid));

                    //Add Notification
                    $(roleController.ui.grid + ' ' + roleController.ui.btnAdd).on("click", roleController.addRole);
                    //Delete Notification
                    var oGrid = $(ui.grid).data("kendoGrid");
                    oGrid.table.on("click", ui.btnDel, delRole);
                    oGrid.table.on("click", ui.btnEdit, roleController.editRole);
                    oGrid.table.on("click", ui.btnShowPermission, roleController.showPermission);
                    break;
                case "details":
                    break;
            }
        },
        // Event edit Role
        showPermission: function (e) {
            e.preventDefault();
            var ui = roleController.ui;
            var options = {
                id: ui.popupShowPermissionId,
                title: 'Edit Permission',
                width: 690
            };
            var popupWindow = setaJs.initPopupWindow(options);
            var row = $(this).closest("tr");
            var grid = $(ui.grid);
            var gridData = grid.data("kendoGrid");

            var dataItem = gridData.dataItem(row);
            popupWindow.refresh({
                url: '/Role/showpermission/' + dataItem.RoleID,
                async: false
            });
            popupWindow.center().open();
            //init form
            roleController.initFormWindow(ui.formPermissionId, popupWindow, $(ui.grid), ui);
        },
        editRole: function (e) {
            e.preventDefault();
            var ui = roleController.ui;
            var row = $(this).closest("tr");
            var grid = $(ui.grid);
            var gridData = grid.data("kendoGrid");
            var dataItem = gridData.dataItem(row);
            roleController.UpdateRoleData(dataItem);
        },
        UpdateRoleData: function (dataItem) {
            var ui = roleController.ui;
            var options = {
                id: ui.popupAddOrEditId,
                title: 'Edit Role',
                width: 690
            };
            var popupWindow = setaJs.initPopupWindow(options);
            popupWindow.refresh({
                url: '/Role/Edit/' + dataItem.RoleID,
                async: false
            });
            popupWindow.center().open();
            //init form
            setaJs.initFormWindow(ui.formId, popupWindow, $(ui.grid), ui);
        },
        // Event add Meal Periods
        addRole: function (e) {
            e.preventDefault();
            var ui = roleController.ui;
            var options = {
                id: ui.popupAddOrEditId,
                title: 'Create Role',
                width: 690
            };
            var popupWindow = setaJs.initPopupWindow(options);

            popupWindow.refresh({
                url: '/Role/Add',
                async: false
            });
            popupWindow.center().open();
            //init form
            setaJs.initFormWindow(ui.formId, popupWindow, $(ui.grid), ui);
        },

        // init popup
        initFormWindow: function (formId, popupWindow, grid, ui) {
            var $form = $(formId);
            //handler button
            $form.find(".k-grid-cancel").on("click", function () {
                if (popupWindow) popupWindow.close();
            });
            $form.find(".k-grid-update").on("click", function () {
                $form.submit();
                setaJs.focusFirstInputForm($form);
            });
            $(":checkbox").attr("autocomplete", "off");
            //handler post form
            $.validator.unobtrusive.parse($form);
            var validator = $form.data("validator");
            //validator.settings.ignore = ':hidden, [readonly=readonly], select';
            validator.settings.submitHandler = function (form) {
                
               // var lstModInRole = $(form).find('input[name="lstModuleID"]').val();
                    var lstModInRole = new Array();
                    $('#ShowPermission input[name="checkedFiles"]').each(function () {
                        var _this = jQuery(this);
                        if (_this.is(':checked')) {
                            lstModInRole += _this.attr("value") + ",";                            
                        }
                    });                    
                    var postData = {
                        RoleID: $(ui.RoleID).val(),
                        RoleName: $(ui.RoleName).val(),
                        listModuleInRole: lstModInRole,
                        DefaultRole: $(ui.DefaultRole).val(),
                        RoleIDCopy: $(ui.RoleIDCopy).val()
                    };
                $.ajax(
                {
                    url: $(form).attr("action"),
                    type: "POST",
                    data: JSON.stringify({ model: postData }),
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    traditional: true,
                    success: function (result, status, xhr) {
                        
                        if (result.Success == true) {
                            if (popupWindow) popupWindow.close();
                            if (grid) {
                                var gridData = grid.data("kendoGrid");
                                if (gridData) gridData.dataSource.read();
                            }
                            return false;
                        } else {
                           // console.log(result.Errors)
                           // validator.showErrors(result.Errors);
                            //display message if available
                            
                            return false;
                        }
                    },
                    error: function (result, textStatus, thrownError) {
                        validator.showErrors(result.status + ": " + thrownError);
                        //if fails
                        return false;
                    }
                });
            };
            //focus first text box
            setaJs.focusFirstInputForm($form);
        },
    };

})(window, jQuery);


var ROLES = {
    onDataBoundRole:function(){
        ROLES.ActDblClickRole();
    },
    onDataBound: function (data) {
        var treeview = $("#treeview").data("kendoTreeView");
        treeview.expand(".k-item");
        jQuery('li.k-item > div input').change(function () {
            if ($(this).is(':checked') == false) {
                var val = jQuery(this).val();
                var dataBase = jQuery(this).closest('li').find('ul.k-group').html();
                jQuery(this).closest('li').find('ul.k-group input').prop('checked', false);
            }
        });
    },
    resultData: function (data) {
        
        setTimeout(function () {
            var lstChecked = $('#ShowPermission').find('input[name="lstModuleID"]').val();            
            for (var i in data.response) {
                if (typeof (data.response[i]) == 'object') {
                    var idObj = data.response[i].id;
                    var idParent = data.response[i].parent;
                    var hasSub =data.response[i].hasChildren;
                    if (lstChecked.indexOf("," + idObj + ",") > -1) {
                        $('#ShowPermission input[value="' + idObj + '"]').prop('checked', true);
                        $('#ShowPermission input[value="' + idParent + '"]').prop('checked', true);
                        
                    }
                    
                    //else {
                    //    $('#ShowPermission input[value="' + idObj + '"]').prop('checked', false);
                    //}
                        
                }
            }
        }, 'fast')
    },
    /****HungPQ Update Action DblClick*****/
    ActDblClickRole: function () {
        jQuery("#RoleGrid table tr td[role='gridcell']").dblclick(function (e) {
            e.preventDefault();
            var grid = jQuery("#RoleGrid").data("kendoGrid");
            var dataItem = grid.dataItem(jQuery(e.currentTarget).closest("tr"));
            roleController.UpdateRoleData(dataItem);
        })
    }

}