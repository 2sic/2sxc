---
uid: ToSic.Eav.LookUp
---

Sometimes objects need to get values from the context - like...

* a URL Parameter
* the current date/time 
* an App Setting or Resource 

etc. This is done through [](xref:ToSic.Eav.LookUp).ILookUp objects. 

In many cases, we need to look up a few - like when we have a configuration made with _Tokens_. 
These would look like `[App:Path]` or `[QueryString:Ui]`. 
The tool which takes a list of these and looks all of them up is the [](xref:ToSic.Eav.LookUp).ILookupEngine. 

> [!TIP]
> Read more about this in [](xref:Specs.LookUp.Intro)