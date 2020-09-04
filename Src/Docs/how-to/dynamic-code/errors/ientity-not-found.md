---
uid: HowTo.DynamicCode.Errors.IEntityNotFound
---

# Error IEntity does not exist in the Namespace 

If you see an error like this: 

```
error CS0234: The type or namespace name 'IEntity' does not exist in the namespace 'ToSic.Eav
```

It usually means that you have code using IEntity which had to be moved to another namespace for consistency. We're sorry about the breaking change. 

## Background: IEntity was moved

To create the public documentation we had to make sure our API was consistent, and IEntity was one of the exceptions. Before 2sxc 10.20 it was in the namespace `ToSic.Eav` and later in `ToSic.Eav.Interfaces`. We standardized it now to `ToSic.Eav.Data` so the full name is `ToSic.Eav.Data.IEntity`. 

## Solution: Change the namespace

Your code probably has a `@using ToSic.Eav` or `@using ToSic.Eav.Interfaces`. You should change this to 

```
@using ToSic.Eav.Data;
```

And everything should work. 