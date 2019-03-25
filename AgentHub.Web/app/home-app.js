(function () {
    "use strict";

    var module = angular.module("home", ['app', 'rzModule']);

    //Configuration for Angular UI routing.
    module.config([
        "$stateProvider", "$urlRouterProvider",
        function($stateProvider, $urlRouterProvider) {
            $urlRouterProvider.otherwise("");
            $stateProvider.state("home", { url: "", templateUrl: "/App/views/home.html", menu: "Home" });
        }
    ]);
})();