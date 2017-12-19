(function (window, $) {
    $.fn.myprofileController = function (options) {
        var defaultOptions = {};
        // Establish our default settings
        var settings = $.extend(defaultOptions, options);
        $.extend(myprofileController, settings);
        return this.each(function () {
            //code here
        });
    };

})(window, jQuery);



var MyProfile = {
    kendoComboBoxCountryOnChange : function(e) {
        var selectedIndex = this.select();
        if (e.sender._old == "America United State") {
            jQuery('#empStateData').removeClass('hidden');
            jQuery('#otherState').addClass('hidden');
        } else {
            jQuery('#empStateData').addClass('hidden');
            jQuery('#otherState').removeClass('hidden');
            jQuery('#State').val("");
        }
        if (selectedIndex < 0) {
            this.value(null);
            this.dataSource.filter([]);
        }
     },
        kendoComboBoxStateOnChange: function (e) {
            var selectedIndex = this.select();
            jQuery('#empStateData .field-validation-error').remove();
            if (selectedIndex < 0) {
                this.value(null);
                this.dataSource.filter([]);
            }
        }
}