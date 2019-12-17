---
uid: ToSic.Sxc.Dnn.Run
---

Run is all about Runtime / Execution of the EAV. It contains things that describe the environment it's running in and has base material for specific implementations. 

For example, the `ITenant` and `ITenant<T>` are inherited by the `DnnTenant`. 

> [!NOTE]
> All the things starting with `Dnn...` are DNN specific implementations of EAV or 2sxc features. 
> We've documented them so you know how things work, but you usually won't care about them.
> The `DNN` prefix helps us better detect in our code when we're using DNN stuff vs. generic stuff.