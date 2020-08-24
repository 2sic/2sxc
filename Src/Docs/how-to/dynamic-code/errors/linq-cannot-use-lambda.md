---
uid: HowTo.DynamicCode.Errors.LinqLambda
---

# Error Cannot use a lambda expression 

If you see an error like this: 

```
Cannot use a lambda expression as an argument to a dynamically dispatched operation without first casting it to a delegate or expression tree type at System.Web.Compilation.AssemblyBuilder.Compile() at System.Web.Compilation.BuildProvidersCompiler.PerformBuild() at System.Web.Compilation.BuildManager.CompileWebFile(VirtualPath virtualPath) at System.Web.Compilation.BuildManager.GetVPathBuildResultInternal(VirtualPath virtualPath, Boolean noBuild, Boolean allowCrossApp, Boolean allowBuildInPrecompile, Boolean throwIfNotFound, Boolean ensureIsUpToDate) at System.Web.Compilation.BuildManager.GetVPathBuildResultWithNoAssert(HttpContext context, VirtualPath virtualPath, Boolean noBuild, Boolean allowCrossApp, Boolean allowBuildInPrecompile, Boolean throwIfNotFound, Boolean ensureIsUpToDate) at System.Web.Compilation.BuildManager.GetVirtualPathObjectFactory(VirtualPath virtualPath, HttpContext context, Boolean allowCrossApp, Boolean throwIfNotFound) at System.Web.Compilation.BuildManager.GetCompiledType(VirtualPath virtualPath) at ToSic.Sxc.Engines.RazorEngine.CreateWebPageInstance() in
```

It usually means that you tried to write LINQ code like a `.First(...)` or `.Select(...)` on an object, and the compiler can't be sure that you tried to write LINQ.

## Background: Dynamic Code and Extension Methods

Razor is dynamically compiled code, and many objects like `Content` are typed as `dynamic`. Because of this, the compiler can't be sure what's in a `dynamic` object, and also not what is in a `Content.Tags` - since this too is regarded as `dynamic`. 

This is why you can't just write `Content.Tags.First()`, because `.First()` is an extension method which the compiler must find first - but it can't do that, since it doesn't know that `Content.Tags` are of the type `IEnumerable<...>`. 

## Solution #1 - use AsList(...)

2sxc 10.20 introduces `AsList(...)` which the compiler knows is an IEnumerable. Unfortunately if the the compiler isn't sure about `Content.Tags`, then it's also not sure about `AsList(Content.Tags)`. This is a minor inconvenience, since `AsList(...)` would figure things out, but Razor wants to be sure. So to use `AsList()` for solving this problem, you'll need to write `AsList(Content.Tags as object)`. That solves it. 

## Solution #2 - cast as `IEnumerable<dynamic>`

If you already know it's a list, you can also cast it as an `IEnumerable<dynamic>`. Since `IEnumerable<T>` is in the namespace `System.Collections.Generic` you have 3 options: 

#### Cast with full Namespace

This is what the compiler actually understands - but it's a bit long and hard to read:

```c#
var authors = (book.Authors as System.Collections.Generic.IEnumerable<dynamic>)
    .Select(a => a.FirstName + " " + a.LastName);
```

#### Cast with `@using` and `IEnumerable<dynamic>`

This is the same thing, just nicer to read:

```c#
@using System.Collections.Generic;

var authors = (book.Authors as IEnumerable<dynamic>)
    .Select(a => a.FirstName + " " + a.LastName);
```

#### Cast with `@using Dynlist = ...`

This is the same thing, but the nicest, easiest to read method:

```c#
@using Dynlist = System.Collections.Generic.IEnumerable<dynamic>;
var authors = (book.Authors as Dynlist)
    .Select(a => a.FirstName + " " + a.LastName);
```