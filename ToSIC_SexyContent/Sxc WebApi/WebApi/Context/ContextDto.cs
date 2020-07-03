using System.Collections.Generic;
using Newtonsoft.Json;
using static Newtonsoft.Json.NullValueHandling;

namespace ToSic.Sxc.WebApi.Context
{
    public class ContextDto
    {
        [JsonProperty(NullValueHandling = Ignore)] public AppDto App;
        [JsonProperty(NullValueHandling = Ignore)] public LanguageDto Language;
        [JsonProperty(NullValueHandling = Ignore)] public UserDto User;
        [JsonProperty(NullValueHandling = Ignore)] public WebResourceDto System;
        [JsonProperty(NullValueHandling = Ignore)] public WebResourceDto Site;
        [JsonProperty(NullValueHandling = Ignore)] public WebResourceDto Page;
        [JsonProperty(NullValueHandling = Ignore)] public EnableDto Enable;
    }

    public class WebResourceDto
    {
        [JsonProperty(NullValueHandling = Ignore)] public int? Id;
        [JsonProperty(NullValueHandling = Ignore)] public string Url;
    }

    public class AppDto: WebResourceDto
    {
        public string Name;
        public string Identifier;
        public string GettingStartedUrl;
    }

    public class EnableDto
    {
        public bool AppPermissions;
        public bool CodeEditor;
        public bool Query;
    }

    public class LanguageDto
    {
        public string Primary;
        public string Current;
        public Dictionary<string, string> All;
    }

    /// <summary>
    /// Will be enhanced later
    /// </summary>
    public class UserDto
    {
    }
}
