(function() {
    "use strict";

    var app = angular.module("app", [
        "ui.router",
        "ngCookies",
        "ngAnimate",
        "ngMaterial",
        "uiGmapgoogle-maps",
        "angular-input-stars",
        "angular-carousel",
        "satellizer",
        "720kb.background",
        "fcsa-number",
        "mdPickers"
    ]);

    app.constant('globalConstants', {
        APIKey: "E6C4AC5F-7CFE-4B3B-ADBB-4EE9A6AD86FC:857E30F4-26D0-46CC-8679-D440E5538C4C",
        GoogleClientId: "30530709154-32prkot8i4ajvgu54v7j8jebgdtg2rtg.apps.googleusercontent.com",
        FacebookClientId: "",
        BusinessOwner: 0,
        Provider: 1,
        Consumer: 2
    });

    //Configuration
    app.config(function(uiGmapGoogleMapApiProvider) {
        uiGmapGoogleMapApiProvider.configure({
            key: 'AIzaSyAGccTvr_HfDoZsjG4vdsMiX1RUR9zIpy8',
            v: '3.exp', //defaults to latest 3.X anyhow
            libraries: 'weather,geometry,visualization,places',
            language: "vi"
        });
    });

    app.config(function($mdThemingProvider) {
        $mdThemingProvider.theme('default').primaryPalette('blue');
    });

    app.config(function ($authProvider, globalConstants) {
        $authProvider.tokenName = 'token';
        $authProvider.tokenPrefix = 'satellizer';
        $authProvider.authHeader = 'Authorization';
        $authProvider.authToken = 'Bearer';
        $authProvider.storageType = 'localStorage';

        // Facebook
        $authProvider.facebook({
            name: 'facebook',
            clientId: globalConstants.FacebookClientId,
            url: '/api/Account/CompleteOAuthSignIn',
            authorizationEndpoint: '/api/Account/ExternalLogin?provider=Facebook&response_type=token&APIKey=' + globalConstants.APIKey + '&redirect_uri=https://www.kuuparking.com/',
            redirectUri: window.location.origin,
            requiredUrlParams: ['display', 'scope'],
            scope: ['email'],
            scopeDelimiter: ',',
            display: 'popup',
            type: '2.0',
            popupOptions: { width: 1024, height: 768 }
        });
        // Google        
        $authProvider.google({
            name: 'google',
            clientId: globalConstants.GoogleClientId,
            url: '/api/Account/CompleteOAuthSignIn',
            authorizationEndpoint: '/api/Account/ExternalLogin?provider=Google&response_type=token&APIKey=' + globalConstants.APIKey + '&redirect_uri=https://www.kuuparking.com/',
            redirectUri: window.location.origin,
            requiredUrlParams: ['scope'],
            optionalUrlParams: ['display'],
            scope: ['profile', 'email'],
            scopePrefix: 'openid',
            scopeDelimiter: ' ',
            display: 'popup',
            type: '2.0',
            popupOptions: { width: 1024, height: 768 }
        });
    });

    app.run(function ($http, $cookies, globalConstants) {
        $http.defaults.headers.common.Authorization = $cookies.get('accessToken');
        $http.defaults.headers.common.APIKey = globalConstants.APIKey;
    });
})();