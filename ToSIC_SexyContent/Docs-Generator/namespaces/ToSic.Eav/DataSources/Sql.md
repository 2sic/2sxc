---
uid: ToSic.Eav.DataSources.Sql
---
# Data Source: SqlDataSource

The **SqlDataSource** [DataSource](xref:Specs.DataSources.DataSource) is part of the [Standard EAV Data Sources](xref:Specs.DataSources.ListAll). It lets you use data from SQL databases as if they were entities.  

## How to use with the Visual Query
When using the [Visual Query](xref:ToSic.Eav.DataSources.Query.VisualQueryAttribute) you can just drag it into your query. The `Default` out will contain the items from the DB:

<img src="/assets/data-sources/sqldatasource-basic.png" width="100%">

We recommend that you rename it so you know what it's for: 

<img src="/assets/data-sources/sqldatasource-renamed.png" width="100%">

You can then configure your connection to the DB and Query as you need it:

<img src="/assets/data-sources/sqldatasource-config-full.png" width="100%">



## Understanding the Settings

### Title & Notes
This is just for your notes, to remind you what this is for and to put notes about anything you wanted to remember. 

<img src="/assets/data-sources/sqldatasource-config-title-notes.png" width="100%">

### Connection
There are two ways to connect to SQL databases: using a **Connection Name** (which points to a detailed connection string in the _web.config_) or using a detailed **Connection String** as you need it. We recommend to use connection names where possible. If you provide both, the connection name will be used:

<img src="/assets/data-sources/sqldatasource-config-connection.png" width="100%">

### The SQL-Query 
The **Query** section has quite a lot of options, most of which are not required but are important for this to work: 

<img src="/assets/data-sources/sqldatasource-config-query.png" width="100%">

Here's what you need to know
* Content Type
  * all entities must have a type-name - so you can just enter something here - in most cases the exact name isn't important, because you usually don't refer back to this, unless further processing will try to filter this or something
* EntityId and EntityTitle
  * when using entities in 2sxc / EAV, each entity must be able to supply a title and an ID which is an integer. This is important for internal data processing to work. 
  * For this to work with SQL, the source needs to know what data-fields to use for this. By default it will use a field called `EntityId` and `EntityTitle`, but you can also use any other field - in which case you must supply the names in the following fields. 

## Using URL Parameters in Queries
The SQL DataSource can also use queries which use URL Parameters. It's safe and automatically prevent SQL Injection. For example, you can do the following:

```sql
SELECT TOP (1000) PortalId as EntityId, HomeDirectory as EntityTitle,PortalID,ExpiryDate,
AdministratorRoleId,GUID,HomeDirectory,
CreatedOnDate,PortalGroupID
  FROM [Portals]
  Where PortalId = [QueryString:Id]
```
This will automatically use the `id` parameter from the URL to filter the result. 

## Using Another Entity As SQL Parameter
You can of course use the `In` stream to provide entities which configure the SQL. The following example has a content-type `SqlSetting` with only one item (to make the example easier to understand). The [App DataSource](xref:ToSic.Eav.DataSources.App) delivers this in the `AppSetting` stream, which also goes into the Sql as `AppSetting` and is then used as a token in the SQL:

<img src="/assets/data-sources/sqldatasource-using-entity-as-config.png" width="100%">

## Using A Value from a Module-Instance as SQL Parameter
This works like in the App-example: Provide the data from the `ModuleDataSource` as an `In` stream to the SqlDataSource, and use that to filter:

<img src="/assets/data-sources/sqldatasource-using-instance-as-config.png" width="100%">

## Programming With The SqlDataSource DataSource
[!include["simpler-with-vqd"](shared-use-vqd.md)]

An example code 

```razor
@{
  var sql = CreateSource<ToSic.Eav.DataSources.Sql>();
  sql.ConnectionString = "SiteSqlServer"; // use DNN
  sql.SelectCommand = "Select ... From"; // your sql here
}
@foreach(var post in AsDynamic(sql["Default"]))
{
  <div>@post.EntityTitle</div>
}
```

### Important When Coding
Note that data sources only retrieve data once, and then ignore any further configuration. So you must set everything before accessing the data. 

[!include["Read-Also-Section"](shared-read-also.md)]

[!include["Demo-App-Intro"](shared-demo-app.md)]

[!include["Heading-History"](shared-history.md)]

1. Introduced in EAV 3.x, in 2sxc ?


[!include["Start-APIs"](shared-api-start.md)]