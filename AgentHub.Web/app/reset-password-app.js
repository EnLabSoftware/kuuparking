(function () {
    "use strict";

    var module = angular.module("reset-password", ['app']);

    //Configuration for Angular UI routing.
    module.config([
        "$stateProvider", "$urlRouterProvider",
        function($stateProvider, $urlRouterProvider) {
            $urlRouterProvider.otherwise("");
            $stateProvider.state("home", { url: "", controller: "resetPasswordController", templateUrl: "/App/views/reset-password.html", menu: "Home" });
        }
    ]);
})();