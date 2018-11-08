// (function () {

export default class tinymceWysiwygConfig {

    svc() {
        return {
            // cdn root
            cdnRoot: "//cdn.tinymce.com/4",
            // these are the sizes we can auto-resize to
            imgSizes: [100, 75, 70, 66, 60, 50, 40, 33, 30, 25, 10],

            // the default language, in which we have all labels/translations
            defaultLanguage: "en",

            // all other languages
            languages: "de,es,fr,it,uk,nl".split(","),

            // tinyMCE plugins we're using
            plugins: [
                "code", // allow view / edit source
                "contextmenu", // right-click menu for things like insert, etc.
                "autolink", // automatically convert www.xxx links to real links
                "tabfocus", // get in an out of the editor with tab
                "image", // image button and image-settings
                "link", // link button + ctrl+k to add link
                // "autosave",     // temp-backups the content in case the browser crashes, allows restore
                "paste", // enables paste as text from word etc. https://www.tinymce.com/docs/plugins/paste/
                "anchor", // allows users to set an anchor inside the text
                "charmap", // character map https://www.tinymce.com/docs/plugins/visualchars/
                "hr", // hr
                "media", // video embed
                "nonbreaking", // add button to insert &nbsp; https://www.tinymce.com/docs/plugins/nonbreaking/
                "searchreplace", // search/replace https://www.tinymce.com/docs/plugins/searchreplace/
                "table", // https://www.tinymce.com/docs/plugins/searchreplace/
                "lists", // should fix bug with fonts in list-items (https://github.com/tinymce/tinymce/issues/2330),
                "textpattern" // enable typing like "1. text" to create lists etc.
            ],

            validateAlso: '@[class]' // allow classes on all elements, 
                + ',i' // allow i elements (allows icon-font tags like <i class="fa fa-...">)
                + ",hr[sxc|guid]" // experimental: allow inline content-blocks
        }
    };

    buildModes(settings) {
        // the WYSIWYG-modes we offer, standard with simple toolbar and advanced with much more
        return {
            standard: {
                menubar: false,
                toolbar: " undo redo removeformat "
                    + "| bold formatgroup "
                    + "| h1 h2 hgroup "
                    + "| listgroup "
                    + "| linkfiles linkgroup "
                    + "| " + (settings.enableContentBlocks ? " addcontentblock " : "") + "modeadvanced ",
                contextmenu: "charmap hr" + (settings.enableContentBlocks ? " addcontentblock" : "")
            },
            advanced: {
                menubar: true,
                toolbar: " undo redo removeformat "
                    + "| styleselect "
                    + "| bold italic "
                    + "| h1 h2 hgroup "
                    + "| bullist numlist outdent indent "
                    + "| images linkfiles linkgrouppro "
                    + "| code modestandard ",
                contextmenu: "link image | charmap hr adamimage"
            }
        };
        // return modes;
    }

    getDefaultOptions(settings) {
        var modes = this.buildModes(settings);
        var svc = this.svc();
        return {
            baseURL: svc.cdnRoot,
            inline: true, // use the div, not an iframe
            automatic_uploads: false, // we're using our own upload mechanism
            modes: modes, // for later switch to another mode
            menubar: modes.standard.menubar, // basic menu (none)
            toolbar: modes.standard.toolbar, // basic toolbar
            plugins: svc.plugins.join(" "),
            contextmenu: modes.standard.contextmenu, //"link image | charmap hr adamimage",
            autosave_ask_before_unload: false,
            paste_as_text: true,
            extended_valid_elements: this.svc.validateAlso,
            //'@[class]' // allow classes on all elements, 
            //+ ',i' // allow i elements (allows icon-font tags like <i class="fa fa-...">)
            //+ ",hr[sxc|guid]", // experimental: allow inline content-blocks
            custom_elements: "hr",

            // Url Rewriting in images and pages
            //convert_urls: false,  // don't use this, would keep the domain which is often a test-domain
            relative_urls: false, // keep urls with full path so starting with a "/" - otherwise it would rewrite them to a "../../.." syntax
            default_link_target: "_blank", // auto-use blank as default link-target
            object_resizing: false, // don't allow manual scaling of images

            // General looks
            skin: "lightgray",
            theme: "modern",
            // statusbar: true,    // doesn't work in inline :(

            language: svc.defaultLanguage,

            debounce: false // DONT slow-down model updates - otherwise we sometimes miss the last changes

            //paste_preprocess: function (plugin, args) {
            //    console.log(args.content);
            //    args.content += ' preprocess';
            //},

            //paste_postprocess: function (plugin, args) {
            //    console.log(args.node);
            //    args.node.setAttribute('id', '42');
            //}
        };
    };
}
// })();
