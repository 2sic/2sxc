{
  "General": {
    "Buttons": {
      "Add": "add",
      "Refresh": "refresh",
      "System": "advanced system functions",
      "Save": "save",
      "Cancel": "cancel",
      "Permissions": "permissions",
      "Edit": "edit",
      "Delete": "delete",
      "Copy": "copy"
    },
    "Messages": {
      "Loading": "loading...",
      "NothingFound": "no items found",
      "CantDelete": "can't delete {{target}}"
    },
    "Questions": {
      "Delete": "are you sure you want to delete {{target}}?",
      "SystemInput": "This is for very advanced operations. Only use this if you know what you're doing. \n\n Enter admin commands:"
    },
    "Terms": {
      "Title": "title"
    }
  },
  "ContentTypes": {
    "Title": "Content-Types and Data",
    "TypesTable": {
      "Name": "Name",
      "Description": "Description",
      "Fields": "Fields",
      "Items": "Items",
      "Actions": ""
    },
    "TitleExportImport": "Export / Import",
    "Buttons": {
      "Export": "Export",
      "Import": "Import"
    }
  },
  "ContentTypeEdit": {
    "Title": "Edit Content Type",
    "Name": "Name",
    "Description": "Description",
    "Scope": "Scope"
  },
  "Fields": {
    "Title": "Content Type Fields EN",
    "TitleEdit": "Add Fields",
    "Table": {
      "Title": "Title",
      "Name": "Static Name",
      "DataType": "Data Type",
      "Edit": "Edit & Data Type",
      "Label": "Label",
      "InputType": "Input Type",
      "Notes": "Notes",
      "Sort": "Sort",
      "Action": ""
    },
    "General": "General"
  },
  "Permissions": {
    "Title": "Permissions",
    "Table": {
      "Name": "Name",
      "Id": "ID",
      "Condition": "Condition",
      "Grant": "Grant",
      "Actions": ""
    }
  },
  "Pipeline": {
    "Manage": {
      "Title": "Visual Queries / Pipelines",
      "Intro": "Use the visual designer to create queries or merge data from various sources. This can then be used in views or accessed as JSON (if permissions allow). <a href='http://2sxc.org/en/help?tag=visualquerydesigner' target='_blank'>read more</a>",
      "Table": {
        "Id": "ID",
        "Name": "Name",
        "Description": "Description",
        "Actions": ""
      }

    },
    "Designer": {

    }
  },
  "Content": {
    "Manage": {
      "Title": "Manage Content / Data",
      "Table": {
        "Id": "ID",
        "Published": "Publ",
        "Title": "Title",
        "Actions": ""
      },
      "NoTitle": "- no title -"
    },
    "Publish": {
      "PnV": "published and visible",
      "DoP": "this is a draft of another published item",
      "D": "not published at the moment",
      "HD": "has draft: {{id}}",
      "HP": "will replace published: {{id}}"
    },
    "Import": {
      "Title": "Import Content / Data Step {{currentStep}} of 3",
      "Help":  "This will import content-items / data into 2sxc. It requires that you already defined the Content-Type before you try importing, and that you created the import-file using the template provided by the Export. Please visit <a href='http://2sxc.org/help' target='_blank'>http://2sxc.org/help</a> for more instructions.",
      "Fields": {
        "File": {
          "Label": "Choose file"
        },
        "FileReferences": {
          "Label": "References to pages / files",
          "Options": {
            "Keep": "Import links as written in the file (for example /Portals/...)",
            "Resolve": "Try to resolve pathes to references"
          }
        },
        "ClearEntities": {
          "Label": "Clear all other entities",
          "Options": {
            "None": "Keep all entities not found in import",
            "All": "Remove all entities not found in import"
          }
        }
      },
      "Commands": {
        "Preview": "Preview Import",
        "Import": "Import",
        "Back": "Back",
        "Close": "Close"
      },
      "Messages": {
        "BackupContentBefore": "Remember to backup your DNN first!"
      }
    },
    "Export": {
      "Title": "Export Content"
    },
    "History": {
      "Title": "History of {{id}}",
      "Table": {
        "Id": "#",
        "When": "When",
        "User": "User",
        "Actions": ""
      }
    }
  }
}