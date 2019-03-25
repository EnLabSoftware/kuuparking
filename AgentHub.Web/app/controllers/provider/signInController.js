(function () {
    "use strict";

    angular.module("app").controller("providerSignInController", function ($http, $scope, $auth, $cookies, $timeout, baseService, userService, providerService) {
        $scope.loading = false;

        userService.getCurrentUser().then(function (userInfo) {
            baseService.showSignedInMessage();

            $timeout(function () { providerService.goToDashboard(); }, 1500);
        });

        $scope.btnSignIn_Click = function (e) {
            if (!$scope.user || !$scope.user.UserName || $scope.user.UserName.length === 0) {
                baseService.showWarningToast("Vui lòng nhập lại email.");
            } else if (!$scope.user.Password || $scope.user.Password.length === 0) {
                baseService.showWarningToast("Vui lòng nhập mật khẩu");
            } else {
                $scope.loading = true;
                //var loginObject = "grant_type=password&username=" + $scope.user.UserName + "&password=" + $scope.user.Password;
                //userService.signIn(loginObject).then(function (response) {
                var loginObject = { "UserName": $scope.user.UserName, "Password": $scope.user.Password };
                userService.logIn(loginObject).then(function (response) {
                    if (response.AccessToken) {
                        baseService.setAccessToken(response.AccessToken);
                        // Show the provider admin console
                        providerService.goToDashboard();
                    } else {
                        baseService.showErrorToast("Đăng nhập lỗi, vui lòng kiểm tra lại email và mật khẩu.");
                    }
                }).finally(function () {
                    setTimeout(function () { $scope.loading = false; }, 200);
                });
            }
        };

        var oAuthSignIn = function (userInfo) {
            if (userInfo.HasLocalAccount) {
                //
                // Get acces token from response.data.AccessToken (response.data is an entire ApplicationUser object so front-end has all detail of the login user)
                baseService.setAccessToken(userInfo.AccessToken);
                // Show the provider admin console
                providerService.goToDashboard();
            } else {
                userService.storeExternalUserInfo(userInfo);
                providerService.goToRegister();
            }
        };

        $scope.signInByFaceook_Click = function () {
            //var redirectUri = "api/Account/CompleteGoogleSignin";

            //var externalProviderUrl = "/api/Account/ExternalLogin?provider=Facebook&response_type=token&APIKey=E6C4AC5F-7CFE-4B3B-ADBB-4EE9A6AD86FC:857E30F4-26D0-46CC-8679-D440E5538C4C&redirect_uri=http://localhost:64669";
            //window.$windowScope = $scope;

            //var oauthWindow = window.open(externalProviderUrl, "Authenticate Account", "location=0,status=0,width=1024,height=768");

            $auth.authenticate("facebook").then(function (response) {
                if (response && response.data) {
                    oAuthSignIn(response.data);
                }
            }).catch(function (error) {
                console.log("authenticate with Facebook error", error);
            }).finally(function () {
                $scope.loading = false;
                $scope.hide();
            });
        };

        $scope.signInByGoogle_Click = function () {
            $scope.loading = true;
            $auth.authenticate('google').then(function (response) {
                if (response && response.data) {
                    oAuthSignIn(response.data);
                } else {
                    $scope.loading = false;
                }
            }, function() {
                $scope.loading = false;
            }).catch(function (error) {
                console.log("authenticate with Google error", error);
            }).finally(function () {
                $scope.loading = false;
                $scope.hide();
            });
        };
    });
})();
