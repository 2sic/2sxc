---
uid: WebApi.Query
---

# 2sxc Query REST Web API for Read Operations

Every Visual Query you create has a REST URL. If you set the permissions, you can then read from the Query through REST. You can also pass query-parameters in the URL.



<iframe src="https://azing.org/2sxc/r/34pAzAF2?embed=1" width="100%" height="400" frameborder="0" allowfullscreen style="box-shadow: 0 1px 3px rgba(60,64,67,.3), 0 4px 8px 3px rgba(60,64,67,.15)"></iframe>


TODO: CONTINUE HERE



2sxc provides a full set of REST endpoints for CRUD (Create, Read, Update, Delete) operations. 

To use them, you would need to enable the permissions and then you can access them using the REST URLs.








2sxc has a fully featured WebApi for use in JavaScript or as Headless CMS. So you can call HTTP `GET` to an endpoint like `app/News/content/NewsItems/` and get a list of news items. 

You can also create / modify data using REST, access Queries and work with your custom Web-APIs - across all 2sxc platforms (Dnn / Oqtane / Custom) and with the security you need. 

<img src="./assets/web-api-overview.svg" width="100%">


## Simple Example

Here's an example JavaScript which would run in a 2sxc News-App on a DNN page:

```javascript
// Get news as a promise
var allNewsPromise = sxc.webApi.get('app/auto/content/News/');

// now log to console
allNewsPromise.then(data => console.log(data));
```

This example has some magic happening in the background. Specifically 3 important things happen on the **client** before sending:

1. The real URL which is requested is a bit longer, but the `webApi` takes care of that.
1. The server needs to know what App and Module the script is calling from. This _Context_ information is also added by the `webApi`.
1. To add security, a crypto-token is added which will prove that the JavaScript requesting this is running on the page. This is also added by `webApi`.

When the **server** receives the request, it too does important stuff.

1. First it will use the URL and _Context_ information to figure out what _Site_, _Language_ and _App_ should be accessed.
1. The server will do some security checks to see if this request should be answered.
1. Then it returns the data in a simple **JSON** format


## Getting Started

To get started, we recommend the following steps:

1. First review some tutorials and examples to get acquainted
1. Usually you'll be more interested in reading data, for that you should read about
  1. Getting lists of items / entities
  1. Getting a single item/ entity
  1. Security settings for reading data
1. 



## History

1. Introduced Content-REST API in 2sxc 5.x
2. Enhanced with Polymorph Editions in 2sxc 9.35 (allowing subfolder/api)
