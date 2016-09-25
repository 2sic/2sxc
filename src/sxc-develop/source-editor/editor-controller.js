(function () {

    angular.module("SourceEditor")
        .controller("Editor", EditorController)
    ;

    function EditorController(sourceSvc, snippetSvc, appAssetsSvc, appId, sxcDialogs, item, $uibModalInstance, $window, $scope, $translate, saveToastr, ctrlS, debugState) {
        $translate.refresh();   // necessary to load stuff added in this lazy-loaded app

        var vm = this;
        vm.debug = debugState;

        // if item is an object with EntityId, it referrs to a template, otherwise it's a relative path

        var svc = sourceSvc(item.EntityId !== undefined ? item.EntityId : item.Path);

        vm.view = {};
        vm.tempCodeBecauseOfBug = "";
        vm.editor = null;

        svc.get().then(function (result) {
            vm.view = result.data;
            svc.initSnippets(vm.view);
        });

        // load appropriate snippets from the snippet service
        svc.initSnippets = function (template) {
            vm.snipSvc = snippetSvc(template, ace);
            vm.snipSvc.getSnippets().then(function (result) {
                vm.snippets = result;
                vm.snippetSet = "Content";    // select default
                vm.snippetHelp = vm.snipSvc.help;
                vm.snippetLabel = vm.snipSvc.label;

                // now register the snippets in the editor
                vm.registerSnippets();
            });
        };

        //#region close / prevent-close
        vm.close = function () {
            if (!confirm($translate.instant("Message.ExitOk")))
                return;
            window.close();
        };
        
        // prevent all kind of closing when accidentally just clicking on the side of the dialog
        $scope.$on("modal.closing", function (e) { e.preventDefault(); });

        $window.addEventListener("beforeunload", function (e) {
            var unsavedChangesText = $translate.instant("Message.ExitOk");
            (e || window.event).returnValue = unsavedChangesText; //Gecko + IE
            return unsavedChangesText; //Gecko + Webkit, Safari, Chrome etc.
        });

        //#endregion

        //#region save
        vm.save = function (autoClose) {
            var after = autoClose ? vm.close : function () { };

            //#region bugfix 607
            // check if there is still some temp-snippet which we must update first 
            // - because of issue https://github.com/2sic/2sxc/issues/607
            // it's very important that we place the text into a copy of the variable
            // and NOT in the view.Code, otherwise undo will stop working
            var latestCode = vm.editor.getValue();
            var savePackage = angular.copy(vm.view);
            if (savePackage.Code !== latestCode) //{
                savePackage.Code = latestCode;
            //#endregion

            // now save with appropriate toaster
            saveToastr(svc.save(savePackage)).then(after);
        };
        //#endregion

        activate();

        function activate() {
            // add ctrl+s to save
            ctrlS(function () { vm.save(false); });


        }

        //#region show file picker
        vm.browser = {
            show: false,
            svc: appAssetsSvc(appId),
            toggle: function() {
      
                vm.browser.show = !vm.browser.show;
                if (!vm.assets)
                    vm.assets = vm.browser.svc.liveList();
            },
            editFile: function(filename) {
                window.open(vm.browser.assembleUrl(filename));
                vm.browser.toggle();
            },
            assembleUrl: function(newFileName) {
                // note that as of now, we'll just use the initial url and change the path
                // then open a new window
                var url = window.location.href;
                var newItems = JSON.stringify([{ Path: newFileName }]);
                return url.replace(new RegExp("items=.*?%5d", "i"), "items=" + encodeURI(newItems)); // note: sometimes it doesn't have an appid, so it's [0-9]* instead of [0-9]+
            },
            addFile: function () {
                // todo: i18n
                var result = prompt("please enter full file name"); // $translate.instant("AppManagement.Prompt.NewApp"));
                if (result)
                    vm.browser.svc.create(result);

            }
        };

        //#endregion

        //#region snippets
        vm.addSnippet = function addSnippet(snippet) {
            var snippetManager = ace.require("ace/snippets").snippetManager;
            snippetManager.insertSnippet(vm.editor, snippet);
            vm.editor.focus();
        };

        vm.registerSnippets = function registerSnippets() {
            // ensure we have everything first (this may be called multiple times), then register them
            if (!(vm.snipSvc && vm.editor))
                return;
            vm.snipSvc.registerInEditor();
        };
        //#endregion

        // this event is called when the editor is ready
        vm.aceLoaded = function (_editor) {
            vm.editor = _editor;        // remember the editor for later actions
            vm.registerSnippets();      // try to register the snippets
        };

    }

}());