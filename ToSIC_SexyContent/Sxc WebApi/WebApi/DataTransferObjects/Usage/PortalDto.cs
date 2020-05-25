using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToSic.Sxc.WebApi.DataTransferObjects.Usage
{
    public class PortalDto
    {
        public int Id;

        public PortalDto(int portalId)
        {
            Id = portalId;
        }
    }
}
