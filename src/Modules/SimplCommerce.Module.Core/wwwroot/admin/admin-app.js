/*global angular*/
(function () {
    var adminApp = angular.module("simplAdmin", [
        "ui.router",
        "ngAnimate",
        "ngMaterial",
        "ngMessages",
        "smart-table",
        "angular-loading-bar",
        "ngFileUpload",
        "ui.bootstrap",
        "ui.bootstrap.datetimepicker",
        "ui.tree",
        "summernote",
        "xeditable",
        "colorpicker.module",
        "simplAdmin.common",
        "simplAdmin.dashboard",
        "simplAdmin.core",
        "simplAdmin.catalog",
        "simplAdmin.orders",
        "simplAdmin.cms",
        "simplAdmin.search",
        "simplAdmin.reviews",
        "simplAdmin.activityLog",
        "simplAdmin.vendors",
        "simplAdmin.localization",
        "simplAdmin.news",
        "simplAdmin.contacts",
        "simplAdmin.pricing",
        "simplAdmin.tax",
        "simplAdmin.shippings",
        "simplAdmin.shipping-tablerate",
        "simplAdmin.payments",
        "simplAdmin.paymentStripe",
        "simplAdmin.paymentPaypalExpress"
    ]);

    toastr.options.closeButton = true;
    adminApp.config([
        "$urlRouterProvider",
        "$httpProvider",
        "$locationProvider",
        "cfpLoadingBarProvider",
        "$mdDateLocaleProvider",
        "stConfig",
        function (
            $urlRouterProvider,
            $httpProvider,
            $locationProvider,
            cfpLoadingBarProvider,
            $mdDateLocaleProvider,
            stConfig,
        ) {
            // Remove prefix '!' (Default)
            $locationProvider.hashPrefix("");

            // Default route
            $urlRouterProvider.otherwise("/dashboard");

            // Turn of Spinner of Loading bar
            cfpLoadingBarProvider.includeSpinner = false;

            // Format for md-datepicker
            $mdDateLocaleProvider.formatDate = (date) => date ? moment(date).format('ll') : '';

            // smart-table config: custom pagination to override the default one
            stConfig.pagination.template = 'modules/core/admin/common/pagination.custom.html';

            $httpProvider.interceptors.push(function () {
                return {
                    request: function (config) {
                        if (/modules.*admin.*\.html/i.test(config.url)) {
                            var separator = config.url.indexOf("?") === -1 ? "?" : "&";
                            config.url = `${config.url}${separator}v=${window.Global_AssetVersion}`;
                        }

                        return config;
                    }
                };
            });
        }
    ]);

    adminApp.run(['editableOptions', function (editableOptions) {
        editableOptions.theme = 'bs3'; // bootstrap3 theme. Can be also 'bs2', 'default'
    }]);
})();
