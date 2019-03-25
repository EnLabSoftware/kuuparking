(function () {
    "use strict";

    var module = angular.module("provider-signin", ['app']);

    //Configuration for Angular UI routing.
    module.config([
        "$stateProvider", "$urlRouterProvider",
        function($stateProvider, $urlRouterProvider) {
            $urlRouterProvider.otherwise("");
            $stateProvider.state("home", { url: "", controller: "providerSignInController", templateUrl: "/App/views/provider/signin.html", menu: "Home" });
        }
    ]);
})();