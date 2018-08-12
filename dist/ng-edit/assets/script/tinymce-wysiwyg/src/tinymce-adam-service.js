export function attachAdam(vm) {
    vm.adam = vm.host.attachAdam();

    vm.adamSetValue = function (fileItem, modeImage) {
        if (modeImage === undefined) // if not supplied, use the setting in the adam
            modeImage = vm.adam.adamModeImage;
        var fileName = fileItem.Name.substr(0, fileItem.Name.lastIndexOf('.'));
        console.log('setAdamValue fileName', fileName);

        var content = modeImage
            ? '<img src="' + fileItem.FullPath + '" + alt="' + fileName + '">'
            : '<a href="' + fileItem.FullPath + '">' + fileName + '</a>';
        console.log('setAdamValue content:', content);
        //var body = vm.editor.getBody();
        //vm.editor.selection.setCursorLocation(body, 0);
        //debugger;
        //var range = window.savedRange;
        //vm.editor.selection.setCursorLocati
        tinymce.get(vm.id).insertContent(content);
    };

    vm.adamAfterUpload = function (fileItem) {
        console.log('adamAfterUpload');
        vm.adamSetValue(fileItem, fileItem.Type === 'image');
    }

    vm.toggleAdam = function (imagesOnly, usePortalRoot) {
        console.log('tinymce toggleAdam');
        vm.adam.adamModeImage = imagesOnly;
        vm.adam.toggleAdam({
            showImagesOnly: imagesOnly,
            usePortalRoot: usePortalRoot
        })
    };
}


