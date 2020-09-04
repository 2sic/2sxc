using System.IO;
using ToSic.Eav.Documentation;
using ToSic.Eav.Run;
using ToSic.Sxc.Mvc.TestStuff;

namespace ToSic.Sxc.Mvc.Run
{
    /// <summary>
    /// This is a Mvc implementation of a Tenant-object. 
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("this is just fyi")]
    public class MvcTenant : Tenant<MvcPortalSettings>
    {
        public MvcTenant() : base(new MvcPortalSettings()) { }

        public override ITenant Init(int tenantId)
        {
            UnwrappedContents = new MvcPortalSettings(tenantId);
            return this;
        }

        /// <inheritdoc />
        public override string DefaultLanguage => UnwrappedContents.DefaultLanguage;

        /// <inheritdoc />
        public override int Id => UnwrappedContents.Id;

        public override string Url => "todo-mvc-tenant-should-be-host-name";

        /// <inheritdoc />
        public override string Name => UnwrappedContents.Name;

        [PrivateApi]
        public override string AppsRoot => Path.Combine(UnwrappedContents.HomePath, Settings.AppsRootFolder);

        [PrivateApi]
        public override bool RefactorUserIsAdmin => false;

        /// <inheritdoc />
        public override string ContentPath => UnwrappedContents.HomePath;

        public override int ZoneId => UnwrappedContents.Id;

        public MvcTenant(MvcPortalSettings settings) : base(settings ?? new MvcPortalSettings()) { }
    }
}
