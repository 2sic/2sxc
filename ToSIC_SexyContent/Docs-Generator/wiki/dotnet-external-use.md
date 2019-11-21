# Use 2sxc Instances or App-Data from External C# Code 
[//]: # "The title should say if it's an event/method/property, the name, + the Technology like Razor, JavaScript, jQuery"

## Purpose / Description
[//]: # "short description / purpose, 2-3 lines"
Sometimes you want to leverage 2sxc to create a solution, provide data input etc. but want to output or re-use the data in your own Module, Skin, Script or something else. This is easy to do.

## Simple Example
Imagine this was your C# code in your WebForms Code-Behind:

```c#
// the app id
var appId = 42;
 
// create a simple app object to then access data
var appSimple = ToSic.SexyContent.Environment.Dnn7.Factory.App(appId);
 
// example getting all data of content type Tag
var tags = appSimple.Data["Tag"];
 
// example accessing a query
var tagsSorted = appSimple.Query["Tags sorted"];
 
// Creating an entity
var vals = new Dictionary<string, object>();
vals.Add("Tag", "test-tag");
vals.Add("Label", "Test Tag");
 
App.Data.Create("Tag", vals);
```

## Read also
[//]: # "Additional links - often within this documentation, but can also go elsewhere"

* To dive deeper, you must check the [blog post][blog-post]


## History
[//]: # "If possible, tell when it was added or modified strongly"

1. Introduced in 2sxc 08.03

[blog-post]: http://2sxc.org/en/blog/post/using-app-data-outside-of-2sxc-in-razor-custom-webapi-skin-or-another-module-300