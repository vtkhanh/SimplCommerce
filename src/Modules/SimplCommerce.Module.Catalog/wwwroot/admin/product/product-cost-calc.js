/*global angular confirm*/
(function () {
    angular
        .module('simplAdmin.catalog')
        .directive('productCostCalc', productCostCalc);

    function productCostCalc() {
        var directive = {
            restrict: 'E',
            templateUrl: 'modules/catalog/admin/product/product-cost-calc.html',
            scope: {},
            controller: ProductCostCalCtrl,
            controllerAs: 'vm',
            bindToController: {
                cost: '=',
                onCostChange: '&',
                setting: '=',
                modalId: '@',
                title: '@'
            }
        };

        return directive;
    }

    /* @ngInject */
    function ProductCostCalCtrl() {
        const vm = this;
        vm.purchasePrice = 0;
        vm.weight = 0;
        vm.total = 0;

        vm.update = function () {
            if (vm.purchasePrice != null && vm.weight != null) {
                vm.total = vm.purchasePrice * vm.setting.conversionRate * (1 + vm.setting.feeOfPicker/100) + vm.weight * vm.setting.conversionRate * vm.setting.feePerWeightUnit;
            }
        }

        vm.save = function () {
            vm.cost = vm.total;
            vm.onCostChange({ cost: vm.total });
        }
    }
})();