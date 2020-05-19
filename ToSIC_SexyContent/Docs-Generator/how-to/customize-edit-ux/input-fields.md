---
uid: HowTo.Customize.InputFields
---
# How To Create Custom Input Fields (v11.2)

Sometimes you want a custom input field - as color-picker, dropdown-from-api or whatever. 

> [!TIP]
> 1. 2sxc 11 finally allows you to do this using simple [WebComponents](https://developer.mozilla.org/en-US/docs/Web/Web_Components)
> 1. Registering these happens by placing them in a specific folder  
> 1. You can also make them configurable by placing a content-type json in another folder  

> [!NOTE]
> There are more ways to provide and register custom input fields - like when you need them globally across many apps and portals. 
> That is not discussed here. 

## What kind of Custom Input Field can you Create

You can create any kind of custom input field, as a JavaScript WebComponent. 

1. Look and Feel however you want it
1. Any kind of JS code
1. Talking to any other system (Google Maps, etc.)
1. Talking to any endpoint (weather APIs)

> [!TIP]
> This overview will get you started, but we've already created demos on the [2sxc Tutorials](TODO:). If you want to know more, you should also read the [specs](TODO:)

## Getting Started with Custom Input Fields

Basically a custom Input Field is just a `index.js` in the correct folder. These are the specs:

1. An input field as described here is an **App Extension**. 
    1. All **App Extensions** must each lie in an own folder...
    1. within a folder called `system` inside the _App folder_
1. The folder name for your custom input field must obey certain naming rules so that they are auto-detected. 
1. The javascript that will be loaded must be called `index.js`
1. Your script must register a [custom element](https://developer.mozilla.org/en-US/docs/Web/Web_Components/Using_custom_elements) - a WebComponent - in the browser
1. The name of your custom element is predefined, and must adhere to the naming rules.

Here's a checklist to get this setup

<iframe src="https://azing.org/2sxc/r/n8nJ1dfd?embed=1" width="100%" height="400" frameborder="0" allowfullscreen style="box-shadow: 0 1px 3px rgba(60,64,67,.3), 0 4px 8px 3px rgba(60,64,67,.15)"></iframe>

> [!Note]
> Once you have that setup, the input field is automatically detected and a user can choose it as a field type in the configuration. 


## Some Background on WebComponents

1. The WebComponent has a simple lifecycle - from when it's created to when it receives data and can push changes back to the form.
1. The form itself is reactive. This means that your field will receive messages when the value changes or when other values change (in case you want to use other field values in your input).
1. The API to communicate with the form has a few complexities you need to know. This is because the form is very dynamic - so the user could switch languages, and your input field needs to react to this.

So let's get started 🚀!


## Getting the HTML into the Custom Input

WebControls are developed using pure JavaScript, but a control is automatically a rich DOM object. So your `this` object can do all kinds of DOM manipulations, but in most cases you'll just do something like this:

```js
this.innerHTML = 'Hello <em>world</em>!';
```

Now you have to wait with doing this, till your object has been added to the DOM, so you need to kick this off in the `connectedCallback()` like this:

```js
class EmptyHelloWorld extends HTMLElement {

  /* Constructor for WebComponents - the first line must always be super() */
  constructor() {
    super();
  }

  /* connectedCallback() is the standard callback when the component has been attached */
  connectedCallback() {
    this.innerHTML = 'Hello <em>world</em>!';
  }
}
```

## Reading and Writing Values

The 2sxc form will initialize your custom element and attach a `connector` object. This happens automatically, so you will have it once `connectedCallback()` is fired. This connector is a rich object with lots of stuff, but for your field value you need to know these bits

* `connector.data.value` gets you the current value
* `connector.data.update(newValue)` updates the form with the changed value
* `connector.data.value$` is the observable version of the value - this is great for advanced use cases, but otherwise you can stick to the simple `.value`

> [!TIP]
> Avoid calling `update(...)` if nothing changed - as it will make the form dirty, so the user will be asked if he wants to save when cancelling the dialog, even though nothing changed. 

> [!TIP]
> Check out this [tutorial example of Pickr](https://2sxc.org/dnn-tutorials/en/razor/ui211/page) to see all this in action

## Loading Custom CSS and JS Libraries

Since this is all standard JavaScript, you can do it anyhow you want. For example, to load some CSS we recommend that you simply add a `<link>` tag to your html, like this:

```html
<link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/@simonwep/pickr/dist/themes/classic.min.css"/>
```

For JavaScript you can do the same, either using a `<script>` tag or telling the browser to load the JS using DOM commands. We also provide a helper on `connector.loadScript(name, url, callback)` which does the following:

1. Check if the `name` given in the first parameter exists on the `window` object (to check if it's already loaded)
1. If not, load the script provided in the `url`
1. Watch the window object using polling to see when the item with `name` is created
1. Then trigger your callback function

> [!TIP]
> Check out the [tutorial example of Pickr](https://2sxc.org/dnn-tutorials/en/razor/ui211/page) to see all this in action

## Making your Fields Configurable

Now you have a color-picker, but each field may require a different set of preconfigured colors. Or maybe your date picker has could optionally restrict dates to weekdays. In these cases, you need configuration specific for the field. 

<iframe src="https://azing.org/2sxc/r/1_bUtjCH?embed=1" width="100%" height="400" frameborder="0" allowfullscreen style="box-shadow: 0 1px 3px rgba(60,64,67,.3), 0 4px 8px 3px rgba(60,64,67,.15)"></iframe>

## Create Your own WYSIWYG Field (WIP)

WYSIWYG fields are very hard to do right. Basically you can simply create your own using the same principles as mentioned above. But we recommend that you use the existing WYSIWYG field provided by 2sxc and just change some of the configurations. 

> [!TIP]
> By just reconfiguring the existing 2sxc WYSIWYG you will benefit from ADAM file-upload and continuous updates to the main component. 


Here's what you need to know

* The WYSIWYG field is based on [TinyMCE](https://www.tiny.cloud/) - so to make configuration changes, you'll need to understand that API pretty well.
* To change it, you need to create a wrapper component which contains the standard 2sxc-wysiwyg and give it different configurations.
* To do this, we are calling various methods on a `reconfigure` object of your wrapper - so you can override most of the defaults
* This API is WIP (work-in-progress) so we make have to make some minor breaking changes in 2020. This shouldn't stop you, but just be aware of this. 

TODO: This is still WIP, should get done within a few days...



## Read More

Basically you have what it takes. To go further: 

* review the specs WIP