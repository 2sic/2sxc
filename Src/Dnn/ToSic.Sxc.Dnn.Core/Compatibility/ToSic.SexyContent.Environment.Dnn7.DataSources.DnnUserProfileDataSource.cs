using ToSic.Eav.Data.Build;
using ToSic.Sxc.Dnn.DataSources;

// ReSharper disable once CheckNamespace
namespace ToSic.SexyContent.Environment.Dnn7.DataSources;

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
[Obsolete("This class was moved / to ToSic.Sxc.Dnn.DataSources.DnnUserProfile, use that instead.")]
public class DnnUserProfileDataSource(DnnUserProfile.MyServices services, IDataFactory dataFactory)
    : ToSic.Sxc.Dnn.DataSources.DnnUserProfile(services, dataFactory)
{
    // Todo: leave this helper class/message in till 2sxc 09.00, then either extract into an own DLL
    // - we might also write some SQL to update existing pipelines, but it's not likely to have been used much...
    // - and otherwise im might be in razor-code, which we couldn't auto-update
}