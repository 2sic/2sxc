---
uid: Specs.Data.AppState
---
# App State

The EAV caches everything in memory, to ensure that everything is super-fast and doesn't require lazy loading. This is because lazy-loading has a dangerous tendancy to ping-pong a lot of requests if the code isn't very optimized - and caching everything solves that problem. 

[!include["Before you Start"](../../shared/before-you-start-idynamicentity.md)]

Once an app is accessed by code, a sophisticated internal system loads everything into the app-state which is then cached. Everything then uses this data, and save-operations usually do a partial update of the cache. This is one of the things that makes 2sxc and the EAV so amazingly fast. 

Usually you don't care much about the app-state, since you simply use the data provided by the current context. If you want to know more, check out the @ToSic.Eav.Apps.AppState.

## History

1. Introduced in 2sxc 5.0
1. Partial Updates introduced in 2sxc 9.14
