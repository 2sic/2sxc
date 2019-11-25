---
uid: Specs.DataSources.Custom
---

# EAV DataSources: Create Your Custom DataSources

If you want to create your own DataSource and use it in the VisualQuery designer, this is for you

## Basic Use Case
Here's an example of a complete data-source, which just delivers 1 item with the current date:

Introtext - then code:

```c#
using System;
using System.Collections.Generic;
using ToSic.Eav.DataSources;
using ToSic.Eav.DataSources.VisualQuery;
using ToSic.Eav.Interfaces;

namespace ToSic.Tutorial.DataSource
{
    // additional info so the visual query can provide the correct buttons and infos
    [VisualQuery(
        NiceName = "DateTime-Basic",
        GlobalName = "7aee541c-7188-429f-a4bb-2663a576b19e",   // namespace or guid
        HelpLink = "https://github.com/2sic/2sxc/wiki/DotNet-DataSources-Custom"
    )]
    public class DateTimeDataSourceBasic: ExternalDataDataSource
    {
        public const string DateFieldName = "Date";

        /// <summary>
        /// Constructor to tell the system what out-streams we have
        /// </summary>
        public DateTimeDataSourceBasic()
        {
            Provide(GetList); // default out, if accessed, will deliver GetList
        }

        /// <summary>
        /// Get-List method, which will load/build the items once requested 
        /// Note that the setup is lazy-loading,
        /// ...so this code will not execute unless it's really used
        /// </summary>
        /// <returns></returns>
        private IEnumerable<IEntity> GetList()
        {
            var values = new Dictionary<string, object>
            {
                {DateFieldName, DateTime.Now}
            };
            var entity = AsEntity(values);
            return new List<IEntity> {entity};
        }
    }
}
```

## How it works
Basically what you need are

1. The @ToSic.Eav.DataSources.VisualQuery.VisualQueryAttribute attribute, so that this data-source will be shown in VisualQuery
1. The **constructor**, which tells the source what Out-streams it has, in this case it's just the Default
1. A **method** which gets the items, if ever requested

### More on the VisualQuery Attribute
This source will only become available in the UI for use, if this attribute is given. You can set many things, in this demo we only set:

* GlobalName (required) - this is used for lookup/storing a reference to this source), should be unique
* NiceName (optional) a nice label in the UI
* HelpLink (optional) a help-link

there are more properties, but these are the important ones. See @ToSic.Eav.DataSources.VisualQuery.VisualQueryAttribute

### More on the Constructor and Provide
The constructor is in charge of _wiring up_ the data-source. It should not get any data - because the data may not be needed and because configuration isn't loaded yet. 

The important command you need to know is **Provide**. This will do a few things

1. Ensure that the `Out` stream offers a stream with a name - if not specified it's going to offer the `Default` stream.
1. Wire that up to the method (in this case GetList) which will be called if the `Out`-stream is ever requested

### More on the GetList Method and AsEntity
The first thing you need to know is that it won't be called if nobody requests the `Out` stream. If the the Out is requested, this method must return a list (actually an enumerable) of Entities. The most common way to build entities is to prepare a `Dictionary<string, object>` and convert it to an Entity using `AsEntity`. 

Note that `AsEntity` has many more features, which are not in this demo to keep it simpler. 


## Demo App and further links

* @Specs.DataSources.Api.AsEntity
* [Basic DataSources for EAV and 2sxc](https://github.com/2sic/2sxc-eav-tutorial-custom-datasource)
* [Blog about this feature](https://2sxc.org/en/blog/post/tutorial-custom-datasources-for-eav-2sxc-9-13-part-1)

## History

1. Introduced in 2sxc ca. 4 but with a difficult API
1. API strongly enhanced and simplifield in 2sxc 09.13 

