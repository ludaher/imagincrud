﻿var homeModule = angular.module("Form", ['ui.bootstrap']).service('LoadingInterceptor', ['$q', '$rootScope', '$log', LoadingInterceptor])
.config(['$httpProvider', function ($httpProvider) {$httpProvider.interceptors.push('LoadingInterceptor');}]);;

homeModule.controller("FormController", function ($scope, $http) {
    $scope.seachEntity = { UserName: '', Email: '', TotalPages: 0, Page: 1, ItemsByPage: "10",
        SortOrder: '', Descendant: false
    };
    $scope.setFormId = function (formId) {
        $scope.formId = formId;
        $scope.find();
    }

    ///Función que realiza la busqueda
    ///
    $scope.find = function () {
        var formData = {
            searchModel: {
                TotalPages: $scope.seachEntity.TotalPages, Page: $scope.seachEntity.Page, ItemsByPage: $scope.seachEntity.ItemsByPage,
                SortOrder: $scope.seachEntity.SortOrder, Descendant: $scope.seachEntity.Descendant,
                EntityToFind: {
                    UserName: $scope.seachEntity.UserName,
                    FormId: $scope.formId
                }
            }
        };
        $http.post(getUsersView, formData)
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
    }
    $scope.next = function () {
        $scope.seachEntity.Page = $scope.seachEntity.Page + 1;
        $scope.find();
    }
    $scope.previous = function () {
        $scope.seachEntity.Page = $scope.seachEntity.Page - 1;
        $scope.find();
    }
    $scope.findWithParameters = function () {
        $scope.seachEntity.Page = 1;
        $scope.find();
    }
    $scope.clickTyper = function (userName) {
        var formData = {
            userName: userName,
            formId: $scope.formId
        };
        $http.post(changeTypistAssignation, formData)
        .success(function (result) {
            $('.load').removeClass('active');
            if (result.Status == 3) {
                AlertUI(result.UserMessage, "error");
            }
        });
    }
    $scope.clickValidator = function (userName) {
        var formData = {
            userName: userName,
            formId: $scope.formId
        };
        $http.post(changeValidatorAssignation, formData)
        .success(function (result) {
            $('.load').removeClass('active');
            if (result.Status == 3) {
                AlertUI(result.UserMessage, "error");
            }
        });
    }
    $scope.changeOrder = function (sortColumn) {
        $scope.seachEntity.SortOrder = sortColumn;
        $scope.seachEntity.Descendant = !$scope.seachEntity.Descendant;
        $scope.find();
    }
    $scope.showOrderIcon = function (columnName, isDescendant) {
        return $scope.seachEntity.SortOrder == columnName && $scope.seachEntity.Descendant == isDescendant;
    }
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