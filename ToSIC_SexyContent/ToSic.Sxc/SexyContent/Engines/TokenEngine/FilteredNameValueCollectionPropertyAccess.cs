using System.Collections.Specialized;
using ToSic.Eav.ValueProvider;
using ToSic.Eav.ValueProviders;

namespace ToSic.SexyContent.Engines.TokenEngine
{
    /// <inheritdoc />
    /// <summary>
    /// Copied from Form and List Module
    /// </summary>
    internal class FilteredNameValueCollectionPropertyAccess : BaseValueProvider
    {
	    readonly NameValueCollection _nameValueCollection;
        public FilteredNameValueCollectionPropertyAccess(string name, NameValueCollection list)
        {
            Name = name;
            _nameValueCollection = list;
        }
        

        public override string Get(string property, string format, ref bool propertyNotFound) 
            => _nameValueCollection == null 
            ? string.Empty 
            : FormatString(_nameValueCollection[property], format);

        public override bool Has(string property) => throw new System.NotImplementedException();
    }
}