/*global angular*/
(function () {
    angular
        .module('simplAdmin.catalog')
        .factory('supplierService', supplierService);

    /* @ngInject */
    function supplierService($http) {
        var service = {
            getSupplier: getSupplier,
            createSupplier: createSupplier,
            editSupplier: editSupplier,
            deleteSupplier: deleteSupplier,
            getSuppliers: getSuppliers
        };
        return service;

        function getSupplier(id) {
            return $http.get('api/suppliers/' + id);
        }

        function getSuppliers() {
            return $http.get('api/suppliers');
        }

        function createSupplier(supplier) {
            return $http.post('api/suppliers', supplier);
        }

        function editSupplier(supplier) {
            return $http.put('api/suppliers/' + supplier.id, supplier);
        }

        function deleteSupplier(id) {
            return $http.delete('api/suppliers/' + id, null);
        }
    }
})();
