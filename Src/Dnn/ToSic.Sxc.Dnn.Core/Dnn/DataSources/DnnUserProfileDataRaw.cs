using DotNetNuke.Entities.Users;
using System.Collections;
using System.Collections.Immutable;
using ToSic.Eav.Context;
using ToSic.Eav.Context.Sys.ZoneMapper;
using ToSic.Eav.Data;
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
public class DnnUserProfile(DnnUserProfile.Dependencies services)
    : CustomDataSource(services.ParentServices, "Dnn.Profile", connect: [services])
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

    #endregion

    protected override IEnumerable<IRawData> GetDefault()
    {
        var l = Log.Fn<IImmutableList<IRawEntity>>();
        Configuration.Parse();

        var realTenant = services.Site.Id != EavConstants.NullId
            ? services.Site
            : services.ZoneMapper.SiteOfApp(AppId);
        l.A($"realTenant {realTenant.Id}");

        var portalId = realTenant.Id;

        // Results list - a special DNN type
        ArrayList users;

        // Option 1: read all user Profiles
        if (!UserIds.HasValue() || UserIds == "disabled") // note: 'disabled' was the default text in <v15. can probably be removed, but not sure
            users = UserController.GetUsers(portalId);
        // Option 2: read user Profiles of specified UserIds
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
            .Select(IRawEntity (user) =>
            {
                var name = GetDnnProfileValue(user, TitleField.ToLowerInvariant());

                // add Profile-Properties
                var userProps = properties
                    .ToDictionary(
                        property => property,
                        object (property) => GetDnnProfileValue(user, property)
                    );

                // TODO: This is copied from some older code, not sure if it's actually relevant to add these names
                // ...as they are probably auto-added (especially the title...?)
                userProps.Add(AttributeNames.TitleNiceName, Name);
                userProps.Add(nameof(DnnUserProfileDataRaw.Name), name);

                return new DnnUserProfileDataRaw
                {
                    Id = user.UserID,
                    Guid = services.DnnSecurity.Value.UserGuid(user),
                    Name = name,
                    Values = userProps,
                };
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
    private const string TypeName = "UserProfile";
  
    public int Id { get; init; }
    public Guid Guid { get; init; }

    /// <summary>
    /// Also known as `DisplayName`
    /// </summary>
    [ContentTypeTitle]
    public string Name { get; init; }

    public DateTime Created { get; init; }
    public DateTime Modified { get; init; }

    [PrivateApi]
    public required IDictionary<string, object> Values { get; init; }
}
