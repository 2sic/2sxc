# SaveInAdam command in SxcApiController

## Purpose / Description
The `SaveInAdam` command helps your WebApi to upload files so they are in an ADAM container of an item.

## How to use
Here's a simple example, taken from [mobius forms](https://github.com/2sic/app-mobius-forms/blob/master/api/FormController.cs)

```c#
SaveInAdam(stream: dataStream,
    fileName: fileNameInForm,
    contentType: "Advertisement",
    guid: guid,
    field: "Images");
```

Here's what the parameters are:

1. `stream` contains a stream of bytes with the file
1. `fileName` contain the file name
1. `contentType` is the content-type of the entity we're saving to
1. `guid` is the entity-guid which receives this item
1. `field` is the field we're saving to

## Example

The following example is also from [mobius forms](https://github.com/2sic/app-mobius-forms/blob/master/api/FormController.cs) and assumes that the html form encoded the data in JavaScript for sending to the WebApi:

```c#
// Save files to Adam
var files = ((Newtonsoft.Json.Linq.JArray)contactFormRequest["Files"])
    .ToObject<IEnumerable<Dictionary<string, string>>>();
foreach(var file in files)
{
    var data = Convert.FromBase64String((file["Encoded"]).Split(',')[1]);
    SaveInAdam(stream: new MemoryStream(data),
        fileName: file["Name"],
        contentType: type.Name,
        guid: guid,
        field: file["Field"]);
}

```


## Notes and Clarifications

* all parameters (`stream`, `fileName`, ...) must be named by [convention](convention-named-parameters)
* the uploaded files are placed in the container of the field...
* ...and not added as a link to the file, so you will usually use library fields
* the field must be a field of type hyperlink, library or wysiwyg, other fields cannot receive files

## Read also, Demo App and further links

You should find some code examples in these apps

* [Mobius Forms v2 in App Catalog](https://2sxc.org/en/apps/app/mobius-forms-2-with-file-upload)
* [Mobius Forms v2 on Github](https://github.com/2sic/app-mobius-forms)
* [Blog Recipe for uploading into ADAM in your WebAPI](https://2sxc.org/en/blog/post/recipe-form-files-saveinadam-in-your-custom-webapi)

## History

1. Introduced in 2sxc 9.30
