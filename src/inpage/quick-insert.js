$(function () {
    "use strict";
    var enableModuleMove = false; // not implemented yet
    var enableCb = true;
    var enableMod = true;
    var selectors = {
        listContainerSelector: ".sc-content-block-list",
        contentBlockClass: "sc-content-block",
        contentBlockSelector: ".sc-content-block",
        moduleSelector: ".DnnModule",
        paneSelector: ".DNNEmptyPane, .dnnDropEmptyPanes, :has(>.DnnModule)", // Found no better way to get all panes - the hidden variable does not exist when not in edit page mode
        listDataAttr: "data-list-context",
        selected: "sc-cb-is-selected"
    };

    // if it has inner-content, then it's probably a details page, where quickly adding modules
    // would be a problem, so for now, disable modules in this case
    if ($(selectors.listContainerSelector).length > 0)
        enableMod = false;

    var newBlockMenu = $("<div class='sc-content-block-menu sc-i18n'></div>");
    var strButtons = "<a class='sc-content-block-menu-addcontent sc-invisible' data-type='Default' data-i18n='[title]QuickInsertMenu.AddBlockContent'>content</a>"
        + "<a class='sc-content-block-menu-addapp sc-invisible' data-type='' data-i18n='[title]QuickInsertMenu.AddBlockApp'>app</a>"
        + "<a class='sc-content-block-menu-btn sc-cb-action icon-sxc-scissors sc-invisible' data-action='cut' data-i18n='[title]QuickInsertMenu.Cut'></a>"
        + "<a class='sc-content-block-menu-btn sc-cb-action icon-sxc-paste sc-invisible sc-unavailable' data-action='paste' data-i18n='[title]QuickInsertMenu.Paste'></a>"
        + "<a class='sc-content-block-menu-btn sc-cb-action icon-sxc-trash sc-invisible sc-unavailable' data-action='delete' data-i18n='[title]QuickInsertMenu.Delete'></a>";
    var blockActions = $(strButtons);
    if (enableCb)
        newBlockMenu.append(blockActions);

    var moduleActions = $(strButtons.replace(/QuickInsertMenu.AddBlock/g, "QuickInsertMenu.AddModule")).attr("data-context", "module").addClass("sc-content-block-menu-module");
    if (enableMod) 
        newBlockMenu.append(moduleActions);

    $("body").append(newBlockMenu);


    blockActions.click(function () {
        var list = newBlockMenu.actionsForCb.closest(selectors.listContainerSelector);
        var listItems = list.find(selectors.contentBlockSelector);
        var actionConfig = JSON.parse(list.attr(selectors.listDataAttr));
        var index = 0;

        if (newBlockMenu.actionsForCb.hasClass(selectors.contentBlockClass))
            index = listItems.index(newBlockMenu.actionsForCb[0]) + 1;

        // check cut/paste
        var cbAction = $(this).data("action");
        if (!cbAction) {
            var appOrContent = $(this).data("type");
            $2sxc(list).manage.createContentBlock(actionConfig.parent, actionConfig.field, index, appOrContent, list);
        } else
        // this is a cut/paste action
        {
            return copyPasteInPage(cbAction, list, index);
        }
    });

    function copyPasteInPage(cbAction, list, index) {
        var listItems = list.find(selectors.contentBlockSelector);
        var actionConfig = JSON.parse(list.attr(selectors.listDataAttr));
        var currentItem = listItems[index];

        // action!
        if (cbAction === "cut") {
            selectCbOrModule(currentItem);
            $2sxc._cbClipboard = { type: "cb", index: index, item: currentItem };
        } else if (cbAction === "paste") {
            var from = $2sxc._cbClipboard.index, to = index;
            if (!from || !to || from === to || from + 1 === to) // this moves it to the same spot, so ignore
                return unselectAll(); // don't do anything

            $2sxc(list).manage.moveContentBlock(actionConfig.parent, actionConfig.field, from, to);
            unselectAll();
            $2sxc._cbClipboard = null;
        } else if (cbAction === "cancel") // todo: ui not implemented yet
            unselectAll();
        else if (cbAction === "delete") {
            alert("todo not implemented yet");
        }
    }

    function selectCbOrModule(element) {
        $("." + selectors.selected).removeClass(selectors.selected);
        var cb = $(element);
        cb.addClass(selectors.selected);
        if (cb.prev().is("iframe"))
            cb.prev().addClass(selectors.selected);
        setSecondaryActionsState(true);
    }

    function unselectAll() {
        $("." + selectors.selected).removeClass(selectors.selected);
        $2sxc._cbClipboard = {};
        setSecondaryActionsState(false);
    }

    function setSecondaryActionsState(state) {
        var btns = $("a.sc-content-block-menu-btn");
        btns = btns.filter(".icon-sxc-paste");// later also : , .icon-sxc-trash"); // only on the main one...?
        btns.toggleClass("sc-unavailable", !state);
    }

    moduleActions.click(function () {
        var type = $(this).data("type");
        var pane = newBlockMenu.actionsForModule.closest(selectors.paneSelector);
        var paneName = pane.attr("id").replace("dnn_", "");

        var index = 0;
        if (newBlockMenu.actionsForModule.hasClass("DnnModule"))
            index = pane.find(".DnnModule").index(newBlockMenu.actionsForModule[0]) + 1;

        // todo: try to use $2sxc(...).webApi instead of custom re-assembling these common build-up things
        // how: create a object containing the url, data, then just use the sxc.webApi(yourobject)
        var service = $.dnnSF();
        var serviceUrl = service.getServiceRoot("internalservices") + "controlbar/";

        $.ajax({
            url: serviceUrl + "GetPortalDesktopModules",
            type: "GET",
            data: "category=All&loadingStartIndex=0&loadingPageSize=100&searchTerm=",
            beforeSend: service.setModuleHeaders,
            success: function (desktopModules) {
                var moduleToFind = type === "Default" ? " Content" : " App";
                var module = null;
                
                desktopModules.forEach(function (e,i) {
                    if (e.ModuleName === moduleToFind)
                        module = e;
                });

                if (!module)
                    return alert(moduleToFind + " module not found.");

                var postData = {
                    Module: module.ModuleID,
                    Page: "",
                    Pane: paneName,
                    Position: -1,
                    Sort: index,
                    Visibility: 0,
                    AddExistingModule: false,
                    CopyModule: false
                };



                $.ajax({
                    url: serviceUrl + "AddModule",
                    type: "POST",
                    data: postData,
                    beforeSend: service.setModuleHeaders,
                    success: function (d) {
                        window.location.reload();
                    },
                    error: function (xhr) {
                        alert("Error while adding module.");
                        console.log(xhr);
                    }
                });
            },
            error: function (xhr) {
                alert("Error while adding module.");
                console.log(xhr);
            }
        });

        

    });

    var refreshTimeout = null;
    $("body").on("mousemove", function (e) {
        
        if (refreshTimeout === null)
            refreshTimeout = window.setTimeout(function () {
                requestAnimationFrame(function () {
                    refreshMenu(e);
                    refreshTimeout = null;
                });
            }, 20);

    });
    

    function refreshMenu(e) { // ToDo: Performance is not solved with requestAnimationFrame, needs throttling (or more performant selectors etc.)

        // Prepare offset calculation based on body positioning
        var body = $("body"), nearestCb = null, nearestModule = null;
        var bodyPos = body.css("position");
        var offset = bodyPos === "relative" || bodyPos === "absolute"
            ? { x: body.offset().left, y: body.offset().top }
            : { x: 0, y: 0 };

        if (enableCb) {
            var contentBlocks = $(selectors.listContainerSelector).find(selectors.contentBlockSelector)
                .add(selectors.listContainerSelector);
            nearestCb = findNearest(contentBlocks, { x: e.clientX, y: e.clientY }, selectors.contentBlockSelector);
        }

        if (enableMod) {
            var modules = $(selectors.paneSelector).find(selectors.moduleSelector)
                .add(selectors.paneSelector);
            nearestModule = findNearest(modules, { x: e.clientX, y: e.clientY }, selectors.moduleSelector);
        }

        moduleActions.toggleClass("sc-invisible", nearestModule === null);
        blockActions.toggleClass("sc-invisible", nearestCb === null);

        // if previously a parent-pane was highlighted, un-highlight it now
        if (newBlockMenu.parentContainer)
            $(newBlockMenu.parentContainer).removeClass("sc-cb-highlight-for-insert");

        if (nearestCb !== null || nearestModule !== null) {
            var alignTo = nearestCb || nearestModule;

            // find parent pane to highlight
            var parentPane = $(alignTo.element).closest(selectors.paneSelector);
            var parentCbList = $(alignTo.element).closest(selectors.listContainerSelector);
            var parentContainer = (parentCbList.length ? parentCbList : parentPane)[0];

            newBlockMenu.css({
                'left': alignTo.x - offset.x,
                'top': alignTo.y - offset.y,
                'width': alignTo.element.width()
            }).show();

            // Keep current block as current on menu
            newBlockMenu.actionsForCb = nearestCb ? nearestCb.element : null;
            newBlockMenu.actionsForModule = nearestModule ? nearestModule.element : null;
            newBlockMenu.parentContainer = parentContainer;
            $(parentContainer).addClass("sc-cb-highlight-for-insert");
        } else {
            newBlockMenu.hide();
        }
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
});