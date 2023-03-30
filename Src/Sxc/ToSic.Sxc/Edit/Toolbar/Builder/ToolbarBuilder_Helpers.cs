using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial class ToolbarBuilder
    {
        private string PrepareUi(object ui, object uiMerge = default, string uiMergePrefix = default, IEnumerable<object> tweaks = default) 
            => Utils.PrepareUi(ui, uiMerge, uiMergePrefix, _configuration?.Group, tweaks: tweaks);

        private ITweakButton RunTweaksOrErrorIfCombined(
            string noParamOrder = Eav.Parameters.Protector,
            Func<ITweakButton, ITweakButton> tweak = default,
            ITweakButton initial = default,
            object ui = default,
            object parameters = default, object prefill = default, object filter = default, [CallerMemberName] string methodName = null)
        {
            var tweaks = tweak?.Invoke(initial ?? new TweakButton());
            ErrorIfTweakCombined(tweaks, ui, parameters, prefill, filter, methodName);
            return tweaks;
        }

        private void ErrorIfTweakCombined(ITweakButton tweak, object ui, object parameters, object prefill, object filter, string methodName)
        {
            if (tweak == null) return;
            if (parameters == null && ui == null && prefill == null && filter == null) return;
            var prefix = $"You called .{methodName}(...) using the '{nameof(tweak)}:' parameter. ";
            var notAllowedUse = "This is not allowed. When you use tweak, use";
            if (ui != null)
                throw new ArgumentException(
                    $"{prefix}You also supplied a '{nameof(ui)}:'. {notAllowedUse} .{nameof(ITweakButton.Ui)}(...) or other methods.");

            if (parameters != null)
                throw new ArgumentException(
                    $"{prefix}You also supplied a '{nameof(parameters)}:'. {notAllowedUse} .{nameof(ITweakButton.Parameters)}(...) or other methods.");

            if (prefill != null)
                throw new ArgumentException(
                    $"{prefix}You also supplied a '{nameof(prefill)}:'. {notAllowedUse} .{nameof(ITweakButton.Prefill)}(...).");

            if (filter != null)
                throw new ArgumentException(
                    $"{prefix}You also supplied a '{nameof(filter)}:'. {notAllowedUse} .{nameof(ITweakButton.Filter)}(...).");
        }
    }
}
