﻿using ToSic.Eav.Apps;
using ToSic.Eav.Logging;
using System.Linq;
using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Edit.Toolbar
{
    public class ToolbarButtonDecoratorHelper: HasLog
    {

        public ToolbarButtonDecoratorHelper(IAppStates appStates): base($"{Constants.SxcLogName}.TbdHlp")
        {
            _appStates = appStates;
        }
        private readonly IAppStates _appStates;

        public ToolbarButtonDecorator GetDecorator(IAppIdentity appIdentity, string typeName, string command)
        {
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