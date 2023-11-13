using ToSic.Eav.Apps;
using System.Linq;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Edit.Toolbar
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public class ToolbarButtonDecoratorHelper: ServiceBase
    {

        public ToolbarButtonDecoratorHelper(IAppStates appStates): base($"{Constants.SxcLogName}.TbdHlp")
        {
            _appStates = appStates;
        }
        private readonly IAppStates _appStates;

        public IAppIdentity MainAppIdentity { get; set; }

        public ToolbarButtonDecorator GetDecorator(IAppIdentity appIdentity, string typeName, string command)
        {
            // If no special context was given, use the main one from the current context
            appIdentity = appIdentity ?? MainAppIdentity;

            if (appIdentity == null || !typeName.HasValue() || !command.HasValue()) return null;

            var appState = appIdentity as AppState ?? _appStates.Get(appIdentity);

            var type = appState?.GetContentType(typeName);
            if (type == null) return null;

            var md = type.Metadata
                .OfType(ToolbarButtonDecorator.TypeName)
                .ToList();

            return md
                    .Select(m => new ToolbarButtonDecorator(m))
                    .FirstOrDefault(d => d.Command.EqualsInsensitive(command));

        }
    }
}
