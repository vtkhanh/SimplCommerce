/*global angular*/
(function () {
    angular
        .module('simplAdmin.catalog')
        .factory('productService', productService);

    /* @ngInject */
    function productService($http, Upload) {
        const service = {
            addStock: addStock,
            changeStatus: changeStatus,
            changeStock: changeStock,
            createProduct: createProduct,
            deleteProduct: deleteProduct,
            exportProducts: exportProducts,
            editProduct: editProduct,
            importStock: importStock,
            getProducts: getProducts,
            getProductSetting: getProductSetting,
            getProductAttrs: getProductAttrs,
            getProductTemplates: getProductTemplates,
            getProductTemplate: getProductTemplate,
            getProductOptions: getProductOptions,
            getProduct: getProduct,
            getTaxClasses: getTaxClasses,
            getStockImports: getStockImports,
            searchProducts: searchProducts
        };
        return service;

        function searchProducts(query, hasOptions = null) {
            return $http.get(`api/products/search?query=${query}&hasOptions=${hasOptions}`);
        }

        function getProduct(id) {
            return $http.get('api/products/' + id);
        }

        function getProducts(params) {
            return $http.post(`api/products/list`, params);
        }

        function getProductSetting() {
            return $http.get('api/products/setting');
        }

        function getProductAttrs() {
            return $http.get('api/product-attributes');
        }

        function getProductTemplates() {
            return $http.get('api/product-templates');
        }

        function getProductTemplate(id) {
            return $http.get('api/product-templates/' + id);
        }

        function getProductOptions() {
            return $http.get('api/product-options');
        }

        function getStockImports(productId) {
            return $http.get('api/products/stock-imports/' + productId);
        }
        
        function addStock(barcode) {
            return $http.post('api/products/addStock/' + barcode, null);
        }

        function changeStock(id, stock) {
            return $http.post('api/products/changeStock', { id, stock });
        }

        function createProduct(product, thumbnailImage, productImages, productDocuments) {
            return Upload.upload({
                url: 'api/products',
                data: {
                    product: product,
                    thumbnailImage: thumbnailImage,
                    productImages: productImages,
                    productDocuments: productDocuments
                }
            });
        }

        function editProduct(product, thumbnailImage, productImages, productDocuments) {
            return Upload.upload({
                url: 'api/products/' + product.id,
                method: 'PUT',
                data: {
                    product: product,
                    thumbnailImage: thumbnailImage,
                    productImages: productImages,
                    productDocuments: productDocuments
                }
            });
        }

        function changeStatus(product) {
            return $http.post('api/products/change-status/' + product.id, null);
        }

        function deleteProduct(product) {
            return $http.delete('api/products/' + product.id, null);
        }

        function getTaxClasses() {
            return $http.get('api/tax-classes');
        }

        function exportProducts(params) {
            return $http.post('api/products/export', params, { responseType: 'arraybuffer' }).then(downloadFile);
        }

        function importStock(stockImport) {
            return $http.post('api/products/import-stock', stockImport);
        }
    }
})();
