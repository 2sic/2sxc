(function () {

    angular.module("SourceEditor", [
            "EavConfiguration",
            "EavServices",
            "SxcServices",
            "SxcTemplates",
            "pascalprecht.translate",
            "ui.ace"
        ])
        .config(function($translatePartialLoaderProvider) {
            // ensure the language pack is loaded
            $translatePartialLoaderProvider.addPart("source-editor-snippets");
        });

} ());