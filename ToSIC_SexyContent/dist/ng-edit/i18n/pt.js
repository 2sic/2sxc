{
  "Form": {
    "Buttons": {
      "Save": "GRAVAR (CTRL + S)",
      "Save.Tip": "gravar e fechar (CTRL + S grava mas não fecha)",
      "Exit.Tip": "sair - se alguma coisa foi alterada, vai-lhe ser questionado se quer gravar",
      "Return.Tip": "voltar ao formulário anterior"
    }
  },
  "SaveMode": {
    "Label": "Estado:",
    "show": "mostrar",
    "show.Tip": "as alterações são públicas",
    "hide": "ocultar",
    "hide.Tip": "este item não é visível publicamente",
    "branch": "rascunho",
    "branch.Tip": "as alterações são visíveis apenas a editores",
    "Dialog": {
      "Title": "Modo Gravação",
      "Intro": "Isto determina como vai gravar. O padrão é mostrar/publicar.",
      "Show": {
        "Title": "Mostrar / Publicar tudo",
        "Body": "Mostrar alterações ao público depois de gravar."
      },
      "Hide": {
        "Title": "Ocultar tudo",
        "Body": "Este item vai ser oculto e visível apenas a editores de conteúdos."
      },
      "Branch": {
        "Title": "Rascunho / Ocultar Alterações",
        "Body": "Apenas editores podem ver as alterações até serem publicadas em momento posterior."
      }
    }
  },
  "Message": {
    "Saved": "gravado",
    "Saving": "a gravar...",
    "DebugEnabled": "modo de depuração ativado",
    "DebugDisabled": "modo de depuração desativado",
    "SwitchedLanguageToDefault": "Alterámos a língua para a padrão {{language}} a atual tem alguns ou todos os valores em falta",
    "CantSwitchLanguage": "Não pode alterar a língua até a atual ter todos os valores necessários preenchidos"
  },
  "LangMenu": {
    "Translate": "Traduzir",
    "TranslateAll": "Traduzir todos",
    "NoTranslate": "Não traduzir",
    "NoTranslateAll": "Não traduzir nenhum",
    "Link": "Ligar a outra língua",
    "UseDefault": "automático (padrão)",
    "InAllLanguages": "em todas as línguas",
    "MissingDefaultLangValue": "por favor criar o valor na língua padrão {{languages}} antes de traduzir",
    "In": "em {{languages}}",
    "From": "de {{languages}}",
    "Dialog": {
      "Title": "Traduzir {{name}}",
      "Intro": "Pode fazer várias coisas ao traduzir, como ligar línguas.",
      "NoTranslate": {
        "Title": "Não traduzir",
        "Body": "usar valor da língua primária {{primary}}"
      },
      "FromPrimary": {
        "Title": "Traduzir de: {{primary}}",
        "Body": "iniciar tradução com o valor da língua primária"
      },
      "FromOther": {
        "Title": "Traduzir de: ...",
        "Body": "iniciar tradução com um valor de outra língua",
        "Subtitle": "Traduzir da língua"
      },
      "LinkReadOnly": {
        "Title": "Herdar de outra língua (ler-apenas)",
        "Body": "herdar valor de outra língua",
        "Subtitle": "Língua para herdar"
      },
      "LinkShared": {
        "Title": "Partilhar com outra língua (ler/escrever)",
        "Body": "ligar línguas para usarem o mesmo valor editável",
        "Subtitle": "Língua para partilhar"
      },
      "PickLanguageIntro": "Apenas línguas com conteúdos podem ser selecionadas."
    }
  },
  "Errors": {
    "UnsavedChanges": "Tem alterações por guardar.",
    "SaveErrors": "Para gravar o formulário, por favor corrija os seguintes erros:"
  },
  "General": {
    "Buttons": {
      "NotSave": "descartar alterações",
      "Save": "gravar",
      "Debug": "depurar"
    }
  },
  "Data": {
    "Delete.Question": "apagar '{{title}}' ({{id}})?"
  },
  "ItemCard": {
    "DefaultTitle": "Editar item",
    "SlotUsedTrue": "este item está aberto para edição. Clique aqui para tranca-lo / remove-lo e reverter ao valor padrão.",
    "SlotUsedFalse": "este item está trancado e vai permanecer vazia/padrão. Os valores são mostrados para sua conveniência. Clique aqui para destrancar se necessário."
  },
  "ValidationMessage": {
    "NotValid": "Inválido",
    "Required": "Isto é requerido",
    "RequiredShort": "requerido",
    "Min": "Este valor deve ser maior que {{param.Min}}",
    "Max": "Este valor deve ser inferior ou igual a {{param.Max}}",
    "Pattern": "Por favor adeque o formato ao requerido",
    "Decimals": "Este número pode ter até {{param.Decimals}} casas decimais"
  },
  "Fields": {
    "Entity": {
      "Choose": "adicionar item existente",
      "New": "criar novo",
      "EntityNotFound": "(item não encontrado)",
      "DragMove": "arraste para reordernar a lista",
      "Edit": "editar este item",
      "Remove": "remover da lista",
      "Delete": "apagar"
    },
    "EntityQuery": {
      "QueryNoItems": "Não foram encontrados items",
      "QueryError": "Erro: Ocorreu um erro ao executar a query. Veja o registo de erros para mais informação.",
      "QueryStreamNotFound": "Erro: A query não devolveu uma stream nomeada "
    },
    "Hyperlink": {
      "Default": {
        "Tooltip": "Arraste os ficheiros para aqui para serem carregados. Para ajuda ver 2sxc.org/help?tag=adam. ADAM - patrocinado com ♡ por 2sic.com",
        "Sponsor": "ADAM - patrocinado com ♡ por 2sic.com",
        "Fullscreen": "abrir em janela completa",
        "AdamTip": "carregar rapidamente através do ADAM",
        "PageTip": "escolha uma página",
        "MoreOptions": "mais...",
        "MenuAdam": "Carregar ficheiro com o Adam",
        "MenuPage": "Selector de Página",
        "MenuImage": "Gestor de Imagens",
        "MenuDocs": "Gestor de Ficheiros"
      },
      "AdamFileManager": {
        "UploadLabel": "carregar para",
        "UploadTip": "carregar rapidamente através do ADAM",
        "UploadPasteLabel": "colar imagem",
        "UploadPasteFocusedLabel": "carregue em ctrl+v",
        "UploadPasteTip": "clique aqui e carregue em [Ctrl]+[V] para copiar a imagem da área de transferência",
        "NewFolder": "Nova pasta",
        "NewFolderTip": "criar uma nova pasta",
        "BackFolder": "Voltar",
        "BackFolderTip": "voltar à pasta anterior",
        "Show": "Abrir num novo separador",
        "Edit": "Renomear",
        "RenameQuestion": "Renomear ficheiro/ pasta para:",
        "Delete": "Apagar",
        "DeleteQuestion": "Tem a certeza que quer apagar este ficheiro?",
        "Hint": "largar os ficheiros aqui",
        "SponsorTooltip": "ADAM é o Automatic Digital Assets Manager - clique para saber mais",
        "SponsorLine": "é patrocinado com ♡ por"
      },
      "PagePicker": {
        "Title": "Selecione uma página web"
      }
    },
    "DateTime": {
      "Open": "abrir calendário",
      "Cancel": "Cancelar",
      "Set": "Definir"
    },
    "String": {
      "Dropdown": "mudar para seleção vertical",
      "Freetext": "mudar para introdução de texto"
    }
  },
  "Extension.TinyMce": {
    "Link.AdamFile": "Ligar ficheiro ADAM (recommendado)",
    "Link.AdamFile.Tooltip": "Ligar com o ADAM - arraste apenas os ficheiros utilizando o Automatic Digital Assets Manager",
    "Image.AdamImage": "Inserir imagem ADAM (recommendado)",
    "Image.AdamImage.Tooltip": "Imagem do ADAM - arraste apenas os ficheiros utilizando o Automatic Digital Assets Manager",
    "Link.DnnFile": "Ligar ficheiro DNN",
    "Link.DnnFile.Tooltip": "Ligar um ficheiro DNN (todos os ficheiros, lento)",
    "Image.DnnImage": "Inserir imagem DNN",
    "Image.DnnImage.Tooltip": "Imagem do gestor de ficheiros DNN (todos os ficheiros, lento)",
    "Link.Page": "Ligar de outra página",
    "Link.Page.Tooltip": "Ligar uma página do site actual",
    "Link.Anchor.Tooltip": "Ancorar um link para utilizar .../pagina#nomedaancora",
    "SwitchMode.Pro": "Alterar para modo avançado",
    "SwitchMode.Standard": "Mudar para modo padrão",
    "SwitchMode.Expand": "Ecrã completo",
    "H1": "H1",
    "H2": "H2",
    "H3": "H3",
    "H4": "H4",
    "H5": "H5",
    "H6": "H6",
    "Paragraph": "Parágrafo",
    "ContentBlock.Add": "adicionar uma app bloco-conteúdo"
  }
}
