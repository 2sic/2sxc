using System.Reflection;
using static ToSic.Sxc.Sys.SxcSharedAssemblyInfo;

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


// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Sys;


/// <summary>
/// Contains information for all assemblies to use
/// </summary>
//[ShowApiWhenReleased(ShowApiMode.Never)]
internal static class SxcSharedAssemblyInfo
{
    public const string AssemblyVersion = "20.00.05";
    public const string Company = "2sic internet solutions GmbH, Switzerland";
    public const string SxcProduct = "2sxc CMS- and Meta-Module for Dnn and Oqtane";
    public const string SxcCopyright = "Copyright MIT © 2sic 2025";
}