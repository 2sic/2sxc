using ToSic.Eav.Code.Infos;
using static ToSic.Eav.Code.Infos.CodeInfoWarning;

namespace ToSic.Sxc.Code.Internal.CodeErrorHelp;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class DynamicCode16Warnings
{
    public static ICodeInfo AvoidSettingsResources = Warn("no-settings-resources-on-code16",
        message: "Don't use Settings or Resources - use App.Settings/App.Resources or SettingsStack / ResourcesStack");

    public static ICodeInfo NoDataMyContent = Warn("no-data-my-content-code16",
        message: "Don't use Data.MyContent - use new MyItem or MyItems");
    public static ICodeInfo NoDataMyHeader = Warn("no-data-my-header-code16",
        message: "Don't use Data.MyHeader - use new MyHeader");
    public static ICodeInfo NoDataMyData = Warn("no-data-my-data-code16",
        message: "Don't use Data.MyData - use new TODO!");


    public static ICodeInfo NoTypedModel = Warn("no-typed-model",
        message: "Don't use TypedModel - use new MyModel");

    public static ICodeInfo NoCmsContext = Warn("no-cms-context",
        message: "Don't use CmsContext - use new MyContext");
}