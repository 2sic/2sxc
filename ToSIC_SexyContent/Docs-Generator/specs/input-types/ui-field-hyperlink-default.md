---
uid: Specs.Data.Inputs.Hyperlink-Default
---
# Field Input-Type **hyperlink-default**

Use this field type for configuring normal links, page or file/image references, storing [Hyperlink](xref:Specs.Data.Values.Hyperlink). It's an extension of the [hyperlink field type](xref:Specs.Data.Inputs.Hyperlink).

## Features 

1. input field for normal links, page or file/image references ( like `http://whatever/whatever`, `/some-relative-url`, `page:42`, `page:42?something=value`, `file:2750`, `file:2750?w=200` ...)
2. allows users to pick files/images with ADAM (automatic digital asset manager) if activated
3. allows users to pick page reference with the page picker if activated
4. allows users to pick images with the the image manager if activated
5. allows users to pick images with the the file manager if activated

## Configuring an Hyperlink-Default
No relevant settings to be configured.

## History
1.  Introduced in EAV 1.0 / 2sxc 1.0, originally as part of the [hyperlink field type](xref:Specs.Data.Inputs.Hyperlink)
2.	Changed in EAV 3.0 / 2sxc 6.0 (it used to have many configuration fields for all kinds of uses, which were then moved to sub-types)


