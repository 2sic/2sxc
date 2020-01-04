---
uid: HowTo.DynamicCode.Link
---

# Link / @Link Object in Razor / .net

Basically you can always link around to other pages, websites or views using normal `<a href="...">text</a>` html. And often you just want to add some parameters to the current Url like `?id=27` - but the behavior of this can be very different depending on the DNN settings. The `Link` object helps you handle this. 

_Note:_ DNN often has a problem with links, because depending on what page you are on, the behaviour is a bit different. This is especially important on the home page. Use `@Link.To(...)` to make sure everything works no matter what. 

## How to use

Here's a quick example of using the `Link` object in a Razor template: 

```html
<a href="@Link.To(parameters: "id=" + item.EntityId)">
    @item.Title 
</a>
```

This example creates a link to the current page, adding _either_ `?id=27` _or_ `/id/27`, depending on the DNN configuration. 

## How it works
The `Link`-object is always available in all Razor-templates. Internally it uses the DNN API to get the correct url. 

## Using @Link.To()
Example:

```Razor
@Link.To(parameters: "id=17")
@Link.To(parameters: "id=403&category=all")

```

## Using @Link.Base() for JavaScript SPA modules
This is new in 2sxc v9.5.1. It ensures that the url can be used for SPAs, as some pages will otherwise provide a wrong link (like home) which then breaks the SPA.

```html
<base href="@Link.Base()">
```

## Notes and Clarifications

The Link-Object is of type [](xref:ToSic.Sxc.Web.ILinkHelper).

### Enforced Parameter Naming

To promote long term API stability, we require all parameters to be [named](xref:HowTo.DynamicCode.NamedParameters) when used. This allows us to add further parameters later on, and the calls will still work.

```html
<!-- this will work -->
@Link.To(parameters: "id=17")
@Link.To(parameters: "id=403&category=all")

<!-- new in 2sxc 9.5.1 -->
@Link.To(pageId: 40, parameters: "id=403&category=all")

<!-- this won't work -->
@Link.To("id=17")
```

## Demo App and further links

You should find some code examples in this demo App
* [Blog App](xref:App.Blog)

## History

1. Introduced in 2sxc 8.4
2. Enhanced in 2sxc 9.5.1 with Base() and with parameter pageId on Link.To
