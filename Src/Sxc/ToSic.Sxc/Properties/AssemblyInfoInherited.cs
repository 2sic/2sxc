using System.Reflection;
using static ToSic.Sxc.Internal.SharedAssemblyInfo;

// Use the globally defined assembly version information in all projects
// This file lies in the ToSic.Eav.Core project and is used as linked in other EAV projects
// See: https://denhamcoder.net/2018/09/11/visual-studio-synchronize-a-version-number-across-multiple-assemblies/

// For this to work, the .csproj file must also have some <generate...> set to false

[assembly: AssemblyVersion(AssemblyVersion)]
[assembly: AssemblyFileVersion(AssemblyVersion)]
[assembly: AssemblyInformationalVersion(AssemblyVersion)]
[assembly: AssemblyProduct(SxcProduct)]
[assembly: AssemblyCompany(Company)]
[assembly: AssemblyCopyright(SxcCopyright)]