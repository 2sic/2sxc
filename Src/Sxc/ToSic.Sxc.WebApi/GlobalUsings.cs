// Global using directives

global using System;
global using System.Collections.Generic;
global using System.Diagnostics.CodeAnalysis;
global using System.Linq;
global using ToSic.Eav.Apps;
global using ToSic.Eav.Apps.Sys.Work;
global using ToSic.Eav.Context;
global using ToSic.Eav.Data;
global using ToSic.Eav.WebApi.Sys.Context;
global using ToSic.Eav.WebApi.Sys.Dto;
global using ToSic.Eav.WebApi.Sys.Helpers.Http;
global using ToSic.Sys.Coding;
global using ToSic.Sys.DI;
global using ToSic.Sys.Documentation;
global using ToSic.Sys.Data;
global using ToSic.Sys.Logging;
global using ToSic.Sys.Services;
global using ToSic.Sxc.Apps.Sys.Work;
global using ToSic.Sxc.Context.Sys;
global using ToSic.Sys.Performance;
global using static ToSic.Sxc.Sys.SxcLogging;


#if NETFRAMEWORK
global using THttpResponseType = System.Net.Http.HttpResponseMessage;
#else
global using THttpResponseType = Microsoft.AspNetCore.Mvc.IActionResult;
#endif