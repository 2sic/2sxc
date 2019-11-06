---
uid: Articles.ChangeLog.BreakingChanges
---

# Breaking Changes in EAV and 2sxc

We try to minimize breaking changes, and most breaking changes won't affect your work, because it's internal API. 
We're documenting it here to ensure you know what happened, in case you still run into this.

## Version 10

Version 10 has a lot of small breaking changes because we restructured the internal API so it's consistent when we publish it. 
All these things shouldn't affect you, because they were internal APIs, but in case it does - here's what we did.

#### Version 10.10

1. the internal interface `IAppAndDataHelpers` was renamed to `IDynamicCode`
1. the internal interface `IInPageEditingHelpers` was moved from `ToSic.SexyContent.Interfaces` to the namespace `ToSic.Sxc.Interfaces`
1. the internal interface `IHtmlHelper` was moved to `ToSic.Sxc.Dnn`
1. the interface `ToSic.Sxc.Adam.IFile` was moved to `ToSic.Eav.Apps.Assets`
1. the internal namespace `ToSic.Eav.ValueProvider` was changed to `ToSic.Eav.ValueProviders` (added an 's' for consistency)