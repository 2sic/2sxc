
(function() {
	'use strict';

	/* This app registers all field templates for EAV in the angularjs eavFieldTemplates app */

	var eavFieldTemplates = angular.module('eavFieldTemplates', ['formly', 'formlyBootstrap', 'ui.bootstrap', 'eavLocalization'], function (formlyConfigProvider) {

		formlyConfigProvider.setType({
			name: 'string-default',
			template: '<input class="form-control" ng-model="value.Value">',
			wrapper: ['bootstrapLabel', 'bootstrapHasError', 'eavLocalization']
		});

		formlyConfigProvider.setType({
			name: 'string-dropdown',
			template: '<select class="form-control" ng-model="model[options.key]"></select>',
			wrapper: ['bootstrapLabel', 'bootstrapHasError'],
			defaultOptions: function defaultOptions(options) {

				// DropDown field: Convert string configuration for dropdown values to object, which will be bound to the select
				if (!options.templateOptions.options && options.templateOptions.settings.DropdownValues) {
					var o = options.templateOptions.settings.DropdownValues;
					o = o.replace('\r', '').split('\n');
					o = o.map(function (e, i) {
						var s = e.split(':');
						return {
							name: s[0],
							value: s[1] ? s[1] : s[0]
						};
					});
					options.templateOptions.options = o;
				}

				function _defineProperty(obj, key, value) { return Object.defineProperty(obj, key, { value: value, enumerable: true, configurable: true, writable: true }); }

				var ngOptions = options.templateOptions.ngOptions || 'option[to.valueProp || \'value\'] as option[to.labelProp || \'name\'] group by option[to.groupProp || \'group\'] for option in to.options';
				return {
					ngModelAttrs: _defineProperty({}, ngOptions, {
						value: 'ng-options'
					})
				};
			}
		});

		formlyConfigProvider.setType({
			name: 'string-textarea',
			template: '<textarea class="form-control" ng-model="model[options.key]"></textarea>',
			wrapper: ['bootstrapLabel', 'bootstrapHasError'],
			defaultOptions: {
				ngModelAttrs: {
					'{{to.settings.RowCount}}': { value: 'rows' },
					cols: { attribute: 'cols' }
				}
			}
		});

		formlyConfigProvider.setType({
			name: 'number-default',
			template: '<input type="number" class="form-control" ng-model="model[options.key]">',
			wrapper: ['bootstrapLabel', 'bootstrapHasError'],
			defaultOptions: {
				ngModelAttrs: {
					'{{to.settings.Min}}': { value: 'min' },
					'{{to.settings.Max}}': { value: 'max' },
					'{{to.settings.Decimals ? "^[0-9]+(\.[0-9]{1," + to.settings.Decimals + "})?$" : null}}': { value: 'pattern' }
				}
			}
		});

		formlyConfigProvider.setType({
			name: 'boolean-default',
			template: "<div class=\"checkbox\">\n\t<label>\n\t\t<input type=\"checkbox\"\n           class=\"formly-field-checkbox\"\n\t\t       ng-model=\"model[options.key]\">\n\t\t{{to.label}}\n\t\t{{to.required ? '*' : ''}}\n\t</label>\n</div>\n",
			wrapper: ['bootstrapHasError']
		});

		formlyConfigProvider.setType({
			name: 'datetime-default',
			wrapper: ['bootstrapLabel', 'bootstrapHasError'],
			template: '<div><div class="input-group"><div class="input-group-addon" style="cursor:pointer;" ng-click="to.isOpen = true;"><i class="glyphicon glyphicon-calendar"></i></div><input class="form-control" ng-model="model[options.key]" is-open="to.isOpen" datepicker-options="to.datepickerOptions" datepicker-popup /></div>' +
				 '<timepicker ng-show="to.settings.UseTimePicker" ng-model="model[options.key]" show-meridian="ismeridian"></timepicker></div>',
			defaultOptions: {
				templateOptions: {
					datepickerOptions: {},
					datepickerPopup: 'dd.MM.yyyy'
				}
			}
		});
	});

	eavFieldTemplates.controller('test', function($scope) {
		$scope.value = "abcdefgh";
	});

})();