{
  "Form": {
    "Buttons": {
      "Save": "Save (CTRL + S)",
      "SaveAndClose": "Save and close",
      "Exit.Tip": "Exit - if something changed, you will be asked to save",
      "Return.Tip": "Return to previous dialog",
      "History.Tip": "History / previous versions",
      "Metadata.Tip": "This item is metadata for:",
      "Note": {
        "Add": "Add note",
        "ItemNotSaved": "To add a note, please save item first"
      }
    }
  },
  "PublishStatus": {
    "Label": "Status:",
    "show": "show",
    "show.Tip": "Changes are public",
    "hide": "hide",
    "hide.Tip": "This item is not publicly visible",
    "branch": "draft",
    "branch.Tip": "Changes are only visible to editors",
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
    "Saved": "Enregistré",
    "Saving": "En cours d'enregistrement...",
    "Deleted": "Deleted",
    "Deleting": "Deleting...",
    "DeleteError": "Delete failed. Please check console for more information",
    "SwitchedLanguageToDefault": "We have switched language to default {{language}} because it's missing some or all values",
    "CantSwitchLanguage": "Can't switch languages until current language has all required values"
  },
  "LangMenu": {
    "Translate": "Translate",
    "TranslateAll": "Translate all",
    "NoTranslate": "Don't translate",
    "NoTranslateAll": "Don't translate any",
    "Link": "Link to other language",
    "UseDefault": "auto (par défaut)",
    "InAllLanguages": "in all languages",
    "MissingDefaultLangValue": "please create value in the default language {{languages}} before translating",
    "In": "dans {{languages}}",
    "From": "de {{languages}}",
    "Dialog": {
      "Title": "Translate {{name}}",
      "Intro": "You can do many things when translating, like linking languages together.",
      "NoTranslate": {
        "Title": "Don't Translate",
        "Body": "Use value in primary language {{primary}}"
      },
      "FromPrimary": {
        "Title": "Translate from: {{primary}}",
        "Body": "Begin translation with with the value in the primary language"
      },
      "FromOther": {
        "Title": "Translate from: ...",
        "Body": "Begin translation with the value from another language",
        "Subtitle": "Language to translate from"
      },
      "LinkReadOnly": {
        "Title": "Inherit from other language (read-only)",
        "Body": "Inherit value from another language",
        "Subtitle": "Language to inherit from"
      },
      "LinkShared": {
        "Title": "Share with another language (read/write)",
        "Body": "Link languages together to use the same editable value",
        "Subtitle": "Language to share with"
      },
      "PickLanguageIntro": "Only languages with content can be selected."
    }
  },
  "Errors": {
    "UnsavedChanges": "Il y a des données non enregistrées.",
    "SaveErrors": "To save the form, please fix the following errors:",
    "FormulaConfiguration": "There is an error in form configuration. Please report this to admin",
    "FormulaCalculation": "There was an error in form calculations. Please report this to admin"
  },
  "General": {
    "Buttons": {
      "NotSave": "Discard changes",
      "Save": "Save",
      "Cancel": "Cancel"
    },
    "CopyHint": "This is a copy and will be saved as a new item",
    "ReadOnlyHint": {
      "Form": "Form is read only",
      "Language": "Language is read only"
    }
  },
  "Data": {
    "Delete.Question": "Delete '{{title}}' ({{id}})?"
  },
  "ItemCard": {
    "DefaultTitle": "Mise a jour d'item",
    "SlotUsedTrue": "Cet item est en mode modif. Click here to lock / remove it and revert to default.",
    "SlotUsedFalse": "Cet item est verrouillé et restera vide ou par défault. Les valeurs sont montrées a titre indicatif. Cliquer ici pour déverrouiller si nécessaire."
  },
  "ValidationMessage": {
    "NotValid": "Not valid",
    "Required": "This is required",
    "RequiredShort": "required",
    "Min": "This value should be {{param.Min}} or higher",
    "Max": "This value should be {{param.Max}} or lower",
    "Pattern": "Please match the requested format",
    "Decimals": "This number can have up to {{param.Decimals}} decimal places",
    "JsonError": "JSON is not valid",
    "JsonWarning": "JSON is not valid"
  },
  "Fields": {
    "Entity": {
      "Choose": "Choix de l'item a ajouter",
      "New": "Nouveau",
      "Empty": "Empty",
      "EmptySlot": "empty slot",
      "EntityNotFound": "(item non trouvé)",
      "DragMove": "Réordonner la liste a la souris",
      "Edit": "Mettre a jour cet item",
      "Remove": "Supprimer de la liste",
      "Delete": "Delete",
      "Loading": "Loading...",
      "Search": "Search"
    },
    "EntityQuery": {
      "QueryNoItems": "No items found",
      "QueryError": "Error: An error occurred while executing the query. See the console for more information.",
      "QueryStreamNotFound": "Error: The query did not return a stream named"
    },
    "Hyperlink": {
      "Default": {
        "Tooltip": "Drop files here to auto upload. For help see 2sxc.org/help?tag=adam. ADAM - sponsored with ♡ by 2sic.com",
        "Sponsor": "ADAM - sponsored with ♡ by 2sic.com",
        "Fullscreen": "Open in fullscreen",
        "AdamTip": "Quick upload using ADAM",
        "PageTip": "Pick a page",
        "MoreOptions": "More...",
        "MenuAdam": "Upload file with Adam",
        "MenuPage": "Page Picker",
        "MenuImage": "Image Manager",
        "MenuDocs": "File Manager"
      },
      "AdamFileManager": {
        "Name": "ADAM",
        "UploadLabel": "Upload to",
        "UploadTip": "Quick upload using ADAM",
        "UploadPasteLabel": "Paste Image",
        "UploadPasteFocusedLabel": "Press CTRL + V",
        "UploadPasteTip": "Click here and press CTRL + V to paste image from clipboard",
        "NewFolder": "New Folder",
        "NewFolderTip": "Create a new folder",
        "BackFolder": "Back",
        "BackFolderTip": "Return to previous folder",
        "Show": "Open in new tab",
        "ImageSettings": "Image settings",
        "ImageSettingsUnavailable": "Image settings not available. The file is not an image or it doesn't belong to this item",
        "ImageSettingsDisabled": "Image settings are disabled for this field",
        "Edit": "Rename",
        "RenameQuestion": "Rename file / folder to:",
        "Delete": "Delete",
        "DeleteQuestion": "Are you sure you want to delete this file?",
        "Hint": "drop files here",
        "HelpTooltip": "ADAM is the Automatic Digital Assets Manager - click to discover more",
        "SponsorLine": "is sponsored with ♡ by"
      },
      "PagePicker": {
        "Title": "Select a web page"
      }
    },
    "DateTime": {
      "Open": "Open calendar"
    },
    "String": {
      "Dropdown": "Pick existing value",
      "Freetext": "Manual entry"
    },
    "TemplatePicker": {
      "NotSelected": "(no file selected)",
      "NewTemplate": "Create a new file"
    }
  },
  "ManageContentList": {
    "Title": "Manage content-item lists",
    "Description": "You can manage the list header here (if it is defined):",
    "NoHeader": "(this list has no header)",
    "SortItems": "Sort the items by dragging as you need, then save:",
    "SortLotsOfItems": "Sort the items by dragging and scrolling with mouse wheel as you need, then save:"
  },
  "Extension.TinyMce": {
    "Link.AdamFile": "Lien via ADAM fichier (recommandé)",
    "Link.AdamFile.Tooltip": "Lien utilisant ADAM - déposez simplement les fichiers a l'aide du gestionnaire automatique des actifs numériques",
    "Image.AdamImage": "Insérer l'image via ADAM (recommandé)",
    "Image.AdamImage.Tooltip": "Image via ADAM - déposez simplement des fichiers a l'aide du gestionnaire automatique d'actifs numériques",
    "Link.DnnFile": "Lien via DNN fichiers",
    "Link.DnnFile.Tooltip": "Lier un fichier via DNN (tous les fichiers, lent)",
    "Image.DnnImage": "Insérer une image via DNN (lent)",
    "Image.DnnImage.Tooltip": "Image via le gestionnaire de fichiers DNN (tous les fichiers, lent)",
    "Link.Page": "Lien vers une autre page",
    "Link.Page.Tooltip": "Lien vers une page du site actuel",
    "Link.Anchor.Tooltip": "Ancre pour lier a l'utilisation .../page#nomdelancre",
    "SwitchMode.Pro": "Passer en mode avancé",
    "SwitchMode.Standard": "Passer en mode standard",
    "SwitchMode.Expand": "Fullscreen",
    "H1": "H1",
    "H2": "H2",
    "H3": "H3",
    "H4": "H4",
    "H5": "H5",
    "H6": "H6",
    "Paragraph": "Paragraphe",
    "ContentBlock.Add": "Ajouter une application ou un content block"
  }
}
