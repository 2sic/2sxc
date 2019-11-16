
# DataSource Concept: Configuration Injection using Tokens

[DataSource](dotnet-datasource) objects have an sophisticated system to retrieve settings and configuration using tokens and more. 

## Purpose / Description
A [DataSource](dotnet-datasource) is usually configurable, meaning that it needs parameters to do it's job. Some examples:

* a [Module data source](dotnet-datasource-moduledatasource) needs to know the module ID 
* an [Owner-Filter DataSource](dotnet-datasource-ownerfilter) needs to know who the current user is, to find his items
* a [Paging DataSource](dotnet-datasource-paging) needs to know what page size it should use and what page it's on
* A CSV data source needs to know what file it should load

As you can see, some of this information depends on the current context (ModuleId, UserId), others on configured settings (page size) and some on Url-parameters (Page number). In addition, we sometimes want to say _"use the page-size configured in the App-Settings"_ or even more complex _"use from url, but if not specified, try app-settings, and if that isn't defined, use 10"_.

This is what this Token-Configuration-Injection-System is for. 

## Configuration Basics
Each configuration of a [DataSource](dotnet-datasource) is either a fixed string value like `17` or a token like `[Settings:PageNumber]`. In most cases it's a token. This token is parsed _before any data is queried_ to ensure that in the end the [DataSource](dotnet-datasource) has a usefull value before actually performing its task. 

## Token Basics
A token is a piece of text that looks like `[Source:Property]`. It is good to understand the full [token concept, discussed here](concept-tokens). You'll also want to read about fallback and recursion to understand the following content. 

## Shared Token-Suppliers / Token-Sources
When a DataSource is configured, it has many token-suppliers like `Module`, `QueryString`, `App` etc. These are shared and are identical for all objects. 

## Advanced Token-Source for Settings
The `Settings` source is a special source which contains all the properties of the settings-item which configures exactly this one data-source. For example the token `[Settings:PageNumber]` will deliver the number or text in the settings `pagenumber` field. 

## Advanced Token-Source for In
DataSources also have a source called `In` which is different for each DataSource, as each one has its own In-streams. You can use it in tokens like `[In:Default:PageSize]` where the term after `In:` is the stream-name to be consulted. 

## How Tokens are Defined, Settings Edited and Resolved
When you're using the visual query designer, the configuration created is saved as an Entity (aka Content-Item) which must be injected into the DataSource configuration automatically. But when you use the object is your code, your code must be able to provide other values. But how does this work?

1. Each DataSource object has a property called `Configuration` which is a dictionary containing all configuration properties the data source will care about. For example, the `EntityIdFilter` has a Configuration with only one property which is called `EntityIds`. 
2. The each property is first initialized with a Token-Template. For example, the CsvDataSource has a  
`ConfigMask(DelimiterKey, "[Settings:Delimiter||\t]");`   
This says that the delimiter should come from the Settings-Entity field `Delimiter` and if not provided, fall back to `\t` (which is a tab character)  
_read about [ConfigMask here](dotnet-datasource-api-configmask)_  
3. For the programmer who wants to set a number or whatever, this would be fairly unreliable to access from outside, so the DataSource should also have a real property which internally also modifies the dictionary. For example, the CsvDataSource has a string-property `Delimiter` which internally will get/set the in the Configuration dictionary.  
3. When the DataSource is first _sucked_ from, which happens when something tries to access the Out-Property, it will automatically run a token-engine to resolve the values, then run whatever action the data-source wants. _read about [ensuring configuration is parsed](dotnet-datasource-api-ensureconfigurationisloaded) here_

So how does each scenario work out?

1. If the programmer overwrote the `Delimiter` property, then internally the `Configuration["Delimiter"]` is now not a token any more, but instead just a character like `,`. So the token-engine won't change anything. 
1. If the programmer didn't do anything but the [visual query][vqd] engine gave a settings-entity to the system, then the token is resolved and whatever the user entered is used. 
1. if the neither the programmer nor the user provided settings, then the token-engine will resolve to the fallback and use the `\t` as was defined.

## Also Read

* about [Data Streams](DotNet-DataStream)
* [Tokens concept](concept-tokens)
* [DataSource API](dotnet-datasource-api)
* [Ensuring configuration is parsed](dotnet-datasource-api-ensureconfigurationisloaded)


## History

1. General Tokens introduced in 2sxc 1.0
1. Most enhancements were in 2sxc 07.00

[//]: # "The following lines are a list of links used in this page, referenced from above"
[eav-core-code]: https://github.com/2sic/eav-server/tree/master/ToSic.Eav.Core 
[blog-custom-ds]: http://2sxc.org/en/blog/post/new-2sxc7-create-your-own-custom-datasource-for-visual-query
[app-ds-code]: http://2sxc.org/en/apps/app/tutorial-use-a-custom-developed-datasource
[vqd]: http://2sxc.org/en/Learn/Visual-Query-Designer