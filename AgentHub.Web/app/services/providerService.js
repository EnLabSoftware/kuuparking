(function() {
    "use strict";
    angular.module("app").service("providerService", function($http, $q, baseService) {
        var self = this;

        this.base = baseService;

        this.goToRegister = function() {
            location.href = location.origin + "/bai-dau-xe/dang-ky";
        };

        this.goToDashboard = function() {
            location.href = location.origin + "/bai-dau-xe/quan-ly";
        };

        this.goToParkingLot = function() {
            location.href = location.origin + "/bai-dau-xe/quan-ly#/thong-tin-bai-dau-xe";
        };

        this.goToReport = function () {
            location.href = location.origin + "/bai-dau-xe/quan-ly#/bao-cao";
        };

        this.goToProfile = function () {
            location.href = location.origin + "/bai-dau-xe/quan-ly#/thong-tin-tai-khoan";
        };
        this.goToActivation = function () {
            location.href = location.origin + "/kich-hoat-bai-dau-xe";
        };

        var slotProviders = undefined;
        this.getSlotProviders = function(userProfileId) {
            var deferred = $q.defer();

            if (slotProviders) {
                deferred.resolve(slotProviders);
            } else {
                baseService.getAsync("api/ParkingSlot/GetSlotProviders?userProfileId=" + userProfileId).then(
                    function(data) {
                        slotProviders = data;
                        deferred.resolve(slotProviders);
                    },
                    function(errObject) {
                        deferred.reject(errObject);
                    }
                );
            }

            return deferred.promise;
        };

        var inactiveSlotProviders = undefined;
        this.getInactiveSlotProviders = function (userProfileId) {
            var deferred = $q.defer();

            if (inactiveSlotProviders) {
                deferred.resolve(inactiveSlotProviders);
            } else {
                baseService.getAsync("api/ParkingSlot/GetInactiveSlotProviders").then(
                    function (data) {
                        inactiveSlotProviders = data;
                        deferred.resolve(inactiveSlotProviders);
                    },
                    function (errObject) {
                        deferred.reject(errObject);
                    }
                );
            }

            return deferred.promise;
        };

        this.setSlotStatus = function(slotId, isAvailable) {
            return baseService.getAsync("api/ParkingSlot/SetSlotStatus?slotId=" + slotId + "&isAvailable=" + isAvailable);
        };

        this.addSlotProvider = function(slotProvider) {
            return baseService.postAsync("api/ParkingSlot/AddSlotProvider", slotProvider);
        };

        this.updateSlotProvider = function(slotProvider) {
            return baseService.postAsync("api/ParkingSlot/UpdateSlotProvider", slotProvider);
        };

        this.deleteSlotProvider = function(slotProviderId) {
            return baseService.postAsync("api/ParkingSlot/DeleteSlotProvider?slotProviderId=" + slotProviderId, {});
        };

        var tmpSlotProviderId = 0;
        this.newSlotProvider = function() {
            var newSlotProvider = {
                SlotOwnerId: 0,
                Name: "",
                Description: null,
                MaximumSlots: 12,
                Price: 20000.00,
                Rating: null,
                AddressId: 0,
                Address: {
                    CountryId: 1
                },
                Latitude: 16.0544068, // Da Nang
                Longitude: 108.20216670000002,
                ImageLocation1: "/Images/camera-icon-blue.png",
                ImageLocation2: "/Images/camera-icon-blue.png",
                ImageLocation3: "/Images/camera-icon-blue.png",
                AvailableFromTime: null,
                AvailableToTime: null,
                IsAvailable: true,
                Slots: [],
                IsOpen247: true,
                IsWeekendAvailable: true,
                IsCoveredParking: null,
                IsOvernightParking: true,
                IsBusParking: null,
                IsCarWashingServiceAvailable: null,
                IsMondayAvailable: true,
                MondayOpenTime: 6,
                MondayClosedTime: 22,
                IsTuesdayAvailable: true,
                TuesdayOpenTime: 6,
                TuesdayClosedTime: 22,
                IsWednesdayAvailable: true,
                WednesdayOpenTime: 6,
                WednesdayClosedTime: 22,
                IsThursdayAvailable: true,
                ThursdayOpenTime: 6,
                ThursdayClosedTime: 22,
                IsFridayAvailable: true,
                FridayOpenTime: 6,
                FridayClosedTime: 22,
                IsSaturdayAvailable: true,
                SaturdayOpenTime: 6,
                SaturdayClosedTime: 22,
                IsSundayAvailable: true,
                SundayOpenTime: 6,
                SundayClosedTime: 22,
                IsPublic: false,
                ID: --tmpSlotProviderId
            };

            return newSlotProvider;
        };

        this.deleteSlot = function(slotId) {
            return baseService.getAsync("api/ParkingSlot/DeleteSlot?slotId=" + slotId);
        };

        this.addSlot = function(slotProviderId) {
            return baseService.postAsync("api/ParkingSlot/AddSlot?slotProviderId=" + slotProviderId, {});
        };

        this.bookSlot = function(bookingInfo) {
            return baseService.postAsync("api/ParkingSlot/BookSlot", bookingInfo);
        };

        this.releaseSlot = function (slotBookingId) {
            return baseService.getAsync("api/ParkingSlot/ReleaseSlot?slotBookingId=" + slotBookingId);
        };

        this.uploadBookingImage = function (image) {
            return baseService.postFileAsync("api/Commom/UploadBookingImage", [image]);
        };

        this.updateBookingImage = function (image, slotBookingId) {
            return baseService.postFileAsync("api/ParkingSlot/UpdateBookingImage?slotBookingId=" + slotBookingId, [image]);
        };

        this.getSlots = function (slotProviderId) {
            return baseService.getAsync("api/ParkingSlot/Slots?slotProviderId=" + slotProviderId);
        };

        this.getSlotBookings = function (userProfileId) {
            return baseService.getAsync("api/ParkingSlot/GetSlotBookings?userProfileId=" + userProfileId);
        };

        this.getSlotProvidersInPlace = function (stateId, cityId) {
            if (!stateId) stateId = "";
            if (!cityId) cityId = "";

            return baseService.getAsync("api/ParkingSlot/GetSlotProviders?stateId=" + stateId + "&cityId=" + cityId);
        };

        this.getSlotProvider = function (id) {
            return baseService.getAsync("api/ParkingSlot/GetSlotProvider?slotProviderId=" + id);
        };

        this.getSlotProvidersInState = function() {
            return baseService.getAsync("api/ParkingSlot/GetSlotProvidersInState");
        }
    });
}());