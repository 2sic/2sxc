using System;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.DataSources
{
    public sealed partial class CmsBlock
    {
        #region obsolete stuff
        [Obsolete("became obsolete in 2sxc 9.9, use InstanceId instead")]
        [PrivateApi]
        public int? ModuleId
        {
            get => InstanceId;
            set => InstanceId = value;
        }
        #endregion

    }
}
