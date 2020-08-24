---
uid: ToSic.Sxc.Web.IDynamicCode.App
---
You'll usually want to access the data, like `App.Data["Categories"]` or the queries `App.Query["AllPosts"]`.  

```cs
foreach(var cat in AsDynamic(App.Data["Categories"])) {
	@cat.Name
}
```