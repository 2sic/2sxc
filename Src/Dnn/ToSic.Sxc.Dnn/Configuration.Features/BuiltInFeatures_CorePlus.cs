using System;
using ToSic.Eav.Configuration;

namespace ToSic.Sxc.Dnn.Configuration.Features
{
    public partial class BuiltInFeatures
    {
        public static readonly FeatureDefinition DnnPageWorkflowDisable = new FeatureDefinition(
            "DnnPageWorkflowDisable",
            new Guid("da68d954-5220-4f9c-a485-86f16b98629a"),
            "Disable Dnn / Evoq Page Workflow",
            false,
            false,
            "By default, Page Workflow is enabled. This probably only affects Evoq, but it seems that it is buggy. So you can disable it now. ",
            FeaturesCatalogRules.Security0Neutral,
            Eav.Configuration.BuiltInFeatures.ForCorePlusEnabled
        );

    }
}
