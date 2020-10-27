using Oqtane.Models;
using Oqtane.UI;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Oqtane.Shared;

namespace ToSic.Sxc.Oqt.App
{
    public partial class Test 
    {
        public override SecurityAccessLevel SecurityAccessLevel => SecurityAccessLevel.View;

        public override string Actions => "Test";

        private JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions()
        {
            WriteIndented = true,
        };
    }
}
