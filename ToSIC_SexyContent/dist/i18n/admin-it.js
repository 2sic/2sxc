{
	"General": {
		"Buttons": {
			"Add": "aggiungi",
			"Refresh": "aggiorna",
			"System": "funzioni di sistema avanzate",
			"Save": "salva",
			"Cancel": "annulla",
			"Permissions": "permessi",
			"Edit": "modifica",
			"Delete": "cancella",
			"Copy": "copia",
			"Rename": "rinomina"
		},
		"Messages": {
			"Loading": "caricamento...",
			"NothingFound": "nessun elemento trovato",
			"CantDelete": "impossibile eliminare {{target}}"
		},
		"Questions": {
			"Delete": "sei sicuro di voler eliminare {{target}}?",
			"DeleteEntity": "eliminare '{{title}}' ({{id}}?",
			"SystemInput": "Questo è solo per le operazioni avanzate. Usalo solo se sai cosa stai facendo. \n\n Inserisci un comando di amministrazione:"
		},
		"Terms": {
			"Title": "titolo"
		}
	},
	"DataType": {
		"All": {
			"Title": "Impostazioni Generali"
		},
		"Boolean": {
			"Short": "si/no",
			"ShortTech": "Boolean",
			"Choice": "Booleano (si/no)",
			"Explanation": "Si/no o valori vero/falso"
		},
		"DateTime": {
			"Short": "data/tempo",
			"ShortTech": "DateTime",
			"Choice": "Data e/o tempo",
			"Explanation": "per data, tempo o valori combinati"
		},
		"Entity": {
			"Short": "elemento(i)",
			"ShortTech": "Entity",
			"Choice": "Entity (altri content-items)",
			"Explanation": "uno o altri content-items"
		},
		"Hyperlink": {
			"Short": "collegamento",
			"ShortTech": "Hyperlink",
			"Choice": "Link / riferimento ad un file",
			"Explanation": "hyperlink o riferimento ad un'immagine / file"
		},
		"Number": {
			"Short": "numero",
			"ShortTech": "Decimale",
			"Choice": "Numero",
			"Explanation": "qualsiasi tipo di numero"
		},
		"String": {
			"Short": "testo",
			"ShortTech": "Stringa",
			"Choice": "Testo / stringa",
			"Explanation": "qualsiasi tipo di testo"
		},
		"Empty": {
			"Short": "vuoto",
			"ShortTech": "Vuoto",
			"Choice": "Vuoto - per i titoli dei form ecc.",
			"Explanation": "usalo per strutturare il tuo form"
		},
		"Custom": {
			"Short": "personalizzato",
			"ShortTech": "Personalizzato",
			"Choice": "Personalizzato - ui-tools o tipo personalizzato",
			"Explanation": "usalo per cose come un gps-picker (che scrive in più campi) o per dati personalizzati che serializzano nel db cose particolari come array, un json personalizzato o qualsiasi cosa "
		}
	},
	"ContentTypes": {
		"Title": "Tipi di contenuto e dati",
		"TypesTable": {
			"Name": "Nome",
			"Description": "Descrizione",
			"Fields": "Campi",
			"Items": "Elementi",
			"Actions": ""
		},
		"TitleExportImport": "Esporta / Importa",
		"Buttons": {
			"Export": "esporta",
			"Import": "importa",
			"ChangeScope": "cambia contesto",
			"ChangeScopeQuestion": "Questa è una caratteristica avanzata di mostrare tipi di contenuto di un altro contesto. Non usarli se non sai cosa stai facendo, poichè solitamente i tipi dato di altri contesti sono nascosti per una buona ragione."
		},
		"Messages": {
			"SharedDefinition": "questo tipo di contenuto condivide la definizione di #{{SharedDefId}} quindi non puoi modificarlo qui - leggi 2sxc.org/help?tag=shared-types",
			"TypeOwn": "questo è un tipo di contenuto proprio, non usa la definizione di un altro tipo di contenuto - leggi 2sxc.org/help?tag=shared-types",
			"TypeShared": "questo tipo di contenuto eredita la definizione di #{{SharedDefId}} - leggi 2sxc.org/help?tag=shared-types"
		}
	},
	"ContentTypeEdit": {
		"Title": "Modifica Tipo di Contenuto",
		"Name": "Nome",
		"Description": "Descrizione",
		"Scope": "Contesto"
	},
	"Fields": {
		"Title": "Campi del Tipo di Contenuto",
		"TitleEdit": "Aggiungi Campi",
		"Table": {
			"Title": "Titolo",
			"Name": "Nome Statico",
			"DataType": "Tipo di Dato",
			"Label": "Etichetta",
			"InputType": "Tipo di Input",
			"Notes": "Note",
			"Sort": "Ordina",
			"Action": ""
		},
		"General": "Generale"
	},
	"Permissions": {
		"Title": "Permessi",
		"Table": {
			"Name": "Nome",
			"Id": "ID",
			"Condition": "Condizione",
			"Grant": "Concessione",
			"Actions": ""
		}
	},
	"Pipeline": {
		"Manage": {
			"Title": "Visual Queries / Pipelines",
			"Intro": "Usa il visual designer per creare query o per unire dati da risorse varie. Questo può essere usato nelle viste o può essere accessibile come JSON (se i permessi lo rendono possibile). <a href='http://2sxc.org/en/help?tag=visualquerydesigner' target='_blank'>leggi di più</a>",
			"Table": {
				"Id": "ID",
				"Name": "Nome",
				"Description": "Descrizione",
				"Actions": ""
			}
		},
		"Designer": {},
		"Stats": {
			"Title": "Risultato Query",
			"Intro": "Il risultato completo è stato loggato nella console del Browser. Sotto troverai ulteriori informazioni sul debug.",
			"ParamTitle": "Parametri & Statistiche",
			"ExecutedIn": "Eseguita in {{ms}}ms ({{ticks}} tic)",
			"QueryTitle": "Risutalto Query",
			"SourcesAndStreamsTitle": "Sorgenti e Streams",
			"Sources": {
				"Title": "Sorgenti",
				"Guid": "Guid",
				"Type": "Tipo",
				"Config": "Configurazione"
			},
			"Streams": {
				"Title": "Streams",
				"Source": "Sorgente",
				"Target": "Obiettivo",
				"Items": "Oggetti",
				"Error": "Errore"
			}
		}
	},
	"Content": {
		"Manage": {
			"Title": "Gestione contenuto / Dati",
			"Table": {
				"Id": "ID",
				"Status": "Stati",
				"Title": "Titolo",
				"Actions": ""
			},
			"NoTitle": "- nessun titolo -"
		},
		"Publish": {
			"PnV": "pubblicato e visibile",
			"DoP": "questa è una bozza di un altro oggetto pubblicato",
			"D": "non ancora pubblicato",
			"HD": "Ha una bozza: {{id}}",
			"HP": "Sostituirà il pubblicato"
		},
		"Export": {
			"Title": "Esporta contenuto / Data",
			"Help": "Verrà generato un file XML che potrai editarlo in Excel. Se vuoi importare nuovi dati, utilizzalo per esportare lo schema che riempirai usando Excel. Puoi visitare <a href='http://2sxc.org/help' target='_blank'>http://2sxc.org/help</a> per avere magiori istruzioni.",
			"Commands": {
				"Export": "Esporta"
			},
			"Fields": {
				"Language": {
					"Label": "Lingue",
					"Options": {
						"All": "Tutte"
					}
				},
				"LanguageReferences": {
					"Label": "Il valore si riferisce ad un'altra lingua",
					"Options": {
						"Link": "Mantieni le referenze per altri lingue (per successive importazioni)",
						"Resolve": "Sostiuscisci le referenze con valori"
					}
				},
				"ResourcesReferences": {
					"Label": "File / pagine riferimento",
					"Options": {
						"Link": "Mantieni le referenze (per successive importazioni, per esempio Pagina:4711)",
						"Resolve": "Sostituisci le referenze con URL reali (per esempio /Portals/0...)"
					}
				},
				"RecordExport": {
					"Label": "Esporta dati",
					"Options": {
						"Blank": "No, è sufficiente esportare schemi vuoti (per nuove importazioni)",
						"All": "Sì, esporta tutti i contenuti-oggetti"
					}
				}
			}
		},
		"Import": {
			"Title": "Importa contenuto/Dati passo",
			"TitleSteps": "{{step}} di 3",
			"Help": "Verrà importato contenuto-oggetto in 2sxc. E' essenziale che hai già definito i content-type prima di eseguire l'importazione, e che hai creato il file da importare usando il template fornito dal sistema di Esportazione. Per favore visita <a href='http://2sxc.org/help' target='_blank'>http://2sxc.org/help</a> per maggiori informazioni.",
			"Fields": {
				"File": {
					"Label": "Scegli il file"
				},
				"ResourcesReferences": {
					"Label": "Riferimento a pagine/files",
					"Options": {
						"Keep": "I links verranno importati come scritti nel file (for esempio /Portals/...)",
						"Resolve": "Cercare di risolvere path ai riferimenti"
					}
				},
				"ClearEntities": {
					"Label": "Cancellare tutte le altre entità",
					"Options": {
						"None": "Mantieni tutte le entità non trovate durante l'importazione",
						"All": "Rimuovi tutte le entità non trovate durante l'importazione"
					}
				}
			},
			"Commands": {
				"Preview": "Anteprima importazione",
				"Import": "Importazione"
			},
			"Messages": {
				"BackupContentBefore": "Ricordati di eseguire prima un backup di DNN!",
				"WaitingForResponse": "Attendere prego stiamo eseguito la tua richiesta...",
				"ImportSucceeded": "Importazione eseguita.",
				"ImportFailed": "Importazione fallita.",
				"ImportCanTakeSomeTime": "Note: a secondo della quantità dei dati, l'importazione potrebbe richiedere anche diversi minuti."
			},
			"Evaluation": {
				"Error": {
					"Title": "Importazione del file '{{filename}}'",
					"Codes": {
						"0": "Errore sconosciuto.",
						"1": "Il content-type selezionato non esiste.",
						"2": "Il documento non è un file XML valido.",
						"3": "Il content-type selezionato non corrisponde al content-type contenuto nel file XML.",
						"4": "La lingua non è supportata.",
						"5": "Il documento non specifica tutte le lingue per tutte le entità.",
						"6": "Lingua di riferimento non può essere analizzata, la lingua non è supportata",
						"7": "Lingua di riferimento non può essere analizzato, la protezione di lettura-scrittura non è supportata.",
						"8": "Il valore non può essere letto, perchè non un formato valido."
					},
					"Detail": "Dettagli: {{detail}}",
					"LineNumber": "Linea-no: {{number}}",
					"LineDetail": "Linea-dettagli: {{detail}}"
				},
				"Detail": {
					"Title": "Prova importare il file '{{filename}}'",
					"File": {
						"Title": "Il file contiene:",
						"ElementCount": "{{count}} contenuti-oggetti (records/entities)",
						"LanguageCount": "{{count}} lingue",
						"Attributes": "{{count}} colonne: {{attributes}}"
					},
					"Entities": {
						"Title": "Se premi importa, comporta:",
						"Create": "Creare {{count}} contenuti-oggetti",
						"Update": "Aggioranre {{count}} contenuti-oggetti",
						"Delete": "Eliminare {{count}} contenuti-oggetti",
						"AttributesIgnored": "Ignorati {{count}} colonne: {{attributes}}"
					}
				}
			}
		},
		"History": {
			"Title": "Storia di {{id}}",
			"Table": {
				"Id": "#",
				"When": "Quando",
				"User": "Utente",
				"Actions": ""
			}
		}
	},
	"AdvancedMode": {
		"Info": {
			"Available": "questa dialog ha un sistema avanzato di debug per utenti amministratori - per info leggi 2sxc.org/help?tag=debug-mode",
			"TurnOn": "Avanzato / modalità debug attiva",
			"TurnOff": "Avanzato / modalità debug disattivata"
		}
	}
}