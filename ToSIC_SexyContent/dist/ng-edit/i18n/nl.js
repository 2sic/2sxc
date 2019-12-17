{
  "Form": {
    "Buttons": {
      "Save": "SAVE (CTRL + S)",
      "Save.Tip": "save and close (CTRL + S saves and doesn't close)",
      "Exit.Tip": "exit - if something changed, you will be asked to save",
      "Return.Tip": "return to previous dialog"
    }
  },
  "SaveMode": {
    "Label": "Status:",
    "show": "show",
    "show.Tip": "changes are public",
    "hide": "hide",
    "hide.Tip": "this item is not publicly visible",
    "branch": "draft",
    "branch.Tip": "changes are only visible to editors",
    "Dialog": {
      "Title": "Save Mode",
      "Intro": "This determines how you will save. The default is show/publish.",
      "Show": {
        "Title": "Show / Publish Everything",
        "Body": "Show changes to the public after you save."
      },
      "Hide": {
        "Title": "Hide Everything",
        "Body": "This item will be hidden and only visible to content editors."
      },
      "Branch": {
        "Title": "Draft / Hide Changes",
        "Body": "Only editors can see changes until published at a later time."
      }
    }
  },
  "Message": {
    "Saved": "opgeslagen",
    "Saving": "opslaan...",
    "DebugEnabled": "debug mode enabled",
    "DebugDisabled": "debug mode disabled",
    "SwitchedLanguageToDefault": "We have switched language to default {{language}} because it's missing some or all values"
  },
  "LangMenu": {
    "Translate": "Translate",
    "TranslateAll": "Translate all",
    "NoTranslate": "Don't translate",
    "NoTranslateAll": "Don't translate any",
    "Link": "Link to other language",
    "UseDefault": "auto (standaard)",
    "InAllLanguages": "in all languages",
    "MissingDefaultLangValue": "please create value in the default language {{languages}} before translating",
    "In": "in {{languages}}",
    "From": "van {{languages}}",
    "Dialog": {
      "Title": "Translate {{name}}",
      "Intro": "You can do many things when translating, like linking languages together.",
      "NoTranslate": {
        "Title": "Don't Translate",
        "Body": "use value in primary language {{primary}}"
      },
      "FromPrimary": {
        "Title": "Translate from: {{primary}}",
        "Body": "begin translation with with the value in the primary language"
      },
      "FromOther": {
        "Title": "Translate from: ...",
        "Body": "begin translation with the value from another language",
        "Subtitle": "Language to translate from"
      },
      "LinkReadOnly": {
        "Title": "Inherit from other language (read-only)",
        "Body": "inherit value from another language",
        "Subtitle": "Language to inherit from"
      },
      "LinkShared": {
        "Title": "Share with another language (read/write)",
        "Body": "link languages together to use the same editable value",
        "Subtitle": "Language to share with"
      },
      "PickLanguageIntro": "Only languages with content can be selected."
    }
  },
  "Errors": {
    "UnsavedChanges": "U hebt niet opgeslagen wijzigingen.",
    "SaveErrors": "To save the form, please fix the following errors:"
  },
  "General": {
    "Buttons": {
      "NotSave": "discard changes",
      "Save": "save",
      "Debug": "debug"
    }
  },
  "Data": {
    "Delete.Question": "delete '{{title}}' ({{id}})?"
  },
  "ItemCard": {
    "DefaultTitle": "Bewerk item",
    "SlotUsedTrue": "dit item is open voor bewerken. Klik hier om te vergrendelen / verwijder het an keer terug naar de standaard.",
    "SlotUsedFalse": "dit item is vergrendeld en zal leeg/standaard blijven. De waarden worden getoond voor uw gemak. Klik hier om te ontgrendelen indien nodig."
  },
  "ValidationMessage": {
    "NotValid": "Not valid",
    "Required": "This is required",
    "RequiredShort": "required",
    "Min": "This value should be more than {{param.Min}}",
    "Max": "This value should be less or equal {{param.Max}}",
    "Pattern": "Please match the requested format",
    "Decimals": "This number can have up to {{param.Decimals}} decimal places"
  },
  "Fields": {
    "Entity": {
      "Choose": "bestaand item toevoegen",
      "New": "maak nieuw",
      "EntityNotFound": "(item niet gevonden)",
      "DragMove": "drag om de lijst te herordenen",
      "Edit": "bewerk dit item",
      "Remove": "verwijder van lijst",
      "Delete": "delete"
    },
    "EntityQuery": {
      "QueryNoItems": "No items found",
      "QueryError": "Error: An error occurred while executing the query. See the console for more information.",
      "QueryStreamNotFound": "Error: The query did not return a stream named "
    },
    "Hyperlink": {
      "Default": {
        "Tooltip": "Drop files here to auto-upload. For help see 2sxc.org/help?tag=adam. ADAM - sponsored with ♥ by 2sic.com",
        "Sponsor": "ADAM - sponsored with ♡ by 2sic.com",
        "Fullscreen": "open in fullscreen",
        "AdamTip": "quick-upload using ADAM",
        "PageTip": "pick a page",
        "MoreOptions": "more...",
        "MenuAdam": "Upload file with Adam",
        "MenuPage": "Page Picker",
        "MenuImage": "Image Manager",
        "MenuDocs": "Document Manager"
      },
      "AdamFileManager": {
        "UploadLabel": "upload to",
        "UploadTip": "quick-upload using ADAM",
        "UploadPasteLabel": "paste image",
        "UploadPasteFocusedLabel": "press ctrl+v",
        "UploadPasteTip": "click here and press [Ctrl]+[V] to paste image from clipboard",
        "NewFolder": "New folder",
        "NewFolderTip": "create a new folder",
        "BackFolder": "Back",
        "BackFolderTip": "return to previous folder",
        "Show": "Open in new tab",
        "Edit": "Rename",
        "RenameQuestion": "Rename file / folder to:",
        "Delete": "Delete",
        "DeleteQuestion": "Are you sure you want to delete this file?",
        "Hint": "drop files here",
        "SponsorTooltip": "ADAM is the Automatic Digital Assets Manager - click to discover more",
        "SponsorLine": "is sponsored with ♥ by"
      },
      "PagePicker": {
        "Title": "Select a web page"
      }
    },
    "DateTime": {
      "Open": "open calendar",
      "Cancel": "Cancel",
      "Set": "Set"
    },
    "String": {
      "Dropdown": "switch to dropdown select",
      "Freetext": "switch to freetext"
    }
  },
  "Extension.TinyMce": {
    "Link.AdamFile": "ADAM-bestand linken (aanbevolen)",
    "Link.AdamFile.Tooltip": "Link via ADAM - drop simpelweg bestanden met de Automatische Digitale Aanhangsel Manager (ADAM)",
    "Image.AdamImage": "ADAM-afbeelding toevoegen (aanbevolen)",
    "Image.AdamImage.Tooltip": "Afbeelding via ADAM- drop simpelweg afbeeldingen met de Automatische Digitale Aanhangsel Manager (ADAM)",
    "Link.DnnFile": "Link DNN-bestand",
    "Link.DnnFile.Tooltip": "Link een DNN-bestand (alle bestanden, langzaam)",
    "Image.DnnImage": "DNN afbeelding toevoegen",
    "Image.DnnImage.Tooltip": "Afbeelding van DNN bestandsopslag alle bestanden, langzaam)",
    "Link.Page": "Link naar een andere pagina",
    "Link.Page.Tooltip": "Link een pagina van de huidige site",
    "Link.Anchor.Tooltip": "Anker naar link gebruik .../pigina#ankernaam",
    "SwitchMode.Pro": "Switch naar geavanceerde mode",
    "SwitchMode.Standard": "Switch naar standaard mode",
    "SwitchMode.Expand": "Fullscreen",
    "H1": "H1",
    "H2": "H2",
    "H3": "H3",
    "H4": "H4",
    "H5": "H5",
    "H6": "H6",
    "ContentBlock.Add": "app of inhoudsblok toevoegen"
  }
}
