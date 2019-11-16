# Convention: Named Parameters

When working with the C# / Razor API, most commands require named parameters. This means that a command like this is valid

```c#
<div @Edit.TagToolbar(actions: "new", contentType: "BlogPost")>
  ...
</div>
```

...and this is not

```c#
<div @Edit.TagToolbar("new", "BlogPost")>
  ...
</div>
```

## Reason Behind Named Parameters

We often have APIs which start simple - like `@Edit.TagToolbar()` and continue to receive new features. At first, the parameter order will make sense - for that simple use case. But as the API grows, the parameter-order will become strange, simply because we would have to order them in the sequence they were added (to keep compatibility) and not in the order that makes sense.

By using named parameters, we're making sure that the parameter order never matters and the API stays stable/compatible for the future.


## Not all Parameters Require Names

Because of historic reasons and because some APIs simply have a very obvious first or second parameter, it may be that the first 1-2 parameters are not named. An example is `@Edit.TagToolbar(Content)` which assumes that the first parameter without name is the item (entity) for which this toolbar is meant.


## How It's Implemented

Internally the real signature of the command uses a parameter which has a fairly random value. The call then checks if the value of that parameter is this random value, and if not, shows an error. This is to protect you from accidentally using the command without naming the parameters.

_Note: you could of course work around this, by providing that random value and trick the call to accept unnamed parameters. Don't do this - as we will no guarantee that the API signature (parameter order) will stay the same._

## History

1. Introduced ca. in 2sxc 6
