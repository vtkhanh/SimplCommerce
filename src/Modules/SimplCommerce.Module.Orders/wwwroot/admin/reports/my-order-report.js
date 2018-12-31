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
    function OrderReportCtrl(orderReportService, translateService) {
        const vm = this;
        vm.translate = translateService;
        vm.createdById = null;

        vm.$onInit = function () {
            orderReportService.getSellers()
                .then((result) => vm.sellerList = result.data);

            vm.getReport(vm.createdById);
        };

        vm.getReport = function (createdById) {
            orderReportService.getRevenueReport(createdById)
                .then((result) => {
                    const data = result.data;
                    vm.chartConfig = {
                        chart: {
                            type: 'column'
                        },
                        title: {
                            text: data.title
                        },
                        plotOptions: {
                            column: {
                                dataLabels: {
                                    enabled: true
                                }
                            }
                        },
                        xAxis: {
                            categories: data.months
                        },
                        credits: {
                            enabled: false
                        },
                        series: data.series
                    };

                    Highcharts.chart('order-report-chart', vm.chartConfig);
                });
        }
    }
})();
