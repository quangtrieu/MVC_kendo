if (typeof setaJs != 'undefined')
    setaJs.modules = {
        common: {
            objectDefault: {
                dataListYesNo: [

                    {
                        "Text": "No",
                        "Value": "false",
                        "Selected": false
                    },
                    {
                        "Text": "Yes",
                        "Value": "true",
                        "Selected": false
                    }
                ],
                address: {
                    AddressID: 0,
                    Place: "",
                    ModuleID: 0,
                    ObjectID: setaJs.oGlobal.id,
                    //AddressClass: null,
                    Type: null,
                    IsPrimary: 0,
                    Address1: "",
                    Address2: "",
                    Address3: "",
                    City: "",
                    State: "",
                    PostalCode: "",
                    Country: "",
                    County: "",
                    IsPrimaryBill: 0,
                    IsPrimaryShip: 0,
                    Attention: null,
                    IsPrimaryContact: 0,
                    IsOld: 0,
                    AccountID: 0
                },
                mobile: {
                    MobileID: 0,
                    Type: null,
                    IsPrimary: 0,
                    Phone: '',
                    PhoneMaskId: '',
                    Extension: ''
                },
                fax: {
                    FaxID: 0,
                    Type: null,
                    IsPrimary: 0,
                    FaxNumber: '',
                    FaxMaskId: ''
                },
                email: {
                    EmailID: 0,
                    Type: 1,
                    IsPrimary: 0,
                    EmailAddress: ''
                },
                website: {
                    WebsiteID: 0,
                    Type: 1,
                    IsPrimary: 0,
                    Url: ''
                },
                salemetric: {
                    SaleMetricID: 0,
                    MonthToDate: '',
                    YearToDate: '',
                    PreviousYear: ''
                },
                accountRelated: {
                    AccountRelatedID: 0,
                    AccountRelationshipID: "",
                    AccountInvolvedID: "",
                    AccountOwnerID: "",
                    ContactInvolvedID: "",
                    RelatedTime: $('body').attr('datecurrentuser'),
                    Note: ""
                },
                assistant: {
                    AssistantID: 0,
                    AssitantName: "",
                    IsPrimary: 0,
                    AssitantPhone: "",
                    Email: "",
                    PhoneMaskId: "",
                    Type: 1
                },
                reminder: {
                    ModuleID: 0,
                    ObjectID: 0,
                    Value: 1,
                    Unit: 4
                },
                reminderEvent: {
                    ModuleID: 0,
                    ObjectID: 0,
                    Value: 1,
                    Unit: 4
                },
                attendees: {
                    EventID: 0,
                    UserID: '',
                    AttendeesType: null,
                    Order: 1,
                    Top: 0,
                    Required: false,
                    Type: null
                },
                goal: {
                    GoalID: 0,
                    FromDate: '',
                    ToDate: '',
                    Amount: 0
                },
                territoryBoundary: {
                    TerritoryBoundaryID: 0,
                    TerritoryID: 0,
                    Country: 227,
                    State: '',
                    County: '',
                    PostalCodeFrom: '',
                    PostalCodeTo: '',
                    Scope: 1
                },
                itemcomponent: {
                    ItemComponentId: 0,
                    KitItemId: null,
                    ItemId: null,
                    Quantity: 1,
                    UnitPrice: 0,
                    DiscountMethod: 0,
                    DiscountAmount: 0,
                    ExtendedPrice: 0
                },
                itemsownedcomponent: {
                    ItemsOwnedComponentId: 0,
                    ItemsOwnedId: null,
                    ItemId: null,
                    ItemDescription: '',
                    SerialLotNumber: '',
                    SerialLotItem: 1,
                    Quantity: 1,
                    Note: ''
                },
                coveredItem: {
                    ServiceContract_CoveredItemID: '',
                    ItemID: '',
                    ScopeOfCoverage: 2,
                    ServiceContractID: ''
                },
                coveredPart: {
                    ServiceContract_PartID: '',
                    ItemID: '',
                    CoverageScope: 2,
                    ServiceContractID: ''
                },
                coveredLabor: {
                    ServiceContract_JobID: '',
                    CoverageScope: 2,
                    JobType: '',
                    ServiceContractID: ''
                },
                coveredExpenses: {
                    ServiceContract_ExpenseID: '',
                    ExpenseCategory: '',
                    ServiceContractID: ''
                },
                authorizedContacts: {
                    ServiceContract_AuthorizedContactID: '',
                    ContactID: '',
                    ServiceContractID: ''
                }
            },
            method: {
                //Data Post
                dataAddressPost: function (selector) {
                    var addresses = [];
                    $(selector).each(function () {
                        var currentNumber = $(this).attr('data-number');
                        var attr = $(this).attr('data-edit-id');
                        var oAddress = {
                            Place: $('input[name="Addresses[' + currentNumber + '].Place"]').val(),
                            Address1: $.trim($('input[name="Addresses[' + currentNumber + '].Address1"]').val()),
                            Address2: $.trim($('input[name="Addresses[' + currentNumber + '].Address2"]').val()),
                            Address3: $.trim($('input[name="Addresses[' + currentNumber + '].Address3"]').val()),
                            Type: parseInt($('input[name="Addresses[' + currentNumber + '].Type"]').val()),
                            City: $('input[name="Addresses[' + currentNumber + '].City"]').val(),
                            PostalCode: $('input[name="Addresses[' + currentNumber + '].PostalCode"]').val(),
                            State: $('input[name="Addresses[' + currentNumber + '].State"]').val(),
                            County: $('input[name="Addresses[' + currentNumber + '].County"]').val(),
                            Country: $('input[name="Addresses[' + currentNumber + '].Country"]').val(),
                            IsPrimaryBill: $('input[name="Addresses[' + currentNumber + '].IsPrimaryBill"]').is(':checked'),
                            Attention: $('input[name="Addresses[' + currentNumber + '].Attention"]').val(),
                            IsPrimaryShip: $('input[name="Addresses[' + currentNumber + '].IsPrimaryShip"]').is(':checked'),
                            IsPrimary: $('input[name="Addresses[' + currentNumber + '].IsPrimary"]').is(':checked'),
                            IsPrimaryContact: $('input[name="Addresses[' + currentNumber + '].IsPrimaryContact"]').is(':checked')
                        };

                        /*var addressClass = $('input[name="Addresses[' + currentNumber + '].AddressClass"]');
                        if (addressClass.length > 0) {
                            oAddress.AddressClass = parseInt(addressClass.val());
                        }*/
                        if (typeof attr !== 'undefined' && attr !== false) { //has attr - edit
                            oAddress.AddressID = parseInt(attr);
                        }
                        addresses.push(oAddress);
                    });
                    return addresses;
                },
                dataMobilePost: function (selector) {
                    var mobiles = [];
                    $(selector).each(function () {
                        var currentNumber = $(this).attr('data-number');
                        var attr = $(this).attr('data-edit-id');
                        var oMobile = {
                            Type: parseInt($('input[name="Mobiles[' + currentNumber + '].Type"]').val()),
                            IsPrimary: ($('input[name="Mobiles[' + currentNumber + '].IsPrimary"]').val() == "true") ? true : false,
                            Phone: $('input[name="Mobiles[' + currentNumber + '].Phone"]').val(),
                            PhoneMaskId: $('input[name="Mobiles[' + currentNumber + '].PhoneMaskId"]').val(),
                            Extension: $('input[name="Mobiles[' + currentNumber + '].Extension"]').val()
                        };
                        if (typeof attr !== 'undefined' && attr !== false) { //has attr - edit
                            oMobile.MobileID = parseInt(attr);
                        }
                        mobiles.push(oMobile);
                    });
                    return mobiles;
                },
                dataFaxPost: function (selector) {
                    var faxes = [];
                    $(selector).each(function () {
                        var currentNumber = $(this).attr('data-number');
                        var attr = $(this).attr('data-edit-id');
                        var oFax = {
                            Type: parseInt($('input[name="Faxes[' + currentNumber + '].Type"]').val()),
                            IsPrimary: ($('input[name="Faxes[' + currentNumber + '].IsPrimary"]').val() == "true") ? true : false,
                            FaxNumber: $('input[name="Faxes[' + currentNumber + '].FaxNumber"]').val(),
                            FaxMaskId: $('input[name="Faxes[' + currentNumber + '].FaxMaskId"]').val()
                        };
                        if (typeof attr !== 'undefined' && attr !== false) { //has attr - edit
                            oFax.FaxID = parseInt(attr);
                        }
                        faxes.push(oFax);
                    });
                    return faxes;
                },
                dataEmailPost: function (selector) {
                    var emails = [];
                    $(selector).each(function () {
                        var currentNumber = $(this).attr('data-number');
                        var attr = $(this).attr('data-edit-id');
                        var oEmail = {
                            IsPrimary: ($('input[name="Emails[' + currentNumber + '].IsPrimary"]').val() == "true") ? true : false,
                            EmailAddress: $('input[name="Emails[' + currentNumber + '].EmailAddress"]').val()
                        };
                        if (typeof attr !== 'undefined' && attr !== false) { //has attr - edit
                            oEmail.EmailID = parseInt(attr);
                        }
                        emails.push(oEmail);
                    });
                    return emails;
                },
                dataWebsitePost: function (selector) {
                    var websites = [];
                    $(selector).each(function () {
                        var currentNumber = $(this).attr('data-number');
                        var attr = $(this).attr('data-edit-id');
                        var oWebsite = {
                            IsPrimary: ($('input[name="Websites[' + currentNumber + '].IsPrimary"]').val() == "true") ? true : false,
                            Url: $('input[name="Websites[' + currentNumber + '].Url"]').val()
                        };
                        if (typeof attr !== 'undefined' && attr !== false) { //has attr - edit
                            oWebsite.WebsiteID = parseInt(attr);
                        }
                        websites.push(oWebsite);
                    });
                    return websites;
                },
                dataSaleMetricPost: function (selector) {
                    var saleMetrics = [];
                    var isprimary = true;
                    $(selector).each(function () {
                        var currentNumber = $(this).attr('data-number');
                        var attr = $(this).attr('data-edit-id');
                        var oSaleMetric = {
                            MonthToDate: $('input[name="SaleMetrics[' + currentNumber + '].MonthToDate"]').val(),
                            YearToDate: $('input[name="SaleMetrics[' + currentNumber + '].YearToDate"]').val(),
                            PreviousYear: $('input[name="SaleMetrics[' + currentNumber + '].PreviousYear"]').val()
                        };
                        if (typeof attr !== 'undefined' && attr !== false) { //has attr - edit
                            oSaleMetric.SaleMetricID = parseInt(attr);
                        }
                        isprimary = false;
                        saleMetrics.push(oSaleMetric);
                    });
                    return saleMetrics;
                },
                dataGoalPost: function (selector) {
                    var goals = [];
                    $(selector).each(function () {
                        var currentNumber = $(this).attr('data-number');
                        var attr = $(this).attr('data-edit-id');
                        var oGoal = {
                            FromDate: $('input[name="Goals[' + currentNumber + '].FromDate"]').val(),
                            ToDate: $('input[name="Goals[' + currentNumber + '].ToDate"]').val(),
                            Amount: $('input[name="Goals[' + currentNumber + '].Amount"]').val()
                        };
                        if (typeof attr !== 'undefined' && attr !== false) { //has attr - edit
                            oGoal.GoalID = parseInt(attr);
                        }
                        goals.push(oGoal);
                    });
                    return goals;
                },
                //dataAttendeesPost: function (selector) {
                //    var attendees = [];
                //    $(selector).each(function () {
                //        var currentNumber = $(this).attr('data-number');
                //        if (currentNumber != 'undefined' || currentNumber != null) {
                //            var attr = $(this).attr('data-edit-id');
                //            var oAttendees = {
                //                EventID: 0,
                //                UserID: parseInt($('input[name="Attendees[' + currentNumber + '].UserID"]').val()),
                //                AttendeesType: $('input[name="Attendees[' + currentNumber + '].UserID"]').attr('data-type'),
                //                Order: currentNumber
                //            };
                //            if (typeof attr !== 'undefined' && attr !== false) { //has attr - edit
                //                oAttendees.EventID = parseInt(attr);
                //            }
                //            attendees.push(oAttendees);
                //        }
                //    });
                //    return attendees;
                //},
                dataAccountRelatedPost: function (selector) {
                    var relatedAccounts = [];
                    $(selector).each(function () {
                        var currentNumber = $(this).attr('data-number');
                        var attr = $(this).attr('data-edit-id');
                        var oRelated = {
                            AccountRelationshipID: parseInt($('input[name="AccountRelatedOwners[' + currentNumber + '].AccountRelationshipID"]').val()),
                            AccountInvolvedID: $('input[name="AccountRelatedOwners[' + currentNumber + '].AccountInvolvedID"]').data("kendoComboBox").value(),
                            ContactInvolvedID: $('input[name="AccountRelatedOwners[' + currentNumber + '].ContactInvolvedID"]').data("kendoComboBox").value(),
                            RelatedTime: $('input[name="AccountRelatedOwners[' + currentNumber + '].RelatedTime"]').data("kendoDateTimePicker").value(),
                            Note: $('textarea[name="AccountRelatedOwners[' + currentNumber + '].Note"]').val()
                        };
                        if (typeof attr !== 'undefined' && attr !== false) { //has attr - edit
                            oRelated.AccountOwnerID = parseInt($(this).attr('data-owner'));
                            oRelated.AccountRelatedID = parseInt(attr);;
                        }
                        relatedAccounts.push(oRelated);
                    });
                    return relatedAccounts;
                },
                dataItemComponentPost: function (selector) {
                    var itemComponents = new Array();
                    $(selector).each(function () {
                        var currentNumber = $(this).attr('data-number');
                        var attr = $(this).attr('data-edit-id');
                        var oItem = {
                            ItemId: parseInt($('input[name="KitItems[' + currentNumber + '].ItemId"]').data("kendoComboBox").value()),
                            Quantity: $('input[name="KitItems[' + currentNumber + '].Quantity"]').val(),
                            UnitPrice: $('input[name="KitItems[' + currentNumber + '].UnitPrice"]').val(),
                            DiscountMethod: $('input[name="KitItems[' + currentNumber + '].DiscountMethod"]').data("kendoDropDownList").value(),
                            DiscountAmount: $('input[name="KitItems[' + currentNumber + '].DiscountAmount"]').val(),
                            ExtendedPrice: $('input[name="KitItems[' + currentNumber + '].ExtendedPrice"]').val(),
                            DataNumber: currentNumber
                        };
                        if (typeof attr !== 'undefined' && attr !== false) { //has attr - edit
                            oItem.ItemComponentId = parseInt(attr);;
                        }
                        itemComponents.push(oItem);

                    });
                    return itemComponents;
                },
                dataItemsOwnedComponentPost: function (selector) {
                    var itemsOwnedComponents = new Array();
                    $(selector).each(function () {
                        var currentNumber = $(this).attr('data-number');
                        var attr = $(this).attr('data-edit-id');
                        var oItemsOwned = {
                            ItemId: parseInt($('input[name="KitItemsOwneds[' + currentNumber + '].ItemId"]').data("kendoComboBox").value()),
                            ItemDescription: $('input[name="KitItemsOwneds[' + currentNumber + '].ItemDescription"]').val(),
                            SerialLotNumber: $('input[name="KitItemsOwneds[' + currentNumber + '].SerialLotNumber"]').val(),
                            SerialLotItem: $('input[name="KitItemsOwneds[' + currentNumber + '].SerialLotItem"]').val(),
                            Quantity: $('input[name="KitItemsOwneds[' + currentNumber + '].Quantity"]').val(),
                            Note: $('textarea[name="KitItemsOwneds[' + currentNumber + '].Note"]').val(),
                            DataNumber: currentNumber
                        };
                        if (typeof attr !== 'undefined' && attr !== false) { //has attr - edit
                            oItemsOwned.ItemsOwnedComponentId = parseInt(attr);;
                        }
                        itemsOwnedComponents.push(oItemsOwned);

                    });
                    return itemsOwnedComponents;
                },
                dataAssistant: function (selector) {
                    var assistant = [];
                    $(selector).each(function (index) {
                        var currentNumber = $(this).attr('data-number');
                        var attr = $(this).attr('data-edit-id');
                        var oAssistant = {
                            AssitantName: $('input[name="Assistants[' + currentNumber + '].AssitantName"]').val(),
                            IsPrimary: ($('input[name="Assistants[' + currentNumber + ']"].IsPrimary').val() == "true") ? true : false,
                            AssitantPhone: $('input[name="Assistants[' + currentNumber + '].AssitantPhone"]').val(),
                            Email: $('input[name="Assistants[' + currentNumber + '].Email"]').val(),
                            PhoneMaskId: $('input[name="Assistants[' + currentNumber + '].PhoneMaskId"]').val()
                        };
                        if (typeof attr !== 'undefined' && attr !== false) { //has attr - edit
                            oAssistant.AssistantID = parseInt(attr);
                        }
                        assistant.push(oAssistant);
                    });
                    return assistant;
                },
                successAndRedirect: function (val, result) {
                    $("#loading-ajax").find("span").first().html("Redirecting...");
                    setTimeout(function () {
                        $("#loading-ajax").removeClass('hide');
                    }, 10);

                    // just reload the page for now
                    switch (val) {
                        case "save":
                        case "save-open"://save and view detail
                        case "save-new":
                            //save and new record
                            if (typeof result.returnUrl != 'undefined' && result.returnUrl != '') {
                                window.location.href = result.returnUrl;
                                return;
                            }
                            window.setaJs.goBack();
                            //location.reload(true);
                            break;
                    }
                },
                //Bind For Form
                handlerPanelButton: function (form) {
                    var t = this;
                    //console.log(t);
                    $('.btn-crm-cancel, .btn-crm-save-new, .btn-crm-save-open, .btn-crm-submit').on("click", function (e) {
                        //e.preventDefault();
                        //window.setaJs.log(form.valid()); //true is pass, false is not valid
                        var typeRedirect = "";
                        switch (true) { // method hasClass() better method is() - Performance
                            case $(this).hasClass('btn-crm-save-new'):
                                typeRedirect = "save-new";
                                break;
                            case $(this).hasClass('btn-crm-save-open'):
                                typeRedirect = "save-open";
                                break;
                            case $(this).hasClass('btn-crm-submit'):
                                typeRedirect = "save";
                                break;
                            case $(this).hasClass('btn-crm-cancel'):
                                e.preventDefault();
                                window.setaJs.goBack();
                                return false;
                                break;
                        }
                        if (!t.checkElementExist('#typeRedirect')) {
                            $("<input name='typeRedirect' id='typeRedirect' type='hidden' value='" + typeRedirect + "' />").appendTo('#' + form.attr('id'));
                        } else {
                            $('#typeRedirect').val(typeRedirect);
                        }
                    });
                },
                //
                handlerAddElement: function (control, maxElement, settings, selectorForm) {
                    var t = this;
                    var data = {};
                    switch (control) {
                        case "address":
                            data = window.setaJs.modules.common.objectDefault.address;
                            if (settings.AccountID != undefined)
                                data.AccountID = settings.AccountID;
                            if (settings.ModuleID != undefined)
                                data.ModuleID = settings.ModuleID;
                            if (settings.Attention != undefined)
                                data.Attention = settings.Attention;
                            break;
                        case "phone":
                            data = window.setaJs.modules.common.objectDefault.mobile;
                            break;
                        case "fax":
                            data = window.setaJs.modules.common.objectDefault.fax;
                            break;
                        case "related":
                            data = window.setaJs.modules.common.objectDefault.accountRelated;
                            break;
                        case "assistants":
                            data = window.setaJs.modules.common.objectDefault.assistant;
                            break;
                        case "managers":
                            data = window.setaJs.modules.common.objectDefault.assistant;
                            data.Type = 2;
                            break;
                        case "Reminder":
                            data = window.setaJs.modules.common.objectDefault.reminder;
                            break;
                        case "ReminderEvent":
                            data = window.setaJs.modules.common.objectDefault.reminderEvent;
                            break;
                        case "email":
                            data = window.setaJs.modules.common.objectDefault.email;
                            break;
                        case "website":
                            data = window.setaJs.modules.common.objectDefault.website;
                            break;
                        case "saleMetric":
                            data = window.setaJs.modules.common.objectDefault.salemetric;
                            break;
                        case "attendees":
                            data = window.setaJs.modules.common.objectDefault.attendees;
                            break;
                        case "goal":
                            data = window.setaJs.modules.common.objectDefault.goal;
                            break;
                        case "territoryBoundary":
                            data = window.setaJs.modules.common.objectDefault.territoryBoundary;
                            break;
                        case "itemcomponent":
                            data = window.setaJs.modules.common.objectDefault.itemcomponent;
                            break;
                        case "itemsownedcomponent":
                            data = window.setaJs.modules.common.objectDefault.itemsownedcomponent;
                            break;
                        case "coveredItem":
                            data = window.setaJs.modules.common.objectDefault.coveredItem;
                        case "coveredItemOwned":
                            data = window.setaJs.modules.common.objectDefault.coveredItem;
                            break;
                        case "coveredPart":
                            data = window.setaJs.modules.common.objectDefault.coveredPart;
                            break;
                        case "coveredLabor":
                            data = window.setaJs.modules.common.objectDefault.coveredLabor;
                            break;
                        case "coveredExpenses":
                            data = window.setaJs.modules.common.objectDefault.coveredExpenses;
                            break;
                        case "authorizedContacts":
                            data = window.setaJs.modules.common.objectDefault.authorizedContacts;
                            break;
                    }

                    data.numberInc = maxElement;
                    t.renderElement(control, data, settings, selectorForm);
                },
                removeRow: function (elBtnDel, elForm) {
                    //var el = $(this).closest(".delete-row"); //find parent has class .delete-row
                    var el = $(elBtnDel).closest(".delete-row"); //find parent has class .delete-row
                    var rowData = el.prev('.row');
                    var elValid = "AddressPrimary";
                    switch (true) { // method hasClass() better method is() - Performance
                        case $(rowData).hasClass('row-address'):
                            elValid = "";
                            break;
                        case $(rowData).hasClass('row-phone'):
                            elValid = "PhonePrimary";
                            break;
                        case $(rowData).hasClass('row-assistant'):
                            elValid = "AssistantPrimary";
                            break;
                        case $(rowData).hasClass('row-related'):
                            elValid = "RelatedType";
                            break;
                        case $(rowData).hasClass('row-reminder'):
                            elValid = "Reminder";
                            break;
                        case $(rowData).hasClass('row-reminder-event'):
                            elValid = "ReminderEvent";
                            break;
                        case $(rowData).hasClass('row-itemcomponent'):
                            elValid = "ItemComponent";
                            break;
                        case $(rowData).hasClass('row-itemsownedcomponent'):
                            elValid = "ItemsOwnedComponent";
                            break;
                    }
                    rowData.remove();
                    el.remove();
                    if (elValid == "Reminder" || elValid == "ReminderEvent" || elValid == "ItemsOwnedComponent") return;
                    if (elValid != "")
                        $('input[name^="' + elValid + '"]').valid();
                    //$('AddressPrimary').valid();
                    return;
                },
                removeEmailRow: function (elBtnDel, elForm) {
                    var rowData = $(elBtnDel).closest(".row-email"); //find parent has class .delete-row
                    //var rowData = el.prev('.row');
                    rowData.remove();
                    //el.remove();
                    return;
                },
                removeWebsiteRow: function (elBtnDel, elForm) {
                    var rowData = $(elBtnDel).closest(".row-website"); //find parent has class .delete-row
                    //var rowData = el.prev('.row');
                    rowData.remove();
                    //el.remove();
                    return;
                },
                removeSaleMetricRow: function (elBtnDel, elForm) {
                    var rowData = $(elBtnDel).closest(".row-sale-metric"); //find parent has class .delete-row
                    //var rowData = el.prev('.row');
                    rowData.remove();
                    //el.remove();
                    return;
                },
                removeTerritoryBoundaryRow: function (elBtnDel, elForm) {
                    var rowData = $(elBtnDel).closest(".row-boundary");
                    rowData.remove();
                    return;
                },
                removeGoalRow: function (elBtnDel, elForm) {
                    var rowData = $(elBtnDel).closest(".row-goal");
                    rowData.remove();
                    return;
                },
                removeCoveredItemRow: function (elBtnDel, elForm) {
                    var rowData = $(elBtnDel).closest(".row-covered-item");
                    rowData.remove();
                    return;
                },
                removeCoveredItemOwnedRow: function (elBtnDel, elForm) {
                    var rowData = $(elBtnDel).closest(".row-covered-item-owned");
                    rowData.remove();
                    return;
                },
                removeCoveredPartRow: function (elBtnDel, elForm) {
                    var rowData = $(elBtnDel).closest(".row-covered-part");
                    rowData.remove();
                    return;
                },
                removeCoveredLaborRow: function (elBtnDel, elForm) {
                    var rowData = $(elBtnDel).closest(".row-covered-labor");
                    rowData.remove();
                    return;
                },
                removeCoveredExpensesRow: function (elBtnDel, elForm) {
                    var rowData = $(elBtnDel).closest(".row-covered-expense");
                    rowData.remove();
                    return;
                },
                removeAuthorizedContactsRow: function (elBtnDel, elForm) {
                    var rowData = $(elBtnDel).closest(".row-authorized-contacts");
                    rowData.remove();
                    return;
                },
                /*duplicateRow: function (source, destination, selectorForm) {
                    var idSource = source.attr('data-number');
                    var idDest = destination.attr('data-number');

                    source.find('input').each(function (i) {
                        var nameDest = $(this).attr('name').replace('[' + idSource + ']', '[' + idDest + ']');
                        var attrTypeBind = $(this).attr('type-bind');
                        var elDest = 'input[name="{0}"]'.format(nameDest);
                        if (typeof (attrTypeBind) != 'undefined') {
                            switch (attrTypeBind) {
                                case "kendoDropDownList":
                                    $(elDest).data(attrTypeBind).value($(this).val());
                                    break;
                                case "kendoComboBox":
                                    $(elDest).data(attrTypeBind).value($(this).val());
                                    break;
                            }
                        } else {
                            $(elDest).val($(this).val());
                        }
                    });
                    //$.validator.unobtrusive.parseDynamicContent('#frm-create');
                    $.validator.unobtrusive.parseDynamicContent(selectorForm);
                },*/
                renderElement: function (control, data, settings, selectorForm, isFocus) {
                    var t = this;
                    data.AccountID = settings.AccountID;
                    //Get the external template definition using a jQuery selector
                    //using useWithBlock for performance template
                    window.setaJs.setValueDefaultObject(data);
                    var templ = kendo.template($("#templ" + control.capitalize()).html(), { useWithBlock: false });
                    if (typeof templ == "function") {
                        // check for address
                        if (control == "address") {
                            var hasPrimary = false;
                            var length = $('input.primary-address').length;
                            if (length == 0) {
                                if (data.IsOld != undefined && !data.IsOld) {
                                    data.Place = setaJs.l('TEXT_ADDRESS_MAIN');
                                    data.IsPrimary = true;
                                }
                            }
                            else {
                                $('input.primary-address').each(function () {
                                    if ($(this).is(':checked')) {
                                        data.IsPrimary = false;
                                        return;
                                    }
                                });
                            }

                            var hasPrimaryContact = false;
                            var length = $('input.primary-address-contact').length;
                            if (length == 0) {
                                if (data.IsOld != undefined && !data.IsOld) {
                                    if (data.ModuleID == 1) {
                                        data.Place = $('#FullName').val();
                                        data.IsPrimaryContact = true;
                                    }
                                }
                            }
                            else {
                                $('input.primary-address-contact').each(function () {
                                    if ($(this).is(':checked')) {
                                        data.IsPrimaryContact = false;
                                        return;
                                    }
                                });
                            }
                            if (data.Country == "") {
                                data.Country = "United States";
                            }
                        }
                        var result = templ(data); //Template
                        data.Place = '';
                        $('#create-' + control).append(result); //Append the result
                        switch (control) {
                            case "address":
                                t.addressElement(data, data.numberInc, settings, isFocus);
                                break;
                            case "phone":
                                t.phoneElement(data.numberInc, settings, isFocus);
                                break;
                            case "fax":
                                t.faxElement(data.numberInc, settings, isFocus);
                                break;
                            case "related":
                                t.reletedElement(data.numberInc, settings, isFocus);
                                break;
                            case "assistants":
                                t.assistantsElement(data.numberInc, settings, isFocus);
                                break;
                            case "managers":
                                t.managersElement(data.numberInc, settings, isFocus);
                                break;
                            case "Reminder":
                                t.reminderElement(data.numberInc, settings, isFocus);
                                break;
                            case "ReminderEvent":
                                t.reminderElement(data.numberInc, settings, isFocus);
                                break;
                            case "email":
                                t.emailElement(data.numberInc, settings, isFocus);
                                break;
                            case "website":
                                t.websiteElement(data.numberInc, settings, isFocus);
                                break;
                            case "saleMetric":
                                t.saleMetricElement(data.numberInc, settings, isFocus);
                                break;
                            case "attendees":
                                t.attendeesElement(data.numberInc, settings, isFocus);
                                break;
                            case "goal":
                                t.goalElement(data.numberInc, settings, isFocus);
                                break;
                            case "territoryBoundary":
                                t.territoryBoundaryElement(data.numberInc, settings, isFocus);
                                break;
                            case "itemcomponent":
                                t.itemcomponentElement(data.numberInc, settings, isFocus);
                                break;
                            case "itemsownedcomponent":
                                t.itemOwnedcomponentElement(data.numberInc, settings, isFocus);
                                break;
                            case "coveredItem":
                                t.coveredItemElement(data.numberInc, settings, isFocus);
                                break;
                            case "coveredItemOwned":
                                t.coveredItemOwnedElement(data.numberInc, settings, isFocus);
                                break;
                            case "coveredPart":
                                t.coveredPartElement(data.numberInc, settings, isFocus);
                                break;
                            case "coveredLabor":
                                t.coveredLaborElement(data.numberInc, settings, isFocus);
                                break;
                            case "coveredExpenses":
                                t.coveredExpensesElement(data.numberInc, settings, isFocus);
                                break;
                            case "authorizedContacts":
                                t.coveredAuthorizedContactsElement(data.numberInc, settings, isFocus);
                                break;
                        }
                    }
                    if (control == "Reminder" || control == "ReminderEvent") return;
                    $.validator.unobtrusive.parseDynamicContent(selectorForm);
                    $.validator.unobtrusive.parse(selectorForm);
                },
                addressElement: function (data, numberIncr, settings, isFocus) {
                    $('input[name="Addresses[' + numberIncr + '].Type"]')
                                        .kendoDropDownList({
                                            dataSource: settings.dataView.oAddressType,
                                            dataTextField: "Text",
                                            dataValueField: "Value"
                                        });

                    var template = kendo.template("<div class='header'><span> #=name # </span></div>");
                    var templateData = { name: window.setaJs.l('Show_Top_Results') };
                    var tempHtml = template(templateData);
                    $('input[name="Addresses[' + numberIncr + '].Attention"]')
                         .kendoComboBox({
                             placeholder: "Select Attention",
                             dataTextField: "FullName",
                             dataValueField: "ContactID",
                             filter: "contains",
                             autoBind: true,
                             minLength: 0,
                             suggest: true,
                             headerTemplate: tempHtml,
                             open: kendoComboboxFix.scrollFixHeader,
                             change: function () {
                                 var selectedIndex = this.select();
                                 if (selectedIndex < 0) {
                                     this.value(null);
                                 }
                             },
                             dataSource: {
                                 type: "json",
                                 serverFiltering: true,
                                 transport: {
                                     read: {
                                         url: "/Address/GetContacts",
                                         data: function (sender) {
                                             var recordName = $('input[name="Addresses[' + numberIncr + '].Attention"]').getKendoComboBox().input.val();
                                             if (sender.filter == undefined || sender.filter.filters == undefined)
                                                 recordName = ''; //Clear value
                                             return {
                                                 contactId: $('input[name="Addresses[' + numberIncr + '].Attention"]').attr("data-id"),
                                                 recordName: recordName,
                                                 accountId: data.AccountID,
                                                 currentContactId: (setaJs.oGlobal.CurrentModule == 1) ? setaJs.oGlobal.id : 0
                                             }
                                         }
                                     }
                                 }
                             },
                             dataBound: function (e) {
                                 $('input[name="Addresses[' + numberIncr + '].Attention"]').attr("data-id", 0);
                             }
                         });


                    $('.row-address .primary-address').click(function (e) {
                        if ($(this).is(':checked')) {
                            $('.row-address input.primary-address.checked').attr('checked', null).attr('value', false);
                            $('.row-address input.primary-address').removeClass('checked');
                            $(this).prop('checked', true).attr('value', true);
                            $(this).addClass('checked');
                        } else {
                            $(this).removeClass('checked').attr('value', false);
                        }
                    });

                    $('.row-address .primary-address-contact').click(function (e) {
                        if ($(this).is(':checked')) {
                            $('.row-address input.primary-address-contact.checked').attr('checked', null).attr('value', false);
                            $('.row-address input.primary-address-contact').removeClass('checked');
                            $(this).prop('checked', true).attr('value', true);
                            $(this).addClass('checked');
                        } else {
                            $(this).removeClass('checked').attr('value', false);
                        }
                    });

                    $('.row-address .primary-address-bill').click(function (e) {
                        if ($(this).is(':checked')) {
                            $('.row-address input.primary-address-bill.checked').attr('checked', null).attr('value', false);
                            $('.row-address input.primary-address-bill').removeClass('checked');
                            $(this).prop('checked', true).attr('value', true);
                            $(this).addClass('checked');
                        } else {
                            $(this).removeClass('checked').attr('value', false);
                        }
                    });

                    $('.row-address .primary-address-ship').click(function (e) {
                        if ($(this).is(':checked')) {
                            $('.row-address input.primary-address-ship.checked').attr('checked', null).attr('value', 'false');
                            $('.row-address input.primary-address-ship').removeClass('checked');
                            $(this).prop('checked', true).attr('value', true);
                            $(this).addClass('checked');
                        } else {
                            $(this).removeClass('checked').attr('value', false);
                        }
                    });


                    ////Rules validation address
                    //$('input[name="Addresses[' + numberIncr + '].IsPrimary"]').rules("add", {
                    //    required: true,
                    //    onlyPrimary: {
                    //        el: 'input[id^="AddressPrimary"]',
                    //        msg: window.setaJs.l("Common_OnlyPrimaryValue").format("addres"),
                    //        primaryValue: "true"
                    //    }
                    //});

                    //$('input[name^="Street1"]').each(function () {
                    //    $(this).rules("add", {
                    //        required: true,
                    //        checkSameValue: {
                    //            el: 'input[name^="Street1"]',
                    //            msg: setaJs.l("Common_EnterDiffValue").format("addres")
                    //        }
                    //    });
                    //});

                    window.setaJs.modules.common.method.autocompletePostalCode('input[name="Addresses[' + numberIncr + '].PostalCode"]', 'input[name="Addresses[' + numberIncr + '].City"]', 'input[name="Addresses[' + numberIncr + '].State"]', 'input[name="Addresses[' + numberIncr + '].County"]', 'input[name="Addresses[' + numberIncr + '].Country"]');
                    window.setaJs.modules.common.method.autocompleteState('input[name="Addresses[' + numberIncr + '].State"]', 'input[name="Addresses[' + numberIncr + '].Country"]');
                    window.setaJs.modules.common.method.autocompleteCountry('input[name="Addresses[' + numberIncr + '].State"]', 'input[name="Addresses[' + numberIncr + '].Country"]');
                    return this; //fluent
                },
                phoneElement: function (numberIncr, settings, isFocus) {
                    var t = this;
                    $('input[name="Mobiles[' + numberIncr + '].IsPrimary"]')
                                        .kendoDropDownList({
                                            "dataSource": window.setaJs.modules.common.objectDefault.dataListYesNo,
                                            "dataTextField": "Text",
                                            "dataValueField": "Value",
                                            change: function (e) {
                                                var value = this.value();
                                                t.onChangeYesNo(value, "Mobiles", numberIncr);
                                            }
                                        });
                    $('input[name="Mobiles[' + numberIncr + '].PhoneMaskId"]')
                                        .kendoComboBox({
                                            placeholder: "Select country",
                                            dataTextField: "CountryName",
                                            dataValueField: "PhoneFormatId",
                                            filter: "contains",
                                            autoBind: true,
                                            suggest: true,
                                            minLength: 1,
                                            change: function (e) {
                                                var selectedIndex = this.select();
                                                if (selectedIndex < 0) {
                                                    this.value(null);
                                                    this.dataSource.filter([]);
                                                    $('input[name="Mobiles[' + numberIncr + '].Phone"]').val('');
                                                    $('input[name="Mobiles[' + numberIncr + '].Phone"]').mask(' ');
                                                } else {
                                                    $.ajax({
                                                        url: "/Mobile/GetCountryById?countryId=" + this.value(),
                                                        type: 'GET',
                                                        contentType: 'application/json;',
                                                        dataType: 'json',
                                                        cache: false,
                                                        success: function (result) {
                                                            $('input[name="Mobiles[' + numberIncr + '].Phone"]').val('');
                                                            $('input[name="Mobiles[' + numberIncr + '].Phone"]').mask(result.Mask);
                                                        }
                                                    });
                                                }
                                            },
                                            dataSource: {
                                                type: "json",
                                                serverFiltering: true,
                                                transport: {
                                                    read: {
                                                        url: "/Mobile/GetCountries",
                                                    }
                                                }
                                            }
                                        });
                    //init mask if edit
                    var phoneMaskId = $('input[name="Mobiles[' + numberIncr + '].PhoneMaskId"]').val();
                    if ($.isNumeric(phoneMaskId)) {
                        $.ajax({
                            url: "/Mobile/GetCountryById?countryId=" + phoneMaskId,
                            type: 'GET',
                            contentType: 'application/json;',
                            dataType: 'json',
                            cache: false,
                            success: function (result) {
                                $('input[name="Mobiles[' + numberIncr + '].Phone"]').mask(result.Mask);
                            }
                        });
                    } else {
                        $('input[name="Mobiles[' + numberIncr + '].Phone"]').mask("+1(###)###-####");
                        $('input[name="Mobiles[' + numberIncr + '].PhoneMaskId"]').data("kendoComboBox").value(292);
                    }
                    $('input[name="Mobiles[' + numberIncr + '].Type"]')
                        .kendoDropDownList({
                            dataSource: settings.dataView.oPhoneType,
                            dataTextField: "Text",
                            dataValueField: "Value"
                        });

                    //Rules validation phone
                    $('input[name="Mobiles[' + numberIncr + '].Phone"]').rules("add", {
                        phoneNumber: true
                    });

                    $('input[name="Mobiles[' + numberIncr + '].Extension"]').rules("add", {
                        // phoneExtension: true
                    });

                    //$('input[name="Mobiles[' + numberIncr + '].Phone"]').keypress(function(e) {
                    //    var charCode = (e.which) ? e.which : e.keyCode;
                    //    if (charCode != 46 && charCode != 40 && charCode != 41 && charCode != 43 && charCode != 45 && charCode > 31 && (charCode < 48 || charCode > 57))
                    //        return false;
                    //    return true;
                    //});

                    //$('input[name^="PhoneNumer"]').each(function () {
                    //    $(this).rules("add", {
                    //        required: true,
                    //        checkSameValue: {
                    //            el: 'input[name^="PhoneNumer"]',
                    //            msg: setaJs.l("Common_EnterDiffValue").format("phone")
                    //        }
                    //    });
                    //});

                    //$('input[name="PhonePrimary[' + numberIncr + ']"]').rules("add", {
                    //    required: true,
                    //    onlyPrimary: {
                    //        el: 'input[name^="PhonePrimary"]',
                    //        msg: window.setaJs.l("Common_OnlyPrimaryValue").format("phone number"),
                    //        primaryValue: "true"
                    //    }
                    //});
                    return this; //fluent
                },
                faxElement: function (numberIncr, settings, isFocus) {
                    $('input[name="Faxes[' + numberIncr + '].IsPrimary"]')
                                        .kendoDropDownList({
                                            "dataSource": window.setaJs.modules.common.objectDefault.dataListYesNo,
                                            "dataTextField": "Text",
                                            "dataValueField": "Value"
                                        });
                    $('input[name="Faxes[' + numberIncr + '].FaxMaskId"]')
                                        .kendoComboBox({
                                            placeholder: "Select country",
                                            dataTextField: "CountryName",
                                            dataValueField: "PhoneFormatId",
                                            filter: "contains",
                                            autoBind: true,
                                            suggest: true,
                                            minLength: 1,
                                            change: function (e) {
                                                var selectedIndex = this.select();
                                                if (selectedIndex < 0) {
                                                    this.value(null);
                                                    this.dataSource.filter([]);
                                                    $('input[name="Faxes[' + numberIncr + '].FaxNumber"]').val('');
                                                    $('input[name="Faxes[' + numberIncr + '].FaxNumber"]').mask(' ');
                                                } else {
                                                    $.ajax({
                                                        url: "/Mobile/GetCountryById?countryId=" + this.value(),
                                                        type: 'GET',
                                                        contentType: 'application/json;',
                                                        dataType: 'json',
                                                        cache: false,
                                                        success: function (result) {
                                                            $('input[name="Faxes[' + numberIncr + '].FaxNumber"]').val('');
                                                            $('input[name="Faxes[' + numberIncr + '].FaxNumber"]').mask(result.Mask);
                                                        }
                                                    });
                                                }
                                            },
                                            dataSource: {
                                                type: "json",
                                                serverFiltering: true,
                                                transport: {
                                                    read: {
                                                        url: "/Mobile/GetCountries",
                                                    }
                                                }
                                            }
                                        });
                    //init mask if edit
                    var faxMaskId = $('input[name="Faxes[' + numberIncr + '].FaxMaskId"]').val();
                    if ($.isNumeric(faxMaskId)) {
                        $.ajax({
                            url: "/Mobile/GetCountryById?countryId=" + faxMaskId,
                            type: 'GET',
                            contentType: 'application/json;',
                            dataType: 'json',
                            cache: false,
                            success: function (result) {
                                $('input[name="Faxes[' + numberIncr + '].FaxNumber"]').mask(result.Mask);
                            }
                        });
                    } else {
                        $('input[name="Faxes[' + numberIncr + '].FaxNumber"]').mask("+1(###)###-####");
                        $('input[name="Faxes[' + numberIncr + '].FaxMaskId"]').data("kendoComboBox").value(292);
                    }
                    $('input[name="Faxes[' + numberIncr + '].Type"]')
                        .kendoDropDownList({
                            dataSource: settings.dataView.oFaxType,
                            dataTextField: "Text",
                            dataValueField: "Value"
                        });

                    //Rules validation phone
                    $('input[name="Faxes[' + numberIncr + '].FaxNumber"]').rules("add", {
                        phoneNumber: true
                    });

                    return this; //fluent
                },
                emailElement: function (numberIncr, settings, isFocus) {
                    var t = this;
                    var primary = $('input[name="Emails[' + numberIncr + '].IsPrimary"]')
                                        .kendoDropDownList({
                                            dataSource: window.setaJs.modules.common.objectDefault.dataListYesNo,
                                            dataTextField: "Text",
                                            dataValueField: "Value",
                                            change: function (e) {
                                                var value = this.value();
                                                t.onChangeYesNo(value, "Emails", numberIncr);
                                            }
                                        });
                    $('input[name="Emails[' + numberIncr + '].EmailAddress"]').rules("add", {
                        email: {
                            required: true,
                            email: true
                        }
                    });
                    var hasPrimary = false;
                    //if ($("input.email_primary").length > 1)
                    {
                        for (var i = 0; i < $("input.email_primary").length; i++) {
                            if ($($("input.email_primary").get(i)).val() == 'true') {
                                hasPrimary = true;
                            } else {
                                hasPrimary = false;
                            }
                        }
                    }
                    /*$("input.email_primary").each(function (index, item) {
                        //default the first is always YES
                        if ($("input.email_primary").length > 1) {
                            if ($(item).val() == 'true') {
                                hasPrimary = true;
                            }
                        }
                    });*/
                    if (hasPrimary) {
                        primary.data('kendoDropDownList').select(1);
                    }
                    if (isFocus == undefined || isFocus) if (isFocus == undefined || isFocus) if (isFocus == undefined || isFocus)
                        $('input[name="Emails[' + numberIncr + '].EmailAddress"]').focus();
                    return this; //fluent
                },
                websiteElement: function (numberIncr, settings, isFocus) {
                    $('input[name="Websites[' + numberIncr + '].IsPrimary"]')
                                        .kendoDropDownList({
                                            "dataSource": window.setaJs.modules.common.objectDefault.dataListYesNo,
                                            "dataTextField": "Text",
                                            "dataValueField": "Value"
                                        });
                    $('input[name="Websites[' + numberIncr + '].Url"]').rules("add", {
                        url: {
                            required: true,
                            url: true
                        }
                    });
                    if (isFocus == undefined || isFocus)
                        $('input[name="Websites[' + numberIncr + '].Url"]').focus();
                    return this; //fluent
                },
                saleMetricElement: function (numberIncr, settings, isFocus) {
                    //$('input[name="SaleMetrics[' + numberIncr + '].MonthToDate"]').rules("add", {
                    //    url: {
                    //        required: true
                    //    }
                    //});
                    $('input[name="SaleMetrics[' + numberIncr + '].MonthToDate"]').kendoNumericTextBox({ spinners: false, format: "#", decimals: 0 });
                    $('input[name="SaleMetrics[' + numberIncr + '].YearToDate"]').kendoNumericTextBox({ spinners: false, format: "#", decimals: 0 });
                    $('input[name="SaleMetrics[' + numberIncr + '].PreviousYear"]').kendoNumericTextBox({ spinners: false, format: "#", decimals: 0 });

                    if (isFocus == undefined || isFocus)
                        $('input[name="SaleMetrics[' + numberIncr + '].MonthToDate"]').focus();
                    return this; //fluent
                },
                //AccountRelatedOwners[#= data.numberInc #].AccountInvolvedID
                reletedElement: function (numberIncr, settings, isFocus) {
                    var self = this;
                    var accountID = $("#btn-create-related").attr("data-account-id");
                    $('input[name="AccountRelatedOwners[' + numberIncr + '].AccountRelationshipID"]')
                        .kendoDropDownList({
                            dataSource: settings.dataView.oRelatedType,
                            dataTextField: "Text",
                            dataValueField: "Value",
                            index: 1,
                            select: function (e) {
                                var dataItem = this.dataItem(e.item.index());
                                self.validateAccountRelated($('input[name="AccountRelatedOwners[' + numberIncr + '].AccountInvolvedID"]').data("kendoComboBox").value(), dataItem.Value == "1" ? 2 : 1, accountID, numberIncr);
                            }
                        });

                    $('input[name="AccountRelatedOwners[' + numberIncr + '].AccountInvolvedID"]').kendoComboBox({
                        delay: 300,
                        dataSource: {
                            serverFiltering: true,
                            serverPaging: true,
                            type: "json",
                            transport: {
                                read: {
                                    url: "/Account/PickList_Account?accountId=" + $('input[name="AccountRelatedOwners[' + numberIncr + '].AccountInvolvedID"]').val() + "&currentAccountId=" + accountID,
                                    contentType: "application/json;",
                                    dataType: "json",
                                    data: {
                                        name: function () {
                                            return $('input[name="AccountRelatedOwners[' + numberIncr + '].AccountInvolvedID"]').val();
                                        },
                                        accounts: function () {
                                            var rsList = "";
                                            $('input[unique="RelatedAccount"]').each(function (index, control) {
                                                var acc = $(control);
                                                var value = acc.data("kendoComboBox").value();
                                                if ('AccountRelatedOwners[' + numberIncr + '].AccountInvolvedID' != acc.attr("name")) {
                                                    if (value != '') {
                                                        rsList += value;
                                                        rsList += ",";
                                                    }
                                                }
                                            });
                                            return rsList;
                                        },
                                    }
                                }
                            }
                        },
                        change: function (e) {
                            var selectedIndex = this.select();
                            if (selectedIndex < 0) {
                                this.value(null);
                                this.dataSource.filter([]);
                            }
                            var value = this.value();
                            //if (value == null) this.value(null);
                            var accountRelationship = $('input[name="AccountRelatedOwners[' + numberIncr + '].AccountRelationshipID"]').data("kendoDropDownList").value();
                            self.validateAccountRelated(value, accountRelationship == "1" ? 2 : 1, accountID, numberIncr);
                            //load contact related
                            var contactRelated = $('input[name="AccountRelatedOwners[' + numberIncr + '].ContactInvolvedID"]').data("kendoComboBox");
                            contactRelated.value(null);
                            contactRelated.dataSource.read();

                        },
                        open: function (e) {
                            //var dropdown = $('input[name="AccountRelatedOwners[' + numberIncr + '].AccountInvolvedID"]').data("kendoComboBox");
                            //dropdown.dataSource.read();
                            kendoComboboxFix.scrollFixHeader(e);
                        },
                        headerTemplate: "<div class='header'><span>" + window.setaJs.l("Show_Top_Results") + "</span></div>",
                        dataTextField: "Text",
                        dataValueField: "Value",
                        filter: "startswith",
                        suggest: true,
                        placeholder: window.setaJs.l("Account_SelectAccountRelated")
                    });

                    //Rule validation
                    $('input[name="AccountRelatedOwners[' + numberIncr + '].AccountRelationshipID"]').rules("add", {
                        required: true,
                        onlyPrimary: {
                            el: 'input[name^="RelatedType"]',
                            msg: window.setaJs.l("Account_OnlyParentAccount"),
                            primaryValue: settings.dataView.ParentAccountTypeId
                        }
                    });

                    $('input[name="AccountRelatedOwners[' + numberIncr + '].AccountInvolvedID"]').rules("add", {
                        required: true,
                        messages: {
                            required: "Account Related Name is required"
                        }
                    });


                    $('input[name="AccountRelatedOwners[' + numberIncr + '].ContactInvolvedID"]').kendoComboBox({
                        delay: 300,
                        dataSource: {
                            //cascadeFrom: 'input[name="AccountRelatedOwners[' + numberIncr + '].AccountInvolvedID"]',
                            serverFiltering: false,
                            serverPaging: false,
                            type: "json",
                            transport: {
                                read: {
                                    url: "/Account/GetContactRelated",
                                    contentType: "application/json;",
                                    dataType: "json",
                                    data: {
                                        accountId: function () {
                                            return $('input[name="AccountRelatedOwners[' + numberIncr + '].AccountInvolvedID"]').val();
                                        }
                                    }
                                }
                            }
                        },
                        open: kendoComboboxFix.scrollFixHeader,
                        change: setaJs.kendoComboBoxOnChange,
                        headerTemplate: "<div class='header'><span>" + window.setaJs.l("Show_Top_Results") + "</span></div>",
                        dataTextField: "Name",
                        dataValueField: "ContactId",
                        filter: "startswith",
                        suggest: true,
                        placeholder: window.setaJs.l("Account_SelectPrimaryContactRelated")
                    });

                    var relateTime = $('input[name="AccountRelatedOwners[' + numberIncr + '].RelatedTime"]');
                    var time = relateTime.val();
                    relateTime.kendoDateTimePicker(
                    {
                        format: "MM/dd/yyyy hh:mm tt",
                        value: time.length == 0 ? null : new Date(time)//,
                    });

                    relateTime.rules("add", {
                        required: false,
                        datetime: true
                    });

                    relateTime.on("blur", function (e) {
                        var $this = $(this);
                        var error = $("span[data-valmsg-for='AccountRelatedOwners[" + numberIncr + "].RelatedTime']");
                        if ($this.val() != '' && (!setaJs.ValidateDateTime($this.val()))) {
                            relateTime.addClass('input-validation-error');
                            error.text(window.setaJs.l("Message_DateTimeInvalid"));

                            error.show();
                        } else {
                            relateTime.removeClass('input-validation-error');
                            error.hide();
                        }
                    });
                    return this; //fluent
                },
                validateAccountRelated: function (accountId, accountRelationshipID, accountOwnerId, numberIncr) {
                    //data-valmsg-for
                    var input = $("input[name='AccountRelatedOwners[" + numberIncr + "].AccountInvolvedID_input']");
                    var error = $("span[data-valmsg-for='AccountRelatedOwners[" + numberIncr + "].AccountInvolvedID']");
                    if (accountId != null && accountId != "") {
                        $.ajax({
                            url: "/Account/CheckAccountRelatedHasParent",
                            type: "GET",
                            dataType: "json",
                            contentType: "application/json",
                            data: {
                                accountId: accountId
                                , accountRelationshipID: accountRelationshipID
                                , accountOwnerId: accountOwnerId
                            },
                            success: function (result) {
                                //This is the subsidiary of another account.

                                if (result === false) {
                                    input.closest('span').removeClass("combobox-validation-error");
                                    error.text("");
                                } else {
                                    input.closest('span').addClass("combobox-validation-error");
                                    error.text(window.setaJs.l("Account_SubsidiaryOfOther"));
                                }
                            }
                        });
                    }
                    else {
                        input.closest('span').removeClass("combobox-validation-error");
                        error.text("");
                        //error.text(window.setaJs.l("Text_Item_Requried").format(setaJs.l("RelatedAccounts_AccountId")));

                    }
                },
                assistantsElement: function (numberIncr, settings, isFocus) {
                    var t = this;
                    $('input[name="Assistants[' + numberIncr + '].IsPrimary"]')
                                        .kendoDropDownList({
                                            "dataSource": window.setaJs.modules.common.objectDefault.dataListYesNo,
                                            "dataTextField": "Text",
                                            "dataValueField": "Value",
                                            change: function (e) {
                                                var value = this.value();
                                                t.onChangeYesNo(value, "Assistants", numberIncr);
                                            }
                                        });

                    $('input[name="Assistants[' + numberIncr + '].IsPrimary"]').rules("add", {
                        required: true,
                        onlyPrimary: {
                            el: 'input[name^="AssistantPrimary"]',
                            msg: window.setaJs.l("Common_OnlyPrimaryValue").format("assistant"),
                            primaryValue: 1
                        }
                    });
                    $('input[name="Assistants[' + numberIncr + '].PhoneMaskId"]')
                                        .kendoComboBox({
                                            placeholder: "Select country",
                                            dataTextField: "CountryName",
                                            dataValueField: "PhoneFormatId",
                                            filter: "contains",
                                            autoBind: true,
                                            suggest: true,
                                            minLength: 1,
                                            change: function (e) {
                                                var selectedIndex = this.select();
                                                if (selectedIndex < 0) {
                                                    this.value(null);
                                                    this.dataSource.filter([]);
                                                    $('input[name="Assistants[' + numberIncr + '].AssitantPhone"]').val('');
                                                    $('input[name="Assistants[' + numberIncr + '].AssitantPhone"]').mask(' ');
                                                } else {
                                                    $.ajax({
                                                        url: "/Mobile/GetCountryById?countryId=" + this.value(),
                                                        type: 'GET',
                                                        contentType: 'application/json;',
                                                        dataType: 'json',
                                                        cache: false,
                                                        success: function (result) {
                                                            $('input[name="Assistants[' + numberIncr + '].AssitantPhone"]').val('');
                                                            $('input[name="Assistants[' + numberIncr + '].AssitantPhone"]').mask(result.Mask);
                                                        }
                                                    });
                                                }
                                            },
                                            dataSource: {
                                                type: "json",
                                                serverFiltering: true,
                                                transport: {
                                                    read: {
                                                        url: "/Mobile/GetCountries",
                                                    }
                                                }
                                            }
                                        });
                    //init mask if edit
                    var phoneMaskId = $('input[name="Assistants[' + numberIncr + '].PhoneMaskId"]').val();
                    if ($.isNumeric(phoneMaskId)) {
                        $.ajax({
                            url: "/Mobile/GetCountryById?countryId=" + phoneMaskId,
                            type: 'GET',
                            contentType: 'application/json;',
                            dataType: 'json',
                            cache: false,
                            success: function (result) {
                                $('input[name="Assistants[' + numberIncr + '].AssitantPhone"]').mask(result.Mask);
                            }
                        });
                    } else {
                        $('input[name="Assistants[' + numberIncr + '].AssitantPhone"]').mask("+1(###)###-####");
                        $('input[name="Assistants[' + numberIncr + '].PhoneMaskId"]').data("kendoComboBox").value(292);
                    }
                    var focusControl = 'Assistants[' + numberIncr + '].AssitantName';
                    $("input[name='" + focusControl + "']").kendoComboBox({
                        placeholder: "Select assitant name",
                        dataTextField: "text",
                        dataValueField: "text",
                        filter: "startswith",
                        suggest: true,
                        select: function (e) {
                            var dataItem = this.dataItem(e.item.index());
                            var refContactIdCtrl = $("input[name='Assistants[" + numberIncr + "].RefContactID']");
                            refContactIdCtrl.val(dataItem.id);

                            if (dataItem.id != '' && parseInt(dataItem.id) > 0) {
                                $.getJSON("/Contact/GetInfoMobileAndEmail/" + dataItem.id, function (data) {
                                    if (data != null) {
                                        $('input[name="Assistants[' + numberIncr + '].AssitantPhone"]').val(data.Mobile);
                                        $('input[name="Assistants[' + numberIncr + '].Email"]').val(data.Email);
                                    } else {
                                        $('input[name="Assistants[' + numberIncr + '].AssitantPhone"]').val('');
                                        $('input[name="Assistants[' + numberIncr + '].Email"]').val('');
                                    }
                                }, "json").error(function (error) {
                                    //console.log("error: " + JSON.stringify(error)); 
                                });
                            }
                        },
                        autoBind: true,
                        minLength: 1,
                        dataSource: {
                            serverFiltering: true,
                            transport: {
                                read: {
                                    url: "/Contact/PickList_ContactByAccountNew",
                                    data: function () {
                                        return {
                                            //name: $("input[name='" + focusControl + "']").data("kendoComboBox").input.val(),
                                            contactId: setaJs.oGlobal.id > 0 ? setaJs.oGlobal.id : 0,
                                            accountId: $('#Account_ContactID').val()
                                        };
                                    },
                                }
                            }
                        },
                        open: kendoComboboxFix.scrollFixHeader,
                        headerTemplate: "<div class='header'><span>" + window.setaJs.l("Show_Top_Results") + "</span></div>"
                    });
                    return this; //fluent
                },
                managersElement: function (numberIncr, settings, isFocus) {
                    var t = this;
                    $('input[name="Managers[' + numberIncr + '].IsPrimary"]')
                                        .kendoDropDownList({
                                            "dataSource": window.setaJs.modules.common.objectDefault.dataListYesNo,
                                            "dataTextField": "Text",
                                            "dataValueField": "Value",
                                            change: function (e) {
                                                var value = this.value();
                                                t.onChangeYesNo(value, "Managers", numberIncr);
                                            }
                                        });

                    $('input[name="Managers[' + numberIncr + '].IsPrimary"]').rules("add", {
                        required: true,
                        onlyPrimary: {
                            el: 'input[name^="ManagerPrimary"]',
                            msg: window.setaJs.l("Common_OnlyPrimaryValue").format("manager"),
                            primaryValue: 1
                        }
                    });
                    $('input[name="Managers[' + numberIncr + '].PhoneMaskId"]')
                                        .kendoComboBox({
                                            placeholder: "Select country",
                                            dataTextField: "CountryName",
                                            dataValueField: "PhoneFormatId",
                                            filter: "contains",
                                            autoBind: true,
                                            suggest: true,
                                            minLength: 1,
                                            change: function (e) {
                                                var selectedIndex = this.select();
                                                if (selectedIndex < 0) {
                                                    this.value(null);
                                                    this.dataSource.filter([]);
                                                    $('input[name="Managers[' + numberIncr + '].AssitantPhone"]').val('');
                                                    $('input[name="Managers[' + numberIncr + '].AssitantPhone"]').mask(' ');
                                                } else {
                                                    $.ajax({
                                                        url: "/Mobile/GetCountryById?countryId=" + this.value(),
                                                        type: 'GET',
                                                        contentType: 'application/json;',
                                                        dataType: 'json',
                                                        cache: false,
                                                        success: function (result) {
                                                            $('input[name="Managers[' + numberIncr + '].AssitantPhone"]').val('');
                                                            $('input[name="Managers[' + numberIncr + '].AssitantPhone"]').mask(result.Mask);
                                                        }
                                                    });
                                                }
                                            },
                                            dataSource: {
                                                type: "json",
                                                serverFiltering: true,
                                                transport: {
                                                    read: {
                                                        url: "/Mobile/GetCountries",
                                                    }
                                                }
                                            }
                                        });
                    //init mask if edit
                    var phoneMaskId = $('input[name="Managers[' + numberIncr + '].PhoneMaskId"]').val();
                    if ($.isNumeric(phoneMaskId)) {
                        $.ajax({
                            url: "/Mobile/GetCountryById?countryId=" + phoneMaskId,
                            type: 'GET',
                            contentType: 'application/json;',
                            dataType: 'json',
                            cache: false,
                            success: function (result) {
                                $('input[name="Managers[' + numberIncr + '].AssitantPhone"]').mask(result.Mask);
                            }
                        });
                    } else {
                        $('input[name="Managers[' + numberIncr + '].AssitantPhone"]').mask("+1(###)###-####");
                        $('input[name="Managers[' + numberIncr + '].PhoneMaskId"]').data("kendoComboBox").value(292);
                    }
                    var focusControl = 'Managers[' + numberIncr + '].AssitantName';
                    $("input[name='" + focusControl + "']").kendoComboBox({
                        placeholder: "Select manager name",
                        dataTextField: "text",
                        dataValueField: "text",
                        filter: "startswith",
                        suggest: true,
                        select: function (e) {
                            var dataItem = this.dataItem(e.item.index());
                            var refContactIdCtrl = $("input[name='Managers[" + numberIncr + "].RefContactID']");
                            refContactIdCtrl.val(dataItem.id);

                            if (dataItem.id != '' && parseInt(dataItem.id) > 0) {
                                $.getJSON("/Contact/GetInfoMobileAndEmail/" + dataItem.id, function (data) {
                                    if (data != null) {
                                        $('input[name="Managers[' + numberIncr + '].AssitantPhone"]').val(data.Mobile);
                                        $('input[name="Managers[' + numberIncr + '].Email"]').val(data.Email);
                                    } else {
                                        $('input[name="Managers[' + numberIncr + '].AssitantPhone"]').val('');
                                        $('input[name="Managers[' + numberIncr + '].Email"]').val('');
                                    }
                                }, "json").error(function (error) {
                                    //console.log("error: " + JSON.stringify(error)); 
                                });
                            }
                        },
                        autoBind: true,
                        minLength: 1,
                        dataSource: {
                            serverFiltering: true,
                            transport: {
                                read: {
                                    url: "/Contact/PickList_ContactByAccountNew",
                                    data: function () {
                                        return {
                                            //name: $("input[name='" + focusControl + "']").data("kendoComboBox").input.val(),
                                            contactId: setaJs.oGlobal.id > 0 ? setaJs.oGlobal.id : 0,
                                            accountId: $('#Account_ContactID').val()
                                        };
                                    },
                                }
                            }
                        },
                        open: kendoComboboxFix.scrollFixHeader,
                        headerTemplate: "<div class='header'><span>" + window.setaJs.l("Show_Top_Results") + "</span></div>"
                    });
                    return this; //fluent
                },
                reminderElement: function (numberIncr, settings, isFocus) {
                    $('input[name="Reminder[' + numberIncr + ']"]').kendoNumericTextBox({
                        format: "n0",
                        min: 0
                    });
                    $('input[name="UnitReminder[' + numberIncr + ']"]')
                                       .kendoDropDownList({
                                           dataTextField: "text",
                                           dataValueField: "value",
                                           dataSource: [
                                               { text: "Minutes", value: "4" },
                                               { text: "Hours", value: "3" },
                                               { text: "Days", value: "1" },
                                               { text: "Weeks", value: "2" }
                                           ],
                                           filter: "contains",
                                           suggest: true,
                                           index: 0
                                       });
                },
                attendeesElement: function (numberIncr, settings, isFocus) {

                    $('input[name="Attendees[' + numberIncr + '].UserID"]')
                                      .kendoDropDownList({
                                          dataSource: null,
                                          dataTextField: "Text",
                                          dataValueField: "Value"
                                      });
                    return this; //fluent
                },
                goalElement: function (numberIncr, settings, isFocus) {
                    $('input[name="Goals[' + numberIncr + '].FromDate"]').kendoDatePicker(
                           {
                               change: function () {
                                   var value = this.value();
                                   if (value != null) {
                                       $("#frm-create-salegoal").find("span[data-valmsg-for='" + "Goals[" + numberIncr + "].FromDate" + "']").empty();
                                       $("#frm-create-salegoal").find("[name='" + "Goals[" + numberIncr + "].FromDate" + "'], [name='" + "Goals[" + numberIncr + "].FromDate" + "_input']").removeClass("input-validation-error");
                                       $("#create-goal").find("span.error, span.field-validation-valid").empty();
                                       $("#create-goal").find("input, textarea, span").removeClass("input-validation-error");
                                   }
                               },
                               format: "MM/dd/yyyy",
                               parseFormats: ["MM/dd/yyyy"]
                           });
                    $('input[name="Goals[' + numberIncr + '].FromDate"]').data("kendoDatePicker").value($('input[name="Goals[' + numberIncr + '].FromDate"]').val().trim());
                    $('input[name="Goals[' + numberIncr + '].ToDate"]').kendoDatePicker(
                           {
                               change: function () {
                                   var value = this.value();
                                   if (value != null) {
                                       $("#frm-create-salegoal").find("span[data-valmsg-for='" + "Goals[" + numberIncr + "].ToDate" + "']").empty();
                                       $("#frm-create-salegoal").find("[name='" + "Goals[" + numberIncr + "].ToDate" + "'], [name='" + "Goals[" + numberIncr + "].ToDate" + "_input']").removeClass("input-validation-error");
                                       $("#create-goal").find("span.error, span.field-validation-valid").empty();
                                       $("#create-goal").find("input, textarea, span").removeClass("input-validation-error");
                                   }
                               },
                               format: "MM/dd/yyyy",
                               parseFormats: ["MM/dd/yyyy"]
                           });
                    $('input[name="Goals[' + numberIncr + '].ToDate"]').data("kendoDatePicker").value($('input[name="Goals[' + numberIncr + '].ToDate"]').val().trim());
                    $('input[name="Goals[' + numberIncr + '].Amount"]').kendoNumericTextBox({
                        format: "n2",
                        spinners: false,
                        change: function () {
                            var value = this.value();
                            if (value == null) {
                                this.value(0);
                            }
                            else {
                                if (value < this.min() || value >= this.max()) {
                                    this.value(this.oldvalue);
                                }
                            }
                        }
                    });
                    // set old value 
                    $("[data-role='numerictextbox']").on("focusin", function (e) {
                        var $this = $(this);
                        var me = $this.data("kendoNumericTextBox");
                        me.oldvalue = $this.val();
                    });
                    $("#frm-create-salegoal").find("span[data-valmsg-for='Goals_Error']").empty();
                    return this; //fluent
                },
                coveredItemElement: function (numberIncr, settings, isFocus) {
                    $('input[name="CoveredItemsList[' + numberIncr + '].ItemID"]')
                                        .kendoComboBox({
                                            placeholder: "Select Item",
                                            dataTextField: "ItemNumber",
                                            dataValueField: "ItemId",
                                            filter: "contains",
                                            autoBind: true,
                                            minLength: 1,
                                            dataSource: {
                                                type: "json",
                                                serverFiltering: true,
                                                transport: {
                                                    read: {
                                                        url: "/Item/GetItems",
                                                    }
                                                }
                                            },
                                            change: function (e) {
                                                setaJs.modules.common.method.checkDuplicateLists('.row-covered-item', 'CoveredItemsList[{0}].ItemID', 'TEXT_ITEM_NUMBER');
                                            }
                                        });
                    return this; //fluent
                },
                coveredItemOwnedElement: function (numberIncr, settings, isFocus) {
                    $('input[name="CoveredItemsOwnedList[' + numberIncr + '].ItemID"]')
                                        .kendoComboBox({
                                            placeholder: "Select Item",
                                            dataTextField: "ItemNumber",
                                            dataValueField: "ItemsOwnedID",
                                            filter: "contains",
                                            autoBind: true,
                                            minLength: 1,
                                            dataSource: {
                                                type: "json",
                                                serverFiltering: true,
                                                transport: {
                                                    read: {
                                                        url: "/Item/GetItemsOwnedByAccountId",
                                                        data: {
                                                            accountId: function () {
                                                                return $("#AccountID").data("kendoComboBox").value();
                                                            }
                                                        }
                                                    }
                                                }
                                            },
                                            change: function (e) {
                                                setaJs.modules.common.method.checkDuplicateLists('.row-covered-item-owned', 'CoveredItemsOwnedList[{0}].ItemID', 'TEXT_ITEM_NUMBER');
                                            },
                                            template: "#: data.ItemsOwnedID #. #: data.ItemNumber #"
                                        });
                    return this; //fluent
                },
                coveredPartElement: function (numberIncr, settings, isFocus) {
                    $('input[name="CoveredPartsList[' + numberIncr + '].ItemID"]')
                                        .kendoComboBox({
                                            placeholder: "Select Item",
                                            dataTextField: "ItemNumber",
                                            dataValueField: "ItemId",
                                            filter: "contains",
                                            autoBind: true,
                                            dataSource: {
                                                type: "json",
                                                serverFiltering: true,
                                                transport: {
                                                    read: {
                                                        url: "/Item/GetItems"
                                                    }
                                                }
                                            },
                                            change: function (e) {
                                                setaJs.modules.common.method.checkDuplicateLists('.row-covered-part', 'CoveredPartsList[{0}].ItemID', 'TEXT_COVERED_PARTS');
                                            }
                                        });
                    return this; //fluent
                },
                coveredLaborElement: function (numberIncr, settings, isFocus) {
                    $('input[name="CoveredLaborList[' + numberIncr + '].JobType"]')
                                        .kendoComboBox({
                                            placeholder: "Select Job Type",
                                            dataTextField: "Text",
                                            dataValueField: "Value",
                                            filter: "contains",
                                            autoBind: true,
                                            minLength: 1,
                                            dataSource: {
                                                type: "json",
                                                serverFiltering: true,
                                                transport: {
                                                    read: {
                                                        url: "/ServiceContract/GetJobType"
                                                    }
                                                }
                                            },
                                            change: function (e) {
                                                setaJs.modules.common.method.checkDuplicateLists('.row-covered-labor', 'CoveredLaborList[{0}].JobType', 'TEXT_COVERED_LABOR');
                                            }
                                        });
                    return this; //fluent
                },
                coveredExpensesElement: function (numberIncr, settings, isFocus) {
                    $('input[name="CoveredExpensesList[' + numberIncr + '].ExpenseCategory"]')
                                        .kendoComboBox({
                                            placeholder: "Select Expense Type",
                                            dataTextField: "Text",
                                            dataValueField: "Value",
                                            filter: "contains",
                                            autoBind: true,
                                            minLength: 1,
                                            dataSource: {
                                                type: "json",
                                                serverFiltering: true,
                                                transport: {
                                                    read: {
                                                        url: "/ServiceContract/GetExpenseCategory"
                                                    }
                                                }
                                            },
                                            change: function (e) {
                                                setaJs.modules.common.method.checkDuplicateLists('.row-covered-expense', 'CoveredExpensesList[{0}].ExpenseCategory', 'ExpenseCategory_Name');
                                            }
                                        });
                    return this; //fluent
                },
                coveredAuthorizedContactsElement: function (numberIncr, settings, isFocus) {
                    $('input[name="AuthorizedContacts[' + numberIncr + '].ContactID"]')
                                        .kendoComboBox({
                                            placeholder: "Select Contact",
                                            dataTextField: "Text",
                                            dataValueField: "Value",
                                            filter: "contains",
                                            autoBind: true,
                                            minLength: 1,
                                            dataSource: {
                                                type: "json",
                                                serverFiltering: true,
                                                transport: {
                                                    read: {
                                                        url: "/Account/GetAvailableContacts",
                                                        data: {
                                                            accountId: function () {
                                                                return $("#AccountID").data("kendoComboBox").value();
                                                            },
                                                            needBlankOption: false
                                                        }
                                                    }
                                                }
                                            },
                                            change: function (e) {
                                                //setaJs.modules.common.method.checkDuplicateAuthorizedContacts();
                                                setaJs.modules.common.method.checkDuplicateLists('.row-authorized-contacts', 'AuthorizedContacts[{0}].ContactID', 'TEXT_CONTACT');
                                            }
                                        });
                    return this; //fluent
                },
                selectedLists: function (selector, template) {
                    var list = [];
                    $(selector).each(function () {
                        var currentNumber = $(this).attr('data-number');
                        if (currentNumber != 'undefined' || currentNumber != null) {
                            var id = parseInt($('input[name="' + template.format(currentNumber) + '"]').val()) || 0;
                            if (id > 0) {
                                list.push(id);
                            }
                        }
                    });
                    return list;
                },
                checkDuplicateLists: function (selector, template, label) {
                    var valid = true;
                    var lists = setaJs.modules.common.method.selectedLists(selector, template);
                    var sortedList = lists.sort();
                    var duplicates = [];
                    for (var i = 0; i < lists.length - 1; i++) {
                        if (sortedList[i + 1] == sortedList[i]) {
                            duplicates.push(sortedList[i]);
                        }
                    }
                    $(selector).each(function () {
                        var currentNumber = $(this).attr('data-number');
                        $('span[data-valmsg-for="' + template.format(currentNumber) + '"]').removeClass("error").html("");
                        $('input[name="' + template.format(currentNumber) + '_input"]').closest("span").removeClass("input-validation-error");
                    });
                    if (duplicates.length > 0) {
                        valid = false;
                        $(selector).each(function () {
                            var currentNumber = $(this).attr('data-number');
                            var contactId = $('input[name="' + template.format(currentNumber) + '"]').data("kendoComboBox").value();
                            if (duplicates.indexOf(parseInt(contactId)) >= 0) {
                                $('input[name="' + template.format(currentNumber) + '_input"]').closest("span").addClass("input-validation-error");
                                $('span[data-valmsg-for="' + template.format(currentNumber) + '"]').addClass("error");
                                $('span[data-valmsg-for="' + template.format(currentNumber) + '"]').html(setaJs.l('Validate_Duplicate').format(setaJs.l(label)));
                            } else {
                                $('input[name="' + template.format(currentNumber) + '_input"]').closest("span").removeClass("input-validation-error");
                                $('span[data-valmsg-for="' + template.format(currentNumber) + '"]').removeClass("error");
                                $('span[data-valmsg-for="' + template.format(currentNumber) + '"]').html("");
                            }
                        });
                    }
                    return valid;
                },
                territoryBoundaryElement: function (numberIncr, settings, isFocus) {
                    var autuBind = !settings.isAddNew;
                    $('input[name="TerritoryBoundaries[' + numberIncr + '].Country"]')
                                       .kendoComboBox({
                                           dataSource: settings.dataView.lstCountry,
                                           dataTextField: "CountryName",
                                           dataValueField: "CountryID",
                                           //value: "3",
                                           select: function (e) {
                                               $('input[name="TerritoryBoundaries[' + numberIncr + '].Scope"]').data("kendoComboBox").select(0);
                                               $('input[name="TerritoryBoundaries[' + numberIncr + '].State"]').closest('.form-group').addClass('hidden');
                                               $('input[name="TerritoryBoundaries[' + numberIncr + '].County"]').closest('.form-group').addClass('hidden');
                                               $('input[name="TerritoryBoundaries[' + numberIncr + '].PostalCodeFrom"]').closest('.form-group').addClass('hidden');
                                           }
                                       });
                    $('input[name="TerritoryBoundaries[' + numberIncr + '].Scope"]')
                                       .kendoComboBox({
                                           dataSource: settings.dataView.lstScope,
                                           dataTextField: "Description",
                                           dataValueField: "Value",
                                           //value: "1",
                                           select: function (e) {
                                               var selectedOne = this.dataItem(e.item.index());
                                               if (selectedOne != null) {
                                                   switch (parseInt(selectedOne.Value)) {
                                                       case 1:
                                                           $('input[name="TerritoryBoundaries[' + numberIncr + '].State"]').closest('.form-group').addClass('hidden');
                                                           $('input[name="TerritoryBoundaries[' + numberIncr + '].County"]').closest('.form-group').addClass('hidden');
                                                           $('input[name="TerritoryBoundaries[' + numberIncr + '].PostalCodeFrom"]').closest('.form-group').addClass('hidden');
                                                           $('input[name="TerritoryBoundaries[' + numberIncr + '].State"]').data("kendoComboBox").value("");
                                                           $('input[name="TerritoryBoundaries[' + numberIncr + '].County"]').data("kendoComboBox").value("");
                                                           $('input[name="TerritoryBoundaries[' + numberIncr + '].PostalCodeFrom"]').val("");
                                                           $('input[name="TerritoryBoundaries[' + numberIncr + '].PostalCodeTo"]').val("");
                                                           break;
                                                       case 2:
                                                           $('input[name="TerritoryBoundaries[' + numberIncr + '].State"]').closest('.form-group').removeClass('hidden');
                                                           $('input[name="TerritoryBoundaries[' + numberIncr + '].County"]').closest('.form-group').addClass('hidden');
                                                           $('input[name="TerritoryBoundaries[' + numberIncr + '].PostalCodeFrom"]').closest('.form-group').addClass('hidden');
                                                           var state = $('input[name="TerritoryBoundaries[' + numberIncr + '].State"]').data("kendoComboBox");
                                                           state.value("");
                                                           state.dataSource.read();
                                                           $('input[name="TerritoryBoundaries[' + numberIncr + '].County"]').data("kendoComboBox").value("");
                                                           $('input[name="TerritoryBoundaries[' + numberIncr + '].PostalCodeFrom"]').val("");
                                                           $('input[name="TerritoryBoundaries[' + numberIncr + '].PostalCodeTo"]').val("");
                                                           break;
                                                       case 3:
                                                           $('input[name="TerritoryBoundaries[' + numberIncr + '].State"]').closest('.form-group').addClass('hidden');
                                                           $('input[name="TerritoryBoundaries[' + numberIncr + '].County"]').closest('.form-group').removeClass('hidden');
                                                           $('input[name="TerritoryBoundaries[' + numberIncr + '].PostalCodeFrom"]').closest('.form-group').addClass('hidden');
                                                           var county = $('input[name="TerritoryBoundaries[' + numberIncr + '].County"]').data("kendoComboBox");
                                                           county.value("");
                                                           county.dataSource.read();
                                                           $('input[name="TerritoryBoundaries[' + numberIncr + '].County"]').data("kendoComboBox").value("");
                                                           $('input[name="TerritoryBoundaries[' + numberIncr + '].State"]').data("kendoComboBox").value("");
                                                           $('input[name="TerritoryBoundaries[' + numberIncr + '].PostalCodeFrom"]').val("");
                                                           $('input[name="TerritoryBoundaries[' + numberIncr + '].PostalCodeTo"]').val("");
                                                           break;
                                                       case 4:
                                                           $('input[name="TerritoryBoundaries[' + numberIncr + '].State"]').closest('.form-group').addClass('hidden');
                                                           $('input[name="TerritoryBoundaries[' + numberIncr + '].County"]').closest('.form-group').addClass('hidden');
                                                           $('input[name="TerritoryBoundaries[' + numberIncr + '].PostalCodeFrom"]').closest('.form-group').removeClass('hidden');
                                                           $('input[name="TerritoryBoundaries[' + numberIncr + '].State"]').data("kendoComboBox").value("");
                                                           $('input[name="TerritoryBoundaries[' + numberIncr + '].County"]').data("kendoComboBox").value("");
                                                           $('input[name="TerritoryBoundaries[' + numberIncr + '].PostalCodeFrom"]').val("");
                                                           $('input[name="TerritoryBoundaries[' + numberIncr + '].PostalCodeTo"]').val("");
                                                           break;
                                                   }
                                               }
                                           }
                                       });

                    $('input[name="TerritoryBoundaries[' + numberIncr + '].State"]')
                                       .kendoComboBox({
                                           autoBind: autuBind,
                                           dataSource: new kendo.data.DataSource({
                                               serverFiltering: true,
                                               transport: {
                                                   read: {
                                                       url: '/Address/GetState',
                                                       type: 'GET',
                                                       dataType: 'json'
                                                   },
                                                   parameterMap: function (e) {
                                                       var ct = $('input[name="TerritoryBoundaries[' + numberIncr + '].Country"]').data("kendoComboBox");
                                                       return {
                                                           state: "",//$('input[name="TerritoryBoundaries[' + numberIncr + '].State"]').val(),
                                                           country: ct.dataItem(ct.selectedIndex).CountryName
                                                       };
                                                   }
                                               }
                                           }),
                                           dataTextField: "StateName",
                                           dataValueField: "StateID",
                                           //value: $('input[name="TerritoryBoundaries[' + numberIncr + '].State"]').val()
                                       });
                    $('input[name="TerritoryBoundaries[' + numberIncr + '].County"]')
                                       .kendoComboBox({
                                           autoBind: autuBind,
                                           dataSource: new kendo.data.DataSource({
                                               serverFiltering: true,
                                               transport: {
                                                   read: {
                                                       url: '/Address/GetCounty',
                                                       type: 'GET',
                                                       dataType: 'json'
                                                   },
                                                   parameterMap: function (e) {
                                                       var ct = $('input[name="TerritoryBoundaries[' + numberIncr + '].Country"]').data("kendoComboBox");
                                                       return {
                                                           county: "",// $('input[name="TerritoryBoundaries[' + numberIncr + '].County"]').val(),
                                                           country: ct.dataItem(ct.selectedIndex).CountryName
                                                       };
                                                   }
                                               }
                                           }),
                                           dataTextField: "CountyName",
                                           dataValueField: "CountyID",
                                           //value: $('input[name="TerritoryBoundaries[' + numberIncr + '].County"]').val()
                                       });

                    $('input[name="TerritoryBoundaries[' + numberIncr + '].PostalCodeFrom"]').kendoAutoComplete({
                        minLength: 3,
                        filter: "contains",
                        dataTextField: "PostalCode",
                        dataSource: new kendo.data.DataSource({
                            serverFiltering: true,
                            transport: {
                                read: {
                                    url: '/Address/GetPostalCode',
                                    type: 'GET',
                                    dataType: 'json'
                                },
                                parameterMap: function (e) {
                                    return {
                                        zipCode: $('input[name="TerritoryBoundaries[' + numberIncr + '].PostalCodeFrom"]').val(),
                                        country: $('input[name="TerritoryBoundaries[' + numberIncr + '].Country"]').data("kendoComboBox").text()
                                    };
                                }
                            },
                            serverAggregates: true
                        }),
                        template: kendo.template("#:City#, #:State# (#:PostalCode#)"),
                        open: function (e) {
                            $(e.sender.ul).niceScroll();
                        },
                        select: function (e) {
                        }
                    });
                    $('input[name="TerritoryBoundaries[' + numberIncr + '].PostalCodeTo"]').kendoAutoComplete({
                        minLength: 3,
                        filter: "contains",
                        dataTextField: "PostalCode",
                        dataSource: new kendo.data.DataSource({
                            serverFiltering: true,
                            transport: {
                                read: {
                                    url: '/Address/GetPostalCode',
                                    type: 'GET',
                                    dataType: 'json'
                                },
                                parameterMap: function (e) {
                                    return {
                                        zipCode: $('input[name="TerritoryBoundaries[' + numberIncr + '].PostalCodeTo"]').val(),
                                        country: $('input[name="TerritoryBoundaries[' + numberIncr + '].Country"]').data("kendoComboBox").text()
                                    };
                                }
                            },
                            serverAggregates: true
                        }),
                        template: kendo.template("#:City#, #:State# (#:PostalCode#)"),
                        open: function (e) {
                            $(e.sender.ul).niceScroll();
                        },
                        select: function (e) {
                        }
                    });
                    delete settings.isAddNew;
                    switch (parseInt($('input[name="TerritoryBoundaries[' + numberIncr + '].Scope"]').val())) {
                        case 1:
                            $('input[name="TerritoryBoundaries[' + numberIncr + '].State"]').closest('.form-group').addClass('hidden');
                            $('input[name="TerritoryBoundaries[' + numberIncr + '].County"]').closest('.form-group').addClass('hidden');
                            $('input[name="TerritoryBoundaries[' + numberIncr + '].PostalCodeFrom"]').closest('.form-group').addClass('hidden');
                            break;
                        case 2:
                            $('input[name="TerritoryBoundaries[' + numberIncr + '].State"]').closest('.form-group').removeClass('hidden');
                            $('input[name="TerritoryBoundaries[' + numberIncr + '].County"]').closest('.form-group').addClass('hidden');
                            $('input[name="TerritoryBoundaries[' + numberIncr + '].PostalCodeFrom"]').closest('.form-group').addClass('hidden');
                            break;
                        case 3:
                            $('input[name="TerritoryBoundaries[' + numberIncr + '].State"]').closest('.form-group').addClass('hidden');
                            $('input[name="TerritoryBoundaries[' + numberIncr + '].County"]').closest('.form-group').removeClass('hidden');
                            $('input[name="TerritoryBoundaries[' + numberIncr + '].PostalCodeFrom"]').closest('.form-group').addClass('hidden');
                            break;
                        case 4:
                            $('input[name="TerritoryBoundaries[' + numberIncr + '].State"]').closest('.form-group').addClass('hidden');
                            $('input[name="TerritoryBoundaries[' + numberIncr + '].County"]').closest('.form-group').addClass('hidden');
                            $('input[name="TerritoryBoundaries[' + numberIncr + '].PostalCodeFrom"]').closest('.form-group').removeClass('hidden');
                            break;
                    }
                    return this; //fluent
                },
                itemcomponentElement: function (numberIncr, settings, isFocus) {
                    var self = this;
                    var ItemId = $("#btn-create-itemcomponent").attr("data-itemcomponent-id");
                    var dataListCashPercent = [{
                        "Text": "Cash",
                        "Value": "0",
                        "Selected": true
                    }, {
                        "Text": "Percent",
                        "Value": "1",
                        "Selected": false
                    }];
                    $('input[name="KitItems[' + numberIncr + '].DiscountMethod"]')
                        .kendoDropDownList({
                            dataSource: dataListCashPercent,
                            dataTextField: "Text",
                            dataValueField: "Value",
                            index: 1,
                            select: function (e) {
                                var dataItem = this.dataItem(e.item.index());
                                self.calcItimeComponent(numberIncr, dataItem.Value);
                            }
                        });

                    $('input[name="KitItems[' + numberIncr + '].ItemId"]').kendoComboBox({
                        dataSource: {
                            serverFiltering: true,
                            serverPaging: true,
                            type: "json",
                            transport: {
                                read: {
                                    url: "/Item/GetItems",
                                    contentType: "application/json;",
                                    dataType: "json",
                                    data: {
                                        name: function () {
                                            return $('input[name="KitItems[' + numberIncr + '].ItemId"]').val();
                                        }
                                    }
                                }
                            }
                        }
                        , select: function (e) {
                            var dataItem = this.dataItem(e.item.index());
                            var ItemId = $("#btn-create-itemcomponent").attr("data-itemcomponent-id");
                            $('input[name="KitItems[' + numberIncr + '].UnitPrice"]').data("kendoNumericTextBox").value(dataItem.UnitPrice);
                            self.calcItimeComponent(numberIncr);
                        }
                        , open: kendoComboboxFix.scrollFixHeader,
                        headerTemplate: "<div class='header'><span>" + window.setaJs.l("Show_Top_Results") + "</span></div>",
                        dataTextField: "ItemNumber",
                        dataValueField: "ItemId",
                        filter: "startswith",
                        suggest: true,
                        placeholder: "Select Item Number",// window.setaJs.l("Account_SelectAccountRelated")
                    });

                    $('input[name="KitItems[' + numberIncr + '].Quantity"]').kendoNumericTextBox({
                        decimals: 2,
                        min: 0,
                        max: 9999999999.99,

                    });
                    $('input[name="KitItems[' + numberIncr + '].UnitPrice"]').kendoNumericTextBox({
                        decimals: 2,
                        min: 0,
                        max: 9999999999.99,
                    });
                    $('input[name="KitItems[' + numberIncr + '].DiscountAmount"]').kendoNumericTextBox({
                        decimals: 2,
                        min: 0,
                        max: 9999999999.99,

                    });
                    $('input[name="KitItems[' + numberIncr + '].ExtendedPrice"]').kendoNumericTextBox({
                        decimals: 2,
                        min: 0,
                        max: 9999999999999999.99
                    });

                    $('input[name="KitItems[' + numberIncr + '].ExtendedPrice"]').data("kendoNumericTextBox").enable(false);
                    $('input[name="KitItems[' + numberIncr + '].ItemId"]').rules("add", {
                        required: true
                    });
                    $('input[name="KitItems[' + numberIncr + '].Quantity"]').rules("add", {
                        required: true
                    });
                    $('input[name="KitItems[' + numberIncr + '].UnitPrice"]').rules("add", {
                        required: true
                    });
                    $('input[name="KitItems[' + numberIncr + '].DiscountAmount"]').rules("add", {
                        required: true
                    });

                    //-------------

                    $('input[name="KitItems[' + numberIncr + '].Quantity"]').change(function () {
                        self.calcItimeComponent(numberIncr);
                    });
                    $('input[name="KitItems[' + numberIncr + '].UnitPrice"]').change(function () {
                        self.calcItimeComponent(numberIncr);
                    });
                    $('input[name="KitItems[' + numberIncr + '].DiscountAmount"]').change(function () {
                        self.calcItimeComponent(numberIncr);
                    });
                    return this; //fluent
                },
                calcItimeComponent: function (numberIncr, method) {
                    var discountMethod = $('input[name="KitItems[' + numberIncr + '].DiscountMethod"]').data("kendoDropDownList").value();
                    var quantity = $('input[name="KitItems[' + numberIncr + '].Quantity"]').data("kendoNumericTextBox");
                    //console.log(quantity);
                    var unitprice = $('input[name="KitItems[' + numberIncr + '].UnitPrice"]').data("kendoNumericTextBox");
                    var discountAmount = $('input[name="KitItems[' + numberIncr + '].DiscountAmount"]').data("kendoNumericTextBox");
                    var extendedPrice = $('input[name="KitItems[' + numberIncr + '].ExtendedPrice"]').data("kendoNumericTextBox");
                    if ($.isNumeric(quantity.value()) && $.isNumeric(unitprice.value()) && $.isNumeric(discountAmount.value())) {
                        if (quantity.value() == 0 || unitprice.value() == 0) {
                            extendedPrice.value(0);
                            discountAmount.value(0);
                        } else {
                            if (method != undefined)
                                discountMethod = method;
                            if (discountMethod == 0)//Cash
                            {
                                var totalExtendedPriceCash = (quantity.value() * unitprice.value()) - discountAmount.value();
                                extendedPrice.value(totalExtendedPriceCash);
                            } else if (discountMethod == 1)//Percent
                            {
                                var extendedPricePercent = (quantity.value() * unitprice.value());
                                var totalExtendedPricePercent = extendedPricePercent - (extendedPricePercent * discountAmount.value()) / 100;
                                extendedPrice.value(totalExtendedPricePercent);
                            }
                        }
                    } else {
                        extendedPrice.value(0);
                        discountAmount.value(0);
                    }
                },
                validateItemNumber: function (itemId, kitItemId, numberIncr) {
                    //data-valmsg-for
                    if (accountId != "") {
                        $.ajax({
                            url: "/Account/CheckAccountRelatedHasParent",
                            type: "GET",
                            dataType: "json",
                            contentType: "application/json",
                            data: {
                                accountId: accountId
                                , accountRelationshipID: accountRelationshipID
                                , accountOwnerId: accountOwnerId
                            },
                            success: function (result) {
                                //This is the subsidiary of another account.
                                var error = $("span[data-valmsg-for='AccountRelatedOwners[" + numberIncr + "].AccountInvolvedID']");
                                error.text(window.setaJs.l("Account_SubsidiaryOfOther"));

                                if (result === false) {
                                    error.hide();
                                } else {
                                    error.show();
                                }
                            }
                        });
                    }
                },
                itemOwnedcomponentElement: function (numberIncr, settings, isFocus) {
                    var self = this;
                    $('input[name="KitItemsOwneds[' + numberIncr + '].ItemId"]').kendoComboBox({
                        dataSource: {
                            serverFiltering: true,
                            serverPaging: true,
                            type: "json",
                            transport: {
                                read: {
                                    url: "/Item/GetItems",
                                    contentType: "application/json;",
                                    dataType: "json",
                                    data: {
                                        name: function () {
                                            return $('input[name="KitItemsOwneds[' + numberIncr + '].ItemId"]').val();
                                        }
                                    }
                                }
                            }
                        }
                        , select: function (e) {
                            var dataItem = this.dataItem(e.item.index());

                            $('input[name="KitItemsOwneds[' + numberIncr + '].ItemDescription"]').val(dataItem.ItemDescription);
                            if (dataItem.SerializedItem && !dataItem.ControlledItem) {
                                $('input[name="KitItemsOwneds[' + numberIncr + '].SerialLotItem"]').val(3);
                                $('input[name="KitItemsOwneds[' + numberIncr + '].SerialLotNumber"]').removeAttr('readonly');
                            } else if (!dataItem.SerializedItem && dataItem.ControlledItem) {
                                $('input[name="KitItemsOwneds[' + numberIncr + '].SerialLotNumber"]').removeAttr('readonly');
                                $('input[name="KitItemsOwneds[' + numberIncr + '].SerialLotItem"]').val(2);
                            } else {
                                $('input[name="KitItemsOwneds[' + numberIncr + '].SerialLotNumber"]').val("");
                                $('input[name="KitItemsOwneds[' + numberIncr + '].SerialLotItem"]').val(1);
                                $('input[name="KitItemsOwneds[' + numberIncr + '].SerialLotNumber"]').attr('readonly', 'readonly');
                            }
                        }, change: function (e) {
                            var selectedIndex = this.select();
                            var prItemValue = $("#ItemID").data("kendoComboBox").value();
                            if (this.value() == prItemValue) {
                                $('[data-valmsg-for="' + this.element.attr("name") + '"]').html(setaJs.l('Text_Item_ItemComponent_CantDuplicate'));
                                return true;
                            } else {
                                $('[data-valmsg-for="' + this.element.attr("name") + '"]').html('');
                            }
                            if (this.text() == "") {
                                $('[data-valmsg-for="' + this.element.attr("name") + '"]').html(setaJs.l('Validate_Required').format('Item Number'));
                                return true;
                            } else {
                                $('[data-valmsg-for="' + this.element.attr("name") + '"]').html('');
                            }
                            if (selectedIndex < 0) {
                                $('[data-valmsg-for="' + this.element.attr("name") + '"]').html(setaJs.l('Validate_Invalid').format('Item Number'));
                                return true;
                            } else {
                                $('[data-valmsg-for="' + this.element.attr("name") + '"]').html('');
                            }
                            return false;
                            // Use the value of the widget
                        }
                        , open: kendoComboboxFix.scrollFixHeader,
                        headerTemplate: "<div class='header'><span>" + window.setaJs.l("Show_Top_Results") + "</span></div>",
                        dataTextField: "ItemNumber",
                        dataValueField: "ItemId",
                        filter: "startswith",
                        suggest: true,
                        placeholder: "Select Item Number",// window.setaJs.l("Account_SelectAccountRelated")
                    });

                    $('input[name="KitItemsOwneds[' + numberIncr + '].Quantity"]').kendoNumericTextBox({
                        decimals: 2,
                        min: 0,
                        max: 9999999999.99,
                        change: (function (e) {
                            if (this.value() == null) {
                                this.value(0);
                            }
                        })
                    });

                    //$('input[name="KitItemsOwneds[' + numberIncr + '].ItemId"]').rules("add", {
                    //    required: true
                    //});
                    //$('input[name="KitItemsOwneds[' + numberIncr + '].Quantity"]').rules("add", {
                    //    required: true
                    //});
                    return this; //fluent
                },
                //function
                checkElementExist: function (selector) {
                    if ($(selector).length > 0) return true;
                    else return false;
                },
                autocompletePostalCode: function (selectorZip, selectorCity, selectorState, selectorCounty, selectorCountry) {
                    // only input number
                    $(selectorZip).keydown(function (e) {
                        // Allow: backspace, delete, tab, escape, enter
                        if ($.inArray(e.keyCode, [46, 8, 9, 27, 13]) !== -1 ||
                            // Allow: Ctrl+A
                            (e.keyCode == 65 && e.ctrlKey === true) ||
                            // Allow: home, end, left, right
                            (e.keyCode >= 35 && e.keyCode <= 39)) {
                            // let it happen, don't do anything
                            return;
                        }
                        // Ensure that it is a number and stop the keypress
                        if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
                            e.preventDefault();
                        } else {
                            return;
                        }

                    });
                    $(selectorZip).kendoAutoComplete({
                        minLength: 3,
                        filter: "contains",
                        dataTextField: "PostalCode",
                        suggest: true,
                        dataSource: new kendo.data.DataSource({
                            serverFiltering: true,
                            type: "json",
                            transport: {
                                read: {
                                    url: '/Address/GetPostalCode',
                                    type: 'GET'
                                },
                                parameterMap: function (data, type) {
                                    return {
                                        zipCode: $(selectorZip).val(),
                                        country: $(selectorCountry).val()
                                    };
                                }
                            },
                            serverAggregates: true
                        }),
                        template: kendo.template("#:City#, #:State# (#:PostalCode#)"),
                        open: function (e) {
                            $(".k-list").niceScroll();
                        },
                        select: function (e) {
                            var selectedOne = this.dataItem(e.item.index());
                            if (selectedOne != null) {
                                $(selectorCity).val(selectedOne.City);
                                $(selectorState).val(selectedOne.State);
                                $(selectorCountry).val(selectedOne.Country);
                                $(selectorCounty).val(selectedOne.County);
                            }
                        }
                    });
                },
                autocompleteState: function (selectorState, selectorCountry) {
                    $(selectorState).kendoAutoComplete({
                        dataTextField: "StateName",
                        suggest: true,
                        dataSource: new kendo.data.DataSource({
                            serverFiltering: true,
                            transport: {
                                read: {
                                    url: '/Address/GetState',
                                    type: 'GET',
                                    dataType: 'json'
                                },
                                parameterMap: function (e) {
                                    return {
                                        state: $(selectorState).val(),
                                        country: $(selectorCountry).val()
                                    };
                                }
                            }
                        }),
                        template: kendo.template("#:StateName#"),
                        open: function (e) {
                            $(".k-list").niceScroll();
                        },
                        select: function (e) {
                            var selectedOne = this.dataItem(e.item.index());
                            if (selectedOne != null) {
                                //$("#State").val(selectedOne.County);
                                $(selectorCountry).val(selectedOne.CountryName);
                            }
                        }
                    });
                    /*
                    $(selectorState).on('keydown', function (evg) {
                        //Up Down Key
                        if (evg.which == 38 || evg.which == 40) {
                            var kendoAuto = $(this).data("kendoAutoComplete");
                            var wapper = kendoAuto.list.closest(".k-animation-container");
                            if (!$(wapper).is(":visible")) {
                                var keyword = $(this).val();
                                if (keyword == null || keyword.length <= 0)
                                    keyword = " ";
                                kendoAuto.search(keyword);
                                evg.preventDefault();
                            }
                        }
                    });      
                     */
                    $(selectorState).on('focus', function (evg) {
                        var kendoAuto = $(this).data("kendoAutoComplete");
                        var wapper = kendoAuto.list.closest(".k-animation-container");
                        if (!$(wapper).is(":visible")) {
                            var keyword = $(this).val();
                            if (keyword == null || keyword.length <= 0)
                                keyword = " ";
                            kendoAuto.search(keyword);
                            evg.preventDefault();
                        }
                    });

                },
                autocompleteCountry: function (selectorState, selectorCountry) {
                    $(selectorCountry).kendoAutoComplete({
                        dataTextField: "CountryName",
                        suggest: true,
                        dataSource: new kendo.data.DataSource({
                            serverFiltering: true,
                            transport: {
                                read: {
                                    url: '/Address/GetCountry',
                                    type: 'GET',
                                    dataType: 'json'
                                },
                                parameterMap: function (e) {
                                    return {
                                        state: $(selectorState).val(),
                                        country: $(selectorCountry).val()
                                    };
                                }
                            }
                        }),
                        template: kendo.template("#:CountryName#"),
                        open: function (e) {
                            $(".k-list").niceScroll();
                        },
                        select: function (e) {
                            var selectedOne = this.dataItem(e.item.index());
                            if (selectedOne != null) {
                                //$("#Country").val(selectedOne.County);
                            }
                        }
                    });
                    /*
                    $(selectorCountry).on('keydown', function (evg) {
                        //Up Down Key
                        if (evg.which == 38 || evg.which == 40) {
                            var kendoAuto = $(this).data("kendoAutoComplete");
                            var wapper = kendoAuto.list.closest(".k-animation-container");
                            if (!$(wapper).is(":visible")) {
                                var keyword = $(this).val();
                                if (keyword == null || keyword.length <= 0)
                                    keyword = " ";
                                kendoAuto.search(keyword);
                                evg.preventDefault();
                            }
                        }
                    });
                    */
                    $(selectorCountry).on('focus', function (evg) {
                        var kendoAuto = $(this).data("kendoAutoComplete");
                        var wapper = kendoAuto.list.closest(".k-animation-container");
                        if (!$(wapper).is(":visible")) {
                            var keyword = $(this).val();
                            if (keyword == null || keyword.length <= 0)
                                keyword = " ";
                            kendoAuto.search(keyword);
                            evg.preventDefault();
                        }
                    });

                },
                onActivityBoxInit: function () {
                    taskController.init();
                    eventController.init();

                    var eventSettings = eventController.plugin().settings;
                    var eventPageSize = eventSettings.pageSize;
                    var intMoreEvent = eventPageSize;
                    var taskSettings = taskController.plugin().settings;
                    var taskPageSize = taskSettings.pageSize;
                    var intMoreTask = eventPageSize;

                    $("#more-task").on("click", function () {
                        intMoreTask = intMoreTask + taskPageSize;
                        taskController.TaskByDate(1, intMoreTask, taskPageSize);
                        return false;
                    });
                    $("#more-event").on("click", function () {
                        intMoreEvent = intMoreEvent + eventPageSize;
                        eventController.EventByDate(1, intMoreEvent, eventPageSize);
                        return false;
                    });

                    $('#select-activity-date').kendoCalendar({
                        value: $('#select-activity-date').attr('currentdateuser') != undefined ? new Date($('#select-activity-date').attr('currentdateuser')) : new Date(),
                        footer: "Today - " + kendo.toString(($('#select-activity-date').attr('currentdateuser') != undefined ? new Date($('#select-activity-date').attr('currentdateuser')) : new Date()), 'D'),
                        //footer: "Today - #: kendo.toString(data, 'D') #",
                        current: new Date($('#select-activity-date').attr('currentdateuser')),
                        change: function (e) {
                            $('#select-activity-date').hide();
                            var date = $("#select-activity-date").data("kendoCalendar").value();
                            var todayDate = $('#select-activity-date').attr('currentdateuser') != undefined ? new Date($('#select-activity-date').attr('currentdateuser')) : new Date();
                            if (date.setHours(0, 0, 0, 0) == todayDate.setHours(0, 0, 0, 0)) {
                                $('#current-date').next().html("Today");
                            }
                            else {
                                $('#current-date').next().html(date.format("mm/dd/yyyy"));
                            }
                            intMoreTask = taskPageSize;
                            taskController.TaskByDate(1, intMoreTask, taskPageSize);
                            intMoreEvent = eventPageSize;
                            eventController.EventByDate(1, intMoreEvent, eventPageSize);
                        }
                    });
                    $('#select-activity-date').hide();
                    $("#btn-select-date").on('click', function (e) {
                        e.preventDefault();
                        $("#select-activity-date").show(function (e) {
                            $('div#select-activity-date a.k-nav-today.k-link').attr('title', kendo.toString(($('#select-activity-date').attr('currentdateuser') != undefined ? new Date($('#select-activity-date').attr('currentdateuser')) : new Date()), 'D'));
                        });
                    });
                    $('div#select-activity-date a.k-nav-today.k-link').on('click', function (e) {
                        $('#select-activity-date').hide();
                        $('#current-date').next().html("Today");
                        $("#select-activity-date").data("kendoCalendar").value($('#select-activity-date').attr('currentdateuser') != undefined ? new Date($('#select-activity-date').attr('currentdateuser')) : new Date());
                        intMoreTask = taskPageSize;
                        taskController.TaskByDate(1, intMoreTask, taskPageSize, true);
                        intMoreEvent = eventPageSize;
                        eventController.EventByDate(1, intMoreEvent, eventPageSize, true);
                    });
                    $(document).ready(function () {
                    }).on('mouseup', function (e) {
                        var container = $("#select-activity-date");
                        if (!container.is(e.target)
                            && container.has(e.target).length === 0) {
                            container.hide();
                        }
                    });
                    //Bind data
                    taskController.TaskByDate(1, intMoreTask, taskPageSize);
                    eventController.EventByDate(1, intMoreEvent, eventPageSize);
                },
                onChangeYesNo: function (value, nameControl, numberIcr) {
                    if (value == "true") {
                        $('input[name*=' + nameControl + '][name*=".IsPrimary"]').each(function (idx, item) {
                            if ($(item).attr('name') != nameControl + '[' + numberIcr + '].IsPrimary')
                                $(item).data("kendoDropDownList").value("false");
                        });
                    }
                },
                onViewMoreContactPhonePopover: function (element, contactId, placement) {
                    if (!$(element).hasClass('init-popover')) {
                        $.getJSON("/Contact/GetMorePhone/" + contactId, function (data) {
                            var str = '<div class="popover-custom"><ul>';
                            $.each(data, function (i, item) {
                                str += '<li value="' + item.MobileID + '"><span><span title=' + item.CountryName + ' class="flag-icon flag-icon-' + item.CountryCode + '"></span>' + item.Phone + '</span></li>';
                            });
                            str += '</ul></div>';
                            window.setaJs.ViewMorePopover(element, 'PHONE NUMBERS', str);
                        }, "json").error(function (error) { });
                    }
                },
                //for related account
                onOpenEditRelatedAccountGrid: function (accountRelatedId) {
                    //$("#RelatedAccountId").val(accountRelatedId);
                    //$("#relateAccountOwnerId").val(ownerAccountId);
                    var erawindow = $("#EditRelatedAccount").data("kendoWindow");
                    erawindow.refresh({
                        url: '/Account/EditAccountRelated',
                        data: { accountRelatedId: accountRelatedId, isGrid: true }
                    });
                    erawindow.center().open();
                },
                Grid_OnRowSelect: function (e) {
                    var selectedDataItem = e != null ? e.sender.dataItem(e.sender.select()) : null;
                    $('#RelationNotes').text(selectedDataItem.Note);
                },
                onDeleteAccountRelated: function (accountRelatedId) {
                    window.setaJs.msgConfirm({
                        text: "Do you want to delete this account related?",
                        buttons: [{
                            text: 'Yes',
                            onClick: function ($noty) {
                                $noty.close();
                                $.ajax({
                                    url: "/Account/DeleteAccountRelated/" + accountRelatedId,
                                    type: 'GET',
                                    contentType: 'application/json;',
                                    dataType: 'json',
                                    cache: false,
                                    success: function (result) {
                                        if (result.status === true) {
                                            window.setaJs.msgSuccess({ text: result.msg });
                                            var datasource = $("#RelatedAccounsGrid").data("kendoGrid").dataSource;
                                            datasource.page(1);
                                            datasource.read();
                                            //$('#RelationNotes').text(result.Note);
                                            //clear parent Account
                                        } else {
                                            window.setaJs.msgError({ text: result.msg });
                                        }
                                    }
                                });
                            }
                        }, {

                            text: 'No',
                            onClick: function ($noty) {
                                $noty.close();
                            }
                        }]
                    });
                },
                onOpenAddRelatedAccount: function (ownerAccount, isGrid) {
                    if (isGrid == undefined) isGrid = false;
                    $("#RelatedAccountId").val(0);
                    $("#relateAccountOwnerId").val(ownerAccount);

                    var raaWindow = $("#createRelatedAccount").data("kendoWindow");
                    raaWindow.refresh({
                        url: "/Account/AddAccountRelated",
                        data: { ownerAccount: ownerAccount, relatedAccount: 0, isGrid: isGrid }
                    });
                    raaWindow.center().open();
                },
                RelatedAccountsGridClose: function () {
                    $("#RelatedAccounts").data("kendoWindow").close();
                },
                RollUpChildren: function () {
                    window.setaJs.msgWarning({ text: setaJs.l('TEXT_COMING_SOON') });
                },
                accountRelatedDatabound: function () {
                    var combo2 = $("#AvailableAccount").data("kendoComboBox");
                    if ($("#relatedAccountId").val() > 0) {
                        combo2.value($("#relatedAccountId").val());
                        $("#relatedAccountId").val(0);
                    }
                    var combo3 = $("#cboPrimaryContact").data("kendoComboBox");
                    if ($("#relatedContactId").val() > 0) {
                        combo3.enable(true);
                        //combo3.toggle();
                        combo3.close();
                    }
                },
                onFilterContactByAccount: function () {
                    var accountId = 0;
                    var cboAccount = $("#AvailableAccount").data("kendoComboBox");
                    if (cboAccount != null)
                        accountId = cboAccount.value();
                    return {
                        accountId: accountId
                    };
                },
                onOpenRelatedAccountForm: function (isEdit, isGrid) {
                    if (isGrid == undefined) isGrid = false;
                    var formId;
                    var controlDateId;
                    if (isEdit) {
                        var relatedType = $("#EditRelatedAccount #RelatedTypeForEdit").val();
                        //set origin relate type into param relateTypeOrigin
                        if (!isGrid)
                            accountController.relateTypeOrigin = relatedType;

                        formId = "#EditRelatedAccount";
                        controlDateId = "#relatedDateForEdit";
                    } else {
                        formId = "#createRelatedAccount";
                        controlDateId = "#relatedDate";
                    }
                    // check valid for datetime picker  
                    $(formId + " [data-role='datetimepicker']").on("blur", function (e) {
                        var $this = $(this);
                        //var me = $this.data("kendoDateTimePicker");
                        var id = $this.attr('id');
                        if ($this.val() != '' && (!setaJs.ValidateDateTime($this.val()))) {
                            $('[name=' + id + ']').addClass('input-validation-error');
                            $('[data-valmsg-for=' + id + ']').html(setaJs.l('Message_DateTimeInvalid'));
                            $('[data-valmsg-for=' + id + ']').show();

                            $('.btn-save').attr('disabled', 'disabled');
                        } else {
                            $(controlDateId).removeClass('input-validation-error');
                            $('[data-valmsg-for=' + id + ']').hide();
                            $('.btn-save').removeAttr('disabled');

                        }
                    });
                },
                onSaveAddRelatedAccount: function (isGrid) {
                    if (isGrid == undefined) isGrid = false;
                    var rAData = { AccountRelatedId: "", OwnerAccount: "", RelatedAccount: "", Type: "" };
                    var relatedType = $("#RelatedType").data("kendoDropDownList");
                    var relatedAccount = $("#AvailableAccount").data("kendoComboBox");
                    var relatedContact = $("#cboPrimaryContact").data("kendoComboBox");
                    var relatedDate = $("#relatedDate").data("kendoDateTimePicker");
                    if (relatedAccount.select() == -1) {
                        window.setaJs.msgError({ text: setaJs.l('Related_Account_No_Select') });
                        return;
                    }
                    rAData.AccountRelatedId = !isGrid ? $("#RelatedAccountId").val() : 0;
                    rAData.OwnerAccount = !isGrid ? $("#relateAccountOwnerId").val() : $('#accountOwnerRelateId').val();
                    rAData.RelatedAccount = relatedAccount.value();
                    rAData.RelatedContact = relatedContact.value();
                    rAData.Type = relatedType.value();
                    rAData.RelatedTime = relatedDate.value();
                    rAData.Note = $("#txtRelatedNote").val();
                    var jsonData = JSON.stringify(rAData);
                    $("button.btn-save").attr("disabled", "disabled");
                    $.ajax({
                        url: "/Account/SaveRelatedAccount",
                        data: jsonData,
                        type: 'POST',
                        contentType: 'application/json;',
                        dataType: 'json',
                        cache: false,
                        success: function (result) {
                            if (result.Status === true) {
                                window.setaJs.msgSuccess({ text: result.msg });
                                if (!isGrid) {
                                    accountController.onRelatedAccountReadData(rAData.OwnerAccount);
                                } else {
                                    $("#RelatedAccounsGrid").data("kendoGrid").dataSource.read();
                                    //$('#RelationNotes').text(result.Note);
                                }
                                if (result.IsCreate == true) {
                                    $("#createRelatedAccount").data("kendoWindow").close();
                                    //Update Parent Account
                                    if (result.AccountRelate == 1) {
                                        $("#parentAccount").html(relatedAccount.text());
                                    }

                                } else {
                                    $("#EditRelatedAccount").data("kendoWindow").close();
                                }

                                if (!isGrid)
                                    RecordDetailRefesh($("#CurrentModuleId").val(), rAData.OwnerAccount);
                            }
                            else {
                                window.setaJs.msgError({ text: result.msg });
                                $("button.btn-save").removeAttr("disabled");
                            }
                        }
                    });
                },
                onSaveEditRelatedAccount: function (isGrid) {
                    if (isGrid == undefined) isGrid = false;
                    var rAData = { AccountRelatedId: "", OwnerAccount: "", RelatedAccount: "", Type: "" };
                    var relatedType = $("#RelatedTypeForEdit").data("kendoDropDownList");
                    var relatedAccount = $("#AvailableAccountForEdit").data("kendoComboBox");
                    var relatedContact = $("#cboPrimaryContactForEdit").data("kendoComboBox");
                    var relatedDateTime = $("#relatedDateForEdit").data("kendoDateTimePicker");
                    if (relatedAccount.select() == -1) {
                        window.setaJs.msgError({ text: setaJs.l('Related_Account_No_Select') });
                        return;
                    }
                    rAData.AccountRelatedId = !isGrid ? $("#RelatedAccountId").val() : $('#accountRelatedId').val();
                    rAData.OwnerAccount = !isGrid ? $("#relateAccountOwnerId").val() : $('#accountOwnerRelateId').val();
                    rAData.RelatedAccount = relatedAccount.value();
                    rAData.RelatedContact = relatedContact.value();
                    rAData.Type = relatedType.value();
                    rAData.RelatedTime = relatedDateTime.value();
                    rAData.Note = $("#txtRelatedNote").val();
                    $("button.btn-save").attr("disabled", "disabled");
                    var jsonData = JSON.stringify(rAData);
                    $.ajax({
                        url: "/Account/SaveRelatedAccount",
                        data: jsonData,
                        type: 'POST',
                        contentType: 'application/json;',
                        dataType: 'json',
                        cache: false,
                        success: function (result) {
                            if (result.Status === true) {
                                window.setaJs.msgSuccess({ text: result.msg });
                                if (!isGrid) {
                                    accountController.onRelatedAccountReadData(rAData.OwnerAccount);
                                } else {
                                    $("#RelatedAccounsGrid").data("kendoGrid").dataSource.read();
                                    //$('#RelationNotes').text(result.Note);
                                    $('#RelationNotes').text("");
                                }
                                if (result.IsCreate === true) {
                                    $("#createRelatedAccount").data("kendoWindow").close();
                                    //Update Parent Account
                                    if (result.AccountRelate == 1) {
                                        $("#parentAccount").html(relatedAccount.text());
                                    }

                                } else {
                                    //if RelatedType Dropdown change value
                                    if (!isGrid && accountController.relateTypeOrigin != result.AccountRelate) {
                                        //if RelatedType is Parent
                                        if (result.AccountRelate == 1) {
                                            $("#parentAccount").html(relatedAccount.text());
                                        } else {
                                            $("#parentAccount").html('');
                                        }
                                    }

                                    $("#EditRelatedAccount").data("kendoWindow").close();

                                    //Clear param relateTypeOrigin
                                    if (!isGrid)
                                        delete accountController.relateTypeOrigin;
                                }
                                if (!isGrid)
                                    RecordDetailRefesh($("#CurrentModuleId").val(), rAData.OwnerAccount);
                            }
                            else {
                                window.setaJs.msgError({ text: result.msg });
                                $("button.btn-save").removeAttr("disabled");
                            }
                        }
                    });
                },
                accountRelatedDataboundEdit: function () {
                    var combo2 = $("#AvailableAccountForEdit").data("kendoComboBox");
                    if ($("#relatedAccountIdForEdit").val() > 0) {
                        combo2.value($("#relatedAccountIdForEdit").val());
                        $("#relatedAccountIdForEdit").val(0);
                    }
                    var combo3 = $("#cboPrimaryContactForEdit").data("kendoComboBox");
                    if ($("#relatedContactIdForEdit").val() > 0) {
                        combo3.enable(true);
                        //combo3.toggle();
                        combo3.close();
                    }
                },
                onFilterContactByAccountEdit: function () {
                    var accountId = 0;
                    var cboAccount = $("#AvailableAccountForEdit").data("kendoComboBox");
                    if (cboAccount != null)
                        accountId = cboAccount.value();
                    return {
                        accountId: accountId
                    };
                },
                exportListView: function (ui, title, confirmText, exportUrl, fileUrl, hiddenId) {
                    var layoutId = 0;
                    if ($(ui.cbbLayoutView).length > 0) {
                        layoutId = $(ui.cbbLayoutView).data("kendoComboBox").value();
                    }

                    var grid = $(ui.grid).data().kendoGrid;
                    var data = grid.dataSource._params();
                    var prepared = grid.dataSource.transport.parameterMap(data);
                    var search = $(ui.keyword).val();
                    prepared.keyword = search;
                    prepared.title = title;
                    prepared.layoutId = layoutId;
                    var onlyTag = $("input[name='optradio'][value='1']").prop("checked");
                    if (onlyTag == true) {
                        prepared.ids = $("#" + hiddenId).getHiddenArray();
                    } else {
                        prepared.ids = null;
                    }
                    var totalRecord = grid.dataSource.total();
                    if (totalRecord > 500) {
                        window.setaJs.msgConfirm({
                            text: confirmText,
                            buttons: [{
                                text: 'Yes',
                                onClick: function ($noty) {
                                    $noty.close();
                                    $.ajax({
                                        type: "POST",
                                        url: exportUrl,
                                        contentType: "application/json; charset=utf-8",
                                        dataType: "json",
                                        data: JSON.stringify(prepared),
                                        beforeSend: function () {
                                            $("#loading-ajax").removeClass("hide");

                                        }
                                    })
                                        .done(function (p) {
                                            window.location = kendo.format("{0}?title={1}&fileName={2}",
                                                fileUrl,
                                                p.fileDownload, p.filename);
                                            $("#loading-ajax").addClass("hide");
                                        });
                                }
                            },
                                {
                                    text: 'No',
                                    onClick: function ($noty) {
                                        $noty.close();
                                        return false;
                                    }
                                }]
                        });
                    } else {
                        $.ajax({
                            type: "POST",
                            url: exportUrl,
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            data: JSON.stringify(prepared),
                            beforeSend: function () {
                                $("#loading-ajax").removeClass("hide");

                            }
                        })
                            .done(function (p) {
                                window.location = kendo.format("{0}?title={1}&fileName={2}",
                                    fileUrl,
                                    p.fileDownload, p.filename);
                                $("#loading-ajax").addClass("hide");
                            });
                    }
                    var clearAllTag = $("#chkClearAllTag").prop("checked");
                    if (clearAllTag == true) {
                        $("#" + hiddenId).val("");
                        $('.check-box').each(function () {
                            this.checked = false;
                            var row = $(this).closest("tr");
                            row.removeClass("k-state-selected");
                        });
                        $("input[name='optradio'][value='0']").prop("checked", true);
                        $("input[type='checkbox']").prop("checked", false);
                        $(".menu-action-option input").attr("disabled", true);
                    }
                },
                printListView: function (ui, title, exportUrl, hiddenId) {
                    var layoutId = 0;
                    if ($(ui.cbbLayoutView).length > 0) {
                        layoutId = $(ui.cbbLayoutView).data("kendoComboBox").value();
                    }
                    var grid = $(ui.grid).data().kendoGrid;
                    var data = grid.dataSource._params();
                    var prepared = grid.dataSource.transport.parameterMap(data);
                    var search = $(ui.keyword).val();
                    prepared.keyword = search;
                    prepared.columns = grid.columns;
                    prepared.title = title;
                    prepared.layoutId = layoutId;
                    var onlyTag = $("input[name='optradio'][value='1']").prop("checked");
                    if (onlyTag == true) {
                        prepared.ids = $("#" + hiddenId).getHiddenArray();
                    } else {
                        prepared.ids = null;
                    }
                    $.ajax({
                        type: "POST",
                        url: exportUrl,
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        data: JSON.stringify(prepared)
                    })
                    .done(function (p) {
                        var divprint = $('<div>');
                        $(divprint).html(p.ex);
                        $(divprint).printArea();
                        $(divprint).html("");
                    });
                    var clearAllTag = $("#chkClearAllTag").prop("checked");
                    if (clearAllTag == true) {
                        $("#" + hiddenId).val("");
                        $('.check-box').each(function () {
                            this.checked = false;
                            var row = $(this).closest("tr");
                            row.removeClass("k-state-selected");
                        });
                        $("input[name='optradio'][value='0']").prop("checked", true);
                        $("input[type='checkbox']").prop("checked", false);
                        $(".menu-action-option input").attr("disabled", true);
                    }
                },
                selectionListView: function (gridId, hiddenId, checkAllId, checkClass) {

                    var oGrid = $("#" + gridId).data("kendoGrid");
                    oGrid.table.on("click", ".check-box", selectRow);

                    //on click of the checkbox:
                    function selectRow() {
                        var checked = this.checked,
                            row = $(this).closest("tr"),
                            dataItem = oGrid.dataItem(row);

                        if (checked) {
                            if ($("#" + hiddenId).inHiddenArray(dataItem.Id) <= -1) {
                                $("#" + hiddenId).addToHiddenArray(dataItem.Id);
                            }
                            row.addClass("k-state-selected");
                        } else {
                            if ($("#" + hiddenId).inHiddenArray(dataItem.Id) > -1) {
                                $("#" + hiddenId).removeFromHiddenArray(dataItem.Id);
                            }
                            row.removeClass("k-state-selected");
                        }
                        var arrCount = $("#" + hiddenId).getHiddenArray().length;
                        if (arrCount > 0) {
                            $("input[name='optradio'][value='1']").prop("checked", true);
                            $(".menu-action-option input").removeAttr("disabled");
                            var checkAll = true;
                            $('#' + gridId + ' .' + checkClass + '[id!="' + checkAllId + '"]').each(function () {
                                if (this.checked == false) {
                                    $('#' + checkAllId).prop("checked", false);
                                    checkAll = false;
                                    return;
                                }
                            });
                            if (checkAll) {
                                $('#' + checkAllId).prop("checked", true);
                            }
                        } else {
                            $('#' + checkAllId).prop("checked", false);
                            $("input[name='optradio'][value='0']").prop("checked", true);
                            $("input[type='checkbox']").prop("checked", false);
                            $(".menu-action-option input").attr("disabled", true);
                        }
                    }
                    $(function () {
                        $('#' + checkAllId).click(function (event) {
                            if (this.checked) {
                                $("input[name='optradio'][value='1']").prop("checked", true);
                                $('.' + checkClass).each(function () {
                                    this.checked = true;
                                    var row = $(this).closest("tr");
                                    row.addClass("k-state-selected");
                                    var dataItem = oGrid.dataItem(row);
                                    if ($("#" + hiddenId).inHiddenArray(dataItem.Id) <= -1) {
                                        $("#" + hiddenId).addToHiddenArray(dataItem.Id);
                                    }
                                });
                            } else {
                                $('.' + checkClass).each(function () {
                                    this.checked = false;
                                    var row = $(this).closest("tr");
                                    row.removeClass("k-state-selected");
                                    var dataItem = oGrid.dataItem(row);
                                    if ($("#" + hiddenId).inHiddenArray(dataItem.Id) > -1) {
                                        $("#" + hiddenId).removeFromHiddenArray(dataItem.Id);
                                    }
                                });
                            }
                            if ($("#" + hiddenId).getHiddenArray().length > 0) {
                                $("input[name='optradio'][value='1']").prop("checked", true);
                                $(".menu-action-option input").removeAttr("disabled");
                            } else {
                                $("input[name='optradio'][value='0']").prop("checked", true);
                                $("input[type='checkbox']").prop("checked", false);
                                $(".menu-action-option input").attr("disabled", true);
                            }
                        });
                    });
                },
                selectionListViewDataBound: function (grid, hiddenId, checkAllId, checkClass) {
                    var checkAll = true;
                    var view = grid.dataSource.view();
                    if ($("#" + hiddenId).getHiddenArray().length > 0) {
                        for (var i = 0; i < view.length; i++) {
                            if ($("#" + hiddenId).inHiddenArray(view[i].Id) > -1) {
                                grid.tbody.find("tr[data-uid='" + view[i].uid + "']")
                                    .addClass("k-state-selected")
                                    .find("." + checkClass)
                                    .attr("checked", "checked");
                            } else {
                                checkAll = false;
                            }
                        }

                        if ($("#" + hiddenId).getHiddenArray().length > 0) {
                            $("input[name='optradio'][value='1']").prop("checked", true);
                            $(".menu-action-option input").removeAttr("disabled");
                        } else {
                            $("input[name='optradio'][value='0']").prop("checked", true);
                            $("input[type='checkbox']").prop("checked", false);
                            $(".menu-action-option input").attr("disabled", true);
                        }
                    } else {
                        $("input[name='optradio'][value='0']").prop("checked", true);
                        $("input[type='checkbox']").prop("checked", false);
                        $(".menu-action-option input").attr("disabled", true);
                        checkAll = false;
                    }
                    if (checkAll) {
                        $('#' + checkAllId).prop("checked", true);
                    } else {
                        $('#' + checkAllId).prop("checked", false);
                    }
                },
                exportShortKey: function (ui) {
                    $(document).keydown(function (e) {
                        if (e.ctrlKey) {
                            switch (e.keyCode) {
                                // Ctrl + E: Export to Excel
                                case 69:
                                    e.preventDefault();
                                    $(ui.exportexcel).trigger("click");
                                    break;
                                    // Ctrl + F: Export to Pdf
                                case 70:
                                    e.preventDefault();
                                    $(ui.exportpdf).trigger("click");
                                    break;
                                    // Ctrl + P: Export to printer
                                case 80:
                                    e.preventDefault();
                                    $(ui.exportprinter).trigger("click");
                                    break;
                                default:
                            }
                        }
                    });
                },
                onOpenAssignRecord: function (moduleId, objectId) {
                    var raaWindow = $("#AssignRecord").data("kendoWindow");
                    raaWindow.refresh({
                        url: "/ServiceTicket/OpenAssignRecord",
                        data: { moduleId: moduleId, objectId: objectId }
                    });
                    var title = '';
                    switch (moduleId) {
                        case 3:
                            title = window.setaJs.l('TEXT_ASSIGN_RECORD_FORMAT').format(window.setaJs.l('TEXT_SERVICE_TICKET')) + ' [' + objectId + ']';
                            break;
                        case 19:
                            title = window.setaJs.l('TEXT_ASSIGN_RECORD_FORMAT').format(window.setaJs.l('TEXT_DEFECT')) + ' [' + objectId + ']';
                            break;
                    }
                    raaWindow.title(title);
                    raaWindow.center().open();
                },
                assignToTypeChange: function () {
                    var type = this.select();
                    var $divAssignto = $('#assigntoCondition');
                    switch (type) {
                        case 0:
                            $('label[for="AssignToId"]').html(window.setaJs.l(''));
                            $divAssignto.addClass('hidden');
                            break;
                        case 1:
                            $('label[for="AssignToId"]').html(window.setaJs.l('TEXT_USER_ID') + "*");
                            $divAssignto.removeClass('hidden');
                            break;
                        case 2:
                            window.setaJs.msgWarning({ text: setaJs.l('TEXT_COMING_SOON') });
                            this.select(1);
                            $divAssignto.removeClass('hidden');
                            break;
                    }
                },
                onSaveAssignRecord: function () {
                    var assignTypeId = $('#AssignToTypeId').val();
                    var assignToId = parseInt(assignTypeId) > 1 ? ($('#AssignToId').val() != "" ? $('#AssignToId').val() : null) : null;
                    var moduleId = window.setaJs.oGlobal.CurrentModule;
                    var objectId = $('#objectId').val();
                    var gridName = '';
                    switch (moduleId) {
                        case 3:
                            gridName = "ServiceTicketGrid";
                            break;
                        case 19:
                            gridName = "DefectGrid";
                            break;
                    }

                    var assignRecord = {
                        AssignToTypeId: assignTypeId,
                        AssignToId: assignToId,
                        ModuleId: moduleId,
                        ObjectId: objectId
                    };
                    var panel = $('#AssignRecord');
                    var jsonData = JSON.stringify(assignRecord);
                    $.ajax({
                        url: "/ServiceTicket/SaveAssignRecord",
                        data: jsonData,
                        type: 'POST',
                        contentType: 'application/json;',
                        dataType: 'json',
                        async: false,
                        success: function (response) {
                            if (response.Status === true) {
                                var grid = $("#" + gridName);
                                var gridData = grid.data("kendoGrid");
                                $("#AssignRecord").data("kendoWindow").close();
                                if (grid.length > 0) {
                                    setTimeout(function () {
                                        gridData.dataSource.read();
                                    }, 500);
                                }
                            } else {
                                $(response.Errors).each(function (key, error) {
                                    if (error.msgs.length > 0) {
                                        $(panel).find("[name='" + error.field + "']").parents(".row").addClass("error").find("span.error").html(error.msgs);
                                        $(panel).find("[name='" + error.field + "']").parents(".row").find('input').addClass("input-validation-error").on("keypress", function () {
                                            $(this).removeClass("input-validation-error");
                                            $(panel).find("[name='" + error.field + "']").parents(".row").find("span.error").html('');
                                        });
                                        $(panel).find("[name='" + error.field + "']").parents(".row").find('input').addClass("input-validation-error").on("change", function () {
                                            $('.input-validation-error').removeClass("input-validation-error");
                                            $(panel).find("[name='" + error.field + "']").parents(".row").find("span.error").html('');
                                        });
                                    } else {
                                        $(panel).find("[name='" + error.field + "']").parents(".row").removeClass("error").find("span.error").empty();
                                        $(panel).find("[name='" + error.field + "']").parents(".row").find('input').removeClass("input-validation-error");
                                    }
                                    $(panel).find("[name='" + $(response.Errors)[0].field + "']").focus();
                                });
                            }
                        }
                    });
                },

            }
        }
    };