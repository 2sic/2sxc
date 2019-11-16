[//]: # "Just fyi: this is a comment - it won't show up in the resulting output"

[//]: # "Notes on naming your new files"
[//]: # "Because we use a flat structure like wikipedia, which also makes linking between pages more reliable"
[//]: # "To keep things clear though, please use fairly clear names like 'JavaScript-2sxc.Property.md'"

# Link / @Link Object in Razor / .net
[//]: # "The title should say if it's an event/method/property, the name, + the Technology like Razor, JavaScript, jQuery"

## Purpose / Description
[//]: # "short description / purpose, 2-3 lines"
Basically you can always link around to other pages, websites or views using normal `<a href="...">text</a>` html. And often you just want to add some parameters to the current Url like `?id=27` - but the behavior of this can be very different depending on the DNN settings. The `Link` object helps you handle this. 

_Note:_ DNN often has a problem with links, because depending on what page you are on, the behaviour is a bit different. This is especially important on the home page. Use `@Link.To(...)` to make sure everything works no matter what. 

## How to use
[//]: # "usually start with some demo code, as it's probably the quickest way to learn"

Here's a quick example of using the `Link` object in a Razor template: 

```html
<a href="@Link.To(parameters: "id=" + item.EntityId)">
    @item.Title 
</a>
```
This example creates a link to the current page, adding _either_ `?id=27` _or_ `/id/27`, depending on the DNN configuration. 

## How it works
[//]: # "Some explanations on the functionality"
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
### Object and Interfaces
The Edit-Object is of type `ToSic.SexyContent.Interfaces.ILinkHelper`.

### Enforced Parameter Naming
To promote long term API stability, we require all parameters to be [named](convention-named-parameters) when used. This allows us to add further parameters later on, and the calls will still work.

```html
<!-- this will work -->
@Link.To(parameters: "id=17")
@Link.To(parameters: "id=403&category=all")

<!-- new in 2sxc 9.5.1 -->
@Link.To(pageId: 40, parameters: "id=403&category=all")

<!-- this won't work -->
@Link.To("id=17")
```

## Read also
[//]: # "Additional links - often within this documentation, but can also go elsewhere"
...

## Demo App and further links
[//]: # "Apps which provide sample code using this"

You should find some code examples in this demo App
* [Blog App](http://2sxc.org/en/apps/app/dnn-blog-app-for-dnn-dotnetnuke)

## History
[//]: # "If possible, tell when it was added or modified strongly"

1. Introduced in 2sxc 8.4
2. Enhanced in 2sxc 9.5.1 with Base() and with parameter pageId on Link.To

[//]: # "This is a comment - for those who have never seen this"
[//]: # "The following lines are a list of links used in this page, referenced from above"



[float-toolbar]: http://2sxc.org/en/Docs-Manuals/Feature/feature/2875

