(function() {
    "use strict";
    angular.module("app").controller("providerController", function ($http, $q, $rootScope, $scope, $state, $cookies, baseService, userService, providerService) {
        $scope.currentState = "";
        $rootScope.$on('$stateChangeSuccess', function(event, toState, toParams, fromState, fromParams) {
            $scope.currentState = toState.name;
        });
    });
})();