/*!
 * jQuery Print Previw Plugin v1.0.1
 *
 * Copyright 2011, Tim Connell
 * Licensed under the GPL Version 2 license
 * http://www.gnu.org/licenses/gpl-2.0.html
 *
 * Date: Wed Jan 25 00:00:00 2012 -000
 */
 
(function($) { 
    
	// Initialization
	$.fn.printPreview = function() {
		this.each(function() {
		    $(this).bind('click', function (e) {
		        e.preventDefault();
		        var $objID = jQuery('#showVariance').html();
			    if (!$('#print-modal').length) {
			        $.printPreview.loadPrintPreview($objID);
			    }
			});
		});
		return this;
	};
    
    // Private functions
    var mask, size, print_modal, print_controls;
    $.printPreview = {
        loadPrintPreview: function ($objID) {
            // Declare DOM objects
            print_modal = $('<div id="print-modal" style="width: 1166px; left: 550px"></div>');
            print_controls = $('<div id="print-modal-controls">' + 
                                    '<a href="#" class="print" title="Print page">Print page</a>' +
                                    '<a href="#" class="close" title="Close print preview">Close</a>').hide();
            var print_frame = $('<iframe id="print-modal-content" scrolling="no" border="0" frameborder="0" name="print-frame" />');

            // Raise print preview window from the dead, zooooooombies
            print_modal
                .hide()
                .append(print_controls)
                .append(print_frame)
                .appendTo('body');

            // The frame lives
            var print_frame_ref = '';
            for (var i = 0; i < window.frames.length; i++) {
                if (i == window.frames.length-1) {
                    if (window.frames[i].name == "print-frame") {
                        var print_frame_ref = window.frames[i].document;
                        break;
                    }
                }
                
                
               
                /*
                if (window.frames[i].name == "print-frame") {
                    var print_frame_ref = window.frames[i].document;
                    break;
                }*/
            }

            print_frame_ref.open();
            print_frame_ref.write('<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">' +
                '<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en" lang="en" style="background-color: white;">' +
                '<head><title>' + document.title + '</title></head>' +
                '<body></body>' +
                '</html>');
            print_frame_ref.close();

            // add annual sales
            var $divPrint = $('<div style="width: 100%"></div>');
            var annualSales = $('.active input[id^="Annual_Sales_"]').val();
            $divPrint.append($('<div class="col-xs-12" style="border-bottom: 1px solid #111; padding: 20px 0px 10px 0px; font-size: 12pt; font-weight: bold; margin-bottom: 10px;">'
                + '<div class="col-xs-4" style="width: 537px;padding: 0px">Annual Sales:</div>'
                + '<div class="col-xs-4" style="padding: 0px">' + annualSales + '</div></div>'));

            var createYear = ' ' + $('.active:last').attr('budget-tab-name').replace('Period budget - ', '');

            var budgetReport = $($objID).find('.budget-detail-item');
            var dataRowByCategoryList = $(budgetReport).children();
            var colLength = $(dataRowByCategoryList).last().children().length;
            for (var colIndex = 1; colIndex <= colLength;) {
                $.each(dataRowByCategoryList, function (i, item) {
                    var $childrenDiv = $('<div style="width: 1166px; float: left; display: inline;"></div>');
                    if (i == 0) {
                        if (colIndex == 1) {
                            var $categoryItem1 = $(item).children().first();
                            $childrenDiv.append($categoryItem1.attr('style', 'width: 538px; height: 41px !important;'));

                            var $categoryItem = $(item).children().first().css('border', '1px solid #A7A7A7').css('width', '537px');
                            $categoryItem.html($categoryItem.children().append(createYear).css('font-weight', 'bold').css('margin-top', '7px'));
                            $childrenDiv.append($categoryItem); //1
                        } else {
                            var $categoryItem = $(item).children().first().css('border', '1px solid #A7A7A7').css('width', '537px');
                            $categoryItem.html($categoryItem.children().append(createYear).css('font-weight', 'bold').css('margin-top', '7px'));
                            if ($categoryItem.children().length != 0)
                                $categoryItem.css('border-right-width', '0px');

                            $childrenDiv.append($categoryItem); //1

                            var $categoryItem2 = $(item).children().first().css('border', '1px solid #A7A7A7').css('width', '537px');
                            $categoryItem2.html($categoryItem2.children().append(createYear).css('font-weight', 'bold').css('margin-top', '7px'));
                            //if ($categoryItem2.children().length != 0)
                            //    $categoryItem2.css('border-right-width', '0px');
                            $childrenDiv.append($categoryItem2); //2
                        }
                    } else {
                        if (colIndex == 1) {
                            var $categoryItem = $(item).children().first();
                            $categoryItem.children().html($categoryItem.children().attr('title'));
                            if ($categoryItem.children().length > 0 && $categoryItem.children().html().length > 100) {
                                var categoryName = $categoryItem.children().html();
                                $categoryItem.empty().html(categoryName);
                                $categoryItem.css('font-size', '7pt');
                            }
                            $categoryItem.css('width', '537px');

                            $childrenDiv.append($.printPreview.convertBudgetItemInputToLabel($categoryItem));
                            $childrenDiv.append($.printPreview.convertBudgetItemInputToLabel($(item).children().first().css('width', '179px'))); //1
                            $childrenDiv.append($.printPreview.convertBudgetItemInputToLabel($(item).children().first().css('width', '179px'))); //2
                            $childrenDiv.append($.printPreview.convertBudgetItemInputToLabel($(item).children().first().css('width', '179px').css('border-right', '1px solid #A7A7A7'))); //3
                        } else {
                            $childrenDiv.append($.printPreview.convertBudgetItemInputToLabel($(item).children().first().css('width', '179px'))); //1
                            $childrenDiv.append($.printPreview.convertBudgetItemInputToLabel($(item).children().first().css('width', '179px'))); //2
                            $childrenDiv.append($.printPreview.convertBudgetItemInputToLabel($(item).children().first().css('width', '179px'))); //3
                            $childrenDiv.append($.printPreview.convertBudgetItemInputToLabel($(item).children().first().css('width', '179px'))); //4
                            $childrenDiv.append($.printPreview.convertBudgetItemInputToLabel($(item).children().first().css('width', '179px'))); //5
                            $childrenDiv.append($.printPreview.convertBudgetItemInputToLabel($(item).children().first().css('width', '179px').css('border-right', '1px solid #A7A7A7'))); //6
                        }
                    }
                    $divPrint.append($childrenDiv);
                });
                $divPrint.append($('<p>&nbsp;</p>'));
                if (colIndex == 1 && dataRowByCategoryList.length < 10) {
                    $divPrint.append($('<div class="break-page-one-row-and-one-column">&nbsp;</div>'));
                }

                colIndex += 6;
                if (colIndex <= colLength) {
                    $divPrint.append($('<div style="page-break-after: always; width: 100%">&nbsp;</div>'));
                    $divPrint.append($('<div style="height: 55px">&nbsp;</div>'));
                }
            }

            // Grab contents and apply stylesheet
            var $iframe_head = $('head link').clone(),
                //$iframe_body = $($(budgetReport) + ':not(#print-modal):not(script):not(iframe)').clone(),
                $iframe_body_link = $('body link').clone();

            $iframe_head.each(function() {
                $(this).attr('media', 'all');
            });
            $iframe_body_link.each(function () {
                $(this).attr('media', 'all');
            });
            //$iframe_body.find("iframe").remove();
            //if (!$.browser.msie && !($.browser.version < 7) ) {
            if (!(navigator.appName == 'Microsoft Internet Explorer')) {
                $('head', print_frame_ref).append($iframe_head);
                $('head', print_frame_ref).append($iframe_body_link);

                $('body', print_frame_ref).append('<style type="text/css" media="print">@page{size: auto; margin: 5mm 10mm 9mm 10mm;};</style>'
                                                + '<div id="printContainer" style="background-color: white;"/>');
                $('body', print_frame_ref).find('#printContainer').append($divPrint);
                
                //$('body', print_frame_ref).find("svg > path").each(function () {
                //    $('#searchHelp').remove();
                //});
            }
            else {
               
            }
            
            // Disable all links
            $('a', print_frame_ref).bind('click.printPreview', function(e) {
                e.preventDefault();
            });
            
            // Introduce print styles
           

            // Load mask
            $.printPreview.loadMask();

            // Disable scrolling
            $('body').css({overflowY: 'hidden', height: '100%'});
            $('img', print_frame_ref).load(function() {
                print_frame.height($('body', print_frame.contents())[0].scrollHeight);
            });
            
            // Position modal            
            starting_position = $(window).height() + $(window).scrollTop();
            var css = {
                top: starting_position,
                height: '100%',
                overflowY: 'auto',
                zIndex: 10055,
                display: 'block',
                //margin: '5mm 10mm 5mm 10mm',
            };
            print_modal
                .css(css)
                .animate({ top: $(window).scrollTop() }, 400, 'linear', function () {
                    print_controls.fadeIn('slow').focus();
                });
            print_frame.height($('body', print_frame.contents())[0].scrollHeight + 20);
            
            // Bind closure
            $('a', print_controls).bind('click', function(e) {
                e.preventDefault();
                //if ($(this).hasClass('print')) { window.print(); }

                if ($(this).hasClass('print')) {
                    var pwin = window.open('', 'print_content', 'height=' + screen.height + ', width=' + screen.width);//window.open('', 'print_content', 'width=100vh,height=100vh');
                    pwin.document.open();
                    var print_content = $("#print-modal-content").contents().find("html").html();
                    if ($(print_content).find('.break-page-one-row-and-one-column').length > 0) {
                        print_content = print_content.replace('<div class="break-page-one-row-and-one-column">&nbsp;</div>', '<div class="break-page-one-row-and-one-column" style="margin-top: 500px">&nbsp;</div>');
                    }

                    // FOR IE browser
                    print_content = print_content.replace('<BODY>', '<BODY onload="window.print()">');
                    // FOR Other browser
                    print_content = print_content.replace('<body>', '<body onload="window.print()">');
                    pwin.document.write(print_content);
                    pwin.document.close();
                    setTimeout(function () { pwin.close(); }, 1000);
                    $.printPreview.distroyPrintPreview();
                }

                else { $.printPreview.distroyPrintPreview(); }
            });
    	},
    	
    	distroyPrintPreview: function() {
    	    print_controls.fadeOut(100);
    	    print_modal.animate({ top: $(window).scrollTop() - $(window).height(), opacity: 1}, 100, 'linear', function(){
    	        print_modal.remove();
    	        $('body').css({overflowY: 'auto', height: 'auto'});
    	    });
    	    mask.fadeOut('slow', function()  {
    			mask.remove();
    		});

    		$(document).unbind("keydown.printPreview.mask");
    		mask.unbind("click.printPreview.mask");
    		$(window).unbind("resize.printPreview.mask");
	    },
	    
    	/* -- Mask Functions --*/
	    loadMask: function() {
	        size = $.printPreview.sizeUpMask();
            mask = $('<div id="print-modal-mask" />').appendTo($('body'));
    	    mask.css({
    			position:           'absolute', 
    			top:                0, 
    			left:               0,
    			width:              "100vh",
    			height:             size[1],
    			display:            'none',
    			opacity:            0,
    			zIndex:             10004,
    			backgroundColor:    '#000',
    			//margin: '5mm 10mm 5mm 10mm',
    		});
	
    		mask.css({display: 'block'}).fadeTo('400', 0.75);
    		
            $(window).bind("resize.printPreview.mask", function() {
				$.printPreview.updateMaskSize();
			});
			
			mask.bind("click.printPreview.mask", function(e)  {
				$.printPreview.distroyPrintPreview();
			});
			
			$(document).bind("keydown.printPreview.mask", function(e) {
			    if (e.keyCode == 27) {  $.printPreview.distroyPrintPreview(); }
			});

			//setTimeout(function () {
			//    $('.print').click();
			//}, 1000);
        },
    
        sizeUpMask: function() {
            if (navigator.appName == 'Microsoft Internet Explorer') {
            	// if there are no scrollbars then use window.height
            	var d = $(document).height(), w = $(window).height();
            	return [
            		window.innerWidth || 						// ie7+
            		document.documentElement.clientWidth || 	// ie6  
            		document.body.clientWidth, 					// ie6 quirks mode
            		d - w < 20 ? w : d
            	];
            } else { return [$(document).width(), $(document).height()]; }
        },
    
        updateMaskSize: function() {
    		var size = $.printPreview.sizeUpMask();
    		mask.css({ width: size[0], height: size[1] });
        },

        convertBudgetItemInputToLabel: function (item) {
            if ($(item).children().length == 0) return $(item);

            var childrenList = $(item).children();
            var $newDiv = $(item).empty();
            var fontWeight = "";
            if ($(item).hasClass('header-format')) {
                $newDiv.css('background-color', '#D9F0FB !important');
                fontWeight = "font-weight: bold; ";
            }
            if (childrenList.length == 1) {
                var $newLabel = $('<label style="margin-top: 5px; ' + fontWeight + '">' + $(childrenList).html() + '</label>');
                $newDiv.append($newLabel);
            } else {
                var $childrenDiv = $('<div style="display: inline-flex"></div>');
                $.each(childrenList, function (i, v) {
                    fontWeight += ($(v).hasClass('bcs-currency-textbox')) ? "width: 105px" : "width: 50px";
                    var $newLabel = $('<label style="margin-top: 5px; ' + fontWeight + '">' + $(v).val() + '</label>');
                    $childrenDiv.append($newLabel);
                })
                $newDiv.append($childrenDiv);
            }
            return $newDiv;
        }
    }
})(jQuery);