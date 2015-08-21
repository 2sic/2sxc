
(function() {
	'use strict';

	/* This app registers all field templates for EAV in the angularjs eavFieldTemplates app */

	angular.module('sxcFieldTemplates', ['formly', 'formlyBootstrap', 'ui.bootstrap', 'ngCkeditor'], function (formlyConfigProvider) {

		formlyConfigProvider.setType({
			name: 'string-wysiwyg',
			template: '<textarea ckeditor ng-model="model[options.key]" class="form-control"></textarea>',
			wrapper: ['bootstrapLabel', 'bootstrapHasError']
		});

		formlyConfigProvider.setType({
			name: 'hyperlink-default',
			template: '<span>Hyperlink template not implemented yet</span>',
			wrapper: ['bootstrapLabel', 'bootstrapHasError']
		});

	});
})();