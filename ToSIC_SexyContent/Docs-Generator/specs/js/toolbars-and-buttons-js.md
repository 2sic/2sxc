---
uid: Specs.Js.Toolbar.Js
---
# JS: Toolbars and Buttons

## Purpose / Description
When a user is logged on and has edit permissions, he should see buttons to edit his content or perform other actions. This is all done in HTML / JavaScript.

## How to use

The most common use-case is actually to provide some HTML, which the JavaScript will pick up automatically and convert into a menu. You can read more about this in [Html Toolbars and Buttons](xref:Specs.Js.Toolbar.Intro). 

You can also generate the html as needed - for example when working with a javascript template in AngularJS, React, Ember etc. Here's a small example:

```javascript
var btnHtml = $2sxc(740).manage.getButton({
    action: "new",
    contentType: "BlogPost"
});

var toolbarHtml = $2sxc(740).manage.getToolbar({
    action: "new,edit,add",
    entityId: 203
});
```

So this is really it. There are only two commands you must know:

1. `...getButton(...)`
2. `...getToolbar(...)`

The rest of the magic lies in the configuration objects which you pass into these buttons. So let's continue with that.

## A Deeper Look Inside the System

The following terms help you understand what we're doing:

1. 2sxc has many [commands](xref:Specs.Js.Commands) like `new`, `edit` etc. which you can **run** with parameters like:  
`command = { action: 'new', contentType: 'BlogPost'};`
3. a **button** will run such a command when clicked, but for it to work, the button must have the command ready, including the necessary parameters. In JavaScript a button is defined like:   
`btn = { command: { action: "...", ...}, icon: "...", ... };`  
and will later be converted to HTML like  
`<a onclick='$2sxc(this).manage.run({"action": "new", "contentType": "Dummy"})'>new</a>`
4. a **button group** is an array of buttons, plus some shared specs like  
`group = { buttons: [ { command: ...}, { }, { }], defaults: {...}, ... };`  
this will later be converted to a list of `<li>` nodes containing buttons
5. a **toolbar** contains an array of _button groups_ and again some shared specs / defaults, like    
`toolbar = { groups: [ ... ], defaults: ...};`  
which will also be converted to the list of `<li>` nodes, but shows only one group at a time until the user presses a `more` button

The full object tree of a toolbar is fairly sophisticated, and in most cases you can use shorthands which will be expanded internally before use. So you'll write

```javascript
// this is what you would normally write
var toolbar = { 
    action: "new,edit,sort", 
    contentType: "BlogPost", 
    useModuleList: true,
    sortOrder: 2 
};

// which internally expands to this:
var toolbar = {
    groups: [{
        buttons: [{
            title: "Toolbar.New",
            command: {
                action: "new",
                contentType: "BlogPost"
            },
            icon: "icon-sxc-plus",
            addCondition: true,
            // more stuff here
        },{
            title: "Toolbar.Edit",
            command: {
                action: "edit",
                useModuleList: true,
                sortOrder: 2
            },
            icon: "icon-sxc-pencil",
            addCondition: true,
            // more stuff here
        }, {
            // etc.
        }]
    }]
}
```
In most cases you just care about the shorthand. But in advanced cases where you really want to affect the behavior, you may go want to go deeper. 

## Buttons and Commands

1. [Buttons are explained in more details here](xref:Specs.Js.Toolbar.Buttons)
1. [Commands which run when a button is pressed are here](xref:Specs.Js.Commands)
1. [Custom commands which run your JS are here](xref:Specs.Js.Commands.Code)

## Button group

This is just a simple object containing an array of buttons and some more settings which are rarely used and not documented yet. 

```javascript
var group = {
    name: "...",
    buttons: "...",
    defaults: {
        // ...
    }
};

var group2 = {
    buttons: [
        {
             action: "new"
        },
        {
            action: "edit",
            icon: "icon-sxc-bomb"
        },
        "button3",
        {
            title: "hello there",
            command: {
                action: "new",
            }
        }
    ],
    defaults: {
        // todo
    }
}
```


## Toolbar and Toolbar Configuration

The toolbar can be defined very precisely, but in most cases you will opt for a short format which is expanded internally. Let's look at it:

```javascript
// very compact version
var tb1 = { 
    action: "new,edit,moveup", 
    contentType: "BlogPost", 
    entityId: 17
};

// array version 
var tb2 = [{ button1 }, { button2 }]

// very expanded edition
var tb2 = {
    groups: [
        {
            name: "group 1",
            buttons: "new,edit,more"
        },
        {
            name: "group 2",
            buttons: "moveup,movedown,more"
        }
    ],
    defaults: {
        contentType: "BlogPost",
        //...
    }
};

```

## Toolbar settings
You can customize _hover behavior_, _show behavior_ and more. Read about it in the [toolbar settings](xref:Specs.Js.Toolbar.Settings).

## Custom Buttons with Custom Commands
You can easily create custom buttons with custom icons, parameters and even custom scripts. It's not well documented yet, but it's best to just look through the tutorial app below. It's also explained more in the [Html Custom Code](xref:Specs.Js.Commands.Code) 


## Demo App and further links
You should find some code examples in this demo App

* [JS Manage / Toolbar API Tutorial App](xref:App.TutorialJsToolbars)

## History

1. Introduced in 2sxc v01.00
2. Public API since 2sxc v08.06
