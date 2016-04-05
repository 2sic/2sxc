

// ToDo: Move this to correct location (just poc-code)
var newBlockMenu = $("<div class='sc-content-block-menu'><a class='sc-content-block-menu-addcontent' data-type='Default'>Add Content</a><a class='sc-content-block-menu-addapp' data-type=''>Add app</a></div>");
$("body").append(newBlockMenu);

newBlockMenu.find('a').click(function () {
    var type = $(this).data("type");
    var list = newBlockMenu.actionsFor.closest('.sc-content-block-list');

    var actionConfig = JSON.parse(list.attr('data-sc-list'));

    $2sxc(newBlockMenu.actionsFor).manage.createContentBlock(actionConfig.id, actionConfig.field, 0, type);
});

$("body").on('mousemove', function (e) {
    // e.clientX, e.clientY holds the coordinates of the mouse
    var maxDistance = 30; // Defines the maximal distance of the cursor when the menu is displayed

    var nearest = null;
    var nearestDistance = maxDistance;

    var contentBlocks = $(".sc-content-block").add(".sc-content-block-list");

    // Find nearest content block
    contentBlocks.each(function () {
        var block = $(this);
        var x = block.offset().left;
        var w = block.width();
        var y = block.offset().top;
        var h = block.height();

        var mouseX = e.clientX + $(window).scrollLeft();
        var mouseY = e.clientY + $(window).scrollTop();

        // First check x coordinates - must be within container
        if (mouseX < x || mouseX > x + w)
            return;

        // For content-block elements, the menu must be visible at the end
        // For content-block-lists, the menu must be at top
        var cmpHeight = block.hasClass('sc-content-block') ? y + h : y;

        // Check if y coordinates are within boundaries
        var distance = Math.abs(mouseY - cmpHeight);

        if (distance < maxDistance && distance < nearestDistance) {

            newBlockMenu.css({
                'left': x,
                'top': cmpHeight,
                'width': w
            }).show();

            // Add class if menu creates modules
            //newBlockMenu.toggleClass('sc-content-block-menu-module', block.hasClass('sc-viewport'));

            // Keep current block as current on menu
            newBlockMenu.actionsFor = nearest = block;

            nearestDistance = distance;
        }
    });

    if (nearest === null)
        newBlockMenu.hide();
});
