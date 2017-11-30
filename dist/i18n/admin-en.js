{
	"General": {
		"Note2Translator": "these are the main keys for buttons and short messages, used in various dialogs",
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
			"Metadata": "metadata"
		},
		"Messages": {
			"Loading": "loading...",
			"NothingFound": "no items found",
			"CantDelete": "can't delete {{target}}"
		},
		"Questions": {
			"Delete": "are you sure you want to delete {{target}}?",
			"DeleteEntity": "delete '{{title}}' ({{id}})?",
			"Rename": "what new name would you like for {{target}}?",
			"SystemInput": "This is for very advanced operations. Only use this if you know what you're doing. \n\n Enter admin commands:",
			"ForceDelete": "do you want to force delete '{{title}}' ({{id}})?"
		},
		"Terms": {
			"Title": "title"
		}
	},
	"DataType": {
		"All": {
			"Title": "General Settings"
		},
		"Boolean": {
			"Short": "yes/no",
			"ShortTech": "Boolean",
			"Choice": "Boolean (yes/no)",
			"Explanation": "Yes/no or true/false values"
		},
		"DateTime": {
			"Short": "date/time",
			"ShortTech": "DateTime",
			"Choice": "Date and/or time",
			"Explanation": "for date, time or combined values"
		},
		"Entity": {
			"Short": "item(s)",
			"ShortTech": "Entity",
			"Choice": "Entity (other content-items)",
			"Explanation": "one or more other content-items"
		},
		"Hyperlink": {
			"Short": "link",
			"ShortTech": "Hyperlink",
			"Choice": "Link / file reference",
			"Explanation": "hyperlink or reference to a picture / file"
		},
		"Number": {
			"Short": "number",
			"ShortTech": "Decimal",
			"Choice": "Number",
			"Explanation": "any kind of number"
		},
		"String": {
			"Short": "text",
			"ShortTech": "String",
			"Choice": "Text / string",
			"Explanation": "any kind of text"
		},
		"Empty": {
			"Short": "empty",
			"ShortTech": "Empty",
			"Choice": "Empty - for form-titles etc.",
			"Explanation": "use to structure your form"
		},
		"Custom": {
			"Short": "custom",
			"ShortTech": "Custom",
			"Choice": "Custom - ui-tools or custom types",
			"Explanation": "use for things like gps-pickers (which writes into multiple fields) or for custom-data which serializes something exotic into the db like an array, a custom json or anything "
		}
	},
	"ContentTypes": {
		"Title": "Content-Types and Data",
		"TypesTable": {
			"Name": "Name",
			"Description": "Description",
			"Fields": "Fields",
			"Items": "Items",
			"Actions": "Actions"
		},
		"TitleExportImport": "Export / Import",
		"Buttons": {
			"Export": "export",
			"Import": "import",
			"ChangeScope": "change scope",
			"ChangeScopeQuestion": "This is an advanced feature to show content-types of another scope. Don't use this if you don't know what you're doing, as content-types of other scopes are usually hidden for a good reason."
		},
		"Messages": {
			"SharedDefinition": "this content-type shares the definition of #{{SharedDefId}} so you can't edit it here - read 2sxc.org/help?tag=shared-types",
			"TypeOwn": "this is an own content-type, it does not use the definition of another content-type - read 2sxc.org/help?tag=shared-types",
			"TypeShared": "this content-type inherits the definition of #{{SharedDefId}} - read 2sxc.org/help?tag=shared-types"
		}
	},
	"ContentTypeEdit": {
		"Title": "Edit Content Type",
		"Name": "Name",
		"Description": "Description",
		"Scope": "Scope"
	},
	"Fields": {
		"Title": "Content Type Fields",
		"TitleEdit": "Add Fields",
		"Table": {
			"Title": "Title",
			"Name": "Static Name",
			"DataType": "Data Type",
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
			"Actions": "Actions"
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
		"Designer": {},
		"Stats": {
			"Title": "Query Results",
			"Intro": "The Full result was logged to the Browser Console. Further down you'll find more debug-infos.",
			"ParamTitle": "Parameters & Statistics",
			"ExecutedIn": "Executed in {{ms}}ms ({{ticks}} ticks)",
			"QueryTitle": "Query Results",
			"SourcesAndStreamsTitle": "Sources and Streams",
			"Sources": {
				"Title": "Sources",
				"Guid": "Guid",
				"Type": "Type",
				"Config": "Configuration"
			},
			"Streams": {
				"Title": "Streams",
				"Source": "Source",
				"Target": "Target",
				"Items": "Items",
				"Error": "Error"
			}
		}
	},
	"Content": {
		"Manage": {
			"Title": "Manage Content / Data",
			"Table": {
				"Id": "ID",
				"Status": "Status",
				"Title": "Title"
			},
			"NoTitle": "- no title -"
		},
		"Publish": {
			"PnV": "published and visible",
			"DoP": "this is a draft of another published item",
			"D": "not published at the moment",
			"HD": "has draft: {{id}}",
			"HP": "will replace published"
		},
		"Export": {
			"Title": "Export Content / Data",
			"Help": "This will generate an XML file which you can edit in Excel. If you just want to import new data, use this to export the schema that you can then fill in using Excel. Please visit <a href='http://2sxc.org/help' target='_blank'>http://2sxc.org/help</a> for more instructions.",
			"Commands": {
				"Export": "Export"
			},
			"Fields": {
				"Language": {
					"Label": "Languages",
					"Options": {
						"All": "All"
					}
				},
				"LanguageReferences": {
					"Label": "Value references to other languages",
					"Options": {
						"Link": "Keep references to other languages (for re-import)",
						"Resolve": "Replace references with values"
					}
				},
				"ResourcesReferences": {
					"Label": "File / page references",
					"Options": {
						"Link": "Keep references (for re-import, for example Page:4711)",
						"Resolve": "Replace references with real URLs (for example /Portals/0...)"
					}
				},
				"RecordExport": {
					"Label": "Export data",
					"Options": {
						"Blank": "No, just export blank data schema (for new data import)",
						"All": "Yes, export all content-items",
						"Selection": "Export selected {{count}} items"
					}
				}
			}
		},
		"Import": {
			"Title": "Import Content / Data Step",
			"TitleSteps": "{{step}} of 3",
			"Help": "This will import content-items into 2sxc. It requires that you already defined the content-type before you try importing, and that you created the import-file using the template provided by the Export. Please visit <a href='http://2sxc.org/help' target='_blank'>http://2sxc.org/help</a> for more instructions.",
			"Fields": {
				"File": {
					"Label": "Choose file"
				},
				"ResourcesReferences": {
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
				"Import": "Import"
			},
			"Messages": {
				"BackupContentBefore": "Remember to backup your DNN first!",
				"WaitingForResponse": "Please wait while processing...",
				"ImportSucceeded": "Import done.",
				"ImportFailed": "Import failed.",
				"ImportCanTakeSomeTime": "Note: The import validates much data and may take several minutes."
			},
			"Evaluation": {
				"Error": {
					"Title": "Try to import file '{{filename}}'",
					"Codes": {
						"0": "Unknown error occured.",
						"1": "Selected content-type does not exist.",
						"2": "Document is not a valid XML file.",
						"3": "Selected content-type does not match the content-type in the XML file.",
						"4": "The language is not supported.",
						"5": "The document does not specify all languages for all entities.",
						"6": "Language reference cannot be parsed, the language is not supported.",
						"7": "Language reference cannot be parsed, the read-write protection is not supported.",
						"8": "Value cannot be read, because of it has an invalid format."
					},
					"Detail": "Details: {{detail}}",
					"LineNumber": "Line-no: {{number}}",
					"LineDetail": "Line-details: {{detail}}"
				},
				"Detail": {
					"Title": "Try to import file '{{filename}}'",
					"File": {
						"Title": "File contains:",
						"ElementCount": "{{count}} content-items (records/entities)",
						"LanguageCount": "{{count}} languages",
						"Attributes": "{{count}} columns: {{attributes}}"
					},
					"Entities": {
						"Title": "If you press Import, it will:",
						"Create": "Create {{count}} content-items",
						"Update": "Update {{count}} content-items",
						"Delete": "Delete {{count}} content-items",
						"AttributesIgnored": "Ignore {{count}} columns: {{attributes}}"
					}
				}
			}
		},
		"History": {
			"Title": "History of {{id}}",
			"Table": {
				"Id": "#",
				"When": "When",
				"User": "User"
			}
		}
	},
	"AdvancedMode": {
		"Info": {
			"Available": "this dialog has an advanced / debug mode for power-users - read more 2sxc.org/help?tag=debug-mode",
			"TurnOn": "advanced / debug mode on",
			"TurnOff": "advanced / debug mode off"
		}
	}
}