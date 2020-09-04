---
uid: ToSic.Sxc.Web.IDynamicCode.Data
---
You'll usually want to access the "Default" stream like `Data["Default"]`.  

```cs
foreach(var person in AsDynamic(Data["Default"])) {
	@person.Name
}
```