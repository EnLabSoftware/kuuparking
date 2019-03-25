(function () {
    "use strict";
    angular.module("app").controller("providerProfileController", function ($scope, $timeout, baseService, userService) {
        $scope.userInfo = undefined;
        $scope.avatarImageFile = undefined;
        $scope.defaultAvatar = userService.defaultAvatar;
        $scope.loading = false;

        userService.getCurrentUser().then(function (userInfo) {
            $scope.userInfo = userInfo;
        });
        //
        // Fire after an image being selected.
        $scope.imageChanged = function (fileInput) {
            if (fileInput.files && fileInput.files[0]) {
                var reader = new FileReader();
                reader.onload = function (e) {
                    $scope.userInfo.UserProfile.AvatarImageLocation = e.target.result;
                    $scope.$apply();
                };
                reader.readAsDataURL(fileInput.files[0]);
            }
        };
        //
        // Fire when an image being clicked.
        $scope.browseImage = function () {
            document.getElementById('provider-avatar-image' + $scope.userInfo.UserProfileId).click();
        };

        $scope.saveProfile = function () {
            $scope.loading = true;

            var saveInfo = function () {
                userService.updateUserProfile($scope.userInfo.UserProfile).then(function (response) {
                    baseService.showSuccessToast("Lưu thông tin thành công!");
                }).finally(function() { $timeout(function() { $scope.loading = false; }, 300); });
            };

            // Since the avata is changed, upload to server.
            if ($scope.avatarImageFile) {
                userService.uploadProfileImage($scope.avatarImageFile).then(function (response) {
                    if (response && response.length > 0) {
                        $scope.userInfo.UserProfile.AvatarImageLocation = response;
                    }
                    saveInfo();
                });
            } else {
                saveInfo();
            }
        };
    });
})();