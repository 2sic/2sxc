---
uid: ToSic.Sxc.Dnn.RazorComponent
---

To use this, create `cshtml` files like `_person-list.cshtml` in your app-folder. 
_By default, they will be typed the old way - which continues to work for compatibility._ 
We recommend to use this from now on. To do it, your code file must begin with an `@inherits` statement, like this:

```cshtml
@inherits ToSic.Sxc.Dnn.RazorComponent

<h1>hello from RazorComponent</h1>

```