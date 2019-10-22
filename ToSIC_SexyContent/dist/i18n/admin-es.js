{
	"General": {
		"Note2Translator": "these are the main keys for buttons and short messages, used in various dialogs",
		"Buttons": {
			"Add": "añadir",
			"Cancel": "cancelar",
			"Copy": "copiar",
			"Delete": "borrar",
			"Edit": "editar",
			"ForceDelete": "forzar eliminar",
			"NotSave": "no guardar",
			"Permissions": "permisos",
			"Refresh": "refrescar",
			"Rename": "renombrar",
			"Save": "guardar",
			"System": "funciones avanzadas del sistema",
			"Metadata": "metadata"
		},
		"Messages": {
			"Loading": "cargando...",
			"NothingFound": "no se han encontrado elementos",
			"CantDelete": "no se puede borrar {{target}}"
		},
		"Questions": {
			"Delete": "¿seguro que quiere borrar {{target}}?",
			"DeleteEntity": "¿borrar '{{title}}' ({{id}}?",
			"SystemInput": "Esto es para operaciones muy avanzadas. Úselo sólo si sabe qué está haciendo. \n\n Escriba sentencias de administración:",
			"ForceDelete": "¿quiere forzar la eliminación '{{title}}' ({{id}})?"
		},
		"Terms": {
			"Title": "título"
		}
	},
	"DataType": {
		"All": {
			"Title": "Configuración general"
		},
		"Boolean": {
			"Short": "sí/no",
			"ShortTech": "Boolean",
			"Choice": "Booleano (sí/no)",
			"Explanation": "Valores sí/no o verdadero/falso"
		},
		"DateTime": {
			"Short": "fecha/hora",
			"ShortTech": "DateTime",
			"Choice": "Fecha y/o hora",
			"Explanation": "para fecha, hora o valores combinados"
		},
		"Entity": {
			"Short": "elemento(s)",
			"ShortTech": "Entity",
			"Choice": "Entidad (otros elementos de contenido)",
			"Explanation": "uno o más elementos de contenido"
		},
		"Hyperlink": {
			"Short": "enlace",
			"ShortTech": "Hyperlink",
			"Choice": "Enlace / referencia a archivo",
			"Explanation": "hiperenlace o referencia a una imagen / archivo"
		},
		"Number": {
			"Short": "número",
			"ShortTech": "Decimal",
			"Choice": "Número",
			"Explanation": "cualquier tipo de número"
		},
		"String": {
			"Short": "texto",
			"ShortTech": "String",
			"Choice": "Texto / cadena",
			"Explanation": "cualquier tipo de texto"
		},
		"Empty": {
			"Short": "vacío",
			"ShortTech": "Empty",
			"Choice": "Vacío - para títulos de formularios, etc.",
			"Explanation": "usar para estructurar su formularios"
		},
		"Custom": {
			"Short": "personalizado",
			"ShortTech": "Custom",
			"Choice": "Personalizado - herramientas de interface de usuario o tipos personalizados",
			"Explanation": "usar para cosas como selectores gps (que escribe múltiples campos) o para datos personalizados que serializan algo exótico en la bbdd como una matriz, un json personalizado o lo que sea "
		}
	},
	"ContentTypes": {
		"Title": "Content-Types y Datos",
		"TypesTable": {
			"Name": "Nombre",
			"Description": "Descripción",
			"Fields": "Campos",
			"Items": "Elementos",
			"Actions": ""
		},
		"TitleExportImport": "Exportar / Importar",
		"Buttons": {
			"Export": "exportar",
			"Import": "importar",
			"ChangeScope": "cambiar ámbito",
			"ChangeScopeQuestion": "Ésta es una característica avanzada para mostrar content-types de otro ámbito. No use esto si no sabe lo que está haciendo, pues los content-types de otros ámbitos normalmente se ocultar por una buena razón."
		},
		"Messages": {
			"SharedDefinition": "este content-type comparte la definición con #{{SharedDefId}} así que no puede editarlo aquí - lea 2sxc.org/help?tag=shared-types",
			"TypeOwn": "esto es un content-type propio, no usa la definición de otro content-type - lea 2sxc.org/help?tag=shared-types",
			"TypeShared": "este content-type hereda la definición de #{{SharedDefId}} - lea 2sxc.org/help?tag=shared-types"
		}
	},
	"ContentTypeEdit": {
		"Title": "Editar Content Type",
		"Name": "Nombre",
		"Description": "Descripción",
		"Scope": "Ámbito"
	},
	"Fields": {
		"Title": "Campos de Content Type",
		"TitleEdit": "Añadir campos",
		"Table": {
			"Title": "Título",
			"Name": "Nombre estático",
			"DataType": "Tipo de datos",
			"Label": "Etiqueta",
			"InputType": "Tipo de entrada",
			"Notes": "Notas",
			"Sort": "Orden",
			"Action": ""
		},
		"General": "General"
	},
	"Permissions": {
		"Title": "Permisos",
		"Table": {
			"Name": "Nombre",
			"Id": "ID",
			"Condition": "Condición",
			"Grant": "Permitido",
			"Actions": ""
		}
	},
	"Pipeline": {
		"Manage": {
			"Title": "Consultas visuales / Canales",
			"Intro": "Use el diseñador visual para crear consultas o combinar datos de varias fuentes. Esto puede ser usado en vistas o mediante JSON (si tiene permiso). <a href='http://2sxc.org/en/help?tag=visualquerydesigner' target='_blank'>leer más</a>",
			"Table": {
				"Id": "ID",
				"Name": "Nombre",
				"Description": "Descripción",
				"Actions": ""
			}
		},
		"Designer": {},
		"Stats": {
			"Title": "Resultados de la consulta",
			"Intro": "El resultado completo fue registrado en la consola del visor. Más abajo encontrará más información de depuración.",
			"ParamTitle": "Parámetros y estadísticas",
			"ExecutedIn": "Ejecutado en {{ms}}ms ({{ticks}} ticks)",
			"QueryTitle": "Resultados de consulta",
			"SourcesAndStreamsTitle": "Fuentes y flujos de datos",
			"Sources": {
				"Title": "Fuentes",
				"Guid": "Guid",
				"Type": "Tipo",
				"Config": "Configuración"
			},
			"Streams": {
				"Title": "Flujos de datos",
				"Source": "Fuentes",
				"Target": "Destino",
				"Items": "Elementos",
				"Error": "Error"
			}
		}
	},
	"Content": {
		"Manage": {
			"Title": "Gestión de contenido / Datos",
			"Table": {
				"Id": "ID",
				"Status": "Estado",
				"Title": "Título",
				"Actions": ""
			},
			"NoTitle": "- sin título -"
		},
		"Publish": {
			"PnV": "publicado y visible",
			"DoP": "esto es un borrador de otro elemento publicado",
			"D": "no publicado ahora",
			"HD": "tiene borrador: {{id}}",
			"HP": "reemplazará lo publicado"
		},
		"Export": {
			"Title": "Exportar contenido / Datos",
			"Help": "Esto generará un archivo XML que puede ser editadon con Excel. Si sólo quiere importar nuevos datos, use esto para exportar el esquema que podrá rellenar usando Excel. Visite <a href='http://2sxc.org/help' target='_blank'>http://2sxc.org/help</a> para encontrar más instrucciones.",
			"Commands": {
				"Export": "Exportar"
			},
			"Fields": {
				"Language": {
					"Label": "Idiomas",
					"Options": {
						"All": "Todos"
					}
				},
				"LanguageReferences": {
					"Label": "Referencias con valores para otros idiomas",
					"Options": {
						"Link": "Mantener referencias a otros idiomas (para re-importar)",
						"Resolve": "Reemplazar referencias con valores"
					}
				},
				"ResourcesReferences": {
					"Label": "Referencias archivo / página",
					"Options": {
						"Link": "Mantener referencias (para re-importar, por ejemplo Page:4711)",
						"Resolve": "Reemplazar referencias con URL reales (por ejemplo /Portals/0...)"
					}
				},
				"RecordExport": {
					"Label": "Exportar datos",
					"Options": {
						"Blank": "No, sólo exportar esquema de datos en blanco (para importación de nuevo datos)",
						"All": "Sí, exportar todos los elementos de contenido"
					}
				}
			}
		},
		"Import": {
			"Title": "Importar Contenido / Datos",
			"TitleSteps": "{{step}} de 3",
			"Help": "Esto importará elementos de contenido en 2sxc. Requiere que haya definido el content-type antes de intentar la importación, y que se haya creado el archivo de importación usando la plantilla proporcionada por la exportación. Visite <a href='http://2sxc.org/help' target='_blank'>http://2sxc.org/help</a> para más instrucciones.",
			"Fields": {
				"File": {
					"Label": "Elija archivo"
				},
				"ResourcesReferences": {
					"Label": "Referencias a páginas / archivos",
					"Options": {
						"Keep": "Importar enlaces cómo están escritos en el archivo (por ejemplo /Portals/...)",
						"Resolve": "Intentar resolver parches para referencias"
					}
				},
				"ClearEntities": {
					"Label": "Limpiar todas las demás entidades",
					"Options": {
						"None": "Mantener todas las entidades no encontradas en importación",
						"All": "Quitar todas las entidades no encontradas en importación"
					}
				}
			},
			"Commands": {
				"Preview": "Previsualizar importación",
				"Import": "Importar"
			},
			"Messages": {
				"BackupContentBefore": "¡Recuerde hacer copia de seguridad de DNN antes!",
				"WaitingForResponse": "Espere mientras se procesa...",
				"ImportSucceeded": "Importación terminada.",
				"ImportFailed": "Importación fallida.",
				"ImportCanTakeSomeTime": "Nota: La importación valida muchos datos y puede tomar bastante tiempo."
			},
			"Evaluation": {
				"Error": {
					"Title": "Intentar importar archivo '{{filename}}'",
					"Codes": {
						"0": "Error desconocido.",
						"1": "Content-type seleccionado no existe.",
						"2": "El documento no es un archivo XML correcto.",
						"3": "El content-type no se corresponde con el content-type en el archivo XML.",
						"4": "El idioma no está soportado.",
						"5": "El documento no especifica todos los idiomas para todas las entidades.",
						"6": "El idioma no puede ser analizado o no está soportado.",
						"7": "La referencia de idioma no puede ser analizada, la protección de sólo-lectura no está soportada.",
						"8": "No se puede leer el valor porque tiene un formato incorrecto."
					},
					"Detail": "Detalles: {{detail}}",
					"LineNumber": "Núm. línea: {{number}}",
					"LineDetail": "Detalles línea: {{detail}}"
				},
				"Detail": {
					"Title": "Intente importar el archivo '{{filename}}'",
					"File": {
						"Title": "El archivo contiene:",
						"ElementCount": "{{count}} elementos de contenido (registros/entidades)",
						"LanguageCount": "{{count}} idiomas",
						"Attributes": "{{count}} columnas: {{attributes}}"
					},
					"Entities": {
						"Title": "Si pulsa Importar, entonces:",
						"Create": "Crear {{count}} elementos de contenido",
						"Update": "Actualizar {{count}} elementos de contenido",
						"Delete": "Borrar {{count}} elementos de contenido",
						"AttributesIgnored": "Ignorar {{count}} columnas: {{attributes}}"
					}
				}
			}
		},
		"History": {
			"Title": "Historia de {{id}}",
			"Table": {
				"Id": "#",
				"When": "Cuándo",
				"User": "Usuario",
				"Cuándo": "When",
				"Usuario": "User",
				"Acciones": ""
			}
		}
	},
	"AdvancedMode": {
		"Info": {
			"Available": "esta ventana tiene un modo avanzado / depuración para usuarios avanzados - lea más en 2sxc.org/help?tag=debug-mode",
			"TurnOn": "modo avanzado / depuración activo",
			"TurnOff": "modo avanzado / depuración desactivado",
			"Disponible": "esta ventana tiene un modo avanzado / depuración para usuarios avanzados - lea más en 2sxc.org/help?tag=debug-mode"
		}
	}
}