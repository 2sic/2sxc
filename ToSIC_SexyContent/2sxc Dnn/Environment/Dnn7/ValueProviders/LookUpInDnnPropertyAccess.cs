using System;
using System.Globalization;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Tokens;
using ToSic.Eav.LookUp;

// ReSharper disable once CheckNamespace
namespace ToSic.SexyContent.Environment.Dnn7.ValueProviders
{
    public class LookUpInDnnPropertyAccess: LookUpBase
    {
        private readonly IPropertyAccess _source;
        private readonly UserInfo _user;
        private readonly CultureInfo _loc;

        public LookUpInDnnPropertyAccess(string name, IPropertyAccess source, UserInfo user, CultureInfo localization)
        {
            Name = name;
            _source = source;
            _user = user;
            _loc = localization;
        }

        public override string Get(string key, string format, ref bool propertyNotFound)
        {
            bool blnNotFound = true;
            string result = _source.GetProperty(key, format, _loc, _user, Scope.DefaultSettings, ref blnNotFound);
            return result;
        }

        public override bool Has(string key)
        {
            throw new NotImplementedException();
        }
    }
}