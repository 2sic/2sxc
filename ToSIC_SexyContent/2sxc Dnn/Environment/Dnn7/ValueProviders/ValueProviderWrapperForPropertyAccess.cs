using System;
using System.Globalization;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Tokens;

using ToSic.Eav.ValueProviders;

namespace ToSic.SexyContent.Environment.Dnn7.ValueProviders
{
    public class ValueProviderWrapperForPropertyAccess: BaseValueProvider
    {
        private IPropertyAccess _source { get; set; }
        private UserInfo _user { get; set; }
        private CultureInfo _loc { get; set; }

        public ValueProviderWrapperForPropertyAccess(string name, IPropertyAccess source, UserInfo user, CultureInfo localization)
        {
            Name = name;
            _source = source;
            _user = user;
            _loc = localization;
        }

        public override string Get(string property, string format, ref bool propertyNotFound)
        {
            bool blnNotFound = true;
            string result = _source.GetProperty(property, format, _loc, _user, Scope.DefaultSettings, ref blnNotFound);
            return result;
        }

        public override bool Has(string property)
        {
            throw new NotImplementedException();
        }
    }
}