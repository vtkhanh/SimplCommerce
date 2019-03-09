/*global angular*/
(function () {
    angular
        .module('simplAdmin.orders')
        .controller('OrderListCtrl', OrderListCtrl);

    /* @ngInject */
    function OrderListCtrl(orderService, translateService) {
        const vm = this;
        let tableStateRef;
        vm.translate = translateService;
        vm.orders = [];

        orderService.getOrderStatus().then(function (result) {
            vm.orderStatus = result.data;
        });

        vm.getOrders = (tableState) => {
            tableStateRef = tableState;
            vm.isLoading = true;
            orderService.getOrdersForGrid(tableState).then(function (result) {
                vm.orders = result.data.items;
                tableState.pagination.numberOfPages = result.data.numberOfPages;
                vm.isLoading = false;

                updateCssClass(vm.orders);
            });
        };

        vm.exportOrders = () => orderService.exportOrders(tableStateRef);

        vm.showOrderStatus = (statusId) => {
            if (vm.orderStatus) {
                const status = _.find(vm.orderStatus, item => item.id === statusId);
                return status.name;
            }
        };

        vm.changeOrderStatus = (orderId, statusId) => {
            return orderService
                .changeOrderStatus(orderId, statusId)
                .then((result) => {
                    const updatedOrder = result.data;

                    updateOrderItem(updatedOrder);
                    
                    toastr.success("Saved successfully.");
                })
                .catch((response) => toastr.error(response.data.error));
        };

        vm.changeTrackingNumber = (orderId, trackingNumber) => {
            return orderService
                .changeTrackingNumber(orderId, trackingNumber)
                .then(() => toastr.success("Saved successfully."))
                .catch((response) => toastr.error(response.data.error));
        };

        vm.hasOrdersSelected = () => _.some(vm.orders, ['isSelected', true]);

        vm.updateMultipleStatuses = () => {
            if (!vm.selectedStatus) {
                toastr.error("Please select a status.");
                return;
            }

            const selectedIds = _(vm.orders).filter(order => order.isSelected).map(order => order.id).value();

            orderService
                .updateMultipleStatuses(selectedIds, vm.selectedStatus.id)
                .then((result) => {
                    const updatedOrders = result.data;

                    _.forEach(updatedOrders, (updatedOrder) => updateOrderItem(updatedOrder));

                    toastr.success("Saved successfully.");
                });
        };

        function updateOrderItem(updatedOrder) {
            const orderListItem = _.find(vm.orders, item => item.id === updatedOrder.orderId);

            orderListItem.statusId = updatedOrder.orderStatus;
            orderListItem.total = updatedOrder.orderTotal;
            orderListItem.cost = updatedOrder.orderTotalCost;
            orderListItem.completedOn = updatedOrder.completedOn;

            updateCssClassPerOrder(orderListItem);
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
