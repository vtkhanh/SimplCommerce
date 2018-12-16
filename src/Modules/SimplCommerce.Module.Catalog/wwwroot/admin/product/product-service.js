﻿/*global angular*/
(function () {
    angular
        .module('simplAdmin.catalog')
        .factory('productService', productService);

    /* @ngInject */
    function productService($http, Upload) {
        const service = {
            getProducts: getProducts,
            createProduct: createProduct,
            editProduct: editProduct,
            getProductSetting: getProductSetting,
            getProductAttrs: getProductAttrs,
            getProductTemplates: getProductTemplates,
            getProductTemplate: getProductTemplate,
            getProductOptions: getProductOptions,
            getProduct: getProduct,
            changeStatus: changeStatus,
            deleteProduct: deleteProduct,
            getTaxClasses: getTaxClasses,
            searchProducts: searchProducts,
            addStock: addStock
        };
        return service;

        function searchProducts(query) {
            return $http.get(`api/products/search?query=${query}`);
        }

        function getProduct(id) {
            return $http.get('api/products/' + id);
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

        function getProducts(params) {
            return $http.post('api/products/list', params);
        }

        function addStock(barcode) {
            return $http.post('api/products/addStock/' + barcode, null);
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
    }
})();
