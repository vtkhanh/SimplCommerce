/*global angular*/
(function () {
    angular
        .module('simplAdmin.orders')
        .factory('orderReportService', orderReportService);

    /* @ngInject */
    function orderReportService($http) {
        var service = {
            getRevenueReport: getRevenueReport,
        };
        return service;

        function getRevenueReport(time) {
            return $http.post('api/order-reports/revenue-report', time);
        }
       }
    }
})();
