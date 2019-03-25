(function () {
    "use strict";

    angular.module("app").controller("forgetPasswordController", function ($http, $scope, baseService, userService) {
        $scope.loading = false;
        $scope.phoneOrEmail = "";

        $scope.submit = function() {
            if (!$scope.phoneOrEmail || $scope.phoneOrEmail.length === 0) {
                baseService.showWarningToast("Vui lòng nhập số điện thoại hoặc e-mail");
            } else {
                $scope.loading = true;
                userService.forgotPassword({ EmailAddress: $scope.phoneOrEmail }).then(function(response) {
                    if (response && response.message === "sent") {
                        var message = "Một e-mail đã gửi đến hòm thư của bạn, vui lòng kiểm tra để lấy lại mật khẩu.";
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