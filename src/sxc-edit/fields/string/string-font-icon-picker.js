/* 
 * Field: String - font-icon picker
 */

angular.module("sxcFieldTemplates")
    .config(function(formlyConfigProvider, defaultFieldWrappers) {

        formlyConfigProvider.setType({
            name: "string-font-icon-picker",
            templateUrl: "fields/string/string-font-icon-picker.html",
            wrapper: defaultFieldWrappers,
            controller: "FieldTemplate-String-Font-Icon-Picker as vm"
        });

    })
    .controller("FieldTemplate-String-Font-Icon-Picker", function ($scope, debugState, $ocLazyLoad, appRoot) {
        var vm = angular.extend(this, {
            iconFilter: "", // used for in-line search
            prefix: "", // used to find the right css-classes
            previewPrefix: "", // used to preview the icon, in addition to the built-in class
            icons: [], // list of icons, to be filled
            useTestValues: false, // to prefill with test-values, in case needed
            selectorIsOpen: false
    });


        //#region icon css-class-methods
        function getIconClasses(className) {
            var charcount = className.length, foundList = [], duplicateDetector = {};
            if (!className) return foundList;
            for (var ssSet = 0; ssSet < document.styleSheets.length; ssSet++) {
                var classes = document.styleSheets[ssSet].rules || document.styleSheets[ssSet].cssRules;
                if(classes)
                    for (var x = 0; x < classes.length; x++)
                        if (classes[x].selectorText && classes[x].selectorText.substring(0, charcount) === className) {
                            // prevent duplicate-add...
                            var txt = classes[x].selectorText,
                                icnClass = txt.substring(0, txt.indexOf(":")).replace(".", "");
                            if (!duplicateDetector[icnClass]) {
                                foundList.push({ rule: classes[x], 'class': icnClass });
                                duplicateDetector[icnClass] = true;
                            }
                        }
            }
            return foundList;
        }

//#endregion

        //#region load additional resources
        function loadAdditionalResources(files) {
            files = files || "";
            var mapped = files.replace("[App:Path]", appRoot)
                .replace(/([\w])\/\/([\w])/g,   // match any double // but not if part of https or just "//" at the beginning
                "$1/$2");
            var fileList = mapped ? mapped.split("\n") : [];
            return $ocLazyLoad.load(fileList);
        }
        //#endregion

        vm.setIcon = function(newValue) {
            $scope.value.Value = newValue;
            vm.selectorIsOpen = false;
            //$scope.status.isopen = false;
            $scope.form.$setDirty();
        };

        vm.activate = function() {
            // get configured
            var controlSettings = $scope.to.settings["string-font-icon-picker"];
            vm.files = (controlSettings) ? controlSettings.Files || "" : "";
            vm.prefix = (controlSettings) ? controlSettings.CssPrefix || "" : "";
            vm.previewPrefix = (controlSettings) ? controlSettings.PreviewCss || "" : "";

            if (vm.useTestValues)
                angular.extend(vm, {
                    iconFilter: "",
                    prefix: ".glyphicon-",
                    previewPrefix: "glyphicon",
                });

            // load all additional css, THEN load the icons
            loadAdditionalResources(vm.files).then(function() {
                // load the icons
                vm.icons = getIconClasses(vm.prefix);

            });

            vm.debug = debugState;
            if (debugState.on) console.log($scope.options.templateOptions);
        };

        vm.activate();
    });