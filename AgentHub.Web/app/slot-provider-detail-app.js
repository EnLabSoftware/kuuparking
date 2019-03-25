(function () {
    "use strict";

    var register = angular.module("slot-provider-detail", ['app']);

    //Configuration for Angular UI routing.
    register.config([
        "$stateProvider", "$urlRouterProvider",
        function ($stateProvider, $urlRouterProvider) {
            $urlRouterProvider.otherwise("");
            $stateProvider.state("home", { url: "", controller: 'slotProviderDetailController', templateUrl: "/App/views/provider/detail.html", menu: "Home" });
        }
    ]);
})();