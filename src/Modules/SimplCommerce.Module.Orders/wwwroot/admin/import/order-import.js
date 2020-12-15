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
                .then(() => {
                    toastr.success("Uploaded successfully");
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

        vm.viewResult = (importResultId) => {
            orderImportService
                .getImportResult(importResultId)
                .then(result => {
                    vm.importResult = result.data;
                    vm.importResult.time = moment(vm.importResult.importedAt).format('lll');
                });
        };

        vm.runImport = (orderFileId) => {
            orderImportService
                .runImport(orderFileId)
                .then(() => {
                    toastr.success("Re-Importing...");
                    $state.reload();
                });
        }

        vm.deleteImport = (orderFileId) => {
            if (confirm("Are you sure?")) {
                orderImportService
                    .deleteImport(orderFileId)
                    .then(result => {
                        if (result.data === true) {
                            toastr.success("Deleted successfully...");
                        }
                        else {
                            toastr.error("Deleted unsuccessfully...");
                        }
                        $state.reload();
                    });
            }
        }
    }
})();
