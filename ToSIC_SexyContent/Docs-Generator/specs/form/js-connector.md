---
uid: Specs.Form.JsConnector
---

# API of Connector Object in Custom Input Fields

The `connector` object provides values to your WebComponent and let's you communicate with the form. Here's the API you need:

## Background

The `connector` object is attached by the form to your custom WebComponent before the `connectedCallback()` is triggered. So you can access it using `this.connector`.

## `connector` API

* `data` - a `ConnectorData<T>` object which has various properties you need to read/write values
* `dialog` - a `ConnectorDialog<T>` object which lets you open/close details-dialogs in your component
* `field` - contains a `FieldConfig` object telling you about the field, it's configuration etc.
* `field$` - an observable with the field configuration, will emit a new `FieldConfig` whenever it changes
* `loadScript(...)` - a helper method to load additional javascript files
* `_experimental` - internal API for things which do not have a stable API yet


## `connector.data` Object

The data object is of type `ConnectorData<T>`. It has these members:

```ts
export interface ConnectorData<T> {

  /** Current value of the field */
  value: T;

  /**
   * Client updates value in the host
   * @param newValue - New value of the field from the client
   */
  update(newValue: T): void;

  /**
   * Client adds callback functions to be executed every time value changes in the host.
   * So call it to register your function which should run on change.
   *
   * Use this if you are not familier with observables.
   * @param callback - Function to be executed every time value changes in the host
   */
  onValueChange(callback: (newValue: T) => void): void;

  /**
   * Observable on field value
   * Use this if you are familiar with observables.
   */
  value$: Observable<T>;

  /**
   * Fired before form is saved.
   * It tells your control that the form is about to save, and that this is the last moment you can update the value.
   * Used in case your input doesn't always push changed values, like in WYSIWYG and other complex input fields which may buffer changes.
   */
  forceConnectorSave$: Observable<T>;
}
```


## `connector.dialog` Object

This is the API to open a new dialog or close it again. 

```ts
/**
 * Responsible for opening/closing dialogs in a control.
 */
export interface ConnectorDialog<T> {
  /**
   * Opens a dialog and shows a WebComponent inside it.
   *
   * @param {string} [componentTag] name of the WebComponent which will be loaded inside the dialog
   */
  open(componentTag?: string): void;

  /**
   * Closes the dialog
   */
  close(): void;
}
```

## `connector.field` FieldConfig Object

This gives you information about the field. For simplity in maintaining the docs, here's a copy of the type file:

```ts
export interface FieldConfig {

  /** Static name of the field */
  name: string;

  /** Ordering index of the field inside the form */
  index: number;

  /** Field label */
  label: string; // updated on language change

  /** Field placeholder text */
  placeholder: string; // never updated atm. Probably will be

  /** Input type of the field. e.g. string-default, string-dropdown, etc. */
  inputType: string;

  /** Data type of the field. e.g. String, Hyperlink, Entity, etc. */
  type: string;

  /** Tells whether the field is required */
  required: boolean; // updated on language change

  /** Tells whether the field is disabled. This is the initial value that was set in settings for this field */
  disabled: boolean;

  settings: FieldSettings;
}
```

## `connector.field` and `connector.field$` FieldConfig Object

The `connector.field` object tells you how the field is configured. Note that if you use this object directly, you won't be notified of changes. For that you should use the `connector.field$` stream.

```ts
export interface FieldConfig {

  /** Static name of the field */
  name: string;

  /** Ordering index of the field inside the form */
  index: number;

  /** Field label */
  label: string; // updated on language change

  /** Field placeholder text */
  placeholder: string; // never updated atm. Probably will be

  /** Input type of the field. e.g. string-default, string-dropdown, etc. */
  inputType: string;

  /** Data type of the field. e.g. String, Hyperlink, Entity, etc. */
  type: string;

  /** Tells whether the field is required */
  required: boolean; // updated on language change

  /** Tells whether the field is disabled. This is the initial value that was set in settings for this field */
  disabled: boolean;

  /** 
   * Settings of the field, as configured in the UI 
   * This is just a normal dictionary-object with keys having the same names as the fields in the configuration dialog. 
   * Note that most keys are PascalCase, not camelCase.
   */
  settings: FieldSettings;
}
```

## `connector.loadScript()` Method

```ts
  /**
   * Load a script into the browser - but only once. 
   * Makes sure that script with the same source is loaded only once and executes callback.
   *
   * @param {string} globalObject - name on window.xxx which is checked if the js is already loaded
   * @param {string} src - path to the script
   * @param {(...args: any[]) => any} callback - your callback function
   * @memberof Connector
   */
  loadScript(globalObject: string, src: string, callback: (...args: any[]) => any): void;
```

## Read Also

* [How To Create Custom Input Fields](xref:HowTo.Customize.InputFields)
* [Tutorials for Custom Input Fields](https://2sxc.org/dnn-tutorials/en/razor/ui/home)


## History

1. Introduced in 2sxc 11.02