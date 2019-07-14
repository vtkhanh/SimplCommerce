/*global angular*/
(function () {
    angular
        .module('simplAdmin.orders')
        .factory('orderImportService', orderImportService);

    /* @ngInject */
    function orderImportService($http, Upload) {
        var service = {
            upload: upload,
            getOrderFilesForGrid: getOrderFilesForGrid,
            getImportResult: getImportResult,
            runImport: runImport
        };
        return service;

        function upload(orderFile) {
            return Upload.upload({
                url: 'api/order-import/upload',
                method: 'POST',
                data: {
                    orderFile
                }
            });
        }

        function getOrderFilesForGrid(params) {
            return $http.post('api/order-import/list', params);
        }

        function getImportResult(importResultId) {
            return $http.get('api/order-import/import-result/' + importResultId);
        }

        function runImport(orderFileId) {
            return $http.post('api/order-import/run-import/' + orderFileId);
        }
    }
})();
