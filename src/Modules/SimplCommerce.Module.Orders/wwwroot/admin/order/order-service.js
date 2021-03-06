﻿/*global angular, jQuery*/
(function ($) {
    angular
        .module('simplAdmin.orders')
        .factory('orderService', orderService);

    /* @ngInject */
    function orderService($http) {
        var service = {
            createOrder: createOrder,
            updateOrder: updateOrder,
            updateMultipleStatuses: updateMultipleStatuses,
            changeOrderStatus: changeOrderStatus,
            changeTrackingNumber: changeTrackingNumber,
            getOrders: getOrders,
            getOrdersForGrid: getOrdersForGrid,
            getOrderForEditing: getOrderForEditing,
            getOrder: getOrder,
            getOrderStatus: getOrderStatus,
            getStatusList: getStatusList,
            getPaymentList: getPaymentList,
            exportOrders: exportOrders,
            getShopeeFee: getShopeeFee
        };
        return service;

        function createOrder(params) {
            return $http.post('api/orders', params);
        }

        function updateOrder(params) {
            return $http.put(`api/orders`, params);
        }

        function updateMultipleStatuses(orderIds, status) {
            return $http.put('api/orders/update-multiple-statuses', { orderIds, status });
        }

        function getOrdersForGrid(params) {
            return $http.post('api/orders/list', params);
        }

        function exportOrders(params) {
            return $http.post('api/orders/export', params, { responseType: 'arraybuffer' }).then(downloadFile);
        }

        function getOrders(status, numRecords) {
            return $http.get('api/orders?status=' + status + '&numRecords=' + numRecords);
        }

        function getOrder(orderId) {
            return $http.get('api/orders/' + orderId);
        }

        function getOrderForEditing(orderId) {
            return $http.get('api/orders/edit/' + orderId);
        }

        function getOrderStatus() {
            return $http.get('api/orders/order-status');
        }

        function changeOrderStatus(orderId, statusId) {
            return $http.put('api/orders/change-order-status', { orderId, status: statusId });
        }

        function changeTrackingNumber(orderId, trackingNumber) {
            return $http.put('api/orders/change-tracking-number', { orderId, trackingNumber });
        }

        function getStatusList() {
            return $http.get('api/orders/status-list');
        }

        function getPaymentList() {
            return $http.get('api/orders/payment-list');
        }

        function getShopeeFee() {
            return $http.get('api/orders/shopee-fee');
        }
    }
})(jQuery);
