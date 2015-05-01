using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Tokens;
using ToSic.Eav.ValueProvider;

namespace ToSic.SexyContent.Engines.TokenEngine
{
    public class ValueProviderWrapperForPropertyAccess: BaseValueProvider
    {
        private IPropertyAccess _source { get; set; }

        public ValueProviderWrapperForPropertyAccess(string name, IPropertyAccess source)
        {
            Name = name;
            _source = source;
        }

        public override string Get(string property, string format, ref bool propertyNotFound)
        {
            bool blnNotFound = true;
            string result = _source.GetProperty(property, format, null, null, Scope.DefaultSettings, ref blnNotFound);
            return result;
        }

        public override bool Has(string property)
        {
            throw new NotImplementedException();
        }
    }
}