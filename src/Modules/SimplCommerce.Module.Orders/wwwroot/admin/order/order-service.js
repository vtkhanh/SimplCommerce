/*global angular, jQuery*/
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
            exportOrders: exportOrders
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
            //const downloadPath = 'api/orders/export?' + $.param(params);
            //window.open(downloadPath, '_blank', ''); 
            //return $http
            //    .get(downloadPath, { responseType: 'arraybuffer' })
            //    .then(downloadFile);
            return $http({
                method: 'GET',
                url: 'api/orders/export',
                params: params,
                responseType: 'arraybuffer'
            }).then(downloadFile);
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

        function downloadFile(data) {
            const headers = data.headers();
            const contentDisposition = headers['content-disposition'] || '';
            const filename = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/.exec(contentDisposition)[1].replace(/"/g,'');

            const contentType = headers['content-type'];

            const linkElement = document.createElement('a');
            try {
                const blob = new Blob([data.data], { type: contentType });
                const url = window.URL.createObjectURL(blob);

                linkElement.setAttribute('href', url);
                linkElement.setAttribute("download", filename);

                const clickEvent = new MouseEvent("click", {
                    "view": window,
                    "bubbles": true,
                    "cancelable": false
                });
                linkElement.dispatchEvent(clickEvent);
            } catch (ex) {
                console.log(ex);
            }
        }
    }
})(jQuery);
