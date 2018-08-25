/*global angular confirm*/
(function () {
    angular
        .module('simplAdmin.catalog')
        .controller('ProductListCtrl', ProductListCtrl);

    /* @ngInject */
    function ProductListCtrl($state, productService, translateService) {
        const vm = this;
        let tableStateRef;

        vm.translate = translateService;
        vm.products = [];

        vm.getProducts = (tableState) => {
            tableStateRef = tableState;
            vm.isLoading = true;
            productService
                .getProducts(tableState)
                .then((result) => {
                    vm.products = result.data.items;
                    tableState.pagination.numberOfPages = result.data.numberOfPages;
                    vm.isLoading = false;
                });
        };

        vm.changeStatus = (product) => {
            productService
                .changeStatus(product)
                .then(() => {
                    product.isPublished = !product.isPublished;
                });
        };

        vm.deleteProduct = (product) => {
            bootbox.confirm('Are you sure you want to delete this product: ' + product.name, function (result) {
                if (result) {
                    productService
                        .deleteProduct(product)
                        .then(() => {
                            vm.getProducts(tableStateRef);
                            toastr.success(product.name + ' has been deleted');
                        })
                        .catch((response) => {
                            toastr.error(response.data.error);
                        });
                }
            });
        };

        vm.addStock = () => {
            if (vm.barcode) {
                productService
                    .addStock(vm.barcode)
                    .then((response) => {
                        const data = response.data;

                        if (data.value === true) {
                            toastr.success("Added successfully");

                            // Refresh the product list
                            if (!tableStateRef.search.predicateObject) {
                                tableStateRef.search.predicateObject = {}; // Initialize predicateObject
                            }
                            tableStateRef.search.predicateObject.Sku = vm.barcode;
                            vm.getProducts(tableStateRef);
                        } else {
                            $state.go('product-create', { sku: vm.barcode });
                        }

                        vm.barcode = null;
                    })
                    .catch((response) => toastr.error(response.data));
            }
        };
    }
})();