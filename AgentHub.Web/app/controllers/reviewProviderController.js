(function () {
    "use strict";
    angular.module("app").controller("reviewProviderController", function ($scope, $timeout, $location, $anchorScroll, baseService, userService, googleMapService, providerService, uiGmapGoogleMapApi, $mdDialog) {
        $scope.slotProviders = [];
        $scope.providers = [];
        $scope.selectedSlotProvider = undefined;
        $scope.slotProviderLoading = true;
        $scope.userInfo = undefined;
        $scope.existingProvider = false;
        $scope.hours = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23];

        $scope.$watch("selectedSlotProvider", function (newValue, oldValue) {
            if (newValue) {
                $scope.slotProviderLoading = true;
                $timeout(function () {
                    $scope.providers = [newValue];
                    $scope.slotProviderLoading = false;
                }, 200);
            }
        });

        //
        // Google Map is ready
        uiGmapGoogleMapApi.then(function (maps) {
            userService.getCurrentUser().then(function (userInfo) {
                $scope.userInfo = userInfo;

                providerService.getInactiveSlotProviders().then(function (slotProviders) {
                    // Load State and City, init the map for each Slot Providers
                    for (var i = 0; i < slotProviders.length; i++) {
                        $scope.initProvider(slotProviders[i]);
                    }
                    $scope.slotProviders = slotProviders;
                    $scope.selectedSlotProvider = $scope.slotProviders[0];
                }).finally(function () {
                    // Wait for UI rendering
                    $timeout(function () { $scope.slotProviderLoading = false; }, 300);
                });
            }, function () { $scope.slotProviderLoading = false; });
        });
        //
        // Get Contries && States from server
        userService.getCountriesAndStates().then(function (response) {
            $scope.countryList = response.countries;
            $scope.stateList = response.states;
        });
        //
        // Address changed events
        $scope.streetNumberChanged = function (slotProvider) {
            slotProvider.buildAddressLine();
        };
        $scope.streetChanged = function (slotProvider) {
            slotProvider.buildAddressLine();
        };
        $scope.countryChanged = function (slotProvider) {
            slotProvider.buildAddressLine();
        };
        $scope.stateChanged = function (slotProvider) {
            // Clear city and district list
            slotProvider.cityList = [];
            slotProvider.districtList = [];
            // Get State by selected country from server
            if (slotProvider && slotProvider.Address && slotProvider.Address.StateId) {
                userService.getCities(slotProvider.Address.StateId).then(function (response) {
                    slotProvider.cityList = response;
                    slotProvider.buildAddressLine();
                });
            }
        };
        $scope.cityChanged = function (slotProvider) {
            // Clear district list
            slotProvider.districtList = [];
            // Get State by selected country from server
            if (slotProvider && slotProvider.Address && slotProvider.Address.CityId) {
                userService.getDistricts(slotProvider.Address.CityId).then(function (response) {
                    slotProvider.districtList = response;
                    slotProvider.buildAddressLine();
                });
            }
        };
        $scope.districtChanged = function (slotProvider) {
            slotProvider.buildAddressLine();
        };

        //
        // Init Map for Slot Provider
        $scope.initProvider = function (slotProvider) {
            if (!slotProvider || slotProvider.map)
                return;

            if (slotProvider.AvailableFromTime)
                slotProvider.AvailableFromTime = new Date(slotProvider.AvailableFromTime);
            if (slotProvider.AvailableToTime)
                slotProvider.AvailableToTime = new Date(slotProvider.AvailableToTime);

            $scope.stateChanged(slotProvider);
            $scope.cityChanged(slotProvider);

            slotProvider.map = {
                center: { latitude: slotProvider.Latitude, longitude: slotProvider.Longitude },
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
                    template: 'parking-lot-searchbox',
                    events: {
                        places_changed: function (searchBox) {
                            var place = searchBox.getPlaces();
                            if (!place || place == 'undefined' || place.length == 0) {
                                console.log('no place data :(');
                                return;
                            }

                            var coords = { latitude: place[0].geometry.location.lat(), longitude: place[0].geometry.location.lng() };
                            slotProvider.map.center = coords;
                            slotProvider.map.setLocation(coords);
                            slotProvider.map.showAddress(coords);
                        }
                    }
                },
                events: {
                    tilesloaded: function (map) {
                        slotProvider.mapInstance = map;
                    },
                    click: function (e, eName, c) {
                        var coords = { latitude: c[0].latLng.lat(), longitude: c[0].latLng.lng() };
                        slotProvider.map.setLocation(coords);
                        slotProvider.map.showAddress(coords);
                    }
                },
                currentLocation: {
                    id: -1,
                    latitude: slotProvider.Latitude,
                    longitude: slotProvider.Longitude,
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
                            var coords = { latitude: marker.getPosition().lat(), longitude: marker.getPosition().lng() };
                            slotProvider.map.setLocation(coords);
                            slotProvider.map.showAddress(coords);
                        }
                    }
                },
                setLocation: function (coords) {
                    if (coords && coords.latitude && coords.longitude && !isNaN(coords.latitude) && !isNaN(coords.longitude)) {
                        slotProvider.map.currentLocation.latitude = coords.latitude;
                        slotProvider.map.currentLocation.longitude = coords.longitude;
                        slotProvider.Latitude = coords.latitude;
                        slotProvider.Longitude = coords.longitude;
                        window.dispatchEvent(new Event('resize'));
                    }
                },
                getLocation: function () {
                    return angular.copy(slotProvider.map.currentLocation);
                },
                showAddress: function (coords) {
                    googleMapService.getGeocode(coords).then(function (geoData) {
                        googleMapService.geocodeToAddress(geoData).then(function (address) {
                            slotProvider.Address.StreetNumber = address.StreetNumber;
                            slotProvider.Address.Street = address.Street;
                            slotProvider.Address.StateId = address.StateId;
                            slotProvider.Address.CityId = address.CityId;
                            slotProvider.Address.DistrictId = address.DistrictId;
                            slotProvider.cityList = address.cityList;
                            slotProvider.districtList = address.districtList;

                            if (!slotProvider.Name || slotProvider.Name.length === 0) {
                                slotProvider.Name = (slotProvider.Address.StreetNumber ? slotProvider.Address.StreetNumber + " " : "") + slotProvider.Address.Street;
                            }

                            slotProvider.buildAddressLine();
                        });
                    });
                }
            };

            // Build address line for slot provider
            slotProvider.buildAddressLine = function () {
                if (slotProvider.Address) {
                    var address = slotProvider.Address;
                    var addressArr = [], i = 0;
                    if (address.StreetNumber && address.Street)
                        addressArr.push(address.StreetNumber + " " + address.Street);
                    else if (address.Street)
                        addressArr.push(address.Street);
                    //
                    // Find District
                    if (address.DistrictId && slotProvider.districtList) {
                        for (i = 0; i < slotProvider.districtList.length; i++) {
                            if (slotProvider.districtList[i].ID === address.DistrictId) {
                                addressArr.push(slotProvider.districtList[i].Name);
                                break;
                            }
                        }
                    }
                    // Find City
                    if (address.CityId && slotProvider.cityList) {
                        for (i = 0; i < slotProvider.cityList.length; i++) {
                            if (slotProvider.cityList[i].ID === address.CityId) {
                                addressArr.push(slotProvider.cityList[i].Name);
                                break;
                            }
                        }
                    }
                    // Find State
                    if (address.StateId && $scope.stateList) {
                        for (i = 0; i < $scope.stateList.length; i++) {
                            if ($scope.stateList[i].ID === address.StateId) {
                                addressArr.push($scope.stateList[i].Name);
                                break;
                            }
                        }
                    }

                    slotProvider.Address.AddressLine = addressArr.join(", ");
                }
            };

            // Images
            if (!slotProvider.ImageLocation1 || slotProvider.ImageLocation1.length === 0)
                slotProvider.ImageLocation1 = "/Images/camera-icon-blue.png";

            if (!slotProvider.ImageLocation2 || slotProvider.ImageLocation2.length === 0)
                slotProvider.ImageLocation2 = "/Images/camera-icon-blue.png";

            if (!slotProvider.ImageLocation3 || slotProvider.ImageLocation3.length === 0)
                slotProvider.ImageLocation3 = "/Images/camera-icon-blue.png";

            //
            // Remove the address object like country, state to solve saving problem.
            slotProvider.removeAddressObjects = function () {
                if (slotProvider.Address) {
                    if (slotProvider.Country)
                        slotProvider.Country = null;
                    if (slotProvider.State)
                        slotProvider.State = null;
                    if (slotProvider.City)
                        slotProvider.City = null;
                    if (slotProvider.District)
                        slotProvider.District = null;
                }
            };
            slotProvider.removeAddressObjects();
            //
            // Remove slot
            slotProvider.removeSlot = function (slot, number, target) {
                if (!slot)
                    return;

                var confirm = $mdDialog.confirm()
                .title('Bạn có muốn xóa chỗ đậu xe số ' + number + ' không?')
                .textContent('Mọi dữ liệu liên quan cũng sẽ được xóa.')
                .ariaLabel('Xác nhận xóa chỗ đậu xe')
                .targetEvent(target)
                .ok('Có, tôi muốn xóa')
                .cancel('Không');

                $mdDialog.show(confirm).then(function () {
                    slotProvider.saving = true;
                    providerService.deleteSlot(slot.ID).then(function () {
                        // Remove Slot from array
                        for (var i = 0; i < slotProvider.Slots.length; i++) {
                            if (slotProvider.Slots[i].ID === slot.ID) {
                                slotProvider.Slots.splice(i, 1);
                                slotProvider.MaximumSlots--;
                                break;
                            }
                        }
                        baseService.showSuccessToast("Xóa chỗ đậu xe thành công!");
                    }).finally(function () { slotProvider.saving = false; });
                });
            };
            //
            // Add new slot
            slotProvider.addSlot = function () {
                if (!slotProvider.ID) {
                    baseService.showWarningToast("Vui lòng lưu bãi đậu xe trước!");
                    return;
                }

                slotProvider.saving = true;
                providerService.addSlot(slotProvider.ID).then(function (slot) {
                    slotProvider.Slots.push(slot);
                    slotProvider.MaximumSlots++;
                    baseService.showSuccessToast("Thêm chỗ đậu xe thành công!");
                }).finally(function () { slotProvider.saving = false; });
            };

            return slotProvider;
        };
        //
        // Fire after an image being selected.
        $scope.imageChanged = function (fileInput, imageIndex) {
            var spId = parseInt(fileInput.getAttribute("sp-id"));
            var slotProvider = undefined;
            for (var i = 0; i < $scope.slotProviders.length; i++) {
                if ($scope.slotProviders[i].ID == spId) {
                    slotProvider = $scope.slotProviders[i];
                    break;
                }
            }

            if (!slotProvider)
                return;

            if (fileInput.files && fileInput.files[0]) {
                var saveThumbnailImage = (imageIndex === 0);
                var oldImage = "";
                if (imageIndex === 0)
                    oldImage = slotProvider.ImageLocation1;
                else if (imageIndex === 1)
                    oldImage = slotProvider.ImageLocation2;
                else if (imageIndex === 2)
                    oldImage = slotProvider.ImageLocation3;

                slotProvider.saving = true;
                userService.uploadProviderImage(fileInput.files[0], oldImage, saveThumbnailImage).then(function (newImages) {
                    if (imageIndex === 0) {
                        slotProvider.ImageLocation1 = newImages[0];
                        slotProvider.ThumbnailImageLocation = newImages[1];
                    } else if (imageIndex === 1)
                        slotProvider.ImageLocation2 = newImages[0];
                    else if (imageIndex === 2)
                        slotProvider.ImageLocation3 = newImages[0];

                    slotProvider.saving = false;
                });
            }            
        };
        //
        // Fire when an image being clicked.
        $scope.browseImage = function (providerIndex, imageIndex) {
            if ($scope.providers.length === 0)
                return;

            var sp = $scope.providers[0];
            document.getElementById('slotProviderFile' + sp.ID + '-' + imageIndex).click();
        };
        //
        // Copy Properties of Slot Provider
        $scope.copyProperties = function (from, to) {
            if (!from)
                return undefined;

            if (!to)
                to = {};

            var igoneKeys = [
                "$$hashKey", "map", "mapInstance", "cityList", "districtList", "saving", "active",
                "imageFiles", "imageFiles0", "imageFiles1", "imageFiles2", "Base64Image1", "Base64Image2", "Base64Image3"
            ];
            for (var key in from) {
                if (from.hasOwnProperty(key) && typeof (from[key]) !== "function") {
                    if (igoneKeys.indexOf(key) === -1)
                        to[key] = from[key];
                }
            }

            return to;
        };
        //
        // Generate Slots
        $scope.generateSlots = function (slotProvider) {
            if (!slotProvider)
                return slotProvider;

            var i = 0;
            var slotsMax = slotProvider.MaximumSlots;
            var slotsCount = slotProvider.Slots ? slotProvider.Slots.length : 0;

            if (slotsMax > slotsCount) {
                // Add some slots
                for (i = slotsCount; i < slotsMax; i++) {
                    slotProvider.Slots.push({
                        "Name": (i + slotsCount + 1).toString(),
                        "Description": null,
                        "SlotProviderId": slotProvider.ID,
                        "Price": slotProvider.Price,
                        "IsAvailable": true,
                        "ID": 0
                    });
                }
            } else if (slotsCount > slotsMax) {
                // Remove some slots
                for (i = slotsCount - slotsMax; i < slotsMax; i++) {
                    slotProvider.Slots.pop();
                }
            }

            return slotProvider;
        };
        //
        // Save slot Provider
        $scope.saveSlotProvider = function (slotProvider) {
            if (!slotProvider)
                return;

            slotProvider.saving = true;
            var saveInfo = function () {
                var tmpSlotProvider = $scope.copyProperties(slotProvider);
                if (tmpSlotProvider.ID) {
                    // Update slot provider
                    providerService.updateSlotProvider(tmpSlotProvider).finally(function () {
                        slotProvider.saving = false;
                        baseService.showSuccessToast("Lưu bãi đậu xe thành công!");
                    });
                } else {
                    // Add new slot provider
                    $scope.generateSlots(tmpSlotProvider);
                    providerService.addSlotProvider(tmpSlotProvider).then(function (newProvider) {
                        $scope.copyProperties(newProvider, slotProvider);
                        slotProvider.removeAddressObjects();
                        baseService.showSuccessToast("Lưu bãi đậu xe thành công!");
                    }).finally(function () {
                        slotProvider.saving = false;
                    });
                }
            };

            saveInfo();

            // Save Images
            //var files = [];
            //if (slotProvider.imageFiles0 || slotProvider.imageFiles1 || slotProvider.imageFiles2) {
            //    if (slotProvider.imageFiles0)
            //        files.push(slotProvider.imageFiles0);
            //    if (slotProvider.imageFiles1)
            //        files.push(slotProvider.imageFiles1);
            //    if (slotProvider.imageFiles2)
            //        files.push(slotProvider.imageFiles2);
            //}

            //if (files.length > 0) {
            //    userService.uploadProviderImageFiles(files, slotProvider.ImageLocation1, slotProvider.ImageLocation2, slotProvider.ImageLocation3).then(function(response) {
            //        if (response && response.length > 0) {
            //            var index = 0;
            //            if (slotProvider.imageFiles0 && response[index]) {
            //                slotProvider.ImageLocation1 = response[index];
            //                index++;
            //            }

            //            if (slotProvider.imageFiles1 && response[index]) {
            //                slotProvider.ImageLocation2 = response[index];
            //                index++;
            //            }

            //            if (slotProvider.imageFiles2 && response[index]) {
            //                slotProvider.ImageLocation3 = response[index];
            //                index++;
            //            }

            //            if (response[index])
            //                slotProvider.ThumbnailImageLocation = response[index];
            //        }
            //        saveInfo();
            //    }).finally(function() {
            //        slotProvider.saving = false;
            //    });
            //} else {
            //    saveInfo();
            //}
        };
        //
        // Add new Slot Provider
        $scope.addSlotProvider = function () {
            $scope.existingProvider = false;
            if (!$scope.slotProviders)
                $scope.slotProviders = [];

            var add = function (coords) {
                var newSlotProvider = providerService.newSlotProvider();
                newSlotProvider.SlotOwnerId = $scope.userInfo.UserProfileId;
                if (coords) {
                    newSlotProvider.Latitude = coords.latitude;
                    newSlotProvider.Longitude = coords.longitude;
                }

                $scope.initProvider(newSlotProvider).map.showAddress(coords);
                $scope.slotProviders.push(newSlotProvider);
                $scope.selectedSlotProvider = $scope.slotProviders[$scope.slotProviders.length - 1];
            };
            //
            // Request current location
            if (navigator.geolocation) {
                try {
                    navigator.geolocation.getCurrentPosition(function (position) {
                        add(position.coords);
                    });
                } catch (e) {
                    add();
                }
            } else {
                add();
            }
        };
        //
        // Delete Slot Provider
        $scope.deleteSlotProvider = function (slotProvider, target) {
            if (!slotProvider || !slotProvider.ID)
                return;

            if ($scope.slotProviders.length === 1) {
                baseService.showDialog({ content: "Chỉ có một bãi đậu xe nên bạn không thể xóa nó." });
                return;
            }

            var confirm = $mdDialog.confirm()
                .title('Bạn có muốn xóa bãi đậu xe ' + slotProvider.Name + ' không?')
                .textContent('Mọi dữ liệu liên quan cũng sẽ được xóa.')
                .ariaLabel('Xác nhận xóa bãi đậu xe')
                .targetEvent(target)
                .ok('Có, tôi muốn xóa')
                .cancel('Không');

            $mdDialog.show(confirm).then(function () {
                slotProvider.saving = true;
                providerService.deleteSlotProvider(slotProvider.ID).then(function () {
                    // Remove Slot Provider from array
                    for (var i = 0; i < $scope.slotProviders.length; i++) {
                        if ($scope.slotProviders[i].ID === slotProvider.ID) {
                            $scope.slotProviders.splice(i, 1);
                            $scope.selectedSlotProvider = $scope.slotProviders[0];
                            break;
                        }
                    }
                    // Since there is no Slot Provider, add a new one.
                    if ($scope.slotProviders.length === 0) {
                        //setTimeout(function () { $scope.addSlotProvider(); }, 2000);
                        $timeout(function () { $scope.addSlotProvider(); }, 300);
                    }
                    baseService.showSuccessToast("Xóa bãi đậu xe thành công!");
                }).finally(function () { slotProvider.saving = false; });
            });
        }
    });
})();