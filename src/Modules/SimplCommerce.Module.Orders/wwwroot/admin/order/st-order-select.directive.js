(function () {
    angular
        .module('simplAdmin.orders')
        .directive('stOrderSelect', stOrderSelect);

    function stOrderSelect() {
        var directive = {
            require: '^stTable',
            template: '<input type="checkbox" ng-if="row.canEdit"/>',
            scope: {
                row: '=stOrderSelect'
            },
            link: function (scope, element, attr, ctrl) {

                element.bind('change', function (evt) {
                    scope.$apply(function () {
                        ctrl.select(scope.row, 'multiple');
                    });
                });

                scope.$watch('row.isSelected', function (newValue, oldValue) {
                    if (newValue === true) {
                        element.parent().removeClass(scope.row.cssClass);
                        element.parent().addClass('st-selected');
                    } else {
                        element.parent().addClass(scope.row.cssClass);
                        element.parent().removeClass('st-selected');
                    }
                });
            }
        };

        return directive;
    }
})();
