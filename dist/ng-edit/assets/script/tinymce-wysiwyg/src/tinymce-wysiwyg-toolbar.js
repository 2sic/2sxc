
// (function () {

export function addTinyMceToolbarButtons(vm, editor, imgSizes) {
    var editor = editor;
    //#region helpers like initOnPostRender(name)

    //TODO: translate:
    // if ($scope.tinymceOptions.language)
    //             tinyMceHelpers.addTranslations(editor, $scope.tinymceOptions.language);

    // helper function to add activate/deactivate to buttons like alignleft, alignright etc.
    function initOnPostRender(name) { // copied/modified from https://github.com/tinymce/tinymce/blob/ddfa0366fc700334f67b2c57f8c6e290abf0b222/js/tinymce/classes/ui/FormatControls.js#L232-L249
        return function () {
            var self = this; // keep ref to the current button?

            function watchChange() {
                editor.formatter.formatChanged(name, function (state) {
                    self.active(state);
                });
            }
            if (editor.formatter) {
                watchChange();
            }
            else {
                editor.on('init', function () {
                    watchChange();
                });
            }
        };
    }

    //#endregion

    //#region register formats

    // the method that will register all formats - like img-sizes
    function registerTinyMceFormats(editor, host) {
        var imgformats = {};
        for (var is = 0; is < imgSizes.length; is++)
            imgformats["imgwidth" + imgSizes[is]] = [{ selector: "img", collapsed: false, styles: { 'width': imgSizes[is] + "%" } }];
        editor.formatter.register(imgformats);
    }

    // call register once the editor-object is ready
    editor.on('init', function () {
        console.log('editor SetContent init registerTinyMceFormats');
        registerTinyMceFormats(editor, vm.host);
    });

    //#endregion

    // group with adam-link, dnn-link
    editor.addButton("linkfiles", {
        type: "splitbutton",
        icon: " eav-icon-file-pdf",
        title: "Link.AdamFile.Tooltip",
        onclick: function () {
            vm.toggleAdam(false);
        },
        menu: [
            {
                text: "Link.AdamFile",
                tooltip: "Link.AdamFile.Tooltip",
                icon: " eav-icon-file-pdf",
                onclick: function () {
                    vm.toggleAdam(false);
                }
            }, {
                text: "Link.DnnFile",
                tooltip: "Link.DnnFile.Tooltip",
                icon: " eav-icon-file",
                onclick: function () {
                    vm.openDnnDialog("documentmanager");
                }
            }
        ]
    });

    //#region link group with web-link, page-link, unlink, anchor
    var linkgroup = {
        type: "splitbutton",
        icon: "link",
        title: "Link",
        onPostRender: initOnPostRender("link"),
        onclick: function () {
            editor.execCommand("mceLink");
        },

        menu: [
            { icon: "link", text: "Link", onclick: function () { editor.execCommand("mceLink"); } },
            {
                text: "Link.Page",
                tooltip: "Link.Page.Tooltip",
                icon: " eav-icon-sitemap",
                onclick: function () {
                    vm.openDnnDialog("pagepicker");
                }
            }
        ]
    };
    var linkgroupPro = { ...linkgroup };
    linkgroupPro.menu.push({ icon: " eav-icon-anchor", text: "Anchor", tooltip: "Link.Anchor.Tooltip", onclick: function () { editor.execCommand("mceAnchor"); } });
    editor.addButton("linkgroup", linkgroup);
    editor.addButton("linkgrouppro", linkgroupPro);
    //#endregion

    // group with images (adam) - only in PRO mode
    editor.addButton("images", {
        type: "splitbutton",
        text: "",
        icon: "image",
        onclick: function () {
            vm.toggleAdam(true);
        },
        menu: [
            {
                text: "Image.AdamImage",
                tooltip: "Image.AdamImage.Tooltip",
                icon: "image",
                onclick: function () {
                    vm.toggleAdam(true);
                }
            }, {
                text: "Image.DnnImage",
                tooltip: "Image.DnnImage.Tooltip",
                icon: "image",
                onclick: function () {
                    vm.toggleAdam(true, true);
                }
            }, {
                text: "Insert\/edit image", // i18n tinyMce standard
                icon: "image",
                onclick: function () { editor.execCommand("mceImage"); }

            },
            // note: all these use i18n from tinyMce standard
            { icon: "alignleft", tooltip: "Align left", onclick: function () { editor.execCommand("JustifyLeft"); } },
            { icon: "aligncenter", tooltip: "Align center", onclick: function () { editor.execCommand("JustifyCenter"); } },
            { icon: "alignright", tooltip: "Align right", onclick: function () { editor.execCommand("JustifyRight"); } }
        ]
    });

    // drop-down with italic, strikethrough, ...
    editor.addButton("formatgroup", {
        type: "splitbutton",
        tooltip: "Italic",  // will be autotranslated
        text: "",
        icon: "italic",
        cmd: "italic",
        onPostRender: initOnPostRender("italic"),
        menu: [
            { icon: "strikethrough", text: "Strikethrough", onclick: function () { editor.execCommand("strikethrough"); } },
            { icon: "superscript", text: "Superscript", onclick: function () { editor.execCommand("superscript"); } },
            { icon: "subscript", text: "Subscript", onclick: function () { editor.execCommand("subscript"); } }
        ]
    });

    // drop-down with italic, strikethrough, ...
    editor.addButton("listgroup", {
        type: "splitbutton",
        tooltip: "Numbered list",  // official tinymce key
        text: "",
        icon: "numlist",
        cmd: "InsertOrderedList",
        onPostRender: initOnPostRender("numlist"),  // for unknown reasons, this just doesn't activate correctly :( - neither does the bullist
        menu: [
            { icon: "bullist", text: "Bullet list", onPostRender: initOnPostRender("bullist"), onclick: function () { editor.execCommand("InsertUnorderedList"); } },
            { icon: "outdent", text: "Outdent", onclick: function () { editor.execCommand("Outdent"); } },
            { icon: "indent", text: "Indent", onclick: function () { editor.execCommand("Indent"); } }
        ]
    });

    //#region mode switching and the buttons for it
    function switchModes(mode) {
        console.log('switchModes1', editor.settings.modes[mode].toolbar);
        editor.settings.toolbar = editor.settings.modes[mode].toolbar;
        editor.settings.menubar = editor.settings.modes[mode].menubar;
        // editor.settings.contextmenu = editor.settings.modes[mode].contextmenu; - doesn't work at the moment

        // refresh editor toolbar when it's in inline mode  (inline true)
        // editor.theme.panel.remove();    // kill current toolbar
        // editor.theme.renderUI(editor);

        // refresh editor toolbar when it's NOT in inline mode (inline false)
        tinymce.remove(editor);
        tinymce.init(editor.settings);

        editor.execCommand("mceFocus");

        // focus away...
        document.getElementById("dummyfocus").focus();

        // ...and focus back a bit later
        setTimeout(function () {
            editor.focus();
        }, 100);
    }

    editor.addButton("modestandard", {
        icon: " eav-icon-cancel",
        tooltip: "SwitchMode.Standard",
        onclick: function () { switchModes("standard"); }
    });

    editor.addButton("modeadvanced", {
        icon: " eav-icon-pro",
        tooltip: "SwitchMode.Pro",
        onclick: function () { switchModes("advanced"); }
    });
    //#endregion

    //#region h1, h2, etc. buttons, inspired by http://blog.ionelmc.ro/2013/10/17/tinymce-formatting-toolbar-buttons/
    // note that the complex array is needede because auto-translate only happens if the string is identical
    [["pre", "Preformatted", "Preformatted"],
    ["p", "Paragraph", "Paragraph"],
    ["code", "Code", "Code"],
    ["h1", "Heading 1", "H1"],
    ["h2", "Heading 2", "H2"],
    ["h3", "Heading 3", "H3"],
    ["h4", "Heading 4", "Heading 4"],
    ["h5", "Heading 5", "Heading 5"],
    ["h6", "Heading 6", "Heading 6"]].forEach(function (tag) {
        editor.addButton(tag[0], {
            tooltip: tag[1],
            text: tag[2],
            onclick: function () { editor.execCommand("mceToggleFormat", false, tag[0]); },
            onPostRender: function () {
                var self = this,
                    setup = function () {
                        editor.formatter.formatChanged(tag[0], function (state) {
                            self.active(state);
                        });
                    };
                var x = editor.formatter ? setup() : editor.on("init", setup);
            }
        });
    });

    // group of buttons with an h3 to start and showing h4-6 + p // ) angular.extend({}, editor.buttons.h3,
    editor.addButton("hgroup", Object.assign({}, editor.buttons.h3,
        {
            type: "splitbutton",
            menu: [
                editor.buttons.h4,
                editor.buttons.h5,
                editor.buttons.h6,
                editor.buttons.p
            ]
        }));
    //#endregion

    // #region inside content
    editor.addButton("addcontentblock", {
        icon: " eav-icon-content-block",
        classes: "btn-addcontentblock",
        tooltip: "ContentBlock.Add",
        onclick: function () {
            var guid = Math.uuid().toLowerCase(); // requires the uuid-generator to be included
            editor.insertContent('<hr sxc=\"sxc-content-block\" guid=\"' + guid + '\" />');
        }
    });
    // #endregion

    //#region image alignment / size buttons
    editor.addButton("alignimgleft", { icon: " eav-icon-align-left", tooltip: "Align left", cmd: "JustifyLeft", onPostRender: initOnPostRender("alignleft") });
    editor.addButton("alignimgcenter", { icon: " eav-icon-align-center", tooltip: "Align center", cmd: "justifycenter", onPostRender: initOnPostRender("aligncenter") });
    editor.addButton("alignimgright", { icon: " eav-icon-align-right", tooltip: "Align right", cmd: "justifyright", onPostRender: initOnPostRender("alignright") });

    var imgMenuArray = [];
    function makeImgFormatCall(size) { return function () { editor.formatter.apply("imgwidth" + size); }; }
    for (var is = 0; is < imgSizes.length; is++) {
        var config = {
            icon: " eav-icon-resize-horizontal",
            tooltip: imgSizes[is] + "%",
            text: imgSizes[is] + "%",
            onclick: makeImgFormatCall(imgSizes[is]),
            onPostRender: initOnPostRender("imgwidth" + imgSizes[is])
        };
        editor.addButton("imgresize" + imgSizes[is], config);
        imgMenuArray.push(config);
    }

    editor.addButton("resizeimg100", {
        icon: " eav-icon-resize-horizontal", tooltip: "100%",
        onclick: function () { editor.formatter.apply("imgwidth100"); },
        onPostRender: initOnPostRender("imgwidth100")
    });

    // group of buttons to resize an image 100%, 50%, etc.
    editor.addButton("imgresponsive", Object.assign({}, editor.buttons.resizeimg100,
        { type: "splitbutton", menu: imgMenuArray }));
    //#endregion

    //#region my context toolbars for links, images and lists (ul/li)
    function makeTagDetector(tagWeNeedInTheTagPath) {
        return function tagDetector(currentElement) {
            // check if we are in a tag within a specific tag
            var selectorMatched = editor.dom.is(currentElement, tagWeNeedInTheTagPath) && editor.getBody().contains(currentElement);
            return selectorMatched;
        };
    }

    editor.addContextToolbar(makeTagDetector("a"), "link unlink");
    editor.addContextToolbar(makeTagDetector("img"), "image | alignimgleft alignimgcenter alignimgright imgresponsive | removeformat | remove");
    editor.addContextToolbar(makeTagDetector("li,ol,ul"), "bullist numlist | outdent indent");
    //#endregion
}


// })();

