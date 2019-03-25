(function () {
    "use strict";

    var register = angular.module("provider", ['app']);

    //Configuration for Angular UI routing.
    register.config([
        "$stateProvider", "$urlRouterProvider",
        function($stateProvider, $urlRouterProvider) {
            $urlRouterProvider.otherwise("/");
            $stateProvider.state("provider", { url: "/", controller: 'providerDashboardController', templateUrl: "/App/views/provider/dashboard.html", menu: "Provider" });
            $stateProvider.state("parkinglot", { url: "/thong-tin-bai-dau-xe", controller: 'providerParkingSlotController', templateUrl: "/App/views/provider/parking-slot.html", menu: "Parking Slot" });
            $stateProvider.state("charts", { url: "/bao-cao", controller: 'providerChartsController', templateUrl: "/App/views/provider/charts.html", menu: "Charts" });
            $stateProvider.state("profile", { url: "/thong-tin-tai-khoan", controller: 'providerProfileController', templateUrl: "/App/views/provider/profile.html", menu: "Profile" });
        }
    ]);
})();