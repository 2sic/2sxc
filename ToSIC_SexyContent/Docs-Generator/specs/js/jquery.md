---
uid: Specs.Js.JQuery
---
# JQuery in DNN and 2sxc

Often you'll need and want jQuery, but when you don't it shouldn't be loaded for performance reasons. 

> [!TIP]
> Removing jQuery and jQueryUI will boost your mobile PageSpeed like crazy. 
> So only include it on pages where you really need it. 

## How DNN Auto-Loades jQuery

There is some history to this which we'll explain briefly. DNN made jQuery a first-class citizen around DNN 4, and since then most of the UI was jQuery based. For a while there even was a standard that buttons etc. should be built and styled with jQuery UI. That is not the case any more. You can now easily run DNN (at least in browsing mode, not editing) without jQuery. 

But because _jQuery was always there_ developers never noticed that they could leave it away, and many parts like Templates would simply rely on them. There were also many things that automatically added jQuery but were never noticed. Here some important examples:

1. 2sxc always used the ServicesFramework of DNN which internally auto-added jQuery
1. Most DNN websites use popups for login, and just doing this automatically adds jQueryUI and jQuery to the page. You can easily stop this using the recipe [Remove jQueryUI from my page](https://azing.org/dnn-community/r/fjgSyTfI)

In case your code is running on a page without jQuery but you need it, your code should tell DNN that you want jQuery, like this:

<iframe src="https://azing.org/dnn-community/r/YqJFbNKH?embed=1" width="100%" height="400" frameborder="0" allowfullscreen style="box-shadow: 0 1px 3px rgba(60,64,67,.3), 0 4px 8px 3px rgba(60,64,67,.15)"></iframe>

> [!WARNING]
> Don't manually add urls to the jQuery files, make sure you use the official API. This should help you prevent loading jQuery multiple times and avoid conflicts between jQuery version.

> [!TIP]
> If you do need a newer version of jQuery, that is possible but needs some tweaking to get them to run side-by-side. It's done using the [jQuery.noConflict()](https://api.jquery.com/jQuery.noConflict/).

## How 2sxc loads jQuery in DNN

* Up until 2sxc 10.24, every 2sxc module automatically loaded jQuery because 2sxc used the ServicesFramework of DNN
* Starting from 2sxc 10.25 
  * old templates auto-load jQuery for backward compatibility. This includes token-templates and Razor templates which don't have an `@inherits` statement at the beginning. 
  * Anything new done using the [RazorComponent](xref:HowTo.Razor.Templates) will not do that unless your template code requests it.

In 2sxc 10.25 and newer, all core features of 2sxc _don't_ need jQuery. So anonyomus browsing of your site won't require jQuery at all, even if you're doing API calls or using the [$2sxc javascript API](xref:Specs.Js.$2sxc). 

But what you do need (if you're using the new [RazorComponent](xref:HowTo.Razor.Templates)) is to tell 2sxc that you plan to use JavaScript and APIs, so that 2sxc can add the stuff to the page to make the magic happen. This is done with this line in your razor code:

```
@Edit.Enable(js:true)
```

Because 2sxc doesn't use jQuery any more for normal stuff, this will have the following effect:

1. Add a special header to the page containing information needed for API calls
1. Load the 2sxc.api.min.js in the correct way

_It's important to note that this will not load jQuery._





