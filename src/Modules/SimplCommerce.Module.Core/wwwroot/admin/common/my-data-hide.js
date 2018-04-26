(function () {
    'use strict';

    angular
        .module('simplAdmin.common')
        .directive('myDataHide', [function() {
            return {
                restrict: 'A',
                scope: {
                    myDataHide: '=' // using the same name as attribute
                },
                link: function (scope, element, attrs) {
                    element.bind('click', () => {
                        scope.myDataHide = null;
                        scope.$apply();
                    });
                }
            };
        }]);

})();