(function () {
    "use strict";

    var module = angular.module("user-signin", ['app']);

    //Configuration for Angular UI routing.
    module.config([
        "$stateProvider", "$urlRouterProvider",
        function($stateProvider, $urlRouterProvider) {
            $urlRouterProvider.otherwise("");
            $stateProvider.state("home", { url: "", templateUrl: "/App/views/user/signin.html", menu: "Home" });
        }
    ]);
})();