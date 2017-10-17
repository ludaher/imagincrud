var homeModule = angular.module("Form", ['ui.bootstrap', 'ngMask']).service(['$q', '$rootScope', '$log']);
//var homeModule = angular.module("Form", ['ui.bootstrap', 'ngMask']).service('LoadingInterceptor', ['$q', '$rootScope', '$log', LoadingInterceptor])
//.config(['$httpProvider', function ($httpProvider) { $httpProvider.interceptors.push('LoadingInterceptor'); }]);

homeModule.controller("FormController", function ($scope, $http) {
    $scope.client = { Name: '', CustomerId: 0, IdentificationType: 'NIT' };
    $scope.defaultIframeHeigth = 400;
    $scope.iframeHeight = 9000;
    $scope.newForm = { Name: "", Active: true, TemplateHeight: 2000 };
    $scope.savingSection = false;
    $scope.sections = [];
    $scope.formErrors = [];
    $scope.actualSection = 0;
    $scope.actualField = 0;
    $scope.rotate = false;
    $scope.viewSection = false;
    $scope.dataLoaded = false;
    $scope.configuration = { basicData: true, sections: new Array() };
    $scope.$iframe = $('#viewer');
    $scope.$formPanel = $('#formPanel');
    $scope.actionsFieldPosX = 0;
    $scope.actionsFieldPosY = 0;
    $scope.actualFocusField = -1;
    $scope.actualFocusRow = 1;
    $scope.tabNumber = 0;
    $scope.sectionRows = 1;
    $scope.addOptionInForm = function () {
        $scope.sections[$scope.actualSection].Fields[$scope.actualField].options.push({});
    }
    $scope.removeOptionInForm = function (option) {
        var index = $scope.sections[$scope.actualSection].Fields[$scope.actualField].options.indexOf(option);
        $scope.sections[$scope.actualSection].Fields[$scope.actualField].options.splice(index, 1);
        $scope.isValidForm();
    }
    $scope.onDropComplete1 = function (data, evt) {
        if (data == undefined) { return false; }
        if ($scope.sections[$scope.actualSection].Fields == undefined) { $scope.sections[$scope.actualSection].Fields = new Array(); }
        var newItemId = $scope.sections[$scope.actualSection].Fields.length;
        data.Id = newItemId;
        data.FieldName = 'Field' + newItemId;
        $scope.sections[$scope.actualSection].Fields.push(angular.copy(data));
        $scope.isValidForm();
    };
    $scope.clickBody = function (evt) {
        var target = evt.target || evt.srcElement;
        if (target.className.includes("edit-control") || target.className.includes("navbar-btn") || target.className.includes("btn") || $(target).parents('.edit-control').length)
            return;
        $scope.actualFocusField = 0;
        $scope.refocus();
    };
    $scope.addSectionPosition = function (value) {

        $scope.sections[$scope.actualSection].Position += value;
        if ($scope.sections[$scope.actualSection].Position > $scope.newForm.TemplateHeight - $scope.defaultIframeHeigth) {
            $scope.sections[$scope.actualSection].Position = $scope.newForm.TemplateHeight - $scope.defaultIframeHeigth
        } else if ($scope.sections[$scope.actualSection].Position < 0) {
            $scope.sections[$scope.actualSection].Position = 0;
        }
        $('.section-position').slider('setValue', parseInt($scope.sections[$scope.actualSection].Position));
        $scope.changePdfPosition();
    };
    $scope.setActualFocusField = function (index, row) {
        $scope.actualFocusField = index;
        $scope.actualFocusRow = row;
    }
    $scope.refocus = function () {
        if ($('.formField' + $scope.actualFocusField + '_' + $scope.actualFocusRow).is('input')) {
            $('.formField' + $scope.actualFocusField + '_' + $scope.actualFocusRow).focus();
            return;
        }
        $('.formField' + $scope.actualFocusField + '_' + $scope.actualFocusRow).parent().children('button').focus();
    }
    $scope.keyDown = function (evt) {
        if (evt.shiftKey && evt.keyCode == 9) {
            evt.preventDefault();
            $scope.previousField();
        }///Tab key
        else if (evt.keyCode == 9) {
            evt.preventDefault();
            $scope.nextField();
        }///Page Up key
        else if (evt.keyCode == 38) {
            if (evt.ctrlKey) {
                evt.preventDefault();
                if (evt.shiftKey) {
                    $scope.addSectionPosition(-100);
                } else {
                    $scope.addSectionPosition(-10);
                }
            }
        }///Page Down key
        else if (evt.keyCode == 40) {
            if (evt.ctrlKey) {
                evt.preventDefault();
                if (evt.shiftKey) {
                    $scope.addSectionPosition(+100);
                } else {
                    $scope.addSectionPosition(+10);
                }
            }
        }///Escape
        else if (evt.keyCode == 27) {
            if ($scope.actualSection > 0) {
                $scope.actualSection--;
            }
            if ($scope.sections[$scope.actualSection].IsTable == true) {
                $scope.sectionRows = $scope.sections[$scope.actualSection].NumberOfRows;
            }
            else {
                $scope.sectionRows = 1;
            }
            $('.section-position').slider('setValue', parseInt($scope.sections[$scope.actualSection].Position));
            $scope.changePdfPosition();
            $scope.setActualFocusField(1, 1);
            $scope.$apply();
            $scope.refocus();
        }
    };
    $scope.nextField = function () {
        if ($scope.actualFocusField + 1 >= $scope.sections[$scope.actualSection].Fields.length) {
            $scope.actualFocusField = 0;
            if ($scope.actualFocusRow < $scope.sectionRows) {
                $scope.actualFocusRow++;
            } else {
                $scope.actualFocusRow = 1;
            }

        } else {
            $scope.actualFocusField++;
        }
        $scope.refocus();
    }
    $scope.previousField = function () {
        if ($scope.actualFocusField <= 0) {
            $scope.actualFocusField = $scope.sections[$scope.actualSection].Fields.length - 1;
            if ($scope.actualFocusRow > 1) {
                $scope.actualFocusRow--;
            }
            else {
                $scope.actualFocusRow = $scope.sectionRows - 1;
            }
        } else {
            $scope.actualFocusField--;
        }
        $scope.refocus();
    }
    $scope.moveBackField = function () {
        if ($scope.actualField <= 0) {
            return false;
        }
        var tempField = $scope.sections[$scope.actualSection].Fields[$scope.actualField];
        $scope.sections[$scope.actualSection].Fields[$scope.actualField] = $scope.sections[$scope.actualSection].Fields[$scope.actualField - 1];
        $scope.sections[$scope.actualSection].Fields[$scope.actualField - 1] = tempField;
        $scope.actualField--;
    };
    $scope.moveNextField = function () {
        if ($scope.actualField + 1 >= $scope.sections[$scope.actualSection].Fields.length) {
            return false;
        }
        var tempField = $scope.sections[$scope.actualSection].Fields[$scope.actualField];
        $scope.sections[$scope.actualSection].Fields[$scope.actualField] = $scope.sections[$scope.actualSection].Fields[$scope.actualField + 1];
        $scope.sections[$scope.actualSection].Fields[$scope.actualField + 1] = tempField;
        $scope.actualField++;
    };
    $scope.addSizeField = function (size) {
        if ($scope.sections[$scope.actualSection].Fields[$scope.actualField].Size > 11 && size > 0
            || $scope.sections[$scope.actualSection].Fields[$scope.actualField].Size <= 1 && size < 0)
            return false;
        $scope.sections[$scope.actualSection].Fields[$scope.actualField].Size += size;
    };

    $scope.pdfClass = function () {
        var classes = '';
        if ($scope.rotate == true) {
            classes += ' rotate180';
        }
        return classes;
    }
    $scope.selectedClass = function (field) {
        var section = $scope.sections[$scope.actualSection];
        if (!section)
            return '';
        var fields = section.Fields;
        var selectedClass = fields && fields.indexOf(field) == $scope.actualField ? 'selected' : '';
        return selectedClass;
    };
    $scope.removeField = function () {
        $scope.sections[$scope.actualSection].Fields.splice($scope.actualField, 1);
        $scope.actualField = -1;
    };
    $scope.isMultipleOption = function (field) {
        return field == '4' || field == '5';
    };
    $scope.isTextBox = function (field) {
        return field == '1' || field == '2';
    };

    $scope.formFieldClass = function ($index, row) {
        return 'formField' + $index + '_' + row;
    };

    ///Función que carga la sección en el formulario de edición
    ///
    $scope.loadSection = function (section, evt) {
        if (section == null) {
            return;
        }
        var target = evt.target || evt.srcElement;
        var index = $scope.sections.indexOf(section);
        $scope.actualSection = index;
        $scope.viewSection = index != -1;
        $('.section-position').slider('setValue', parseInt(section.Position));
        $scope.changePdfPosition();
        $scope.setActualFocusField(-1, 1);
        if ($scope.sections[$scope.actualSection].IsTable == true) {
            $scope.sectionRows = $scope.sections[$scope.actualSection].NumberOfRows;
        }
        else {
            $scope.sectionRows = 1;
        }
        $scope.$formPanel.scrollTop(0);
    };
    $scope.getTimes = function (count) {
        var ratings = [];
        for (var i = 1; i <= count; i++) {
            ratings.push(i)
        }
        return ratings;
    };
    $scope.indexOfSection = function (section) {
        var index = $scope.sections.indexOf(section);
        return index;
    };
    ///Vista previa de la imagen
    ///
    $scope.PreviewImage = function () {
        var files = document.getElementById("uploadPDF").files;
        if (files == undefined || files == null || files.length == 0) {
            return;
        }
        pdffile = files[0];
        pdffile_url = URL.createObjectURL(pdffile);
        $scope.$iframe.attr('src', pdffile_url);
        $('.section-config').addClass('show');
    };
    ///Evento para ajuste de tamaño del documento
    ///
    $scope.onSlide = function (e) {
        $scope.newForm.TemplateHeight = e.value;
        $scope.changePdfHeight();
    };
    $scope.changePdfHeight = function () {
        $(".section-position").slider({ min: 0, max: $scope.newForm.TemplateHeight - $scope.defaultIframeHeigth });
        $scope.changePdfPosition();
    };
    $scope.changePdfPosition = function () {
        var value = parseInt($scope.sections[$scope.actualSection].Position) + $scope.defaultIframeHeigth;
        if ($scope.rotate) {
            $scope.$iframe.css("top", '-' + ($scope.iframeHeight - $scope.defaultIframeHeigth - $scope.newForm.TemplateHeight + value - 0) + 'px');
        } else {
            $scope.$iframe.css("top", '-' + (value - $scope.defaultIframeHeigth) + 'px');
        }
    }

    $scope.setCustomer = function (customerId, customerName) {
        $scope.client.CustomerId = customerId;
        $scope.client.Name = customerName;
    };
    $scope.isValidForm = function () {
        $scope.formErrors = [];
        $scope.verifyDuplicate();
        angular.forEach($scope.sections, function (section, key) {
            if (section.SectionName == undefined || section.SectionName == '' || section.Position == undefined || section.Fields == undefined || section.Fields.length == 0) {
                $scope.formErrors.push('Sec. ' + (key + 1) + ' incompleta');
            }
            else if (section.isDuplicate == true) {
                $scope.formErrors.push('Sec. ' + (key + 1) + ' duplicada');
            }
        });
        return $scope.formErrors.length == 0;
    };

    $scope.verifyDuplicate = function () {
        var sorted, i;
        sorted = $scope.sections.concat().sort(function (a, b) {
            if (a.SectionName > b.SectionName) return 1;
            if (a.SectionName < b.SectionName) return -1;
            return 0;
        });
        for (i = 0; i < $scope.sections.length; i++) {
            sorted[i].isDuplicate = ((sorted[i - 1] && sorted[i - 1].SectionName == sorted[i].SectionName) || (sorted[i + 1] && sorted[i + 1].SectionName == sorted[i].SectionName));
        }
    };
    $scope.loadFromData = function (formDataId, typingProcessId, formId, name, description, active, pdfHeight) {
        $scope.newForm = { FormId: formId, Name: name, Description: description, Active: active, TemplateHeight: pdfHeight, TypingProcessId: typingProcessId, FormDataId: formDataId }
        $('#iframeHeight').slider('setValue', pdfHeight);
        var model = { formId: formId, typingProcessId: typingProcessId, formDataId: formDataId };
        $http.post(getSectionsForm, model)
         .success(function (d) {
             $('#welcomeModal').modal('show')
                 .on('shown.bs.modal', function () {
                     $('#WelcomeButton').focus();
                 });
             $scope.sections = d.model.Sections;
             $scope.dataLoaded = true;
             $scope.changePdfPosition();
         });
    };

    $scope.saveSection = function () {
        if ($scope.savingSection == true)
        { return; }
        if ($scope.formSection.$invalid) {
            angular.forEach($scope.formSection.$error, function (field) {
                angular.forEach(field, function (errorField) {
                    errorField.$setTouched();
                });
            });
            angular.forEach($scope.formSection.$$success, function (field) {
                angular.forEach(field, function (errorField) {
                    errorField.$setTouched();
                });
            });
            var control = $('input.ng-invalid, select.ng-invalid').first();
            if (control.is('select')) {
                control.parent().children('button').focus();
            } else {
                control.focus();
            }
            return;
        }
        if ($scope.savingSection == true) {
            return;
        }
        $scope.savingSection = true;
        $scope.actualSection++;
        if ($scope.actualSection < $scope.sections.length) {
            if ($scope.sections[$scope.actualSection].IsTable == true) {
                $scope.sectionRows = $scope.sections[$scope.actualSection].NumberOfRows;
            } else {
                $scope.sectionRows = 1;
            }
            $('.section-position').slider('setValue', parseInt($scope.sections[$scope.actualSection].Position));
            $scope.changePdfPosition();
            $scope.actualFocusField = -1;
            $scope.actualFocusRow = 1;
            $scope.$formPanel.scrollTop(0);
        }
        else {
            $('#loadingDialog').modal();
        }
        var sections = [];
        $scope.sections.forEach(function (item, index) {
            sections.push({
                Fields: item.Fields.map(function (field) {
                    return {
                        Value: field.Value
                        , FieldId: field.FieldId
                        , MultipleRowMultiselectValues: field.MultipleRowMultiselectValues
                        , MultipleRowValue: field.MultipleRowValue
                        , FieldTypeId: field.FieldTypeId
                    };
                })
            });
        });

        var model = {
            entity: {
                TypingProcessId: $scope.newForm.TypingProcessId, FormId: $scope.newForm.FormId, FormDataId: $scope.newForm.FormDataId,
                Sections: sections, CompletedSections: $scope.actualSection
            }, closeCapture: $scope.actualSection >= $scope.sections.length
        }
        $http.post(saveFormData, model)
         .success(function (d) {
             $scope.savingSection = false;
             $scope.newForm.FormDataId = d.FormDataId;
             if ($scope.actualSection >= $scope.sections.length) {
                 $('#loadingDialog').modal('toggle');
                 $('#AssignModal').modal({ backdrop: 'static', keyboard: false })
                 .on('shown.bs.modal', function () {
                     $('#ContinueNewProcess').focus(); s
                 });

             }
         });

    };

    $scope.isRequired = function (template, row) {
        if (template.Required == false) {
            return false;
        }
        if (template.Required && $scope.sections[$scope.actualSection].NumberOfRows <= 1) {
            return true;
        }
        var required = false;
        angular.forEach($scope.sections[$scope.actualSection].Fields, function (field, key) {
            ///Verifica que todos los campos sean vacios
            if (field.MultipleRowValue != null) {
                comparerValue = field.MultipleRowValue[row - 1];
            } else if(field.MultipleRowMultiselectValues[row - 1]) {
                comparerValue = field.MultipleRowMultiselectValues[row - 1];
            } else
            {
                field.MultipleRowMultiselectValues[row - 1] = '';
                comparerValue = null;
            }
            if (comparerValue && comparerValue != null && comparerValue.length > 0 && comparerValue[0] != '' ) {
                required = true;
                return;
            }
        });
        return required;
    }

    $scope.getDatePlaceHolder = function (format) {
        if (format == '9999-19-39') {
            return 'yyyy-MM-dd';
        }
        else if (format == '19/39/9999') {
            return 'MM/dd/yyyy';
        }
        else {
            return 'dd/MM/yyyy';
        }
    }

    $scope.changeSelect = function () {

    }
    $scope.assignNewProcess = function () {
        window.location.href = assignNewProcess;
    };
    $scope.goToList = function () {
        window.location.href = assignedForms;
    };

    $scope.init = function () {
        $('body').keydown($scope.keyDown);
        $scope.$formPanel.scrollTop(0);
    };
    $scope.buldFieldClasses = function (isValid, touched, patternError) {
        if (!touched) {
            return 'has-warning has-feedback';
        } else if (isValid && !patternError) {
            return ' has-success has-feedback';
        } else {
            return ' has-error has-feedback';

        }
    };
    $scope.textboxType = function (validation) {
        if (validation == 'email' || validation == 'url') {
            return validation;
        }
        return 'text';
    };
    $scope.buildNumberTitle = function (min, max) {
        var title = '';
        if (min && min > 0)
            title += 'Desde: ' + min;
        if (max && max > 0)
            title += ' Hasta: ' + max;
        return title;
    }
    $scope.init();
});

homeModule.directive('sliderControl', function ($timeout) {
    return {
        restrict: 'A',
        require: 'ngModel', // require ngModel controller
        link: function ($scope, $element, $attr, $ngModelCtrl) {
            $scope.$watch($attr.sliderControl, function () {
                $timeout(function () {
                    $($element).slider({ min: 0, max: $scope.newForm.TemplateHeight - $scope.defaultIframeHeigth })
                    .on('slide', function (ev) {
                        $scope.$iframe.css("top", '-' + ($scope.iframeHeight + ev.value) + 'px');
                        $ngModelCtrl.$setViewValue($element.val());
                        $scope.changePdfPosition();
                    });
                });
            });
        }
    }
});

homeModule.directive("formControl", function ($timeout) {
    return {
        require: '?ngModel',
        $scope: {
            template: '=?',
            row: '=?'
        },
        link: function ($scope, $element, $attr, $ngModelCtrl) {
            var validateTyping = function (comparerValue) {
                var value1, value2;
                if ($scope.template.MultipleRowValue1 != null) {
                    value1 = $scope.template.MultipleRowValue1[$scope.row - 1];
                    value2 = $scope.template.MultipleRowValue2[$scope.row - 1];
                } else {
                    value1 = $scope.template.MultipleRowMultiselectValues1[$scope.row - 1];
                    value2 = $scope.template.MultipleRowMultiselectValues2[$scope.row - 1];
                    comparerValue = $scope.template.MultipleRowMultiselectValues[$scope.row - 1]
                }
                if (value1 == null && value2 == null)
                {
                    $ngModelCtrl.$setValidity('typingValidator', true);
                }
                else if (value1 != comparerValue && value2 != comparerValue) {
                    $ngModelCtrl.$setValidity('typingValidator', false);
                }
                else {
                    $ngModelCtrl.$setValidity('typingValidator', true);
                }
            }


            $ngModelCtrl.$parsers.push(function (value, value1) {
                if ($($element).is('input') && $($element).hasClass('autocomplete') && (value == '')) {
                    $ngModelCtrl.$setValidity('typingValidator',true);
                    return value;
                }
                if ($scope.template.Value1 == null && $scope.template.Value2 == null) {
                    $ngModelCtrl.$setValidity('typingValidator', true);
                    return value;
                }
                validateTyping(value);
                return value;
            });
            $scope.$watch($attr.formControl, function () {
                $timeout(function () {
                    $($element).tooltip({ 'trigger': 'focus' });
                    $scope.$formPanel[0].scrollTop = $scope.$formPanel[0].scrollHeight + 100;
                    if ($($element).is('input') && $($element).hasClass('autocomplete')) {

                        var result = $.grep($scope.template.OptionList, function (e) { return e.value == $scope.template.MultipleRowMultiselectValues[$scope.row - 1]; })
                        if (result.length > 0) {
                            $scope.template.MultipleRowMultiselectLabels[$scope.row - 1] = result[0].label;
                        };
                        $($element).autocomplete({
                            //minLength: 0,
                            source: $scope.template.OptionList,
                            select: function (event, ui) {
                                $($element).val(ui.item.label);
                                $scope.template.MultipleRowMultiselectLabels[$scope.row - 1] = ui.item.label;
                                $scope.template.MultipleRowMultiselectValues[$scope.row - 1] = [ui.item.value];
                                validateTyping(ui.item.value);
                                $scope.nextField();
                                return false;
                            },
                            search: function (event, ui) {
                                $ngModelCtrl.$setValidity('typingValidator', false);
                                $scope.template.MultipleRowMultiselectLabels[$scope.row - 1] = "";
                                $scope.template.MultipleRowMultiselectValues[$scope.row - 1] = "";
                            }
                        }).autocomplete("instance")._renderItem = function (ul, item) {
                            return $("<li>")
                              .append("<div>" + item.label + "</div>")
                              .appendTo(ul);
                        };
                    }
                });
            });
        }
    };
});

homeModule.directive('validNumber', function () {
    return {
        require: '?ngModel',
        link: function (scope, element, attrs, ngModelCtrl) {
            if (!ngModelCtrl) {
                return;
            }

            ngModelCtrl.$parsers.push(function (val) {
                if (angular.isUndefined(val)) {
                    var val = '';
                }

                var clean = val.replace(/[^-0-9\.]/g, '');
                var negativeCheck = clean.split('-');
                var decimalCheck = clean.split('.');
                if (!angular.isUndefined(negativeCheck[1])) {
                    negativeCheck[1] = negativeCheck[1].slice(0, negativeCheck[1].length);
                    clean = negativeCheck[0] + '-' + negativeCheck[1];
                    if (negativeCheck[0].length > 0) {
                        clean = negativeCheck[0];
                    }

                }

                if (!angular.isUndefined(decimalCheck[1])) {
                    decimalCheck[1] = decimalCheck[1].slice(0, 2);
                    clean = decimalCheck[0] + '.' + decimalCheck[1];
                }

                if (val !== clean) {
                    ngModelCtrl.$setViewValue(clean);
                    ngModelCtrl.$render();
                }
                return clean;
            });

            element.bind('keypress', function (event) {
                if (event.keyCode === 32) {
                    event.preventDefault();
                }
            });
        }
    };
});
homeModule.directive('ngMin', function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, elem, attr, ctrl) {
            var minValidator = function (value) {
                var min = attr.ngMin || 0;
                if (!isEmpty(value) && value < min) {
                    ctrl.$setValidity('ngMin', false);
                    return undefined;
                } else {
                    ctrl.$setValidity('ngMin', true);
                    return value;
                }
            };
            ctrl.$parsers.push(minValidator);
        }
    };
});

homeModule.directive('ngMax', function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, elem, attr, ctrl) {
            var maxValidator = function (value) {
                var max = attr.ngMax || Infinity;
                if (!isEmpty(value) && value > max) {
                    ctrl.$setValidity('ngMax', false);
                    return undefined;
                } else {
                    ctrl.$setValidity('ngMax', true);
                    return value;
                }
            };

            ctrl.$parsers.push(maxValidator);
        }
    };
});


homeModule.directive('validDate', function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, elem, attr, ctrl) {
            var validDate = function (value) {
                if (value.length < 10) {
                    return value;
                }
                var format = elem.attr('data-date-format');
                var date;
                var splitDate = value.split('/');
                var dd = 0;
                var mm = 0;
                var yy = 0;
                if (format == '9999-19-39') {
                    //date = new Date(value);
                    splitDate = value.split('-');
                    dd = parseInt(splitDate[2]);
                    mm = parseInt(splitDate[1]);
                    yy = parseInt(splitDate[0]);
                }
                else if (format == '19/39/9999') {
                    //    date = new Date(splitDate[2] + '-' + splitDate[0] + splitDate[1] + '-');
                    dd = parseInt(splitDate[1]);
                    mm = parseInt(splitDate[0]);
                    yy = parseInt(splitDate[2]);
                }
                else {
                    //    date = new Date(splitDate[2] + '-' + splitDate[1] + '-' + splitDate[0]);
                    dd = parseInt(splitDate[0]);
                    mm = parseInt(splitDate[1]);
                    yy = parseInt(splitDate[2]);
                }

                if (yy < 1800) {
                    ctrl.$setValidity('validateDate', false);
                    return undefined;
                }
                if (mm > 12 || mm < 1) {
                    ctrl.$setValidity('validateDate', false);
                    return undefined;
                }
                // Create list of days of a month [assume there is no leap year by default]  
                var ListofDays = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];
                if (mm == 1 || mm > 2) {
                    if (dd > ListofDays[mm - 1]) {
                        ctrl.$setValidity('validateDate', false);
                        return undefined;
                    }
                }
                if (mm == 2) {
                    var lyear = false;
                    if ((!(yy % 4) && yy % 100) || !(yy % 400)) {
                        lyear = true;
                    }
                    if ((lyear == false) && (dd >= 29)) {
                        ctrl.$setValidity('validateDate', false);
                        return undefined;
                    }
                    if ((lyear == true) && (dd > 29)) {
                        ctrl.$setValidity('validateDate', false);
                        return undefined;
                    }
                }
                ctrl.$setValidity('validateDate', true);
                return value;
            };
            ctrl.$parsers.push(validDate);
        }
    };
});

function isEmpty(strIn) {
    if (strIn === undefined) {
        return true;
    }
    else if (strIn == null) {
        return true;
    }
    else if (strIn == "") {
        return true;
    }
    else {
        return false;
    }
}