---
uid: Specs.Cms.Templates
---
# Template Files

Template files will generate HTML - often based on the data a editor entered, and/or which was provided from the App.

> [!NOTE]
> As of now there are two types of template files: Token and Razor. The system is built in a way, that other templating engines could also be implemented at any time. 

## How it Works

Each [View](xref:Specs.Cms.Views) has a configuration referencing a template file. 2sxc will then decide what type it is, and run the appropriate engine. 

The template files usually reside inside app root folder or sub folder. As of now, there are two types:

* **Razor / MVC** - These always begin with an `_` and end with `.cshtml`. Placeholders and code usually is marked with `@` like `@Content.Name`
* **Tokens** - these always end with `.html`. Placeholders usually look like `[Content.Name]`. Tokens cannot have any server-side code aside from the basic placeholders. 

> [!TIP]
> If your view just hosts a JavaScript SPA, it will also be one of these types of files.

## Re-Using Templates

> [!NOTE]
> **Re-Using in many Views**  
> Sometimes you'll want to use the same template file in multiple Views. This can just be configured at the [View](xref:Specs.Cms.Views) level

> [!TIP]
> **Re-Using Templates Across Portals**  
> Instead of placing the template in the App-folder of the current portal, 
> you can also place it in a global App-folder in the `_default` portal of DNN. 
> This is great if you have the same app in many portals, and want to centralize the template.

> [!TIP]
> [Razor Templates](xref:Specs.Cms.Templates.Razor) also support re-using template parts or any C# code. 
> You can also pass parameters to these parts, which allows you to share template-code across templates. 

## Future Features & Wishes

1. Out-of the box support for [polymorphism](xref:Specs.Cms.Polymorphism)

## Read also

* [Views](xref:Specs.Cms.Views)
* [Razor Tutorial](https://2sxc.org/dnn-tutorials/en/razor)

## History

1. Introduced in 2sxc 1.0