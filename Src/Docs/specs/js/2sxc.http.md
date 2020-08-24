---
uid: Specs.Js.$2sxc.Http
---
# JS: The $2sxc.http API

The `$2sxc.http` object contains information for doig API calls. _It's new - introduced in 2sxc 10.25_.

> [!NOTE]
> The `http`-object was introduced to help $2sxc run without jQuery and without the rather instable ServicesFramework of DNN. 

It has the following important methods:

* `apiUrl(url)` resolves a partial url like `app/auto/api/Posts/All` to the real url needed in DNN
* `apiUrl(url, endpointName)` special version of the `apiUrl(...)` method, for calling endpoints which are not in 2sxc
* `headers()` returns the headers you need to add to a WebApi call for it to work in DNN
* `headers(moduleId)` the headers incl. the one needed when addressing a specific module

Internally this information is automatically retrieved from the html-header. 
The environment looks for a special meta-tag called `_jsApi` which contains all this information. 
This is new in 2sxc 10.25 and was added to avoid using jQuery when not necessary. 

> [!WARNING]
> Internally all these commands need the [env](xref:Specs.Js.$2sxc.Env) to be ready. 
> This means that the entire html `<head>` tag was processed by the browser. 
> A very safe way to do this is to run your code on-document-ready, 
> or just to ensure that whatever bootstraps your application runs inside the `<body>` tag. 

## Internal Stuff

The `http` also has some internal methods like:

* `apiRoot(endpointName)` the full api-root to use in calling DNN or 2sxc endpoints. It's internal, because for all 2sxc stuff and 2sxc Apps the endpointName is always `2sxc`
* `headers(moduleId, contentBlockId)` headers incl. ModuleId and content-Block ID - rarely used

## History

1. Introduced in 2sxc 10.25
