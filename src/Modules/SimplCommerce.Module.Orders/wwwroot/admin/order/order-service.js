/*global angular*/
(function () {
    angular
        .module('simplAdmin.orders')
        .factory('orderService', orderService);

    /* @ngInject */
    function orderService($http) {
        var service = {
            getOrders: getOrders,
            getOrdersForGrid: getOrdersForGrid,
            getOrderForEditing: getOrderForEditing,
            getOrder: getOrder,
            createOrder: createOrder,
            updateOrder: updateOrder,
            getOrderStatus: getOrderStatus,
            changeOrderStatus: changeOrderStatus,
            changeTrackingNumber: changeTrackingNumber,
            getStatusList: getStatusList,
            getPaymentList: getPaymentList
        };
        return service;

        function createOrder(params) {
            return $http.post('api/orders', params);
        }

        function updateOrder(params) {
            return $http.put(`api/orders`, params);
        }

        function getOrdersForGrid(params) {
            return $http.post('api/orders/list', params);
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
    }
})();
