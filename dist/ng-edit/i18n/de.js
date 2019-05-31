{
  "Form": {
    "Buttons": {
      "Save": "Speichern (CTRL + S)",
      "Save.Tip": "speichern und schliessen - mit CTRL + S nur speichern",
      "Exit.Tip": "verlassen - bei Änderungen wird zum Speichern aufgefordert",
      "Return.Tip": "zurück zum vorherigen Dialog"
    }
  },

  "SaveMode": {
    "show": "anzeigen",
    "show.Tip": "Änderungen sind öffentlich",
    "hide": "verstecken",
    "hide.Tip": "dieses Element ist nicht öffentlich",
    "branch": "entwurf",
    "branch.Tip": "nur Redakteure sehen Änderungen",
    "Dialog": {
      "Title": "Speichern",
      "Intro": "Entscheide, wie du speichern möchtest. Standard ist anzeigen / veröffentlichen.",
      "Show": {
        "Title": "Alles anzeigen / veröffentlichen",
        "Body": "Änderungen werden sofort öffentlich angezeigt (empfohlen)."
      },
      "Hide": {
        "Title": "Alles verstecken",
        "Body": "Das ganze element wird versteckt und ist nur sichtbar für Redakteure."
      },
      "Branch": {
        "Title": "Entwurf, Änderungen verstecken",
        "Body": "Nur Redakteure sehen Änderungen (bis es später veröffentlicht wird)."
      }
    }
	},
	"Message": {
		"Saved": "gespeichert"
  },

	"LangMenu": {
		"UseDefault": "auto (Standardsprache)",
		"In": "in {{languages}}",
		"From": "von {{languages}}",
    "Dialog": {
      "Title": "{{name}} übersetzen",
      "Intro": "Du kannst auf verschiedene Arten übersetzen, sogar Sprachen aneinander koppeln.",
      "NoTranslate": {
        "Title": "Nicht übersetzen",
        "Body": "verwendet den Wert aus der Primärsprache {{primary}}"
      },
      "FromPrimary": {
        "Title": "Übersetzen von: {{primary}}",
        "Body": "beginne beim Übersetzen mit dem Wert in der Primärsprache"
      },
      "FromOther": {
        "Title": "Übersetzen von: ...",
        "Body": "beginne beim Übersetzen mit dem Wert in einer anderen Sprache",
        "Subtitle": "Vorlagensprache"
      },
      "LinkReadOnly": {
        "Title": "Von anderer Sprache erben (nur anzeigen)",
        "Body": "nutzt den Wert einer anderen sprache, ist jedoch nicht bearbeitbar",
        "Subtitle": "Sprache, von der geerbt wird"
      },
      "LinkShared": {
        "Title": "Mit anderer Sprache verbinden (bearbeitbar)",
        "Body": "verbindet Sprachen zusammen, damit sie den gleichen Wert verwenden",
        "Subtitle": "Sprache, die verbunden wird"
      },
      "PickLanguageIntro": "Nur Sprachen mit bestehendem Inhalt können ausgewählt werden."
    },


    "//": "unsure if used, probably not",
		"EditableIn": "editable in {{languages}}",
		"AlsoUsedIn": ", also used in {{more}}",
		"NotImplemented": "This action is not implemented yet.",
		"CopyNotPossible": "Copy not possible: the field is disabled.",
		"Unlink": "translate (unlink)",
		"LinkDefault": "link to default",
		"GoogleTranslate": "machine-translate (Google)",
		"Copy": "copy from",
		"Use": "use from",
		"Share": "share from",
    "AllFields": "all fields"
	},


  "//": "Everything below this hasn't been verified yet - much may be unused",


	"Errors": {
		"UnclearError": "Da lief was falsch - vielleicht funktioniert ein Teil, vielleicht auch nicht. Entschuldigung :(",
		"InnerControlMustOverride": "Inner control must override this function.",
		"UnsavedChanges": "Du hast nicht gespeicherte Änderungen.",
		"DefLangNotFound": "Default language value not found, but found multiple values - can't handle editing for",
		"AdamUploadError": "Der Upload der Datei ist fehlgeschlagen. Möglicherweise ist die Datei zu gross."
	},
	"General": {},
	"EditEntity": {
		"DefaultTitle": "Element bearbeiten",
		"SlotUsedtrue": "Dieses Element kann bearbeitet werden. Klicke um die Bearbeitung zu sperren - eingegebene Werte werden dann mit den Standardwerten ersetzt.",
		"SlotUsedfalse": "Dieses Element kann nicht bearbeitet werden und zeigt die Standardwerte an. Klicke um das Element freizuschalten."
	},
	"FieldType": {
		"Entity": {
			"Choose": "-- Element auswählen --",
			"New": "-- Neues Element erstellen --",
			"EntityNotFound": "(Element nicht gefunden)",
			"DragMove": "Ziehen um die Liste neu anzuordnen",
			"Edit": "Dieses Element bearbeiten",
			"Remove": "Aus Liste entfernen"
		}
	},
	"LangWrapper": {
		"CreateValueInDefFirst": "Bitte das Feld '{{fieldname}}' zuerst in der Standardsprache befüllen vor dem Übersetzen."
	},
	"Dialog1": {},
	"dialog2": {},
	"CalendarPopup": {
		"ClearButton": "Löschen",
		"CloseButton": "Fertig",
		"CurrentButton": "Heute"
	},
	"Extension.TinyMce": {
		"Link.AdamFile": "ADAM-Datei (empfohlen)",
		"Link.AdamFile.Tooltip": "ADAM-Dateien verlinken - Dateien einfach hierhin ziehen - verwendet den Automatic Digital Assets Manager",
		"Image.AdamImage": "ADAM-Bild (empfohlen)",
		"Image.AdamImage.Tooltip": "ADAM-Bild einfügen - Dateien einfach hierhin ziehen - verwendet den Automatic Digital Assets Manager",
		"Link.DnnFile": "DNN-Datei verlinken",
		"Link.DnnFile.Tooltip": "DNN-Datei verlinken (alle Dateien, langsam)",
		"Image.DnnImage": "DNN-Bild",
		"Image.DnnImage.Tooltip": "DNN-Bild einfügen (alle Dateien, langsam)",
		"Link.Page": "Seite verlinken",
		"Link.Page.Tooltip": "Eine Seite aus dieser Website verlinken",
		"Link.Anchor.Tooltip": "Texmarke (Anchor) für Verlinkung mit .../page#anchorname",
		"SwitchMode.Pro": "Zum Profi-Modus wechseln",
		"SwitchMode.Standard": "Zum Standard-Modus wechseln",
		"H1": "Ü1",
		"H2": "Ü2",
		"H3": "Ü3",
		"Remove": "Entfernen",
		"ContentBlock.Add": "App oder Inhaltsbaustein einfügen"
	}
}
