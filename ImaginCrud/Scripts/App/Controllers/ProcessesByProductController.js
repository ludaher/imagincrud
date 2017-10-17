var homeModule = angular.module("Form", ['ui.bootstrap']).service('LoadingInterceptor', ['$q', '$rootScope', '$log', LoadingInterceptor])
.config(['$httpProvider', function ($httpProvider) { $httpProvider.interceptors.push('LoadingInterceptor'); }]);;

homeModule.controller("FormController", function ($scope, $http) {
    $scope.seachEntity = {
        Name: '', TotalPages: 0, Page: 1, ItemsByPage: "10",
        SortOrder: '', Descendant: false, SelectedField: -1
    };
    ///Función que realiza la busqueda
    ///
    $scope.find = function () {
        var fields = [];
        fields.push($scope.template);
        var formData = {
            searchModel: {
                TotalPages: $scope.seachEntity.TotalPages, Page: $scope.seachEntity.Page, ItemsByPage: $scope.seachEntity.ItemsByPage,
                SortOrder: $scope.seachEntity.SortOrder, Descendant: $scope.seachEntity.Descendant,
                EntityToFind: {
                    Name: $scope.seachEntity.Name,
                    FormId: $scope.seachEntity.FormId,
                    CustomerId: $scope.seachEntity.CustomerId,
                    FieldsToSearch: fields
                }
            }
        };
        $http.post(getFormsDataView, formData)
         .success(function (result) {
             $('.load').removeClass('active');
             if (result.Status == 3) {
                 AlertUI(result.UserMessage, "error");
             }
             else {
                 $scope.listHtml = result.partialView;
                 $scope.seachEntity.TotalPages = result.totalPages;
             }
         })
    };
    $scope.next = function () {
        $scope.seachEntity.Page = $scope.seachEntity.Page + 1;
        $scope.find();
    }
    $scope.previous = function () {
        $scope.seachEntity.Page = $scope.seachEntity.Page - 1;
        $scope.find();
    };
    $scope.findWithParameters = function () {
        $scope.seachEntity.Page = 1;
        $scope.find();
    };
    $scope.changeOrder = function (sortColumn) {
        $scope.seachEntity.SortOrder = sortColumn;
        $scope.seachEntity.Descendant = !$scope.seachEntity.Descendant;
        $scope.find();
    };
    $scope.showOrderIcon = function (columnName, isDescendant) {
        return $scope.seachEntity.SortOrder == columnName && $scope.seachEntity.Descendant == isDescendant;
    };
    $scope.loadFromData = function (formId, formName) {
        $scope.seachEntity.FormId = formId;
        $scope.find();
        //$scope.seachEntity.Form.Name = formName;
    };
    $scope.loadFields = function () {
        var model = { id: $scope.seachEntity.SelectedSection };
        $http.post(getSectionFields, model)
         .success(function (d) {
             $scope.fields = d.fields;
             //$scope.seachEntity.SelectedField = -1;
         });
    };
    $scope.changeField = function () {
        $scope.template = $scope.fields[$scope.seachEntity.SelectedField];
    };
    $scope.viewHistory = function (formId, processId, name) {
        var model = { id:formId, processId: processId };
        $http.post(getHistory, model)
         .success(function (d) {
             $scope.history = d.history;
             $('#HistoryModal').modal();
         });
    };
});

homeModule.directive("formControl", function ($timeout) {
    return {
        require: '?ngModel',
        link: function ($scope, $element, $attr, $ngModelCtrl) {
            $scope.$watch($attr.formControl, function () {
                $timeout(function () {
                    $($element).tooltip({ 'trigger': 'focus' });
                    if ($($element).is('select')) {
                        $($element).selectpicker('destroy').selectpicker({ size: 4, liveSearch: true, noneSelectedText: 'Sin selección', dropupAuto: false });
                    }
                });
            });
        }
    };
});

homeModule.directive('dynamic', function ($compile) {
    return {
        restrict: 'A',
        replace: true,
        link: function (scope, ele, attrs) {
            scope.$watch(attrs.dynamic, function (listHtml) {
                ele.html(listHtml);
                $compile(ele.contents())(scope);
            });
        }
    };
});