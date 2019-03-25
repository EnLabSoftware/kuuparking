(function() {
    "use strict";

    angular.module("app").controller("mainHeaderController", function ($rootScope, $scope, $state, $mdDialog, baseService, userService, providerService) {
        $scope.isNavigationOpen = false;
        $scope.toggleNavigation = function () { $scope.isNavigationOpen = !$scope.isNavigationOpen; };
        $scope.userInfo = undefined;
        $scope.defaultAvatar = userService.defaultAvatar;
        $scope.currentState = "";
        $rootScope.$on('$stateChangeSuccess', function (event, toState, toParams, fromState, fromParams) {
            $scope.currentState = toState.name;
        });

        userService.getCurrentUser().then(function (userInfo) {
            $scope.userInfo = userInfo;
        });
        $scope.goToDashboard = function () { providerService.goToDashboard(); };
        $scope.goToParkingLot = function () { providerService.goToParkingLot(); };
        $scope.goToReport = function () { providerService.goToReport(); };
        $scope.goToProfile = function () { providerService.goToProfile(); };
        $scope.goToActivation = function () { providerService.goToActivation(); };
        $scope.signout = function () {
            baseService.signOut();
            $scope.userInfo = undefined;
            $scope.isNavigationOpen = false;
            // Go to login when current page is not home
            if (window.location.pathname !== "/") {
                baseService.goToSignin();
            }
        };

        // Search term
        $scope.searchTerm = "";

        $scope.clearSearchTerm = function() {
            $scope.searchTerm = "";
            $rootScope.$broadcast('clearSearchTerm');
        };

        // Search focus in
        $scope.textSearchFocus = function() { $scope.searchFocused = true; };

        // Search focus out
        $scope.textSearchBlur = function () { $scope.searchFocused = false; };
    });
})();