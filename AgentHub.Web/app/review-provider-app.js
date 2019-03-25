(function () {
    "use strict";

    var register = angular.module("review-provider", ['app']);

    //Configuration for Angular UI routing.
    register.config([
        "$stateProvider", "$urlRouterProvider",
        function ($stateProvider, $urlRouterProvider) {
            $urlRouterProvider.otherwise("");
            $stateProvider.state("provider", { url: "", controller: 'reviewProviderController', templateUrl: "/App/views/review-provider.html", menu: "Provider" });
        }
    ]);
})();