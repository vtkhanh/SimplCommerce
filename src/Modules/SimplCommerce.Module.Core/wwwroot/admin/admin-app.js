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
        "$locationProvider",
        "cfpLoadingBarProvider",
        "$mdDateLocaleProvider",
        function (
            $urlRouterProvider,
            $locationProvider,
            cfpLoadingBarProvider,
            $mdDateLocaleProvider,
        ) {
            // Remove prefix '!' (Default)
            $locationProvider.hashPrefix("");

            // Default route
            $urlRouterProvider.otherwise("/dashboard");

            // Turn of Spinner of Loading bar
            cfpLoadingBarProvider.includeSpinner = false;

            // Format for md-datepicker
            $mdDateLocaleProvider.formatDate = (date) => date ? moment(date).format('ll') : '';
        }
    ]);

    adminApp.run(['editableOptions', function (editableOptions) {
        editableOptions.theme = 'bs3'; // bootstrap3 theme. Can be also 'bs2', 'default'
    }]);
})();
