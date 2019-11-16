# Data Type: Hyperlink

## Purpose / Description
Hyperlink data is a basic [data type](data-types) and is actually a string, but on reading it's automatically converts to an output-friendly format. It's used for normal links, page or file/image references as well as for complete sets of files (like image galleries). 


## Storage in the SQL Database in the EAV-Model
This is simply stored as a string in the DB, in the original format like `page:22`.

## Storage in the SQL Database in the JSON-Model
This is simply stored as a JSON string in the original format like `page:22`.

## Special: Automatic Conversions
The hyperlink data internally can contain values like:
1. `http://whatever/whatever` - will not be converted
1. `/some-relative-url` - will not be converted
1. `page:42` - will usually be converted to the real url of the page in DNN
1. `page:42?something=value` - this will also be converted, but keeping the parameters
1. `page:42#something=value` - this will also be converted, but keeping the parameters
1. `file:2750` - will usually be converted to the real url of the file in DNN
1. `file:2750?w=200` - this will also be converted, but keeping the parameters (like for thumbnails)
1. `file:2750#page=2` - this will also be converted, but keeping the parameters (like for pdf-page-links)

In 99% of all use cases, you want to generate html with a real link, which is why the content-objects in Razor will deliver an `http:...` instead of `file:27`. Some demo-code: 

```razor
  // assume that Content is a dynamic entity
  // assume that Image actually contains "file:274"
  <img src="@Content.Image">

  // the result is now
  <img src="/portals/0/adam/20603963uaothutaoer/daniel.jpg">
```


## Read also

* [Hyperlink fields](ui-field-hyperlink) documentation about using it in the UI

## History
1. Introduced in EAV 1.0 2sxc 1.0