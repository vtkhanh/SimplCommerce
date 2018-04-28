/*global angular*/
(function () {
    angular
        .module('simplAdmin.orders')
        .controller('OrderFormCtrl', OrderFormCtrl);

    /* @ngInject */
    function OrderFormCtrl($state, $stateParams, translateService,
        orderService, userService, productService) {

        const vm = this;
        vm.translate = translateService;

        init();

        vm.searchCustomers = (query) =>
            userService
                .searchCustomers(query)
                .then((result) => result.data)
                .catch((response) => toastr.error(response.data.error));

        vm.searchProducts = (query) =>
            productService
                .searchProducts(query)
                .then((result) => {
                    return result.data;
                })
                .catch((response) => toastr.error(response.data.error));

        vm.addToCart = (product) => {
            const addedItem = _.find(vm.orderItems, { productId: product.id });
            if (addedItem) {
                addedItem.quantity = (addedItem.quantity || 0) + 1;
                vm.updateSubtotal(addedItem);
            } else {
                const orderItem = {
                    productId: product.id,
                    productName: product.name,
                    productPrice: product.price,
                    display: product.display,
                    productImage: product.thumbnailImageUrl,
                    quantity: 1,
                    subtotal: product.price
                };
                vm.orderItems.push(orderItem);
                vm.updateOrderSubTotal();
            }
        };

        vm.removeFromCart = (product) => {
            _.remove(vm.orderItems, item => item.productId === product.productId);
            vm.updateOrderSubTotal();
        };

        vm.updateSubtotal = (orderItem) => {
            orderItem.subtotal = orderItem.productPrice * orderItem.quantity;
            vm.updateOrderSubTotal();
        };

        vm.updateOrderTotal = () => {
            vm.orderTotal = vm.orderSubTotal + vm.shippingAmount - vm.discount;
        };

        vm.updateOrderSubTotal = () => {
            vm.orderSubTotal = _.sumBy(vm.orderItems, item => item.subtotal);
            vm.updateOrderTotal();
        }

        vm.save = () => {
            toastr.success("Saved successfully.");
        };

        function init() {
            vm.orderId = $stateParams.id;
            vm.customer = null;
            vm.selectedProduct = null;
            vm.orderItems = [];
            vm.orderSubTotal = 0;
            vm.shippingAmount = 0;
            vm.discount = 0;
            vm.orderTotal = 0;
        }

        // TODO: move to a global scope
        Number.prototype.toCurrency = function () {
            // TODO: Currently use VND format hard-codedly
            return this.toLocaleString('vi-VN', {
                style: 'currency',
                currency: 'VND'
            });
        }
    }
})();