---
uid: ToSic.Eav.DataSources.App
---

# Data Source: App

## Purpose / Description
The **App** [DataSource](xref:Specs.DataSources.DataSource) is part of the [Standard EAV Data Sources](xref:Specs.DataSources.ListAll). It provides all items of the current App or of another App if needed. 

## How to use with the Visual Query
When using the [Visual Query](xref:Temp.VisualQuery) you can just drag it into your query. Now you can create out-connections with the names of the types you need - which looks like this:

<img src="/assets/data-sources/app-out-2-in-0.png" width="100%">

 There are three common use cases: 

### 1. Using App With The Current App
You can either just use it without any _In_ stream, then it will just deliver the published items. This is because without an _In_, the **App** will automatically build an In providing published only. 

If you do provide any kind of in, it will use that as the source. So if you provide a Publishing-Source on the in, which will cause the **App DataSource** to differ the result based on the user who is looking at it. So editors would see unpublished as well: 

<img src="/assets/data-sources/app-compare-no-in-with-publishing-filter.png" width="100%">

### 2. Using App with Other App
The App-DataSource can also be configured to deliver data from _another_ app. For this, to configure and set the ZoneId and AppId:

<img src="/assets/data-sources/app-from-other-app.png" width="100%">

You can also deliver data from different Apps by using multiple App sources: 

<img src="/assets/data-sources/app-multiple-apps.png" width="100%">

## Programming With The App DataSource

[!include["simpler-with-vqd"](shared-use-vqd.md)]

Important: to access data of the current App, please use the `App.Data` as it's a pre-build object with the same streams. For example, use `App.Data["BlogPost"]` to get all the BlogPost items. 

An example code 

```razor
@{
  var blog = CreateSource<ToSic.Eav.DataSources.App>();
  blog.ZoneSwitch = 2; // go to Zone 2
  blog.AppSwitch = 403; // go to App 403
}
@foreach(var post in AsDynamic(blog["BlogPost"]))
{
  <div>@post.EntityTitle</div>
}
```

The previous example creates an App source to the zone 2, app 403 and retrieves all items of type  `BolgPost` to show in a loop. 

### Important When Coding
Note that data sources only retrieve data once, and then ignore any further configuration. So you must set Zone/App before accessing the data. 

[!include["Read-Also-Section"](shared-read-also.md)]
* [Razor examples using App.Data](Razor-App)

## Demo App and further links
You should find some examples in this demo App
* [Demo App with examples for most DataSources](https://github.com/2sic/app-demo-visual-query/releases/latest)


## History

1. Introduced in EAV 3.x, in ca. 2sxc 6.x
