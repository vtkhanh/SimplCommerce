/*global angular*/
(function () {
    angular
        .module('simplAdmin.orders')
        .factory('orderReportService', orderReportService);

    /* @ngInject */
    function orderReportService($http) {
        var service = {
            getRevenueReport: getRevenueReport,
            getSellers: getSellers
        };
        return service;

        function getRevenueReport(createdById) {
            const url = `api/order-reports/revenue-report?createdById=${createdById || 0}`;
            return $http.get(url);
        }

        function getSellers() {
            return $http.get('api/users/seller-list');
        }
    }
})();
