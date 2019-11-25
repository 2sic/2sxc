---
uid: Specs.Js.$2sxc.Cms
---
# JS: The $2sxc.cms API

## Purpose / Description

The `$2sxc.cms` object is the core JavaScript API to perform CMS actions such as opening edit-dialogs etc. As of now (2sxc 9.30) it only has 1 command `run(...)` but will be enhanced in the future to do more.

You need this in advanced use cases. _otherwise you don't need this_. Such advanced cases are:

1. when you create custom JS buttons to start a content-management action

## How to use

Before you start, ensure you have the necessary JS scripts loaded:

1. in edit-mode this happens automatically
2. if you want to provide this to low-priviledge users, use `@Edit.Enable(...)` in [razor](xref:HowTo.Razor.Edit.Enable)

Simple example:

```html
@* enable the editing *@
@Edit.Enable(api: true, forms: true, context: true, autoToolbar: false)

<script>
    // simple function to run the command and handle the returned promise
    function addProject(tag) {
        $2sxc.cms.run(tag, "new", { contentType: "Project"})
            .then(function () {
                alert("Thanks - we'll review your entry and publish it.")
            });
    }
</script>

<span onclick='window.addProject(this)'>
    add your project
</span>

```

1. the first parameter is an HTML tag in the DOM, which is used to look up the context automatically (see [edit-context](xref:Concepts.EditContext))
1. the second parameter is the verb for the [cms-command](xref:Specs.Js.Commands) to run
1. the third parameter is additional parameters for that command


## Demo App and further links

* [Tutorial app for Public Forms](https://2sxc.org/en/apps/app/tutorial-public-forms-with-2sxc-9-30)
* [Blog Recipe for using Public Forms with 2sxc](https://2sxc.org/en/blog/post/recipe-create-public-forms-with-2sxc)


## History

1. Introduced in 2sxc 09.30
