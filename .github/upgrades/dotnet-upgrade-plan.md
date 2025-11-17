# .NET 10 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that an .NET 10.0 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET 10.0 upgrade.
3. Upgrade eav-server/ToSic.Sys.Core/ToSic.Sys.Core.csproj
4. Upgrade eav-server/ToSic.Sys.Utils/ToSic.Sys.Utils.csproj
5. Upgrade eav-server/ToSic.Sys.Caching/ToSic.Sys.Caching.csproj
6. Upgrade eav-server/ToSic.Sys.Features/ToSic.Sys.Features.csproj
7. Upgrade eav-server/ToSic.Sys.Security/ToSic.Sys.Security.csproj
8. Upgrade eav-server/ToSic.Sys.Code/ToSic.Sys.Code.csproj
9. Upgrade eav-server/ToSic.Eav.LookUp/ToSic.Eav.LookUp.csproj
10. Upgrade eav-server/ToSic.Eav.Data/ToSic.Eav.Data.csproj
11. Upgrade eav-server/ToSic.Eav.Data.Build/ToSic.Eav.Data.Build.csproj
12. Upgrade eav-server/ToSic.Eav.Persistence/ToSic.Eav.Persistence.csproj
13. Upgrade eav-server/ToSic.Eav.Data.Stack/ToSic.Eav.Data.Stack.csproj
14. Upgrade eav-server/ToSic.Eav.Apps/ToSic.Eav.Apps.csproj
15. Upgrade eav-server/ToSic.Eav.Context/ToSic.Eav.Context.csproj
16. Upgrade eav-server/ToSic.Eav.Apps.Persistence/ToSic.Eav.Apps.Persistence.csproj
17. Upgrade eav-server/ToSic.Eav.Persistence.Efc/ToSic.Eav.Persistence.Efc.csproj
18. Upgrade eav-server/ToSic.Eav.DataSource/ToSic.Eav.DataSource.csproj
19. Upgrade eav-server/ToSic.Sys.OData/ToSic.Sys.OData.csproj
20. Upgrade eav-server/ToSic.Eav.Repository.Efc/ToSic.Eav.Repository.Efc.csproj
21. Upgrade eav-server/ToSic.Eav.ImportExport/ToSic.Eav.ImportExport.csproj
22. Upgrade eav-server/ToSic.Eav.DataSources/ToSic.Eav.DataSources.csproj
23. Upgrade eav-server/ToSic.Eav.Work/ToSic.Eav.Work.csproj
24. Upgrade Sxc/ToSic.Sxc.Core/ToSic.Sxc.Core.csproj
25. Upgrade eav-server/ToSic.Eav.Insights/ToSic.Eav.Insights.csproj
26. Upgrade Sxc/ToSic.Sxc.Data/ToSic.Sxc.Data.csproj
27. Upgrade eav-server/ToSic.Eav.WebApi/ToSic.Eav.WebApi.csproj
28. Upgrade Sxc/ToSic.Sxc.Adam/ToSic.Sxc.Adam.csproj
29. Upgrade Sxc/ToSic.Sxc.Apps/ToSic.Sxc.Apps.csproj
30. Upgrade Sxc/ToSic.Sxc.Cms/ToSic.Sxc.Cms.csproj
31. Upgrade Sxc/ToSic.Sxc.Blocks/ToSic.Sxc.Blocks.csproj
32. Upgrade Sxc/ToSic.Sxc.Edit/ToSic.Sxc.Edit.csproj
33. Upgrade Sxc/ToSic.Sxc.Engines/ToSic.Sxc.Engines.csproj
34. Upgrade Sxc/ToSic.Sxc.Images/ToSic.Sxc.Images.csproj
35. Upgrade Sxc/ToSic.Sxc.Render/ToSic.Sxc.Render.csproj
36. Upgrade Sxc/ToSic.Sxc.Services/ToSic.Sxc.Services.csproj
37. Upgrade Sxc/ToSic.Sxc.Web/ToSic.Sxc.Web.csproj
38. Upgrade Sxc/ToSic.Sxc.Code.HotBuild/ToSic.Sxc.Code.HotBuild.csproj
39. Upgrade Sxc/ToSic.Sxc.LightSpeed/ToSic.Sxc.LightSpeed.csproj
40. Upgrade Sxc/ToSic.Sxc.Code.Generate/ToSic.Sxc.Code.Generate.csproj
41. Upgrade Sxc/ToSic.Sxc.Code/ToSic.Sxc.Code.csproj
42. Upgrade Sxc/ToSic.Sxc.WebApi/ToSic.Sxc.WebApi.csproj
43. Upgrade Sxc/ToSic.Sxc.Custom/ToSic.Sxc.Custom.csproj
44. Upgrade Sxc/ToSic.Sxc.Core.TestHelpers/ToSic.Sxc.Core.TestHelpers.csproj
45. Upgrade Razor/ToSic.Sxc.Razor/ToSic.Sxc.Razor.csproj
46. Upgrade Oqtane/ToSic.Sxc.Oqt.Shared/ToSic.Sxc.Oqt.Shared.csproj
47. Upgrade Sxc/ToSic.Sxc.Various.SystemTests/ToSic.Sxc.Various.SystemTests.csproj
48. Upgrade Oqtane/ToSic.Sxc.Oqt.Server/ToSic.Sxc.Oqt.Server.csproj
49. Upgrade Oqtane/ToSic.Sxc.Oqt.Client/ToSic.Sxc.Oqt.Client.csproj
50. Upgrade SharedImports/Shared Imports (never build this).csproj
51. Upgrade Sxc/ToSic.Sxc.WebApi.Tests/ToSic.Sxc.WebApi.Tests.csproj
52. Upgrade Sxc/ToSic.Sxc.Various.UnitTests/ToSic.Sxc.Various.UnitTests.csproj
53. Upgrade Build/ToSic.Sxc.BuildTasks/ToSic.Sxc.BuildTasks.csproj
54. Upgrade Integration/SxcEdit01/SxcEdit01.csproj
55. Upgrade Integration/BasicEav01/BasicEav01.csproj
56. Upgrade Mvc/Website/Website.csproj

## Settings

This section contains settings and data used by execution steps.

### Excluded projects

Table below contains projects that do belong to the dependency graph for selected projects and should not be included in the upgrade.

| Project name                                   | Description                 |
|:-----------------------------------------------|:---------------------------:|
| Src/Dnn/*                                      | Explicitly excluded         |
| eav-server projects targeting only net472      | Explicitly excluded         |

### Aggregate NuGet packages modifications across all projects

NuGet packages used across all selected projects or their dependencies that need version update in projects that reference them.

| Package Name                         | Current Version           | New Version                     | Description                               |
|:-------------------------------------|:-------------------------:|:-------------------------------:|:------------------------------------------|
| Microsoft.AspNetCore.Mvc.NewtonsoftJson | 9.0.0;9.0.0               | 10.0.0                          | Recommended for .NET 10                    |
| Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation | 9.0.0;9.0.5            | 10.0.0                          | Recommended for .NET 10                    |
| Microsoft.EntityFrameworkCore        | 2.1.1;9.0.0               | 10.0.0                          | Recommended for .NET 10                    |
| Microsoft.EntityFrameworkCore.Design | 2.1.1                     | 10.0.0                          | Recommended for .NET 10                    |
| Microsoft.EntityFrameworkCore.Relational | 2.1.1                   | 10.0.0                          | Recommended for .NET 10                    |
| Microsoft.EntityFrameworkCore.SqlServer | 2.1.1                   | 10.0.0                          | Recommended for .NET 10                    |
| Microsoft.EntityFrameworkCore.Tools  | 2.1.1                     | 10.0.0                          | Recommended for .NET 10                    |
| Microsoft.Extensions.DependencyInjection | 9.0.0;2.1.1            | 10.0.0                          | Recommended for .NET 10                    |
| Microsoft.Extensions.DependencyInjection.Abstractions | 9.0.5;2.1.1         | 10.0.0                          | Recommended for .NET 10                    |
| Microsoft.VisualStudio.Web.CodeGeneration.Design | 9.0.0                  | 10.0.0-rc.1.25458.5             | Recommended for .NET 10                    |
| System.Collections.Immutable         | 9.0.0                     | 10.0.0                          | Recommended for .NET 10                    |
| System.ComponentModel.Annotations    | 5.0.0                     | (remove - via framework)        | Included with framework in .NET 10         |
| System.Data.DataSetExtensions        | 4.5.0                     | (remove - via framework)        | Included with framework in .NET 10         |
| System.Runtime.InteropServices.RuntimeInformation | 4.3.0                 | (remove - via framework)        | Included with framework in .NET 10         |
| System.Text.Json                     | 9.0.0;9.0.5               | 10.0.0                          | Recommended for .NET 10                    |

### Project upgrade details

#### eav-server/ToSic.Sys.Core/ToSic.Sys.Core.csproj

Project properties changes:
  - Target frameworks should be changed from `net472;net9.0` to `net472;net10.0`

NuGet packages changes:
  - Microsoft.Extensions.DependencyInjection for net9.0 should be updated from `9.0.0` to `10.0.0` (recommended for .NET 10)
  - System.Collections.Immutable should be updated from `9.0.0` to `10.0.0` (recommended for .NET 10)
  - System.Text.Json should be updated from `9.0.0` to `10.0.0` (recommended for .NET 10)

Other changes:
  - Keep net472 item group unchanged.

#### Oqtane/ToSic.Sxc.Oqt.Shared/ToSic.Sxc.Oqt.Shared.csproj

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

NuGet packages changes:
  - System.ComponentModel.Annotations should be removed (functionality included in framework)
  - Microsoft.AspNetCore.Components stays at `10.0.0` (compatible)
  - Oqtane.Shared stays at `10.0.0` (compatible)

Other changes:
  - None

... (Project-specific details for all projects listed above analogous to the analysis results, excluding DNN-only projects)
