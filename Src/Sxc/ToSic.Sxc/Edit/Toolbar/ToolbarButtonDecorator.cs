﻿using ToSic.Eav.Data;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Serialization;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Edit.Toolbar
{
    /// <summary>
    /// This is a decorator for Content-Types, which allows special configuration of buttons etc. on that type
    /// </summary>
    /// <remarks>
    /// WIP new in v14; as of now only used on NoteDecorator, should have more soon
    /// </remarks>
    public class ToolbarButtonDecorator : EntityBasedType
    {
        public static string TypeName = "ToolbarButtonDecorator";
        public static string TypeNameId = "acc185a7-f300-4468-bce8-d6a64038989d";

        public static string FieldCommand = "Command";
        public static string FieldUi = "Ui";
        public static string FieldUiIcon = "UiIcon";
        public static string FieldUiColor = "UiColor";
        public static string FieldUiData = "UiData";

        public static string KeyColor = "color";
        public static string KeyIcon = "icon";
        public static string KeyIconSvgPrefix = "svg:";


        public ToolbarButtonDecorator(IEntity entity) : base(entity)
        {
        }

        public string Command => Get(FieldCommand, "");

        public bool UiData => Get(FieldUiData, false);

        public string Ui => Get(FieldUi, "");

        public string UiIcon => Get(FieldUiIcon, "");

        public string UiIconSvg
        {
            get
            {
                var icon = UiIcon;
                if (!icon.HasValue()) return icon;
                return $"{KeyIcon}={KeyIconSvgPrefix}{Base64.Encode(icon)}";
            }
        }

        public string UiColor=> Get(FieldUiColor, "").Trim('#');


        public string AllRules()
        {
            var rules = Ui;
            var svg = UiIconSvg;
            if (svg.HasValue()) rules = UrlParts.ConnectParameters(rules, svg);
            var color = UiColor;
            if (color.HasValue()) rules = UrlParts.ConnectParameters(rules, $"{KeyColor}={color}");
            return rules;
        }

    }
}