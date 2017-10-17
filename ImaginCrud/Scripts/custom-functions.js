var LoadingInterceptor = function ($q, $rootScope, $log) {
    'use strict';
    return {
        request: function (config) {
            $('#loadingDialog').modal();
            return config;
        },
        requestError: function (rejection) {
            $('#loadingDialog').modal('toggle');
            $log.error('Error:', rejection);
            alert("Ocurrió un error, contacte al administrador.");
            //return $q.reject(rejection);
        },
        response: function (response) {
            $('#loadingDialog').modal('toggle');
            return response;
        },
        responseError: function (rejection) {
            $('#loadingDialog').modal('toggle');
            $log.error('Response error:', rejection);
            alert("Ocurrió un error, contacte al administrador.");
            //return $q.reject(rejection);
        }
    };
};