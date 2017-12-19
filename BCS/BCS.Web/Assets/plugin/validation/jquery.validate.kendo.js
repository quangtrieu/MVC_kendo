//http://stackoverflow.com/questions/17656971/jquery-ui-validation-plugin-does-not-work-with-kendo-ui-controls
(function ($) {
    var _highlight = $.validator.defaults.highlight;
    var _unhighlight = $.validator.defaults.unhighlight;
    var _errorClassKendo = 'input-validation-error-kendo';
    $.extend($.validator.defaults, {
        ignore: [],
        highlight: function (element, errorClass, validClass) {
            var $element = $(element);
            var role = $element.data('role');

            if (role === 'combobox') {
                var comboBox = $element.data('kendoComboBox');
                comboBox.input.removeClass(validClass);
                comboBox.wrapper.children(':first').addClass(_errorClassKendo);
            }
            else if (role === 'dropdownlist') {
                var dropdownlist = $element.data('kendoDropDownList');
                dropdownlist.element.removeClass(validClass);
                dropdownlist.wrapper.children(':first').addClass(_errorClassKendo);
            }
            else if (role === 'editor') {
                $element.data('kendoEditor').wrapper.find('iframe').addClass(_errorClassKendo).removeClass(validClass);
            }
            else if (role === 'multiselect') {
                $element.data('kendoMultiSelect')._innerWrapper.addClass(_errorClassKendo).removeClass(validClass);
            }
            else if (role === 'numerictextbox') {
                $element.data('kendoNumericTextBox')._text.addClass(_errorClassKendo).removeClass(validClass);
            }
            else {
                _highlight(element, errorClass, validClass);
            }
        },
        unhighlight: function (element, errorClass, validClass) {
            var $element = $(element);
            var role = $element.data('role');

            if (role === 'combobox') {
                var comboBox = $element.data('kendoComboBox');
                comboBox.input.addClass(validClass);
                comboBox.wrapper.children(':first').removeClass(_errorClassKendo);
            }
            else if (role === 'editor') {
                $element.data('kendoEditor').wrapper.find('iframe').addClass(validClass).removeClass(_errorClassKendo);
            }
            else if (role === 'multiselect') {
                $element.data('kendoMultiSelect')._innerWrapper.addClass(validClass).removeClass(_errorClassKendo);
            }
            else if (role === 'numerictextbox') {
                $element.data('kendoNumericTextBox')._text.addClass(validClass).removeClass(_errorClassKendo);
            } else {
                _unhighlight(element, errorClass, validClass);
            }
        }
    });

}(jQuery));