{
  "Form": {
    "Buttons": {
      "Save": "Speichern (CTRL + S)",
      "SaveAndClose": "Speichern und schliessen",
      "Exit.Tip": "Verlassen - bei Änderungen wird zum Speichern aufgefordert",
      "Return.Tip": "Zurück zum vorherigen Dialog",
      "History.Tip": "Änderungen / Vorversionen",
      "Metadata.Tip": "Dies sind Metadaten für:",
      "Note": {
        "Add": "Notiz hinzufügen",
        "ItemNotSaved": "To add a note, please save item first"
      }
    }
  },
  "PublishStatus": {
    "Label": "Status:",
    "show": "anzeigen",
    "show.Tip": "Änderungen sind öffentlich",
    "hide": "verstecken",
    "hide.Tip": "Dieses Element ist nicht öffentlich",
    "branch": "entwurf",
    "branch.Tip": "Nur Redakteure sehen Änderungen",
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
    "Saved": "Gespeichert",
    "Saving": "Speichern...",
    "Deleted": "Gelöscht",
    "Deleting": "Löschen...",
    "DeleteError": "Löschen hat versagt. Bitte genauen Grund aus der Konsole entnehmen. ",
    "SwitchedLanguageToDefault": "Es fehlen Werte in der Primärsprache, deshalb wurde auf die Standardsprache {{language}} umgestellt.",
    "CantSwitchLanguage": "Sprachwechsel erst möglich wenn alle Pflichtfelder in der aktuellen Sprache befüllt sind"
  },
  "LangMenu": {
    "Translate": "Übersetzen",
    "TranslateAll": "Alles übersetzen",
    "NoTranslate": "Nicht übersetzen",
    "NoTranslateAll": "Keine übersetzen",
    "Link": "An andere Sprache verknüpfen",
    "UseDefault": "auto (Standardsprache)",
    "InAllLanguages": "in allen Sprachen",
    "MissingDefaultLangValue": "please create value in the default language {{languages}} before translating",
    "In": "in {{languages}}",
    "From": "von {{languages}}",
    "Dialog": {
      "Title": "{{name}} übersetzen",
      "Intro": "Du kannst auf verschiedene Arten übersetzen, auch Sprachen aneinander koppeln.",
      "NoTranslate": {
        "Title": "Nicht übersetzen",
        "Body": "Verwendet den Wert aus der Primärsprache {{primary}}"
      },
      "FromPrimary": {
        "Title": "Übersetzen von: {{primary}}",
        "Body": "Beginne beim Übersetzen mit dem Wert in der Primärsprache"
      },
      "FromOther": {
        "Title": "Übersetzen von: ...",
        "Body": "Beginne beim Übersetzen mit dem Wert in einer anderen Sprache",
        "Subtitle": "Vorlagensprache"
      },
      "LinkReadOnly": {
        "Title": "Von anderer Sprache erben (nur anzeigen)",
        "Body": "Nutzt den Wert einer anderen sprache, ist jedoch nicht bearbeitbar",
        "Subtitle": "Sprache, von der geerbt wird"
      },
      "LinkShared": {
        "Title": "Mit anderer Sprache verbinden (bearbeitbar)",
        "Body": "Verbindet Sprachen zusammen, damit sie den gleichen Wert verwenden",
        "Subtitle": "Sprache, die verbunden wird"
      },
      "PickLanguageIntro": "Nur Sprachen mit bestehendem Inhalt können ausgewählt werden."
    }
  },
  "Errors": {
    "UnsavedChanges": "Du hast Änderungen.",
    "SaveErrors": "Um zu speichern, bitte folgende Probleme beheben:",
    "FormulaConfiguration": "Es gibt einen Fehler bei den Formulareinstellungen. Bitte dem Admin melden",
    "FormulaCalculation": "Es gibt einen Fehler bei den Formularberechnungen. Bitte dem Admin melden"
  },
  "General": {
    "Buttons": {
      "NotSave": "Änderungen verwerfen",
      "Save": "Speichern",
      "Cancel": "Abbrechen"
    },
    "CopyHint": "Dies ist eine Kopie und wird als neues Element gespeichert",
    "ReadOnlyHint": {
      "Form": "Form is read only",
      "Language": "Language is read only"
    }
  },
  "Data": {
    "Delete.Question": "'{{title}}' ({{id}}) löschen?"
  },
  "ItemCard": {
    "DefaultTitle": "Element bearbeiten",
    "SlotUsedTrue": "Dieses Element kann bearbeitet werden. Klicke um die Bearbeitung zu sperren - eingegebene Werte werden dann mit den Standardwerten ersetzt.",
    "SlotUsedFalse": "Dieses Element kann nicht bearbeitet werden und zeigt die Standardwerte an. Klicke um das Element freizuschalten."
  },
  "ValidationMessage": {
    "NotValid": "Ungültig",
    "Required": "Pflichtfeld",
    "RequiredShort": "Pflichtfeld",
    "Min": "Dieser Wert sollte mindestens {{param.Min}} sein",
    "Max": "Dieser Wert sollte höchstens {{param.Max}} sein",
    "Pattern": "Bitte richtig eingeben",
    "Decimals": "Diese Zahl kann {{param.Decimals}} Dezimalstellen haben",
    "JsonError": "JSON ist nicht valide",
    "JsonWarning": "JSON ist nicht valide"
  },
  "Fields": {
    "Entity": {
      "Choose": "Bestehendes Element auswählen",
      "New": "Neues Element erstellen",
      "Empty": "Leer",
      "EmptySlot": "leerer Eintrag",
      "EntityNotFound": "(Element nicht gefunden)",
      "DragMove": "Ziehen um die Liste neu zu ordnen",
      "Edit": "Element bearbeiten",
      "Remove": "Aus Liste entfernen",
      "Delete": "Löschen",
      "Loading": "Lädt...",
      "Search": "Suchen"
    },
    "EntityQuery": {
      "QueryNoItems": "Keine Elemente gefunden",
      "QueryError": "Fehler: Die Abfrage konnte nicht ausgeführt werden. In der Konsole findest du genauere Details.",
      "QueryStreamNotFound": "Fehler: Die Abfrage enthielt keinen Stream mit dem Namen"
    },
    "Hyperlink": {
      "Default": {
        "Tooltip": "Dateien einfach hierher ziehen. Weitere Infos: 2sxc.org/help?tag=adam. ADAM - gesponsored ♡ von 2sic.com",
        "Sponsor": "ADAM - gesponsored ♡ von 2sic.com",
        "Fullscreen": "Grossdialog öffnen",
        "AdamTip": "Schnell upload mit ADAM",
        "PageTip": "Seite wählen",
        "MoreOptions": "Mehr...",
        "MenuAdam": "Datei mit ADAM uploaden",
        "MenuPage": "Seitenauswahl",
        "MenuImage": "Bildverwaltung",
        "MenuDocs": "Dateiverwaltung"
      },
      "AdamFileManager": {
        "Name": "ADAM",
        "UploadLabel": "Upload nach",
        "UploadTip": "Schnell upload mit ADAM",
        "UploadPasteLabel": "Bild aus Zwischenablage",
        "UploadPasteFocusedLabel": "CTRL + V drücken",
        "UploadPasteTip": "Klick hier und drück CTRL + V um aus der Zwischenablage einzufügen",
        "NewFolder": "Neuer Ordner",
        "NewFolderTip": "Neuen Ordner erstellen",
        "BackFolder": "Zurück",
        "BackFolderTip": "Zum vorherigen Ordner",
        "Show": "In neuem Fenster öffnen",
        "ImageSettings": "Bildeinstellungen",
        "ImageSettingsUnavailable": "Bildeinstellungen nicht verfügbar. Diese Datei ist kein Bild oder sie gehört nicht zu diesem Element",
        "ImageSettingsDisabled": "Bei diesem Feld sind Bildeinstellungen deaktiviert",
        "Edit": "Umbenennen",
        "RenameQuestion": "Umbenennen:",
        "Delete": "Löschen",
        "DeleteQuestion": "Bist du sicher, dass die die Datei löschen möchtest?",
        "Hint": "Dateien hierher ziehen ",
        "HelpTooltip": "ADAM ist der Automatic Digital Assets Manager - mehr erfahren",
        "SponsorLine": "mit ♡ gesponsort von"
      },
      "PagePicker": {
        "Title": "Seite auswählen"
      }
    },
    "DateTime": {
      "Open": "Kalendar öffnen"
    },
    "String": {
      "Dropdown": "Bestehendens auswählen",
      "Freetext": "Manuell eintragen"
    },
    "TemplatePicker": {
      "NotSelected": "(no file selected)",
      "NewTemplate": "Create a new file"
    }
  },
  "ManageContentList": {
    "Title": "Elementen-Liste verwalten",
    "Description": "Hier kannst du das Titel-Element verwalten (sofern definiert):",
    "NoHeader": "(diese Liste hat kein Titel-Element)",
    "SortItems": "Sortier die Einträge indem du sie mit der Maus rumziehst. ",
    "SortLotsOfItems": "Sortier die vielen Einträge indem du sie mit der Maus ziehst und dann mit dem Mausrad scrollst. "
  },
  "Extension.TinyMce": {
    "Link.AdamFile": "ADAM Datei verlinken (empfohlen)",
    "Link.AdamFile.Tooltip": "ADAM Dateien verlinken - Dateien einfach hierhin ziehen - verwendet den Automatic Digital Assets Manager",
    "Image.AdamImage": "ADAM Bild (empfohlen)",
    "Image.AdamImage.Tooltip": "ADAM Bild einfügen - Dateien einfach hierhin ziehen - verwendet den Automatic Digital Assets Manager",
    "Link.DnnFile": "DNN Datei verlinken",
    "Link.DnnFile.Tooltip": "DNN Datei verlinken (alle Dateien, langsam)",
    "Image.DnnImage": "DNN Bild",
    "Image.DnnImage.Tooltip": "DNN Bild einfügen (alle Dateien, langsam)",
    "Link.Page": "Seite verlinken",
    "Link.Page.Tooltip": "Eine Seite aus dieser Website verlinken",
    "Link.Anchor.Tooltip": "Texmarke (Anchor) für Verlinkung mit .../page#anchorname",
    "SwitchMode.Pro": "Zum Profi-Modus wechseln",
    "SwitchMode.Standard": "Zum Standard-Modus wechseln",
    "SwitchMode.Expand": "Vollbildmodus",
    "H1": "Ü1",
    "H2": "Ü2",
    "H3": "Ü3",
    "H4": "Ü4",
    "H5": "Ü5",
    "H6": "Ü6",
    "Paragraph": "Absatz",
    "ContentBlock.Add": "App oder Inhaltsbaustein einfügen"
  }
}
