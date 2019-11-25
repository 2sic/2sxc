---
uid: Specs.DataSources.ListAll
---
# All DataSource Objects in 2sxc / EAV

2sxc provides a large set of [DataSource](xref:Specs.DataSources.DataSource) objects which either get data from somewhere (SQL, CSV, ...) or modify data on the `In` and passing it to `Out`. This page will give you an overview and link you to further sources if you need to know more. 

## How to use
Many data-sources are simply used in the [Visual Query](xref:Temp.VisualQuery), and if all you want is visual-query, then this reference will give you an overview regarding what things are possible. It will usually look like this: 

<img src="/assets/data-sources/app-out-2-in-0.png" width="100%">


If on the other hand you want to program with these [DataSource](xref:Specs.DataSources.DataSource) objects, then it will usually look a bit like this: 

An example code 

```c#
// A source which can filter by Content-Type (EntityType)
var allAuthors = CreateSource<EntityTypeFilter>();
allAuthors.TypeName = "Author";

// Sort by FullName
var sortedAuthors = CreateSource<ValueSort>(allAuthors);
sortedAuthors.Attributes = "FullName";

```

The previous example creates an initial source `allAuthors` which has all data on the in, then filters to only provide those of type _Author_ to the out. This is then piped to the `sortedAuthors`, which sorts on the _Attributes_ field. 

## Understanding Data-Flow and Configuration
This is explained in the [DataSource documentation](xref:Specs.DataSources.DataSource). 

## All Public DataSources
These are all the data sources which are either provided in the default installation of 2sxc, or which are available for you to install (manually). 





<table>
  <tr>
    <td><strong>Data Source</strong></td>
    <td><strong>Purpose</strong></td>
    <td><strong>Description &amp; Details</strong></td>
  </tr>
  <tr>
    <td><a href="xref:ToSic.Eav.DataSources.App">App</a></td>
    <td>Organize</td>
    <td>
      <details>
        <summary>
          Provides each content-type on the out-stream (...)
        </summary>
        Provides each content-type on the out-stream, so that you can use <code>ds["any-type-name"]</code> as if there was a table for each type.
      </details>
  </tr>
  <tr>
    <td><a href="xref:ToSic.Eav.DataSources.AttributeFilter">AttributeFilter</a></td>
    <td>Modify</td>
    <td>
      <details>
        <summary>
          Removes properties on the entities (...)
        </summary>
          Removes properties on the entities, typically         before you stream it to JSON, so that only the fields you want are
        transmitted.
      </details>
  </tr>
  <tr>
    <td>BaseCache</td>
    <td width="84">(internal)</td>
    <td>
      <details>
        <summary>
          Base class (...)
        </summary>
          The
      base class for all system caches (QuickCache, FarmCache, etc.) <br>
      </details>
  </tr>
  <tr>
    <td>BaseDataSource</td>
    <td>(internal)</td>
    <td>
      <details>
        <summary>
          Base class (...)
        </summary>
          This is just a base class. <br>
      </details>
  </tr>
  <tr>
    <td>CacheAllStreams</td>
    <td>Caching</td>
    <td>
      <details>
        <summary>
          Cache all streams passing through (...)
        </summary>
        Every stream on this "In" are also available on the "Out" but cached a certain amount of time or till the upstream source is invalidated.
      </details>
  </tr>
  <tr>
    <td><a href="xref:ToSic.Eav.DataSources.EntityTypeFilter">ContentTypeFilter</a> EntityTypeFilter </td>
    <td>Filter</td>
    <td>
      <details>
        <summary>
          Only items of a specific content-type (...)
        </summary>
        Returns
      only items which are of a certain content-type.<br>
      Note: was previously called EntityTypeFilter.
      </details>
  </tr>
  <tr>
    <td>[CsvDataSource](xref:ToSic.Eav.DataSources.CsvDataSource)</td>
    <td>Get Data</td>
    <td>
      <details>
        <summary>
          Get data from a CSV-file (...)
        </summary>
          Lets you read a CSV file and provide the data as
      Entities for further use.<br>
      not well documented yet, see <a href="/2sic/eav-server/blob/master/ToSic.Eav.DataSources/CsvDataSource.cs">code here</a>
      </details>  </tr>
  <tr>
    <td>DataTable DataSource</td>
    <td>Get Data</td>
    <td>
      <details>
        <summary>
          Base class for coding only (...)
        </summary>
          Lets
      you convert a .net DataTable object into a DataSource. This is great for when
      you find it easier to generate a DataTable, and this will auto-provide it as
      a stream.
      </details>
  </tr>
  <tr>
    <td>Deferred Pipeline Query</td>
    <td>(internal)</td>
    <td>
      <details>
        <summary>
          Internal object to optimize performance (...)
        </summary>
          This is used internally, because it will
      auto-generate an entire query-pipeline from configuration and query it, but
      only if accessed.<br>
      <code>ToSic.Eav.DataSources.DeferredPipelineQuery</code> <br>
      <a href="/2sic/eav-server/blob/master/ToSic.Eav.DataSources/DeferredPipelineQuery.cs">Deferred Pipeline Query code</a>
      </details>
  </tr>
  <tr>
    <td>Dnn FormAndList</td>
    <td>Get Data</td>
    <td>
      <details>
        <summary>
          Use old FnL data in 2sxc (...)
        </summary>
          Will
      let you access Form-And-List aka UDT (Universal Data Table) data.<br>
      <code>ToSic.SexyContent.Environment.Dnn7.<br>
      DataSources.DnnFormAndList</code> <br>
      <a href="/2sic/2sxc/blob/master/Environment/Dnn7/DataSources/DnnFormAndList.cs">Dnn FormAndList code</a>
      </details> 
  </tr>
  <tr>
    <td>DnnSql DataSource</td>
    <td>Get Data</td>
    <td>
      <details>
        <summary>
          DNN Sql DataSource (...)
        </summary>
          SqlDataSource which only uses the DNN DB and nothing else, based on the SqlDataSource
      </details>
  </tr>
  <tr>
    <td>DnnUserProfile DataSource</td>
    <td>Get Data</td>
    <td>
      <details>
        <summary>
          Get DNN Users and profiles (...)
        </summary>
          Get DNN users and profiles to create user directories, details-pages etc.<br>
      <code>ToSic.SexyContent.Environment.Dnn7.<br>
      not well documented, check out the source <a href="/2sic/2sxc/blob/master/Environment/Dnn7/DataSources/DnnUserProfileDataSource.cs">code here</a><br>
      DataSources.DnnUserProfileDataSource</code>
      </details>
  </tr>
  <tr>
    <td>ExternalData DataSource</td>
    <td>(internal)</td>
    <td>
      <details>
        <summary>
          Base Class for external data DataSources (...)
        </summary>
          This is a base-class for all kinds of external data
      sources (like Csv…) because it provides more information related to enabling
      cachning etc.<br>
      <code>ToSic.Eav.DataSources.ExternalDataDataSource</code> <br>
      <a href="/2sic/eav-server/blob/master/ToSic.Eav.DataSources/ExternalDataDataSource.cs">ExternalData DataSource code</a>
      </details>
  </tr>
  <tr>
    <td><a href="xref:ToSic.Eav.DataSources.EntityIdFilter">ItemIdFilter</a> EntityIdFilter</td>
    <td>Filter</td>
    <td>
      <details>
        <summary>
          One or more items with an Id (...)
        </summary>
          Return only 0, 1 or more items which fit the IDs in
      the string provided.<br>
      Previously named EntityIdFilter
      </details>
  </tr>


  <tr>
    <td><a href="xref:ToSic.Eav.DataSources.ItemFilterDuplicates">ItemFilterDuplicates</a></td>
    <td>Logic</td>
    <td>
      <details>
        <summary>
          Find and remove OR retrieve duplicate items (...)
        </summary>
        Use this to remove duplicates or just find them (or both) 
      </details>
  </tr>

  <tr>
    <td>Module-Instance DataSource</td>
    <td>Get Data</td>
    <td>
      <details>
        <summary>
          Get current modules data (...)
        </summary>
          Will get the content-items assigned to a DNN-Module. This is used internally on each view, but can also be used when using module-data to configure a query.
      </details>
  </tr>
  <tr>
    <td><a href="xref:ToSic.Eav.DataSources.OwnerFilter">OwnerFilter</a></td>
    <td>Filter</td>
    <td>
      <details>
        <summary>
          Only items which are "owned" by a user (...)
        </summary>
          Returns only items which are "owned" =
      created by a specific person. Great for tools where the users have their own
      data / registrations which they can still modify.
      </details>
  </tr>
  <tr>
    <td><a href="xref:ToSic.Eav.DataSources.Paging">Paging</a></td>
    <td>Logic</td>
    <td>
      <details>
        <summary>
          Page through items (...)
        </summary>
          Returns only a specific amount of items after skiping another amount; also provides a stream telling you how many items / pages are in the stream, to let you      assemble a pager UI element
      </details>
  </tr>
  <tr>
    <td>[PassThrough](xref:ToSic.Eav.DataSources.PassThrough)</td>
    <td>(internal)</td>
    <td>
      <details>
        <summary>
          Do-Nothing DataSource (...)
        </summary>
          This data source doesn't actually do anything - it
      just lets the data on the in to the out. For (internal) and testing stuff.
      </details>
  </tr>
  <tr >
    <td ><a href="xref:ToSic.Eav.DataSources.PublishingFilter">PublishingFilter</a></td>
    <td>Filter</td>
    <td>
      <details>
        <summary>
          Filters items the current user shouldn't see (...)
        </summary>
          This is
      part of the "Unpublished-Data" concept. Since each item could be
      either published or draft, this helps you show the correct ones for the
      current user based on his edit-rights. It's automatically in the default
      pipeline, unless you explicitly don't want it. 
      </details>
  </tr>
  <tr>
    <td>QuickCache</td>
    <td >(internal)</td>
    <td >
      <details>
        <summary>
          Internal cache class (...)
        </summary>
          The quick and simple cache used by default
      internally.<br>
      <code>ToSic.Eav.DataSources.Caches.QuickCache</code> <br>
      <a href="/2sic/eav-server/blob/master/ToSic.Eav.DataSources/Caches/QuickCache.cs">QuickCache code</a>
      </details>
  </tr>
  <tr >
    <td><a href="xref:ToSic.Eav.DataSources.RelationshipFilter">RelationshipFilter</a></td>
    <td >Filter</td>
    <td>
      <details>
        <summary>
          Filter items which have a relationship (...)
        </summary>
          This helps you find items which are related to another item - like "All Books by Author Daniel Mettler"<br>
      New in 8.12: In-Stream "Fallback" which is returned if the filter didn't return any hits.
      </details>  </tr>
  <tr>
    <td><a href="xref:ToSic.Eav.DataSources.SqlDataSource">SqlDataSource</a></td>
    <td>Get Data</td>
    <td>
      <details>
        <summary>
          Get SQL data as entities (...)
        </summary>
          This lets you get data from any SQL data base. It
      also has powerful script-injection protection, so messy parameters won't hurt
      it.
      </details>
  </tr>
    <tr>
    <td><a href="xref:ToSic.Eav.DataSources.Shuffle">Shuffle</a></td>
    <td>Logic</td>
    <td>
      <details>
        <summary>
          Shuffle/randomize item order (...)
        </summary>
        This source mixes up the order of the items, typically for things like "show 3 random quotes"
      </details>
  </tr>
  <tr>
    <td><a href="xref:ToSic.Eav.DataSources.StreamFallback">StreamFallback</a></td>
    <td>Logic</td>
    <td>
      <details>
        <summary>
          Returns the first in-stream with results (...)
        </summary>
        Use this to choose from multiple in-streams which data to show. It will use all the in-streams sorted A-Z, and return the first stream which can deliver data. The remaining streams will not be queried. 
      </details>
  </tr>

  <tr>
    <td><a href="xref:ToSic.Eav.DataSources.StreamMerge">StreamMerge</a></td>
    <td>Logic</td>
    <td>
      <details>
        <summary>
          Merge all in-stream into 1 (...)
        </summary>
        Use this to merge multiple in-streams into one output stream. 
      </details>
  </tr>


  <tr >
    <td><a href="xref:ToSic.Eav.DataSources.ValueFilter">ValueFilter</a></td>
    <td>Filter</td>
    <td>
      <details>
        <summary>
          Filters by value (...)
        </summary>
        Returns all items where a specified property       matches the filter. Very powerfull, with filters like contains, between, etc.<br>
      New in 8.12: In-Stream "Fallback" which is returned if the filter didn't return any hits. 
      </details>  </tr>
  <tr>
    <td><a href="xref:ToSic.Eav.DataSources.ValueSort">ValueSort</a></td>
    <td>Sort</td>
    <td>
      <details>
        <summary>
          Sorts all items (...)
        </summary>
        Sorts the items in the stream asc/desc based on a property.
      </details>
  </tr>
  <tr>
    <td><a href="/2sic/2sxc/blob/master/SexyContent/DataSources/ViewDataSource.cs">ViewDataSource</a></td>
    <td>(internal)</td>
    <td>
      <details>
        <summary>
          Internal class for view-handling (...)
        </summary>
        This is technically just the target of a
      pipeline, which is then routed to the Razor/Token View. Basically just does a
      full pass-through. <br>
      <code>ToSic.SexyContent.DataSources.ViewDataSource</code>
      </details>
  </tr>
</table>

























## Demo App and further links

You should find some code examples in this demo App
* ...

More links: [Description of the feature on 2sxc docs](http://2sxc.org/en/Docs-Manuals/Feature/feature/2683)

## History

1. Introduced in 2sxc ??.??
2. 


