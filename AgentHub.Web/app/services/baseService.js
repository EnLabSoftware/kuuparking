(function() {
    "use strict";

    angular.module("app").service("baseService", function($http, $q, $cookies, $location, $mdToast, $mdDialog, $timeout) {
        var self = this;
        this.baseUrl = "/";

        this.handleAjaxError = function(errObject, status) {
            var message = "Có lỗi xảy ra.";
            if (status === 500) {
                message = "Lỗi: ";
                if (errObject.Message !== "An error has occurred.") 
                    message += " " + errObject.Message;
                message += errObject.ExceptionMessage;

            } else if (status === 400) {
                switch (errObject.Message) {
                    case "The user name or password is incorrect.":
                        message = "Tài khoản hoặc mật khẩu không đúng.";
                        break;
                    case "The new password and confirmation password do not match.":
                        message = "Mật khẩu không trùng khớp.";
                        break;
                    default:
                        message = errObject.Message;
                }
            } else if (status === 401) {
                message = "Phiên làm việc đã hết, vui lòng đăng nhập lại.";
                self.removeAccessToken();
            } else if (status === 404) {
                message += " Không tìm thấy trang (404).";
            } else if (status === 408) {
                message = "Thời gian xử lý yêu cầu đã hết. Bạn có thể thực hiện lại hoặc tải lại trang.";
            }

            // Token is expired.
            if (status === 401) {
                // Go to login when current page is not home
                if (window.location.pathname !== "/" && window.location.pathname !== "/bai-dau-xe/dang-nhap" && window.location.pathname !== "/bai-dau-xe/dang-ky") {
                    $timeout(function() { self.goToSignin(); }, 500);
                } else {
                    // No need to show this message in home page
                    return null;
                }
            }

            // Return toast promise
            return self.showToast(message, self.ToastThemes.Error);
        };

        this.getAsync = function(apiPath) {
            var deferred = $q.defer();
            $http({ method: "GET", url: self.baseUrl + apiPath, data: {} })
                .success(function(data) { deferred.resolve(data); })
                .error(function(errObject, status) {
                    self.handleAjaxError(errObject, status);
                    deferred.reject(errObject);
                });

            return deferred.promise;
        };

        this.postAsync = function(apiPath, dataToSend) {
            var deferred = $q.defer();
            $http({ method: "POST", url: self.baseUrl + apiPath, data: dataToSend })
                .success(function(data) { deferred.resolve(data); })
                .error(function(errObject, status) {
                    self.handleAjaxError(errObject, status);
                    deferred.reject(errObject);
                });

            return deferred.promise;
        };

        this.putAsync = function(apiPath, dataToSend) {
            var deferred = $q.defer();
            $http({ method: "PUT", url: self.baseUrl + apiPath, data: dataToSend })
                .success(function(data) { deferred.resolve(data); })
                .error(function(errObject, status) {
                    self.handleAjaxError(errObject, status);
                    deferred.reject(errObject);
                });

            return deferred.promise;
        };

        this.deleteAsync = function(apiPath) {
            var deferred = $q.defer();
            $http({ method: "DELETE", url: self.baseUrl + apiPath })
                .success(function(data) { deferred.resolve(data); })
                .error(function(errObject, status) {
                    self.handleAjaxError(errObject, status);
                    deferred.reject(errObject);
                });

            return deferred.promise;
        };

        this.postFileAsync = function (apiPath, files) {
            var deferred = $q.defer();
            var fd = new FormData();
            for (var i = 0; i < files.length; i++) {
                fd.append('file' + i, files[i]);
            }
            $http.post(self.baseUrl + apiPath, fd, {
                    transformRequest: angular.identity,
                    headers: { 'Content-Type': undefined }
                })
                .success(function(data) { deferred.resolve(data); })
                .error(function(errObject, status) {
                    self.handleAjaxError(errObject, status);
                    deferred.reject(errObject);
                });

            return deferred.promise;
        };

        // 
        // Common APIs
        this.getAddressInformations = function(country, state, city, district) {
            return self.getAsync("api/Commom/GetAddressInformations?country=" + country + "&state=" + state + "&city=" + city + "&district=" + district);
        };

        this.updateAddress = function(address) {
            return self.postAsync("api/Commom/UpdateAddress", address);
        };
        //
        // Toast
        this.ToastThemes = {
            Error: "error",
            Warning: "warning",
            Success: "success"
        };
        this.showToast = function(content, theme) {
            var toast = $mdToast.simple()
                .textContent(content)
                .action('OK')
                .highlightAction(false)
                .position("top right")
                .theme(theme)
                .hideDelay(5000)
                .parent(angular.element(document).find('body').eq(0));

            // Return toast promise
            return $mdToast.show(toast);
        };
        this.showSuccessToast = function (content) { return self.showToast(content, self.ToastThemes.Success); };
        this.showWarningToast = function (content) { return self.showToast(content, self.ToastThemes.Warning); };
        this.showErrorToast = function (content) { return self.showToast(content, self.ToastThemes.Error); };
        //
        // Dialog
        this.showDialog = function (data) {
            var deferred = $q.defer();

            data = data || {};
            data.title = data.title || "KuuParking";
            data.content = data.content || "";
            data.closeText = data.closeText || "OK";

            $mdDialog.show({
                clickOutsideToClose: true,
                targetEvent: data.$event,
                template:
                    '<md-dialog>' +
                        '   <md-toolbar>' +
                        '       <div class="md-toolbar-tools">' +
                        '           <h5 class="two-lines" style="font-weight: normal; margin: 0;">' + data.title + '</h5>' +
                        '           <span flex></span>' +
                        '           <md-button class="md-icon-button" ng-click="cancel()">' +
                        '              <md-icon style="font-size: 16px"><i class="fa fa-close" aria-hidden="true"></i></md-icon>' +
                        '           </md-button>' +
                        '       </div>' +
                        '   </md-toolbar>' +
                        '   <md-dialog-content  class="login-dlg-content md-padding">' + data.content + '</md-dialog-content>' +
                        '   <md-dialog-actions>' +
                        '       <md-button ng-click="closeDialog()" class="md-primary">' + data.closeText + '</md-button>' +
                        '   </md-dialog-actions>' +
                        '</md-dialog>',
                controller: function dialogController($scope, $mdDialog) {
                    $scope.closeDialog = function() { $mdDialog.hide(); };
                    $scope.cancel = function() { $mdDialog.cancel(); };
                }
            }).then(
                function() { deferred.resolve(data.closeText); },
                function () { deferred.reject(); }
            );

            return deferred.promise;
        };
        //
        // Convert provider form API to marker of Google Map
        this.providerToMarker = function(provider, events) {
            if (!provider)
                return provider;

            var priceText = provider.IsPublic ? "public_icon" : "0";

            if (!provider.IsPublic && provider.Price && typeof (provider.Price) === "number" && provider.Price > 0) {
                var price = (provider.Price - (provider.Price % 1000)) / 1000;
                if (price <= 0)
                    price = 1;
                else if (price > 100)
                    price = 100;
                priceText = price.toString();
            }

            return {
                id: provider.ID,
                provider: provider,
                latitude: provider.Latitude,
                longitude: provider.Longitude,
                animation: google.maps.Animation.DROP,
                icon: {
                    url: '/Images/pin/' + priceText + '.png',
                    size: new google.maps.Size(30, 36),
                    origin: new google.maps.Point(0, 36),
                    anchor: new google.maps.Point(15, 36),
                },
                showWindow: false,
                title: provider.Name,
                events: {
                    click: function(e, name, m) {
                        if (events && typeof (events.click) === "function")
                            events.click(e, m);
                    },
                    mouseover: function(e, name, m) {
                        m.hovered = true;
                        m.icon.origin.y = 0;
                        //debugger;                    
                        //m.icon.origin = new google.maps.Point(0, 0);
                        //m.icon.url = "";
                        if (events && typeof (events.mouseover) === "function")
                            events.mouseover(e, m);
                    },
                    mouseout: function(e, name, m) {
                        m.hovered = false;
                        //m.icon.origin.y = 36;
                        m.icon.origin = new google.maps.Point(0, 36);
                        if (events && typeof (events.mouseout) === "function")
                            events.mouseout(e, m);
                    }
                },
                getPosition: function () {
                    return new google.maps.LatLng(provider.Latitude, provider.Longitude);
                }
            };
        };

        this.removeVietnameseSigns = function(inStr) {
            if (typeof(inStr) !== "string")
                return inStr;

            var str = inStr;
            str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, 'a');
            str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, 'e');
            str = str.replace(/ì|í|ị|ỉ|ĩ/g, 'i');
            str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, 'o');
            str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, 'u');
            str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, 'y');
            str = str.replace(/đ/g, 'd');
            str = str.replace(/À|Á|Ạ|Ả|Ã|Â|Ầ|Ấ|Ậ|Ẩ|Ẫ|Ă|Ằ|Ắ|Ặ|Ẳ|Ẵ/g, 'A');
            str = str.replace(/È|É|Ẹ|Ẻ|Ẽ|Ê|Ề|Ế|Ệ|Ể|Ễ/g, 'E');
            str = str.replace(/Ì|Í|Ị|Ỉ|Ĩ/g, 'I');
            str = str.replace(/Ò|Ó|Ọ|Ỏ|Õ|Ô|Ồ|Ố|Ộ|Ổ|Ỗ|Ơ|Ờ|Ớ|Ợ|Ở|Ỡ/g, 'O');
            str = str.replace(/Ù|Ú|Ụ|Ủ|Ũ|Ư|Ừ|Ứ|Ự|Ử|Ữ/g, 'U');
            str = str.replace(/Ỳ|Ý|Ỵ|Ỷ|Ỹ/g, 'Y');
            str = str.replace(/Đ/g, 'D');

            return str;
        };

        this.getQueryParam = function (paramName) {
            paramName = paramName.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + paramName + "=([^&#]*)"),
                results = regex.exec($location.absUrl());
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        };
        
        this.lowerMatch = function(left, right) {
            if (left === right)
                return true;

            if (left && right) {
                left = left.toLowerCase();
                right = right.toLowerCase();
                if (left.indexOf(right) >= 0)
                    return true;

                if (right.indexOf(left) >= 0)
                    return true;
            }

            return false;
        };

        this.toString = function(value) {
            if (value === undefined || value === null)
                return "";
            else
                return value;
        };
        
        //
        // Constant
        this.ClusterTypeNames = ['standard', 'beer'];
        this.ClusterTypes = {
            beer: {
                title: '',
                gridSize: 60,
                ignoreHidden: true,
                minimumClusterSize: 2,
                enableRetinaIcons: true,
                styles: [
                    {
                        url: '/images/beer.png',
                        textColor: '#ddddd',
                        textSize: 18,
                        width: 33,
                        height: 33,
                    }
                ]
            },
            standard: {
                title: 'View parking slots',
                gridSize: 60,
                ignoreHidden: true,
                minimumClusterSize: 2,
                imageExtension: 'png',
                imagePath: '/images/cluster/m'
            }
        };

        // Token
        this.setAccessToken = function(accessToken) {
            var expireDate = new Date();
            expireDate.setDate(expireDate.getDate() + 30);

            if (accessToken.toLowerCase().indexOf('bearer ') === -1)
                accessToken = 'Bearer ' + accessToken;

            $cookies.put('accessToken', accessToken, { path: "/", secure: false, expires: expireDate });

            // Set the Authorization
            $http.defaults.headers.common.Authorization = accessToken;
        };

        this.getAccessToken = function() {
            return $cookies.get('accessToken', { path: "/" });
        };

        this.removeAccessToken = function () {
            $cookies.remove("accessToken", { path: "/" });
        };

        this.signOut = function () {
            self.removeAccessToken();
        };

        this.isLoggedIn = function() {
            return $cookies.get('accessToken') ? true : false;
        };

        //
        // Current logged user
        var userInfo = undefined;
        var userInfoPromisse = undefined;
        this.getCurrentUser = function() {
            var deferred = $q.defer();

            if (userInfo) {
                deferred.resolve(userInfo);
            } else {
                if (!userInfoPromisse) {
                    userInfoPromisse = self.getAsync("api/Account/UserInfo");
                }

                userInfoPromisse.then(
                    function(response) {
                        userInfo = response;
                        deferred.resolve(userInfo);
                    },
                    function(errObject) { deferred.reject(errObject); }
                );
            }

            return deferred.promise;
        };

        this.goToHome = function () {
            location.href = location.origin;
        };

        this.goToSignin = function () {
            location.href = location.origin + "/bai-dau-xe/dang-nhap";
        };

        this.showSignedInMessage = function() {
            var content = '<div class="md-padding">Bạn đã đăng nhập. Đang chuyển hướng đến trang quản lý.</div>';
            content += '<md-progress-linear md-mode="indeterminate"></md-progress-linear>';

            return self.showDialog({ content: content });
        }

        this.deg2rad = function(deg) {
            return deg * (Math.PI / 180);
        }

        this.getDistanceFromLatLng = function (coords1, coords2) {
            var R = 6371; // Radius of the earth in km
            var dLat = self.deg2rad(coords2.latitude - coords1.latitude);  // deg2rad below
            var dLon = self.deg2rad(coords2.longitude - coords1.longitude);
            var a =
              Math.sin(dLat / 2) * Math.sin(dLat / 2) +
              Math.cos(self.deg2rad(coords1.latitude)) * Math.cos(self.deg2rad(coords2.latitude)) *
              Math.sin(dLon / 2) * Math.sin(dLon / 2)
            ;
            var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
            var d = R * c; // Distance in km
            return d;
        }

        this.parseQueryString = function () {
            var str = window.location.search;
            var params = {};

            str.replace(
                new RegExp("([^?=&]+)(=([^&]*))?", "g"),
                function ($0, $1, $2, $3) {
                    params[$1] = $3;
                }
            );
            return params;
        };
    });
}());