---
uid: Specs.DataSources.Api.EnsureConfigurationIsLoaded
---
# DataSource API: EnsureConfigurationIsLoaded

## Purpose / Description
If a [DataSource](xref:Specs.DataSources.DataSource) is [configurable](xref:Specs.DataSources.Configuration), then the code must parse any [configuration tokens](xref:Specs.DataSources.ConfigurationTokens) before accessing the values. This is done with `EnsureConfigurationIsLoaded()`.

## How to use EnsureConfigurationIsLoaded
Here's a simple example of the [PublishingFilter DataSources](https://github.com/2sic/eav-server/blob/master/ToSic.Eav.DataSources/PublishingFilter.cs): 

```c#
// get the correct stream, depending on ShowDrafts
private IEnumerable<IEntity> GetList()
{
    EnsureConfigurationIsLoaded();
    Log.Add($"get incl. draft:{ShowDrafts}");
    var outStreamName = ShowDrafts 
        ? Constants.DraftsStreamName 
        : Constants.PublishedStreamName;
    return In[outStreamName].List;
}
```
This example needs ShowDrafts to be boolean (true/false), but the built-in token-template for it is `[Settings:ShowDrafts||false]`. This is why `EnsureConfigurationIsLoaded()` must be called first. 

## Advanced Use Case: Overwrite EnsureConfigurationIsLoaded
In some scenarios you may want to overwrite EnsureConfigurationIsLoaded. An example is the SqlDataSource, which has a custom implementation to protect agains Sql-Injection. You can find a good example in the [source code of the SqlDataSource](https://github.com/2sic/eav-server/blob/master/ToSic.Eav.DataSources/SqlDataSource.cs).


## Read also

* [DataSource API](xref:Specs.DataSources.Api) - DataSource API overview

## Demo Code and further links

* [demo data source code](https://github.com/2sic/2sxc-eav-tutorial-custom-datasource)
* [FnL DataSource](https://github.com/2sic/dnn-datasource-form-and-list)

## History

1. Introduced in EAV 4.x, 2sxc 07.00
