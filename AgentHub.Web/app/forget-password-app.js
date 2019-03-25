(function () {
    "use strict";

    var module = angular.module("forget-password", ['app']);

    //Configuration for Angular UI routing.
    module.config([
        "$stateProvider", "$urlRouterProvider",
        function($stateProvider, $urlRouterProvider) {
            $urlRouterProvider.otherwise("");
            $stateProvider.state("home", { url: "", controller: "forgetPasswordController", templateUrl: "/App/views/forget-password.html", menu: "Home" });
        }
    ]);
})();