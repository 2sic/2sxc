---
uid: Specs.Js.Commands
---
# Html & JS: Calling Commands of the CMS

Whenever you press a button in the edit-ui, a edit-command is handled by the javascript layer. These commands  are things like

* `edit` an item on the screen
* open the `layout`-picker dialog for a content-block

To keep things simple, these commands all have the convention of using a **short command name** like `new`, some **parameters** like `entityId` which differ for each command and usually some context like _this is happening in the module 7503_.   

## How to use
In most cases you don't have to even think about this, because the hover-buttons will automatically call the command as needed. But there are cases where you may want to do so yourself - for example with very custom buttons or if you want to automate something. Here's an example:

```html
<!-- quick version with name only --> 
<a onclick="$2sxc(this).manage.run('layout', event)">
    change layout
</a>

<!-- expanded version -->
<a onclick="$2sxc(this).manage.run({ action: 'layout' }, event)">
    change layout
</a>

<!-- expanded version with many params -->
<a onclick="$2sxc(this).manage.run({ action: 'new', contentType: 'BlogPost' }, event)">
    createBlogPost
</a>
```

These examples example gets the $2sxc-controller related to the `<a>` tag using `$2sxc(this)` and thereby giving it a context so it knows what module-id, etc. Then it executes the command. 

Here's a fairly realistic setup using Razor and custom buttons in HTML:

```html
@if(Permissions.UserMayEditContent)
{


<ol>
    <li>
        <a onclick='$2sxc(this).manage.run({"action": "layout"})'>layout</a>
    </li>
    <li>
        <a onclick='$2sxc(this).manage.run({"action": "new", "contentType": "Dummy"})'>new</a>
    </li>
    <li>
        <a onclick='$2sxc(this).manage.run({"action": "edit", "entityId": @Content.EntityId})'>edit #@Content.EntityId</a>
    </li>
    <li>
        <a onclick='$2sxc(this).manage.run({"action": "edit", "useModuleList": true, "sortOrder": 0 })'>edit slot 0 of module list</a>
    </li>
    <li>manage table of <a onclick='$2sxc(this).manage.run({"action": "contentitems"})'>items of the type used in this template</a> or of 
        <a onclick='$2sxc(this).manage.run({"action": "contentitems", contentType: "Quotes"})'>Quotes</a>
    </li>
</ol>
}
```

Maybe you also want to put the command-construction in more code, like this:

```javascript
// the function which does this
function openLayout(moduleId){
    var command = {
        action: "layout"
    }
    $2sxc(moduleId).manage.run(command);
}

// the jquery call to do this on-load
$(function() {
    openLayoutOnPageLoad(740);
})
```


## Running a Command
Always use the sxc-controller of a module, then access the `.manage.run(...)` method to run a command. There are 3 calls you can use:

* `run("layout")` - for simple commands requiring only the name
* `run("new", { contentType: "BlogPost" })` - for additional parameters
* `run({ action: "new", contentType: "BlogPost" })` - does the same as above
* `run(..., event)` - if you had an event like click, it's best to always include it as last parameter

### Some Examples
Every action in the UI is a command, and for it to run, it must know a few things, like

```javascript
    var sxc = $2sxc(7523);  // get sxc for moduleId 7523
    var newCommand = {
        action: "new", 
        contentType: "BlogPost"
    };
    sxc.manage.run(newCommand);

    var editItemCommand = {
        action: "edit",
        entityId: 760
    };
    sxc.manage.run(editItemCommand);

    var editSlot7Params = {
        useModuleList: true,
        sortOrder: 7
    };
    sxc.manage.run("edit", editSlot7Params);
```

## Command With Custom Code
There is a command called **custom** which is meant to be used for this. Check out the example on [Custom Code](xref:Specs.Js.Commands.Code)

## All Commands & Parameters (todo - update)

To understand the internals, check out the [source code](https://github.com/2sic/2sxc-ui/blob/master/src/inpage/commands/commands.definitions.js). These are the currently available actions and their parameters:




<table border="0" cellpadding="0" cellspacing="0" width="650" style="border-collapse:
 collapse;table-layout:fixed;width:488pt">
 <colgroup><col width="130" style="mso-width-source:userset;mso-width-alt:4754;width:98pt">
 <col width="84" style="mso-width-source:userset;mso-width-alt:3072;width:63pt">
 <col width="436" style="mso-width-source:userset;mso-width-alt:15945;width:327pt">
 </colgroup><tbody><tr height="20" style="height:15.0pt">
  <td height="20" class="xl6622490" align="left" width="130" style="height:15.0pt;
  width:98pt;font-size:11.0pt;color:black;font-weight:700;text-decoration:none;
  text-underline-style:none;text-line-through:none;font-family:Calibri;
  border-top:.5pt solid black;border-right:none;border-bottom:.5pt solid black;
  border-left:none">Action Name</td>
  <td class="xl6622490" align="left" width="84" style="width:63pt;font-size:11.0pt;
  color:black;font-weight:700;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri;border-top:.5pt solid black;
  border-right:none;border-bottom:.5pt solid black;border-left:none">Purpose</td>
  <td class="xl6622490" align="left" width="436" style="width:327pt;font-size:11.0pt;
  color:black;font-weight:700;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri;border-top:.5pt solid black;
  border-right:none;border-bottom:.5pt solid black;border-left:none">Description
  and Parameters</td>
 </tr>
 <tr height="180" style="height:135.0pt">
  <td height="180" class="xl6522490" align="left" width="130" style="height:135.0pt;
  width:98pt;font-size:11.0pt;color:black;font-weight:400;text-decoration:none;
  text-underline-style:none;text-line-through:none;font-family:Calibri;
  background:#D9D9D9;mso-pattern:#D9D9D9 none">new</td>
  <td class="xl6522490" align="left" width="84" style="width:63pt;font-size:11.0pt;
  color:black;font-weight:400;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri;background:#D9D9D9;mso-pattern:
  #D9D9D9 none">Edit</td>
  <td class="xl6522490" align="left" width="436" style="width:327pt;font-size:11.0pt;
  color:black;font-weight:400;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri;background:#D9D9D9;mso-pattern:
  #D9D9D9 none">Open the edit-dialog for a new content-item. <br>
    * contentType<br>
    <br>
    Then it needs either the ID...:<br>
    * entityId<br>
    <br>
    ...or it needs the position within the list:<br>
    * useModuleList: true <br>
    * sortOrder: [number] (important so it knows the position</td>
 </tr>
 <tr height="80" style="height:60.0pt">
  <td height="80" class="xl6522490" align="left" width="130" style="height:60.0pt;
  width:98pt;font-size:11.0pt;color:black;font-weight:400;text-decoration:none;
  text-underline-style:none;text-line-through:none;font-family:Calibri">add</td>
  <td class="xl6522490" align="left" width="84" style="width:63pt;font-size:11.0pt;
  color:black;font-weight:400;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri">Edit</td>
  <td class="xl6522490" align="left" width="436" style="width:327pt;font-size:11.0pt;
  color:black;font-weight:400;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri">Adds a content-item to the
  current list of items, right below the item where it was clicked.<br>
    * useModuleList: true (required to be true for it to work)<br>
    * sortOrder: [number] (important so it knows the position)</td>
 </tr>
 <tr height="160" style="height:120.0pt">
  <td height="160" class="xl6522490" align="left" width="130" style="height:120.0pt;
  width:98pt;font-size:11.0pt;color:black;font-weight:400;text-decoration:none;
  text-underline-style:none;text-line-through:none;font-family:Calibri;
  background:#D9D9D9;mso-pattern:#D9D9D9 none">edit</td>
  <td class="xl6522490" align="left" width="84" style="width:63pt;font-size:11.0pt;
  color:black;font-weight:400;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri;background:#D9D9D9;mso-pattern:
  #D9D9D9 none">Edit</td>
  <td class="xl6522490" align="left" width="436" style="width:327pt;font-size:11.0pt;
  color:black;font-weight:400;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri;background:#D9D9D9;mso-pattern:
  #D9D9D9 none">Opens the edit-dialog. If the item is module-content it may
  also open the presentation-item as well. <br>
    It needs either the ID...:<br>
    * entityId<br>
    <br>
    ...or it needs the position within the list:<br>
    * useModuleList: true <br>
    * sortOrder: [number] (important so it knows the position</td>
 </tr>
 <tr height="20" style="height:15.0pt">
  <td height="20" class="xl6822490" align="left" width="130" style="height:15.0pt;
  width:98pt"><s>dash-view</s></td>
  <td class="xl6522490" align="left" width="84" style="width:63pt;font-size:11.0pt;
  color:black;font-weight:400;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri">-</td>
  <td class="xl6522490" align="left" width="436" style="width:327pt;font-size:11.0pt;
  color:black;font-weight:400;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri">internal, don't use this</td>
 </tr>
 <tr height="40" style="height:30.0pt">
  <td height="40" class="xl6522490" align="left" width="130" style="height:30.0pt;
  width:98pt;font-size:11.0pt;color:black;font-weight:400;text-decoration:none;
  text-underline-style:none;text-line-through:none;font-family:Calibri;
  background:#D9D9D9;mso-pattern:#D9D9D9 none">app-import</td>
  <td class="xl6522490" align="left" width="84" style="width:63pt;font-size:11.0pt;
  color:black;font-weight:400;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri;background:#D9D9D9;mso-pattern:
  #D9D9D9 none">Manage</td>
  <td class="xl6522490" align="left" width="436" style="width:327pt;font-size:11.0pt;
  color:black;font-weight:400;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri;background:#D9D9D9;mso-pattern:
  #D9D9D9 none">Open the app-import dialog to import a new app.<br>
    * [no parameters]</td>
 </tr>
 <tr height="20" style="height:15.0pt">
  <td height="20" class="xl6522490" align="left" width="130" style="height:15.0pt;
  width:98pt;font-size:11.0pt;color:black;font-weight:400;text-decoration:none;
  text-underline-style:none;text-line-through:none;font-family:Calibri">metadata</td>
  <td class="xl6522490" align="left" width="84" style="width:63pt;font-size:11.0pt;
  color:black;font-weight:400;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri">Edit</td>
  <td class="xl6522490" align="left" width="436" style="width:327pt;font-size:11.0pt;
  color:black;font-weight:400;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri">todo - more documentation</td>
 </tr>
 <tr height="80" style="height:60.0pt">
  <td height="80" class="xl6722490" align="left" width="130" style="height:60.0pt;
  width:98pt;font-size:11.0pt;color:#0563C1;font-weight:400;text-decoration:
  underline;text-underline-style:single;text-line-through:none;font-family:
  Calibri;background:#D9D9D9;mso-pattern:#D9D9D9 none"><a href="https://github.com/2sic/2sxc/wiki/Html-Js-Command-Delete">delete</a></td>
  <td class="xl6522490" align="left" width="84" style="width:63pt;font-size:11.0pt;
  color:black;font-weight:400;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri;background:#D9D9D9;mso-pattern:
  #D9D9D9 none">Edit</td>
  <td class="xl6522490" align="left" width="436" style="width:327pt;font-size:11.0pt;
  color:black;font-weight:400;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri;background:#D9D9D9;mso-pattern:
  #D9D9D9 none">delete (not just remove) a content-item. Needs:<br>
    * entityId<br>
    * entityGuid<br>
    * entityTitle</td>
 </tr>
 <tr height="60" style="height:45.0pt">
  <td height="60" class="xl6522490" align="left" width="130" style="height:45.0pt;
  width:98pt;font-size:11.0pt;color:black;font-weight:400;text-decoration:none;
  text-underline-style:none;text-line-through:none;font-family:Calibri">remove</td>
  <td class="xl6522490" align="left" width="84" style="width:63pt;font-size:11.0pt;
  color:black;font-weight:400;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri">List</td>
  <td class="xl6522490" align="left" width="436" style="width:327pt;font-size:11.0pt;
  color:black;font-weight:400;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri">Removes an item from a list of
  items. <br>
    * useModuleList: true (required to be true for it to work)<br>
    * sortOrder: [number] (important so it knows the position)</td>
 </tr>
 <tr height="60" style="height:45.0pt">
  <td height="60" class="xl6522490" align="left" width="130" style="height:45.0pt;
  width:98pt;font-size:11.0pt;color:black;font-weight:400;text-decoration:none;
  text-underline-style:none;text-line-through:none;font-family:Calibri;
  background:#D9D9D9;mso-pattern:#D9D9D9 none">moveup</td>
  <td class="xl6522490" align="left" width="84" style="width:63pt;font-size:11.0pt;
  color:black;font-weight:400;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri;background:#D9D9D9;mso-pattern:
  #D9D9D9 none">List</td>
  <td class="xl6522490" align="left" width="436" style="width:327pt;font-size:11.0pt;
  color:black;font-weight:400;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri;background:#D9D9D9;mso-pattern:
  #D9D9D9 none">Move a content-item up one position in the list<br>
    * useModuleList: true (required to be true for it to work)<br>
    * sortOrder: [number] (important so it knows the position)</td>
 </tr>
 <tr height="60" style="height:45.0pt">
  <td height="60" class="xl6522490" align="left" width="130" style="height:45.0pt;
  width:98pt;font-size:11.0pt;color:black;font-weight:400;text-decoration:none;
  text-underline-style:none;text-line-through:none;font-family:Calibri">movedown</td>
  <td class="xl6522490" align="left" width="84" style="width:63pt;font-size:11.0pt;
  color:black;font-weight:400;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri">List</td>
  <td class="xl6522490" align="left" width="436" style="width:327pt;font-size:11.0pt;
  color:black;font-weight:400;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri">Move a content-item down one
  position in the list<br>
    * useModuleList: true (required to be true for it to work)<br>
    * sortOrder: [number] (important so it knows the position)</td>
 </tr>
 <tr height="40" style="height:30.0pt">
  <td height="40" class="xl6522490" align="left" width="130" style="height:30.0pt;
  width:98pt;font-size:11.0pt;color:black;font-weight:400;text-decoration:none;
  text-underline-style:none;text-line-through:none;font-family:Calibri;
  background:#D9D9D9;mso-pattern:#D9D9D9 none">instance-list</td>
  <td class="xl6522490" align="left" width="84" style="width:63pt;font-size:11.0pt;
  color:black;font-weight:400;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri;background:#D9D9D9;mso-pattern:
  #D9D9D9 none">List</td>
  <td class="xl6522490" align="left" width="436" style="width:327pt;font-size:11.0pt;
  color:black;font-weight:400;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri;background:#D9D9D9;mso-pattern:
  #D9D9D9 none">Open a dialog to manually re-order items in a list.<br>
    (note: in older versions was called "sort"</td>
 </tr>
 <tr height="60" style="height:45.0pt">
  <td height="60" class="xl6522490" align="left" width="130" style="height:45.0pt;
  width:98pt;font-size:11.0pt;color:black;font-weight:400;text-decoration:none;
  text-underline-style:none;text-line-through:none;font-family:Calibri">publish</td>
  <td class="xl6522490" align="left" width="84" style="width:63pt;font-size:11.0pt;
  color:black;font-weight:400;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri">Edit</td>
  <td class="xl6522490" align="left" width="436" style="width:327pt;font-size:11.0pt;
  color:black;font-weight:400;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri">Tells the system to update a
  content-items status to published. If there was a published and a draft
  before, the draft will replace the previous item.<span style="mso-spacerun:yes">&nbsp;</span></td>
 </tr>
 <tr height="60" style="height:45.0pt">
  <td height="60" class="xl6522490" align="left" width="130" style="height:45.0pt;
  width:98pt;font-size:11.0pt;color:black;font-weight:400;text-decoration:none;
  text-underline-style:none;text-line-through:none;font-family:Calibri;
  background:#D9D9D9;mso-pattern:#D9D9D9 none">replace</td>
  <td class="xl6522490" align="left" width="84" style="width:63pt;font-size:11.0pt;
  color:black;font-weight:400;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri;background:#D9D9D9;mso-pattern:
  #D9D9D9 none">Edit Slot</td>
  <td class="xl6522490" align="left" width="436" style="width:327pt;font-size:11.0pt;
  color:black;font-weight:400;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri;background:#D9D9D9;mso-pattern:
  #D9D9D9 none">Only available on module-assigned content items. Will open the
  dialog to assign a different content-item in this slot.<br>
    *…</td>
 </tr>
 <tr height="20" style="height:15.0pt">
  <td height="20" class="xl6522490" align="left" width="130" style="height:15.0pt;
  width:98pt;font-size:11.0pt;color:black;font-weight:400;text-decoration:none;
  text-underline-style:none;text-line-through:none;font-family:Calibri">item-history</td>
  <td class="xl6522490" align="left" width="84" style="width:63pt;font-size:11.0pt;
  color:black;font-weight:400;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri">Versioning</td>
  <td class="xl6522490" align="left" width="436" style="width:327pt;font-size:11.0pt;
  color:black;font-weight:400;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri">Review previous versions of this
  item and restore if necessary.</td>
 </tr>
 <tr height="60" style="height:45.0pt">
  <td height="60" class="xl6522490" align="left" width="130" style="height:45.0pt;
  width:98pt;font-size:11.0pt;color:black;font-weight:400;text-decoration:none;
  text-underline-style:none;text-line-through:none;font-family:Calibri;
  background:#D9D9D9;mso-pattern:#D9D9D9 none">layout</td>
  <td class="xl6522490" align="left" width="84" style="width:63pt;font-size:11.0pt;
  color:black;font-weight:400;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri;background:#D9D9D9;mso-pattern:
  #D9D9D9 none">Design</td>
  <td class="xl6522490" align="left" width="436" style="width:327pt;font-size:11.0pt;
  color:black;font-weight:400;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri;background:#D9D9D9;mso-pattern:
  #D9D9D9 none">Opens the in-page dialog to change the layout of the current
  content.<br>
    * [no parameters needed]</td>
 </tr>
 <tr height="40" style="height:30.0pt">
  <td height="40" class="xl6522490" align="left" width="130" style="height:30.0pt;
  width:98pt;font-size:11.0pt;color:black;font-weight:400;text-decoration:none;
  text-underline-style:none;text-line-through:none;font-family:Calibri">template-develop</td>
  <td class="xl6522490" align="left" width="84" style="width:63pt;font-size:11.0pt;
  color:black;font-weight:400;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri">Develop</td>
  <td class="xl6522490" align="left" width="436" style="width:327pt;font-size:11.0pt;
  color:black;font-weight:400;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri">Opens the template-editor dialog
  in a new window.<br>
    (note: in older versions was called "develop")</td>
 </tr>
 <tr height="40" style="height:30.0pt">
  <td height="40" class="xl6522490" align="left" width="130" style="height:30.0pt;
  width:98pt;font-size:11.0pt;color:black;font-weight:400;text-decoration:none;
  text-underline-style:none;text-line-through:none;font-family:Calibri;
  background:#D9D9D9;mso-pattern:#D9D9D9 none">template-query</td>
  <td class="xl6522490" align="left" width="84" style="width:63pt;font-size:11.0pt;
  color:black;font-weight:400;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri;background:#D9D9D9;mso-pattern:
  #D9D9D9 none">Develop</td>
  <td class="xl6522490" align="left" width="436" style="width:327pt;font-size:11.0pt;
  color:black;font-weight:400;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri;background:#D9D9D9;mso-pattern:
  #D9D9D9 none">Opens the pipeline/query-designer in a new window.<br>
    It's invisible on content, and disabled if no pipeline is configured.</td>
 </tr>
 <tr height="20" style="height:15.0pt">
  <td height="20" class="xl6522490" align="left" width="130" style="height:15.0pt;
  width:98pt;font-size:11.0pt;color:black;font-weight:400;text-decoration:none;
  text-underline-style:none;text-line-through:none;font-family:Calibri">template-settings</td>
  <td class="xl6522490" align="left" width="84" style="width:63pt;font-size:11.0pt;
  color:black;font-weight:400;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri">Develop</td>
  <td class="xl6522490" align="left" width="436" style="width:327pt;font-size:11.0pt;
  color:black;font-weight:400;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri">Change settings on the template
  currently used.</td>
 </tr>
 <tr height="60" style="height:45.0pt">
  <td height="60" class="xl6722490" align="left" width="130" style="height:45.0pt;
  width:98pt;font-size:11.0pt;color:#0563C1;font-weight:400;text-decoration:
  underline;text-underline-style:single;text-line-through:none;font-family:
  Calibri;background:#D9D9D9;mso-pattern:#D9D9D9 none"><a href="https://github.com/2sic/2sxc/wiki/Html-Js-Command-Content-Items">contentitems</a></td>
  <td class="xl6522490" align="left" width="84" style="width:63pt;font-size:11.0pt;
  color:black;font-weight:400;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri;background:#D9D9D9;mso-pattern:
  #D9D9D9 none">Admin</td>
  <td class="xl6522490" align="left" width="436" style="width:327pt;font-size:11.0pt;
  color:black;font-weight:400;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri;background:#D9D9D9;mso-pattern:
  #D9D9D9 none">Opens the dialog to manage content-items for the current
  template. Will use the settings of the current template to open. <br>
    * contentType (optional) - name of data-type to manage/open</td>
 </tr>
 <tr height="40" style="height:30.0pt">
  <td height="40" class="xl6522490" align="left" width="130" style="height:30.0pt;
  width:98pt;font-size:11.0pt;color:black;font-weight:400;text-decoration:none;
  text-underline-style:none;text-line-through:none;font-family:Calibri">contenttype</td>
  <td class="xl6522490" align="left" width="84" style="width:63pt;font-size:11.0pt;
  color:black;font-weight:400;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri">Develop</td>
  <td class="xl6522490" align="left" width="436" style="width:327pt;font-size:11.0pt;
  color:black;font-weight:400;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri">Opens the dialog to view or
  change the current content-type, meaning you can change what fields it has,
  their types etc.</td>
 </tr>
 <tr height="20" style="height:15.0pt">
  <td height="20" class="xl6522490" align="left" width="130" style="height:15.0pt;
  width:98pt;font-size:11.0pt;color:black;font-weight:400;text-decoration:none;
  text-underline-style:none;text-line-through:none;font-family:Calibri;
  background:#D9D9D9;mso-pattern:#D9D9D9 none">app</td>
  <td class="xl6522490" align="left" width="84" style="width:63pt;font-size:11.0pt;
  color:black;font-weight:400;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri;background:#D9D9D9;mso-pattern:
  #D9D9D9 none">Admin</td>
  <td class="xl6522490" align="left" width="436" style="width:327pt;font-size:11.0pt;
  color:black;font-weight:400;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri;background:#D9D9D9;mso-pattern:
  #D9D9D9 none">Opens the app-admin dialog.</td>
 </tr>
 <tr height="40" style="height:30.0pt">
  <td height="40" class="xl6522490" align="left" width="130" style="height:30.0pt;
  width:98pt;font-size:11.0pt;color:black;font-weight:400;text-decoration:none;
  text-underline-style:none;text-line-through:none;font-family:Calibri">app-settings</td>
  <td class="xl6522490" align="left" width="84" style="width:63pt;font-size:11.0pt;
  color:black;font-weight:400;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri">Admin</td>
  <td class="xl6522490" align="left" width="436" style="width:327pt;font-size:11.0pt;
  color:black;font-weight:400;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri">Opens the edit dialog for the
  app-settings. It's disabled if the app doesn't have setting-values to
  configure.<span style="mso-spacerun:yes">&nbsp;</span></td>
 </tr>
 <tr height="60" style="height:45.0pt">
  <td height="60" class="xl6522490" align="left" width="130" style="height:45.0pt;
  width:98pt;font-size:11.0pt;color:black;font-weight:400;text-decoration:none;
  text-underline-style:none;text-line-through:none;font-family:Calibri;
  background:#D9D9D9;mso-pattern:#D9D9D9 none">app-resources</td>
  <td class="xl6522490" align="left" width="84" style="width:63pt;font-size:11.0pt;
  color:black;font-weight:400;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri;background:#D9D9D9;mso-pattern:
  #D9D9D9 none">Admin</td>
  <td class="xl6522490" align="left" width="436" style="width:327pt;font-size:11.0pt;
  color:black;font-weight:400;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri;background:#D9D9D9;mso-pattern:
  #D9D9D9 none">Opens the edit for app-resources (multi-language texts, labels
  etc.). It's disable if the app doesn't have resource-values to
  configure.<span style="mso-spacerun:yes">&nbsp;</span></td>
 </tr>
 <tr height="20" style="height:15.0pt">
  <td height="20" class="xl6522490" align="left" width="130" style="height:15.0pt;
  width:98pt;font-size:11.0pt;color:black;font-weight:400;text-decoration:none;
  text-underline-style:none;text-line-through:none;font-family:Calibri">zone</td>
  <td class="xl6522490" align="left" width="84" style="width:63pt;font-size:11.0pt;
  color:black;font-weight:400;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri">Admin</td>
  <td class="xl6522490" align="left" width="436" style="width:327pt;font-size:11.0pt;
  color:black;font-weight:400;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri">Opens the manage all apps dialog.</td>
 </tr>
 <tr height="40" style="height:30.0pt">
  <td height="40" class="xl6722490" align="left" width="130" style="height:30.0pt;
  width:98pt;font-size:11.0pt;color:#0563C1;font-weight:400;text-decoration:
  underline;text-underline-style:single;text-line-through:none;font-family:
  Calibri;background:#D9D9D9;mso-pattern:#D9D9D9 none"><a href="https://github.com/2sic/2sxc/wiki/Html-Js-Command-Custom-Code">custom</a></td>
  <td class="xl6522490" align="left" width="84" style="width:63pt;font-size:11.0pt;
  color:black;font-weight:400;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri;background:#D9D9D9;mso-pattern:
  #D9D9D9 none">Special</td>
  <td class="xl6522490" align="left" width="436" style="width:327pt;font-size:11.0pt;
  color:black;font-weight:400;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri;background:#D9D9D9;mso-pattern:
  #D9D9D9 none">Execute custom javascript<br>
    * customCode - some JS like "alert('hello');"</td>
 </tr>
 <tr height="40" style="height:30.0pt">
  <td height="40" class="xl6522490" align="left" width="130" style="height:30.0pt;
  width:98pt;font-size:11.0pt;color:black;font-weight:400;text-decoration:none;
  text-underline-style:none;text-line-through:none;font-family:Calibri;
  border-top:none;border-right:none;border-bottom:.5pt solid black;border-left:
  none">more</td>
  <td class="xl6522490" align="left" width="84" style="width:63pt;font-size:11.0pt;
  color:black;font-weight:400;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri;border-top:none;border-right:none;
  border-bottom:.5pt solid black;border-left:none">Ui</td>
  <td class="xl6522490" align="left" width="436" style="width:327pt;font-size:11.0pt;
  color:black;font-weight:400;text-decoration:none;text-underline-style:none;
  text-line-through:none;font-family:Calibri;border-top:none;border-right:none;
  border-bottom:.5pt solid black;border-left:none">Only needed in toolbars,
  creates a "…" button which flips through the menu-buttons.<span style="mso-spacerun:yes">&nbsp;</span></td>
 </tr>
 <!--[if supportMisalignedColumns]-->
 <tr height="0" style="display:none">
  <td width="130" style="width:98pt"></td>
  <td width="84" style="width:63pt"></td>
  <td width="436" style="width:327pt"></td>
 </tr>
 <!--[endif]-->
</tbody></table>









## Beta Features
1. Work in Progress: [ContentItems with Filters](xref:Specs.Js.Commands.ContentItems)
1. Work in Progress: [Delete](xref:Specs.Js.Commands.Delete)
1. Work in Progress: [Code](xref:Specs.Js.Commands.Code)

## Demo App and further links
You should find some code examples in this demo App

* [JS Manage / Toolbar API Tutorial App](http://2sxc.org/en/apps/app/tutorial-for-the-javascript-apis-and-custom-toolbars)
* Blog post about [Calling commands from links](http://2sxc.org/en/blog/post/create-links-which-run-cms-commands-new-2sxc-8-6)

## History
1. Used inside 2sxc since 01.00
2. Official API since 2sxc 08.06
