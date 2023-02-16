using DotNetNuke.Entities.Users;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Raw;
using ToSic.Eav.DataSources;
using ToSic.Eav.DataSources.Queries;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Run;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Dnn.Run;

namespace ToSic.Sxc.Dnn.DataSources
{
    /// <summary>
    /// Get DNN user profiles as <see cref="IEntity"/> objects of one or many users.
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
	[VisualQuery(
        NiceName = "Dnn User Profiles",
        UiHint = "Users profiles of specified users in Dnn",
        Icon = Icons.Face,
        Type = DataSourceType.Source, 
        GlobalName = "ToSic.Sxc.Dnn.DataSources.DnnUserProfile, ToSic.Sxc.Dnn",
        ExpectsDataOfType = "|Config ToSic.SexyContent.DataSources.DnnUserProfileDataSource",
        PreviousNames = new []
        {
            "ToSic.SexyContent.Environment.Dnn7.DataSources.DnnUserProfileDataSource, ToSic.SexyContent"
        }
        )]
	public class DnnUserProfile : ExternalData
	{
        private readonly IDataBuilder _dataBuilder;

        #region Configuration-properties

        /// <summary>
        /// The user id list of users to retrieve, comma-separated
        /// </summary>
        [Configuration]
		public string UserIds
        {
            get => Configuration.GetThis();
            set => Configuration.SetThis(value);
        }

        /// <summary>
        /// List of profile-properties to retrieve, comma-separated
        /// </summary>
        [Configuration(Fallback = "DisplayName,Email,FirstName,LastName,Username")]
		public string Properties
        {
            get => Configuration.GetThis();
            set => Configuration.SetThis(value);
        }

		/// <summary>
		/// Gets or sets the Name of the ContentType to simulate
		/// </summary>
		[Configuration(Field = "ContentTypeName", Fallback = "DnnUserInfo")]
		public string ContentType
		{
			get => Configuration.GetThis();
            set => Configuration.SetThis(value);
        }

		/// <summary>
		/// Gets or sets the Name of the Title Attribute of the DNN-UserInfo
		/// </summary>
		[Configuration(Field = "TitleFieldName", Fallback = "DisplayName")]
		public string TitleField
		{
			get => Configuration.GetThis();
            set => Configuration.SetThis(value);
        }

		#endregion

        #region Constructor / DI

        public new class MyServices: MyServicesBase<DataSource.MyServices>
        {
            public ISite Site { get; }
            public IZoneMapper ZoneMapper { get; }

            public MyServices(
                DataSource.MyServices rootServices,
                ISite site,
                IZoneMapper zoneMapper
            ) : base(rootServices)
            {
                ConnectServices(
                    Site = site,
                    ZoneMapper = zoneMapper
                );
            }
        }

		public DnnUserProfile(MyServices services, IDataBuilder dataBuilder) : base(services.RootServices, "Dnn.Profile")
        {
            ConnectServices(
                _dataBuilder = dataBuilder.Configure(typeName: ContentType)
            );
            _deps = services.SetLog(Log);
            Provide(GetList);
        }

        private readonly MyServices _deps;

        #endregion

        private IImmutableList<IEntity> GetList() => Log.Func(l =>
        {
            Configuration.Parse();

            var realTenant = _deps.Site.Id != Eav.Constants.NullId ? _deps.Site : _deps.ZoneMapper.SiteOfApp(AppId);
            l.A($"realTenant {realTenant.Id}");

            var properties = Properties.Split(',').Select(p => p.Trim()).ToArray();
            var portalId = realTenant.Id;

            // read all user Profiles
            ArrayList users;
            if (!UserIds.HasValue() ||
                UserIds == "disabled") // note: 'disabled' was the default text in <v15. can probably be removed, but not sure
                users = UserController.GetUsers(portalId);
            // read user Profiles of specified UserIds
            else
            {
                var userIds = UserIds.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
                users = new ArrayList();
                foreach (var user in userIds.Select(userId => UserController.GetUserById(portalId, userId)))
                    users.Add(user);
            }
            l.A($"users: {users.Count}");

            // convert Profiles to Entities
            var results = new List<DnnUserProfileInfo>();
            foreach (UserInfo user in users)
            {
                var dnnUserProfile = new DnnUserProfileInfo
                {
                    Id = user.UserID,
                    Guid = user.UserGuid(),
                    Name = GetDnnProfileValue(user, TitleField.ToLowerInvariant())
                };

                // add Profile-Properties
                foreach (var property in properties)
                    dnnUserProfile.Properties.Add(property, GetDnnProfileValue(user, property));

                results.Add(dnnUserProfile);
            }
            l.A($"results: {results.Count}");

            return (_dataBuilder.CreateMany(results), "ok");
        });

        private static string GetDnnProfileValue(UserInfo user, string property)
        {
            string value;
            switch (property.ToLowerInvariant())
            {
                case "displayname":
                    value = user.DisplayName;
                    break;
                case "email":
                    value = user.Email;
                    break;
                case "firstname":
                    value = user.FirstName;
                    break;
                case "lastname":
                    value = user.LastName;
                    break;
                case "username":
                    value = user.Username;
                    break;
                default:
                    value = user.Profile.GetPropertyValue(property);
                    break;
            }
            return value;
        }
    }

    /// <summary>
    /// Internal class to hold all the information about the user profile,
    /// until it's converted to an IEntity in the <see cref="DnnUserProfile"/> DataSource.
    ///
    /// For detailed documentation, check the docs of the underlying objects:
    ///
    /// * TODO:
    /// * TODO:
    /// Important: this is an internal object.
    /// We're just including in in the docs to better understand where the properties come from.
    /// We'll probably move it to another namespace some day.
    /// </summary>
    /// <remarks>
    /// Make sure the property names never change, as they are critical for the created Entity.
    /// </remarks>
    [InternalApi_DoNotUse_MayChangeWithoutNotice]
    public class DnnUserProfileInfo : IRawEntity
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public string Name { get; set; } // aka DisplayName

        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public Dictionary<string, object> Properties { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Data but without Id, Guid, Created, Modified
        /// </summary>
        [PrivateApi]
        public Dictionary<string, object> RawProperties => new Dictionary<string, object>(Properties)
        {
            { Attributes.TitleNiceName, Name },
            { nameof(Name), Name },
        };
    }

}