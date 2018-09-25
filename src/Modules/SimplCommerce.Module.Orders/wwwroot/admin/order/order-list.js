/*global angular*/
(function () {
    angular
        .module('simplAdmin.orders')
        .controller('OrderListCtrl', OrderListCtrl);

    /* @ngInject */
    function OrderListCtrl(orderService, translateService) {
        var vm = this;
        vm.translate = translateService;
        vm.orders = [];

        orderService.getOrderStatus().then(function (result) {
            vm.orderStatus = result.data;
        });

        vm.getOrders = function getOrders(tableState) {
            vm.isLoading = true;
            orderService.getOrdersForGrid(tableState).then(function (result) {
                vm.orders = result.data.items;
                tableState.pagination.numberOfPages = result.data.numberOfPages;
                vm.isLoading = false;

                updateCssClass(vm.orders);
            });
        };

        vm.showOrderStatus = function (statusId) {
            if (vm.orderStatus) {
                const status = _.find(vm.orderStatus, item => item.id === statusId);
                return status.name;
            }
        }

        vm.changeOrderStatus = function (orderId, statusId) {
            return orderService
                .changeOrderStatus(orderId, statusId)
                .then((result) => {
                    const updatedOrder = result.data;
                    const orderListItem = _.find(vm.orders, item => item.id === orderId);

                    orderListItem.total = updatedOrder.orderTotal;
                    orderListItem.cost = updatedOrder.orderTotalCost;

                    updateCssClassPerOrder(orderListItem);

                    toastr.success("Saved successfully.");
                })
                .catch((response) => toastr.error(response.data.error));
        }

        vm.changeTrackingNumber = function (orderId, trackingNumber) {
            return orderService
                .changeTrackingNumber(orderId, trackingNumber)
                .then(() => toastr.success("Saved successfully."))
                .catch((response) => toastr.error(response.data.error));
        }

        function updateCssClass(orders) {
            for (let order of orders) {
                updateCssClassPerOrder(order);
            }
        }

        function updateCssClassPerOrder(order) {
            switch (order.statusId) {
                case 6: // Complete
                    order.cssClass = 'success';
                    break;
                case 8: // Cancelled
                    order.cssClass = 'danger';
                    break;
                default:
                    order.cssClass = '';
                    break;
            }

        }

    }
})();
