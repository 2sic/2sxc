using DotNetNuke.Entities.Users;
using System.Collections;
using System.Collections.Immutable;
using ToSic.Eav.Context;
using ToSic.Eav.Context.Sys.ZoneMapper;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.ContentTypes;
using ToSic.Eav.Data.Raw;
using ToSic.Eav.Data.Sys;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.Sys;
using ToSic.Eav.DataSource.VisualQuery;
using ToSic.Eav.Sys;
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
    NameId = "34bbcbee-72cd-483f-8c42-c2e696b21b14",
    ConfigurationType = "|Config ToSic.SexyContent.DataSources.DnnUserProfileDataSource",
    NameIds =
    [
        "ToSic.Sxc.Dnn.DataSources.DnnUserProfile, ToSic.Sxc.Dnn",
        "ToSic.SexyContent.Environment.Dnn7.DataSources.DnnUserProfileDataSource, ToSic.SexyContent"
    ]
)]
public class DnnUserProfile : CustomDataSource
{
    #region Configuration-properties

    /// <summary>
    /// The user id list of users to retrieve, comma-separated
    /// </summary>
    [Configuration]
    public string UserIds
    {
        get => field ?? Configuration.GetThis();
        set;
    }

    /// <summary>
    /// List of profile-properties to retrieve, comma-separated
    /// </summary>
    [Configuration(Fallback = "DisplayName,Email,FirstName,LastName,Username")]
    public string Properties
    {
        get => field ?? Configuration.GetThis();
        set;
    }

    ///// <summary>
    ///// Gets or sets the Name of the ContentType to simulate
    ///// </summary>
    //[Configuration(Field = "ContentTypeName", Fallback = DnnUserProfileDataRaw.TypeName)]
    //public string ContentType
    //{
    //    get => _contentType ?? Configuration.GetThis();
    //    set => _contentType = value;
    //}
    //private string _contentType;

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

    public new record Dependencies(CustomDataSource.Dependencies ParentServices, ISite Site, IZoneMapper ZoneMapper, LazySvc<DnnSecurity> DnnSecurity)
        : DependenciesBase(connect: [Site, ZoneMapper, DnnSecurity]);

    public DnnUserProfile(Dependencies services) : base(services.ParentServices, "Dnn.Profile", connect: [services])
    {
        ProvideOutRaw(() => GetList(services));
    }


    #endregion

    private IImmutableList<IRawEntity> GetList(Dependencies _services)
    {
        var l = Log.Fn<IImmutableList<IRawEntity>>();
        Configuration.Parse();

        var realTenant = _services.Site.Id != EavConstants.NullId
            ? _services.Site
            : _services.ZoneMapper.SiteOfApp(AppId);
        l.A($"realTenant {realTenant.Id}");

        var portalId = realTenant.Id;

        // read all user Profiles
        ArrayList users;
        if (!UserIds.HasValue() ||
            UserIds == "disabled") // note: 'disabled' was the default text in <v15. can probably be removed, but not sure
            users = UserController.GetUsers(portalId);
        // read user Profiles of specified UserIds
        else
        {
            var userIds = UserIds.CsvToArrayWithoutEmpty().Select(int.Parse).ToArray();
            users = [];
            foreach (var user in userIds.Select(userId => UserController.GetUserById(portalId, userId)))
                users.Add(user);
        }
        l.A($"users: {users.Count}");

        // convert Profiles to Entities
        var properties = Properties.CsvToArrayWithoutEmpty();
        var results = users
            .OfType<UserInfo>()
            .Select(user =>
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

                return (IRawEntity)dnnUserProfile;
            })
            .ToImmutableList();
        l.A($"results: {results.Count}");
        return l.Return(results, "ok");
    }

    private static string GetDnnProfileValue(UserInfo user, string property) =>
        property.ToLowerInvariant() switch
        {
            "displayname" => user.DisplayName,
            "email" => user.Email,
            "firstname" => user.FirstName,
            "lastname" => user.LastName,
            "username" => user.Username,
            _ => user.Profile.GetPropertyValue(property)
        };
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
[ContentType(
    Name = TypeName,
    Guid = "8c72d18b-41a9-451c-905e-dd32ec953567"
)]
public class DnnUserProfileDataRaw : IRawEntity
{
    internal const string TypeName = "UserProfile";
  
    public int Id { get; set; }
    public Guid Guid { get; set; }

    [ContentTypeTitle]
    public string Name { get; set; } // aka DisplayName

    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }

    [PrivateApi]
    public IDictionary<string, object> Values => field ??= new Dictionary<string, object>(Properties)
    {
        { AttributeNames.TitleNiceName, Name },
        { nameof(Name), Name },
    };

    public Dictionary<string, object> Properties { get; } = new();

}