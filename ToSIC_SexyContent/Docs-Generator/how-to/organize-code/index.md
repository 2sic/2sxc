---
uid: HowTo.OrganizeCode.Index
---
# How To Organize your Code in Razor (DOCS: WIP)

In simple scenarios you have some Razor files containing a bit of HTML and some code. As your solution grows, you'll want to organize your work better to ensure that you can maintain it. 2sxc offers various ways to do this:

1. You can split a razor file into a Template and Code-Behind (requires v11)
1. Reuse a Partial-View Razor file with RenderTemplate()
1. You can have a shared razor file which is used as a library (v9)
1. You can have a shared .cs file as a library

## Splitting Razor Templates with Code-Behind

_This is a new feature in 2sxc 11_

Just create another file with the identical name as your template file, but with `.code.cshtml` as the extension. So you'll then have something like:

* `_List View.code.cshtml`
* `_List View.cshtml`

The _code_ file looks just like a normal razor file, but must inherit from `ToSic.Sxc.Dnn.RazorComponentCode`. Here's an example of a `_demo.code.cshtml`: 

```cs
@inherits ToSic.Sxc.Dnn.RazorComponentCode

@functions {
  public string Hello() {
    return "Hello from inner code";
  }
}

@helper ShowDiv(string message) {
  <div>@message</div>
}
```

This code is automatically available in the primary file - as an object called `Code`. Here's an example `_demo.cshtml` using the above code:

```html
@inherits ToSic.Sxc.Dnn.RazorComponent

<h1>Demo Code Use</h1>

<div @Edit.TagToolbar(Content)>
    Something in it: @Code.Hello()
</div>

@Code.ShowDiv("test helper!")
```

> [!TIP]
> _Why would you do this?_  
> The main reason is to keep template-html separate from most of the code. 
> This is common when designers like to modify the html but don't like all that programming stuff. 

Note that the code-behind also has same methods/events which are automatically called. These methods can be overriden

1. `CustomizeData()`  
  This has the same effect as overriding CustomizeData in the template file
1. `CustomizeSearch(Dictionary<string, List<ISearchItem>> searchInfos, IContainer moduleInfo,
            DateTime beginDate)`  
              This has the same effect as overriding CustomizeSearch in the template file
1. `OnRender()`  
  This lets you run code before rendering. Typically you would use it to set page headers or similar. Note that it doesn't output HTML (that's the responsibility of the main template). 
1. `OnRendered()`  
  This lets you run code after rendering. Typically also for setting page headers etc, but possibly you need to wait for values which didn't exist till you rendered. 


> [!TIP]
> _Why would you do this?_  
> These methods could always be created in the Template file, but it just doesn't feel right.
> A typical `CustomizeSearch` is very technical and feels scary to people who just want to change the look and feel. 


## Reuse a Partial View with RenderTemplate()

`RenderTemplate(...)` is a standard asp.net function to render another razor file where you need it. You usually use it to make small component razor files which might just show a button or something, and then call that file. 

* You can find some examples in the [tutorials](https://2sxc.org/dnn-tutorials/en/razor/reuse110/page)


## Share a .cshtml File as Library

When you have a **lot of components** it may be easier to create a library of `@helper` commands. This library is just a normal `.cshtml` file - usually in a folder called `shared` or something, and you can then call these snippets and helpers from all your template files. 

* See [examples in the tutorials](https://2sxc.org/dnn-tutorials/en/razor/reuse210/page)


## Share a .cs File as Library

If you:

a) need to share code with razor and Webapi
b) don't need razor specific features like `@helper`

You can create a .cs class file and share this across razor files AND WebAPI files. 

* See [examples in the tutorials](https://2sxc.org/dnn-tutorials/en/razor/reuse320/page)

> [!IMPORTANT]
> This requires 2sxc 10.01 to work. 