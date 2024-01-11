namespace ToSic.Sxc.Dnn.Code;

/// <summary>
/// This interface extends the IAppAndDataHelpers with the DNN Context.
/// It's important, because if 2sxc also runs on other CMS platforms, then the Dnn Context won't be available, so it's in a separate interface.
/// </summary>
[PublicApi]
public interface IDnnDynamicCode : Sxc.Code.IDynamicCode, IHasDnn
{
}