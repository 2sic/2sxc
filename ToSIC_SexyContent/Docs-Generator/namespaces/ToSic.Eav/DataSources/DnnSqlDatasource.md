# Data Source: DnnSqlDataSource

## Purpose / Description
The **DnnSqlDataSource** [DataSource](xref:ToSic.Eav.DataSources.IDataSource) is part of the [Standard DNN Data Sources](xref:Specs.DataSources.ListAll). It lets you use data from the DNN SQL databases as if they were entities. 

This is what it looks like:

<img src="/assets/data-sources/dnnsqldatasource-basic.png" width="100%">

## How to use 
Internally the DnnSqlDataSource is exactly the same as the [SqlDataSource](DotNet-DataSource-SqlDataSource) just with fewer options, because you cannot choose most of the settings. Please consult the [SqlDataSource documentation](DotNet-DataSource-SqlDataSource) to see how you can use it. 

## Coding

FQN: `ToSic.SexyContent.Environment.Dnn7.DataSources.DnnSqlDataSource`

## Read also
* [Source code of the DnnSqlDataSource](https://github.com/2sic/2sxc/blob/master/Environment/Dnn7/DataSources/DnnSqlDataSource.cs)
* [Source code of the SqlDataSource](//github.com/2sic/eav-server/blob/master/ToSic.Eav.DataSources/SqlDataSource.cs)
* [List of all EAV Data Sources](xref:Specs.DataSources.ListAll)

## Demo App and further links
You should find some examples in this demo App
* [Demo App with examples for most DataSources](https://github.com/2sic/app-demo-visual-query/releases/latest)


## History
[//]: # "If possible, tell when it was added or modified strongly"

1. Introduced in EAV 3.x, in 2sxc ?




