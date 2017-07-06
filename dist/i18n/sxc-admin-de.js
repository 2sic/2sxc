{
	"Main": {
		"Title": "Administration",
		"Tab": {
			"GS": "Home",
			"GettingStarted": "Erste Schritte",
			"CD": "Daten",
			"ContentData": "Datentypen / Daten",
			"VT": "Vorlagen",
			"ViewsTemplates": "Vorlagen",
			"Q": "Query",
			"Query": "Query Designer",
			"WA": "WebApi",
			"WebApi": "WebApi / Data",
			"IE": "",
			"ImportExport": "Import / Export",
			"PL": "Global",
			"PortalLanguages": "Website / Sprachen",
			"AS": "App",
			"AppSettings": "App Einstellungen"
		},
		"Portal": {
			"Title": "Globale Einstellungen",
			"Intro": "Diese Einstellungen gelten für alle Inahlte und Apps auf dieser Website (Portal)."
		}
	},
	"Templates": {
		"Title": "Vorlagen verwalten",
		"InfoHideAdvanced": "Die Benutzerfreundlichkeit der Benutzer welche die Website bearbeiten kann verbessert werden indem die Einstellungen für fortgeschrittene Benutzer ausgeblendet werden. Diese Benutzer können der Benutzergruppe “2sxc designers“ zugewiesen werden. <a href='http://2sxc.org/en/help?tag=hide-advanced' target='_blank'>Dokumentation</a>",
		"Table": {
			"TName": "Vorlage",
			"TPath": "Pfad",
			"CType": "Datentypen",
			"DemoC": "Demo Eintrag",
			"Show": "Sichtbar",
			"UrlKey": "Url key",
			"Actions": "Aktionen"
		}
	},
	"TemplateEdit": {
		"Title": "Vorlage bearbeiten"
	},
	"WebAPIData": {
		"unused": {
			"Title": "Get content as if it were data for your template or JavaScript",
			"Intro": "Use the query designer to create complex queries. They can be used in normal templates (Token/Razor) or in JavaScript. Or you can create your custom JSON-WebAPI which is only for JavaScript - for example to save something, retrieve a file or perform a query too complex for the designer. Read more about <a href='http://2sxc.org/en/Docs/tag/Data%20and%20Data%20Sources' target='_blank'>coding data pipelines</a> and about <a href='http://2sxc.org/en/Docs/tag/Data%20and%20Data%20Sources' target='_blank'>using the data in JavaScript with jQuery, AngularJS and more</a>.",
			"Visual": {
				"Title": "Visual Data Query",
				"Intro": "Use the Visual Query Designer (Pipeline-Designer) to create queries to data from 2sxc, SQL, RSS and more.",
				"Button": "Visual Query Designer"
			}
		}
	},
	"WebApi": {
		"Title": "WebApi für diese App",
		"Intro": "Erstelle eine WebApi in wenigen Minuten. Implementiere den Quellcode im Ordner “API“. Mehr Informationen über <a href='http://2sxc.org/en/Docs-Manuals/Feature/feature/3361' target='_blank'>WebApi</a> oder über <a href='http://2sxc.org/en/Docs-Manuals/Feature/feature/3360' target='_blank'>C# data editing API</a>.",
		"ListTitle": "Die folgende Liste zeigt die cs-Dateien im AppApi Ordner:",
		"InfoMissingFolder": "(der Ordner existiert nicht)",
		"QuickStart": "Für einen guten Start empfehlen wir die WebApi Demo-App zu installieren. Die Demo-App beinhaltet einige WebApi Kontroller mit verschiedenen Aktionen und Beispiel Vorlagen um die Kontroller zu verwenden. Lade die <a href='http://2sxc.org/en/Apps/tag/WebApi' target='_blank'>WebApi Demos im App-Katalog</a> oder erfahre mehr unter <a href='http://2sxc.org/en/help?tag=webapi' target='_blank'>Hilfe</a>",
		"AddDoesntExist": "Automatisches Hinzufügen nicht möglich – Bitte erstelle manuell eine cs-Datei im Ordner “API“ oder kopiere ein existierendes Projekt."
	},
	"ImportExport": {
		"Title": "Exportiere oder importiere <em>einzelne Elemente</em> aus Content/App",
		"Intro": "Erstelle eine xml- oder zip-Datei mit <em>einzelnen Elementen</em> aus dieser App oder aus diesem Content. Oder importiere ein solches Paket.",
		"FurtherHelp": "Für mehr Informationen besuche die <a href='http://2sxc.org/en/help?tag=import' target='_blank'>2sxc Hilfe</a>.",
		"Buttons": {
			"Import": "Importieren",
			"Export": "Exportieren"
		},
		"Import": {
			"Title": "Importiere ein Content-Export (.zip) oder ein Teil-Export (.xml)",
			"Explanation": "Dieser Import wird Datentypen, Vorlagen und Inhalte zum aktuellen Content oder zur aktuellen App hinzufügen.",
			"Select": "Bitte wähle die zu importierende Datei.",
			"Choose": "Datei wählen"
		},
		"Export": {
			"Title": "Teil-Export von Datentypen, Vorlagen und Inhalte",
			"Intro": "Dies ist ein fortgeschrittener Export um einzelne Elemente aus dieser App oder aus diesem Content zu exportieren. Es wird eine xml-Datei erstellt welche auf einer anderen Website oder in eine andere App importiert werden kann.",
			"FurtherHelp": "Für mehr Informationen besuche die <a href='http://2sxc.org/en/help?tag=export' target='_blank'>2sxc Hilfe</a>.",
			"Data": {
				"GroupHeading": "Datentyp: {{name}} ({{id}}",
				"Templates": "Vorlage",
				"Items": "Inhalte",
				"SimpleTemplates": "Vorlagen ohne Datentyp"
			},
			"ButtonExport": "Exportieren"
		}
	},
	"Portal": {
		"Title": "Virtuelle Datenbank (VDB)",
		"VdbLabel": "Virtuelle Datenbank für diese Website (Portal)",
		"Rename": "Note to 2tk - rename is not necessary any more, don't implement!"
	},
	"Language": {
		"Title": "Sprache / Lokalisierung",
		"Intro": "Verwalte die aktiven / inaktiven Sprachen von diese Webseite (Portal)",
		"Table": {
			"Code": "Sprachcode",
			"Culture": "Lokalisierung / Kultur",
			"Status": "Status"
		}
	},
	"AppConfig": {
		"Title": "App Konfiguration",
		"Intro": "Konfiguriere die App und spezielle App-Einstellungen.",
		"Settings": {
			"Title": "App Einstellungen",
			"Intro": "App Einstellungen sind Konfigurationen welche innerhalb einer App verwendet werden können. Z.B. SQL-Verbindungen oder Standard-Einstellungen wie “Anzahl anzuzeigende Inhalte“. Bei diesen Feldern wird die Mehrsprachigkeit unterstützt, eine Einstellung (z.B. Standard RSS-Feed) kann für jede Sprache einen anderen Wert haben.",
			"Edit": "App Einstellungen bearbeiten",
			"Config": "App Einstellungen konfigurieren"
		},
		"Resources": {
			"Title": "App Ressourcen",
			"Intro": "App Ressourcen können für Labels und andere Beschriftungen innerhalb einer App verwendet werden. Hauptsächlich sind sie für Mehrsprachige Vorlagen gedacht und sollten nicht für App-Einstellungen verwendet werden.",
			"Edit": "App Ressourcen bearbeiten",
			"Config": "App Ressourcen konfigurieren"
		},
		"Definition": {
			"Title": "App Paket Definitionen",
			"Intro": "Die App Paket Definitionen sind wichtig wenn das App exportiert/importiert wird.",
			"Edit": "App Paket Definitionen bearbeiten"
		},
		"Export": {
			"Title": "Diese App <em>vollständig</em> exportieren",
			"Intro": "Erstelle ein App-Paket (.zip) welches auf einer anderen Website (Portal) installiert werden kann.",
			"Button": "export"
		}
	},
	"AppManagement": {
		"Title": "Apps in dieser Zone (Portal) verwalten",
		"Table": {
			"Name": "Name",
			"Folder": "Ordner",
			"Templates": "Vorlagen",
			"Show": "Diese App den Benutzern anbieten",
			"Actions": "Aktionen"
		},
		"Buttons": {
			"Browse": "Mehr Apps",
			"Import": "App importieren",
			"Create": "App erstellen",
			"Export": "App exportieren"
		},
		"Prompt": {
			"NewApp": "Gib ein App Name ein (wird auch für den App-Ordner verwendet)",
			"DeleteApp": "Kann nicht rückgängig gemacht werden. Wenn die App wirklich gelöscht werden soll, tippe den App Namen in das Feld (oder per Copy-and-Paste). Soll die App '{{name}}' (ID: {{id}}) wirklich gelöscht werden?",
			"FailedDelete": "Die Eingabe stimmt nicht überein – App wird nicht gelöscht."
		}
	},
	"ReplaceContent": {
		"Title": "Inhalt ersetzen",
		"Intro": "Wenn der Inhalt mit einem bestehenden Inhalt ersetzt wird, kann derselbe Eintrag in verschiedenen Modulen angezeigt und verändert werden.",
		"ChooseItem": "Inhalt auswählen:"
	},
	"ManageContentList": {
		"Title": "Inhaltsliste verwalten",
		"HeaderIntro": "Hier können Listen-Einstellungen und Kopfdaten (sofern vorhanden) verwaltet und Listen-Einträge manuell sortiert werden.",
		"NoHeaderInThisList": "(Diese Liste hat keine Einstellungen oder Kopfdaten)",
		"Intro": "Listen-Einträge"
	},
	"Edit": {
		"Fields": {
			"Hyperlink": {
				"Default": {
					"Tooltip1": "Dateien hier ablegen für den Auto-Upload",
					"Tooltip2": "Für Hilfe besuche 2sxc.org/help?tag=adam",
					"Tooltip3": "ADAM - spendiert mit ♥ von 2sic.com",
					"AdamUploadLabel": "Schnell-Upload mit ADAM",
					"MenuAdam": "Datei mit ADAM hochladen",
					"MenuPage": "Page Picker",
					"MenuImage": "Image Manager",
					"MenuDocs": "Document Manager",
					"SponsoredLine": "<a href='http://2sxc.org/help?tag=adam' target='_blank' tooltip='ADAM ist der automatische Digital Assets Manager – Klicke um mehr zu erfahren'>Adam</a> spendiert mit ♥ von <a tabindex='-1' href='http://2sic.com/' target='_blank'>2sic.com</a>"
				},
				"FileManager": {},
				"PagePicker": {
					"Title": "Wähle eine Seite"
				}
			}
		}
	},
	"TemplatePicker": {
		"AppPickerDefault": "App auswählen",
		"ContentTypePickerDefault": "Inhaltstyp auswählen",
		"LayoutElement": "Design-Element",
		"ChangeView": "Darstellung anpassen",
		"Save": "Speichern",
		"Cancel": "Abbrechen",
		"Close": "Schliessen",
		"Install": "Apps installieren",
		"Catalog": "App Katalog durchsuchen",
		"App": "App Konfigurieren",
		"Zone": "Apps verwalten"
	},
	"ItemHistory": {
		"Title": "Versionen dieses Elements",
		"Version": "Version {{version}}",
		"NoHistory": "Es gibt keine früheren Versionen dieses Elements",
		"Buttons": {
			"RestoreLive": "Wiederherstellen",
			"RestoreDraft": "Als Entwurf wiederherstellen"
		}
	}
}