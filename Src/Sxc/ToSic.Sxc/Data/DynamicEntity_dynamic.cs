using System.Dynamic;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Data
{
    // These are all the bits needed by a DynamicObject in .net
    public partial class DynamicEntity
    {
        /// <inheritdoc />
        [PrivateApi]
        public override bool TryGetMember(GetMemberBinder binder, out object result)
            => TryGetMember(binder.Name, out result);

        [PrivateApi]
        public bool TryGetMember(string memberName, out object result)
        {
            result = _getValue(memberName);
            return true;
        }

    }
}
