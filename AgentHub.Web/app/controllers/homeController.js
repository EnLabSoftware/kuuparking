(function () {
    "use strict";

    angular.module("app").controller("homeController", function ($q, $rootScope, $scope, $filter, $state, $mdDialog, $mdMedia, $timeout, uiGmapGoogleMapApi, baseService, parkingSlotService) {
        //$scope.Radius = 5;
        $scope.currentLocation = undefined;
        $scope.searchingLocation = undefined;
        $scope.sortList = [{ id: 1, name: "distance", text: "Khoảng cách" }, { id: 2, name: "lowPrice", text: "Giá thấp nhất" }, { id: 3, name: "highPrice", text: "Giá cao nhất" }];
        $scope.sortBy = $scope.sortList[0];
        $scope.filterList = [
            { id: 0, name: "", text: "Bất kỳ" },
            { id: 1, name: "IsOpen247", text: "Mở cửa 24/7" },
            { id: 2, name: "IsWeekendAvailable", text: "Đậu xe cuối tuần" },
            { id: 3, name: "IsCoveredParking", text: "Có mái che" },
            { id: 4, name: "IsOvernightParking", text: "Đậu xe qua đêm" },
            { id: 5, name: "IsBusParking", text: "Đậu được xe bus" },
            { id: 6, name: "IsCarWashingServiceAvailable", text: "Có rửa xe" },
        ];
        $scope.filterByItems = [$scope.filterList[0]];
        $scope.showList = false;
        $scope.loading = true;

        $scope.getSelectedFilters = function() {
            return "Lọc theo: " + $scope.filterByItems.map(function (elem) {
                return elem.text;
            }).join(", ");
        }

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
            events: {
                tilesloaded: function (map) {
                    if (!$scope.mapInstance) {
                        $scope.mapInstance = map;
                        //
                        // Get current location
                        if (navigator.geolocation) {
                            try {
                                navigator.geolocation.getCurrentPosition(function (position) {
                                    setCurrentLocation(position.coords);
                                    // Search slot providers 
                                    $scope.searchProviders(position.coords);
                                });
                            } catch (e) { }
                        }
                    }
                }
            }
        };

        // Radius slider
        $scope.slider = {
            value: 5000,
            options: {
                showSelectionBar: true,
                floor: 1000,
                ceil: 50000,
                step: 100,
                translate: function (value) {
                    var text = (value / 1000);
                    if (value % 1000 === 0)
                        text += '.0';

                    return 'Bán kính ' + text + ' km';
                },
                hideLimitLabels: true,
                keyboardSupport: false,
                onEnd: function (sliderId, modelValue, highValue, pointerType) {
                    var location = $scope.searchingLocation ? $scope.searchingLocation : $scope.currentLocation;
                    if (location)
                        $scope.searchProviders(location);
                }
            }
        };

        //
        // Show current location and set it as center in Google Map
        var setCurrentLocation = function (coords) {
            $scope.map.center = { latitude: coords.latitude, longitude: coords.longitude };

            var image = {
                url: '/Images/pin/user_location_icon.png',
                size: new google.maps.Size(30, 37),
                origin: new google.maps.Point(0, 0),
                anchor: new google.maps.Point(15, 37)
            };

            $scope.currentLocation = {
                id: -1,
                latitude: coords.latitude,
                longitude: coords.longitude,
                title: 'Bạn đang ở đây',
                options: {
                    icon: image,
                    draggable: false,
                    animation: 0,
                    label: "",
                    title: "Bạn đang ở đây"
                }
            };

            $scope.$apply();
        };

        //
        // Show search location
        var setSearchingLocation = function (coords) {
            $scope.map.center = { latitude: coords.latitude, longitude: coords.longitude };
            $scope.searchingLocation = {
                id: -2,
                latitude: coords.latitude,
                longitude: coords.longitude,
                address: coords.address, // From Google Place API
                photos: coords.photos, // From Google Place API
                title: 'Di chuyển để tìm bãi đậu xe',
                options: {
                    //icon: image,
                    draggable: true,
                    animation: 0,
                    label: "",
                    title: "Di chuyển để tìm bãi đậu xe"
                },
                infoWindow: {
                    options: {
                        boxClass: 'current-location-info-window',
                        closeBoxDiv: '<div" class="pull-right" style="position: relative; cursor: pointer; margin: -20px -15px;">X</div>',
                        disableAutoPan: true,
                    },
                    show: false
                },
                events: {
                    dragend: function (e, name, marker) {
                        console.log("dragend", e, marker);
                        if (e && e.position) {
                            $scope.searchProviders({ "latitude": e.position.lat(), "longitude": e.position.lng() });
                        }
                    },
                    click: function (e, name, marker) {
                        $scope.showProviderDetailPopup(marker);
                    },
                    mouseout: function (e, name, marker) {
                        //console.log("mouseout", e, marker);
                    },
                    mouseover: function (e, name, marker) {
                        //console.log("mouseover", e, marker);
                    }
                }
            };

            $scope.$apply();
        };
        
        //
        // Clear direction on the map
        $scope.clearDirections = function() {
            if ($scope.directionsDisplay)
                $scope.directionsDisplay.setMap(null);
        };

        //
        // Show the direction between 2 locations
        $scope.showDirection = function(origin, destination) {
            var deferred = $q.defer();
            $scope.clearDirections();
            $scope.showList = false;

            if (origin && destination) {
                $scope.directionsDisplay = new google.maps.DirectionsRenderer({
                    map: $scope.mapInstance,
                    draggable: true,
                    suppressMarkers: true // Disable A & B marker
                });
                $scope.directionsService = new google.maps.DirectionsService();

                // Set destination, origin and travel mode.
                var request = {
                    destination: { lat: destination.latitude, lng: destination.longitude },
                    origin: { lat: origin.latitude, lng: origin.longitude },
                    travelMode: google.maps.TravelMode.DRIVING
                };

                // Pass the directions request to the directions service.
                $scope.directionsService.route(request, function(response, status) {
                    if (status == google.maps.DirectionsStatus.OK) {
                        // Display the route on the map.
                        $scope.directionsDisplay.setDirections(response);

                        //
                        // Repaint gmap
                        setTimeout(function() {
                            window.dispatchEvent(new Event('resize'));
                        }, 500); // Delay for animation of the map.

                        deferred.resolve(response);
                    } else {
                        deferred.reject(response);
                    }
                });
            } else {
                deferred.reject("Missing origin or destination");
            }

            return deferred.promise;
        };

        //
        // Show the direction from current location to another location
        $scope.directTo = function (destination) {
            var loc = $scope.currentLocation ? $scope.currentLocation : $scope.searchingLocation;
            $scope.showDirection(loc, destination).then(function(data) {
                if ($scope.searchingLocation && data && data.routes && data.routes[0] && data.routes[0].legs && data.routes[0].legs[0]) {
                    // Show distance in case of the distance of Searching Location and Current Location is lower than 100 met.
                    if (baseService.getDistanceFromLatLng($scope.searchingLocation, $scope.currentLocation) > 0.1) {
                        destination.direction = {
                            "distance": data.routes[0].legs[0].distance,
                            "duration": data.routes[0].legs[0].duration
                        };
                    }
                }
            });
        };

        //
        // Use google api to get the distances to provider locations
        var getDistances = function (origins, destinations) {
            var deferred = $q.defer();

            if (origins && origins.length > 0 && destinations && destinations.length > 0) {
                var mapOrigins = [], mapDestinations = [], i;
                for (i = 0; i < origins.length; i++)
                    mapOrigins.push(new google.maps.LatLng(origins[i].latitude, origins[i].longitude));
                for (i = 0; i < destinations.length; i++)
                    mapDestinations.push(new google.maps.LatLng(destinations[i].latitude, destinations[i].longitude));

                var service = new google.maps.DistanceMatrixService();
                var options = {
                    origins: mapOrigins,
                    destinations: mapDestinations,
                    travelMode: google.maps.TravelMode.DRIVING
                }

                service.getDistanceMatrix(options, function (response, status) {
                    if (status === "OK") {
                        deferred.resolve(response);
                    } else {
                        deferred.reject({ "error": "The distances aren't calculated." });
                    }
                });
            } else {
                deferred.reject({ "error": "Missing origin or destination" });
            }

            return deferred.promise;
        };

        //
        // Sort the locations list
        var sortMapMarkers = function () {
            if ($scope.map.markers && $scope.map.markers.length > 0) {
                var sortField = "distance.value";
                var reverse = false;
                if ($scope.sortBy && $scope.sortBy.name) {
                    if ($scope.sortBy.name === "lowPrice") {
                        sortField = "provider.Price";
                    }
                    else if ($scope.sortBy.name === "highPrice") {
                        sortField = "provider.Price";
                        reverse = true;
                    }
                }
                $scope.map.markers = $filter('orderBy')($scope.map.markers, sortField, reverse);
            }
        };
        
        //
        // Show Provider Details Popup
        $scope.showProviderDetailPopup = function (marker) {
            $mdDialog.show({
                data: { "marker": marker, "getDirection": function() { $scope.directTo(marker); } },
                controller: ProviderDetailsDialogController,
                templateUrl: '/App/views/provider/detail-dialog.html',
                parent: angular.element(document.body),
                //targetEvent: ev,
                clickOutsideToClose: true,
                fullscreen: window.outerWidth <= 550,
            });
        };
        //
        // Zoom to show all markers
        $scope.zoomMapToCoverAllMarkers = function () {
            if (!$scope.map.markers || $scope.map.markers.length === 0) {
                $scope.mapInstance.setZoom(14);
                return;
            }

            var bounds = new google.maps.LatLngBounds();
            for (var i = 0; i < $scope.map.markers.length; i++) {
                bounds.extend($scope.map.markers[i].getPosition());
            }

            //center the map to the geometric center of all markers
            $scope.mapInstance.setCenter(bounds.getCenter());
            //$scope.mapInstance.panTo(bounds.getCenter());

            $scope.mapInstance.fitBounds(bounds);

            // Remove one zoom level to ensure no marker is on the edge.
            if($scope.mapInstance.getZoom() < 10)
                $scope.mapInstance.setZoom($scope.mapInstance.getZoom() - 1);

            // Set a minimum zoom, if you got only 1 marker or all markers are on the same address map will be zoomed too much.
            if ($scope.mapInstance.getZoom() > 15) {
                $scope.mapInstance.setZoom(15);
            }

            //Alternatively this code can be used to set the zoom for just 1 marker and to skip redrawing.
            //Note that this will not cover the case if you have 2 markers on the same address.
            //if ($scope.map.markers.length === 1) {
            //    $scope.mapInstance.setMaxZoom(15);
            //    $scope.mapInstance.fitBounds(bounds);
            //    $scope.mapInstance.setMaxZoom(null);
            //}
        };
        //
        // Search provider locations near current location
        $scope.searchProviders = function (coords) {
            $scope.loading = true;
            $scope.clearDirections();

            var dataToSend = {
                Latitude: coords.latitude,
                Longitude: coords.longitude,
                //Radius: $scope.Radius * 1000,
                Radius: $scope.slider.value
            };

            // Filter
            angular.forEach($scope.filterByItems, function (item, key) {
                if (item.name && item.name.length > 0)
                    dataToSend[item.name] = true;
            });

            $scope.map.markers = [];
            parkingSlotService.searchProviders(dataToSend).then(
                function(data) {
                    var markers = [];
                    if (data && data.length > 0) {
                        angular.forEach(data, function (provider, key) {
                            // Events - Marker - Of Provider
                            var events = { click: function(e, m) { $scope.showProviderDetailPopup(m); } };
                            var marker = baseService.providerToMarker(provider, events);
                            markers.push(marker);
                            // Conver Datetime
                            if (provider.AvailableFromTime)
                                provider.AvailableFromTime = new Date(provider.AvailableFromTime);
                            if (provider.AvailableToTime)
                                provider.AvailableToTime = new Date(provider.AvailableToTime);
                        });
                    }
                    $scope.map.markers = markers;
                    //
                    // Cover All Markers
                    $scope.zoomMapToCoverAllMarkers();
                    //
                    // Get distances to providers
                    var location = $scope.searchingLocation ? $scope.searchingLocation : $scope.currentLocation;
                    if (location) {
                        getDistances([location], $scope.map.markers).then(function (data) {
                            if (data && data.rows && data.rows.length > 0) {
                                var distances = data.rows[0].elements;
                                if (distances.length === $scope.map.markers.length) {
                                    for (var i = 0; i < distances.length; i++) {
                                        if (distances[i].distance) {
                                            $scope.map.markers[i].distance = distances[i].distance;
                                            $scope.map.markers[i].distanceValue = distances[i].distance.value;
                                        }
                                        if (distances[i].duration)
                                            $scope.map.markers[i].duration = distances[i].duration;
                                    }
                                    //
                                    // Sort markers
                                    sortMapMarkers();
                                }
                            }
                        });
                    }

                    $scope.loading = false;
                },
                function(status) {
                    $scope.map.markers = [];
                    $scope.loading = false;
                }
            );
        };

        //
        // Handle clear search term to perform first searching
        $scope.$on('clearSearchTerm', function (event, args) {
            $scope.searchingLocation = undefined;
            setTimeout(function() { setCurrentLocation($scope.currentLocation); });
            $scope.searchProviders($scope.currentLocation);
        });
        
        //
        // Google Map is ready
        uiGmapGoogleMapApi.then(function (maps, a, b, c) {
            var searchElement = document.getElementById('autocomplete');
            var autocomplete = new google.maps.places.Autocomplete((searchElement), { types: ['geocode'] });
            searchElement.placeholder = "";
            autocomplete.addListener('place_changed', function () {
                // Get the place details from the autocomplete object.
                var place = autocomplete.getPlace();
                if (place.geometry && place.geometry.location) {
                    var coords = { latitude: place.geometry.location.lat(), longitude: place.geometry.location.lng() };
 
                    coords.address = place.adr_address;
                    coords.photos = place.photos;

                    setSearchingLocation(coords);
                    // Search slot providers 
                    $scope.searchProviders(coords);
                }
            });
        });

        //
        // Handle sorting changed event
        $scope.$watch('sortBy', function (newValue, oldValue) {
            if (newValue.id !== oldValue.id) {
                console.log('sortBy changed', newValue, oldValue);
                sortMapMarkers();
            }
        });

        //
        // Handle filtering changed event
        $scope.$watch('filterByItems', function (newValue, oldValue) {
            if (newValue.length !== oldValue.length) {
                var location = $scope.searchingLocation ? $scope.searchingLocation : $scope.currentLocation;
                $scope.searchProviders(location);
            }
        });

        $scope.moveToCurrentLocation = function () {
            if ($scope.currentLocation) {
                $scope.map.center = { latitude: $scope.currentLocation.latitude, longitude: $scope.currentLocation.longitude };
            }
            else if (navigator.geolocation) {
                try {
                    navigator.geolocation.getCurrentPosition(function (position) {
                        setCurrentLocation(position.coords);

                        // Search slot providers 
                        $scope.searchProviders(position.coords);
                    });
                } catch (e) {

                }
            }
        };

        $scope.getThumbnailImage = function (provider) {
            if (!provider || typeof (provider.ImageLocation1) !== "string")
                return "";

            return provider.ImageLocation1.replace("/ProviderImages/", "/ProviderImages/Thumbnail/");
        };
    });
})();