(function() {
    "use strict";
    angular.module("app").service("parkingSlotService", function($http, $q, baseService) {
        var self = this;

        this.base = baseService;

        this.apiPrefix = "api/ParkingSlot/";

        //
        // Search for slot providers
        this.searchSlotProviders = function(criteria) {
            return baseService.postAsync(self.apiPrefix + "SearchSlotProviders", criteria);
        };

        //
        // Search for slot providers
        this.searchProviders = function(criteria) {
            return baseService.postAsync(self.apiPrefix + "SearchProviders", criteria);
        };

        //
        // Search for slot providers
        this.getProviderLocations = function(location) {
            return baseService.getAsync(self.apiPrefix + "GetProviderLocations?latitude=" + location.latitude + "&longitude=" + location.longitude);
        };
    });
}());