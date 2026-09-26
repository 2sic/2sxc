using ToSic.Sys.HookUp;

namespace ToSic.Sxc.Web.Sys.Url;

/// <summary>
/// Interface for processing URL Values before keeping / converting to a string-url
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
internal interface IUrlParameterInGroupOperation: IOperation<UrlParameter?>;