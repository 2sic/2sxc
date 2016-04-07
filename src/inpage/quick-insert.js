(function () {
    'use strict';
    var strButtons = "<a class='sc-content-block-menu-addcontent' data-type='Default' data-i18n='[title]QuickInsertMenu.AddBlockContent'>content</a>"
        + "<a class='sc-content-block-menu-addapp' data-type='' data-i18n='[title]QuickInsertMenu.AddBlockApp'>app</a>"
        + "<a class='sc-content-block-menu-btn sc-cb-action icon-sxc-scissors' data-action='cut' data-i18n='[title]QuickInsertMenu.Cut'></a>"
        + "<a class='sc-content-block-menu-btn sc-cb-action icon-sxc-paste sc-invisible' data-action='paste' data-i18n='[title]QuickInsertMenu.Cut'></a>"
        + "<a class='sc-content-block-menu-btn sc-cb-action icon-sxc-trash sc-invisible' data-action='delete' data-i18n='[title]QuickInsertMenu.Cut'></a>";
    var blockActions = $(strButtons);
    var newBlockMenu = $("<div class='sc-content-block-menu sc-i18n'></div>");
    var moduleActions = $(strButtons.replace(/QuickInsertMenu.AddBlock/g, "QuickInsertMenu.AddModule")).attr('data-context', 'module').addClass('sc-content-block-menu-module');
    newBlockMenu.append(blockActions).append(moduleActions);

    $("body").append(newBlockMenu);

    var selectors = {
        listContainerSelector: '.sc-content-block-list',
        contentBlockClass: 'sc-content-block',
        contentBlockSelector: '.sc-content-block',
        moduleSelector: '.DnnModule',
        paneSelector: '.DNNEmptyPane, :has(>.DnnModule)', // Found no better way to get all panes - the hidden variable does not exist when not in edit page mode
        listDataAttr: 'data-list-context',
    };

    blockActions.click(function () {
        var type = $(this).data("type");
        var list = newBlockMenu.actionsForCb.closest(selectors.listContainerSelector);
        var actionConfig = JSON.parse(list.attr(selectors.listDataAttr));
        var index = 0;

        if (newBlockMenu.actionsForCb.hasClass(selectors.contentBlockClass))
            index = list.find(selectors.contentBlockSelector).index(newBlockMenu.actionsForCb[0]) + 1;

        // check cut/paste
        var cbAction = $(this).data("action");
        if (!cbAction)
            $2sxc(list).manage.createContentBlock(actionConfig.parent, actionConfig.field, index, type, list);
        else
        // this is a cut/paste action
        {
            if (cbAction === "cut") {
                $2sxc._cbClipboard = { index: index, guid: 'todo later' };
                setSecondaryActionsState("inline-block!important");
            }
            if (cbAction === "paste") {
                var from = $2sxc._cbClipboard.index, to = index;
                if (from === to || from + 1 === to) // this moves it to the same spot, so ignore
                    return;

                $2sxc(list).manage.moveContentBlock(actionConfig.parent, actionConfig.field, from, to);
                $2sxc._cbClipboard = null;
            }
            if (cbAction === "delete") {
                alert("todo not implemented yet");
            }
            return;
        } 
    });

    function setSecondaryActionsState(state) {
        var btns = $("a.sc-content-block-menu-btn");
        btns = btns.filter(".icon-sxc-paste");// later also : , .icon-sxc-trash"); // only on the main one...?
        if (state) 
            btns.removeClass("sc-invisible");
        else 
            btns.addClass("sc-invisible");
    }

    moduleActions.click(function () {
        var type = $(this).data("type");
        var pane = newBlockMenu.actionsForModule.closest(selectors.paneSelector);
        var paneName = pane.attr('id').replace('dnn_', '');

        var index = 0;
        if (newBlockMenu.actionsForModule.hasClass('DnnModule'))
            index = pane.find('.DnnModule').index(newBlockMenu.actionsForModule[0]) + 1;

        var service = $.dnnSF();
        var serviceUrl = service.getServiceRoot('internalservices') + 'controlbar/';

        $.ajax({
            url: serviceUrl + 'GetPortalDesktopModules',
            type: 'GET',
            data: 'category=All&loadingStartIndex=0&loadingPageSize=100&searchTerm=',
            beforeSend: service.setModuleHeaders,
            success: function (desktopModules) {
                var moduleToFind = type === 'Default' ? ' Content' : ' App';
                var module = null;
                
                desktopModules.forEach(function (e,i) {
                    if (e.ModuleName === moduleToFind)
                        module = e;
                });

                if (module === null)
                    alert(moduleToFind + ' module not found.');

                var postData = {
                    Module: module.ModuleID,
                    Page: '',
                    Pane: paneName,
                    Position: -1,
                    Sort: index,
                    Visibility: 0,
                    AddExistingModule: false,
                    CopyModule: false
                };



                $.ajax({
                    url: serviceUrl + 'AddModule',
                    type: 'POST',
                    data: postData,
                    beforeSend: service.setModuleHeaders,
                    success: function (d) {
                        window.location.reload();
                    },
                    error: function (xhr) {
                        alert('Error while adding module.');
                        console.log(xhr);
                    }
                });
            },
            error: function (xhr) {
                alert('Error while adding module.');
                console.log(xhr);
            }
        });

        

    });

    var refreshTimeout = null;
    $("body").on('mousemove', function (e) {
        
        if (refreshTimeout === null)
            refreshTimeout = window.setTimeout(function () {
                requestAnimationFrame(function () {
                    refreshMenu(e);
                    refreshTimeout = null;
                });
            }, 60);

    });

    function refreshMenu(e) { // ToDo: Performance is not solved with requestAnimationFrame, needs throttling (or more performant selectors etc.)
        var contentBlocks = $(selectors.listContainerSelector).find(selectors.contentBlockSelector)
                .add(selectors.listContainerSelector);

        var modules = $(selectors.paneSelector).find(selectors.moduleSelector)
                .add(selectors.paneSelector);

        var nearestCb = findNearest(contentBlocks, { x: e.clientX, y: e.clientY }, selectors.contentBlockSelector);
        var nearestModule = findNearest(modules, { x: e.clientX, y: e.clientY }, selectors.moduleSelector);

        moduleActions.toggle(nearestModule !== null);
        blockActions.toggle(nearestCb !== null);

        if (nearestCb !== null || nearestModule !== null) {
            var alignTo = nearestCb || nearestModule;

            newBlockMenu.css({
                'left': alignTo.x,
                'top': alignTo.y,
                'width': alignTo.element.width()
            }).show();

            // Keep current block as current on menu
            newBlockMenu.actionsForCb = nearestCb ? nearestCb.element : null;
            newBlockMenu.actionsForModule = nearestModule ? nearestModule.element : null;
        }
        else
            newBlockMenu.hide();
    }

    // Return the nearest element to the mouse cursor from elements (jQuery elements)
    function findNearest(elements, position, useBottomLineSelector) {
        var maxDistance = 30; // Defines the maximal distance of the cursor when the menu is displayed

        var nearest = null;
        var nearestDistance = maxDistance;

        // Find nearest element
        elements.each(function () {
            var element = $(this);
            var x = element.offset().left;
            var w = element.width();
            var y = element.offset().top;
            var h = element.height();

            var posX = position.x + $(window).scrollLeft();
            var posY = position.y + $(window).scrollTop();

            // First check x coordinates - must be within container
            if (posX < x || posX > x + w)
                return;

            // For content-block elements, the menu must be visible at the end
            // For content-block-lists, the menu must be at top
            var cmpHeight = element.is(useBottomLineSelector) ? y + h : y;

            // Check if y coordinates are within boundaries
            var distance = Math.abs(posY - cmpHeight);

            if (distance < maxDistance && distance < nearestDistance) {
                nearest = { x: x, y: cmpHeight, element: element };
                nearestDistance = distance;
            }
        });

        return nearest;
    }
})();