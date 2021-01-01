/*global angular, confirm*/
(function () {
    angular
        .module('simplAdmin.catalog')
        .controller('SupplierListCtrl', SupplierListCtrl);

    /* @ngInject */
    function SupplierListCtrl(supplierService, translateService) {
        var vm = this;
        vm.translate = translateService;
        vm.suppliers = [];

        vm.getSuppliers = function () {
            supplierService.getSuppliers().then(function (result) {
                vm.suppliers = result.data;
            });
        };

        vm.deleteSupplier = function (supplier) {
            bootbox.confirm('Are you sure you want to delete this supplier: ' + supplier.name, function (result) {
                if (result) {
                    supplierService.deleteSupplier(supplier.id)
                        .then(function () {
                            vm.getSuppliers();
                            toastr.success(supplier.name + ' has been deleted');
                        })
                        .catch(function (response) {
                            toastr.error(response.data.error);
                        });
                }
            });
        };

        vm.getSuppliers();
    }
})();
