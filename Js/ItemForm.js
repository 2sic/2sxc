var ToSexyContent = ToSexyContent || {};
ToSexyContent.ApplicationPath = null;
ToSexyContent.Services = ToSexyContent.Services || {};
ToSexyContent.Services.Ajax = function (methodName, opts) {
	///<summary>Call Webservice and return data-object</summary>
	var result;
	$.ajax({
		type: "POST",
		contentType: "application/json; charset=utf-8",
		url: ToSexyContent.ApplicationPath + "DesktopModules/ToSIC_SexyContent/API/Services.asmx/" + methodName,
		data: JSON.stringify(opts),
		async: false,
		success: function (data) {
			result = data.d;
		},
	});

	return result;
};
Eav.InitializeFormsReadyList.push(function () {	// init application Path
	ToSexyContent.ApplicationPath = $(".eav-form")[0].Controller.ApplicationPath;
	if (ToSexyContent.ApplicationPath[ToSexyContent.ApplicationPath.length - 1] != "/")	// ensure trailing slash
	    ToSexyContent.ApplicationPath += "/";
});

ToSexyContent.ItemForm = {
	Hyperlink: {
		OpenDialog: function (sender, dialogOpenerId, type, attributeStaticName, portalId, portalHomeDirectory) {
			switch (type) {
				case "ImageManager":
					$find(dialogOpenerId).open('ImageManager', { CssClasses: [], AttributeStaticName: attributeStaticName, PortalId: portalId, PortalHomeDirectory: portalHomeDirectory });
					break;
				case "FileManager":
					var args = new Telerik.Web.UI.EditorCommandEventArgs("DocumentManager", null, document.createElement("a"));
					args.CssClasses = [];
					args.AttributeStaticName = attributeStaticName;
					args.PortalId = portalId;
					args.PortalHomeDirectory = portalHomeDirectory;

					$find(dialogOpenerId).open('DocumentManager', args);
					break;
				case "PagePicker":
					ToSexyContent.ItemForm.Hyperlink._pagePicker.open(attributeStaticName, portalId);
					break;
			}
		},
		_pagePicker: {
			currentOpenField: null,
			currentOpenDialog: null,
			open: function (attributeStaticName, portalId) {
				var field = $(".eav-field[data-staticname='" + attributeStaticName + "']");
				var controller = field[0].Controller;
				var fieldController = controller.FieldController;
				var formController = controller.FormController;
				var currentDimension = Eav.Dimensions[formController.CurrentCultureDimension];
				var activeLanguage = currentDimension != null ? currentDimension.ExternalKey : null;

				var selectedTabId;
				try {
					selectedTabId = fieldController.GetValue().match(/(\w+):\s?([0-9]+)/i)[2];
				}
				catch (e) { selectedTabId = -1; }

				ToSexyContent.ItemForm.Hyperlink._pagePicker.currentOpenField = field;

				// use DNN 7.1 Picker (if available)
				var dnnPageDropDownList = field.find(".dnnDropDownList");
				if (dnnPageDropDownList.length == 1)
					ToSexyContent.ItemForm.Hyperlink._pagePicker.openDnn71Picker(fieldController, selectedTabId, activeLanguage, portalId, dnnPageDropDownList);
				else
					ToSexyContent.ItemForm.Hyperlink._pagePicker.openDnn6Picker(fieldController, selectedTabId, activeLanguage, portalId);
			},
			openDnn6Picker: function (fieldController, selectedTabId, activeLanguage, portalId) {
				var pickerContent = $('<div id="PagePicker" class="sc-hyperlink-pagepicker"><select id="ddlPagePicker" size="20"></select><div class="sc-hyperlink-pagepicker-buttons"><input id="btnUsePage" type="button" value="ok"/></div></div>');
				var pagePicker = pickerContent.find("#ddlPagePicker");

				ToSexyContent.ItemForm.Hyperlink._pagePicker.currentOpenDialog = pickerContent;

				pickerContent.dialog({
					modal: true, autoOpen: true, position: "center", dialogClass: "dnnFormPopup", title: "Select Page", width: 500, close: function () {
						pickerContent.dialog("destroy");
						pickerContent.remove();
					}
				});
				var portalTabs = ToSexyContent.Services.Ajax("GetPortalTabs", { portalId: portalId, activeLanguage: activeLanguage });
				$.each(portalTabs, function (i, tabInfo) {
					var item = $("<option/>").val(tabInfo.TabID).text(tabInfo.IndentedTabName);
					if (selectedTabId == tabInfo.TabID)
						item.prop("selected", true);
					pagePicker.append(item);
				});
				var useSelectedPage = function () {
					var selectedValue = pagePicker.val();
					var url = "";
					if (selectedValue && selectedValue != -1)
						url = "Page:" + selectedValue;
					fieldController.SetValue(url);
					pickerContent.dialog("destroy");
					pickerContent.remove();
				};
				pickerContent.find("#btnUsePage").click(useSelectedPage);
				pickerContent.find("#ddlPagePicker").dblclick(useSelectedPage);

			},
			openDnn71Picker: function (fieldController, selectedTabId, activeLanguage, portalId, dnnPageDropDownList) {
				var pickerContent = dnnPageDropDownList.parent();
				var objDnnPageDropDownList = dnn[dnnPageDropDownList.attr("id")];
				if (objDnnPageDropDownList._openItemList != undefined)	// works in DNN 7.2+
					objDnnPageDropDownList._openItemList(); // expand the treeView
				dnnPageDropDownList.find(".dt-container").width(498);	// set treeView width (hight can't be set)
				ToSexyContent.ItemForm.Hyperlink._pagePicker.currentOpenDialog = pickerContent;
				pickerContent.dialog({
					modal: true, autoOpen: true, position: "center", dialogClass: "dnnFormPopup", title: "Select Page", width: 500, height: 500, close: function () {
						ToSexyContent.ItemForm.Hyperlink._pagePicker.currentSetValue = null;
						pickerContent.dialog("destroy");
						//pickerContent.remove();	// don't remove dnnPageDropDownList from DOM
					}
				});
			},
			dnn71PickerSelectionChanged: function (selectedItem) {
				var selectedTabId = selectedItem.key;
				var url = "";
				if (selectedTabId != "")
					url = "Page:" + selectedTabId;
				ToSexyContent.ItemForm.Hyperlink._pagePicker.currentOpenField[0].Controller.FieldController.SetValue(url);
				ToSexyContent.ItemForm.Hyperlink._pagePicker.currentOpenDialog.dialog("destroy");
			}
		},
		ImageManagerCallback: function (sender, args) {
			if (!args)
				return;

			var path = args.value[0] == null ? args.value.getAttribute("src", 2) : args.value[0].getAttribute("src", 2);
			var url = path.indexOf("%") != -1 ? decodeURIComponent(path) : path;

			ToSexyContent.ItemForm.Hyperlink._fileManagerCallback(sender, args, url);
		},
		DocumentManagerCallback: function (sender, args) {
			if (!args)
				return;

			var url = args.value.getAttribute("href", 2);
			ToSexyContent.ItemForm.Hyperlink._fileManagerCallback(sender, args, url);
		},
		_fileManagerCallback: function (sender, args, url) {
			var resultValue = url;
			var fileId = ToSexyContent.ItemForm.Hyperlink.GetFileIdByUrl(sender.ClientParameters.PortalId, url.replace(sender.ClientParameters.PortalHomeDirectory, ""));
			if (fileId)
				resultValue = "File:" + fileId;

			var eavField = $(".eav-field[data-staticname='" + sender.ClientParameters.AttributeStaticName + "']");
			eavField[0].Controller.FieldController.SetValue(resultValue, { UpdateTestLink: false });
			ToSexyContent.ItemForm.Hyperlink.UpdateTestLink(eavField, url);
		},
		GetFileIdByUrl: function (portalId, relativePath) {
			var fileInfo = ToSexyContent.Services.Ajax("GetFileByPath", { portalId: portalId, relativePath: relativePath });
			try {
				return fileInfo.FileId;
			} catch (e) {
				return null;
			}
		},
		GetFilePathById: function (fileId, excludeDefaultPortal) {
			var fileInfo = ToSexyContent.Services.Ajax("GetFileById", { fileId: fileId });
			try {
				if (excludeDefaultPortal && fileInfo.PortalId == -1)
					return null;

				return fileInfo.RelativePath;
			} catch (e) {
				return null;
			}
		},
		UpdateTestLink: function (staticNameOrFieldRef, targetUrl) {
			var eavField;
			if (typeof staticNameOrFieldRef == "string")
				eavField = $(".eav-field[data-staticname='" + staticNameOrFieldRef + "']");
			else
				eavField = staticNameOrFieldRef;

			var testLink = eavField.find(".sc-hyperlink-testlink");
			var url = eavField[0].Controller.FieldController.GetValue();
			if (url) {
				var invalidFileUrl = "Invalid File";
				if (targetUrl == undefined) {
					targetUrl = url;
					var urlMatch = url.match(/(\w+):\s?([0-9]+)/i);
					if (urlMatch != null) {
						var eavFieldControl = eavField.find(".eav-field-control");
						switch (urlMatch[1].toLowerCase()) {
							case "file":
								var filePath = ToSexyContent.ItemForm.Hyperlink.GetFilePathById(urlMatch[2], true);
								if (filePath != null)
									targetUrl = eavFieldControl.attr("data-homedirectory") + filePath;
								else
									targetUrl = invalidFileUrl;
								break;
							case "page":
								targetUrl = ToSexyContent.ApplicationPath + "?TabId=" + urlMatch[2];
								break;
						}
					}
				}

				var linkText = targetUrl;
				if (linkText.length > 35)
					linkText = "..." + linkText.substr(linkText.length - 35);
				testLink.text(linkText).attr("title", targetUrl).attr("href", targetUrl != invalidFileUrl ? targetUrl : null).parent().show();
			}
			else
				testLink.parent().hide();
		}
	}
};


Eav.FieldControllerManager.hyperlink = function (objWrapper) {
	var controller = new Object();

	var field = objWrapper.find("input[type=text]");

	var pickerWrapper = objWrapper.find(".sc-hyperlink-picker-wrapper");
	controller.SetReadOnly = function (readOnlyState) {
		field.prop("disabled", readOnlyState);
		pickerWrapper.toggle(!readOnlyState);
	};
	controller.SetValue = function (value, args) {
		field.val(value);
		if (args == undefined)
			ToSexyContent.ItemForm.Hyperlink.UpdateTestLink($(field.context));
		else if (args.UpdateTestLink == true)
			ToSexyContent.ItemForm.Hyperlink.UpdateTestLink($(field.context), value);
	};
	controller.GetValue = function () {
		return field.val();
	};

	Eav.InitializeFormsReadyList.push(function () {
		// Update test link on value change
		field.bind("change keyup", function () {
			ToSexyContent.ItemForm.Hyperlink.UpdateTestLink($(field.context));
		});
		ToSexyContent.ItemForm.Hyperlink.UpdateTestLink($(field.context));
	});

	var menuBase = objWrapper.find(".eav-contextmenu");
	Eav._initContextMenu(menuBase, menuBase.find(".eav-contextmenu-actions"));

	return controller;
};

Eav.FieldControllerManager.wysiwyg = function (Controller, objWrapper) {
	// Check if CKEDITOR is used
	if (window.CKEDITOR) {
		var instanceId = objWrapper.find(".editor").get(0).id;

		Controller.SetReadOnly = function (readOnlyState) {

			var setReadOnly = function (editor, readyOnlyState) {
				editor.setReadOnly(readOnlyState);

				if (readOnlyState)
					editor.document.getBody().setStyle('background-color', '#EEE');
				else
					editor.document.getBody().setStyle('background-color', '');
			};

			// If the instance is ready, run now - else wait
			if (CKEDITOR.instances[instanceId] && CKEDITOR.instances[instanceId].instanceReady)
				setReadOnly(CKEDITOR.instances[instanceId], readOnlyState);
			else
				CKEDITOR.on('instanceReady', function (ev) {
					if (!CKEDITOR.instances[instanceId])
						return;
					setReadOnly(CKEDITOR.instances[instanceId], readOnlyState);
				});

		};
		Controller.SetValue = function (value) {
			// If the instance is not yet ready, set textarea, else via CKEditor API
			if (!CKEDITOR.instances[instanceId] || !CKEDITOR.instances[instanceId].instanceReady)
				objWrapper.find("textarea.editor").val(value);
			else {
				var editor = CKEDITOR.instances[instanceId];
				editor.setData(value);
				// After setting the data, set readOnlyState again - else the background color will be reset
				Controller.SetReadOnly(editor.readOnly);
			}
		};
		Controller.GetValue = function () {
			// If instance is not yet ready, get HTML out of the textarea, else via CKEditor API
			if (!CKEDITOR.instances[instanceId] || !CKEDITOR.instances[instanceId].instanceReady)
				return objWrapper.find("textarea.editor").val();
			var editor = CKEDITOR.instances[instanceId]; //ev.editor;
			return editor.getData();
		};
	}
	else {
		// Use default Telerik RadEditor
		var editor = $find(objWrapper.find(".RadEditor").get(0).id);
		Controller.SetReadOnly = function (readOnlyState) {
			// Bug DNN 7: Radeditor won't get disabled if this runs without timeout
			window.setTimeout(function () {
				// Bug in Radeditor: Must not set editable to the same value twice!
				if (!readOnlyState != editor.get_editable()) {
					editor.enableEditing(!readOnlyState);
					editor.set_editable(!readOnlyState);
					if (readOnlyState == true) editor.get_document().body.style.backgroundColor = "#EEE";
					else editor.get_document().body.style.backgroundColor = "";
				}
			}, 1);
		};
		Controller.SetValue = function (value) {
			editor.set_html(value);
		};
		Controller.GetValue = function () {
			return editor.get_html();
		};
	}

}
