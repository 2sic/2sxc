using DotNetNuke.Entities.Users;
using System.Collections;
using System.Collections.Immutable;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.Raw;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.Internal;
using ToSic.Eav.DataSource.VisualQuery;
using ToSic.Eav.Integration;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Services;
using ToSic.Sxc.Dnn.Run;

namespace ToSic.Sxc.Dnn.DataSources;

/// <summary>
/// Get DNN user profiles as <see cref="IEntity"/> objects of one or many users.
/// </summary>
[PublicApi]
[VisualQuery(
    NiceName = "Dnn User Profiles",
    UiHint = "Users profiles of specified users in Dnn",
    Icon = DataSourceIcons.Face,
    Type = DataSourceType.Source, 
    NameId = "ToSic.Sxc.Dnn.DataSources.DnnUserProfile, ToSic.Sxc.Dnn",
    ConfigurationType = "|Config ToSic.SexyContent.DataSources.DnnUserProfileDataSource",
    NameIds =
    [
        "ToSic.SexyContent.Environment.Dnn7.DataSources.DnnUserProfileDataSource, ToSic.SexyContent"
    ]
)]
public class DnnUserProfile : CustomDataSourceAdvanced
{
    private readonly IDataFactory _dataFactory;

    #region Configuration-properties

    /// <summary>
    /// The user id list of users to retrieve, comma-separated
    /// </summary>
    [Configuration]
    public string UserIds
    {
        get => _userIds ?? Configuration.GetThis();
        set => _userIds = value;
    }
    private string _userIds;

    /// <summary>
    /// List of profile-properties to retrieve, comma-separated
    /// </summary>
    [Configuration(Fallback = "DisplayName,Email,FirstName,LastName,Username")]
    public string Properties
    {
        get => _properties ?? Configuration.GetThis();
        set => _properties = value;
    }
    private string _properties;

    /// <summary>
    /// Gets or sets the Name of the ContentType to simulate
    /// </summary>
    [Configuration(Field = "ContentTypeName", Fallback = DnnUserProfileDataRaw.TypeName)]
    public string ContentType
    {
        get => _contentType ?? Configuration.GetThis();
        set => _contentType = value;
    }
    private string _contentType;

    /// <summary>
    /// Gets or sets the Name of the Title Attribute of the DNN-UserInfo
    /// </summary>
    [Configuration(Field = "TitleFieldName", Fallback = "DisplayName")]
    public string TitleField
    {
        get => _titleField ?? Configuration.GetThis();
        set => _titleField = value;
    }
    private string _titleField;

    #endregion

    #region Constructor / DI

    public new class MyServices(CustomDataSourceAdvanced.MyServices parentServices, ISite site, IZoneMapper zoneMapper, LazySvc<DnnSecurity> dnnSecurity)
        : MyServicesBase<CustomDataSourceAdvanced.MyServices>(parentServices, connect: [site, zoneMapper, dnnSecurity])
    {
        public ISite Site { get; } = site;
        public IZoneMapper ZoneMapper { get; } = zoneMapper;
        public LazySvc<DnnSecurity> DnnSecurity { get; } = dnnSecurity;
    }

    public DnnUserProfile(MyServices services, IDataFactory dataFactory) : base(services, "Dnn.Profile")
    {
        ConnectLogs([
            _dataFactory = dataFactory
        ]);
        _services = services;
        ProvideOut(GetList);
    }

    private readonly MyServices _services;

    #endregion

    private IImmutableList<IEntity> GetList()
    {
        var l = Log.Fn<IImmutableList<IEntity>>();
        Configuration.Parse();

        var realTenant = _services.Site.Id != Eav.Constants.NullId
            ? _services.Site
            : _services.ZoneMapper.SiteOfApp(AppId);
        l.A($"realTenant {realTenant.Id}");

        var properties = Properties.CsvToArrayWithoutEmpty();
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
            users = [];
            foreach (var user in userIds.Select(userId => UserController.GetUserById(portalId, userId)))
                users.Add(user);
        }
        l.A($"users: {users.Count}");

        // convert Profiles to Entities
        var results = new List<DnnUserProfileDataRaw>();
        foreach (UserInfo user in users)
        {
            var dnnUserProfile = new DnnUserProfileDataRaw
            {
                Id = user.UserID,
                Guid = _services.DnnSecurity.Value.UserGuid(user),
                Name = GetDnnProfileValue(user, TitleField.ToLowerInvariant())
            };

            // add Profile-Properties
            foreach (var property in properties)
                dnnUserProfile.Properties.Add(property, GetDnnProfileValue(user, property));

            results.Add(dnnUserProfile);
        }
        l.A($"results: {results.Count}");
        var userProfileDataFactory = _dataFactory.New(options: new(DnnUserProfileDataRaw.Options, typeName: ContentType?.NullIfNoValue()));
        return l.Return(userProfileDataFactory.Create(results), "ok");
    }

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
public class DnnUserProfileDataRaw : IRawEntity
{
    internal const string TypeName = "UserProfile";

    internal static DataFactoryOptions Options = new(typeName: TypeName, titleField: nameof(Name));
    public int Id { get; set; }
    public Guid Guid { get; set; }
    public string Name { get; set; } // aka DisplayName

    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }

    public Dictionary<string, object> Properties { get; } = new();

    /// <summary>
    /// Data but without Id, Guid, Created, Modified
    /// </summary>
    [PrivateApi]
    public IDictionary<string, object> Attributes(RawConvertOptions options) => new Dictionary<string, object>(Properties)
    {
        { Eav.Data.Attributes.TitleNiceName, Name },
        { nameof(Name), Name },
    };

}