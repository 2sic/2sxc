using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ToSic.SexyContent
{
    /// <summary>
    /// Interface used in EditContentGroupItem
    /// Allows editing of ContentGroupItems and Entities
    /// </summary>
    interface IEditContentControl
    {
        void Cancel();
        void Save();
    }
}
