(function () {
    'use strict';

    angular.module("app").controller("gmapPickingLocationController", function ($scope, uiGmapGoogleMapApi, baseService) {
        var getAddress = function (lat, lng) {
            var geocoder = new google.maps.Geocoder();
            var latlng = new google.maps.LatLng(lat, lng);

            geocoder.geocode({ 'latLng': latlng }, function (results, status) {
                if (status == google.maps.GeocoderStatus.OK) {
                    if (results[0]) {
                        $scope.locationResult = results[0];
                        $scope.$apply();
                    } else {
                        console.log('Location not found');
                    }
                } else {
                    console.log('Geocoder failed due to: ' + status);
                }
            });
        };

        //
        // Show current location and set it as center in Google Map
        var setCurrentLocation = function (coords) {
            if ($scope.currentLocation) {
                $scope.currentLocation.latitude = coords.latitude;
                $scope.currentLocation.longitude = coords.longitude;
            } else {
                $scope.currentLocation = {
                    id: -1,
                    latitude: coords.latitude,
                    longitude: coords.longitude,
                    Name: "",
                    showWindow: true,
                    title: 'Di chuyển để chọn vị trí',
                    options: {
                        icon: {
                            url: '/Images/pin/user_location_icon.png',
                            size: new google.maps.Size(30, 37),
                            origin: new google.maps.Point(0, 0),
                            anchor: new google.maps.Point(15, 37)
                        },
                        draggable: true,
                        animation: 0,
                        label: "",
                        title: "Di chuyển để chọn vị trí"
                    },
                    events: {
                        dragend: function (marker, eventName, args) {
                            getAddress(marker.getPosition().lat(), marker.getPosition().lng());
                        }
                    }
                };

                $scope.$apply();
            }

            getAddress(coords.latitude, coords.longitude);
        };

        //
        // Google Map options
        $scope.map = {
            center: { latitude: 16.053174, longitude: 108.202842 },
            zoom: 14,
            markers: [],
            doClusterMarkers: false,
            currentClusterType: 'standard',
            clusterTypes: baseService.ClusterTypeNames,
            selectClusterType: 'standard',
            selectedClusterTypes: baseService.ClusterTypes,
            clusterOptions: baseService.ClusterTypes.standard,
            clickedMarker: {
                id: 0,
                options: {}
            },
            searchbox: {
                template: 'searchbox.tpl.html',
                events: {
                    places_changed: function(searchBox) {
                        var place = searchBox.getPlaces();
                        if (!place || place == 'undefined' || place.length == 0) {
                            console.log('no place data :(');
                            return;
                        }

                        var center = { latitude: place[0].geometry.location.lat(), longitude: place[0].geometry.location.lng() };
                        $scope.map.center = center;
                        setCurrentLocation(center);
                    }
                }
            },
            events: {
                tilesloaded: function (map) {
                    $scope.mapInstance = map;
                },
                click: function (e, eName, coords) {
                    var center = { latitude: coords[0].latLng.lat(), longitude: coords[0].latLng.lng() };
                    setCurrentLocation(center);
                }
            }
        };

        //
        // Coords from parent
        var coords = $scope.$parent && $scope.$parent ? angular.copy($scope.$parent.coords) : undefined;

        //
        // Google Map is ready
        uiGmapGoogleMapApi.then(function(maps) {
            if (coords) {
                // Waiting for UI
                setTimeout(function() {
                    // Refresh the map
                    window.dispatchEvent(new Event('resize'));
                    // Show current location
                    $scope.map.center.latitude = coords.latitude;
                    $scope.map.center.longitude = coords.longitude;
                    setCurrentLocation(coords);
                }, 300);
            } else if ("geolocation" in navigator) {
                //
                // Get current location
                navigator.geolocation.getCurrentPosition(function(position) {
                    // Refresh the map
                    window.dispatchEvent(new Event('resize'));
                    // Show current location
                    $scope.map.center.latitude = position.coords.latitude;
                    $scope.map.center.longitude = position.coords.longitude;
                    setCurrentLocation(position.coords);
                });
            } else {
                /* geolocation IS NOT available */
            }
        });
    });
})();