using System.Web;
using DotNetNuke.Services.Tokens;

namespace ToSic.SexyContent.DataSources.Tokens
{
	public class QueryStringPropertyAccess : ToSic.Eav.PropertyAccess.IPropertyAccess, DotNetNuke.Services.Tokens.IPropertyAccess
	{
		#region Properties

		public string Name { get; private set; }

		public CacheLevel Cacheability
		{
			get { return CacheLevel.notCacheable; }
		}
		#endregion

		/// <summary>
		/// Constructor
		/// </summary>
		public QueryStringPropertyAccess(string name)
		{
			Name = name;
		}

		public string GetProperty(string propertyName, string format, ref bool propertyNotFound)
		{
			var context = HttpContext.Current;

			if (context == null)
			{
				propertyNotFound = false;
				return null;
			}

			return context.Request.QueryString[propertyName.ToLower()];
		}


		public string GetProperty(string propertyName, string format, System.Globalization.CultureInfo formatProvider, DotNetNuke.Entities.Users.UserInfo accessingUser, DotNetNuke.Services.Tokens.Scope accessLevel, ref bool propertyNotFound)
		{
			return GetProperty(propertyName, format, ref propertyNotFound);
		}
	}
}