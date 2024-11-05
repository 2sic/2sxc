using System;
using System.Text;
using ToSic.Lib.Coding;
using ToSic.Lib.DI;
using ToSic.Razor.Blade;
using ToSic.Sxc.Oqt.Shared.Interfaces;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Oqt.Server.Services;

internal class OqtTurnOnService(LazySvc<IHtmlTagsService> htmlTagsService) : TurnOnService(htmlTagsService), IOqtTurnOnService
{
    protected override string TagName => base.TagName + GenerateRandomHtmlTag();

    private static readonly char[] Characters = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
    private static readonly Random Random = new();

    private static string GenerateRandomHtmlTag()
    {
        var tagLength = Random.Next(7, 14); // Length of the tag name between 7 and 14 characters
        var tagBuilder = new StringBuilder(tagLength);

        for (var i = 0; i < tagLength; i++)
            tagBuilder.Append(Characters[Random.Next(Characters.Length)]);

        return tagBuilder.ToString();
    }

    public string Run(object runOrSpecs, NoParamOrder noParamOrder = default, object require = null, object data = null,
        IEnumerable<object> args = default, string addContext = default) =>
        base.Run(runOrSpecs, noParamOrder, require, data, args, addContext).ToString();
}