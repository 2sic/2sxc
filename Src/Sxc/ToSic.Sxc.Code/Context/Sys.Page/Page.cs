namespace ToSic.Sxc.Context.Sys.Page;

/// <summary>
/// A page state object, very minimal.
/// </summary>
/// <remarks>
/// Currently only returned by DNN, where we split factory from data (v22).
/// Oqtane not updated yet.
/// </remarks>
/// <param name="Id"></param>
/// <param name="Url"></param>
/// <param name="Parameters"></param>
public record Page(int Id, string Url, IParameters Parameters) : IPage;