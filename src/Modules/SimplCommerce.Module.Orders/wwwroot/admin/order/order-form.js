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

        vm.orderId = $stateParams.id;
        vm.customer = null;
        vm.selectedProduct = null;
        vm.products = [];

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

        // test only
        vm.selectedChange = () => {
            console.log(vm.selectedProduct);
        }

    }
})();