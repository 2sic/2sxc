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
    "Label": "Status:",
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
    "Saved": "gespeichert",
    "Saving": "saving...",
    "DebugEnabled": "debug aktiviert",
    "DebugDisabled": "debug deaktiviert",
    "SwitchedLanguageToDefault": "We have switched language to default {{language}} because it's missing some or all values"
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
    }
  },
  "Errors": {
    "UnsavedChanges": "Du hast Änderungen.",
    "SaveErrors": "Um zu speichern, bitte folgende Probleme beheben:"
  },
  "General": {
    "Buttons": {
      "NotSave": "Änderungen verwerfen",
      "Save": "speichern",
      "Debug": "debug"
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
    "Min": "Wert muss grösser als {{param.Min}} sein",
    "Max": "Wert muss kleiner als {{param.Max}} sein",
    "Pattern": "Bitte richtig eingeben",
    "Decimals": "Diese Zahl kann {{param.Decimals}} Dezimalstellen haben"
  },
  "Fields": {
    "Entity": {
      "Choose": "bestehendes Element auswählen",
      "New": "neues Element erstellen",
      "EntityNotFound": "(Element nicht gefunden)",
      "DragMove": "Ziehen um die Liste neu zu ordnen",
      "Edit": "Element bearbeiten",
      "Remove": "Aus Liste entfernen",
      "Delete": "Löschen"
    },
    "EntityQuery": {
      "QueryNoItems": "Keine Elemente gefunden",
      "QueryError": "Fehler: Die Abfrage konnte nicht ausgeführt werden. In der Konsole findest du genauere Details.",
      "QueryStreamNotFound": "Fehler: Die Abfrage enthielt keinen Stream mit dem Namen "
    },
    "Hyperlink": {
      "Default": {
        "Tooltip": "Dateien einfach hierher ziehen. Weitere Infos: 2sxc.org/help?tag=adam. ADAM - gesponsored ♡ von 2sic.com",
        "Sponsor": "ADAM - gesponsored ♡ von 2sic.com",
        "Fullscreen": "Grossdialog öffnen",
        "AdamTip": "schnell-upload mit ADAM",
        "PageTip": "Seite wählen",
        "MoreOptions": "mehr...",
        "MenuAdam": "Datei mit ADAM uploaden",
        "MenuPage": "Seitenauswahl",
        "MenuImage": "Bildverwaltung",
        "MenuDocs": "Dateiverwaltung"
      },
      "AdamFileManager": {
        "UploadLabel": "upload nach",
        "UploadTip": "schnell-upload mit ADAM",
        "UploadPasteLabel": "Bild aus Zwischenablage",
        "UploadPasteFocusedLabel": "Ctrl+V drücken",
        "UploadPasteTip": "Klick hier und drück [Ctrl]+[V] um aus der Zwischenablage einzufügen",
        "NewFolder": "Neuer Ordner",
        "NewFolderTip": "Neuen Ordner erstellen",
        "BackFolder": "Zurück",
        "BackFolderTip": "zum vorherigen Ordner",
        "Show": "In neuem Fenster öffnen",
        "Edit": "Umbenennen",
        "RenameQuestion": "Umbenennen:",
        "Delete": "Löschen",
        "DeleteQuestion": "Bist du sicher, dass die die Datei löschen möchtest?",
        "Hint": "Dateien hierher ziehen ",
        "SponsorTooltip": "ADAM ist der Automatic Digital Assets Manager - mehr erfahren",
        "SponsorLine": "mit ♡ gesponsort von"
      },
      "PagePicker": {
        "Title": "Seite auswählen"
      }
    },
    "DateTime": {
      "Open": "Kalendar öffnen",
      "Cancel": "Abbrechen",
      "Set": "Speichern"
    },
    "String": {
      "Dropdown": "Auswahl",
      "Freetext": "Freie Texteingabe"
    }
  },
  "Extension.TinyMce": {
    "Link.AdamFile": "ADAM-Datei verlinken (empfohlen)",
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
    "SwitchMode.Expand": "Vollbildmodus",
    "H1": "Ü1",
    "H2": "Ü2",
    "H3": "Ü3",
    "H4": "Ü4",
    "H5": "Ü5",
    "H6": "Ü6",
    "ContentBlock.Add": "App oder Inhaltsbaustein einfügen"
  }
}
