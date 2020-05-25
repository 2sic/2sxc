---
uid: Specs.Form.Field.Wysiwyg
---

# Customizing the WYSIWYG Field in 2sxc 11

> [!TIP]
> These are the technical specs for reference. Make sure you first read the [how to](xref:HowTo.Customize.EditUx) before you start. 

Since WYSIWYG is so complex, with image-handling, special paste etc. we believe most developers are better of using the existing system, and just reconfiguring it. That's what we'll explain here. 

## Make sure TinyMCE is loaded

We must first load the standard WYSIWYG control before we start, otherwise you'll run into timing issues. The best way to do this can be seen in the [tutorial](https://2sxc.org/dnn-tutorials/en/razor/ui241/page), but this is what you need:

```ts
  const builtInWysiwyg = '[System:Path]/system/field-string-wysiwyg/index.js';

  /** Our WebComponent which is a custom, lightweight wysiwyg editor */
  class StringWysiwygCustom extends HTMLElement {

    /* connectedCallback() is the standard callback  when the component has been attached */
    connectedCallback() {
      // We need to ensure that the standard WYSIWYG is also loaded
      this.connector.loadScript('tinymce', builtInWysiwyg, (x) => { this.initWysiwygCallback() })
    }

    initWysiwygCallback() {
      // ...
    }
  }
```

This way the form will load the built-in WYSIWYG control and trigger your callback, OR if it has already been loaded, immediately trigger your callback. 

## Configure the TinyMCE WYSIWYG Web Control

The control has these public properties:

1. `mode` - values can be `edit` and `preview`, default is `preview`
1. `connector` - the object that every form control needs. You must connect this
1. `reconfigure` a special object that can change the configuration at various points

Code Sample:

```js
  const tagName = 'field-string-wysiwyg-micro';
  const builtInWysiwyg = '[System:Path]/system/field-string-wysiwyg/index.js';

  /** Our WebComponent which is a custom, lightweight wysiwyg editor */
  class StringWysiwygCustom extends HTMLElement {

    /* connectedCallback() is the standard callback  when the component has been attached */
    connectedCallback() {
      // We need to ensure that the standard WYSIWYG is also loaded
      this.connector.loadScript('tinymce', builtInWysiwyg, (x) => { this.initWysiwygCallback() })
    }

    initWysiwygCallback() {
      // 1. Create a built-in field-string-wysiwyg control
      const wysiwyg = document.createElement('field-string-wysiwyg');
      // 2. tell it if it should start in preview or edit
      wysiwyg.mode = 'edit'; // can be 'preview' or 'edit'
      // 3. attach connector
      wysiwyg.connector = this.connector;
      // 4. also attach reconfigure object which can change the TinyMCE as it's initialized
      wysiwyg.reconfigure = new WysiwygReconfigurator();
      // 5. Append it to the DOM. Do this last, as it will trigger connectedCallback() in the wysiwyg
      this.appendChild(wysiwyg);
    }
  }

  /** The object which helps reconfigure what the editor will do */
  class WysiwygReconfigurator {
    configureOptions(options) {
      options.toolbar = "undo redo | bold italic"
      return options;
    }
  }

  // Register this web component - if it hasn't been registered yet
  if (!customElements.get(tagName)) customElements.define(tagName, StringWysiwygCustom);
```

## `connector` Object

* for the `connector` object please consult [connector API](xref:Specs.Form.JsConnector)


## Understanding TinyMCE and Life-Cycle

TincMCE has a huge set of options, so let's just get a quick idea of how things work inside it

1. Once loaded, there is a global `tinymce` object which is like a master-controller for all tinyMCE editors. We call it the **Editor Manager**
1. Each editor has a personal `editor` object which has the configuration for just that editor

### What is configured on `tinymce`?

The global `tinymce` controller is responsible for things like

* Translations
* `options`, including
    * `plugins` which are activated
    * `skin` and `theme`
    * `custom_elements`
    * ...and way more

### What is configured on an `editor`?

* Buttons (for use in Toolbars)
* Toolbars
* ADAM and DNN-Bridge

### Initialization Process / Life-Cycle

This is how the control is loaded / built in the 2sxc/EAV form. Note that for each method explained on the `reconfigure` object, the initializers will do [duck typing](https://en.wikipedia.org/wiki/Duck_typing) to check if that method exists and if detected, will run that.

#### #1 Load Phase

1. The `field-string-wysiwyg` WebControl is created  
At this time, the `mode`, `connector` and `reconfigure` must already be set by the parent
1. It will requests that the from loads all the TinyMCE JavaScripts
1. When that has completed, it will fire a callback to start translating / configuring

#### #2 Translation Phase

In this phase, translation maps are built, so buttons can show labels in various languages. This map is global, so try to avoid name clashes. 

1. First the built-in translations of 2sxc/EAV are added
1. Then `reconfigure.addTranslations(editorManager, currentLanguage)` is called. In this phase you can add your own translations according to tinyMCE standards or modify prebuilt translations. 

#### #3 Manager Configuration Phase

1. At the beginning `reconfigure.managerInit(editorManager)` is called so you can pre-initialize something. We don't really know why you would need this, but we added it just in case. 
1. Then the default `options` are generated
1. Now `reconfigure.optionsInit(options, buttonOptions)` is called. Here you can change the objects as you need to add/remove options.
    1. the `options` are the standard tinyMCE options which have been prebuilt
    1. The `buttonOptions` are a special object which affects automatic button definitions. This is still WIP
1. Now the configuration system builds more options based on the environment, features etc. 
1. Then it calls `reconfigure.optionsReady(options)` so you could make some final changes.
1. Now `tinymce` (the Editor Manager) receives the options to start. This also includes a callback (provided by our form) which will do editor initialization. 

#### #4 Editor Configuration Phase

1. When the tinyMCE editor is finally created, a callback or the wysiwyg is triggered. This will attach various events like `init`, `focus`, `blur`, `change`, `undo` etc.
1. Once attaching these events is done, it calls `reconfigure.editorBuilt(editor)` so you could make changes. 
1. When `init` is triggered, it will first call `reconfigure.editorInit(editor)` so you can make changes or add buttons using the tinyMCE API. 
1. It will then run internal code to add all the button definitions like `H1`, `H2` etc.
1. Then it asks for `reconfigure.disablePagePicker` and if not `true`, will attach the DNN page picker
1. It will also ask for `reconfigure.disableAdam` and if not true, will attach ADAM functionality


## How to Modify the Behavior

Best check out these tutorials

* [Basic tutorial, just providing 4 standard buttons](https://2sxc.org/dnn-tutorials/en/razor/ui241/page)
* [Advanced tutorial adding a custom button](https://2sxc.org/dnn-tutorials/en/razor/ui242/page)


## History

1. New system in 2sxc 11 using WebComponents (previously this was not possible)