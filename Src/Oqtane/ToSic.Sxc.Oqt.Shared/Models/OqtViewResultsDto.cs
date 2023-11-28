using System.Collections.Generic;

namespace ToSic.Sxc.Oqt.Shared.Models;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class OqtViewResultsDto
{
    /// <summary>
    /// The final HTML to show in the browser
    /// </summary>
    public string FinalHtml => $"{Html}{PrerenderHtml}";

    /// <summary>
    /// The content HTML
    /// </summary>
    public string Html { get; set; }

    /// <summary>
    /// For special error messages, in case the backend has trouble with refs etc.
    /// </summary>
    public string ErrorMessage { get; set; }
        
    /// <summary>
    /// The resources which the template will need
    /// </summary>
    public List<SxcResource> TemplateResources { get; set; }

    /// <summary>
    /// The Context meta name tag - null if not needed
    /// </summary>
    public string SxcContextMetaName { get; set; }
        
    /// <summary>
    /// Will return the meta-header which the $2sxc client needs for context, page id, request verification token etc.
    /// </summary>
    /// <returns></returns>
    public string SxcContextMetaContents { get; set; }

    /// <summary>
    /// The JavaScripts needed by 2sxc (not by the template)
    /// </summary>
    /// <returns></returns>
    public List<string> SxcScripts { get; set; }

    /// <summary>
    /// The styles to add for 2sxc inpage to work (not from the template)
    /// </summary>
    /// <returns></returns>
    public List<string> SxcStyles { get; set; }
        
    /// <summary>
    /// List of page property changes as specified
    /// </summary>
    public IEnumerable<OqtPagePropertyChanges> PageProperties { get; set; }

    /// <summary>
    /// Changes to the Page Header like Meta-Tags etc.
    /// </summary>
    public IEnumerable<OqtHeadChange> HeadChanges { get; set; }

    /// <summary>
    /// Features which are defined in the SystemSettings and wer requested by the code and should be enabled.
    /// </summary>
    //IList<IPageFeature> FeaturesFromSettings { get; }

    /// <summary>
    /// Prerender HTML fragment
    /// </summary>
    public string PrerenderHtml { get; set; }

    /// <summary>
    /// List of HttpHeaders to add to the response in format "key:value"
    /// </summary>
    //IList<string> HttpHeaders { get; }
    public IList<HttpHeader> HttpHeaders { get; set; }

    public bool CspEnabled { get; set; }
    public bool CspEnforced { get; set; }
    public IList<string> CspParameters { get; set; }
}