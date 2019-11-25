---
uid: HowTo.WebApi
---
# ASP.net WebAPI in 2sxc

You can create your own custom WebAPI in 2sxc to allow external systems to interact with your data.
The setup builds upon the ASP.net and DNN WebAPI. It enhances the bootstrapping and provides you the syntactical 2sxc goodies in C# known from Razor.

## Where to Store the API Files
All your WebAPIs are C# files saved in the special folder called `api`. The folder must be in root of your 2sxc app, and the files have to end with `...Controller.cs` (this is a convention in ASP.net).

_New in 2sxc 9.35+_: you can now also create `api` folders as _subfolders_ to run the api in multiple editions. This is an experimental [polymorph feature](xref:Specs.Cms.Polymorphism) which is still being worked out, but in general you can use the old and the new way:

* classic (single edition)  
  `[app-folder]/api/YourController.cs`  
  accessed url: `[api-root]/app/auto/api/[Your]`
* multiple editions:  
  `[app-folder]/[edition]/api/YourController.cs`
  access url: `[api-root]/app/auto/[edition]/api/[Your]`

Read more about urls in the [WebApi](xref:HowTo.WebApis) docs.

## How to use
A file named **DemoController.cs** could look like the following:

```c#
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.SexyContent.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

public class DemoController : SxcApiController
{
      [HttpGet]
      [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Anonymous)]
      [ValidateAntiForgeryToken]
      public object Get()
      {
            return new
            {
                  Data = Sxc.Serializer.Prepare(App.Data["MyData"].List)
            };
      }
}
```

The custom controller **DemoController** must have the same name as the file and extends the **SxcApiController** controller. It has a method returning all items of the **MyData** data type. The method is decorated with several attributes:
* [HttGet] defines that the method must be invoked with HTTP GET
* [DnnModuleAuthorize(AccessLevel = ...)] defines the permission an invoker must have
* [ValidateAntiForgeryToken] ensures that a security token from the cookies is validated before the mehod is invoked
You can implement any other methods.

The custom controller can be called with JavaScript and the 2sxc4ng API like this:

```JavaScript
return $http.get("app-api/Demo/Get").then(function (result) {
      return results.data.Data;
});
```

The 2sxc4ng API ensures that the GET request is send to the correct url /DesktopModules/2sxc/API/app-api/Demo/Get. You can also read more about the jQuery [sxc Controller](xref:Specs.Js.Sxc) to use 2sxc-WebApis from jQuery pages.

## Special Object / Commands in SxcApiController

The `SxcApiController` provides various command / helpers to get you productiv. Most are the same as in a normal Razor view, but some are additional. Here are the main ones:

1. AsDynamic(...)
1. AsEntity(...)
1. [Dnn](xref:HowTo.DynamicCode.Dnn)
1. [App](xref:HowTo.DynamicCode.App) with `App.Data`, `App.Query` etc.
1. [Data](xref:HowTo.DynamicCode.Data)
1. [SaveInAdam(...)](xref:HowTo.WebApi.SaveInAdam) _new in 9.30_
## Notes
* Instead of **App.Data["MyData"]** you can fetch data from another data source provided by 2sxc (for exmple from the **App.Query["MyQueryData"]**)
* **Sxc.Serializer.Prepare(...)** converts the object returned by App.Data["MyData"].List to a dynamic and serializable object

## Read also

* [WebApi](xref:HowTo.WebApis)
* [Concepts: Polymorphisms](xref:Specs.Cms.Polymorphism)

## History

1. Introduced in 2sxc 06.05
1. Enhanced with Polymorph Editions in 2sxc 9.35 (allowing subfolder/api)

