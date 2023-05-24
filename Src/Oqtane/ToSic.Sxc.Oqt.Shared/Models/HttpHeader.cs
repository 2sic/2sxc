using Microsoft.VisualBasic;
using System;

namespace ToSic.Sxc.Oqt.Shared.Models;

public class HttpHeader
{
    public HttpHeader(string name, string value)
    {
        Name = name;
        Value = value;
    }

    public string Name { get; set; }
    public string Value { get; set; }

    /// <summary>
    /// If set, would flush existing headers
    /// </summary>
    public bool Reset { get; set; }
}
