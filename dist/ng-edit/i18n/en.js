{
	"Button": {
		"Save": "save",
		"Cancel": "cancel",
		"MoreOptions": "more options",
		"SaveAndKeepOpen": "save but keep dialog open"
	},
	"Status": {
		"Published": "published",
		"Unpublished": "unpublished / draft"
	},
	"SaveMode": {
		"show": "show",
		"hide": "hide",
    "branch": "draft",
    "Dialog": {
      "Title": "Save Mode",
      "Intro": "This determines how you will save. The default is show/publish.",
      "Show": {
        "Title": "Show / Publish Everything",
        "Body": "Show item with changes to the public when saving."
      },
      "Hide": {
        "Title": "Hide Everything",
        "Body": "This item will be hidden, and only visible to content editors."
      },
      "Branch": {
        "Title": "Draft / Hide Changes",
        "Body": "Keep original published. Only editors can see changed data until published at a later time."
      }
    }
	},
	"Message": {
		"Saving": "saving...",
		"Saved": "saved",
		"ErrorWhileSaving": "There was an error while saving.",
		"WillPublish": "will publish when saving",
		"WontPublish": "will not publish when saving",
		"PleaseCreateDefLang": "please edit this in the default language first.",
		"ExitOk": "do you really want to exit?",
		"Deleting": "deleting...",
		"Ok": "ok",
		"Error": "error",
		"CantSaveProcessing": "can't save because something else is still being processed",
		"CantSaveInvalid": "form not valid, can't save - errors on: <ul>{0}</ul>",
		"FieldErrorList": "<li>{field} ({error})</li>"
	},
	"Errors": {
		"UnclearError": "Something went wrong - maybe parts worked, maybe not. Sorry :(",
		"InnerControlMustOverride": "Inner control must override this function.",
		"UnsavedChanges": "You have unsaved changes. Are you sure you want to close this dialog?",
		"DefLangNotFound": "Default language value not found, but found multiple values - can't handle editing for",
		"AdamUploadError": "The upload failed. The most likely cause is that the file you were trying to upload is bigger than the maximum upload size."
	},
	"General": {
		"Buttons": {
			"Add": "add",
			"Cancel": "cancel",
			"Copy": "copy",
			"Delete": "delete",
			"Edit": "edit",
			"ForceDelete": "force delete",
			"NotSave": "not save",
			"Permissions": "permissions",
			"Refresh": "refresh",
			"Rename": "rename",
			"Save": "save",
			"System": "advanced system functions",
			"Metadata": "metadata",
			"Export": "export",
			"Import": "import"
		},
		"Questions": {
			"Delete": "are you sure you want to delete {{target}}?",
			"DeleteEntity": "delete '{{title}}' ({{id}})?",
			"Rename": "what new name would you like for {{target}}?",
			"SystemInput": "This is for very advanced operations. Only use this if you know what you're doing. \n\n Enter admin commands:",
			"ForceDelete": "do you want to force delete '{{title}}' ({{id}})?"
		}
	},
	"EditEntity": {
		"DefaultTitle": "Edit item",
		"SlotUsedtrue": "this item is open for editing. Click here to lock / remove it and revert to default.",
		"SlotUsedfalse": "this item is locked and will stay empty/default. The values are shown for your convenience. Click here to unlock if needed."
	},
	"FieldType": {
		"Entity": {
			"Choose": "add existing item",
			"New": "create new",
			"EntityNotFound": "(item not found)",
			"DragMove": "drag to re-order the list",
			"Edit": "edit this item",
			"Remove": "remove from list"
		},
		"EntityQuery": {
			"QueryNoItems": "No items found",
			"QueryError": "Error: An error occurred while executing the query. See the console for more information.",
			"QueryStreamNotFound": "Error: The query did not return a stream named "
		}
	},
	"LangMenu": {
		"UseDefault": "auto (default)",
		"In": "in {{languages}}",
		"From": "from {{languages}}",
		"EditableIn": "editable in {{languages}}",
		"AlsoUsedIn": ", also used in {{more}}",
		"NotImplemented": "This action is not implemented yet.",
		"CopyNotPossible": "Copy not possible: the field is disabled.",
		"Unlink": "translate (unlink)",
		"LinkDefault": "link to default",
		"GoogleTranslate": "machine-translate (Google)",
		"Copy": "copy from",
		"Use": "use from",
		"Share": "share from",
    "AllFields": "all fields",
    "Dialog": {
      "Title": "Translate {{name}}",
      "Intro": "You can do many things when translating, incl. linking languages together.",
      "NoTranslate": {
        "Title": "Don't Translate",
        "Body": "Use value in primary language ({{primary}})"
      },
      "FromPrimary": {
        "Title": "Translate from ({{primary}})",
        "Body": "Translate and start with the value in the primary language"
      },
      "FromOther": {
        "Title": "Translate from ...",
        "Body": "Translate and start with the value from another language",
        "Subtitle": "Language to translate from"
      },
      "LinkReadOnly": {
        "Title": "Inherit to other languege (read-only)",
        "Body": "Inherit value from another language but don't edit it.",
        "Subtitle": "Language to inherit from"
      },
      "LinkShared": {
        "Title": "Share with another language (read/write)",
        "Body": "Link languages together so they use the same value, editable.",
        "Subtitle": "Language to share with"
      }
    }
	},
	"LangWrapper": {
		"CreateValueInDefFirst": "Please create the value for '{{fieldname}}' in the default language before translating it."
	},
	"CalendarPopup": {
		"ClearButton": "Clear",
		"CloseButton": "Done",
		"CurrentButton": "Today"
	},
	"Extension.TinyMce": {
		"Link.AdamFile": "Link ADAM-file (recommended)",
		"Link.AdamFile.Tooltip": "Link using ADAM - just drop files using the Automatic Digital Assets Manager",
		"Image.AdamImage": "Insert ADAM image (recommended)",
		"Image.AdamImage.Tooltip": "Image from ADAM- just drop files using the Automatic Digital Assets Manager",
		"Link.DnnFile": "Link DNN-file",
		"Link.DnnFile.Tooltip": "Link a DNN-file (all files, slow)",
		"Image.DnnImage": "Insert DNN image",
		"Image.DnnImage.Tooltip": "Image from DNN file storage (all files, slow)",
		"Link.Page": "Link to another page",
		"Link.Page.Tooltip": "Link a page from the current site",
		"Link.Anchor.Tooltip": "Anchor to link to using .../page#anchorname",
		"SwitchMode.Pro": "Switch to advanced mode",
		"SwitchMode.Standard": "Switch to standard mode",
		"H1": "H1",
		"H2": "H2",
		"H3": "H3",
		"Remove": "Remove",
		"ContentBlock.Add": "add app or content-block"
	},
	"ValidationMessage": {
		"RequiredShort": "required",
		"NotValid": "not valid",
		"Required": "This field is required",
		"Min": "This value should be more than {{param.Min}}",
		"Max": "This value should be less or equal {{param.Max}}",
		"Pattern": "Please match the requested format",
		"Decimals": "This number can have up to 2 {{param.Decimals}} decimal places"
	},
	"Edit": {
		"Fields": {
			"Hyperlink": {
				"Default": {
					"Tooltip1": "drop files here to auto-upload",
					"Tooltip2": "for help see 2sxc.org/help?tag=adam",
					"Tooltip3": "ADAM - sponsored with love by 2sic.com",
					"AdamUploadLabel": "quick-upload using ADAM",
					"AdamUploadPasteLabel": "click here and press [Ctrl]+[V] to paste image from clipboard",
					"PageLabel": "pick a page",
					"MenuAdam": "Upload file with Adam",
					"MenuPage": "Page Picker",
					"MenuImage": "Image Manager",
					"MenuDocs": "Document Manager",
					"SponsoredLine": "<a href='https://2sxc.org/help?tag=adam' target='_blank' tooltip='ADAM is the Automatic Digital Assets Manager - click to discover more'>Adam</a> is sponsored with ♥ by <a tabindex='-1' href='https://www.2sic.com' target='_blank'>2sic.com</a>"
				},
				"FileManager": {},
				"PagePicker": {
					"Title": "Select a web page"
				}
			}
		}
	}
}
