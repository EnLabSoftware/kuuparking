(function () {
    "use strict";

    var module = angular.module("provider-register", ['app']);

    //Configuration for Angular UI routing.
    module.config([
        "$stateProvider", "$urlRouterProvider",
        function($stateProvider, $urlRouterProvider) {
            $urlRouterProvider.otherwise("");
            $stateProvider.state("register", { url: "", controller: "providerRegisterController", templateUrl: "/App/views/provider/register.html", menu: "Register" });
            $stateProvider.state("privacy", { url: "/dieu-khoan-ca-nhan", controller: 'providerRegisterController', templateUrl: "/App/views/provider/privacy-statement.html", menu: "Privacy" });
        }
    ]);
})();