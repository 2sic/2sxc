---
uid: Concepts.Views
---
# Concept: Views

Views are what the user will see - and contains things like Html, CSS, Javascript and data from the content. 

## How it Works

Views use template files inside app root folder or sub folder. As of now, there are two types:

* **Razor / MVC** - These always begin with an `_` and end with `.cshtml`. Placeholders and code usually is marked with `@` like `@Content.Name`
* **Tokens** - these always end with `.html`. Placeholders usually look like `[Content.Name]`. Tokens cannot have any server-side code aside from the basic placeholders. 

_If your view just hosts a JavaScript SPA, it will also be one of these types of files._

The template-file is just part of the view. To be used as a view, it must be configured in the App configuration as a view, where you add things like

1. **Name** or **Thumbnail Image** (for the preview when selecting the view)
1. **Data** specs like what type of data is shown, if the data comes from a query etc.
1. **View Parameters** to automatically show this view based on url-parameters

## Understanding the Configuration

A view has a lot of configuration options, but they are all explained in the edit view dialog, so we're not documenting this here. 

## Advanced Topics

* [Switching between views based on the url](https://2sxc.org/en/docs/Feature/feature/4680)
* [Differences between features when using Content or App](https://2sxc.org/en/blog/post/2sxc-app-vs-2sxc-content-which-one-should-i-use)
* [Protecting Views for certain users using permissions](https://2sxc.org/en/Docs/Feature/feature/4737)

## How to Use

todo: recipes to create, change etc.

## Future Features & Wishes

1. Out-of the box support for [polymorphism](xref:Concepts.Polymorphism)

## Read also

* [Hide advanced features from normal editors](https://2sxc.org/en/docs/Feature/feature/3592)
* [Razor Tutorial](https://2sxc.org/dnn-tutorials/en/razor)

## History

1. Introduced in 2sxc 1.0