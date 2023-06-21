using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToSic.Eav.CodeChanges;

namespace ToSic.Sxc.Code
{
    public class DynamicCode16Warnings
    {
        public static ICodeChangeInfo AvoidSettingsResources = CodeChangeInfo.Warn("no-settings-resources-on-code16",
            message: "Don't use Settings or Resources - use App.Settings/App.Resources or SettingsStack / ResourcesStack");

    }
}
