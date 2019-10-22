{
	"General": {
		"Buttons": {
			"Add": "Ajout",
			"Refresh": "Rafraichir",
			"System": "fonctions système avancées",
			"Save": "Enregistrer",
			"NotSave": "Annuler",
			"Cancel": "Annuler",
			"Permissions": "permissions",
			"Edit": "Modifier",
			"Delete": "Supprimer",
			"Copy": "Copier",
			"Rename": "Renommer"
		},
		"Messages": {
			"Loading": "chargement...",
			"NothingFound": "rien trouvé",
			"CantDelete": "impossible de supprimer {{target}}"
		},
		"Questions": {
			"Delete": "voulez-vous supprimer {{target}}?",
			"DeleteEntity": "supprimer '{{title}}' ({{id}}?",
			"Rename": "Par quel nom voulez-vous remplacer {{target}}?",
			"SystemInput": "Réservé aux opérations avancée. A utiliser en connaissance de cause \n\n Entre les commandes d'admin:",
			"ForceDelete": "Voulez-vous forcer la suppression '{{title}}' ({{id}})?"
		},
		"Terms": {
			"Title": "titre"
		}
	},
	"DataType": {
		"All": {
			"Title": "Configuration"
		},
		"Boolean": {
			"Short": "oui/non",
			"ShortTech": "Booléen",
			"Choice": "Booléen (oui/non)",
			"Explanation": "Valeurs de type oui/non, ou vrai/faux"
		},
		"DateTime": {
			"Short": "date/heure",
			"ShortTech": "DateTime",
			"Choice": "Date et/ou heure",
			"Explanation": "pour les dates, les heures ou les 2 combinées"
		},
		"Entity": {
			"Short": "item(s)",
			"ShortTech": "Entité",
			"Choice": "Entité (autre content-items)",
			"Explanation": "un ou plusieurs autres content-items"
		},
		"Hyperlink": {
			"Short": "lien",
			"ShortTech": "Hyperlien",
			"Choice": "Lien / fichier",
			"Explanation": "lien ou pointeur vers un fichier / image"
		},
		"Number": {
			"Short": "nombre",
			"ShortTech": "Decimal",
			"Choice": "Nombre",
			"Explanation": "valeur numérique"
		},
		"String": {
			"Short": "texte",
			"ShortTech": "String",
			"Choice": "texte / string",
			"Explanation": "tout contenu textuel"
		},
		"Empty": {
			"Short": "vide",
			"ShortTech": "Empty",
			"Choice": "Vide - pour les titres de formulaires, etc",
			"Explanation": "permet de structurer un formulaire"
		},
		"Custom": {
			"Short": "custom",
			"ShortTech": "Custom",
			"Choice": "Custom - ui-tools ou custom types",
			"Explanation": "par exemple coordonnées gps (qui renseignent plusieurs champs) ou pour des custom-data qui serialisent des données dans la base comme des tableaux, du json ou tout autre objet"
		}
	},
	"ContentTypes": {
		"Title": "Content-Types and Data",
		"TypesTable": {
			"Name": "Nom",
			"Description": "Description",
			"Fields": "Champs",
			"Items": "Items",
			"Actions": ""
		},
		"TitleExportImport": "Export / Import",
		"Buttons": {
			"Export": "export",
			"Import": "import",
			"ChangeScope": "change scope",
			"ChangeScopeQuestion": "Fonction avancée permettant de voir les content-types en dehors du scope. A utiliser prudemment, il y a de bonnes raisons de cacher les content-types en dehors du scope..."
		},
		"Messages": {
			"SharedDefinition": "ce content-type partage la définition de #{{SharedDefId}} , on ne peut donc pas l'éditer ici - voir 2sxc.org/help?tag=shared-types",
			"TypeOwn": "content-type propre, n'utilise pas la définitin d'un autre content-type - voir 2sxc.org/help?tag=shared-types",
			"TypeShared": "ce content-type hérite la définition de #{{SharedDefId}} - voir 2sxc.org/help?tag=shared-types"
		}
	},
	"ContentTypeEdit": {
		"Title": "Edit Content Type",
		"Name": "Nom",
		"Description": "Description",
		"Scope": "Scope"
	},
	"Fields": {
		"Title": "Champs de Content Type",
		"TitleEdit": "Ajouter un champs",
		"Table": {
			"Title": "Titre",
			"Name": "Nom static",
			"DataType": "Data Type",
			"Label": "Label",
			"InputType": "Input Type",
			"Notes": "Notes",
			"Sort": "Tri",
			"Action": ""
		},
		"General": "Général"
	},
	"Permissions": {
		"Title": "Permissions",
		"Table": {
			"Name": "Nom",
			"Id": "ID",
			"Condition": "Condition",
			"Grant": "Accorder",
			"Actions": ""
		}
	},
	"Pipeline": {
		"Manage": {
			"Title": "Visual Queries / Pipelines",
			"Intro": "Utiliser l'éditeur visuel pour créer des requetes ou fusionner des données de diverses sources. Peut être utilisé dans les vues ou en JSON (sous réserve de permission). <a href='http://2sxc.org/en/help?tag=visualquerydesigner' target='_blank'>read more</a>",
			"Table": {
				"Id": "ID",
				"Name": "Nom",
				"Description": "Description",
				"Actions": ""
			}
		},
		"Designer": {},
		"Stats": {
			"Title": "Résultats de requête",
			"Intro": "Le résultat complet a été loggué dans la console du browser. Plus d'infos de dégoguage plus bas...",
			"ParamTitle": "Paramètres & Statistiques",
			"ExecutedIn": "Exécuté en {{ms}}ms ({{ticks}} ticks)",
			"QueryTitle": "Résultats",
			"SourcesAndStreamsTitle": "Sources et Streams",
			"Sources": {
				"Title": "Sources",
				"Guid": "Guid",
				"Type": "Type",
				"Config": "Configuration"
			},
			"Streams": {
				"Title": "Streams",
				"Source": "Source",
				"Target": "Cible",
				"Items": "Items",
				"Error": "Erreur"
			}
		}
	},
	"Content": {
		"Manage": {
			"Title": "Gérer les contenus / données",
			"Table": {
				"Id": "ID",
				"Status": "Status",
				"Title": "Titre",
				"Actions": ""
			},
			"NoTitle": "- sans titre -"
		},
		"Publish": {
			"PnV": "publié et visible",
			"DoP": "brouillon pour un autre item publié",
			"D": "pas encore publié",
			"HD": "brouillon : {{id}}",
			"HP": "remplacera le publié"
		},
		"Export": {
			"Title": "Exporter contenus / données",
			"Help": "Génération d'un fichier XML editable dans Excel. If you just want to import new data, use this to export the schema that you can then fill in using Excel. Please visit <a href='http://2sxc.org/help' target='_blank'>http://2sxc.org/help</a> for more instructions.",
			"Commands": {
				"Export": "Export"
			},
			"Fields": {
				"Language": {
					"Label": "Langues",
					"Options": {
						"All": "Toutes"
					}
				},
				"LanguageReferences": {
					"Label": "Value references to other languages",
					"Options": {
						"Link": "Keep references to other languages (pour ré-import)",
						"Resolve": "Remplacer les références par des valeurs"
					}
				},
				"ResourcesReferences": {
					"Label": "Références de fichier / page",
					"Options": {
						"Link": "Keep references (for re-import, for example Page:4711)",
						"Resolve": "Remplacer les références par les vraies URLs (par exemple : /Portals/0...)"
					}
				},
				"RecordExport": {
					"Label": "Exportation des contenus",
					"Options": {
						"Blank": "Non, seulement le schéma vide de données (pour réimportation des données)",
						"All": "Oui, tout exporter des content-items"
					}
				}
			}
		},
		"Import": {
			"Title": "Importation de contenu / étapes (data step)",
			"TitleSteps": "{{step}} / 3",
			"Help": "Importation de content-items dans 2sxc. Nécessite d'avoir défini le content-type avant l'import, et que le fichier d'import ait été créé avec le template généré par un export. Voir <a href='http://2sxc.org/help' target='_blank'>http://2sxc.org/help</a>.",
			"Fields": {
				"File": {
					"Label": "Choix du fichier"
				},
				"ResourcesReferences": {
					"Label": "Références de fichier / page",
					"Options": {
						"Keep": "Importer les liens comme indiqué dans le fichier (par exemple /Portals/...)",
						"Resolve": "Essayer de résoudre les URLs vers les références"
					}
				},
				"ClearEntities": {
					"Label": "Suppression de toutes les autres entités",
					"Options": {
						"None": "Conserver les entités absentes de l'import",
						"All": "Supprimer les entités qui ne se trouvent pas dans l'import"
					}
				}
			},
			"Commands": {
				"Preview": "Prévisualisation de l'import",
				"Import": "Import"
			},
			"Messages": {
				"BackupContentBefore": "Faites un backup avant !",
				"WaitingForResponse": "Veuillez patienter...",
				"ImportSucceeded": "Import terminée.",
				"ImportFailed": "Echec de l'import.",
				"ImportCanTakeSomeTime": "Note: l'import valide beaucoup de données et peut prendre plusieurs minutes."
			},
			"Evaluation": {
				"Error": {
					"Title": "Tentative d'import du fichier '{{filename}}'",
					"Codes": {
						"0": "Erreur inconnue.",
						"1": "Content-type choisi inconnu.",
						"2": "Le document n'est pas un fichier XML valide.",
						"3": "Le content-type choisi ne correspond pas à celui décrit dans le fichier XML.",
						"4": "Langue non prise en charge.",
						"5": "Le document ne spécifie pas toutes les langues pour toutes les entités.",
						"6": "La référence à une langue ne peut être évaluée, langue non prise en charge.",
						"7": "La référence à une langue ne peut être évaluée, protection en lecture/écriture non prise en charge.",
						"8": "Valeur illisible à cause d'un format incorrect."
					},
					"Detail": "Détails: {{detail}}",
					"LineNumber": "Ligne-no: {{number}}",
					"LineDetail": "Ligne-details: {{detail}}"
				},
				"Detail": {
					"Title": "Tentative d'import du fichier '{{filename}}'",
					"File": {
						"Title": "Le fichier contient :",
						"ElementCount": "{{count}} content-items (enregistrements/entités)",
						"LanguageCount": "{{count}} langues",
						"Attributes": "{{count}} {{attributes}} de colonnes"
					},
					"Entities": {
						"Title": "Si vous cliquez sur Import, cela :",
						"Create": "Créera {{count}} content-items",
						"Update": "Mettra à jour {{count}} content-items",
						"Delete": "Supprimera {{count}} content-items",
						"AttributesIgnored": "Ignorera {{count}} {{attributes}} de colonnes"
					}
				}
			}
		},
		"History": {
			"Title": "Historique de {{id}}",
			"Table": {
				"Id": "#",
				"When": "Quand",
				"User": "Utilisateur",
				"Actions": ""
			}
		}
	},
	"AdvancedMode": {
		"Info": {
			"Available": "cette interface dispose d'un mode avancé / debug pour les pros - voir : 2sxc.org/help?tag=debug-mode",
			"TurnOn": "mode avancé/debug activé",
			"TurnOff": "mode avancé/debug désactivé"
		}
	}
}