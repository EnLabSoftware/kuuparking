(function () {
    "use strict";

    angular.module("app").controller("aboutUsController", function ($scope, providerService) {
        providerService.getSlotProvidersInState().then(function(data) {
            $scope.providersInState = data;
        });
    });
})();