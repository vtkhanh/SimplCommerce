﻿/*global angular*/
(function () {
    'use strict';

    angular
        .module('simplAdmin.orders', [])
        .config(['$stateProvider',
            function ($stateProvider) {
                $stateProvider
                    .state('order', {
                        url: '/order',
                        templateUrl: 'modules/orders/admin/order/order-list.html',
                        controller: 'OrderListCtrl as vm'
                    })
                    .state('order-create', {
                        url: '/order/create',
                        templateUrl: 'modules/orders/admin/order/order-form.html',
                        controller: 'OrderFormCtrl as vm'
                    })
                    .state('order-edit', {
                        url: '/order/edit/:id',
                        templateUrl: 'modules/orders/admin/order/order-form.html',
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