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

        vm.searchCustomers = (query = '') =>
            userService
                .searchCustomers(query)
                .then((result) => {
                    // vm.customers = result.data;
                    return result.data;
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
            if (!product) return;

            const addedItem = _.find(vm.orderItems, { productId: product.id });
            if (addedItem) {
                addedItem.quantity = (addedItem.quantity || 0) + 1;
                vm.updateSubtotal(addedItem);
            } else {
                const orderItem = {
                    productId: product.id,
                    productName: product.name,
                    productSku: product.sku,
                    productPrice: product.price,
                    productCost: product.cost,
                    productStock: product.stock,
                    stock: product.stock,
                    productImage: product.thumbnailImageUrl,
                    quantity: 1,
                    oldQuantity: 0
                };
                vm.orderItems.push(orderItem);
                vm.updateSubtotal(orderItem);
            }
        };

        vm.removeFromCart = (product) => {
            _.remove(vm.orderItems, item => item.productId === product.productId);
            vm.updateOrderSubTotal();
        };

        vm.onShippingAmountUpdate = () => {
            vm.updateOrderTotal();

            if (!vm.shippingCost || vm.shippingCost < vm.shippingAmount) {
                vm.shippingCost = vm.shippingAmount;
                vm.onShippingCostUpdate();
            }
        }

        vm.onShippingCostUpdate = () => {
            vm.updateOrderTotalCost();
        }

        vm.updateSubtotal = (orderItem) => {
            orderItem.subTotal = orderItem.productPrice * orderItem.quantity;
            orderItem.stock = orderItem.productStock - orderItem.quantity + orderItem.oldQuantity;

            vm.updateOrderSubTotal();

            vm.updateSubtotalCost(orderItem);
        };

        vm.updateSubtotalCost = (orderItem) => {
            orderItem.subTotalCost = orderItem.productCost * orderItem.quantity;
            vm.updateOrderSubTotalCost();
        };

        vm.updateOrderSubTotal = () => {
            vm.orderSubTotal = _.sumBy(vm.orderItems, item => item.subTotal);
            vm.updateOrderTotal();
        }

        vm.updateOrderSubTotalCost = () => {
            vm.orderSubTotalCost = _.sumBy(vm.orderItems, item => item.subTotalCost);
            vm.updateOrderTotalCost();
        }

        vm.updateOrderTotal = () => {
            vm.orderTotal = vm.orderSubTotal + vm.shippingAmount - vm.discount;
        };

        vm.updateOrderTotalCost = () => {
            vm.orderTotalCost = vm.orderSubTotalCost + vm.shippingCost;
        };

        vm.save = () => {
            const params = {
                customerId: vm.customer.id,
                trackingNumber: vm.trackingNumber,
                shippingAmount: vm.shippingAmount,
                shippingCost: vm.shippingCost,
                discount: vm.discount,
                subTotal: vm.orderSubTotal,
                orderTotal: vm.orderTotal,
                orderTotalCost: vm.orderTotalCost,
                orderStatus: vm.orderStatus || 0, // Default: Pending
                orderItems: vm.orderItems
            };
            if (vm.orderId === 0) {
                orderService.createOrder(params)
                    .then((result) => {
                        $state.go('order-edit', { id: result.data.id });
                        toastr.success("Saved successfully.");
                    })
                    .catch((response) => processError(response.data));
            } else {
                params.orderId = vm.orderId;
                orderService.updateOrder(params)
                    .then((result) => toastr.success("Saved successfully."))
                    .catch((response) => processError(response.data));
            }
        };

        function init() {
            vm.orderId = $stateParams.id || 0;

            if (vm.orderId === 0) { // Create order
                vm.customer = null;
                vm.selectedProduct = null;
                vm.trackingNumber = null;
                vm.orderItems = [];
                vm.orderSubTotal = 0;
                vm.shippingAmount = 0;
                vm.shippingCost = 0;
                vm.discount = 0;
                vm.orderTotal = 0;
                vm.orderStatus = 0; // Pending

                vm.pageTitle = "Create order";
            } else {
                orderService.getOrderForEditing(vm.orderId)
                    .then((result) => {
                        const order = result.data;

                        vm.searchCustomers()
                            .then((data) => {
                               vm.customer = _.find(data, item => item.id === order.customerId);
                            });

                        vm.selectedProduct = null;

                        vm.trackingNumber = order.trackingNumber;
                        vm.orderSubTotal = order.subTotal;
                        vm.orderSubTotalCost = order.subTotalCost;
                        vm.shippingAmount = order.shippingAmount;
                        vm.shippingCost = order.shippingCost;
                        vm.discount = order.discount;
                        vm.orderTotal = order.orderTotal;
                        vm.orderTotalCost = order.orderTotalCost;
                        vm.orderStatus = order.orderStatus;
                        vm.orderStatusList = order.orderStatusList;
                        vm.orderItems = order.orderItems;
                        vm.orderItems.forEach(element => {
                            element.productStock = element.stock;
                            element.oldQuantity = element.quantity;
                        });
                    })
                    .catch((response) => processError(response.data));

                vm.pageTitle = `Edit order ${vm.orderId}`;
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

        init();

    }
})();
