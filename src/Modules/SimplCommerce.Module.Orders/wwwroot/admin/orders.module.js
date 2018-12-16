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
                        templateUrl: 'template/orders/order-form',
                        controller: 'OrderFormCtrl as vm'
                    })
                    .state('order-edit', {
                        url: '/order/edit/:id',
                        templateUrl: 'template/orders/order-form',
                        controller: 'OrderFormCtrl as vm'
                    })
                    .state('order-detail', {
                        url: '/order/detail/:id',
                        templateUrl: 'modules/orders/admin/order/order-detail.html',
                        controller: 'OrderDetailCtrl as vm'
                    })
                ;
            }
        ]);
})();
