(function () {
    "use strict";

    var register = angular.module("about", ['app']);

    //Configuration for Angular UI routing.
    register.config([
        "$stateProvider", "$urlRouterProvider",
        function ($stateProvider, $urlRouterProvider) {
            $urlRouterProvider.otherwise("");
            $stateProvider.state("home", { url: "", controller: 'aboutUsController', templateUrl: "/App/views/about-us.html", menu: "Home" });
        }
    ]);
})();