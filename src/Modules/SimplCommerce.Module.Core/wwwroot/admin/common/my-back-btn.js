(function () {
    'use strict';

    angular
        .module('simplAdmin.common')
        .directive('myBackBtn', [function() {
            return {
                restrict: 'E',
                template: `
                    <button type="button" class="btn btn-default" onclick="window.history.back();">
                        <i class="fa fa-angle-double-left">&nbsp;</i>Back
                    </button>
                `
            };
        }]);

})();