using System.Text.Json;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Serialization;
using ToSic.Sxc.Data.Internal.Wrapper;

namespace ToSic.Sxc.Cms.Data;

/// <summary>
/// Represents GPS coordinates.
/// </summary>
/// <remarks>
/// Released in v17.03, still BETA.
/// </remarks>
[PublicApi]
public class GpsCoordinates
{
    /// <summary>
    /// The latitude (North is +, South is -) of the GPS coordinates.
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    /// The longitude (East is +, West is -) of the GPS coordinates.
    /// </summary>
    public double Longitude { get; set; }

    /// <summary>
    /// Parse a json string into a <see cref="GpsCoordinates"/> object.
    /// It's an own function, to ensure that the deserialization is done with the correct options,
    /// since it may be used in places where the IJsonService is not available.
    /// </summary>
    [PrivateApi]
    internal static GpsCoordinates FromJson(string json)
    {
        var safeJson = json.NullIfNoValue() ?? WrapperConstants.EmptyJson;
        return JsonSerializer.Deserialize<GpsCoordinates>(safeJson, options: JsonOptions.SafeJsonForHtmlAttributes);
    }
}