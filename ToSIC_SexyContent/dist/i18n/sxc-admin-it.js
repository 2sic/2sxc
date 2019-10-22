{
	"Main": {
		"Title": "Amministratore",
		"Tab": {
			"GS": "Home",
			"GettingStarted": "Per iniziare",
			"CD": "Dati",
			"ContentData": "Contenuti",
			"VT": "Views",
			"ViewsTemplates": "Views / Templates",
			"Q": "Query",
			"Query": "Query Designer",
			"WA": "WebApi",
			"WebApi": "WebApi / Data",
			"IE": "",
			"ImportExport": "Import / Export",
			"PL": "Global",
			"PortalLanguages": "Portal / Lingue",
			"AS": "App",
			"AppSettings": "Impostazioni App"
		},
		"Portal": {
			"Title": "Impostazioni per tutto il portale",
			"Intro": "Queste impostazioni si applicano a tutti i contenuti e tutte le applicazioni all'interno di questo portale."
		}
	},
	"Templates": {
		"Title": "Gestione Templates",
		"InfoHideAdvanced": "Migliora la user experience per il gestione dell'editor dei contenuti nascondendo le funzionalità avanzate. Se il tuo portale contiene regole di sicurezza chiamate “2sxc designers”, allora in non membri non vedranno i bottoni come in modifica template o gestione template. Maggiori inforamzioni <a href='http://2sxc.org/en/help?tag=hide-advanced' target='_blank'>qui</a>.",
		"Table": {
			"TName": "Nome Template",
			"TPath": "Path",
			"CType": "Tipo di contenuto",
			"DemoC": "Elemento Demo",
			"Show": "Mostra",
			"UrlKey": "Chiave Url",
			"Actions": "Azioni"
		}
	},
	"TemplateEdit": {
		"Title": "Modifica Template"
	},
	"WebAPIData": {
		"unused": {
			"Title": "Prendi contenuti come se si trattasse di dati per il tuo template o JavaScript",
			"Intro": "Utilizza query designer per creare queries complesse. Possono essere utilizzati in templates (Token/Razor) o JavaScript. Oppure puoi creare un tuo JSON-WebAPI da utilizzare con JavaScript - per esempio per salvare qualcosa, recuperare un file o eseguire una query complessa. Per maggiori dettagli <a href='http://2sxc.org/en/Docs/tag/Data%20and%20Data%20Sources' target='_blank'>codice e data pipelines</a> e come <a href='http://2sxc.org/en/Docs/tag/Data%20and%20Data%20Sources' target='_blank'>utilizzare dati con JavaScript, jQuery, AngularJS e altro</a>.",
			"Visual": {
				"Title": "Visual Data Query",
				"Intro": "Utilizza Visual Query Designer (Pipeline-Designer) per creare queries per 2sxc, SQL, RSS e altro.",
				"Button": "Visual Query Designer"
			}
		}
	},
	"WebApi": {
		"Title": "WebApi per questa App",
		"Intro": "Crea una WebApi in pochi minuti inserendo il tuo condice nella cartella denominata API ed ereditando la corretta interfaccia. Puoi fare una prova creando automanticante e premendo qui. Per maggiori informazioni <a href='http://2sxc.org/en/Docs-Manuals/Feature/feature/3361' target='_blank'>WebApi</a> o <a href='http://2sxc.org/en/Docs-Manuals/Feature/feature/3360' target='_blank'>C# data editing API</a>.",
		"ListTitle": "La lista seguente mostra i files .cs files nella cartella App-API:",
		"InfoMissingFolder": "(la cartella non esiste)",
		"QuickStart": "Per veloce inizio, raccomandiamo di installare la WebApi demo-app. Contiene alcuni controllers WebAPI con varie azioni ed alcuni esempi di views che utilizzano i controllers. Scarica <a href='http://2sxc.org/en/Apps/tag/WebApi' target='_blank'>WebApi demos nel catalogo App-Catalog</a> o per maggiori dettagli <a href='http://2sxc.org/en/help?tag=webapi' target='_blank'>clicca qui (help)</a>",
		"AddDoesntExist": "Non è presente una add automatica - per favore crea manualmente la cartella 'api'. Copia un progetto demo (lo trovi online) per iniziare."
	},
	"ImportExport": {
		"Title": "Esporta o importa <em>parti</em> per questa App/Content",
		"Intro": "Crea un xml or zip che contiene <em>parti</em> di questa app, per importare in un'altra app o contenuto. O importa un parts-package.",
		"FurtherHelp": "Per maggiori dettagli visita <a href='http://2sxc.org/en/help?tag=import' target='_blank'>2sxc Help</a>.",
		"Buttons": {
			"Import": "importa",
			"Export": "esporta"
		},
		"Import": {
			"Title": "Importa un centenuto esportato (.zip) o parziale (.xml)",
			"Explanation": "Questa importazione aggiungerà un Content-Types, Templates e Content-Items al corrente Content o App.",
			"Select": "seleziona un file da importare",
			"Choose": "Scegli un file"
		},
		"Export": {
			"Title": "Esportazioni parziali o Content Types, Template Configuration e Content",
			"Intro": "QUesta è una esportazione avanzata per esportare parti di questa Content / App. Verrà creato un XML-file per il quale potrai importare in altro sito o App.",
			"FurtherHelp": "Per maggiori dettagli visita <a href='http://2sxc.org/en/help?tag=export' target='_blank'>2sxc Help</a>.",
			"Data": {
				"GroupHeading": "Content Type: {{name}} ({{id}}",
				"Templates": "Templates",
				"Items": "Content Items",
				"SimpleTemplates": "Templates senza content type"
			},
			"ButtonExport": "Esporta"
		}
	},
	"Portal": {
		"Title": "Virtual Database (VDB)",
		"VdbLabel": "Virtual Database per questo portale",
		"Rename": "Note a 2tk - rename is not necessary any more, don't implement!"
	},
	"Language": {
		"Title": "Lingue / Cultures",
		"Intro": "Per abilitare / disabilitare lingue per questa area (questo portale)",
		"Table": {
			"Code": "Codice",
			"Culture": "Culture",
			"Status": "Stato"
		}
	},
	"AppConfig": {
		"Title": "Configurazione App",
		"Intro": "Configura l'App e speciali impostazioni dell'App qui.",
		"Settings": {
			"Title": "Impstazioni App",
			"Intro": "Le impostazioni sono configurazioni utilizzate dall'app - come SQL-connection strings, \"dati di default da mostrare\" numeri e altro. Possono essere muiltilingua, in modo che le impostazioni (come default RSS-Feed) possono essere differenti in ogni linguaggio.",
			"Edit": "modifica le impostazioni dell'app",
			"Config": "configura le impostazioni dell'app"
		},
		"Resources": {
			"Title": "Risorse App",
			"Intro": "Le risorse sono utilizzate per le labels e altri oggetti simili nell'App. Di solito sono necesssari per creare view multilingua, non doveno essere utilizzati per le impstazionui dell'App.",
			"Edit": "modifica le risorse dell'App",
			"Config": "configura le risorse dell'App"
		},
		"Definition": {
			"Title": "Definizione del pacchetto dell'App",
			"Intro": "La definizione del pacchettto dell'App è importante per il processo di esportazione/importazione dell'App.",
			"Edit": "modifica le definizione dell'App"
		},
		"Export": {
			"Title": "Esporta <em>totalmente</em> questa App",
			"Intro": "Crea un pacchetto dell'App (zip) che potrà essere installato in un altro portale",
			"Button": "esporta"
		}
	},
	"AppManagement": {
		"Title": "Gestisci l'Apps per questa area (Portale)",
		"Table": {
			"Name": "Nome",
			"Folder": "Cartella",
			"Templates": "Templates",
			"Show": "mostra questa App agli utenti",
			"Actions": "Azioni"
		},
		"Buttons": {
			"Browse": "altre Apps",
			"Import": "importa App",
			"Create": "crea",
			"Export": "Esporta App"
		},
		"Prompt": {
			"NewApp": "Inserisci un nome per l'App (verrà utilizzato anche per la cartella)",
			"DeleteApp": "Non può essere cancellata. Per cancellare realmente questa App, digita (o copia e incolla) in nome dell'App qui: sicuro che vuoi cancellare l'App '{{name}}' ({{id}}) ?",
			"FailedDelete": "i dati inserito non sono corretti - non verrà cancellata"
		}
	},
	"ReplaceContent": {
		"Title": "Sostituisci Content-Item",
		"Intro": "ostituendo un content-item si può far apparire un altro contenuto nello slot del contenuto originale.",
		"ChooseItem": "Seleziona un elemento:"
	},
	"ManageContentList": {
		"Title": "Gestici la lista dei content-item",
		"HeaderIntro": "Puoi gestire la lista degli header qui (se è definita)",
		"NoHeaderInThisList": "(questa lista non ha header)",
		"Intro": "Ordinare gli elementi trascinando di cui hai bisogno, quindi salvare."
	},
	"Edit": {
		"Fields": {
			"Hyperlink": {
				"Default": {
					"Tooltip1": "trascina i files qui per un carimaneto automatico",
					"Tooltip2": "per maggiori dettagli 2sxc.org/help?tag=adam",
					"Tooltip3": "ADAM - sposorizzato con amore da 2sic.com",
					"AdamUploadLabel": "caricamento veloce utilizzando ADAM",
					"MenuAdam": "Caricamento file con Adam",
					"MenuPage": "Pagina Picker",
					"MenuImage": "Gestione immagini",
					"MenuDocs": "Gestione documenti",
					"SponsoredLine": "<a href='http://2sxc.org/help?tag=adam' target='_blank' tooltip='ADAM è Automatic Digital Assets Manager - clicca qui per scoprire di più'>Adam</a> è sposorizzato con ♥ da <a tabindex='-1' href='http://2sic.com/' target='_blank'>2sic.com</a>"
				},
				"FileManager": {},
				"PagePicker": {
					"Title": "Seleziona una pagina web"
				}
			}
		}
	},
	"TemplatePicker": {
		"AppPickerDefault": "Scegli app",
		"ContentTypePickerDefault": "Scegli tipo contenuto",
		"Save": "Salva e chiudi",
		"Cancel": "Cancella cambiamenti",
		"Close": "Chiudi"
	}
}