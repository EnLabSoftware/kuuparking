(function () {
    "use strict";

    angular.module("app").service("userService", function($http, $q, $cookies, baseService) {
        var self = this;
        this.base = baseService;

        this.register = function(user) {
            return baseService.postAsync("api/Account/Register", user);
        };

        this.registerExternal = function(user) {
            return baseService.postAsync("api/Account/RegisterExternal", user);
        };

        this.getCountries = function() {
            return baseService.getAsync("api/Commom/GetCountries");
        };

        this.getCountriesAndStates = function() {
            return baseService.getAsync("api/Commom/GetCountriesAndStates");
        };

        this.getStates = function(countryId) {
            return baseService.getAsync("api/Commom/GetStates?countryId=" + countryId);
        };

        this.getCities = function(stateId) {
            return baseService.getAsync("api/Commom/GetCities?stateId=" + stateId);
        };

        this.getDistricts = function(cityId) {
            return baseService.getAsync("api/Commom/GetDistricts?cityId=" + cityId);
        };

        this.uploadProviderImageFiles = function(files, imageFile1, imageFile2, imageFile3) {
            return baseService.postFileAsync("api/Commom/UploadProviderImages?imageFile1=" + baseService.toString(imageFile1).replace('/ProviderImages/', '') + "&imageFile2=" + baseService.toString(imageFile2).replace('/ProviderImages/', '') + "&imageFile3=" + baseService.toString(imageFile3).replace('/ProviderImages/', ''), files);
        };

        this.uploadProviderImage = function (image, oldImage, saveThumbnailImage) {
            return baseService.postFileAsync("api/Commom/UploadProviderImage?oldImageFile=" + baseService.toString(oldImage).replace('/ProviderImages/', '') + '&saveThumbnailImage=' + saveThumbnailImage, [image]);
        };

        this.uploadProfileImage = function (image) {
            return baseService.postFileAsync("api/Commom/UploadProfileImage", [image]);
        };

        this.signIn = function(kuuParkingAccount) {
            return baseService.postAsync("token", kuuParkingAccount);
        };

        this.logIn = function(kuuParkingAccount) {
            return baseService.postAsync("api/Account/Login", kuuParkingAccount);
        };

        this.signOut = function (kuuParkingAccount) {
            baseService.removeAccessToken();
        };

        var externalUserInfoKey = "ExternalUserInfo";
        this.storeExternalUserInfo = function(externalUserInfo) {
            $cookies.putObject(externalUserInfoKey, externalUserInfo, { path: "/", secure: false });
        };

        this.getExternalUserInfo = function() {
            return $cookies.getObject(externalUserInfoKey);
        };

        this.clearExternalUserInfo = function () {
            $cookies.remove(externalUserInfoKey);
        };

        this.getCurrentUser = function() {
            return baseService.getCurrentUser();
        };

        this.updateUserProfile = function (userProfile) {
            return baseService.postAsync("api/Account/UpdateUserProfile", userProfile);
        };

        this.defaultAvatar = "/Images/default-avatar.png";

        this.forgotPassword = function (model) {
            return baseService.postAsync("api/Account/ForgotPassword", model);
        };

        this.resetPassword = function (model) {
            return baseService.postAsync("api/Account/ResetPassword", model);
        };


    });
}());