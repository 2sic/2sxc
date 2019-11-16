# Features API in .net

## Purpose / Description

2sxc / EAV in 9.30+ has a [features management](https://2sxc.org/en/blog/post/new-features-management-in-2sxc-9-30). In certain cases it would be good if the razor-view could verify that the feature is enabled - for example to show a warning when it isn't enabled yet. This is what the Features API is for.

## How To Use

This example is taken from [Mobius Forms](https://2sxc.org/en/apps/app/mobius-forms-2-with-file-upload) and the code can be found in the [Mobius Github Repo](https://github.com/2sic/app-mobius-forms/blob/master/_Shared-Feature-UploadInAdam.cshtml).

```c#
@using ToSic.Eav.Configuration
@{
    // show warning if the save-attachments in web api isn't activated
    var reqFeatures = new[]{new Guid("ecdab0f6-4692-4544-b1e7-72581f489f6a")};
    FeaturesDisabledException missingException;

    if(!Features.EnabledOrException(reqFeatures,
        "Warning: file upload won't work yet, as it hasn't been enabled.",
        out missingException)) {
        <div class="alert alert-warning">
            @missingException.Message
        </div>
    }
}
```

The code above checks if a feature is enabled, and if not, will show a message to the viewer that this must be enabled first.

## What you Need To Know

1. The API lies in the namespace `ToSic.Eav.Configuration`
1. The `Features` object is static, so you don't need to create it, just use the commands on it
1. ATM the public API has the following commands
    1. `Enabled(Guid featureId)` which checks if a feature is enabled
    1. `Enabled(IEnumerable<Guid> featureIds)` which checks if multiple features are enabled
    1. `EnabledOrException(IEnumerable<Guid> featIds, string message, out FeaturesDisabledException)` which will check and give you an error object back, which you can either throw or show the message of (like in the example above)

## Finding Feature GUIDs

At the moment there is no catalog of feature GUIDs yet, and sometimes you may actually create your own. So for now you'll mainly need this for features of 2sxc / eav, and you can simply look them up in the code, or see them in the [features management](https://2sxc.org/en/blog/post/new-features-management-in-2sxc-9-30) of the installation you're developing on.

_Warning: there are constants in the EAV code which have these feature GUIDs, but do not use them, as they will be moved to other places in the code at a later time_


## Read also

...

## History

1. Introduced in 2sxc 09.30
