namespace ToSic.Sxc.Context;

/// <summary>
/// General platform information
/// 
/// 🪒 In [Dynamic Razor](xref:Custom.Hybrid.Razor14) it's found on `CmsContext.Platform`  
/// 🪒 In [Typed Razor](xref:Custom.Hybrid.RazorTyped) it's found on `MyContext.Platform`
/// </summary>
[PublicApi]
public interface ICmsPlatform
{
    /// <summary>
    /// The platform type Id from the enumerator - so stored as an int.
    /// 
    /// 🪒 Use in Dynamic Razor: `CmsContext.Platform.Type`  
    /// 🪒 Use in Typed Razor: `MyContext.Platform.Type`
    /// </summary>
    PlatformType Type { get; }

    /// <summary>
    /// A nice name ID, like "Dnn" or "Oqtane"
    /// 
    /// 🪒 Use in Dynamic Razor: `CmsContext.Platform.Name`  
    /// 🪒 Use in Typed Razor: `MyContext.Platform.Name`
    /// </summary>
    /// <remarks>
    /// Please be aware that platform names may change with time - like Dnn was once DotNetNuke
    /// So to safely ensure you are detecting the right platform you should focus on the Type attribute. 
    /// </remarks>
    string Name { get; }

    /// <summary>
    /// The platform version
    /// 
    /// 🪒 Use in Dynamic Razor: `CmsContext.Platform.Version`  
    /// 🪒 Use in Typed Razor: `MyContext.Platform.Version`
    /// </summary>
    /// <remarks>Added in v13</remarks>
    Version Version { get; }
}