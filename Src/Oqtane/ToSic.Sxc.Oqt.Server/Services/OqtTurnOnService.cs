using System;
using System.Text;
using ToSic.Lib.Coding;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Razor.Blade;
using ToSic.Sxc.Internal;
using ToSic.Sxc.Oqt.Shared.Interfaces;

namespace ToSic.Sxc.Oqt.Server.Services
{
    public class OqtTurnOnService(LazySvc<IHtmlTagsService> htmlTagsService)
        : ServiceBase($"{SxcLogging.SxcLogName}.OqtTrnOnS", connect: [htmlTagsService]), IOqtTurnOnService
    {
        private const string AttributeName = "turn-on";

        //public Attribute Attribute(
        //    object runOrSpecs,
        //    NoParamOrder noParamOrder = default,
        //    object require = default,
        //    object data = default
        //)
        //{
        //    var l = Log.Fn<Attribute>();
        //    var specs = PickOrBuildSpecs(runOrSpecs, require, data);
        //    var attr = htmlTagsService.Value.Attr(AttributeName, specs);
        //    return l.ReturnAsOk(attr);
        //}

        public string Run(
            object runOrSpecs,
            NoParamOrder noParamOrder = default,
            object require = default,
            object data = default
        )
        {
            var l = Log.Fn<string>();
            var specs = PickOrBuildSpecs(runOrSpecs, require, data);
            var tag = htmlTagsService.Value.Custom(GenerateRandomHtmlTag()).Attr(AttributeName, specs);
            return l.ReturnAsOk(tag.ToString());
        }

        private static object PickOrBuildSpecs(object runOrSpecs, object require, object data)
        {
            if (runOrSpecs is not string run) return runOrSpecs;

            if (require is null && data is null) return new { run };
            if (require is null) return new { run, data };
            if (data is null) return new { run, require };

            return new { run, require, data };
        }

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
    }
}
