---
uid: ToSic.Eav.DataSources.Caching
---

DataSources need various kinds of caching mechanisms, like...

* to cache the AppState once loaded
* to cache resulting streams in a cpu intensive query

The Caching system is in charge of all this, and will also take care of clearing caches as well as updating downstream caches if an underlying source has been updated. 

### History

1. Introduced in 2sxc 04.00
