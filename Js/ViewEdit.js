$(document).ready(function () {
    $(".sc-element").hover(function () {
        $(".sc-menu", this).show();
    }, function () {
        $(".sc-menu", this).hide();
    });

    $(".sc-menu").click(function (e) {
        e.stopPropagation();
    });
});

function AddContentGroupItem(ClickedButton, ContentGroupItemID) {
    var ContentGroupItemIDField = $(ClickedButton).parents(".DnnModule").find("[id$=hfContentGroupItemID]:first");
    var ContentGroupItemActionField = $(ClickedButton).parents(".DnnModule").find("[id$=hfContentGroupItemAction]:first");

    ContentGroupItemIDField.val(ContentGroupItemID);
    ContentGroupItemActionField.val("add");
    $("form").submit();
}