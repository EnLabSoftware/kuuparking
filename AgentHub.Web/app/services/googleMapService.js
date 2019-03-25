(function() {
    "use strict";
    angular.module("app").service("googleMapService", function($http, $q, $cookies, baseService, uiGmapGoogleMapApi) {
        var self = this;

        this.base = baseService;

        this.getGeocode = function(coords) {
            var deferred = $q.defer();
            //
            // Google Map is ready
            uiGmapGoogleMapApi.then(
                function(maps) {
                    var geocoder = new google.maps.Geocoder;
                    var latlng = { lat: coords.latitude, lng: coords.longitude };
                    geocoder.geocode({ 'location': latlng }, function(results, status) {
                        if (status === google.maps.GeocoderStatus.OK) {
                            deferred.resolve(results);
                        } else {
                            deferred.reject(status);
                        }
                    });
                },
                function(err) { deferred.reject(err); }
            );

            return deferred.promise;
        };

        this.geocodeToAddress = function(geocodeData) {
            var deferred = $q.defer();

            if (!geocodeData || geocodeData.length === 0) {
                deferred.reject(geocodeData);
            } else {
                var address = { StreetNumber: "", Street: "" };
                var country = '', state = '', city = '', district = '';

                var addressKeys = ["country", "administrative_area_level_1", "administrative_area_level_2", "sublocality_level_1", "route", "street_number", "street_address"];
                
                for (var i = geocodeData.length - 1; i >= 0; i--) {
                    var name = geocodeData[i].types[0];
                    for (var j = 0; j < geocodeData[i].types.length; j++) {
                        if (addressKeys.indexOf(geocodeData[i].types[j]) >=0) {
                            name = geocodeData[i].types[j];
                            break;
                        }                            
                    }
                    var longName = geocodeData[i].address_components[0].long_name;
                    switch (name) {
                    case 'country':
                        country = longName;
                        break;
                    case 'administrative_area_level_1':
                        state = longName;
                        break;
                    case 'administrative_area_level_2':
                        city = longName;
                        break;
                    case 'sublocality_level_1':
                        if (city.length === 0)
                            city = longName;
                        else if (district.length === 0)
                            district = longName;
                        break;
                    case 'route':
                        address.Street = longName;
                        break;
                    case 'street_number':
                        address.StreetNumber = longName;
                        break;
                    case 'street_address':
                        address.StreetNumber = longName;
                        address.Street = geocodeData[i].address_components[1].long_name;
                        break;
                    }
                }

                if (city.length === 0 && district.length > 0) {
                    city = district;
                    district = '';
                }

                baseService.getAddressInformations(country, state, city, district).then(
                    function(response) {
                        address.cityList = response.cities;
                        address.districtList = response.districts;
                        if (response.stateResult)
                            address.StateId = response.stateResult.ID;
                        if (response.cityResult)
                            address.CityId = response.cityResult.ID;
                        if (response.districtResult)
                            address.DistrictId = response.districtResult.ID;

                        deferred.resolve(address);
                    },
                    function(errObject) {
                        deferred.reject(errObject);
                    }
                );
            }

            return deferred.promise;
        };
    });
}());