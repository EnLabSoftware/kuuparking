(function () {
    "use strict";

    var register = angular.module("slot-providers", ['app']);

    //Configuration for Angular UI routing.
    register.config([
        "$stateProvider", "$urlRouterProvider",
        function ($stateProvider, $urlRouterProvider) {
            $urlRouterProvider.otherwise("");
            $stateProvider.state("home", { url: "", controller: 'slotProviderListController', templateUrl: "/App/views/provider/list.html", menu: "Home" });
        }
    ]);
})();