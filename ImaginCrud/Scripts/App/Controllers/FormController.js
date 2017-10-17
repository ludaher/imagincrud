var homeModule = angular.module("Form", ['ui.bootstrap']).service('LoadingInterceptor', ['$q', '$rootScope', '$log', LoadingInterceptor])
.config(['$httpProvider', function ($httpProvider) { $httpProvider.interceptors.push('LoadingInterceptor'); }]);


homeModule.controller("FormController", function ($scope, $http) {
    $scope.client = { Name: '', CustomerId: 0, IdentificationType: 'NIT' };
    $scope.defaultIframeHeigth = 400;
    $scope.newForm = { Name: "", Active: true, PdfHeight: 1000 };
    $scope.sections = [];
    $scope.configuration = { basicData: true, sections: new Array() };
    $scope.$iframe = $('#viewer');
    ///Función que permite agregar sección
    ///
    $scope.addSection = function () {
        var newItemNo = $scope.sections.length;
        $scope.sections.push({ 'Id': +newItemNo });
        //$scope.loadSliderInSections();
    };
    ///Vista previa de la imagen
    ///
    $scope.PreviewImage = function () {
        var files = document.getElementById("uploadPDF").files;
        if (files == undefined || files == null || files.length == 0) {
            return;
        }
        var formData = new FormData();
        formData.append("file", files[0]);
        //We can send more data to server using append         
        $http.post(loadFile, formData,
             {
                 withCredentials: true,
                 headers: { 'Content-Type': undefined },
                 transformRequest: angular.identity
             })
         .success(function (d) {
             $scope.$iframe.attr('src', pdfUrl);
             $('.section-config').addClass('show');
         });
    };
    ///Evento para ajuste de tamaño del documento
    ///
    $scope.onSlide = function (e) {
        $scope.newForm.PdfHeight = e.value;
        $scope.changePdfHeight(e.value);
    };

    $scope.changePdfHeight = function (value) {
        /*$scope.$iframe.height(value);*/
        $scope.$iframe.css("top", '-' + (value - $scope.defaultIframeHeigth) + 'px')
        $(".section-position").slider({ min: 60, max: $scope.newForm.PdfHeight - $scope.defaultIframeHeigth });
        $(".section-position").slider('setValue', 60);
    };

    ///Inicializa el control que permite ajustar el tamaño del documento
    ///
    $scope.initHeightDocumentConfig = function () {
        $(function () {
            // wait till load event fires so all resources are available
            $scope.$slider = $('#iframeHeight').slider({
                scale: 'logarithmic'
            }).on('slideStop', $scope.onSlide).on('slide', $scope.onSlide);
        });

    };
    $scope.setCustomer = function (customerId, customerName) {
        $scope.client.CustomerId = customerId;
        $scope.client.Name = customerName;
    };

    $scope.saveForm = function (valid) {
        $scope.newForm.Sections = $scope.sections;
        var model = { form: $scope.newForm };
        model.form.CustomerId = $scope.client.CustomerId;
        $http.post(saveForm, model)
         .success(function (d) {
             window.location.href = editForm + '/' + d.FormId;
         });
    };

    $scope.verifyDuplicate = function () {
        var sorted, i;
        sorted = $scope.sections.concat().sort(function (a, b) {
            if (a.Name > b.Name) return 1;
            if (a.Name < b.Name) return -1;
            return 0;
        });
        for (i = 0; i < $scope.sections.length; i++) {
            sorted[i].isDuplicate = ((sorted[i - 1] && sorted[i - 1].Name == sorted[i].Name) || (sorted[i + 1] && sorted[i + 1].Name == sorted[i].Name));
        }
    };

    $scope.initHeightDocumentConfig();
});

homeModule.directive('sliderControl', function ($timeout) {
    return {
        restrict: 'A',
        require: 'ngModel', // require ngModel controller
        link: function ($scope, $element, $attr, $ngModelCtrl) {
            $scope.$watch($attr.sliderControl, function () {
                $timeout(function () {
                    $($element).slider({ min: 60, max: $scope.newForm.PdfHeight - $scope.defaultIframeHeigth })
                    .on('slide', function (ev) {
                        $scope.$iframe.css("top", '-' + ev.value + 'px');
                        $ngModelCtrl.$setViewValue($element.val());
                        /*$(ev.currentTarget).trigger('input');*/
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
                                scope.PreviewImage();
                                //scope.$apply(function () {
                                //    modelSetter(scope, element[0].files[0]);
                                //});
                            });
                        }
                    };
                }]);

