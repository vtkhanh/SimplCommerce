 /*global angular*/
(function () {
    angular
        .module('simplAdmin.localization')
        .controller('LocalizationFormCtrl', LocalizationFormCtrl);

    /* @ngInject */
    function LocalizationFormCtrl($state, localizationService, translateService) {
        var vm = this;
        vm.translate = translateService;
        vm.resources = [];
        vm.cultures = [];
        vm.selectedCultureId = 1;
        vm.translation = {};

        vm.changeCulture = () => {
            vm.validationErrors = [];
            localizationService.getResources(vm.selectedCultureId).then(function (result) {
                vm.resources = result.data;
            });
        }

        vm.save = () => {
            vm.validationErrors = [];
            localizationService.updateResources(vm.selectedCultureId, vm.resources)
                .then(function (result) {
                    toastr.success('Translations have been saved');
                })
                .catch(function (response) {
                    var error = response.data;
                    vm.validationErrors = [];
                    if (error && angular.isObject(error)) {
                        for (var key in error) {
                            vm.validationErrors.push(error[key][0]);
                        }
                    } else {
                        vm.validationErrors.push('Could not save translation.');
                    }
                });
        };

        vm.addTranslation = () => {
            vm.translation.cultureId = vm.selectedCultureId;

            localizationService.createResource(vm.translation)
                .then(() => {
                    getResouces();
                    toastr.success('Translations have been added.');
                })
                .catch(function (response) {
                    var error = response.data;
                    vm.validationErrors = [];
                    if (error && angular.isObject(error)) {
                        for (var key in error) {
                            vm.validationErrors.push(error[key][0]);
                        }
                    } else {
                        vm.validationErrors.push('Could not add translation.');
                    }
                });
        }

        function init() {
            localizationService.getCultures().then(function (result) {
                vm.cultures = result.data;
            });

            getResouces();
        }

        function getResouces() {
            localizationService.getResources(vm.selectedCultureId).then(function (result) {
                vm.resources = result.data;
            });
        }

        init();
    }
})();
