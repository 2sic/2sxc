{
	"Main": {
		"Title": "Адміністрування",
		"Tab": {
			"GS": "Домашня",
			"GettingStarted": "Починаємо",
			"CD": "Data",
			"ContentData": "Контент",
			"VT": "Views",
			"ViewsTemplates": "Views / Шаблони",
			"Q": "Запити",
			"Query": "Дизайнер запитів",
			"WA": "WebApi",
			"WebApi": "WebApi / Дані",
			"IE": "",
			"ImportExport": "Імпорт / Експорт",
			"PL": "Глобальні",
			"PortalLanguages": "Портал / Мови",
			"AS": "App",
			"AppSettings": "App Налаштування"
		},
		"Portal": {
			"Title": "Налаштування для всього порталу",
			"Intro": "Ці налаштування застосовуються до усього контенту і усіх apps у цьому порталі."
		}
	},
	"Templates": {
		"Title": "Управління шаблонами",
		"InfoHideAdvanced": "Поліпшення користувацького досвіду для контент-редактора, приховуючи розширені функції від нього. Якщо ваш портал містить роль безпеки під назвою “2sxc designers”, тоді не-члени не зможуть бачити такі кнопки, як Редагування Шаблону або Керування Шаблонами. Документація <a href='http://2sxc.org/en/help?tag=hide-advanced' target='_blank'>тут</a>.",
		"Table": {
			"TName": "Назва Шаблону",
			"TPath": "Шлях",
			"CType": "Тип Контенту",
			"DemoC": "Демонстраційний Елемент",
			"Show": "Показати",
			"UrlKey": "Ключ адреси",
			"Actions": "Дії"
		}
	},
	"TemplateEdit": {
		"Title": "Редагувати Шаблон"
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
		"Title": "WebApi for this App",
		"Intro": "Create a WebApi within minutes by placing the source code in the folder called API and inheriting the correct interface. Try it out by creating one automatically and pressing here. Read more about the <a href='http://2sxc.org/en/Docs-Manuals/Feature/feature/3361' target='_blank'>WebApi</a> or the <a href='http://2sxc.org/en/Docs-Manuals/Feature/feature/3360' target='_blank'>C# data editing API</a>.",
		"ListTitle": "The following list shows the .cs files in the App-API folder:",
		"InfoMissingFolder": "(the directory does not exist)",
		"QuickStart": "For a quick start, we recommend that you install the WebApi demo-app. It contains some WebAPI controllers with various actions and some example views to use these controllers. Download <a href='http://2sxc.org/en/Apps/tag/WebApi' target='_blank'>WebApi demos in the App-Catalog</a> or read more about it in <a href='http://2sxc.org/en/help?tag=webapi' target='_blank'>help</a>",
		"AddDoesntExist": "there is no automatic add yet - please do it manually in the 'api' folder. Just copy one of an existing project to get started."
	},
	"ImportExport": {
		"Title": "Export or Import <em>parts</em> of this App/Content",
		"Intro": "Create an xml or zip containing <em>parts</em> of this app, to import into another app or content. Or import such a parts-package.",
		"FurtherHelp": "For further help visit <a href='http://2sxc.org/en/help?tag=import' target='_blank'>2sxc Help</a>.",
		"Buttons": {
			"Import": "import",
			"Export": "export"
		},
		"Import": {
			"Title": "Import a content export (.zip) or a partial export (.xml)",
			"Explanation": "This import will add Content-Types, Templates and Content-Items to the current Content or App.",
			"Select": "select file to import",
			"Choose": "Choose file"
		},
		"Export": {
			"Title": "Partial Export of Content Types, Template Configuration and Content",
			"Intro": "This is an advanced export feature to export parts of this Content / App. It will create an XML-file for you which you can import into another site or App",
			"FurtherHelp": "For further help visit <a href='http://2sxc.org/en/help?tag=export' target='_blank'>2sxc Help</a>.",
			"Data": {
				"GroupHeading": "Content Type: {{name}} ({{id}}",
				"Templates": "Templates",
				"Items": "Content Items",
				"SimpleTemplates": "Templates without content type"
			},
			"ButtonExport": "Export"
		}
	},
	"Portal": {
		"Title": "Віртуальна База (VDB)",
		"VdbLabel": "Віртуальна База для цього порталу",
		"Rename": "Note to 2tk - rename is not necessary any more, don't implement!"
	},
	"Language": {
		"Title": "Мови / Культури",
		"Intro": "Управління мовами для цієї Зони (цього порталу)",
		"Table": {
			"Code": "Код",
			"Culture": "Культура",
			"Status": "Статус"
		}
	},
	"AppConfig": {
		"Title": "Налаштування App",
		"Intro": "Конфігурація App та спеціальні налаштування App.",
		"Settings": {
			"Title": "App наоаштування",
			"Intro": "Settings are configurations used by the app - like SQL-connection strings, default \"items-to-show\" numbers and things like that. They can also be multi-language, so that a setting (like default RSS-Feed) could be different in each language.",
			"Edit": "редагувати налаштування app",
			"Config": "конфігурація app налаштувань"
		},
		"Resources": {
			"Title": "App ресурси",
			"Intro": "Resources are used for labels and things like that in the App. They are usually needed to create multi-lingual views and such, and should not be used for App-Settings.",
			"Edit": "редагувати ресурси app",
			"Config": "конфігурація app ресурсів"
		},
		"Definition": {
			"Title": "App Package Definition",
			"Intro": "The app-package definition is important when exporting/importing this app.",
			"Edit": "edit app definition"
		},
		"Export": {
			"Title": "Експортувати <em>увесь</em> App",
			"Intro": "Створити app-package (zip) який може бути встановлений на інший портал",
			"Button": "експорт"
		}
	},
	"AppManagement": {
		"Title": "Управління Apps в цій зоні (Портал)",
		"Table": {
			"Name": "Назва",
			"Folder": "Папка",
			"Templates": "Шаблони",
			"Show": "показати цей app користувачам",
			"Actions": "Дії"
		},
		"Buttons": {
			"Browse": "більше apps",
			"Import": "імпорт app",
			"Create": "створення",
			"Export": "експорт app"
		},
		"Prompt": {
			"NewApp": "Введіть назву App (також буде використовуватись для папки)",
			"DeleteApp": "Це незворотня дія. Для того, щоб дійсно видалити цей app, надрукуйте ім'я app тут: впевнені що хочете видалити '{{name}}' ({{id}}) ?",
			"FailedDelete": "введений текст не співпадає - не буде видалено"
		}
	},
	"ReplaceContent": {
		"Title": "Замінити елемент контенту",
		"Intro": "Якщо Ви заміните елемент контенту, то інший контент з'явиться на місці оригінального.",
		"ChooseItem": "Вибрати елемент:"
	},
	"ReorderContentList": {
		"Title": "Відсортуйте елементи контенту",
		"Intro": "Відсортуйте за допомогою перетягування і збережіть"
	},
	"Edit": {
		"Fields": {
			"Hyperlink": {
				"Default": {
					"Tooltip1": "перетягніть сюда файли для завантаження",
					"Tooltip2": "для допомоги зверніться до 2sxc.org/help?tag=adam",
					"Tooltip3": "ADAM - із любов'ю наданий спонсором 2sic.com",
					"AdamUploadLabel": "швидке завантаження із ADAM",
					"MenuAdam": "Завантажити файл із Adam",
					"MenuPage": "Вибір Сторінок",
					"MenuImage": "Менеджер Малюнків",
					"MenuDocs": "Менеджер Документів",
					"SponsoredLine": "<a href='http://2sxc.org/help?tag=adam' target='_blank' tooltip='ADAM це Automatic Digital Assets Manager - натисніть тут щоб дізнатись більше'>Adam</a> із ♥ наданий спонсором <a tabindex='-1' href='http://2sic.com/' target='_blank'>2sic.com</a>"
				},
				"FileManager": {},
				"PagePicker": {
					"Title": "Вибрати веб-сторінку"
				}
			}
		}
	},
	"TemplatePicker": {
		"AppPickerDefault": "Вибрати App",
		"ContentTypePickerDefault": "Вибрати Тип Контенту",
		"Save": "Зберегти та закрити",
		"Cancel": "Відмінити зміни",
		"Close": "Закрити"
	}
}