(function () {
    "use strict";

    angular.module("app").controller("mainFooterController", function ($scope, $mdDialog) {
        //
        // Show About Us
        $scope.showAboutUsPopup = function (target) {
            $mdDialog.show({
                data: {},
                controller: function($scope, $mdDialog, data) {
                    $scope.cancel = function() { $mdDialog.cancel(); };
                },
                templateUrl: '/App/views/about-us-dialog.html',
                parent: angular.element(document.body),
                targetEvent: target,
                clickOutsideToClose: true,
                fullscreen: window.outerWidth <= 550,
            });
        };
    });
})();