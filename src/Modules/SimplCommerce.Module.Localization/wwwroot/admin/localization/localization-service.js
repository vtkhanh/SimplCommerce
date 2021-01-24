/*global angular*/
(function () {
    angular
        .module('simplAdmin.localization')
        .factory('localizationService', localizationService);

    /* @ngInject */
    function localizationService($http) {
        var service = {
            getCultures: getCultures,
            getResources: getResources,
            updateResources: updateResources,
            createResource: createResource
        };
        return service;

        function getCultures() {
            return $http.get('api/localization/get-cultures');
        }

        function getResources(cultureId) {
            return $http.get('api/localization/get-resources?cultureId=' + cultureId);
        }

        function updateResources(cultureId, resources) {
            return $http.post('api/localization/update-resources?cultureId=' + cultureId, resources);
        }

        function createResource(resource) {
            return $http.post('api/localization/create-resource', resource);
        }
    }
})();
