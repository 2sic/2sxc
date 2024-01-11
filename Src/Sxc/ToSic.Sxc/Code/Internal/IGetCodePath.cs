namespace ToSic.Sxc.Code.Internal;

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IGetCodePath
{
    /// <summary>
    /// Location of the current code. This is important when trying to create instances for
    /// other code in relative folders - as this is usually not known. 
    /// </summary>
    /// <returns>The real path to the currently executed code - important for dynamically compiled code like WebApis</returns>
    string CreateInstancePath { get; set; }

}