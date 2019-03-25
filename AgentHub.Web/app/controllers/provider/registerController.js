(function () {
    "use strict";

    angular.module("app").controller("providerRegisterController", function ($http, $q, $scope, $stateParams, $mdDialog, $cookies, $timeout, baseService, userService, providerService, googleMapService, uiGmapGoogleMapApi, globalConstants) {
        userService.getCurrentUser().then(function (userInfo) {
            baseService.showSignedInMessage();

            $timeout(function () { providerService.goToDashboard(); }, 1500);
        });

        $scope.activateCode = "";
        $scope.user = {
            IsOvernightParking: true,
            IsOpen247: true,
            IsWeekendAvailable: true
        };
        $scope.alreadyRegistered = false;
        $scope.withFacebook = false;
        $scope.withGoogle = false;
        $scope.step = 1;
        $scope.imageIndex = 1;
        $scope.imgSources = ["/Images/camera-icon-blue.png", "/Images/camera-icon-blue.png", "/Images/camera-icon-blue.png"];
        $scope.focusProviderName = false;
        $scope.cityList = [];
        $scope.districtList = [];
        $scope.loading = false;

        var initUserInfo = function(userInfo) {
            $scope.user.Email = userInfo.Email;
            $scope.user.LastName = userInfo.LastName;
            $scope.user.FirstName = userInfo.FirstName;
            $scope.user.PhoneNumber = userInfo.PhoneNumber;
            $scope.user.Provider = userInfo.Provider;
            $scope.user.Password = "KuuParking.com123";
            $scope.user.ConfirmPassword = $scope.user.Password;
            $scope.user.ExternalAccessToken = userInfo.ExternalAccessToken;
        };
        //
        // After login with Google or Facebook but doesn't have an account
        var externalUserInfo = userService.getExternalUserInfo();
        if (externalUserInfo && externalUserInfo.Provider) {
            if (externalUserInfo.Provider.toLowerCase() === "google") {
                $scope.withGoogle = true;
            }
            else if (externalUserInfo.Provider.toLowerCase() === "facebook") {
                $scope.withFacebook = true;
            }
            initUserInfo(externalUserInfo);
        }
        //
        // Get Contries && States from server
        userService.getCountriesAndStates().then(function(response) {
            $scope.countryList = response.countries;
            $scope.stateList = response.states;
            if ($scope.countryList && $scope.countryList.length > 0) 
                $scope.user.CountryId = $scope.countryList[0].ID; // First country
            else
                $scope.user.CountryId = 1; // Viet Nam
        });

        $scope.$watch("step", function(newValue, oldValue) {
            if (newValue !== oldValue && newValue === 2 && !$scope.user.Latitude)
                $scope.showCurrentAddress();
        });

        var checkIsNullOrEmpty = function(text) {
            return (text === undefined || text === null || text === '');
        };

        var validateStep1 = function (target) {
            var message = "Vui lòng điền đầy đủ thông tin trước khi tiếp tục.";
            var missingList = [];
            if (checkIsNullOrEmpty($scope.user.FirstName)) {
                missingList.push("Họ và tên đệm");
            }
            if (checkIsNullOrEmpty($scope.user.LastName)) {
                missingList.push("Tên");
            }
            if (checkIsNullOrEmpty($scope.user.PhoneNumber) && checkIsNullOrEmpty($scope.user.Email)) {
                missingList.push("Số điện thoại hoặc Địa chỉ e-mail");
            }
            //
            // Since user login with Facebook or Google, password will not be needed.
            if (!($scope.withGoogle || $scope.withFacebook)) {
                if (checkIsNullOrEmpty($scope.user.Password)) {
                    missingList.push("Mật khẩu");
                } else if (checkIsNullOrEmpty($scope.user.ConfirmPassword)) {
                    missingList.push("Nhập lại mật khẩu");
                } else {
                    message = "Xin kiểm tra lại mật khẩu của bạn.";
                    if ($scope.user.Password !== $scope.user.ConfirmPassword) {
                        missingList.push("Mật khẩu của bạn không trùng khớp");
                    }
                    if ($scope.user.Password.length < 6) {
                        missingList.push("Độ dài tối thiểu của mật khẩu là 6 ký tự");
                    }
                    if (!/[A-Z]/.test($scope.user.Password)) {
                        missingList.push("Mật khẩu phải chứa ít nhất một ký tự vết hoa");
                    }
                    if (!/[a-z]/.test($scope.user.Password)) {
                        missingList.push("Mật khẩu phải chứa ít nhất một ký tự vết thường");
                    }
                    if (!/[0-9]/.test($scope.user.Password)) {
                        missingList.push("Mật khẩu phải chứa ít nhất một số");
                    }
                }
            }

            if (missingList.length > 0) {
                var content = "<h4>" + message + "</h4><ul><li>" + missingList.join("</li><li>") + "</li></ul>";
                baseService.showDialog({ $event: target, content: content });

                return false;
            }

            return true;
        };

        var validateStep2 = function (target) {
            var message = "Vui lòng điền đầy đủ thông tin trước khi tiếp tục.";
            var missingList = [];
            if (checkIsNullOrEmpty($scope.user.ProviderName)) {
                missingList.push("Tên bãi đậu xe");
            }
            if (checkIsNullOrEmpty($scope.user.MaximumSlots)) {
                missingList.push("Số lượng chỗ");
            }
            if (checkIsNullOrEmpty($scope.user.Price)) {
                missingList.push("Giá");
            }
            if (checkIsNullOrEmpty($scope.user.Latitude)) {
                missingList.push("Vị trí bãi đậu xe");
            }
            else if (checkIsNullOrEmpty($scope.user.Longitude)) {
                missingList.push("Vị trí bãi đậu xe");
            }
            if (checkIsNullOrEmpty($scope.user.Street)) {
                missingList.push("Tên đường");
            }

            if (missingList.length > 0) {
                var content = "<h4>" + message + "</h4><ul><li>" + missingList.join("</li><li>") + "</li></ul>";
                baseService.showDialog({ $event: target, content: content });

                return false;
            }

            return true;
        };

        $scope.nextStep = function (e, step) {
            // Validate data first, then move to the next step
            switch (step) {
                case 1:
                    if (!validateStep1(e))
                        return;
                    break;
                case 2:
                    if (!validateStep2(e))
                        return;
                    break;
            }

            $scope.step++;
        };
        
        $scope.imageLocation_Changed = function (input, index) {
            if (input.files && input.files[0]) {
                var reader = new FileReader();
                reader.onload = function (e) {
                    $scope.imgSources[index] = e.target.result;
                    $scope.$apply();
                };
                reader.readAsDataURL(input.files[0]);
            }
        };
        
        var signInAfterRegistration = function (accessToken) {
            //
            // Get acces token from response.AccessToken (response is an entire ApplicationUser object so front-end has all user detail)
            baseService.setAccessToken(accessToken);
            //
            // Show the provider admin console
            providerService.goToDashboard();
        };

        var registerProvider = function (target) {
            $scope.user.FullName = $scope.user.FirstName + ' ' + $scope.user.LastName;
            $scope.user.ExpiredDate = new Date();
            $scope.user.UserProfileType = globalConstants.Provider;

            if ($scope.withGoogle || $scope.withFacebook) {
                userService.registerExternal($scope.user).then(function(response) {
                    signInAfterRegistration(response.AccessToken);
                }, function() {
                    baseService.goToSignin();
                }).finally(function () {
                    //
                    // Clear Google or FaceBook logged object in cookie
                    userService.clearExternalUserInfo();
                    $scope.loading = false;
                });
            } else {
                userService.register($scope.user).then(function (response) {
                    signInAfterRegistration(response.AccessToken);
                }, function () {
                    baseService.goToSignin();
                }).finally(function() {
                    //
                    // Clear Google or FaceBook logged object in cookie
                    userService.clearExternalUserInfo();
                    $scope.loading = false;
                });
            }
        };

        $scope.register = function (e) {
            if (!validateStep1(e)) {
                $scope.step = 1;
                return;
            }

            if (!validateStep2(e)) {
                $scope.step = 2;
                return;
            }
            
            if ($scope.locationImage0 || $scope.locationImage1 || $scope.locationImage2) {
                var files = new Array();
                if ($scope.locationImage0)
                    files.push($scope.locationImage0);
                if ($scope.locationImage1)
                    files.push($scope.locationImage1);
                if ($scope.locationImage2)
                    files.push($scope.locationImage2);

                $scope.loading = true;
                userService.uploadProviderImageFiles(files).then(function (response) {
                    if (response && response.length > 0) {
                        var index = 0;
                        if ($scope.locationImage0 && response[index]) {
                            $scope.user.ImageLocation1 = response[index];
                            index++;
                        }

                        if ($scope.locationImage1 && response[index]) {
                            $scope.user.ImageLocation2 = response[index];
                            index++;
                        }

                        if ($scope.locationImage2 && response[index]) {
                            $scope.user.ImageLocation3 = response[index];
                            index++;
                        }
                        
                        if (response[index])
                            $scope.user.ThumbnailImageLocation = response[index];
                    }

                    registerProvider(e);
                }, function() {
                    $scope.loading = false;
                });
            } else {
                registerProvider(e);
            }
        };

        $scope.selectionState_Changed = function(stateId, e) {
            // Get State by selected country from server
            userService.getCities(stateId).then(function (response) {
                $scope.cityList = response;
            });
        };
        
        $scope.selectionCity_Changed = function (cityId, e) {
            // Get State by selected country from server
            userService.getDistricts(cityId).then(function (response) {
                $scope.districtList = response;
            });
        };

        //
        // Show the Street, District, City, State, and Country
        $scope.showAddress = function (geocodeData) {
            googleMapService.geocodeToAddress(geocodeData).then(function(address) {
                $scope.user.StreetNumber = address.StreetNumber;
                $scope.user.Street = address.Street;
                $scope.user.StateId = address.StateId;
                $scope.user.CityId = address.CityId;
                $scope.user.DistrictId = address.DistrictId;
                $scope.cityList = address.cityList;
                $scope.districtList = address.districtList;

                if (!$scope.user.ProviderName || $scope.user.ProviderName.length === 0) {
                    $scope.user.ProviderName = ($scope.user.StreetNumber ? $scope.user.StreetNumber + " " : "") + $scope.user.Street;
                    // Focust to provider name and select the input.
                    $scope.focusProviderName = true;
                }
            }).finally(function() {
                $scope.gettingAddress = false;
            });
        };
        
        //
        // Show address of current location
        $scope.showCurrentAddress = function () {
            if (navigator.geolocation) {
                $scope.gettingAddress = true;
                try {
                    navigator.geolocation.getCurrentPosition(function (position) {
                        googleMapService.getGeocode(position.coords).then(function (geocodeData) {
                            $scope.user.Latitude = position.coords.latitude;
                            $scope.user.Longitude = position.coords.longitude;
                            $scope.showAddress(geocodeData);
                        });
                    });
                } catch (e) {
                    $scope.gettingAddress = false;
                }
            }
        };

        $scope.showMap = function (e) {
            $mdDialog.show({
                controller: GoogleMapDialogController,
                scope: $scope,
                preserveScope : true,
                templateUrl: '/app/views/gmap-picking-location.html',
                parent: angular.element(document.body),
                targetEvent: e,
                clickOutsideToClose: true,
                fullscreen: window.outerWidth < 425,
            });
        };

        $scope.browseImage = function (e, index) {
            $scope.imageIndex = index;
            // Check the device
            if (Webcam.live) {
                // Show the dialog and webcam
                $mdDialog.imageIndex = index;
                $mdDialog.show({
                    controller: UploadImageDialogController,
                    scope: $scope,
                    preserveScope: true,
                    templateUrl: '/app/views/webcam-view.html',
                    parent: angular.element(document.body),
                    targetEvent: e,
                    clickOutsideToClose: true,
                    fullscreen: window.outerWidth < 425,
                });
            } else {
                // Take pic from computer
                document.getElementById('image-parking-space' + $scope.imageIndex).click();
            }
        };

        $scope.activate = function ($event) {
            if (!$scope.activateCode) {
                baseService.showDialog({ $event: $event, content: "Vui lòng nhập mã kích hoạt" });
                return;
            }
        };

    });
})();

function GoogleMapDialogController($scope, $mdDialog, userService, googleMapService) {
    // Current location.
    if ($scope.user && $scope.user.Longitude && $scope.user.Latitude) {
        $scope.coords = { "latitude": $scope.user.Latitude, "longitude": $scope.user.Longitude };
    }

    $scope.title = "Chọn vị trí bãi đậu xe";
    $scope.hide = function () { $mdDialog.destroy(); };
    $scope.cancel = function () { $mdDialog.cancel(); };

    $scope.btnGetLocation_Click = function (e) {
        if (e) {
            var coords = { "latitude": e.geometry.location.lat(), "longitude": e.geometry.location.lng() };
            $scope.user.Latitude = coords.latitude;
            $scope.user.Longitude = coords.longitude;
            googleMapService.getGeocode(coords).then(function (geocodeData) {
                $scope.showAddress(geocodeData);
            });
        } else {
            console.log('Location not found');
        }

        $mdDialog.hide();
    };
}

function UploadImageDialogController($scope, $mdDialog) {
    $scope.hide = function() { $mdDialog.destroy(); };
    $scope.cancel = function() { $mdDialog.cancel(); };

    $scope.btnDone_Click = function (e) {
        Webcam.snap(function (dataUri) {
            if ($mdDialog.imageIndex == 0)
                $scope.locationImage0 = dataUri;
            if ($mdDialog.imageIndex == 1)
                $scope.locationImage1 = dataUri;
            if ($mdDialog.imageIndex == 2)
                $scope.locationImage2 = dataUri;
        });
        Webcam.unfreeze();
        $mdDialog.destroy();
    };
}