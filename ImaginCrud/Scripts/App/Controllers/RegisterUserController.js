var homeModule = angular.module("Form", ['ui.bootstrap']);

homeModule.controller("FormController", function ($scope, $http) {
    $scope.loadModel = function (UserName, Email, Password, ConfirmPassword) {
        var user = $scope.user = {};
        user.UserName = UserName;
        user.Email = Email;
        user.Password = Password;
        user.ConfirmPassword = ConfirmPassword;

    };
});

homeModule.directive("passwordVerify", function () {
    return {
        require: "ngModel",
        scope: {
            passwordVerify: '='
        },
        link: function (scope, element, attrs, ctrl) {
            scope.$watch(function () {
                var combined;

                if (scope.passwordVerify || ctrl.$viewValue) {
                    combined = scope.passwordVerify + '_' + ctrl.$viewValue;
                }
                return combined;
            }, function (value) {
                if (value) {
                    ctrl.$parsers.unshift(function (viewValue) {
                        var origin = scope.passwordVerify;
                        if (origin !== viewValue) {
                            ctrl.$setValidity("passwordVerify", false);
                            return undefined;
                        } else {
                            ctrl.$setValidity("passwordVerify", true);
                            return viewValue;
                        }
                    });
                }
            });
        }
    };
});

