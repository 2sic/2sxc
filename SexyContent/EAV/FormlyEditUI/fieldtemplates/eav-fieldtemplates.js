
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
			template: '<select class="form-control" ng-model="value.Value"></select>',
			wrapper: ['bootstrapLabel', 'bootstrapHasError', 'eavLocalization'],
			defaultOptions: function defaultOptions(options) {

				// DropDown field: Convert string configuration for dropdown values to object, which will be bound to the select
				if (!options.templateOptions.options && options.templateOptions.settings.String.DropdownValues) {
					var o = options.templateOptions.settings.String.DropdownValues;
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
			template: '<textarea class="form-control" ng-model="value.Value"></textarea>',
			wrapper: ['bootstrapLabel', 'bootstrapHasError', 'eavLocalization'],
			defaultOptions: {
				ngModelAttrs: {
					'{{to.settings.String.RowCount}}': { value: 'rows' },
					cols: { attribute: 'cols' }
				}
			}
		});

		formlyConfigProvider.setType({
			name: 'number-default',
			template: '<input type="number" class="form-control" ng-model="value.Value">{{vm.isGoogleMap}}',
			wrapper: ['bootstrapLabel', 'bootstrapHasError', 'eavLocalization'],
			defaultOptions: {
				ngModelAttrs: {
					'{{to.settings.Number.Min}}': { value: 'min' },
					'{{to.settings.Number.Max}}': { value: 'max' },
					'{{to.settings.Number.Decimals ? "^[0-9]+(\.[0-9]{1," + to.settings.Number.Decimals + "})?$" : null}}': { value: 'pattern' }
				}
			},
			controller: 'FieldTemplate-NumberCtrl as vm'
		});

		formlyConfigProvider.setType({
			name: 'boolean-default',
			template: "<div class=\"checkbox\">\n\t<label>\n\t\t<input type=\"checkbox\"\n           class=\"formly-field-checkbox\"\n\t\t       ng-model=\"value.Value\">\n\t\t{{to.label}}\n\t\t{{to.required ? '*' : ''}}\n\t</label>\n</div>\n",
			wrapper: ['bootstrapLabel', 'bootstrapHasError', 'eavLocalization']
		});

		formlyConfigProvider.setType({
			name: 'datetime-default',
			wrapper: ['bootstrapLabel', 'bootstrapHasError', 'eavLocalization'],
			template: '<div><div class="input-group"><div class="input-group-addon" style="cursor:pointer;" ng-click="to.isOpen = true;"><i class="glyphicon glyphicon-calendar"></i></div><input class="form-control" ng-model="value.Value" is-open="to.isOpen" datepicker-options="to.datepickerOptions" datepicker-popup /></div>' +
				 '<timepicker ng-show="to.settings.DateTime.UseTimePicker" ng-model="value.Value" show-meridian="ismeridian"></timepicker></div>',
			defaultOptions: {
				templateOptions: {
					datepickerOptions: {},
					datepickerPopup: 'dd.MM.yyyy'
				}
			}
		});
	});

	eavFieldTemplates.controller('FieldTemplate-NumberCtrl', function () {
		var vm = this;
		vm.isGoogleMap = "test";
	});

})();