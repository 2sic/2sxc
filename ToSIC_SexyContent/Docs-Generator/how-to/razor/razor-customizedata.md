# Event _CustomizeData()_ on the Razor Page
## Purpose / Description
This event is called by the view-engine before the rest of the script is parsed - and it's usually empty.
It can be overriden to change/configure what data is delivered to the template or search-index. 

## How to use
In your razor page (.cshtml file) you can add a script block implementing this, as follows:

```c#
@functions{

// Prepare the data - get all categories through the pipeline
public override void CustomizeData()
    {
        // new features in 6.1 - the App DataSource CreateSource<App> and also the RelationshipFilter
        // Just add the items which have the relationship to the category in the URL
        var qsOfCat = CreateSource<RelationshipFilter>(App.Data["QandA"]);
        qsOfCat.Relationship = "Categories";
        qsOfCat.Filter = "[QueryString:Category]";
        Data.In.Add("QandA", qsOfCat["Default"]);
    }
}

```
Since the code above is run before the rest of the template is executed, the `Data` object now has a 
stream called `QandA` which the rest of the template can access using `Data["QandA"]`. 

## Notes and Clarifications
In general, you can override this event to prepare data. It has a few benefits like

1. It's always called, even if the data is not templated - for example when it's streamed as JSON or when it's prepared for search indexing
2. In the future, most data-preparations will be possible through a visual designer, but for now, this is the best way to go.

## Connection to Search index
The CustomizeData event runs both when rendering the template as well as when the search is running. 
For further details you may want to read about

* [InstancePurpose][InstancePurpose] - which tells you why the current code is running so you could change the data added
* [CustomizeSearch][CustomizeSearch] - which let's you write code to alter how the data is processed in the search-index

## Demo App and further links
You should find some code examples in this demo App
* [FAQ with Categories](http://2sxc.org/en/apps/app/faq-with-categories-and-6-views)

More links: [Description of the feature on 2sxc docs](http://2sxc.org/en/Docs-Manuals/Feature/feature/2683)

## History
1. Introduced in 2sxc 6.1

[//]: # "Links referenced in this page"
[CustomizeData]:Razor-SexyContentWebPage.CustomizeData
[InstancePurpose]:Razor-SexyContentWebPage.InstancePurpose
[CustomizeSearch]:Razor-SexyContentWebPage.CustomizeSearch