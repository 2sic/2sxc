using System.Dynamic;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Data.Typed;

namespace ToSic.Sxc.Data
{
    public partial class DynamicEntityBase
    {
        /// <inheritdoc />
        [PrivateApi]
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var findResult = GetInternal(binder.Name);
            // ReSharper disable once ExplicitCallerInfoArgument
            if (!findResult.Found && StrictGet) throw TypedHelpers.ErrStrict(binder.Name, ".");
            result = findResult.Result;
            return true;
        }

    }
}
