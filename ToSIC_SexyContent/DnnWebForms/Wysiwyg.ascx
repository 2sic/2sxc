<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Wysiwyg.ascx.cs" Inherits="ToSic.SexyContent.EAV.FormlyEditUI.FieldTemplates.WebForms.Wysiwyg" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>
<%@ Register TagPrefix="dnn" TagName="texteditor" Src="~/controls/texteditor.ascx" %>

<dnn:texteditor Height="400px" Width="100%" ID="Texteditor1" runat="server" EnableViewState="true" HtmlEncode="false" EnableResize="false" />

<script type="text/javascript">

	// Call this function from outside to register the actual bridge
	window.connectBridge = function(bridge) {
		window.bridge = bridge;
		$(document).ready(function() {
		    initWysiwyg();
		});
	};

	function initWysiwyg() {

		var controller = {};

		bridge.setValue = function (v) {
			if(controller.getValue() != v)
				controller.setValue(v);
		};
		bridge.setReadOnly = function(v) {
			controller.setReadOnly(v);
		};
		controller.onChanged = function (v) {
			bridge.onChanged(v);
		};

		// Check if CKEDITOR is used
		if (window.CKEDITOR) {
			var instanceId = $(".editor").get(0).id;

			controller.setReadOnly = function (readOnlyState) {

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

			var preventChangeEvent = false;
			controller.setValue = function (value) {
			    preventChangeEvent = true;
				// If the instance is not yet ready, set textarea, else via CKEditor API
				if (!CKEDITOR.instances[instanceId] || !CKEDITOR.instances[instanceId].instanceReady)
					$("textarea.editor").val(value);
				else {
					var editor = CKEDITOR.instances[instanceId];
					editor.setData(value);
					// After setting the data, set readOnlyState again - else the background color will be reset
					controller.setReadOnly(editor.readOnly);
				}
				preventChangeEvent = false;
			};
			controller.getValue = function () {
				// If instance is not yet ready, get HTML out of the textarea, else via CKEditor API
				if (!CKEDITOR.instances[instanceId] || !CKEDITOR.instances[instanceId].instanceReady)
					return $("textarea.editor").val();
				var editor = CKEDITOR.instances[instanceId]; //ev.editor;
				return editor.getData();
			};

			CKEDITOR.instances[instanceId].on('change', function () {
			    if (!preventChangeEvent)
			        controller.onChanged(controller.getValue());
			});
			CKEDITOR.instances[instanceId].on('instanceReady', function () { $(document).trigger('triggerbridgeresize'); });
		}
		else {
			// Use default Telerik RadEditor
			var editor = $find($(".RadEditor").get(0).id);
			editor.set_useClassicDialogs(true);

			controller.setReadOnly = function (readOnlyState) {
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
			controller.setValue = function(v) { editor.set_html(v); };
			controller.getValue = function() { return editor.get_html(); };

			var updateValue = function () {
				var value = controller.getValue();
				if (editor.get_editable())
					controller.onChanged(value);
			}

			editor.attachEventHandler('onselectionchange', updateValue);
			editor.attachEventHandler('onmousedown', updateValue);
			editor.attachEventHandler('onkeyup', updateValue);
			editor.get_textArea().addEventListener("keyup", updateValue, false);
			$(document).on('keyup mouseup', updateValue);
		}

		bridge.setValue(bridge.initialValue);
		bridge.setReadOnly(bridge.initialReadOnly);
	}
	
</script>

<style type="text/css">
	/* Disable resizing of width */
	.RadEditor { width: 100%!important; }
</style>