(function () {
    "use strict";
    angular.module("app").controller("slotProviderDetailController", function ($q, $scope, $timeout, $location, baseService, providerService, uiGmapGoogleMapApi) {
        $scope.slotProvider = {};
        $scope.slides = [];
        var queryParams = baseService.parseQueryString();
        var directed = false;
        
        //
        // Show the direction between 2 locations
        $scope.showDirection = function () {
            setTimeout(function() {
                var origin = $scope.map.currentLocationMarker;
                var destination = $scope.map.slotProviderMarker;
                if (origin && destination && !directed) {
                    directed = true;
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
                    $scope.directionsService.route(request, function (response, status) {
                        if (status == google.maps.DirectionsStatus.OK) {
                            // Display the route on the map.
                            $scope.directionsDisplay.setDirections(response);

                            //
                            // Repaint gmap
                            setTimeout(function () {
                                window.dispatchEvent(new Event('resize'));
                            }, 500); // Delay for animation of the map.
                        }
                    });
                }
            }, 1000);
        };

        //
        // Google Map is ready
        uiGmapGoogleMapApi.then(function (maps) {
            // 
            // Define current location
            $scope.map.currentLocationMarker = {
                id: -1,
                latitude: 16.0544068, // Da Nang
                longitude: 108.20216670000002,
                Name: "",
                showWindow: true,
                title: 'Vị trí hiện tại',
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
                    title: "Vị trí hiện tại"
                }
            };
            //
            // Get current location
            try {
                navigator.geolocation.getCurrentPosition(function (position) {
                    $scope.map.currentLocationMarker.latitude = position.coords.latitude;
                    $scope.map.currentLocationMarker.longitude = position.coords.longitude;
                    $scope.showDirection();
                });
            } catch (e) { }
            //
            // Get slot provider info
            if (queryParams.id) {
                providerService.getSlotProvider(queryParams.id).then(function (data) {
                    $scope.slotProvider = data;
                    //
                    // Location
                    $scope.map.slotProviderMarker = baseService.providerToMarker(data);
                    $scope.showDirection();
                    //
                    // Images
                    var imageId = 1;
                    if ($scope.slotProvider.ImageLocation1 && $scope.slotProvider.ImageLocation1 !== "/Images/camera-icon-blue.png") {
                        $scope.slides.push({ id: imageId, url: $scope.slotProvider.ImageLocation1 });
                        imageId += 1;
                    }
                    if ($scope.slotProvider.ImageLocation2 && $scope.slotProvider.ImageLocation2 !== "/Images/camera-icon-blue.png") {
                        $scope.slides.push({ id: imageId, url: $scope.slotProvider.ImageLocation2 });
                        imageId += 1;
                    }
                    if ($scope.slotProvider.ImageLocation3 && $scope.slotProvider.ImageLocation3 !== "/Images/camera-icon-blue.png") {
                        $scope.slides.push({ id: imageId, url: $scope.slotProvider.ImageLocation3 });
                    }
                    //
                    // Page title
                    if ($scope.slotProvider) {
                        var title;

                        if ($scope.slotProvider.Address && $scope.slotProvider.Address.AddressLine)
                            title = $scope.slotProvider.Address.AddressLine;
                        else
                            title = $scope.slotProvider.Name;

                        window.document.title = title + " - KuuParking";
                    }
                });
            }
        });

        $scope.map = {
            center: { latitude: 16.0544068, longitude: 108.20216670000002 },
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
                    $scope.mapInstance = map;
                }
            }
        };
    });
})();