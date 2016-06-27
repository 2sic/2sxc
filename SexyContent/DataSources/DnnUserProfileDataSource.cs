using System.Collections.Generic;
using System.Linq;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using ToSic.Eav;
using ToSic.Eav.DataSources;

namespace ToSic.SexyContent.DataSources
{
	[PipelineDesigner]
	public class DnnUserProfileDataSource : BaseDataSource
	{
		#region Configuration-properties
		private const string PropertiesKey = "Properties";
		private const string TitleFieldKey = "TitleField";
		private const string ContentTypeKey = "ContentType";
		private const string PropertiesDefaultKeyToken = "[Settings:Properties]";
		private const string EntityTitleDefaultKeyToken = "[Settings:TitleFieldName||DisplayName]";
		private const string ContentTypeDefaultToken = "[Settings:ContentTypeName||DnnUserInfo]";

		public IEnumerable<string> Properties
		{
			get { return Configuration[PropertiesKey].Split(',').Select(p => p.Trim()); }
			set { Configuration[PropertiesKey] = string.Join(",", value); }
		}

		/// <summary>
		/// Gets or sets the Name of the ContentType Attribute 
		/// </summary>
		public string ContentType
		{
			get { return Configuration[ContentTypeKey]; }
			set { Configuration[ContentTypeKey] = value; }
		}

		/// <summary>
		/// Gets or sets the Name of the Title Attribute of the DNN-UserInfo
		/// </summary>
		public string TitleField
		{
			get { return Configuration[TitleFieldKey]; }
			set { Configuration[TitleFieldKey] = value; }
		}

		#endregion

		public DnnUserProfileDataSource()
		{
			Out.Add(Constants.DefaultStreamName, new DataStream(this, Constants.DefaultStreamName, null, GetList));
			Configuration.Add(PropertiesKey, PropertiesDefaultKeyToken);
			Configuration.Add(ContentTypeKey, ContentTypeDefaultToken);
			Configuration.Add(TitleFieldKey, EntityTitleDefaultKeyToken);
		}

		private IEnumerable<IEntity> GetList()
		{
			EnsureConfigurationIsLoaded();

			// read user Profiles
			var portalId = PortalSettings.Current.PortalId;
			var users = UserController.GetUsers(false, false, portalId);

			// convert Profiles to Entities
			var result = new List<IEntity>();
			foreach (UserInfo user in users)
			{
				// add default Profile-Properties
				var values = new Dictionary<string, object>
				{
					{"DisplayName", user.DisplayName},
					{"Email", user.Email},
					{"FirstName", user.FirstName},
					{"LastName", user.LastName},
					{"Username", user.Username}
				};

				// add custom Profile-Properties
				foreach (var property in Properties.Where(p => !values.ContainsKey(p)))
					values.Add(property, user.Profile.GetPropertyValue(property));

				// create Entity and add to result
				var entity = new Eav.Data.Entity(user.UserID, ContentType, values, TitleField);
				result.Add(entity);
			}

			return result;
		}
	}
}