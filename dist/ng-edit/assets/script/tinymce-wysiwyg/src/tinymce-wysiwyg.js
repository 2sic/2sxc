import tinymceWysiwygConfig from './tinymce-wysiwyg-config.js'
import { addTinyMceToolbarButtons } from './tinymce-wysiwyg-toolbar.js'
import { attachAdam } from './tinymce-adam-service.js'
import { attachDnnBridgeService } from './tinymce-dnnbridge-service.js';

(function () {

    class externalTinymceWysiwyg {

        constructor(name, id, host, options, config, currentLang) {
            this.name = name;
            this.id = id;
            this.host = host;
            this.options = options;
            // this.form = form;
            this.config = config;
            this.currentLang = currentLang;

            this.translateService
            this.adam;
        }

        initialize(host, options, form, translateService, id) {
            // if (!this.host) {
            //     this.host = {};
            // }
            this.host = host;
            this.options = options;
            this.form = form;
            this.id = id;
            this.currentLang = translateService.currentLang;
            this.translateService = translateService;
            // Attach adam
            attachAdam(this);
            // Set Adam configuration
            this.setAdamConfig({
                adamModeConfig: { usePortalRoot: false },
                allowAssetsInRoot: true,
                autoLoad: false,
                enableSelect: true,
                folderDepth: 0,
                fileFilter: '',
                metadataContentTypes: '',
                subFolder: '',
                showImagesOnly: false  //adamModeImage?
            });
        }

        render(container) {
            // <div id="fixed-editor-toolbar` + this.id + `"></div>
            container.innerHTML = `<div class="wrap-float-label" style="height:100% !important">
            <textarea id="` + this.id + `" class="field-string-wysiwyg-mce-box"></div>
            </textarea>
            <span id="dummyfocus" tabindex="-1"></span>`;

            var settings = {
                enableContentBlocks: false,
                // auto_focus: false,
            };

            //TODO: add languages
            // angular.extend($scope.tinymceOptions, {
            //     language: lang2,
            //     language_url: "../i18n/lib/tinymce/" + lang2 + ".js"
            // });

            var selectorOptions = {
                selector: 'textarea#' + this.id,
                body_class: 'field-string-wysiwyg-mce-box', // when inline=false
                content_css: 'assets/script/tinymce-wysiwyg/src/tinymce-wysiwyg.css',
                height: '100%',
                branding: false,
                // fixed_toolbar_container: '#fixed-editor-toolbar' + this.id,
                //init_instance_callback: this.tinyMceInitCallback
                setup: this.tinyMceInitCallback.bind(this),
            };

            this.enableContentBlocksIfPossible(settings);
            var options = Object.assign(selectorOptions, this.config.getDefaultOptions(settings));

            options = this.config.setLanguageOptions(this.currentLang, options);

            tinymce.init(options);
        }

        /**
         * function call on change
         * @param {*} event
         * @param {*} value
         */
        changeCheck(event, value) {
            // do validity checks
            var isValid = this.validateValue(value);
            if (isValid) {
                this.host.update(value);
            }
        }

        /**
         * For validating value
         * @param {*} value 
         */
        validateValue(value) {
            // TODO: show validate message ???
            return true;
        }

        /**
         * On render and change set configuration of control
         * @param {*} container - is html container for component
         * @param {*} disabled 
         */
        setOptions(container, disabled) {
            var isReadOnly = tinymce.get(this.id).readonly;
            if (disabled && !isReadOnly) {
                tinymce.get(this.id).setMode('readonly');
            }
            else if (!disabled && isReadOnly) {
                tinymce.get(this.id).setMode('code');
            }
        }

        /**
         * New value from the form into the view
         * This function can be triggered from outside when value changed
         * @param {} container 
         * @param {*} newValue 
         */
        setValue(container, newValue) {
            console.log('[set value] tynimce id:', this.id);
            var oldValue = tinymce.get(this.id).getContent();
            if (newValue !== oldValue) {
                tinymce.get(this.id).setContent(newValue);
            }
        }

        /**
         * on tinyMce setup we set toolbarButtons and change event listener
         * @param {*} editor 
         */
        tinyMceInitCallback(editor) {
            if (editor.settings.language)
                this.config.addTranslations(editor.settings.language, this.translateService);

            // Attach DnnBridgeService
            attachDnnBridgeService(this, editor);

            var imgSizes = this.config.svc().imgSizes;
            addTinyMceToolbarButtons(this, editor, imgSizes);
            editor.on('init', e => {
                // editor.selection.select(editor.getBody(), true);
                // editor.selection.collapse(false);

                this.host.setInitValues();
            });

            editor.on('change', e => {
                console.log('[set value] Editor was change', editor.getContent());
                this.changeCheck(e, editor.getContent())
            });
        }

        enableContentBlocksIfPossible(settings) {
            // quit if there are no following fields
            if (this.options.allInputTypeNames.length === this.options.index + 1) {
                return;
            }
            var nextField = this.options.allInputTypeNames[this.options.index + 1];
            if (nextField === 'entity-content-blocks') {
                settings.enableContentBlocks = true;
            }
        }
    }

    function externalComponentFactory(name) {
        var config = new tinymceWysiwygConfig();
        return new externalTinymceWysiwyg(name, null, null, null, config, 'en');
    }

    window.addOn.register(externalComponentFactory('tinymce-wysiwyg'));
})();