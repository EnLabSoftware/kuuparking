(function () {
    "use strict";

    angular.module("app").controller("resetPasswordController", function ($http, $scope, baseService, userService) {
        $scope.loading = false;
        $scope.email = baseService.getQueryParam("email"); // User email get from URL (query string)
        $scope.code = baseService.getQueryParam("code"); // Token to reset password get from URL (query string)
        $scope.newPassword = "";
        $scope.confirmPassword = "";

        $scope.submit = function() {
            if (!$scope.newPassword || $scope.newPassword.length === 0 || !$scope.confirmPassword || $scope.confirmPassword.length === 0) {
                baseService.showWarningToast("Vui lòng nhập mật khẩu mới.");
            } else {
                $scope.loading = true;
                userService.resetPassword({ Email: $scope.email, Code: $scope.code, NewPassword: $scope.newPassword, ConfirmPassword: $scope.confirmPassword }).then(function (response) {
                    if (response && response.message === "success") {
                        var message = "Bạn đã thay đổi mật khẩu thành công!";
                        baseService.showDialog({ content: message }).finally(function() { baseService.goToHome(); });
                    } else {
                        $scope.loading = false;
                        baseService.showErrorToast(response.message);
                    }
                }).finally(function() { $scope.loading = false; });
            }
        };
    });
})();