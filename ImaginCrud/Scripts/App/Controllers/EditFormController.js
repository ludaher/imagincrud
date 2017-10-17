var homeModule = angular.module("Form", ['ui.bootstrap', 'ngDraggable']).service('LoadingInterceptor', ['$q', '$rootScope', '$log', LoadingInterceptor])
.config(['$httpProvider', function ($httpProvider) { $httpProvider.interceptors.push('LoadingInterceptor'); }]);


homeModule.controller("FormController", function ($scope, $http) {

    $scope.validations = [
        { Value: '', Text: 'Ninguno' },
        { Value: 'email', Text: 'Email' },
        { Value: 'url', Text: 'Url' }
    ];
    $scope.client = { Name: '', CustomerId: 0, IdentificationType: 'NIT' };
    $scope.defaultIframeHeigth = 400;
    $scope.iframeHeight = 15000;
    $scope.newForm = { Name: "", Active: true, TemplateHeight: 2000 };
    $scope.sections = [];
    $scope.formErrors = [];
    $scope.actualSection = -1;
    $scope.actualField = 0;
    $scope.rotate = false;
    $scope.viewSection = false;
    $scope.viewProperties = false;
    $scope.viewActionsField = false;
    $scope.dataLoaded = false;
    $scope.configuration = { basicData: true, sections: new Array() };
    $scope.$iframe = $('#viewer');
    $scope.$formPanel = $('#formPanel');
    $scope.actionsFieldPosX = 0;
    $scope.actionsFieldPosY = 0;
    $scope.formTemplates = [
       {
           Id: 1,
           "Title": "Número",
           "FieldTypeId": "1",
           "MaxLength": 15,
           "Required": true,
           "Size": 3,
           "MinNumber": 0,
           "MaxNumber": 10,
           "Validation": 'ng-min="0" ng-max="10"'
       },
      {
          Id: 2,
          "FieldTypeId": "2",
          "Title": "Texto",
          "Size": 3,
          "MaxLength": 50,
          "Required": true,
          "validation": {
          }
      },
      {
          Id: 4,
          "FieldTypeId": "3",
          "Title": "Checkbox",
          "Required": true,
          "Size": 3
      },
      {
          Id: 5,
          "FieldTypeId": "4",
          "Title": "Selección",
          "Required": true,
          "Size": 3,
          "OptionList": []
      },
      {
          Id: 6,
          "FieldTypeId": "5",
          "Title": "Múltiple",
          "Required": true,
          "Size": 3,
          "OptionList": []
      },
      {
          Id: 3,
          "FieldTypeId": "6",
          "Title": "Fecha",
          "Size": 3,
          "Required": true,
          "MaxLength": 50,
          "validation": {
              "format": "dd/MM/yyyy"
          }
      }, {
          Id: 0,
          "Title": "Nueva fila",
          "FieldTypeId": "7",
          "Size": 3
      }];
    $scope.dropedTemplates = [];
    $scope.addOptionInForm = function () {
        $scope.sections[$scope.actualSection].Fields[$scope.actualField].OptionList.push({});
    }
    $scope.removeOptionInForm = function (option) {
        var index = $scope.sections[$scope.actualSection].Fields[$scope.actualField].OptionList.indexOf(option);
        $scope.sections[$scope.actualSection].Fields[$scope.actualField].OptionList.splice(index, 1);
        $scope.isValidForm();
    }
    $scope.onDropComplete1 = function (data, evt) {
        if (data == undefined) { return false; }
        if ($scope.sections[$scope.actualSection].Fields == undefined) { $scope.sections[$scope.actualSection].Fields = new Array(); }
        var newItemId = $scope.sections[$scope.actualSection].Fields.length;
        data.Id = newItemId;
        //data.FieldName = 'Field' + newItemId;
        $scope.sections[$scope.actualSection].Fields.push(angular.copy(data));
        $scope.isValidForm();
    };
    $scope.viewPropertiesForm = function (data, event) {
        $scope.actualField = $scope.sections[$scope.actualSection].Fields.indexOf(data);
        $scope.viewProperties = true;
    };
    $scope.viewPropertiesForm2 = function () {
        $scope.viewProperties = true;
    };
    $scope.toggleActionsField = function (data, event) {
        var actualSelection = $scope.actualField;
        $scope.actualField = $scope.sections[$scope.actualSection].Fields.indexOf(data);
        $scope.viewActionsField = true;
        $scope.actionsFieldPosX = event.clientX;
        $scope.actionsFieldPosY = event.clientY;
    };
    $scope.clickBody = function (evt) {
        var target = evt.target || evt.srcElement;
        if (target.className.includes("edit-control") || target.className.includes("navbar-btn") || $(target).parents('.edit-control').length)
            return;
        $scope.viewActionsField = false;
    };
    $scope.loadNewFile = function () {
        var files = document.getElementById("uploadPDF").files;
        if (files == undefined || files == null || files.length == 0) {
            return;
        }
        var formData = new FormData();
        formData.append("file", files[0]);
        formData.append("fileName", $scope.newForm.TemplatePath);
        formData.append("formId", $scope.newForm.FormId);
        //We can send more data to server using append         
        $http.post(saveFile, formData,
             {
                 withCredentials: true,
                 headers: { 'Content-Type': undefined },
                 transformRequest: angular.identity
             })
         .success(function (d) {
             $scope.$iframe.attr('data', pdfUrl);
         });
    };
    $scope.hidePropertiesForm = function (data, event) {
        $scope.optionToAdd = {};
        $scope.viewProperties = false;
        $scope.viewActionsField = false;
    };
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
        $scope.isValidForm();
    };
    $scope.isDataListSelect = function (select) {
        return sections[actualSection].Fields[actualField].DataList == select;
    }

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
        $scope.isValidForm();
    };
    $scope.isMultipleOption = function (field) {
        return field == '4' || field == '5';
    };
    $scope.isTextBox = function (field) {
        return field == '1' || field == '2';
    };
    $scope.isOnlyTextBox = function (field) {
        return field == '2';
    };
    $scope.isNumberTextBox = function (field) {
        return field == '1';
    };
    $scope.isDateTextBox = function (field) {
        return field == '6';
    };
    ///Función que permite agregar sección
    ///
    $scope.addSection = function () {
        var newItemNo = $scope.sections.length;
        $scope.sections.push({ 'Id': +newItemNo, IsTable: false, NumberOfRows: 0, Fields: [], FormId: $scope.newForm.FormId });
        $scope.isValidForm();
        $scope.actualSection = newItemNo;
        $scope.viewSection = true;
        //$scope.loadSliderInSections();
    };

    $scope.setActiveSection = function (index) {
        return $scope.actualSection == index ? 'active' : '';
    }

    $scope.removeSection = function (section) {
        var index = $scope.sections.indexOf(section);
        $scope.actualSection = -1;
        $scope.viewSection = false;
        $scope.sections.splice(index, 1);
        $scope.isValidForm();
    };
    ///Función que carga la información básica
    ///
    $scope.loadBasicData = function () {
        $scope.viewActionsField = false;
        $scope.viewSection = false;
        $scope.actualSection = -1;
        $scope.changePdfPosition();
    };
    ///Función que carga la sección en el formulario de edición
    ///srcElement
    $scope.loadSection = function (section, evt) {
        var target = evt.target || evt.srcElement;
        if (target.className.includes("closeTab")) {
            return removeSection(section);
        }
        $scope.saveSection(function () {
            var index = $scope.sections.indexOf(section);
            $scope.actualSection = index;
            $scope.viewSection = index != -1;
            $('.section-position').slider('setValue', parseInt(section.Position));
            $scope.changePdfPosition();
        }); 
    };
    $scope.indexOfSection = function (section) {
        var index = $scope.sections.indexOf(section);
        return index;
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
        var value = 0;
        if ($scope.viewSection == false) {
            value = $scope.newForm.TemplateHeight;
        } else {
            value = parseInt($scope.sections[$scope.actualSection].Position) + $scope.defaultIframeHeigth;
        }
        if ($scope.rotate) {
            $scope.$iframe.css("top", '-' + ($scope.iframeHeight - value) + 'px');
        } else {
            $scope.$iframe.css("top", '-' + (value - $scope.defaultIframeHeigth) + 'px');
        }
    }
    ///Inicializa el control que permite ajustar el tamaño del documento
    ///
    $scope.initHeightDocumentConfig = function () {
        $scope.$slider = $('#iframeHeight').slider({
            scale: 'logarithmic',
            min: 400, max: $scope.iframeHeight
        }).on('slideStop', $scope.onSlide).on('slide', $scope.onSlide);


    };
    $scope.setCustomer = function (customerId, customerName) {
        $scope.client.CustomerId = customerId;
        $scope.client.Name = customerName;
    };
    $scope.isValidForm = function () {
        $scope.formErrors = [];
        $scope.verifyDuplicate();
        if (!$scope.form.$valid || $scope.newForm.TemplateHeight == undefined) {
            $scope.formErrors.push("Información básica incompleta");
        }
        angular.forEach($scope.sections, function (section, key) {
            if (section.SectionName == undefined || section.SectionName == '' || section.Position == undefined || section.Fields == undefined || section.Fields.length == 0 || (section.IsTable == true && section.NumberOfRows <= 0)) {
                $scope.formErrors.push('Sec. ' + (key + 1) + ' incompleta');
            }
            else if (section.isDuplicate == true) {
                $scope.formErrors.push('Sec. ' + (key + 1) + ' duplicada');
            }
            if (section.IsTable == true) {
                var totalPosition = 0;
                for (var i = 0, len = section.Fields.length; i < len; i++) {
                    totalPosition += section.Fields[i].Size;  // Iterate over your first array and then grab the second element add the values up
                }
                if (totalPosition != 12) {
                    $scope.formErrors.push('Sec. ' + (key + 1) + ' debe tener solo una fila completa');
                }
            }
        });
        return $scope.formErrors.length == 0;
    };
    $scope.saveFormData = function (afterSave) {

        if (!$scope.isValidForm()) {
            return false;
        }
        var model = { form: $scope.newForm, sections: $scope.sections };
        //$http.post(saveAllForm, model)
        // .success(function (d) {
        // });
        $http({
            url: saveAllForm,
            method: "POST",
            data: model,
        })
       .success(function (data, status, headers, config) {
           if (afterSave) {
               afterSave();
           }else{
               window.location.href = editCustomer + '/' + data.CustomerId;
           }
       }).error(function (data, status, headers, config) {

       });

    };

    $scope.saveSection= function (afterSave) {

        if (!$scope.isValidForm()) {
            return false;
        }
        if ($scope.actualSection < 0) {
            var model = { form: $scope.newForm, sections: null };
            //$http.post(saveAllForm, model)
            // .success(function (d) {
            // });
            $http({
                url: saveAllForm,
                method: "POST",
                data: model,
            }).success(function (data, status, headers, config) {
                if (afterSave) {
                    afterSave();
                }
            });
            return;
        }
        var model = { model: $scope.sections[$scope.actualSection] };
        //$http.post(saveAllForm, model)
        // .success(function (d) {
        // });
        $http({
            url: saveSection,
            method: "POST",
            data: model,
        })
       .success(function (data, status, headers, config) {
           if (afterSave) {
               afterSave();
           } else {
               window.location.href = editCustomer + '/' + data.CustomerId;
           }
       }).error(function (data, status, headers, config) {

       });

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
    $scope.loadFromData = function (formId, name, description, active, pdfHeight, templateName, requiredCaptures) {
        $scope.newForm = { FormId: formId, Name: name, Description: description, Active: active, TemplateHeight: pdfHeight, TemplatePath: templateName, RequiredCaptures: requiredCaptures }
        $('#iframeHeight').slider('setValue', pdfHeight);
        var model = { formId: formId };
        $scope.changePdfPosition();
        $http.post(getSectionsForm, model)
         .success(function (d) {
             $scope.sections = d.sections;
             $scope.dataLoaded = true;
         });
    };
    $scope.changeNumberValidation = function () {
        $scope.sections[$scope.actualSection].Fields[$scope.actualField].Validation = "ng-min = '" +
        $scope.sections[$scope.actualSection].Fields[$scope.actualField].MinNumber +
        "' ng-max = '"
        + $scope.sections[$scope.actualSection].Fields[$scope.actualField].MaxNumber + "' ";
    }

    $scope.initHeightDocumentConfig();
    $('footer').remove();
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
                        $scope.isValidForm();
                        $scope.changePdfPosition();
                    });
                });
            });
        }
    }
});

homeModule.directive("akFileModel", ["$parse",
                function ($parse) {
                    return {
                        restrict: "A",
                        link: function (scope, element, attrs) {
                            var model = $parse(attrs.akFileModel);
                            var modelSetter = model.assign;
                            element.bind("change", function () {
                                scope.$apply(function () {
                                    modelSetter(scope, element[0].files[0]);
                                });
                            });
                        }
                    };
                }]);

homeModule.directive("formControl", function ($timeout) {
    return {
        restrict: "A",
        link: function ($scope, $element, $attr) {
            $scope.$watch($attr.formControl, function () {
                $timeout(function () {
                    $scope.$formPanel[0].scrollTop = $scope.$formPanel[0].scrollHeight + 100;
                });
            });
        }
    };
});


homeModule.directive("selectControl", function ($timeout) {
    return {
        restrict: "A",
        link: function ($scope, $element, $attr) {
            $scope.$watch($attr.selectControl, function () {
                $timeout(function () {
                    $($element).selectpicker();
                });
            });
        }
    };
});

homeModule.directive("akFileModel", ["$parse",
                function ($parse) {
                    return {
                        restrict: "A",
                        link: function (scope, element, attrs) {
                            var model = $parse(attrs.akFileModel);
                            var modelSetter = model.assign;
                            element.bind("change", function () {
                                scope.loadNewFile();
                            });
                        }
                    };
                }]);

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