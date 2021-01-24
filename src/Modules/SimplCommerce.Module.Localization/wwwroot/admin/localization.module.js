/*global angular*/
(function () {
    'use strict';

    angular
        .module('simplAdmin.localization', [])
        .config(['$stateProvider', function ($stateProvider) {
            $stateProvider.state('localization', {
                url: '/localization',
                templateUrl: "template/localization/translation-list",
                controller: 'LocalizationFormCtrl as vm'
            });
        }]);
})();
