using ToSic.Eav.Data;

namespace ToSic.Sxc.Services.CmsService
{
    internal class CmsEditDecorator: IDecorator<IEntity>
    {
        public bool EnableEdit { get; }

        private CmsEditDecorator(bool enableEdit)
        {
            EnableEdit = enableEdit;
        }

        public static EntityDecorator12<CmsEditDecorator> Wrap(IEntity entity, bool enableEdit) =>
            new EntityDecorator12<CmsEditDecorator>(entity, new CmsEditDecorator(enableEdit));
    }
}
