---
uid: Specs.DataSources.ConfigurationTokens
---

# Concept: Tokens

Often you need a text-based code which should be replaced at runtime with a real value from elsewhere. In DNN / 2sxc this is called a **token**, and they usually look like `[Source:Property]`. At runtime, this will then show a value like `27`. 

_Note: this article applies to tokens as they are handled server-side. There is a special token-like syntax in JavaScript, which is not discussed here._

## Token Basics
A token is a piece of text that looks like `[Source:Property]`, which will be replaced by an engine so that it will then be a value. For example, `[QueryString:Page]` is replaced with `2` if the current url has `?page=2` in it. You can research more about tokens [in older docs here](https://2sxc.org/en/Learn/Token-Templates-and-Views) and in the [full list of standard tokens](https://2sxc.org/dnn-app-demos/en/Apps/Tutorial-Tokens), it's a standard DNN concept. 

## Additional Token Features in 2sxc/EAV
The EAV and 2sxc have enhanced Tokens to a new level with these features:

## 1. Sub-Tokens
A token like `[App:Settings:PageSize]` will go through a tree of info-objects to find an inner property if it exists. This only works on special object types that are specifically meant to provide sub-data.

## 2. Fallback
A token like `[QueryString:page||1]` will deliver the url-param, and if that is empty, will deliver `1`. Note that you need 2 pipe symbols `|` because the convention is that after the first pipe you can have a format specifier like `#.##`.

## 3. Stacking
Stacking with more Tokens: a token like `[QueryString:PageSize||[App:Settings:PageSize]]` will try the first token, and if it doesn't resolve, try the next one

## 4. **Recursion**
A token can resolve into a token, which would then be looked up again. So if a token `[Settings:Page||1]` is used, and the setting `Page` is not a number but again a token like `[QueryString:Page]`, then it will...

1. resolve `Settings:Page` and find `Querystring:Page`
1. resolve `QueryString:Page` and maybe find something
1. if that is empty, return the fallback 1

## Some Token Examples

1. `[Portal:PortalId]` would return the current portal Id
1. `[App:Settings:PageSize]` would return the page size as configured in app-settings
1. `[QueryString:Id]` would retrieve the id-parameter from the url (note: you should never put this in your data source)
1. `[Settings:ProductId]` would retrieve the id as configured in the UI by the user
1. `[Settings:productId||27]` would also try to get the id, but return 27 if not found


## Advanced Token Sources in Special Scenarios
Some situations will have token sources beyond the default. For example, when configuring data sources they always have 2 more sources

* `In` - used like `[In:Default:PageSize]`
* `Settings` - used like `[Settings:PageSize]`
* There is another special override-token system (not documented) which is used for testing

For more on this, read the [DataSource Configuration](xref:Specs.DataSources.Configuration) section. 

## Also Read

* @Specs.LookUp
* @Specs.DataSources.Configuration
* @ToSic.Eav.DataSources.IDataStream
* @Specs.DataSources.Api
* @Specs.DataSources.Api.EnsureConfigurationIsLoaded


## History

1. General Tokens introduced in 2sxc 1.0
1. Most enhancements were in 2sxc 07.00
