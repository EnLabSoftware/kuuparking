(function () {
    "use strict";
    angular.module("app").controller("providerDashboardController", function ($scope, $timeout, $mdDialog, baseService, userService, providerService) {
        $scope.slotProviders = [];
        $scope.providers = [];
        $scope.selectedSlotProvider = undefined;
        $scope.slotProviderLoading = true;
        $scope.userInfo = { UserProfile: { AllowCaptureVehicleIn: false } };

        $scope.$watch("selectedSlotProvider", function (newValue, oldValue) {
            if (newValue) {
                $scope.slotProviderLoading = true;
                $timeout(function () {
                    $scope.providers = [newValue];
                    $scope.slotProviderLoading = false;
                }, 200);
            }
        });

        userService.getCurrentUser().then(function (userInfo) {
            $scope.userInfo = userInfo;
            if (userInfo && userInfo.UserProfileId) {
                providerService.getSlotProviders(userInfo.UserProfileId).then(function (slotProviders) {
                    //
                    // Define the car parked percentage for each Slot Provider
                    if (slotProviders && slotProviders.length > 0) {
                        for (var i = 0; i < slotProviders.length; i++) {
                            $scope.initSlotProvider(slotProviders[i]);
                        }
                        $scope.slotProviders = slotProviders;
                        $scope.selectedSlotProvider = $scope.slotProviders[0];
                    }
                }).finally(function () {
                    // Wait for UI rendering
                    $timeout(function () { $scope.slotProviderLoading = false; }, 300);
                });
            }
            else {
                $scope.slotProviderLoading = false;
            }
        }, function () { $scope.slotProviderLoading = false; });

        //
        // Define methods for slot provider
        $scope.initSlotProvider = function (slotProvider) {
            if (!slotProvider || slotProvider.parkedPercent)
                return;

            slotProvider.parkedPercent = function () {
                var i, percent = 0, totalSlots = 0, totalAvailable = 0, totalParked = 0, totalAmount = 0;

                if (slotProvider.Slots) {
                    totalSlots = slotProvider.Slots.length;
                    totalAvailable = 0;
                    for (i = 0; i < totalSlots; i++) {
                        if (slotProvider.Slots[i].IsAvailable)
                            totalAvailable++;

                        if (slotProvider.Slots[i].ActiveBooking && slotProvider.Slots[i].ActiveBooking.ID) {
                            totalAmount += slotProvider.Slots[i].ActiveBooking.TotalAmount;
                        }
                    }
                    totalParked = totalSlots - totalAvailable;
                    percent = totalSlots === 0 ? 0 : totalParked / totalSlots * 100;
                }
                slotProvider.totalSlots = totalSlots;
                slotProvider.totalAvailable = totalAvailable;
                slotProvider.totalParked = totalParked;

                return percent;
            };

            slotProvider.carMoveToFirstAvailable = function (target) {
                if (!slotProvider.Slots || slotProvider.Slots.length === 0) {
                    baseService.showDialog({ $event: target, content: "Không có chỗ đậu xe!" });
                    return;
                }

                //
                // Find available slot
                var slot = undefined;
                for (var i = 0; i < slotProvider.Slots.length; i++) {
                    if (slotProvider.Slots[i].IsAvailable) {
                        slot = slotProvider.Slots[i];
                        break;
                    }
                }

                if (!slot) {
                    baseService.showDialog({ $event: target, content: "Không còn chỗ đậu xe trống!" });
                    return;
                }

                slotProvider.takePictureOrBook(slot);
            };
            //
            // Check the setting to take a picture or igone then book the slot
            slotProvider.takePictureOrBook = function (slot) {
                if ($scope.userInfo && $scope.userInfo.UserProfile && $scope.userInfo.UserProfile.AllowCaptureVehicleIn) {
                    $scope.browseImage(slot.ID);
                } else {
                    slotProvider.bookSlot(slot);
                }
            };
            //
            // Save Image which was taken, then book the slot
            slotProvider.saveSlotImage = function (slot) {
                if (slot.imageFile) {
                    slot.loading = true;
                    providerService.uploadBookingImage(slot.imageFile).then(function (response) {
                        if (response)
                            slot.ActiveBooking.VehicleImageFileName = response;

                        slotProvider.bookSlot(slot);
                    }, function () {
                        slot.loading = false;
                    });
                } else {
                    slotProvider.bookSlot(slot);
                }
            };

            slotProvider.bookSlot = function (slot) {
                slot.ActiveBooking = slot.ActiveBooking || {};
                var imageName = slot.ActiveBooking.VehicleImageFileName ? slot.ActiveBooking.VehicleImageFileName : '';
                var bookingInfo = {
                    "SlotProviderId": slotProvider.ID,
                    "SlotId": slot.ID,
                    "SlotName": slot.Name,
                    "BookedByUserId": $scope.userInfo.UserProfileId,
                    "BookedOn": new Date(),
                    "VehicleInfo": "",
                    "VehicleImageFileName": imageName,
                    "Price": slot.Price,
                    "Duration": 1
                };

                slot.loading = true;
                providerService.bookSlot(bookingInfo).then(function (slotBooking) {
                    if (slotBooking && slotBooking.ID) {
                        slot.ActiveBooking = slotBooking;
                        slot.IsAvailable = false;
                    }
                }).finally(function () { $timeout(function () { slot.loading = false; }, 200); });
            };

            slotProvider.releaseSlot = function (slot, target) {
                if (slot && slot.ActiveBooking) {
                    var confirm = $mdDialog.confirm()
                    .title('Xác nhận đưa xe ra.')
                    .textContent('')
                    .ariaLabel('Xác nhận đưa xe ra.')
                    .targetEvent(target)
                    .ok('Đúng, xe đã ra')
                    .cancel('Không');

                    $mdDialog.show(confirm).then(function () {
                        slot.loading = true;
                        providerService.releaseSlot(slot.ActiveBooking.ID).then(function () {
                            slot.IsAvailable = true;
                            slot.ActiveBooking = {};
                        }).finally(function () { $timeout(function () { slot.loading = false; }, 200); });
                    });
                }
            };

            slotProvider.reverseStateOfSlot = function (slot, target) {
                if (slot.IsAvailable) {
                    slotProvider.takePictureOrBook(slot);
                } else {
                    slotProvider.releaseSlot(slot, target);
                }
            };

            slotProvider.updateBookingImage = function (slot) {
                slot.loading = true;
                $timeout(function () {
                    providerService.updateBookingImage(slot.imageFile, slot.ActiveBooking.ID).then(function (newImagePath) {
                        slot.ActiveBooking.VehicleImageFileName = newImagePath;
                    }).finally(function () { $timeout(function () { slot.loading = false; }, 200); });
                }, 200);
            };
        };
        //
        // Fire after an image being selected.
        $scope.imageChanged = function (fileInput) {
            var spIndex = parseInt(fileInput.getAttribute("sp-index"));
            var slotIndex = parseInt(fileInput.getAttribute("slot-index"));
            var slotProvider = $scope.slotProviders[spIndex];
            var slot = slotProvider.Slots[slotIndex];
            slot.ActiveBooking = slot.ActiveBooking || {};

            if (fileInput.files && fileInput.files[0]) {
                if (slot.ActiveBooking && slot.ActiveBooking.ID) {
                    // Since slot already booked, change image
                    slotProvider.updateBookingImage(slot);
                }
                else if (slot.IsAvailable) {
                    var reader = new FileReader();
                    reader.onload = function (e) {
                        // Base 64
                        slot.ActiveBooking.VehicleImageFileName = e.target.result;
                        $scope.$apply();

                        slotProvider.saveSlotImage(slot);
                    };
                    reader.readAsDataURL(fileInput.files[0]);
                }
            }
        };
        //
        // Fire when an image being clicked.
        $scope.browseImage = function (slotId) {
            document.getElementById('slotProvider-SlotFile-' + slotId).click();
        };
        //
        // Update user profile when the setting "allow capture car in" is changed
        $scope.allowCaptureChange = function () {
            userService.updateUserProfile($scope.userInfo.UserProfile);
        };

        $scope.viewBookingImage = function (slot, target) {
            if (slot && slot.ActiveBooking && slot.ActiveBooking.VehicleImageFileName) {
                var content = '<img alt="" src="' + slot.ActiveBooking.VehicleImageFileName + '" class="booking-vehicle-image"/>';
                baseService.showDialog({ content: content, $event: target });
            }
        }
    });
})();