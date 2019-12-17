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
    "Saved": "guardado",
    "Saving": "guardando...",
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
    "UseDefault": "auto (predeterminado)",
    "InAllLanguages": "in all languages",
    "MissingDefaultLangValue": "please create value in the default language {{languages}} before translating",
    "In": "en {{languages}}",
    "From": "desde {{languages}}",
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
    "UnsavedChanges": "Tiene cambios sin guardar.",
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
    "DefaultTitle": "Editar elemento",
    "SlotUsedTrue": "este elemento está abierto para edición. Pulse aquí para bloquearlo / eliminarlo y devolverlo a predeterminado.",
    "SlotUsedFalse": "este elemento está bloqueado y permanecerá vacío/predeterminado. Los valores se muestran a su conveniencia. Pulse aquí para desbloquearlo si es necesario."
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
      "Choose": "-- elija elemento para añadir --",
      "New": "-- crear nuevo --",
      "EntityNotFound": "(elemento no encontrado)",
      "DragMove": "arrastre para reordenar la lista",
      "Edit": "edite este elemento",
      "Remove": "quitar de la lista",
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
    "Link.AdamFile": "Enlace a archivo ADAM (recomendado)",
    "Link.AdamFile.Tooltip": "Enlace a archivo ADAM – solo arrastre y suelte los archivos utilizando el Automatic Digital Assets Manager",
    "Image.AdamImage": "Insertar una imagen ADAM (recomendado)",
    "Image.AdamImage.Tooltip": "Insertar una imagen ADAM- solo arrastre y suelte los archivos utilizando el Automatic Digital Assets Manager",
    "Link.DnnFile": "Enlace a Archivo de DNN",
    "Link.DnnFile.Tooltip": "Enlace a archivo de DNN (todos los archivos, lento)",
    "Image.DnnImage": "Insertar imagen de DNN",
    "Image.DnnImage.Tooltip": "Imagen de archivos almacenados en DNN (todos los archivos, lento)",
    "Link.Page": "Enlace a otra página",
    "Link.Page.Tooltip": "Enlace a otra página del sitio actual",
    "Link.Anchor.Tooltip": "Ancla a enlace para utilizar como .../page#anchorname",
    "SwitchMode.Pro": "Cambiar a modo avanzado",
    "SwitchMode.Standard": "Cambiar a modo estándar",
    "SwitchMode.Expand": "Fullscreen",
    "H1": "H1",
    "H2": "H2",
    "H3": "H3",
    "H4": "H4",
    "H5": "H5",
    "H6": "H6",
    "ContentBlock.Add": "add app or content-block"
  }
}
