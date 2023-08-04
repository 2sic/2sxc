using System;
using System.Dynamic;
using ToSic.Lib.Documentation;

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
            if (!findResult.Found && StrictGet) throw new ArgumentException(ErrStrict(binder.Name, "."));
            result = findResult.Result;
            return true;
        }

    }
}
