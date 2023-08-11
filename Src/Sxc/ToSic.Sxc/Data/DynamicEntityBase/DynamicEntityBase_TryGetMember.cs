using System.Dynamic;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Data.Typed;

namespace ToSic.Sxc.Data
{
    public partial class DynamicEntityBase
    {
        #region TryGetMember for dynamic access

        [PrivateApi]
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var findResult = Helper.GetInternal(binder.Name, lookupLink: true);
            // ReSharper disable once ExplicitCallerInfoArgument
            if (!findResult.Found && Helper.StrictGet)
                throw TypedHelpers.ErrStrict(binder.Name, cName: ".");
            result = findResult.Result;
            return true;
        }

        #endregion

    }
}
