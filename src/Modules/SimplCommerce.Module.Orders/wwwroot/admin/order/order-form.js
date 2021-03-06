/*global angular*/
(function () {
    angular
        .module('simplAdmin.orders')
        .controller('OrderFormCtrl', OrderFormCtrl);

    /* @ngInject */
    function OrderFormCtrl($scope, $state, $stateParams, translateService,
        orderService, userService, productService) {

        const OrderPendingStatus = 0;
        const vm = this;

        vm.translate = translateService;
        vm.formIsDisabled = false;

        vm.searchCustomers = (query = '') =>
            userService
                .searchCustomers(query)
                .then((result) => {
                    return result.data;
                })
                .catch((response) => toastr.error(response.data.error));

        vm.searchProducts = (query) =>
            productService
                .searchProducts(query, false)
                .then((result) => {
                    return result.data;
                })
                .catch((response) => toastr.error(response.data.error));

        vm.addToCart = (product) => {
            if (!product) return;

            const addedItem = _.find(vm.orderItems, {
                productId: product.id
            });
            if (addedItem) {
                addedItem.quantity = (addedItem.quantity || 0) + 1;
                vm.updateSubtotal(addedItem);
            } else {
                const orderItem = {
                    productId: product.id,
                    productName: product.name,
                    productSku: product.sku,
                    originalPrice: product.price,
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

            toastr.success("Added");

            // Clear search text
            vm.productQuery = '';
            vm.selectedProduct = undefined;
        };

        vm.removeFromCart = (product) => {
            if (confirm('Are you sure?'))
            {
                _.remove(vm.orderItems, item => item.productId === product.productId);
                vm.updateOrderSubTotal();

                $scope.orderForm.$setDirty();
            }
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

        vm.onShopeeOrderUpdate = () => {
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
            vm.orderTotalCost = vm.orderSubTotalCost + vm.shippingCost + vm.getShopeeCharge();
        };

        vm.save = () => {
            vm.formIsDisabled = true;

            const params = {
                customerId: vm.customer.id,
                trackingNumber: vm.trackingNumber,
                shippingAmount: vm.shippingAmount,
                shippingCost: vm.shippingCost,
                discount: vm.discount,
                orderStatus: parseInt(vm.orderStatus) || OrderPendingStatus,
                paymentProviderId: parseInt(vm.paymentProviderId),
                orderItems: vm.orderItems,
                note: vm.note,
                isShopeeOrder: vm.isShopeeOrder
            };
            if (vm.orderId === 0) {
                orderService.createOrder(params)
                    .then((result) => {
                        $state.go('order-edit', {
                            id: result.data.id
                        });
                        toastr.success("Saved successfully.");
                    })
                    .catch((response) => processError(response.data))
                    .finally(() => vm.formIsDisabled = false);
            } else {
                params.orderId = vm.orderId;
                orderService.updateOrder(params)
                    .then(() => {
                        // Order is Cancelled
                        if (parseInt(params.orderStatus) === 8) {
                            $state.reload();
                        }
                        toastr.success("Saved successfully.");
                    })
                    .catch((response) => processError(response.data))
                    .finally(() => vm.formIsDisabled = false);
            }
        };

        vm.getShopeeCharge = () => {
            return vm.isShopeeOrder ? (vm.shopeeFee / 100) * vm.orderSubTotal : 0;
        }

        vm.printInvoice = (eleId) => {
            // TODO: A shit way to print an element now, yet to figure out a better way
            const printWindow = window.open('', '_blank', 'Print Invoice');

            printWindow.document.open();
            printWindow.document.write('<html><head><title>Print Invoice</title>');
            // CSS & JS scripts
            printWindow.document.write('<link href="https://fonts.googleapis.com/css?family=Roboto" rel="stylesheet">');
            printWindow.document.write('<link rel="stylesheet" type="text/css" href="https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/3.3.6/css/bootstrap.min.css" />');
            printWindow.document.write('<link rel="stylesheet" type="text/css" href="modules/orders/admin/order/print-invoice.css" />');
            printWindow.document.write('</head><body>');
            printWindow.document.write(document.getElementById(eleId).innerHTML);
            printWindow.document.write('</body></html>');

            setTimeout(function () {
                printWindow.print();
                printWindow.close();
            }, 1000);
        };

        function init() {
            vm.orderId = parseInt($stateParams.id) || 0;
            vm.hasInvoice = vm.orderId > 0;

            orderService.getShopeeFee()
                .then(result => vm.shopeeFee = result.data)
                .catch((response) => processError(response.data));

            if (vm.orderId === 0) { // Create order
                orderService.getStatusList()
                    .then(result => vm.orderStatusList = result.data)
                    .catch((response) => processError(response.data));

                orderService.getPaymentList()
                    .then(result => vm.paymentProviderList = result.data)
                    .catch((response) => processError(response.data));
                
                vm.canEdit = true;
                vm.customer = null;
                vm.selectedProduct = null;
                vm.trackingNumber = null;
                vm.note = null;
                vm.isShopeeOrder = true;
                vm.orderItems = [];
                vm.orderSubTotal = 0;
                vm.shippingAmount = 0;
                vm.shippingCost = 0;
                vm.discount = 0;
                vm.orderTotal = 0;
                vm.orderStatus = OrderPendingStatus;

                vm.pageTitle = "Create order";
            } else {
                orderService.getOrderForEditing(vm.orderId)
                    .then((result) => {
                        const order = result.data;
                        const selectedPayment = _.find(order.paymentProviderList, item => item.value == order.paymentProviderId);

                        vm.searchCustomers()
                            .then((data) => {
                                vm.customer = _.find(data, item => item.id === order.customerId);
                            });

                        vm.selectedProduct = null;

                        vm.canEdit = order.canEdit;
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
                        vm.paymentProviderId = order.paymentProviderId;
                        vm.paymentProvider = selectedPayment != undefined ? selectedPayment.text : '';
                        vm.paymentProviderList = order.paymentProviderList;
                        vm.note = order.note;
                        vm.isShopeeOrder = order.isShopeeOrder;
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
                    vm.validationErrors.push(error[key]);
                }
            } else {
                vm.validationErrors.push('Could not add product.');
            }
        }
        
        init();

    }
})();
