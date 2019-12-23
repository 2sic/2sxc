---
uid: ToSic.Eav.DataSources.Queries
---

Query objects in the EAV contain a configuration how to wire up various Data-Sources to retrieve data. 
Users will usually construct them using the VisualQueryDesigner and the specifications of the query are then stored as Entity objects in the App. 

Most queries are then accessed either through Razor or C# code using `App.Query["queryname"]` or directly using WebApi (if permissions allow this).

### History

1. Introduced in 2sxc 06.00
1. [Api features added in 2sxc 8.10](https://2sxc.org/en/blog/post/releasing-2sxc-8-10-public-rest-api-visual-query-and-webapi-shuffle-datasource)
