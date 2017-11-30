{
	"Main": {
		"Title": "Administración",
		"Tab": {
			"GS": "Inicio",
			"GettingStarted": "Empezando",
			"CD": "Datos",
			"ContentData": "Contenido",
			"VT": "Vistas",
			"ViewsTemplates": "Vistas / Plantillas",
			"Q": "Consulta",
			"Query": "Diseñador de consultas",
			"WA": "WebApi",
			"WebApi": "WebApi / Datos",
			"IE": "",
			"ImportExport": "Importar / Exportar",
			"PL": "Global",
			"PortalLanguages": "Sitio web / Idiomas",
			"AS": "App",
			"AppSettings": "Configuración App"
		},
		"Portal": {
			"Title": "Configuración para todo el sitio web",
			"Intro": "Esta configuración se aplica a todo el contenido y todas las aplicaciones en este sitio web."
		}
	},
	"Templates": {
		"Title": "Gestión de plantillas",
		"InfoHideAdvanced": "Mejore la experiencia de usuario para los editores de contenido ocultando las características avanzadas. Si su sitio web contiene un rol llamado “2sxc designers”, quienes no pertenezcan a él no verán botones como Editar plantilla o Gestión de plantillas. Documentación How-To <a href='http://2sxc.org/en/help?tag=hide-advanced' target='_blank'>aquí</a>.",
		"Table": {
			"TName": "Nombre de plantilla",
			"TPath": "Ruta",
			"CType": "Tipo de contenido",
			"DemoC": "Elemento Demo",
			"Show": "Mostrar",
			"UrlKey": "Clave Url",
			"Actions": "Acciones"
		}
	},
	"TemplateEdit": {
		"Title": "Editar plantilla"
	},
	"WebAPIData": {
		"unused": {
			"Title": "Conseguir contenido como si fuesen datos para su plantilla o JavaScript",
			"Intro": "Use el diseñador de consultas para crear consultas complejas. Pueden usarse en plantillas normales (Token/Razor) o en JavaScript. O puede crear su JSON-WebAPI personalizado sólo para JavaScript - por ejemplo, para guardar algo, recuperar un archivo o ejecutar una consulta demasiado compleja para el diseñador. Aprenda más sobre <a href='http://2sxc.org/en/Docs/tag/Data%20and%20Data%20Sources' target='_blank'>codificando canales de datos</a> y sobre <a href='http://2sxc.org/en/Docs/tag/Data%20and%20Data%20Sources' target='_blank'>usando datos en JavaScript con jQuery, AngularJS y más</a>.",
			"Visual": {
				"Title": "Visual Data Query",
				"Intro": "Use el Diseñador Visual de Consultas (Pipeline-Designer) para crear consultas de datos desde 2sxc, SQL, RSS y más.",
				"Button": "Visual Query Designer"
			}
		}
	},
	"WebApi": {
		"Title": "WebApi para esta App",
		"Intro": "Cree una WebApi en minutos colocando el código fuente en la carpeta  API y heredando el interface adecuado. Inténtelo creando uno automáticamente y pulsando aquí. Aprenda más sobre <a href='http://2sxc.org/en/Docs-Manuals/Feature/feature/3361' target='_blank'>WebApi</a> o la <a href='http://2sxc.org/en/Docs-Manuals/Feature/feature/3360' target='_blank'>API de edición de datos C#</a>.",
		"ListTitle": "La siguiente lista muestra los archivos .cs en la carpeta App-API:",
		"InfoMissingFolder": "(el directorio no existe)",
		"QuickStart": "Para empezar rápidamente, recomendamos que instale la app demo de WebApi. Contiene algunos controladores de WebAPI con varias acciones y vistas de ejemplo para usar esos controladores. Descargue <a href='http://2sxc.org/en/Apps/tag/WebApi' target='_blank'>demos de WebApi en el App-Catalog</a> o aprenda más al respecto en <a href='http://2sxc.org/en/help?tag=webapi' target='_blank'>ayuda</a>",
		"AddDoesntExist": "aún no hay adición automática - hágalo automáticamente en la carpeta 'api'. Simplemente copie una de un proyecto existente y comience."
	},
	"ImportExport": {
		"Title": "Exportar o importar <em>partes</em> de esta App/Contenido",
		"Intro": "Crear un xml o zip conteniendo <em>partes</em> de esta app, para importar en otra app o contenido. O importar el paquete parcial.",
		"FurtherHelp": "Para encontrar más ayuda, visite <a href='http://2sxc.org/en/help?tag=import' target='_blank'>Ayuda 2sxc</a>.",
		"Buttons": {
			"Import": "importar",
			"Export": "exportar"
		},
		"Import": {
			"Title": "Importar una exportación de contenido (.zip) o una exportación parcial (.xml)",
			"Explanation": "Esta importación añadirá Tipos de Contenidos, Plantillas y elementos de contenido al contenido o app actual.",
			"Select": "seleccine archivo para importar",
			"Choose": "Elija archivo"
		},
		"Export": {
			"Title": "Exportación parcial de Content Types, Configuración de plantilla y contenido",
			"Intro": "Característica avanzada de exportación para exportar partes de este Contenido / App. Creará un archivo XML para que pueda ser importado en otro sitio o App",
			"FurtherHelp": "Para encontrar más ayuda, visite <a href='http://2sxc.org/en/help?tag=export' target='_blank'>Ayuda</a>.",
			"Data": {
				"GroupHeading": "Content Type: {{name}} ({{id}}",
				"Templates": "Plantillas",
				"Items": "Elementos de contenido",
				"SimpleTemplates": "Plantillas sin content type"
			},
			"ButtonExport": "Exportar"
		}
	},
	"Portal": {
		"Title": "Base de datos virtual (VDB)",
		"VdbLabel": "Base de datos virtual para este sitio web",
		"Rename": "Nota para 2tk - renombrar no es necesario: ¡no implementar!"
	},
	"Language": {
		"Title": "Idiomas / Culturas",
		"Intro": "Gestionar los idiomas activados / desactivados para esta zona (este sitio web)",
		"Table": {
			"Code": "Códido",
			"Culture": "Cultura",
			"Status": "Estado"
		}
	},
	"AppConfig": {
		"Title": "Configuración de App",
		"Intro": "Configurar la App y configuración especial.",
		"Settings": {
			"Title": "Configuración de App",
			"Intro": "Configuración para la app - como cadenas de conexión SQL, números predeterminados como \"elementos-para-mostrar\" y cosas así. También puede ser multi-lenguage, de forma que cada configuración (como la fuente RSS predeterminada) podría ser diferente para cada idioma.",
			"Edit": "editar configuración de app",
			"Config": "configuración de app"
		},
		"Resources": {
			"Title": "Recursos de App",
			"Intro": "Los recursos se usan para etiquetas y cosas por el estilo de la App. Suelen usarse para crear vistas multi-lenguaje y similares, y no deberían usarse para la configuración de la App.",
			"Edit": "editar recursos de app",
			"Config": "configurar recursos de app"
		},
		"Definition": {
			"Title": "Definición de paquete de App ",
			"Intro": "La definición del paquete de app es importante para exportar/importar esta app.",
			"Edit": "editar definición de app"
		},
		"Export": {
			"Title": "Exportar <em>toda</em> esta App",
			"Intro": "Creaar un paquete de app (zip) que podrá ser instalado en otro sitio web",
			"Button": "exportar"
		}
	},
	"AppManagement": {
		"Title": "Administrar Apps en esta zona (sitio web)",
		"Table": {
			"Name": "Nombre",
			"Folder": "Carpeta",
			"Templates": "Plantillas",
			"Show": "mostrar esta app a usuarios",
			"Actions": "Acciones"
		},
		"Buttons": {
			"Browse": "más apps",
			"Import": "importar app",
			"Create": "crear",
			"Export": "exportar app"
		},
		"Prompt": {
			"NewApp": "Escribir nombre de App (se usará también para la carpeta)",
			"DeleteApp": "Esto no puede deshacerse. Para borrar realmente esta app, escriba (o copie/pegue) el nombre de la app aquí: ¿seguro que quiere borrar '{{name}}' ({{id}}) ?",
			"FailedDelete": "el valor introducido no coincide - no se borrará"
		}
	},
	"ReplaceContent": {
		"Title": "Reemplazar elemento de contenido",
		"Intro": "Reemplazando un elemento de contenido, puede sustituir el contenido original que aparecía en determinado sitio.",
		"ChooseItem": "Elija elemento:"
	},
	"ReorderContentList": {
		"Title": "Reordenar lista de elementos de contenido",
		"Intro": "Ordene arrastrando a su conveniencia, después guarde"
	},
	"Edit": {
		"Fields": {
			"Hyperlink": {
				"Default": {
					"Tooltip1": "suelte aquí archivos para carga automática",
					"Tooltip2": "para más ayuda, visite 2sxc.org/help?tag=adam",
					"Tooltip3": "ADAM - cariñosamente patrocinado por 2sic.com",
					"AdamUploadLabel": "carga rápida usando ADAM",
					"MenuAdam": "Cargar archivo con Adam",
					"MenuPage": "Selector de página",
					"MenuImage": "Administrador de imágenes",
					"MenuDocs": "Administrador de documentos",
					"SponsoredLine": "<a href='http://2sxc.org/help?tag=adam' target='_blank' tooltip='ADAM es el Gestor Automático de Recursos Digitales - pulse para saber más'>Adam</a> está patrocinado con ♥ por <a tabindex='-1' href='http://2sic.com/' target='_blank'>2sic.com</a>"
				},
				"FileManager": {},
				"PagePicker": {
					"Title": "Seleccione una página web"
				}
			}
		}
	},
	"TemplatePicker": {
		"AppPickerDefault": "Elegir App",
		"ContentTypePickerDefault": "Elegir tipo de contenido",
		"Save": "Guardar y cerrar",
		"Cancel": "Cancelar cambios",
		"Close": "Cerrar"
	}
}