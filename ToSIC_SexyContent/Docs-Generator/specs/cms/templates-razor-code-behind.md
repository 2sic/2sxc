---
uid: Specs.Cms.Templates.RazorCodeBehind
---
# Razor Templates - Code-Behind

2sxc 11 introduces a new way to split out most of the C# code from the main template Razor file. We call this code-behind. Best watch the video to get the idea. 

<iframe width="560" height="315" src="https://www.youtube.com/embed/wIa23gy26js" frameborder="0" allow="accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>

## How Code Behind Works

Take any existing `_something.cshtml` and create a new `_something.code.cshtml`. Put something simple in it like 

```cshtml
@inherits ToSic.Sxc.Dnn.RazorComponentCode

@functions {
  public string Hello() {
    return "Hello from inner code";
  }
}

@helper ShowDiv(string message) {
  <div>@message</div>
}

@helper AppName() {
  <div>App Name is: @App.Name</div>
}
```

This is automatically compiled for you and provided to the `_something.cshtml` on the object `Code` so you can write

```cshtml
@Code.ShowDiv("This was created in code-behind")
<span>
  @Code.Hello()
</span>
@{
  var msg = @Code.Hello() + " my friend";
}
```

## Put Customize Search and Customize Data in Code Behind

> [!TIP]
> The Code-Behind can also be the place where you put [CustomizeData](xref:HowTo.Razor.CustomizeData) and [CustomizeSearch](xref:HowTo.Razor.CustomizeSearch).

## Read Also

* [How to Organize Code](xref:HowTo.Razor.OrganizeCode)

## History

1. Introduced in 2sxc 11.0