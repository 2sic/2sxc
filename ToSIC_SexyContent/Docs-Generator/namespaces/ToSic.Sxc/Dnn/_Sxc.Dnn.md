---
uid: ToSic.Sxc.Dnn
summary: *content
---

This contains interfaces that are specific to 2sxc in DNN. 

The purpose is that both the EAV and 2sxc are meant to be platform agnostic, but Razor and WebApi developers in DNN still need access to some helpers like the [DNN object](xref:ToSic.Sxc.Dnn.IDnnContext). 

> [!NOTE]
> All the things starting with `Dnn...` are DNN specific implementations of EAV or 2sxc features. 
> We've documented them so you know how things work, but you usually won't care about them.

### The Classes You Normally Care About

1. [](xref:ToSic.Sxc.Dnn.RazorComponent) which is the recommended base class for razor templates
1. [](xref:ToSic.Sxc.Dnn.ApiController) which is the recommended base class for WebApi Controllers
1. [](xref:ToSic.Sxc.Dnn.Factory) which helps you get 2sxc stuff from outside of 2sxc (like from WebForms)