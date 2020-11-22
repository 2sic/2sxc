---
uid: WebApi.Custom
---

# 2sxc Custom C# Web APIs

You can easily create custom C# WebAPIs, and then access them through the URL. What these endpoints do is completely up to you.

> [!TIP]
> **Discover this Feature**  
> Before you spend too much time reading docs, best discover this live. 
> You can find a real-life example in our [Razor-Tutorials](xref:Tut.WebApi). 


## Where to Store the API Files
All your WebAPIs are C# files saved in the special folder called `api`. The folder must be in root of your 2sxc app, and the files have to end with `...Controller.cs` (this is a convention in ASP.net).

_New in 2sxc 9.35+_: you can now also create `api` folders as _subfolders_ to run the api in multiple editions. This is an experimental [polymorph feature](xref:Specs.Cms.Polymorphism) which is still being worked out, but in general you can use the old and the new way:

* classic (single edition)  
  `[app-folder]/api/YourController.cs`  
  accessed url: `[api-root]/app/auto/api/[Your]`
* multiple editions:  
  `[app-folder]/[edition]/api/YourController.cs`
  access url: `[api-root]/app/auto/[edition]/api/[Your]`

Read more about urls in the [WebApi](xref:WebApi.Index) docs.




## Basic Example

A file in your app in the `api` folder called `BooksController.cs` could look like the following:

```c#
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

public class BooksController : ToSic.Sxc.Dnn.ApiController
{
  [HttpGet]
  [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Anonymous)]
  [ValidateAntiForgeryToken]
  public object Persons()
  {
    return new ToSic.Sxc.Conversion.DataToDictionary(Edit.Enabled)
      .Convert(App.Data["Persons"]);
  }
}
```

The custom controller **BooksController** must have the same name as the file and extends the **ApiController** controller. It has a method returning all items of the **Persons** data type. The method is decorated with several attributes:
* `[HttGet]` defines that the method must be invoked with HTTP GET
* `[DnnModuleAuthorize(AccessLevel = ...)]` defines the permission an invoker must have
* `[ValidateAntiForgeryToken]` ensures that a security token from the cookies is validated before the mehod is invoked

The custom controller can be called with JavaScript like this:

```html
<!-- this ensures the $2sxc scripts are loaded -->
@Edit.Enable(js: true)

<!-- the button which loads everything -->
<button type="button" class="btn btn-primary" onclick="getPersons(this)">
  Get Persons Custom
</button> 
<script>
  // this script does the API call and then shows the result
  function getPersons(moduleContext) {
    $2sxc(moduleContext)
      .webApi.get('app/auto/api/books/persons')
      .then(function (results) {
        alert('Found ' + results.length + ' persons. \n'
          + 'The first one is "' + results[0].FirstName + ' ' + results[0].LastName + '"\n\n'
          + 'The raw JSON: \n' + JSON.stringify(results)
        );
      });
  }
</script>
```

The $2sxc API ensures that the GET request is send to the correct url `/DesktopModules/2sxc/API/app/auto/api/Books/Persons`. You can also read more about the [sxc Controller](xref:Specs.Js.Sxc).




## Special Object / Commands in ApiController

The `ApiController` provides various command / helpers to get you productiv. Most are the same as in a normal Razor view, but some are additional. Here are the main ones:

1. AsDynamic(...)
1. AsList(...)
1. AsEntity(...)
1. [Dnn](xref:HowTo.DynamicCode.Dnn)
1. [App](xref:HowTo.DynamicCode.App) with `App.Data`, `App.Query` etc.
1. [Data](xref:HowTo.DynamicCode.Data)
1. [SaveInAdam(...)](xref:WebApi.Custom.DotNet.SaveInAdam) _new in 9.30_

## Notes

* Instead of **App.Data["MyData"]** you can fetch data from another data source provided by 2sxc (for exmple from the **App.Query["MyQueryData"]**)
* **Sxc.Serializer.Prepare(...)** converts the object returned by App.Data["MyData"].List to a dynamic and serializable object



## Recommended Reading

* [](xref:Tut.WebApi)
* [WebApi](xref:WebApi.Index)
* [Concepts: Polymorphisms](xref:Specs.Cms.Polymorphism)




## History

1. Introduced in 2sxc 06.05
1. Enhanced with Polymorph Editions in 2sxc 9.35 (allowing subfolder/api)

