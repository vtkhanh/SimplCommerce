(function () {
    "use strict";

    angular
        .module("simplAdmin.common")
        .directive("myValidation", [
            function () {
                return {
                    restrict: "E",
                    scope: {
                        errors: "=" // using the same name as attribute
                    },
                    templateUrl: 'modules/core/admin/common/my-validation.html',
                };
            }
        ]);
})();