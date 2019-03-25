function ProviderDetailsDialogController($scope, $mdDialog, data) {
    $scope.marker = data.marker;
    if ($scope.marker && $scope.marker.provider) {
        $scope.marker.provider.slides = [];
        var imageId = 1;
        
        if ($scope.marker.provider.ImageLocation1 && $scope.marker.provider.ImageLocation1 !== "/Images/camera-icon-blue.png") {
            $scope.marker.provider.slides.push({
                id: imageId,
                url: $scope.marker.provider.ImageLocation1
            });
            imageId += 1;
        }

        if ($scope.marker.provider.ImageLocation2 && $scope.marker.provider.ImageLocation2 !== "/Images/camera-icon-blue.png") {
            $scope.marker.provider.slides.push({
                id: imageId,
                url: $scope.marker.provider.ImageLocation2
            });
            imageId += 1;
        }

        if ($scope.marker.provider.ImageLocation3 && $scope.marker.provider.ImageLocation3 !== "/Images/camera-icon-blue.png") {
            $scope.marker.provider.slides.push({
                id: imageId,
                url: $scope.marker.provider.ImageLocation3
            });
        }
    }

    $scope.getDirection = function () {
        if (typeof (data.getDirection) === "function") {
            data.getDirection();
        }
        $mdDialog.hide();
    };
    $scope.call = function () {
        alert("Chúng tôi đang phát triển tính năng này.");
    };
    $scope.hide = function () {
        $mdDialog.hide();
    };
    $scope.cancel = function () {
        $mdDialog.cancel();
    };


}