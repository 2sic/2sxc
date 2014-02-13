<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditDataSource.ascx.cs" Inherits="ToSic.SexyContent.EditDataSource" %>

<link rel="stylesheet" href="/DesktopModules/ToSIC_SexyContent/Styles/EditDataSource/demo-new.css">
<link rel="stylesheet" href="/DesktopModules/ToSIC_SexyContent/Styles/EditDataSource/flowchartDemo.css">

<div class="dnnForm scSettings dnnClear">
    <h2>Edit DataSource</h2>
    <div id="main">
        <div id="render"></div>
    </div>
    <ul class="dnnActions">
        <li><asp:HyperLink runat="server" ID="lnkBack" Text="Back" CssClass="dnnPrimaryAction"></asp:HyperLink></li>
    </ul>
</div>


<%--<script type="text/javascript" src="../../js/lib/jquery-1.8.1-min.js"></script>
<script type="text/javascript" src="../../js/lib/jquery-ui-1.8.23-min.js"></script>--%>
<script type="text/javascript" src="/DesktopModules/ToSIC_SexyContent/Js/EditDataSource/jquery.ui.touch-punch.min.js"></script>

<!-- JS -->
<!-- support lib for bezier stuff -->
<%--<script src="../../js/lib/jsBezier-0.6.js"></script>--%>
<!-- jsplumb util -->
<script src="/DesktopModules/ToSIC_SexyContent/Js/EditDataSource/jsPlumb/jsPlumb-util-1.4.0-RC1.js"></script>
<!-- base DOM adapter -->
<script src="/DesktopModules/ToSIC_SexyContent/Js/EditDataSource/jsPlumb/jsPlumb-dom-adapter-1.4.0-RC1.js"></script>
<!-- main jsplumb engine -->
<script src="/DesktopModules/ToSIC_SexyContent/Js/EditDataSource/jsPlumb/jsPlumb-1.4.0-RC1.js"></script>
<!-- anchors -->
<script src="/DesktopModules/ToSIC_SexyContent/Js/EditDataSource/jsPlumb/jsPlumb-anchors-1.4.0-RC1.js"></script>   
<!-- endpoint -->
<script src="/DesktopModules/ToSIC_SexyContent/Js/EditDataSource/jsPlumb/jsPlumb-endpoint-1.4.0-RC1.js"></script>      
<!-- connection -->
<script src="/DesktopModules/ToSIC_SexyContent/Js/EditDataSource/jsPlumb/jsPlumb-connection-1.4.0-RC1.js"></script>
<!-- connector editors  -->
<script src="/DesktopModules/ToSIC_SexyContent/Js/EditDataSource/jsPlumb/jsPlumb-connector-editors-1.4.0-RC1.js"></script>        
<!-- connectors, endpoint and overlays  -->
<script src="/DesktopModules/ToSIC_SexyContent/Js/EditDataSource/jsPlumb/jsPlumb-defaults-1.4.0-RC1.js"></script>
<!-- state machine connectors -->
<script src="/DesktopModules/ToSIC_SexyContent/Js/EditDataSource/jsPlumb/jsPlumb-connectors-statemachine-1.4.0-RC1.js"></script>
<!-- flowchart connectors -->
<script src="/DesktopModules/ToSIC_SexyContent/Js/EditDataSource/jsPlumb/jsPlumb-connectors-flowchart-1.4.0-RC1.js"></script>        
<!-- SVG renderer -->
<script src="/DesktopModules/ToSIC_SexyContent/Js/EditDataSource/jsPlumb/jsPlumb-renderers-svg-1.4.0-RC1.js"></script>
<!-- canvas renderer -->
<script src="/DesktopModules/ToSIC_SexyContent/Js/EditDataSource/jsPlumb/jsPlumb-renderers-canvas-1.4.0-RC1.js"></script>
<!-- vml renderer -->
<script src="/DesktopModules/ToSIC_SexyContent/Js/EditDataSource/jsPlumb/jsPlumb-renderers-vml-1.4.0-RC1.js"></script>
<!-- jquery jsPlumb adapter -->
<script src="/DesktopModules/ToSIC_SexyContent/Js/EditDataSource/jsPlumb/jquery.jsPlumb-1.4.0-RC1.js"></script>
<!-- /JS -->
		
<!--  demo code -->
<script src="/DesktopModules/ToSIC_SexyContent/Js/EditDataSource/flowchartConnectorsDemo.js"></script>
		
<!--  demo helper code -->
<script src="/DesktopModules/ToSIC_SexyContent/Js/EditDataSource/demo-list.js"></script>  
<script src="/DesktopModules/ToSIC_SexyContent/Js/EditDataSource/demo-helper-jquery.js"></script> 