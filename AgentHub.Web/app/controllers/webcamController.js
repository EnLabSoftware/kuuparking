(function () {
    "use strict";

    angular.module("app").controller("webcamController", function ($scope, $mdDialog) {
        $scope.btnGetImageText = "Chụp ảnh";
        $scope.webcamStatus = 'unfreeze';
        $scope.btnSelectImageFromComputer_Click = function (e) {
            $mdDialog.destroy();
            document.getElementById('image-parking-space' + $mdDialog.imageIndex).click();
        };

        $scope.btnGetImage_Click = function(e) {
            if ($scope.webcamStatus == 'unfreeze') {
                Webcam.freeze();
                $scope.webcamStatus = 'freeze';
                $scope.btnGetImageText = "Chụp ảnh khác";
            } else {
                Webcam.unfreeze();
                $scope.webcamStatus = 'unfreeze';
            }
        };
    });
})();