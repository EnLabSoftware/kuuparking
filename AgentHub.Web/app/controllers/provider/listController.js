(function () {
    "use strict";
    angular.module("app").controller("slotProviderListController", function ($scope, $timeout, $location, baseService, providerService) {
        $scope.model = {};
        $scope.loading = true;
        var queryParams = baseService.parseQueryString();
        var stateId = queryParams.stateId;
        var cityId = queryParams.cityId;

        providerService.getSlotProvidersInPlace(stateId, cityId).then(function (data) {
            $scope.model = data;
            $scope.loading = false;
            //
            // Page title
            if ($scope.model.City || $scope.model.State) {
                var title = "Bãi đậu xe tại ";
                if ($scope.model.City)
                    title += $scope.model.City.Name + ", ";

                if ($scope.model.State)
                    title += $scope.model.State.Name;

                window.document.title = title + " - KuuParking";
            }
        });
    });
})();