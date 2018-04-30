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

        await init();

        vm.searchCustomers = (query) =>
            userService
                .searchCustomers(query)
                .then((result) => {
                    vm.customers = result.data;
                })
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
            const params = {
                customerId: vm.customer.id,
                shippingAmount: vm.shippingAmount,
                discount: vm.discount,
                subTotal: vm.orderSubTotal,
                orderTotal: vm.orderTotal,
                orderItems: vm.orderItems
            };
            orderService.createOrder(params)
                .then((result) => toastr.success("Saved successfully."))
                .catch((response) => processError(response.data));
        };

        async function init() {
            vm.orderId = $stateParams.id;

            if (!orderId) { // Create order
                vm.customer = null;
                vm.selectedProduct = null;
                vm.orderItems = [];
                vm.orderSubTotal = 0;
                vm.shippingAmount = 0;
                vm.discount = 0;
                vm.orderTotal = 0;

                vm.pageTitle = "Create order";
            } else {
                orderService.getOrderForEditing(vm.orderId)
                    .then((result) => {
                        const order = result.data;
                        await vm.searchCustomers();

                        vm.customer = _.find(vm.customers, item => item.id === order.customerId);
                        vm.selectedProduct = null;

                        vm.orderItems = order.orderItems;
                        vm.orderSubTotal = order.subTotal;
                        vm.shippingAmount = order.shippingAmount;
                        vm.discount = order.discount;
                        vm.orderTotal = order.orderTotal;
                    })
                    .catch((response) => processError(response.data));

                vm.pageTitle = `Edit order {vm.orderId}`;
            }
        }

        function processError(error) {
            vm.validationErrors = [];
            if (error && angular.isObject(error)) {
                for (let key in error) {
                    vm.validationErrors.push(error[key][0]);
                }
            } else {
                vm.validationErrors.push('Could not add product.');
            }
        }

        // TODO: move to global scope
        Number.prototype.toCurrency = function () {
            // TODO: Currently use VND format hard-codedly
            return this.toLocaleString('vi-VN', {
                style: 'currency',
                currency: 'VND'
            });
        }
    }
})();