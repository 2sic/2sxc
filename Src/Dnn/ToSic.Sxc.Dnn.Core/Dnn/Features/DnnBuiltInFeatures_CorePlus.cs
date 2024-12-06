using ToSic.Eav.Internal.Features;
using ToSic.Eav.SysData;

namespace ToSic.Sxc.Dnn.Features;

partial class DnnBuiltInFeatures
{
    public static readonly Feature DnnPageWorkflow = new()
    {
        NameId = "DnnPageWorkflow",
        Guid = new("da68d954-5220-4f9c-a485-86f16b98629a"),
        Name = "Support for Dnn / Evoq Page Workflow",
        IsPublic = false,
        Ui = false,
        Description = "Enables support for Page Workflow in Evoq; on by default. It seems that in Evoq ca. v9.4+ it has become buggy. So you can disable it now. ",
        Security = FeaturesCatalogRules.Security0Neutral,
        LicenseRules = BuiltInFeatures.ForCorePlusEnabled
    };

}