{
	"General": {
		"Buttons": {
			"Add": "Hinzufügen",
			"Cancel": "Abbrechen",
			"Copy": "Kopieren",
			"Delete": "Löschen",
			"Edit": "Bearbeiten",
			"ForceDelete": "zwangs-löschen",
			"NotSave": "Nicht speichern",
			"Permissions": "Berechtigungen",
			"Refresh": "Aktualisieren",
			"Rename": "Umbenennen",
			"Save": "Speichern",
			"System": "Erweitere System-Funktionen"
		},
		"Messages": {
			"Loading": "wird geladen...",
			"NothingFound": "Keine Datensätze gefunden",
			"CantDelete": "Kann nicht gelöscht werden: {{target}}"
		},
		"Questions": {
			"Delete": "Wirklich löschen? {{target}}",
			"DeleteEntity": "'{{title}}' ({{id}}) löschen?",
			"SystemInput": "Das ist für fortgeschrittene Benutzer. Benutze es nur, wenn du weisst, was du machst. \n\n Befehl eingeben:",
			"ForceDelete": "möchtest du '{{title}}' ({{id}}) zwangs-löschen?"
		},
		"Terms": {
			"Title": "Titel"
		}
	},
	"DataType": {
		"All": {
			"Title": "Allgemeine Einstellungen"
		},
		"Boolean": {
			"Short": "Ja/Nein",
			"ShortTech": "Boolean",
			"Choice": "Boolean (ja/nein)",
			"Explanation": "ja/nein oder true/false Werte"
		},
		"DateTime": {
			"Short": "Datum/Uhrzeit",
			"ShortTech": "DateTime",
			"Choice": "Datum und/oder Zeit",
			"Explanation": "Für Datum, Zeit oder kombinierte Werte"
		},
		"Entity": {
			"Short": "Inhalt(e)",
			"ShortTech": "Entität",
			"Choice": "Entität (andere Inhalte)",
			"Explanation": "Ein oder mehrere andere Inhalte"
		},
		"Hyperlink": {
			"Short": "Link",
			"ShortTech": "Hyperlink",
			"Choice": "Link / Datei Referenz",
			"Explanation": "Link oder Referenz zu einer Datei / Bild"
		},
		"Number": {
			"Short": "Nummer",
			"ShortTech": "Decimal",
			"Choice": "Nummer",
			"Explanation": "Jede Art von Nummer"
		},
		"String": {
			"Short": "Text",
			"ShortTech": "String",
			"Choice": "Text / Zeichenfolge",
			"Explanation": "Jede Art von Text"
		},
		"Empty": {
			"Short": "Leer",
			"ShortTech": "Empty",
			"Choice": "Leer - z.B. für Formular-Gruppierungen",
			"Explanation": "Hilft, das Formular zu strukturieren"
		},
		"Custom": {
			"Short": "Benutzerdefiniert",
			"ShortTech": "Custom",
			"Choice": "Benutzerdefiniert - ui-tools oder benutzerdefinierte Typen",
			"Explanation": "Für Typen wie GPS-Picker (welcher mehrere Felder verwenden) oder für benutzerdefinierte Datenstrukturen wie z.B. ein Array oder ein JSON-Objekt"
		}
	},
	"ContentTypes": {
		"Title": "Inhaltstypen und Daten",
		"TypesTable": {
			"Name": "Name",
			"Description": "Beschreibung",
			"Fields": "Felder",
			"Items": "Inhalte",
			"Actions": ""
		},
		"TitleExportImport": "Export / Import",
		"Buttons": {
			"Export": "Export",
			"Import": "Import",
			"ChangeScope": "Scope ändern",
			"ChangeScopeQuestion": "Das ist eine erweiterte Funktion um Inhaltstypen aus einem anderen Scope anzuzeigen. Benutze es nur wenn du weisst was du machst. Normalerweise sind die Inhaltstypen aus einem anderen Scope versteckt."
		},
		"Messages": {
			"SharedDefinition": "Dieser Inhaltstyp benutzt die Konfiguration von #{{SharedDefId}}, deshalb kann er hier nicht bearbeitet werden - 2sxc.org/help?tag=shared-types",
			"TypeOwn": "this is an own content-type, it does not use the definition of another content-type - read 2sxc.org/help?tag=shared-types",
			"TypeShared": "this content-type inherits the definition of #{{SharedDefId}} - read 2sxc.org/help?tag=shared-types"
		}
	},
	"ContentTypeEdit": {
		"Title": "Inhaltstyp bearbeiten",
		"Name": "Name",
		"Description": "Beschreibung",
		"Scope": "Scope"
	},
	"Fields": {
		"Title": "Inhaltstyp Felder",
		"TitleEdit": "Feld hinzufügen",
		"Table": {
			"Title": "Titel",
			"Name": "Name (statisch)",
			"DataType": "Datentyp",
			"Label": "Label",
			"InputType": "Eingabetyp",
			"Notes": "Notizen",
			"Sort": "Sortierung",
			"Action": ""
		},
		"General": "Allgemein"
	},
	"Permissions": {
		"Title": "Berechtigungen",
		"Table": {
			"Name": "Name",
			"Id": "ID",
			"Condition": "Bedingung",
			"Grant": "Gewährung",
			"Actions": ""
		}
	},
	"Pipeline": {
		"Manage": {
			"Title": "Visuelle Queries / Pipelines",
			"Intro": "Mit dem visuellen Abfragen Designer können benutzerdefinierte Abfragen erstellt und aus verschiedenen Quellen zusammengeführt werden. Dies kann in Vorlagen mit direkten JSON-Abfragen verwendet werden (Wenn die Berechtigung erteilt ist). <a href='http://2sxc.org/en/help?tag=visualquerydesigner' target='_blank'>Mehr Informationen</a>",
			"Table": {
				"Id": "ID",
				"Name": "Name",
				"Description": "Beschreibung",
				"Actions": ""
			}
		},
		"Designer": {},
		"Stats": {
			"Title": "Resultate der Abfrage",
			"Intro": "Die vollständige Abfrage wurde in der Konsole ausgegeben. Weiter unten findest du weitere Debug-Informationen.",
			"ParamTitle": "Parameter & Statistiken",
			"ExecutedIn": "Ausgeführt in {{ms}}ms ({{ticks}} ticks)",
			"QueryTitle": "Resultate",
			"SourcesAndStreamsTitle": "Quellen und Streams",
			"Sources": {
				"Title": "Quellen",
				"Guid": "Guid",
				"Type": "Typ",
				"Config": "Konfiguration"
			},
			"Streams": {
				"Title": "Streams",
				"Source": "Quelle",
				"Target": "Ziel",
				"Items": "Einträge",
				"Error": "Fehler"
			}
		}
	},
	"Content": {
		"Manage": {
			"Title": "Inhalt / Daten verwalten",
			"Table": {
				"Id": "ID",
				"Status": "Status",
				"Title": "Titel",
				"Actions": ""
			},
			"NoTitle": "- kein Titel -"
		},
		"Publish": {
			"PnV": "Publiziert und sichtbar",
			"DoP": "Dies ist ein Entwurf einer anderen, veröffentlichten Version.",
			"D": "Nicht publiziert im Moment",
			"HD": "Hat einen Entwurf: {{id}}",
			"HP": "Ersetzt die publizierte Version"
		},
		"Export": {
			"Title": "Inhalt / Daten exportieren",
			"Help": "Es wird eine xml-Datei erstellt welche im Excel bearbeitet werden kann. Um neue Daten zu importieren kann diese xml-Datei als Schema im Excel verwendet werden. Besuche <a href='http://2sxc.org/help' target='_blank'>http://2sxc.org/help</a> für mehr Informationen.",
			"Commands": {
				"Export": "Export"
			},
			"Fields": {
				"Language": {
					"Label": "Sprachen",
					"Options": {
						"All": "Alle"
					}
				},
				"LanguageReferences": {
					"Label": "Werte, die auf andere Sprachen verweisen",
					"Options": {
						"Link": "Verweise auf andere Sprachen behalten (für Re-Import)",
						"Resolve": "Referenzen mit Werten ersetzen"
					}
				},
				"ResourcesReferences": {
					"Label": "Datei / Seitenverweise",
					"Options": {
						"Link": "Verweise beibehalten (für Re-Import, z.B. Page:4711)",
						"Resolve": "Verweise mit entsprechender URL ersetzen (z.B. /Portals/0...)"
					}
				},
				"RecordExport": {
					"Label": "Daten exportieren",
					"Options": {
						"Blank": "Nein, nur Schema exportieren (für den Import von neuen Datensätzen)",
						"All": "Ja, alle Inhalte exportieren",
						"Selection": "Nur ausgewählte {{count}} Elemente"
					}
				}
			}
		},
		"Import": {
			"Title": "Inhalt / Daten importieren",
			"TitleSteps": "{{step}} von 3",
			"Help": "Importiert Datensätze in 2sxc. Stelle sicher, dass der Datentyp bereits definiert ist, und dass das XML vorgängig mit dem Export generiert wurde. Für weitere Infos: <a href='http://2sxc.org/help' target='_blank'>http://2sxc.org/help</a>.",
			"Fields": {
				"File": {
					"Label": "Datei auswählen"
				},
				"ResourcesReferences": {
					"Label": "Verweise auf Seiten / Dateien",
					"Options": {
						"Keep": "Links wie in der Import-Datei angegeben importieren (z.B. /Portals/...)",
						"Resolve": "Versuche, die Verweise aufzulösen"
					}
				},
				"ClearEntities": {
					"Label": "Alle anderen Datensätze entfernen",
					"Options": {
						"None": "Behalte die Datensätze, die im Import nicht enthalten sind",
						"All": "Entferne die Datensätze, die im Import nicht enthalten sind"
					}
				}
			},
			"Commands": {
				"Preview": "Vorschau des Imports",
				"Import": "Import"
			},
			"Messages": {
				"BackupContentBefore": "Stelle sicher, dass du ein Backup der Datenbank hast, bevor du fortfährst!",
				"WaitingForResponse": "Bitte warten...",
				"ImportSucceeded": "Import fertiggestellt.",
				"ImportFailed": "Import fehlgeschlagen.",
				"ImportCanTakeSomeTime": "Hinweis: Der Import kann mehrere Minuten dauern."
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
			"Title": "Versionen von {{id}}",
			"Table": {
				"Id": "#",
				"When": "Wann",
				"User": "Benutzer",
				"Actions": ""
			}
		}
	},
	"AdvancedMode": {
		"Info": {
			"Available": "Dieser Dialog einen erweiterten / Debug Modus für Power-Users – erfahre mehr 2sxc.org/help?tag=debug-mode",
			"TurnOn": "erweiterten / Debug Modus ein",
			"TurnOff": "erweiterten / Debug Modus aus"
		}
	}
}