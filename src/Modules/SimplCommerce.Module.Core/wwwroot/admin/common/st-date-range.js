/*global angular*/
(function () {
    'use strict';

    angular
        .module('simplAdmin.common')
        .directive('stDateRange', ['$timeout', function ($timeout) {
            return {
                restrict: 'E',
                require: '^stTable',
                scope: {
                    before: '=',
                    after: '=',
                    mode: '@'
                },
                templateUrl: 'modules/core/admin/common/st-date-range-md.html',
                link: function (scope, element, attrs, ctrl) {
                    var predicateName = attrs.predicate;
                    
                    scope.changeDate = function() {
                        var query = {};
                        if (scope.before) {
                            query.before = scope.before;
                            query.before = new Date(query.before.setDate(query.before.getDate() + 1));
                        }

                        if (scope.after) {
                            query.after = scope.after;
                        }

                        ctrl.search(query, predicateName);
                    };

                    function open(before) {
                        return function ($event) {
                            $event.preventDefault();
                            $event.stopPropagation();

                            if (before) {
                                scope.isBeforeOpen = true;
                            } else {
                                scope.isAfterOpen = true;
                            }
                        };
                    }

                    scope.openBefore = open(true);
                    scope.openAfter = open();

                    scope.hasBefore = attrs.before !== undefined;
                    scope.mode = scope.mode || 'day';
                }
            };
        }]);
})();
