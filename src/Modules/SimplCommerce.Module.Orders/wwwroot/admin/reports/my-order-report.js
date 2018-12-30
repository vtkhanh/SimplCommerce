(function () {
    angular
        .module('simplAdmin.orders')
        .directive('myOrderReport', orderReportDirective);

    function orderReportDirective() {
        const directive = {
            restrict: 'E',
            templateUrl: 'template/orders/order-report',
            scope: {},
            controller: OrderReportCtrl,
            controllerAs: 'vm',
            bindToController: true
        };

        return directive;
    }

    /* @ngInject */
    function OrderReportCtrl(orderService, translateService) {
        const vm = this;
        vm.translate = translateService;
        //vm.orders = [];

        vm.$onInit = function () {
            orderService.getRevenueReport((result) => {
                const data = result.data;
                vm.chartConfig = {
                    chart: {
                        type: 'column'
                    },
                    title: {
                        text: data.title                    },
                    xAxis: {
                        categories: data.months
                    },
                    credits: {
                        enabled: false
                    },
                    series: [{
                        name: 'John',
                        data: [5, 3, 4, 7, 2]
                    }, {
                        name: 'Jane',
                        data: [2, -2, -3, 2, 1]
                    }, {
                        name: 'Joe',
                        data: [3, 4, 4, -2, 5]
                    }]
                };

            });
        };
    }
})();
