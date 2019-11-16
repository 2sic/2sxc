# Concept: Polymorph / Editions / Open-Heart-Surgery (WIP)

<img src="./assets/concepts/polymorph-logo-wide.svg" width="100%">

## Purpose / Description
Imagine you have a running system and you want to make some changes on the live installation. During the time you work, you would always risk breaking the site, but we usually don't have the time to create a staging environment. Enter Polymorphism.

_Note that this is not a complete solution yet. We're working on this and have parts of it working, and will add functionality step-by-step._

Let's compare the perfect multi-edition (polymorph) setup to the classic solution:

<img src="assets/concepts/app-polymorph-classic-vs.png" width="100%">

## Progress
As of now (2sxc 9.35) we have achieved the first step, allowing the WebAPI controllers to be polymorph. So this is the current state of development:

<img src="assets/concepts/app-polymorph-progress.png" width="100%">

This means that:

1. Api Controllers are already fully polymorph. They can be placed in a subfolder like `[app-root]/live/api/WtfController.cs` and can be accessed using a url with the edition in the name, allowing multiple identically named controllers to be used.
1. Views are polymorph if you do the view selection manually. This means, you can place your views in a subfolder like `[app-root]/live/list.cshtml` and then have an entry-point `[app-root]/list.cshtml` which will choose which edition to use - then using `@RenderPage` to pick that edition. This is still manual, because we're not sure yet what the perfect implementation is, so we would rather wait before standardizing a bad solution.
1. Everything that is data (schemas, items, queries, settings and resources) is still one edition only. The data model is able to perform multi-edition content-management, but we're not ready yet to provide the UIs etc. for this, as it could lead to confusion, so we'll hold back on this for now.

## How to use WebApi Polymorph (2sxc 9.35+)

As of now, to use the WebApi Polymorp, this is what you would do:

1. instead of placing your `WtfController.cs` in the `[app-root]/api/` folder, you place it in a `[app-root]/live/api` folder.
1. the live, default JS would then access it using  
`[dnn-api-root]/app/auto/live/api/Wtf`
1. You can then copy this controller to `[app-root]/dev/api` and make your changes there.
1. In your JS, you would then (while testing/developing) access this edition using  
`[dnn-api-root]/app/auto/dev/api/Wtf`  
without causing problems on the live solution, as all other users are still accessing the `live` edition, while you're working on the `dev` edition.
1. Once everything works, deploy (copy) the now modified `WtfController.cs` from the `dev/api` folder to `live/api` and all users benefit from the changes.

## How to use Views Polymorph (manually)

As mentioned above, this isn't automated yet, but the vision is clear, and it works ca. like this:

1. place your real views - let's say `list.cshtml` not in the `[app-root]/list.cshtml` but in an edition folder - like `[app-root]/live/list.cshtml`.
1. In the root, also create a `[app-root]/list.cshtml`, but this will basically just render the file from the editions folder, using `@RenderPage("live/list.cshtml")`
1. Now, determine how you want to switch between editions - for example based on a cookie value or host-user. Add this logic to the `[app-root]/list.cshtml` to determine the edition you want to use, and based on this `if()...else()...` to render either the `live/list.cshtml` or `dev/list.cshtml`
1. Now you can work on the `dev/list.cshtml` without affecting live users.
1. Once it's ready, deploy (copy) the `dev/list.cshtml` to `live/list.cshtml`

## Next Development Steps

Next in line, we want to standardize a few things, like:

1. Providing a UI or something to move/deploy one edition to another, so that you can work in staging, and then use a UI or something to deploy your work to live. This should also auto-backup what was in live before, in case you want to roll back.
1. Standardize how a to automatically determine which "morph" a user is in and how to set the user to this morph, making it easier to use and removing the requirement for "switcher-views" which determine which real view to show.

For now, Data-Polymorphism is low priority, because we're not sure yet if we can "pull this off" in a way that won't confuse the users.

## Notes and Clarifications
_todo_

## Read also
[//]: # "Additional links - often within this documentation, but can also go elsewhere"

* [WebApi](webapi)
* [DotNet-WebApi](dotnet-webapi)

## Demo App and further links
_todo_

## History

1. Introduced in 2sxc 9.35 - WebApi Polymorphism

[//]: # "This is a comment - for those who have never seen this"
[//]: # "The following lines are a list of links used in this page, referenced from above"
[CustomizeData]:Razor-SexyContentWebPage.CustomizeData
[InstancePurpose]:Razor-SexyContentWebPage.InstancePurpose
[CustomizeSearch]:Razor-SexyContentWebPage.CustomizeSearch