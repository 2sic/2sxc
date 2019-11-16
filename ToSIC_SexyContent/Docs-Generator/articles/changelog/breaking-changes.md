---
uid: Articles.ChangeLog.BreakingChanges
---

# Breaking Changes in EAV and 2sxc

We try to minimize breaking changes, and most breaking changes won't affect your work, because it's internal API. 
We're documenting it here to ensure you know what happened, in case you still run into this.

## Version 10

> Version 10 has a lot of small breaking changes because 
> we restructured the internal API so it's consistent when we publish it. 
> All these things shouldn't affect you, because they were internal APIs, 
> but in case it does - here's what we did.

#### Version 10.20.02 (ca. 2019-11-22)

More internal changes which shouldn't affect anybody, but make the API ready for public docs...

1. Moved/renamed the internal `Eav.AppDataPackage` to `Eav.Apps.AppState`
1. Moved/renamed some internal interfaces like `Entity...`
1. Did a major change for how `Attribute<T>` for relationships work.  
	Before they were `Attribute<EntityRelationship>` and now they are `Attribute<IEnumerable<IEntity>>`.  
	This also affects `Value<EntityRelationship>` which is now `Value<IEnumerable<IEntity>>`
1. Moved `Tenant<T>` and `Container<T>` including matching interfaces to `Eav.Environment`
1. Renamed `IAppIdentity` to `IInAppAndZone` and `IZoneIdentity` to `IInZone`
1. Renamed `ICacheKeyProvider` to `ICacheKey`
1. Renamed `CacheChainedIEnumerable<T>` to `SynchronizedList<T>`
1. Moved/Renamed `MetadataFor` to `Eav.Metadata.Target`. Left old name compatible.
1. Moved some extension methods for IEntity from `ToSic.Eav.Data.Query` to `ToSic.Eav.Data`
1. Changed `Permissions` to be strongly typed EntityBased objects

#### Version 10.20.01 (2019-11-12)

1. Internal code now uses the term `Header` instead of `ListContent`. External code provides both for backward-compatibility
1. moved internal interfaces for engines (Razor/Token) to final namespaces `ToSic.Sxc.Engines`
	1. `IEngine`
	1. `EngineBase`
	1. `ITokenEngine`
	1. `IRazorEngine`
1. corrected architecture - some template-management code had slipped into `Eav.Apps`, was moved back to `Sxc.Apps`
1. The `Template` object was moved from `Eav.Apps` to `Sxc.Views` and we added an interface `IView`. 
	We also renamed the internal properti `ViewNameInUrl` to `UrlIdentifier`. 
1. To correct the API a CmsManager was created extending the AppManager, which is in charge of Views
1. Moving internal stuff related to content blocks
	1. `IContentBlock` from `SexyContent.Interfaces` to `Sxc.Blocks`
	1. from `ToSic.SexyContent.ISxcInstance` to `ToSic.Sxc.Blocks.IBlockContext`
	1. actually moved a lot of things there incl. `ContentBlock` now `BlockConfiguration` and more - all internal stuff
1. Moving the `ToSic.SexyContent.App` to `ToSic.Sxc.Apps.App`
1. In a razor page, we added the preferred `Purpose`. The old `InstancePurpose` will still work
1. Placed some things we just moved in 10.20 to a final place - since it's a very recent change, we updated the docs in the 10.20.00 section

#### Changed, but completely internal

1. Some namespaces on `SexyContent.ContentBlocks` were moved to `Sxc.Blocks`


#### Version 10.20.00 (2019-11-05)

1. the internal interface `IInPageEditingHelpers` was moved from `ToSic.SexyContent.Interfaces` to the namespace `ToSic.Sxc.Web`
1. the internal interface `ILinkHelper` was moved to `ToSic.Sxc.Web`
1. the internal interface `IHtmlHelper` was moved to `ToSic.Sxc.Dnn`
1. the property `Configuration` on dynamic entities was deprecated in 2sxc 4 and removed in 2sxc 10 - we don't think it was ever used
1. moved internal Metadata interfaces (ca. 5) into final namespace @ToSic.Eav.Metadata
1. Moved a bunch of internal interfaces which we believe were never used externally from `ToSic.Eav.Interfaces` to `ToSic.Eav.Data`
	1. `ToSic.Eav.Data.IAttribute`
	1. `ToSic.Eav.IAttribute<T>`
	1. `IAttributeBase`
	1. `IAttributeDefinition`
	1. `IChildEntities` 
	1. `IContentType`
	1. `IDimension`
	1. `IEntityLight`
	1. `ILanguage`
	1. `IRelationshipManager`
	1. `IValue`
	1. `IValue<T>`
	1. `IValueOfDimension<T>`
1. Moved a bunch of internal interfaces which we believe were never used externally from `ToSic.Eav.Apps.Interfaces` to `ToSic.Eav.Apps`
	1. `IApp`
	1. `IAppData`
	1. `IAppDataConfiguration`
	1. `IAppEnvironment`
	1. `IEnvironmentFactory`
	1. `IInstanceInfo`
	1. `IItemListAction`
	1. `IPagePublishing`
	1. `ITenant`
	1. `IZoneMapper`
1. the internal namespace `ToSic.Eav.ValueProvider` was changed to `ToSic.Eav.LookUp` and inside it  
	we renamed a bunch of internal interfaces and objects which we believe were never used externally

##### Deprecated/Changed, but not broken

1. the internal interface `ToSic.SexyContent.IAppAndDataHelpers` was renamed to `ToSic.Sxc.IDynamicCode` but the old interface still exists, so it shouldn't break  
	_it was used by Mobius Forms_
1. moved `ToSic.Eav.Interfaces.IEntity` to `ToSic.Eav.Data.IEntity` - but preserved the old interface for compatibility
	_it was used everywhere_

##### Clean-Up, but not broken

1. We're transitioning to the term `Header` instead of `ListContent` in templates.  
	The Razor pages and WebApi have this starting now, while old terms still work. 
	Note that we're _not_ creating a `HeaderPresentation`, because you should use `Header.Presentation`

## Version 9

#### Version 9.20.00 (2018-03-04)

1. Minor breaking change in ADAM properties, like `Id` instead of `FolderID` which was a leftover of DNN naming.  
	see full [blog post](https://2sxc.org/en/blog/post/working-with-the-breaking-change-adam-objects-2sxc-9-20)

#### Version 09.08.00 (2017-11-28)

1. Minor breaking change `List<IEntity>` instead of `Dictionary<int, IEntity>` on the `IDataSource`  
	see full [blog post](https://2sxc.org/en/blog/post/fixing-the-breaking-change-in-2sxc-9-8-list-instead-of-dictionary)

#### Version 09.03.00 (2017-10-08)

1. Breaking change on inconsistent naming `ToSic.Eav.IEntity` instead of `ToSic.Eav.Interfaces.IEntity`.  
	see full [blog post](https://2sxc.org/en/blog/post/fixing-the-breaking-change-on-tosic-eav-ientity-in-2sxc-9-3)