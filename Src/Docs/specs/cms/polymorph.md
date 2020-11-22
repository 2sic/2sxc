---
uid: Specs.Cms.Polymorphism
---
# Polymorphism aka Open-Heart-Surgery

<img src="/assets/concepts/polymorph-logo-wide.svg" width="100%">

> [!TIP]
> The key concept behind **Polymorphism** is having the same template and code in various editions (morphs) which are automatically used based on certain rules. 

## Polymorphism Addresses 3 Problems

### #1 Workin on Live Sites aka Open-Heart-Surgery
Imagine you have a running system and you want to make some changes on the live installation. During the time you work, you would always risk breaking the site, but we usually don't have the time to create a staging environment. 

### #2 Creating Templates that work with Multiple CSS-Frameworks

When the same design must work in various CSS-Frameworks, you actually need different templates for each - and switching between them must be automatic. 

### #3 A/B Testing

In marketing, we want to test various design with different audiences and measure what works best. 

## Polymorph Folder Structure

Let's compare the perfect multi-edition (polymorph) setup to the classic solution:

<img src="/assets/concepts/app-polymorph-classic-vs.png" width="100%">

## Progress

* WebAPI Polymorphims was introduced in 2sxc 9.35
* Manual View-Polymorphism was introduced in 2sxc 9.35
* Automatic View-Polymorphism was introduced in 2sxc 11.00
* Data Polymorphism has not been implemented yet

> [!NOTE]
> Automatic View-Polymorphism replaces the manual approach for CSS-Framework and common Open-Heart-Surgery scenarios. The manual approach is still recommended for complex polymorphism as well as A/B Testing.

## View Polymorphism

### Automatic View Polymorphism based on CSS-Framework

The system is fairly easy to understand. So if polymorphism is activated for CSS-Framework detection, here are the rules:

1. The default template file is the one configured in the view configuration
1. 2sxc will try to find file with the matching name in 2 locations using the name of css framework published by the skin in the [koi.json](https://connect-koi.net/dnn-themes)
  1. beneath the current folder
  1. in the app root folder
1. If nothing is found, the default template file is used

If a match is found, it will load that. Note that if the theme does not have a koi.json, the code used is `unk` for unknown.

Here's a checklist how start using View Polymorphism with CSS-Frameworks:

<iframe src="https://azing.org/2sxc/r/XX6t8PZu?embed=1" width="100%" height="400" frameborder="0" allowfullscreen style="box-shadow: 0 1px 3px rgba(60,64,67,.3), 0 4px 8px 3px rgba(60,64,67,.15)"></iframe>

### Automatic View Polymorphism based on SuperUser Permissions

This is meant for Open-Heart-Surgery - so you can work on templates on a live site without breaking the output for normal users. This is how it works (if you have turned it on):

1. The default template is the one configured in the view configuration
1. 2sxc will try to find a file with matching name in either the `[root]/staging` (for super users) or `[root]/live` (for normal users)
1. If it is found, 2sxc will render that template
1. Otherwise the default template is used

Here's a checklist to get started:

<iframe src="https://azing.org/2sxc/r/RF0NX0xZ?embed=1" width="100%" height="400" frameborder="0" allowfullscreen style="box-shadow: 0 1px 3px rgba(60,64,67,.3), 0 4px 8px 3px rgba(60,64,67,.15)"></iframe>

### Manual View Polymorphs

In case the automatic setup doesn't suit your needs, you can do it manually  like this:

<iframe src="https://azing.org/2sxc/r/hkzLSezS?embed=1" width="100%" height="400" frameborder="0" allowfullscreen style="box-shadow: 0 1px 3px rgba(60,64,67,.3), 0 4px 8px 3px rgba(60,64,67,.15)"></iframe>

## WebAPI Polymorphims

1. Api Controllers are already fully polymorph. They can be placed in a subfolder like `[app-root]/live/api/WtfController.cs` and can be accessed using a url with the edition in the name, allowing multiple identically named controllers to be used.
1. Views are polymorph if you do the view selection manually. This means, you can place your views in a subfolder like `[app-root]/live/list.cshtml` and then have an entry-point `[app-root]/list.cshtml` which will choose which edition to use - then using `@RenderPage` to pick that edition. This is still manual, because we're not sure yet what the perfect implementation is, so we would rather wait before standardizing a bad solution.
1. Everything that is data (schemas, items, queries, settings and resources) is still one edition only. The data model is able to perform multi-edition content-management, but we're not ready yet to provide the UIs etc. for this, as it could lead to confusion, so we'll hold back on this for now.

### How to use WebApi Polymorph

As of now, to use the WebApi Polymorp, this is what you would do:

1. instead of placing your `WtfController.cs` in the `[app-root]/api/` folder, you place it in a `[app-root]/live/api` folder.
1. the live, default JS would then access it using  
`[dnn-api-root]/app/auto/live/api/Wtf`
1. You can then copy this controller to `[app-root]/dev/api` and make your changes there.
1. In your JS, you would then (while testing/developing) access this edition using  
`[dnn-api-root]/app/auto/dev/api/Wtf`  
without causing problems on the live solution, as all other users are still accessing the `live` edition, while you're working on the `dev` edition.
1. Once everything works, deploy (copy) the now modified `WtfController.cs` from the `dev/api` folder to `live/api` and all users benefit from the changes.

## Next Development Steps

For now, Data-Polymorphism is low priority, because we're not sure yet if we can "pull this off" in a way that won't confuse the users.

## Read also

* [WebApi](xref:WebApi.Index)
* [DotNet-WebApi](xref:WebApi.Custom)
* [Checklist for Polymorphism](https://azing.org/2sxc/l/2I53UarY/polymorphism-automatically-switch-templates)
* [Blog Post around Polymorphism](https://2sxc.org/en/blog/tag/polymorph)

## Demo App and further links

* The default Content Templates use CSS-Framework Polymorphism to automatically look great in Bootstrap 3 and 4
* The Mobius Forms App uses SuperUser Polymorphism so you can develop new forms in the background without breaking functionality

## History

1. Introduced in 2sxc 9.35 - WebApi Polymorphism
1. Automatic View Polymorphism introduced in 2sxc 11.0 (css-frameworks and super-user)