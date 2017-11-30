{
	"Main": {
		"Title": "Administration",
		"Tab": {
			"GS": "Accueil",
			"GettingStarted": "Pour commencer",
			"CD": "Data",
			"ContentData": "Contenus",
			"VT": "Vues",
			"ViewsTemplates": "Vues / Templates",
			"Q": "Requête",
			"Query": "Editeur de requêtes",
			"WA": "WebApi",
			"WebApi": "WebApi / Data",
			"IE": "",
			"ImportExport": "Import / Export",
			"PL": "Global",
			"PortalLanguages": "Site / Langues",
			"AS": "App",
			"AppSettings": "Configuration de l'App"
		},
		"Portal": {
			"Title": "Configuration pour tout le site",
			"Intro": "Ces paramètres s'appliqueront à tous les contenus et les apps du site."
		}
	},
	"Templates": {
		"Title": "Gestion des templates",
		"InfoHideAdvanced": "Simplifier l'interface des admins de base en cachant les fonctions avancées. Si le site comporte un rôle DNN nommé “2sxc designers”, alors ceux qui n'ont pas ce rôle n'auront pas accès à la conception ou la gestion des templates. <a href='http://2sxc.org/en/help?tag=hide-advanced' target='_blank'>Voir ici</a>.",
		"Table": {
			"TName": "Nom du template",
			"TPath": "Chemin",
			"CType": "Content Type",
			"DemoC": "item de démo",
			"Show": "Voir",
			"UrlKey": "Url key",
			"Actions": "Actions"
		}
	},
	"TemplateEdit": {
		"Title": "Edition de template"
	},
	"WebAPIData": {
		"unused": {
			"Title": "Importer automatiquement du contenu comme source de données pour votre template ou code JavaScript",
			"Intro": "Utiliser l'éditeur pour créer des requêtes complexes, utilisables dans des templates normaux (Token/Razor) ou en JavaScript. Ou créer votre propre WebAPI JSON (seulement pour du code JavaScript - par exemple pour enregistrer, charger un fichier ou concevoir une requête trop complexe pour l'éditeur). Voir <a href='http://2sxc.org/en/Docs/tag/Data%20and%20Data%20Sources' target='_blank'>coder les data pipelines</a> et <a href='http://2sxc.org/en/Docs/tag/Data%20and%20Data%20Sources' target='_blank'>gérer les données en JavaScript avec jQuery, AngularJS et autres</a>.",
			"Visual": {
				"Title": "Editeur de requête",
				"Intro": "Utiliser l'éditeur (Pipeline-Designer) pour créer des requêtes 2sxc, SQL, RSS et autres.",
				"Button": "Editeur de requêtes"
			}
		}
	},
	"WebApi": {
		"Title": "WebApi pour cette App",
		"Intro": "Créer rapidement une WebApi en copiant le code source dans le dossier API et en héritant de l'interface correcte. Essayer en en créant une automatiquement. Plus sur  <a href='http://2sxc.org/en/Docs-Manuals/Feature/feature/3361' target='_blank'>WebApi</a> ou <a href='http://2sxc.org/en/Docs-Manuals/Feature/feature/3360' target='_blank'>C# data editing API</a>.",
		"ListTitle": "Liste des fichiers .cs dans le dossier App-API :",
		"InfoMissingFolder": "(le dossier n'existe pas)",
		"QuickStart": "Pour démarrer vite, nous vous conseillons d'installer l'App WebApi de démo. Elle contient des controleurs WebAPI avec plusieurs actions et des dexemples de vues pour les utiliser. Télécharger <a href='http://2sxc.org/en/Apps/tag/WebApi' target='_blank'>WebApi demos in the App-Catalog</a> ou pour en savoir plus : <a href='http://2sxc.org/en/help?tag=webapi' target='_blank'>aide</a>",
		"AddDoesntExist": "Il n'y a pas encore d'option de création - veuillez le faire manuellement dans le dossier 'api'. Pour démarrer, en recopier d'un projet existant."
	},
	"Portal": {
		"Title": "Database virtuelle (VDB)",
		"VdbLabel": "Database virtuelle pour ce site",
		"Rename": "Note to 2tk - rename is not necessary any more, don't implement!"
	},
	"Language": {
		"Title": "Langues / Cultures",
		"Intro": "Gérer les activations de langues pour cette zone (ce site)",
		"Table": {
			"Code": "Code",
			"Culture": "Culture",
			"Status": "Status"
		}
	},
	"AppConfig": {
		"Title": "App Configuration",
		"Intro": "Configuration de l'App et paramètres spéciaux.",
		"Settings": {
			"Title": "Configuration",
			"Intro": "Configuration et paramètres techniques tels que les connection strings SQL, les \"items-to-show\" par défaut, etc . La configuration peut être spécifique à une langue (par exemple un flux RSS).",
			"Edit": "modifier la config. d'App",
			"Config": "configurer l'App"
		},
		"Resources": {
			"Title": "Ressources",
			"Intro": "Les ressources sont les messages et autres éléments d'interface utilisateur au sein de l'App. Ils servent à la traduction dans les vues en mode multilingue, mais ne doivent pas être utilisé pour traduire les paramètres de configuration techniques (App-Settings).",
			"Edit": "modifier les ressources",
			"Config": "configurer les ressources"
		},
		"Definition": {
			"Title": "App-Pack Définition",
			"Intro": "La définition d'App-pack sert pour les exports/imports de l'App.",
			"Edit": "modifier la définition d'App-pack"
		},
		"Export": {
			"Title": "Exporter l'App <em>en entier</em>",
			"Intro": "Créer un App-pack (zip), pouvant être installé dans un autre site",
			"Button": "export"
		}
	},
	"AppManagement": {
		"Title": "Gestion des Apps de cette Zone (Site)",
		"Table": {
			"Name": "Nom",
			"Folder": "Dossier",
			"Templates": "Templates",
			"Show": "visible des utilisateurs",
			"Actions": "Actions"
		},
		"Buttons": {
			"Browse": "plus d'apps",
			"Import": "import d'app",
			"Create": "créer",
			"Export": "export d'app"
		},
		"Prompt": {
			"NewApp": "Indiquer un nom d'App (qui sera aussi le nom du dossier)",
			"DeleteApp": "Action irréversible. Pour supprimer cette App, indiquer ou copier son nom ici : sur de vouloir supprimer '{{name}}' ({{id}}) ?",
			"FailedDelete": "Le nom ne correspond pas - suppression annulée"
		}
	},
	"ReplaceContent": {
		"Title": "Replacer un Content Item",
		"Intro": "En remplacant un content-item on peut en faire apparaitre un autre au même emplacement.",
		"ChooseItem": "Choix de l'item :"
	},
	"ManageContentList": {
		"Title": "Configuration des listes d'items",
		"HeaderIntro": "Configuration de l'entête de liste (s'il y en a un)",
		"NoHeaderInThisList": "(cette liste n'a pas d'entête)",
		"Intro": "Repositionner à la souris, puis Enregistrer"
	},
	"Edit": {
		"Fields": {
			"Hyperlink": {
				"Default": {
					"Tooltip1": "glisser-déposer les fichiers à charger",
					"Tooltip2": "voir 2sxc.org/help?tag=adam",
					"Tooltip3": "ADAM - sponsorisé avec amour par 2sic.com",
					"AdamUploadLabel": "chargement rapide avec ADAM",
					"MenuAdam": "Charger un fichier with Adam",
					"MenuPage": "Choix de page",
					"MenuImage": "Gestionnaire d'images",
					"MenuDocs": "Gestionnaire de document",
					"SponsoredLine": "<a href='http://2sxc.org/help?tag=adam' target='_blank' tooltip='ADAM signifie Automatic Digital Assets Manager - en savoir plus '>Adam</a> sponsorisé avec ♥ par <a tabindex='-1' href='http://2sic.com/' target='_blank'>2sic.com</a>"
				},
				"FileManager": {},
				"PagePicker": {
					"Title": "Sélectionner une page web"
				}
			}
		}
	},
	"SourceEditor": {
		"Title": "Source Editeur",
		"SnippetsSection": {
			"Title": "Extraits de code",
			"Intro": "Cliquez sur un extrait de code pour l'insérer"
		}
	},
	"TemplatePicker": {
		"AppPickerDefault": "Choisir une App",
		"ContentTypePickerDefault": "Choisir un Content-type",
		"Save": "Enregistrer et fermer",
		"Cancel": "Annuler",
		"Close": "Fermer",
		"Install": "Installer plus d'applications...",
		"Catalog": "Parcourir le catalogue d'applications",
		"App": "Configurer l'application",
		"Zone": "Gérer toutes les applications"
	},
	"ImportExport": {
		"Title": "Exporter ou importer <em> les parties </em> de cette application",
		"Intro": "Créez un xml contenant des parties de l'application, par exemple le contenu et les templates. <br/> Importez des parties de ce même xml dans cette application.",
		"Buttons": {
			"Export": "Exporter les parties",
			"Import": "Importer les parties"
		},
		"ImportContent": {
			"Title": "Importer le contenu et des templates dans cette application",
			"Intro": "Importez le contenu et les templates à partir d'une exportation de contenu (zip) ou d'une exportation partielle (xml) vers cette application. L'intégralité du contenu du fichier sélectionné sera importé. Si vous souhaitez importer une application entière, accédez à <em> App-Management </em>",
			"FurtherHelp": "Pour plus d'informations, aller voir sur <a href='http://2sxc.org/en/help?tag=import' target='_blank'>2sxc Help</a>.",
			"Messages": {
				"ImportSucceeded": "L'importation a été effectuée. Consultez les messages ci-dessous pour plus d'informations.",
				"ImportFailed": "L'importation a échoué. Consultez les messages ci-dessous pour plus d'informations."
			},
			"Commands": {
				"SelectFile": "Sélectionner un ficher",
				"ToggleSuccessMessages": "Ouvrir/fermer les messages de réussite",
				"Import": "Importer"
			}
		},
		"ImportApp": {
			"Title": "Importer l'application",
			"Intro": "Sélectionner un package d'application (zip) de votre ordinateur pour importer une application. Les nouvelles applications peuvent être téléchargées sur <a href='http://2sxc.org/apps'> http://2sxc.org/apps </a>.",
			"FurtherHelp": "Pour plus d'informations, consulter <a href='http://2sxc.org/en/help?tag=import-app' target='_blank'>2sxc Help</a>.",
			"Messages": {
				"ImportSucceeded": "L'importation a été effectuée. Consulter les messages ci-dessous pour plus d'informations",
				"ImportFailed": "L'importation a échoué. Consulter les messages ci-dessous pour plus d'informations."
			},
			"Commands": {
				"SelectFile": "Sélectionner un ficher",
				"ToggleSuccessMessages": "Ouvrir/fermer les messages de réussite",
				"Import": "Importer"
			}
		},
		"ExportContent": {
			"Title": "Exporter le contenu et les modèles de cette application",
			"Intro": "Il s'agit d'une fonctionnalité avancée pour exporter des parties de l'application. L'exportation créera un fichier <em>xml</em> qui peut être importé dans un autre site ou une application. Pour exporter l'intégralité du contenu de l'application (par exemple, lors de la duplication de l'intégralité du site), accédez au gestionnaire d'exportation de l'application.",
			"FurtherHelp": "Pour plus d'informations, consulter <a href='http://2sxc.org/en/help?tag=export' target='_blank'>2sxc Help</a>.",
			"ContentTypes": {
				"Title": "Content Types",
				"Templates": "Templates",
				"Entities": "Entités"
			},
			"TemplatesWithoutContentTypes": {
				"Title": "Templates avec Content Types"
			},
			"Commands": {
				"Export": "Exporter"
			}
		},
		"ExportApp": {
			"Title": "Exporter l'application",
			"Intro": "Packager l'application entière dans un dossier <em>zip</em> qui peut être importé à nouveau sur un autre site.",
			"FurtherHelp": "Pour plus d'informations, consulter <a href='http://2sxc.org/en/help?tag=export-app' target='_blank'>2sxc Help</a>.",
			"Specifications": {
				"Title": "Specs",
				"AppName": "Name:",
				"AppGuid": "Guid:",
				"AppVersion": "Version:"
			},
			"Content": {
				"Title": "Contient",
				"EntitiesCount": "entités",
				"LanguagesCount": "langues",
				"TemplatesCount": "templates",
				"TokenTemplates": "Token:",
				"RazorTemplates": "Razor:",
				"TransferableFilesCount": "fichiers à exporter",
				"FilesCount": "Les fichiers dans le dossier de l'application complètement"
			},
			"Options": {
				"IncludeContentGroups": "Inclure tous les groupes de contenu pour réimporter l'application dans une copie exacte de ce site. Sélectionnez uniquement cette option lorsque vous copier un site complet de DNN.",
				"ResetAppGuid": "Réinitialiser le GUID de l'application à zéro. Vous n'avez besoin que de cette option pour les applications spéciales, et ne doit généralement pas être sélectionné."
			},
			"Commands": {
				"Export": "Exporter l'application",
				"ExportForVersionControl": "Exporter des données pour la version de version de Github"
			}
		}
	}
}