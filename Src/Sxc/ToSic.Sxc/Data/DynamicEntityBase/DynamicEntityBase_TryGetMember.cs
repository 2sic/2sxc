using System.Dynamic;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Data
{
    public partial class DynamicEntityBase
    {
        /// <inheritdoc />
        [PrivateApi]
        public override bool TryGetMember(GetMemberBinder binder, out object result)
            => TryGetMember(binder.Name, out result);

        [PrivateApi]
        internal bool TryGetMember(string memberName, out object result)
        {
            result = GetInternal(memberName);
            return true;
        }

    }
}
