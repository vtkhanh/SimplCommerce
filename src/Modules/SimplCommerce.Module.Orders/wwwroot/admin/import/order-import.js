/*global angular*/
(function () {
    angular
        .module('simplAdmin.orders')
        .controller('OrderImportCtrl', OrderImportCtrl);

    /* @ngInject */
    function OrderImportCtrl($state, orderImportService, translateService) {
        const vm = this;
        vm.translate = translateService;
        vm.file = null;
        vm.isLoading = false;

        vm.upload = () => {
            orderImportService
                .upload(vm.file)
                .then(result => {
                    toastr.success(result.data);

                    $state.reload();
                });
        }

        vm.getOrderFiles = (tableState) => {
            vm.isLoading = true;

            orderImportService
                .getOrderFilesForGrid(tableState)
                .then(result => {
                    vm.orderFiles = result.data.items;
                    tableState.pagination.numberOfPages = result.data.numberOfPages;
                    vm.isLoading = false;
                });
        };
    }
})();
