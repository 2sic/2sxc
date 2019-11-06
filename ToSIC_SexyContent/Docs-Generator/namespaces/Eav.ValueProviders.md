---
uid: ToSic.Eav.ValueProviders
summary: *content
---

Sometimes objects need to get values from the context - like a URL Parameter, the current date/time etc. This is done through @ToSic.Eav.ValueProviders.IValueProvider objects. 

In many cases, we actually need an entire set of these, and go through them to find something. For this we have the @ToSic.Eav.ValueProviders.IValueCollectionProvider system. 
It also supports a mechanism for re-using the ValueCollection but overriding some stuff for performance enhancemests. 