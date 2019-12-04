---
uid: ToSic.Eav.Logging
---

The EAV system has a powerful internal logging system. It's the backbone to Insights.

This is where it resides - usually you don't want to know about it ;).

If you do, here a short conceptual background:

* Any object can have a property - usually called `Log` which is an [](xref:ToSic.Eav.Logging.ILog) .  
	Using this the object can call the `Log.Add(...)` to add messages. Many other commands help in various scenarios. 

* The real power comes from chaining these - because each logger can know what parent-logger it reports to.  
	This allows us to reproduce the chain of events in the original code, because you can track where  
	loggers were made, and how they relate. 

* Most objects which use the Log, implement the [](xref:ToSic.Eav.Logging.IHasLog), often by inheriting  
	[](xref:ToSic.Eav.Logging).HasLog which automates things when initializing - like the chaining of the Loggers. 
