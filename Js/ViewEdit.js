$(document).ready(function () {
    // Prevent propagation of the click (if menu was clicked)
    $('.sc-menu').click(function (e) {
        e.stopPropagation();
    });

    $('.DnnModule-2sxc .Mod2sxcC').each(function() {
        var moduleId = $.parseJSON($(this).attr('data-2sxc')).moduleId;

        $('.sc-menu', this).each(function () {
            var toolbarSettings = $.parseJSON($(this).attr('data-toolbar'));
            $(this).replaceWith($2sxc(moduleId).manage.getToolbar(toolbarSettings));
        });
    });


});




//function AddContentGroupItem(ClickedButton, ContentGroupItemID) {
//    var ContentGroupItemIDField = $(ClickedButton).parents(".DnnModule").find("[id$=hfContentGroupItemID]:first");
//    var ContentGroupItemActionField = $(ClickedButton).parents(".DnnModule").find("[id$=hfContentGroupItemAction]:first");

//    ContentGroupItemIDField.val(ContentGroupItemID);
//    ContentGroupItemActionField.val("add");
//    $("form").submit();
//}

// Beta-Functionality (not stable yet)
// Basic usage: 
//    $2sxc.beta.OpenDialog("new", {
//       moduleId: 391,
//       tabId: 55,
//       contentGroupId: 391,
//       prefill: {
//           Name: "Raphael Müller"
//       }
//    });
//window.$2sxc = window.$2sxc || {};
//$2sxc.beta = {
//    GetUrl: function (urlType, settings) {

//        if (urlType == "new") {
//            var s = $.extend({
//                moduleId: null,
//                tabId: null,
//                contentGroupId: null,
//                sortOrder: null,
//                prefill: {},
//                returnUrl: window.location.href
//            }, settings);

//            return "/Default.aspx?tabid=" + s.tabId + "&ctl=editcontentgroup&mid=" + s.moduleId + (s.sortOrder != null ? "&SortOrder=" + s.sortOrder : "") + "&ContentGroupID=" +
//                s.contentGroupId + "&popUp=true&ReturnUrl=" + encodeURIComponent(s.returnUrl) + "&EditMode=New&prefill=" + encodeURIComponent(JSON.stringify(s.prefill));
//        }
//        else {
//            throw "Url Type '" + urlType + "' not valid.";
//        }

//    },

//    OpenDialog: function (dialogType, settings) {
//        dnnModal.show(this.GetUrl(dialogType, settings), /*showReturn*/true, 550, 950, true, '');
//    }
//};
