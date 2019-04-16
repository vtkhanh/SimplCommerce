/*global angular confirm*/
(function () {
    angular
        .module('simplAdmin.catalog')
        .controller('ProductListCtrl', ProductListCtrl);

    /* @ngInject */
    function ProductListCtrl($state, $location, productService, translateService) {
        const vm = this;
        let firstLoad = true;
        let tableStateRef;

        vm.translate = translateService;
        vm.products = [];

        vm.getProducts = (tableState) => {
            setPageIndex(tableState);

            tableStateRef = tableState;
            vm.isLoading = true;
            productService
                .getProducts(tableState)
                .then((result) => {
                    vm.products = result.data.items;
                    tableState.pagination.numberOfPages = result.data.numberOfPages;
                    vm.isLoading = false;
                });

            firstLoad = false;
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

        vm.changeStock = (productId, stock) => {
            productService
                .changeStock(productId, stock)
                .then(() => toastr.success("Saved successfully."))
                .catch((response) => toastr.error(JSON.stringify(response.data)));
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
                                tableStateRef.search.predicateObject = {};
                            }
                            tableStateRef.pagination.start = 0;
                            tableStateRef.search.predicateObject.Sku = vm.barcode;
                            vm.getProducts(tableStateRef);
                        } else {
                            $state.go('product-create', { sku: vm.barcode });
                        }

                        vm.barcode = null;
                    })
                    .catch((response) => toastr.error(JSON.stringify(response.data)));
            }
        };

        vm.exportProducts = () => productService.exportProducts(tableStateRef);

        function setPageIndex(tableState) {
            let pageIndex = tableState.pagination.start / tableState.pagination.number + 1;
            if (firstLoad) { // use Query String if this is the first load
                pageIndex = $location.search()['page'] || 1;
                tableState.pagination.start = (pageIndex - 1) * tableState.pagination.number;
            }
            $location.search({ 'page': pageIndex });
        }
    }
})();
