(function () {
    'use strict';

    var newBlockMenu = $("<div class='sc-content-block-menu'></div>");
    var blockActions = $("<a class='sc-content-block-menu-addcontent' data-type='Default'>Add Content</a><a class='sc-content-block-menu-addapp' data-type=''>Add app</a>");

    // ToDo 2rm: Add menu definition for DNN modules (allow quick-insert of modules)
    var moduleActions = blockActions.clone().attr('data-context', 'module').hide();
    
    newBlockMenu.append(blockActions).append(moduleActions);

    $("body").append(newBlockMenu);

    var menuDefinitions = [
        {
            listContainerSelector: '.sc-content-block-list',
            contentBlockSelector: '.sc-content-block',
            action: function (type) {
                var list = newBlockMenu.actionsFor.closest('.sc-content-block-list');
                var actionConfig = JSON.parse(list.attr('data-sc-list'));

                var index = 0;
                if (newBlockMenu.actionsFor.hasClass('sc-content-block'))
                    index = list.find('.sc-content-block').index(newBlockMenu.actionsFor[0]) + 1;

                $2sxc(list).manage.createContentBlock(actionConfig.id, actionConfig.field, index, type, list);
            }
        }
    ];

    newBlockMenu.find('a').click(function () {
        var type = $(this).data("type");
        newBlockMenu.actionsFor[0].menuDefinition.action(type);
    });

    $("body").on('mousemove', function (e) {
        
        var contentBlocks = [];

        menuDefinitions.forEach(function (e,i) {
            var definition = e;
            contentBlocks = $(definition.listContainerSelector).find(definition.contentBlockSelector)
                .add(definition.listContainerSelector);
            contentBlocks.each(function () {
                this.menuDefinition = definition;
            });
        });

        var nearest = findNearest(contentBlocks, { x: e.clientX, y: e.clientY }, menuDefinitions[0].contentBlockSelector);

        if (nearest !== null) {
            newBlockMenu.css({
                'left': nearest.x,
                'top': nearest.y,
                'width': nearest.element.width()
            }).show();

            // Keep current block as current on menu
            newBlockMenu.actionsFor = nearest.element;
        }
        else
            newBlockMenu.hide();

    });

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