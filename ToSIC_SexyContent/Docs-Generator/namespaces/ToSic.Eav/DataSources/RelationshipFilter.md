# Data Source: RelationshipFilter

## Purpose / Description
The **RelationshipFilter** [DataSource](xref:ToSic.Eav.DataSources.IDataSource) is part of the [Standard EAV Data Sources][eavds]. It will return only the items which have a relationship to another item - like books having an author, or blog-posts with the tag _grunt_. 

## How to use with the Visual Query
When using the [Visual Query](xref:Temp.VisualQuery) you can just drag it into your query. This is what it usually looks like:

<img src="/assets/data-sources/relationship-filter-basic.png" width="100%">

## Using Url Parameters
You can of course also use URL parameters for both the value as well as the field: 

<img src="/assets/data-sources/relationship-filter-url.png" width="100%">

## Using the Fallback
In case none of the items match the reqiurement, then either no items are returned, or those in the fallback stream: 

<img src="/assets/data-sources/relationship-filter-fallback.png" width="100%">

You can find more fallback examples like chaining them in the [ValueFilter DataSource](DotNet-DataSource-ValueFilter)

## Separators for Multiple Criterias (2sxc 9.9+)
Until 2sxc 9.8 you could only check for 1 related item, so you could only say "give me all items which have this one author". In 2sxc 9.9 we are now able to specify multiple authors, allowing queries like "give me all items which have all these authors" or "give me all items which have any of these authors". 

This works using the separation-character, which is usually a comma `,` but could be something different (in case your items have commas in the texts you're comparing). If you don't specify a separator, none will be used and the whole `Filter` criteria is treated as one value. Here's where you set it:

<img src="/assets/data-sources/relationship-filter-separator.png" width="100%">

## All Operators (2sxc 9.9+)
Untill 2sxc 9.8, you could not specify an operator, and `contains` was the assumed operator. In 9.9 we added a lot more. To explain what each does, assume that our main stream contains items of `BlogPost` and we only want to keep the posts having certain `Tags`.

Here's the list, each is explained more below:

1. `contains` - will return all items (BlogPosts), having **all** the children (tags) specified
1. `containsany` - will return all items (BlogPosts) having **any** of the children (tags) specified
1. `not-contains` will return all items (BlogPosts) **not-having-all** of the children (tags). So it will also return those items, having some of the children.
1. `not-containsany` will return all items (BlogPosts) having **none** of the children (tags) specified.
1. `any` will return all items (BlogPosts) having **any children at all** (tags). So the filter is ignored. This is the same as count=0.
1. `not-any` will return all items (BlogPosts) having **no children** (tags).
1. `first` will return all items (BlogPosts) where the **first child** (tag) is one of the filter-options. This is for scenarios where you say the first tag is a primary-category or similar.
1. `not-first` will return  all items (BlogPosts) where the first children (tags) is not one of the filter values.
1. `count` will return all items (BlogPosts) having a **specific amount** of children (tags)
1. `not-count` will return all items (BlogPosts) **not having a specific amount** of children (tags)


## Filtering On Fields other than Title and ID (9.9+)
In 2sxc 9.9 we added the ability to specify which field you want to compare (before it was always Id or Title). Here's an example:

<img src="/assets/data-sources/relationship-filter-other-field.png" width="100%">


## Filtering by Relationship-Count (9.9+)
In 2sxc 9.9 we added the ability to filter by amount of relationships - so you could say "give me all blog-posts with exactly 2 tags":

<img src="/assets/data-sources/relationship-filter-count.png" width="100%">

Note: you can also reverse this, so instead of `count` you can use `not-count` to get all the items that don't match this requirement. 

## Filtering by Has-Any (9.9+)
In 2sxc 9.9 we added the ability to filter by 

## Limitations of the RelationshipFilter
Note that as of now (2sxc 9.9) the RelationshipFilter:

1. can only seek child-items 

## Programming With The RelationshipFilter DataSource
_Note: We recommend to use the Visual Query where possible, as it's easier to understand and is consistant for C# and JavaScript. It's also better because it separates data-retrieval from visualization._

Because of limited resources we don't have code-examples. It works, but you'll probably never need it so we don't document it. 

FQN: `ToSic.Eav.DataSources.RelationshipFilter`

## Read also

* [Source code of the RelationshipFilter](//github.com/2sic/eav-server/blob/master/ToSic.Eav.DataSources/RelationshipFilter.cs)
* [List of all EAV Data Sources][eavds]

## Demo App and further links
You should find some examples in this demo App
* [Demo App with examples for most DataSources](https://github.com/2sic/app-demo-visual-query/releases/latest)


## History
[//]: # "If possible, tell when it was added or modified strongly"

1. Introduced in EAV 4.x, 2sxc ?
1. Added AttributeOnRelationship (to compare other fields that title/id) in 2sxc 9.9
1. Added separator to enable multi-filter in 2sxc 9.9
1. Added various operators like `count`, `first`, `containsany`, `any`, `not-*` in 2sxc 9.9

[//]: # "The following lines are a list of links used in this page, referenced from above"

[eavds]: DotNet-DataSources-All
