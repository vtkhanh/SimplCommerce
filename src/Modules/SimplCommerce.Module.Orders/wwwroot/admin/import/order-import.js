/*global angular*/
(function () {
    angular
        .module('simplAdmin.orders')
        .controller('OrderImportCtrl', OrderImportCtrl);

    /* @ngInject */
    function OrderImportCtrl(orderImportService, translateService) {
        var vm = this;
        vm.translate = translateService;
        vm.file = null;
        vm.isLoading = true;

        vm.upload = () => {
            orderImportService
                .upload(vm.file)
                .then(result => toastr.success(result.data));
        }

        init();

        function init() {
            orderImportService
                .getOrderFilesForGrid()
                .then(result => {
                    vm.orderFiles = result.data;
                    vm.isLoading = false;
                });
        }
    }
})();
