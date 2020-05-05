---
uid: Specs.Cms.Templates.Razor
---
# Razor Templates

Razor Templates will generate HTML - often based on the data a editor entered, and/or which was provided from the App.

> [!NOTE]
> The [View](xref:Specs.Cms.Views) determines which template file is being loaded. 

> [!TIP]
> Razor templates are the most powerful kind of templates, since you can program anything you want using C#.

## How it Works

Razor templates use the Asp.Net Razor Engine to generate Html. The convention uses placeholders like `@variable` to put data into the Html. 

The template files usually reside inside app root folder or sub folder. These always begin with an `_` and end with `.cshtml`. 
Placeholders and code usually is marked with `@` like `@Content.Name`

## Technical Conventions

As of 2sxc 10.20, all Razor templates should start with a line like  
`@inherits ToSic.Sxc.Dnn.RazorComponent` 
to make sure that it has all the new features. 

> [!WARNING]
> If you don't specify this first line, many features still work for compatibility reasons. 
> But they inherit from a different class, so certain features won't work exactly as documented. 

> [!TIP]
> Check out the [How-To Razor](xref:HowTo.Razor.Templates) section

> [!TIP]
> You can see the full API available to to your code out-of-the box in the [RazorComponent API](xref:ToSic.Sxc.Dnn.RazorComponent)

## Razor Code-Behind

If your Razor file is getting kind of large because of C# functions, best place it in a [Razor Code-Behind](xref:Specs.Cms.Templates.RazorCodeBehind).

## Reusing Html-Helpers in Razor 

Razor has a `@helper` syntax which allows you to create fragments and re-use them. 
Discover this in the [tutorials](https://2sxc.org/dnn-tutorials/en/razor/reuse/home).

## Shared Sub Templates

Razor templates can _include_ other razor files with more Razor code inside them, using `RenderPage(...)`. 
Discover this in the [tutorials](https://2sxc.org/dnn-tutorials/en/razor/reuse/home).

## Shared Code

Sometimes you want to share C# code which isn't meant for HTML-output. For example, a security check. You can do this using `CreateInstance(...)`. 
Discover this in the [tutorials](https://2sxc.org/dnn-tutorials/en/razor/reuse/home).


## Future Features & Wishes

1. Out-of the box support for [](xref:Specs.Cms.Polymorphism)

## Read also

* [Views](xref:Specs.Cms.Views)
* [Templates](xref:Specs.Cms.Templates)
* [RazorComponent API](xref:ToSic.Sxc.Dnn.RazorComponent)
* [Token Templates](xref:Specs.Cms.Templates.Token)
* [Razor Tutorial](https://2sxc.org/dnn-tutorials/en/razor)

## History

1. Introduced in 2sxc 1.0