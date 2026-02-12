using ToSic.Eav.Metadata.Sys;
using ToSic.Eav.WebApi.Sys.Helpers.Validation;

namespace ToSic.Sxc.Backend.SaveHelpers;

internal class SaveDataValidator(ILog parentLog) : ValidatorBase(parentLog, "Val.EntOk")
{
    /// <summary>
    /// Check if entity was able to deserialize, and if it has attributes.
    /// In rare cases, no-attributes are allowed, but this requires metadata decorators to allow it.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="newEntity"></param>
    /// <returns></returns>
    internal HttpExceptionAbstraction? EntityNotNullAndAttributeCountOk(int index, IEntity? newEntity)
    {
        var l = Log.Fn<HttpExceptionAbstraction?>();
        if (newEntity == null)
        {
            var preparedException = BuildExceptionIfHasIssues($"entity {index} couldn't deserialize", l);
            return l.Return(preparedException, "newEntity is null");
        }

        // New #2595 allow saving empty metadata decorator entities
        var preparedException2 = (newEntity.Attributes.Count == 0 && !newEntity.Type.Metadata.HasType(KnownDecorators.SaveEmptyDecoratorId))
            ? BuildExceptionIfHasIssues($"entity {index} doesn't have attributes (or they are invalid)", l, "EntityIsOk() done")
            : null;

        return l.Return(preparedException2, "second test");
    }

}