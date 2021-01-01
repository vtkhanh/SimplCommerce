/*global angular, confirm*/
(function () {
    angular
        .module('simplAdmin.catalog')
        .controller('SupplierFormCtrl', SupplierFormCtrl);

    /* @ngInject */
    function SupplierFormCtrl($state, $stateParams, translateService, supplierService) {
        var vm = this;
        vm.translate = translateService;
        vm.supplier = {};
        vm.supplierId = $stateParams.id;
        vm.isEditMode = vm.supplierId > 0;

        vm.save = function save() {
            var promise;
            if (vm.isEditMode) {
                promise = supplierService.editSupplier(vm.supplier);
            } else {
                promise = supplierService.createSupplier(vm.supplier);
            }

            promise
                .then(function () {
                    $state.go('supplier');
                    if (vm.isEditMode) {
                        toastr.success(vm.supplier.name + ' has been updated');
                    } else {
                        toastr.success(vm.supplier.name + ' has been created');
                    }
                })
                .catch(function (response) {
                    var error = response.data;
                    vm.validationErrors = [];
                    if (error && angular.isObject(error)) {
                        for (var key in error) {
                            vm.validationErrors.push(error[key][0]);
                        }
                    } else {
                        vm.validationErrors.push('Could not add supplier.');
                    }
                });
        };

        function init() {
            if (vm.isEditMode) {
                supplierService.getSupplier(vm.supplierId).then(function (result) {
                    vm.supplier = result.data;
                });
            }
        }

        init();
    }
})();
