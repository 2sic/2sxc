Eav = {
    FormsInitialized: false,
    FieldControllerManager: new Object(),
    Dimensions: new Object(),
    InitializeFormsReadyList: [],
    InitializeForms: function () {
        // Initialize if not already done
        if (Eav.FormsInitialized == null || !Eav.FormsInitialized) {
            // Hide Save Button until Eav is initialized
            $("." + Eav.CssClasses.SaveButtonClass).hide();

            // Create Dimensions Model
            $.each(EavDimensionsModel, function (i, dimension) {
                Eav.Dimensions[dimension.DimensionId] = dimension;
            });

            // Loop through each Eav form wrapper
            $(".eav-form").each(function (i, form) {
                Eav.AttachFormController(form);
            });

            // call aditional init methods
            var fn, i = 0;
            while ((fn = Eav.InitializeFormsReadyList[i++])) {
                fn.call(document, $);
            }

            // Initialization done, set Initialized to true and show save button again
            Eav.FormsInitialized = true;
            $("." + Eav.CssClasses.SaveButtonClass).show();
        }
    },

    CssClasses: {
        LockedClass: "eav-dimension-locked",
        ButtonDisabledClass: "disabled",
        MenuWrapper: "eav-dimensionmenu-wrapper",
        SaveButtonClass: "eav-save"
    },

    AttachFormController: function (form) {

        var formController = $(form).get(0).Controller = {
            DefaultCultureDimension: parseInt($(form).attr("data-defaultculturedimension")),
            CurrentCultureDimension: parseInt($(form).attr("data-activeculturedimension")),
            ApplicationPath: $(form).attr("data-applicationpath"),

            TranslateAll: function () {
                $.each(formController.EntityModel.Attributes, function (i, e) {
                    e.Controller.Unlink();
                });
            },

            UseDefaultAll: function () {
                $.each(formController.EntityModel.Attributes, function (i, e) {
                    e.Controller.UseDefault();
                });
            },

            ShareFromAll: function (dimensionId) {
                formController.RunForAllAttributesIfRequirement(dimensionId, "CanShareDimension", "Share");
            },

            UseFromAll: function (dimensionId) {
                formController.RunForAllAttributesIfRequirement(dimensionId, "CanUseDimension", "UseFrom");
            },

            CopyFromAll: function (dimensionId) {
                formController.RunForAllAttributesIfRequirement(dimensionId, "CanCopyDimension", "Copy");
            },

            RunForAllAttributesIfRequirement: function (dimensionId, requirement, functionToRun) {
                var can = [];
                var canNot = [];

                $.each(formController.EntityModel.Attributes, function (i, e) {
                    if (e.Controller[requirement](dimensionId))
                        can.push(e);
                    else
                        canNot.push(e);
                });

                var canString = "\n\nWill apply to:\n- " + can.length == 0 ? "(none)" : $.map(can, function (e) { return e.Controller.StaticName; }).join('\n- ');
                var canNotString = "\n\nWill not change:\n- " + canNot.length == 0 ? "(none)" : $.map(canNot, function (e) { return e.Controller.StaticName; }).join('\n- ');
                var confirmString = "Not all fields are available in " + Eav.Dimensions[dimensionId].Name + "." + canString + canNotString;

                if (canNot.length == 0 || confirm(confirmString)) {
                    $.each(can, function (i, e) {
                        e.Controller[functionToRun](dimensionId);
                    });
                }
            },

            IsChanged: function () {
                var changed = false;

                // If there are no form fields, return false
                if ($(form).find(".eav-field").size() == 0)
                    return false;

                // If Entity does not exist yet, changes won't be tracked
                if (formController.EntityModel == null)
                    return true;

                $.each(formController.EntityModel.Attributes, function (i, e) {
                    if (e.Controller.IsChanged())
                        changed = true;
                });
                return changed;
            }
        };

        // If the current language is not the master language, attach Controllers etc.
        if ((formController.CurrentCultureDimension != formController.DefaultCultureDimension) && !isNaN(formController.DefaultCultureDimension))
            $(".eav-message-defaultfirst").html("Edit this in the default language <i>" + Eav.Dimensions[formController.DefaultCultureDimension].Name + "</i> before translating.");

        // Set entityModel to form if available
        if (window.EavEntityModels != null) {
            var entityModelJson = EavEntityModels[$(form).attr("data-entityid")];
            if (entityModelJson != null) {
                formController.EntityModel = Eav.Entities.CreateEntityModel(entityModelJson);
            }
        }

        // Loop through each Eav field in form and attach controller
        $(".eav-field", form).each(function (i, field) {
            Eav.AttachFieldController($(field), $(form));
        });
    },

    // Attaches a controller to the target (Eav field)
    AttachFieldController: function (field, form) {

        // Create controller object
        var controller = field.get(0).Controller = {
            // Data-attributes
            StaticName: field.attr("data-staticname"),
            FieldIsMasterRecord: field.attr("data-ismasterrecord") == 'true',
            MenuWrapper: field.find("." + Eav.CssClasses.MenuWrapper),
            FieldType: field.attr("data-fieldtype"),
            FieldIsEnabled: field.attr("data-enabled") == 'true',
            FormController: form.get(0).Controller,

            // Original Values
            OriginalValue: "",
            OriginalValueId: "",
            OriginalReadOnlyState: "",

            // Hidden fields
            HiddenFieldReadOnly: field.find("input[type=hidden][id$='_hfReadOnly']"),
            HiddenFieldValueId: field.find("input[type=hidden][id$='_hfValueId']"),

            SetReadOnly: function (readOnlyState) {
                controller.FieldController.SetReadOnly(readOnlyState);
                controller.HiddenFieldReadOnly.val(readOnlyState);
                controller.AppendDimensionsMenu();
            },

            GetReadOnly: function () {
                return controller.HiddenFieldReadOnly.val() == 'true';
            },

            SetFieldValue: function (value) {
                controller.FieldController.SetValue(value);
            },

            GetFieldValue: function () {
                return controller.FieldController.GetValue();
            },

            SetFieldValueByValueId: function (valueId) {
                controller.SetFieldValue(controller.AttributeModel.Values[valueId].Value);
            },

            SetValueId: function (valueId) {
                controller.HiddenFieldValueId.val(valueId);
                if (valueId != "" && valueId != null)
                    controller.SetFieldValueByValueId(valueId);
                controller.AppendDimensionsMenu();
            },

            GetValueId: function () {
                return controller.HiddenFieldValueId.val();
            },

            GetCurrentValueModel: function () {
                return controller.AttributeModel.Values[controller.GetValueId()];
            },

            UseFrom: function (dimensionId) {
                var valueId = controller.AttributeModel.GetValueIdByDimensionId(dimensionId);
                controller.SaveFieldValueBeforeIfTranslated();
                controller.SetReadOnly(true);
                controller.SetValueId(valueId);
            },

            Share: function (dimensionId) {
                var valueId = controller.AttributeModel.GetValueIdByDimensionId(dimensionId);
                controller.SaveFieldValueBeforeIfTranslated();
                controller.SetReadOnly(false);
                controller.SetValueId(valueId);
            },

            Copy: function (dimensionId) {
                // Unlink if currently read-only
                if (controller.GetReadOnly())
                    controller.Unlink();

                var valueId = controller.AttributeModel.GetValueIdByDimensionId(dimensionId);
                controller.SetFieldValueByValueId(valueId);
            },

            AutoTranslate: function (dimensionId) {

                // Get value from model
                var valueId = controller.AttributeModel.GetValueIdByDimensionId(dimensionId);
                var value = controller.AttributeModel.Values[valueId].Value;

                // Fix some HTML that could cause issues
                value = value.replace("><", "> <");

                // Prepare taget and source language
                var sourceLanguage = Eav.Dimensions[dimensionId].ExternalKey.substring(0, 2);
                var targetLanguage = Eav.Dimensions[form.get(0).Controller.CurrentCultureDimension].ExternalKey.substring(0, 2);

                var yqlUrl = "http://query.yahooapis.com/v1/public/yql?format=json";
                //var data = "q=" + encodeURIComponent('select * from json where url="http://translate.google.de/translate_a/t?client=t&text=Translate&hl=de&sl=en&tl=de&ie=UTF-8&oe=UTF-8&multires=1&otf=2&ssel=0&tsel=0"');

                var data = "q=" + encodeURIComponent("use 'https://raw.github.com/yql/yql-tables/master/data/jsonpost.xml' as jsonpost; "
                + "select * from jsonpost where "
                + "url='http://translate.google.de/translate_a/t' "
                + "and postdata='client=t&text=" + encodeURIComponent(value).replace('\'', '\\\'') + "&hl=en&sl=" + sourceLanguage + "&tl=" + targetLanguage + "&ie=UTF-8&oe=UTF-8&multires=1&otf=2&ssel=0&tsel=0'");

                jQuery.ajax({
                    async: true,
                    crossDomain: true,
                    url: yqlUrl,
                    data: data,
                    cache: false,
                    success: function (data) {
                        // data is the JSON response
                        // process the response here
                        if (typeof data == "string")
                            data = $.parseJSON(data);

                        var translationResults = data.query.results;
                        if (translationResults != null) {
                            var translationValue = "";

                            $.each(eval(translationResults.postresult)[0], function (i, v) {
                                translationValue += v[0];
                            });

                            // Fix some things Google adds to HTML
                            translationValue = translationValue.replace("</ ", "</");
                            translationValue = translationValue.replace("< / ", "</");
                            translationValue = translationValue.replace("< /", "</");
                            translationValue = translationValue.replace(" >", ">");

                            controller.SetFieldValue(translationValue);

                            // Unlink if currently read-only
                            if (controller.GetReadOnly())
                                controller.Unlink();
                        }
                    },
                    error: function (e) {
                        alert("An error occurred while translating the text. Please try again.");
                    }
                });
            },

            UseDefault: function () {
                controller.SaveFieldValueBeforeIfTranslated();
                controller.SetReadOnly(true);
                controller.SetValueId("");
                controller.SetFieldValueByValueId(controller.AttributeModel.GetValueIdByDimensionId(form.get(0).Controller.DefaultCultureDimension));
            },

            Unlink: function () {
                controller.SetReadOnly(false);
                controller.SetValueId("");

                if (controller.FieldValueBefore != null) {
                    controller.SetFieldValue(controller.FieldValueBefore);
                    controller.FieldValueBefore = null;
                }
            },

            SaveFieldValueBeforeIfTranslated: function () {
                if (controller.GetState() == "Translate")
                    controller.FieldValueBefore = controller.GetFieldValue();
            },

            // Hide the control only
            HideControl: function () {
                field.find(".eav-field-control").hide();

                // Run field-specific HideControl function if exists
                if (controller.FieldController.HideControl != null)
                    controller.FieldController.HideControl();
            },

            AppendDimensionsMenu: function () {
                if (!controller.FieldIsMasterRecord)
                    Eav.AppendDimensionsMenu(field, form);
            },

            GetState: function () {
                if (controller.GetValueId() == "" && controller.GetReadOnly())
                    return "Default";
                if (controller.GetValueId() != "" && controller.GetReadOnly())
                    return "Use";

                if (controller.GetValueId() == "" && !controller.GetReadOnly())
                    return "Translate";

                if (controller.GetValueId() != "" && !controller.GetReadOnly()) {
                    if (controller.GetCurrentValueModel().GetDimensionsCount() == 1 && controller.GetCurrentValueModel().HasDimension(form.get(0).Controller.CurrentCultureDimension))
                        return "Translate";
                    else
                        return "Share";
                }
            },

            CanUseDimension: function (dimensionId) {
                return controller.AttributeModel.HasDimension(dimensionId) && dimensionId != form.get(0).Controller.CurrentCultureDimension;
            },

            CanShareDimension: function (dimensionId) {
                return controller.AttributeModel.HasDimension(dimensionId) && dimensionId != form.get(0).Controller.CurrentCultureDimension;
            },

            CanCopyDimension: function (dimensionId) {
                return controller.AttributeModel.HasDimension(dimensionId);
            },

            IsChanged: function () {
                return (controller.OriginalReadOnlyState != controller.GetReadOnly()) ||
                        (controller.OriginalValue != controller.GetFieldValue()) ||
                        (controller.OriginalValueId != controller.GetValueId());
            }
        };

        // Attach field-specific controllers
        controller.FieldController = Eav.FieldControllerManager[field.get(0).Controller.FieldType.toLowerCase()](field);

        if (controller.FieldIsEnabled) {
            // Attach AttributeModel for this field
            if (form.get(0).Controller.EntityModel != null)
                controller.AttributeModel = form.get(0).Controller.EntityModel.Attributes[controller.StaticName];
            // Append Controller to AttributeModel
            if (controller.AttributeModel != null)
                controller.AttributeModel.Controller = controller;

            // Set field to read-only if required
            controller.SetReadOnly(controller.GetReadOnly());

            // Set current field values for later comparison (IsChanged) - ReadOnly, Value, ValueId
            controller.OriginalReadOnlyState = controller.GetReadOnly();
            controller.OriginalValue = controller.GetFieldValue();
            controller.OriginalValueId = controller.GetValueId();

            // Append Dimensions Menu for this field
            controller.AppendDimensionsMenu(field, form);

            // Let the lock icon jump on hover of field
            field.find(".eav-field-control").mouseenter(function () {
                if (controller.GetReadOnly()) {
                    field.find(".eav-dimension-lockicon").animate({ "top": "-7px" }, 150).animate({ "top": "0px" }, 150);
                    field.find(".eav-dimension-message").animate({ "left": "-3px" }, 100).animate({ "left": "3px" }, 100).animate({ "left": "0px" }, 100);
                }
            });

        } else {
            field.find(".eav-field-control:first").after("<span class='eav-dimension-defaultfirst'>create this in <i>" + Eav.Dimensions[form.get(0).Controller.DefaultCultureDimension].Name + "</i> before translating</span>");
            controller.HideControl();
        }
    },

    AppendDimensionsMenu: function (field, form) {
        var fieldController = field.get(0).Controller;
        var formController = form.get(0).Controller;
        field.find(".eav-dimension").remove();

        var menuBase = $("<div class='eav-dimension'><a class='eav-dimension-lockicon' href='javascript:void(0);'></a><span class='eav-dimension-message'>&nbsp;</span><ul class='eav-dimension-actions' /></div>");
        var menu = menuBase.find(".eav-dimension-actions");

        menuBase.find(".eav-dimension-lockicon").bind("click", function () {
            if (fieldController.GetState() == "Translate")
                fieldController.UseDefault();
            else
                fieldController.Unlink();
        });

        // Append Translate button
        if (fieldController.GetState() != "Translate")
            Eav.AppendDimensionsMenuItem(menu, "Translate (unlink)", fieldController.Unlink, true);

        if (fieldController.GetState() != "Default")
            Eav.AppendDimensionsMenuItem(menu, "Use default", fieldController.UseDefault, true);

        var autoTranslateParent = Eav.AppendDimensionsMenuItem(menu, "Google-Translate from", fieldController.UseDefault, true);

        // Append "Copy" menu
        var copyFromParent = Eav.AppendDimensionsMenuItem(menu, "Copy from", null, true);
        // Append "Use" menu
        var useFromParent = Eav.AppendDimensionsMenuItem(menu, "Use from", null, true);
        // Append "Share" menu
        var shareFromParent = Eav.AppendDimensionsMenuItem(menu, "Share from", null, true);


        // Append "All Items" menu
        var allItemsParent = Eav.AppendDimensionsMenuItem(menu, "All fields", null, true);
        Eav.AppendDimensionsMenuItem(allItemsParent, "Translate", formController.TranslateAll, true);
        Eav.AppendDimensionsMenuItem(allItemsParent, "Use default", formController.UseDefaultAll, true);

        var copyAllItemsParent = Eav.AppendDimensionsMenuItem(allItemsParent, "Copy from", null, true);
        var useAllItemsParent = Eav.AppendDimensionsMenuItem(allItemsParent, "Use from", null, true);
        var shareAllItemsParent = Eav.AppendDimensionsMenuItem(allItemsParent, "Share from", null, true);


        // Loop Dimensions and append to menus
        $.each(Eav.Dimensions, function (i, dimension) {
            Eav.AppendDimensionsMenuItem(autoTranslateParent, dimension.Name, function () { fieldController.AutoTranslate(dimension.DimensionId); }, fieldController.AttributeModel.HasDimension(dimension.DimensionId));
            Eav.AppendDimensionsMenuItem(copyFromParent, dimension.Name, function () { fieldController.Copy(dimension.DimensionId); }, fieldController.CanCopyDimension(dimension.DimensionId));
            Eav.AppendDimensionsMenuItem(useFromParent, dimension.Name, function () { fieldController.UseFrom(dimension.DimensionId); }, fieldController.CanUseDimension(dimension.DimensionId));
            Eav.AppendDimensionsMenuItem(shareFromParent, dimension.Name, function () { fieldController.Share(dimension.DimensionId); }, fieldController.CanShareDimension(dimension.DimensionId));

            Eav.AppendDimensionsMenuItem(copyAllItemsParent, dimension.Name, function () { formController.CopyFromAll(dimension.DimensionId); }, true);
            Eav.AppendDimensionsMenuItem(useAllItemsParent, dimension.Name, function () { formController.UseFromAll(dimension.DimensionId); }, true);
            Eav.AppendDimensionsMenuItem(shareAllItemsParent, dimension.Name, function () { formController.ShareFromAll(dimension.DimensionId); }, true);
        });

        // Append locked class if read-only
        menuBase.toggleClass(Eav.CssClasses.LockedClass, fieldController.GetReadOnly());

        Eav._initContextMenu(menuBase, menu, true);

        // Append message
        Eav.GetDimensionsMenuMessage($(".eav-dimension-message", menuBase), field, form);

        fieldController.MenuWrapper.append(menuBase);
    },
    _initContextMenu: function (menuBase, menu, dimensionsMenu) {
        var menuClass = dimensionsMenu ? "eav-dimension" : "eav-contextmenu";

        // Set hover functions to open the menu
        menuBase.add(menu.find("li")).hover(function () {
            var hoverInterval = $(this).data("hoverTimeout");
            if (hoverInterval != null)
                window.clearTimeout(hoverInterval);

            // Hide ul's on same level, hide hovered li > ul
            $(this).parent().find("> li > ul").not($(this).children("ul")).hide();
            $(this).children("ul").show();

            // If the menu was hovered, close all other open menus
            if ($(this).hasClass(menuClass))
                $(this).parents(".eav-form").find("." + menuClass + "-actions").not($(this).find("." + menuClass + "-actions")).hide();
        }, function () {
            var element = $(this);
            var hoverInterval = window.setTimeout(function () {
                element.children("ul").hide();
            }, 300);
            element.data("hoverTimeout", hoverInterval);
        });

    },
    AppendDimensionsMenuItem: function (wrapper, text, action, active) {
        if (wrapper.is("li")) {
            wrapper.addClass("haschildren");
            if ($("ul", wrapper).size() == 0) {
                wrapper.append("<ul />");
            }
            wrapper = wrapper.find("ul:first");
        }

        var menuItem = $("<li />");

        var anchor = $("<a href='javascript:void(0);' />").text(text);

        if (!active)
            anchor.addClass("disabled");

        anchor.bind("click", function () {
            if (!$(this).hasClass("disabled"))
                anchor.trigger("eav-action");
        });

        if (action != null)
            anchor.bind("eav-action", action);

        menuItem.append(anchor);
        wrapper.append(menuItem);
        return menuItem;
    },

    GetDimensionsMenuMessage: function (target, field, form) {
        fieldController = field.get(0).Controller;
        var message = "&nbsp;";
        var toolTip = "";
        var valueModel = fieldController.GetCurrentValueModel();

        if (valueModel != null && fieldController.GetState() != "Translate") {
            var dimensions = $.map(fieldController.GetCurrentValueModel().Dimensions, function (i) { return i; });
            dimensions = $(dimensions).filter(function (i, e) {
                return e.DimensionId != form.get(0).Controller.CurrentCultureDimension;
            });

            var sharedDimensions = $(dimensions).filter(function (i, e) { return !e.ReadOnly; });
            var usedDimensions = $(dimensions).filter(function (i, e) { return e.ReadOnly; });

            var sharedDimensionsString = "";
            if ($(sharedDimensions).size() != 0) {
                sharedDimensionsString += $.map(sharedDimensions, function (e) {
                    return Eav.Dimensions[e.DimensionId].ExternalKey;
                }).join(', ');
            }

            var usedDimensionsString = "";
            if ($(usedDimensions).size() != 0) {
                usedDimensionsString += $.map(usedDimensions, function (e) {
                    return Eav.Dimensions[e.DimensionId].ExternalKey;
                }).join(', ');
            }

            // Build message
            if (sharedDimensionsString != "" || usedDimensionsString != "") {
                message = "in <strong>" + sharedDimensionsString + "</strong>";
                message += (sharedDimensionsString != "" && usedDimensionsString != "" ? ", " : "") + usedDimensionsString;

                toolTip += sharedDimensionsString != "" ? "editable in " + sharedDimensionsString : "";
                toolTip += usedDimensionsString != "" ? (toolTip == "" ? "" : "\r\n") + "also used in " + usedDimensionsString : "";
            }


        }
        else if (fieldController.GetReadOnly() == true) {
            message = "auto (default)";
        }

        $(target).html(message).attr("title", toolTip);
    },
    /*InitEntityMultiSelector: function (wrapper, reinit) {
		var multiValuesWrapper = wrapper.find(".MultiValuesWrapper");
		var baseDropDown = wrapper.find("select");
		if (reinit) {
			alert("Reinit Entity-Multi-Selector is untested!");
			multiValuesWrapper.find(".MultiValueItem .MultiValueItem").remove();
			baseDropDown.val("");
		} else {
			wrapper.find("a.AddValue").click(function () {
				Eav.AddEntityMultiSelector(wrapper, multiValuesWrapper, baseDropDown);
			});
			baseDropDown.change(function () { Eav.SyncSelectedEntities(wrapper); });
		}

		var entityIds = wrapper.find("input[type=hidden][id$='_hfEntityIds']").val().split(",");
		$.each(entityIds, function (i, entityId) {
			if (i == 0)
				baseDropDown.val(entityId);
			else
				Eav.AddEntityMultiSelector(wrapper, multiValuesWrapper, baseDropDown, i, entityId);
		});
	},
	AddEntityMultiSelector: function (wrapper, multiValuesWrapper, baseDropDown, i, selectedValue) {
		if (i == undefined)
			i = multiValuesWrapper.find(".MultiValueItem").length + 1;
		var elementId = multiValuesWrapper.attr("id").replace("_pnlMultiValues", "_item" + i);
		multiValuesWrapper.append($('<div class="MultiValueItem" id="' + elementId + '"><a class="eav-entityrelationship-remove" href="javascript:void(0)" onclick="javascript:Eav.RemoveEntityMultiSelect(this)">remove</a></div>').prepend(baseDropDown.clone().attr("id", elementId + "_DropDown").removeAttr("name")));
		var dropDown = multiValuesWrapper.find("#" + elementId + "_DropDown");
		dropDown.val(selectedValue);
		dropDown.change(function () { Eav.SyncSelectedEntities(wrapper); });
	},
	SyncSelectedEntities: function (wrapper) {
		var entityIds = [];
		$.each(wrapper.find("select"), function (i, ddl) {
			var value = $(ddl).val();
			if (value != "")
				entityIds.push(value);
		});
		wrapper.find("input[type=hidden][id$='_hfEntityIds']").val(entityIds.join(","));
	},
	RemoveEntityMultiSelect: function (hyperlink) {
		var fieldWrapper = $(hyperlink).closest(".eav-field");
		$(hyperlink).closest(".MultiValueItem").remove();
		Eav.SyncSelectedEntities(fieldWrapper);
	},*/

    Gps: {
        _mapsApiInitDone: false,
        FindOnMap: function (latitudeStaticName, skipMap) {	// skipMap = true will only return LatLng of Address but doesn't act with the map
            if (!skipMap)
                Eav.Gps.ShowMap(latitudeStaticName);

            if (!Eav.Gps._mapsApiInitDone) {	// stop if api not init. Will be handled by callback
                Eav.Gps._mapsInitCallerFindAddress = true;
                return;
            }

            var lattitudeFieldController = $(".eav-field[data-staticname='" + latitudeStaticName + "']")[0].Controller.FieldController;
            var address;
            if (lattitudeFieldController.GpsMap.addressMask)
                address = lattitudeFieldController.GpsMap.addressMask.replace(/\[(.*?)\]/g, function (a, fieldName) { return $(".eav-field[data-staticname='" + fieldName + "'] .eav-field-control input").val(); });
            else
                address = prompt("Please enter an Address: (You can enter an Address-Mask to the GPS Field to use existing Address Field(s))", "");

            if (!address)
                return;

            (new google.maps.Geocoder()).geocode({ 'address': address }, function (results, status) {
                if (status == google.maps.GeocoderStatus.OK) {
                    if (skipMap != true) {
                        lattitudeFieldController.GpsMap.map.setCenter(results[0].geometry.location);
                        lattitudeFieldController.GpsMap.map.setZoom(17);
                        lattitudeFieldController.GpsMap.marker.setPosition(results[0].geometry.location);
                    }

                    lattitudeFieldController.SetValue(results[0].geometry.location.lat().toString().replace(".", Sys.CultureInfo.CurrentCulture.numberFormat.NumberDecimalSeparator));
                    lattitudeFieldController.GpsMap.longitudeController.FieldController.SetValue(results[0].geometry.location.lng().toString().replace(".", Sys.CultureInfo.CurrentCulture.numberFormat.NumberDecimalSeparator));

                    return results[0].geometry.location;
                } else
                    alert('Geocode was not successful for the following reason: ' + status);
            });
        },
        _initMapApiCallback: function () {
            Eav.Gps._mapsApiInitDone = true;
            var centerLatLng = null;

            if (Eav.Gps._mapsInitCallerFindAddress) {
                centerLatLng = Eav.Gps.FindOnMap(Eav.Gps._mapsInitCallerStaticName);
                Eav.Gps._mapsInitCallerFindAddress = null;
            }

            Eav.Gps._initMap(Eav.Gps._mapsInitCallerStaticName, centerLatLng);

            Eav.Gps._mapsInitCallerStaticName = null;
        },
        _initMap: function (latitudeStaticName, centerLatLng) {
            var lattitudeFieldController = $(".eav-field[data-staticname='" + latitudeStaticName + "']")[0].Controller.FieldController;
            if (lattitudeFieldController.GpsMap.map != undefined)	// skip if already initialized
                return;

            lattitudeFieldController.GpsMap.longitudeController = $(".eav-field[data-staticname='" + lattitudeFieldController.GpsMap.longitudeStaticName + "']")[0].Controller;

            // get lat/lng from Textboxes
            function getPositionFromTextboxes() {
                var latVal = lattitudeFieldController.GetValue().replace(Sys.CultureInfo.CurrentCulture.numberFormat.NumberDecimalSeparator, ".");
                var lngVal = lattitudeFieldController.GpsMap.longitudeController.FieldController.GetValue().replace(Sys.CultureInfo.CurrentCulture.numberFormat.NumberDecimalSeparator, ".");
                return latVal == "" || lngVal == "" ? null : new google.maps.LatLng(parseFloat(latVal), parseFloat(lngVal));
            }

            // init map
            var mapOptions = { zoom: centerLatLng ? 17 : 7, mapTypeId: google.maps.MapTypeId.ROADMAP, scrollwheel: false };
            mapOptions.center = centerLatLng || getPositionFromTextboxes() || new google.maps.LatLng(47.16003606930926, 9.477177858352661);
            var mapCanvas = lattitudeFieldController.GpsMap.canvas;
            mapCanvas.show();
            var map = new google.maps.Map(mapCanvas[0], mapOptions);

            // init marker
            var marker = new google.maps.Marker({
                map: map,
                position: mapOptions.center,
                draggable: true,
            });

            // handle marker move/drag
            google.maps.event.addListener(marker, 'drag', function () {
                lattitudeFieldController.SetValue(this.position.lat().toString().replace(".", Sys.CultureInfo.CurrentCulture.numberFormat.NumberDecimalSeparator));
                lattitudeFieldController.GpsMap.longitudeController.FieldController.SetValue(this.position.lng().toString().replace(".", Sys.CultureInfo.CurrentCulture.numberFormat.NumberDecimalSeparator));
            });

            // handle manual change of lat/lng in Textbox
            lattitudeFieldController.GpsMap.latitudeInput.add(lattitudeFieldController.GpsMap.longitudeInput).change(function () {
                var position = getPositionFromTextboxes();
                if (position != null) {
                    lattitudeFieldController.GpsMap.map.setCenter(position);
                    lattitudeFieldController.GpsMap.marker.setPosition(position);
                }
            });

            lattitudeFieldController.GpsMap.map = map;
            lattitudeFieldController.GpsMap.marker = marker;

            lattitudeFieldController.GpsMap.actionsWrapper.find(".eav-gps-map-showhide").val("Show/hide Map").prop("onclick", null).click(function () {
                mapCanvas.toggle();
            });
            $(".eav-gps-map-actions input[type='button']").prop("disabled", false);	// re-enable all buttons again
        },
        ShowMap: function (latitudeStaticName) {
            if (!Eav.Gps._mapsApiInitDone) {
                $(".eav-gps-map-actions input[type='button']").prop("disabled", true);	// disable all map-buttons to prevent multiple click while init
                Eav.Gps._mapsInitCallerStaticName = latitudeStaticName;
                $.getScript("http://maps.googleapis.com/maps/api/js?sensor=false&callback=Eav.Gps._initMapApiCallback");
            } else
                Eav.Gps._initMap(latitudeStaticName);
        }
    },
};

function pageLoad(sender, e) {
    Eav.InitializeForms();
}

// Object used to store field-specific controller functions
Eav.FieldControllerManager = {
    string: function (objWrapper) {
        var Controller = new Object();
        var FieldSubTypeAttr = objWrapper.attr("data-fieldsubtype");
        var FieldSubType = "";

        if (typeof FieldSubTypeAttr !== 'undefined' && FieldSubTypeAttr !== false)
            FieldSubType = FieldSubTypeAttr.toLowerCase();

        switch (FieldSubType) {
            case "dropdown":
                var field = objWrapper.find("select");
                Controller.SetReadOnly = function (readOnlyState) {
                    field.prop("disabled", readOnlyState);
                };
                Controller.SetValue = function (value) {
                    field.val(value);
                };
                Controller.GetValue = function () {
                    return field.val();
                };
                break;
            case ("wysiwyg"):
                if (Eav.FieldControllerManager.wysiwyg != undefined)
                    Eav.FieldControllerManager.wysiwyg(Controller, objWrapper);
                break;
            default:
                var field = objWrapper.find("input[type=text],textarea");
                Controller.SetReadOnly = function (readOnlyState) { field.prop("disabled", readOnlyState); };
                Controller.SetValue = function (value) { field.val(value); };
                Controller.GetValue = function () { return field.val(); };
                break;
        }

        return Controller;
    },

    boolean: function (objWrapper) {
        var Controller = new Object();

        var checkBox = objWrapper.find("input[type=checkbox]");
        Controller.SetReadOnly = function (readOnlyState) { checkBox.prop("disabled", readOnlyState); };
        Controller.SetValue = function (value) {
            var checked = value.toLower() == 'true';
            checkBox.prop("checked", checked);
            $(objWrapper).find(".dnnCheckbox").toggleClass("dnnCheckbox-checked", checked);
        };
        Controller.GetValue = function () {
            return checkBox.prop("checked");
        };
        return Controller;
    },

    datetime: function (objWrapper) {
        var Controller = new Object();

        var field = objWrapper.find("input[type=text]");
        var calPopup = objWrapper.find(".rcCalPopup");

        Controller.SetReadOnly = function (readOnlyState) {
            field.prop("disabled", readOnlyState);
            calPopup.toggle(!readOnlyState);
        };
        Controller.SetValue = function (value) { field.val(value); };
        Controller.GetValue = function () { return field.val(); };

        return Controller;
    },

    number: function (objWrapper) {
        var Controller = new Object();
        var FieldSubTypeAttr = objWrapper.attr("data-fieldsubtype");
        var FieldSubType = "";

        if (typeof FieldSubTypeAttr !== 'undefined' && FieldSubTypeAttr !== false)
            FieldSubType = FieldSubTypeAttr.toLowerCase();

        var field = objWrapper.find("input[type=text]");
        Controller.SetReadOnly = function (readOnlyState) { field.prop("disabled", readOnlyState); };
        Controller.SetValue = function (value) { field.val(value); };
        Controller.GetValue = function () { return field.val(); };

        switch (FieldSubType) {
            case "gps":
                var latitudeStaticName = objWrapper.attr("data-staticname");
                var longitude = objWrapper.next();
                if (longitude.attr("data-fieldtype") != "Number" || longitude.attr("data-fieldsubtype") != "")
                    alert("GPS Number Field \"" + latitudeStaticName + "\" isn't configured correctly.\nThe field below/next to Latitude must be Longitude. Both must be Number fields but only Latitude must be of Typ \"GPS\".");

                objWrapper.find(".eav-field-control").append('<div class="eav-gps-map-actions"><input class="eav-gps-map-showhide" type="button" value="Show Map" onclick="Eav.Gps.ShowMap(\'' + latitudeStaticName + '\')" /><input type="button" value="Find on Map" onclick="Eav.Gps.FindOnMap(\'' + latitudeStaticName + '\')"/></div>');
                objWrapper.before('<div id="eav-gps-map-' + latitudeStaticName + '" class="eav-gps-map-canvas" style="height: 400px; display: none"></div>');

                Controller.GpsMap = { canvas: objWrapper.prev(), longitudeStaticName: longitude.attr("data-staticname"), latitudeInput: field, longitudeInput: longitude.find("input[type=text]"), addressMask: objWrapper.attr("data-addressmask"), actionsWrapper: objWrapper.find(".eav-gps-map-actions") };
                Controller.GpsMap.latitudeInput.addClass("eav-gps-map-latitude");
                Controller.GpsMap.longitudeInput.addClass("eav-gps-map-longitude");
                break;
            default:
                break;
        }


        return Controller;
    },

    entity: function (objWrapper) {
        // init Entity Selection
        //if (objWrapper.attr("data-allowmultivalue") == "true")
        //	Eav.InitEntityMultiSelector(objWrapper);

        var Controller = new Object();
        var field = objWrapper.find("select");
        Controller.SetReadOnly = function (readOnlyState) { field.prop("disabled", readOnlyState); objWrapper.find("a").prop("disabled", readOnlyState); };
        Controller.SetValue = function (value) {
            /*if (objWrapper.attr("data-allowmultivalue") == "true") {
				alert("Multi-Value SetValue() is untested!");	// must set hidden field and add/init DropDowns for each value
				objWrapper.find("input[type=hidden][id$='_hfEntityIds']").val(value);
				Eav.InitEntityMultiSelector(objWrapper, true);
			} else
				field.val(value);*/
            alert("Set-value is untested!");
        };
        Controller.GetValue = function () {
            if (objWrapper.attr("data-allowmultivalue") == "true")
                return objWrapper.find("input[type=text][id$='_hfEntityIds']").val();
            else
                return field.val();
        };
        return Controller;
    }
};



/* Fix cross-browser ajax request support for IE 8 / 9. Source: Comments of http://bugs.jquery.com/ticket/8283 */
$.ajaxTransport(function (options, originalOptions, jqXHR) {
    var xdr;

    return {
        send: function (_, completeCallback) {
            xdr = new XDomainRequest();
            xdr.onload = function () {
                if (xdr.contentType.match(/\/json/)) {
                    options.dataTypes.push("json");
                }

                completeCallback(200, 'success', { text: xdr.responseText });
            };
            xdr.onerror = xdr.ontimeout = function () {
                completeCallback(400, 'failed', { text: xdr.responseText });
            }

            xdr.open(options.type, options.url);
            xdr.send(options.data);
        },
        abort: function () {
            if (xdr) {
                xdr.abort();
            }
        }
    };
});

// Contstruct angular module for EAV
(function () {
    angular.module('2sic-EAV', ['ui.tree']);
})();