/*global angular*/
(function () {
    'use strict';

    angular
        .module('simplAdmin.orders', [])
        .config(['$stateProvider',
            function ($stateProvider) {
                $stateProvider
                    .state('order', {
                        url: '/order',
                        templateUrl: 'template/orders/order-list',
                        controller: 'OrderListCtrl as vm'
                    })
                    .state('order-create', {
                        url: '/order/create',
                        templateUrl: 'template/orders/order-create',
                        controller: 'OrderFormCtrl as vm'
                    })
                    .state('order-edit', {
                        url: '/order/edit/:id',
                        templateUrl: function (params) {
                            return `template/orders/order-edit/${params.id}`;
                        },
                        controller: 'OrderFormCtrl as vm'
                    })
                    .state('order-detail', {
                        url: '/order/detail/:id',
                        templateUrl: 'modules/orders/admin/order/order-detail.html',
                        controller: 'OrderDetailCtrl as vm'
                    })
                    .state('order-import', {
                        url: '/order/import',
                        templateUrl: 'template/orders/order-import',
                        controller: 'OrderImportCtrl as vm'
                    })
                ;
            }
        ]);
})();
