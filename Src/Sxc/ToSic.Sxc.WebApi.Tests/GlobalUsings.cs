// Global using directives

global using System;
global using System.Collections.Generic;
global using System.Diagnostics.CodeAnalysis;
global using System.Linq;
global using ToSic.Eav.Apps;
global using ToSic.Eav.Context;
global using ToSic.Sys.Coding;
global using ToSic.Sys.DI;
global using ToSic.Sys.Logging;


#if NETFRAMEWORK
global using THttpResponseType = System.Net.Http.HttpResponseMessage;
#else
global using THttpResponseType = Microsoft.AspNetCore.Mvc.IActionResult;
#endif

#if NETCOREAPP
global using Microsoft.AspNetCore.Mvc;
#endif