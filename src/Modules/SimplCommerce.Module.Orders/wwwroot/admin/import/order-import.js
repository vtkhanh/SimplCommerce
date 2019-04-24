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

        vm.upload = () => {
            orderImportService
                .upload(vm.file)
                .then(result => toastr.success(result.data));
        }
    }
})();
