
function Paragraph(props) {
    return 'ante';
}

function myFunction() {
    // ReactDOM.render(<Paragraph />,
    //     document.getElementById("rootante"));


    // var result = document.getElementById("alo");


    // console.log(result);
    fireEvent(document.getElementById("demo"), "change");
    document.getElementById("demo").value = 'anteeee'
    // console.log('myFunction');
}

// activities.addEventListener("change", function() {
//     if(activities.value == "addNew")
//     {
//         addActivityItem();
//     }
//     console.log(activities.value);
// });

function myFunction1() {
    // var x = document.getElementById("frm1");
    // var text = "";
    // var i;
    // for (i = 0; i < x.length; i++) {
    //     text += x.elements[i].value + "<br>";
    // }
    document.getElementById("demo").value = 'mateeee';
    console.log(document.getElementById("demo"));
    console.log('myFunction bbbbbbbbbbbbbbbbbbbb')
}

function fireEvent(node, eventName) {
    // Make sure we use the ownerDocument from the provided node to avoid cross-window problems
    var doc;
    if (node.ownerDocument) {
        doc = node.ownerDocument;
    } else if (node.nodeType == 9) {
        // the node may be the document itself, nodeType 9 = DOCUMENT_NODE
        doc = node;
    } else {
        throw new Error("Invalid node passed to fireEvent: " + node.id);
    }

    if (node.dispatchEvent) {
        // Gecko-style approach (now the standard) takes more work
        var eventClass = "";

        // Different events have different event classes.
        // If this switch statement can't map an eventName to an eventClass,
        // the event firing is going to fail.
        switch (eventName) {
            case "click": // Dispatching of 'click' appears to not work correctly in Safari. Use 'mousedown' or 'mouseup' instead.
            case "mousedown":
            case "mouseup":
                eventClass = "MouseEvents";
                break;

            case "focus":
            case "change":
            case "blur":
            case "select":
                eventClass = "HTMLEvents";
                break;

            default:
                throw "fireEvent: Couldn't find an event class for event '" + eventName + "'.";
                break;
        }
        var event = doc.createEvent(eventClass);

        var bubbles = eventName == "change" ? false : true;
        event.initEvent(eventName, bubbles, true); // All events created as bubbling and cancelable.

        event.synthetic = true; // allow detection of synthetic events
        // The second parameter says go ahead with the default action
        node.dispatchEvent(event, true);
    } else if (node.fireEvent) {
        // IE-old school style
        var event = doc.createEventObject();
        event.synthetic = true; // allow detection of synthetic events
        node.fireEvent("on" + eventName, event);
    }
};

// function DivBox(col, row) {
//     console.log('bbbbbbbbbbbbbbbbbbbbbbbbbbbbb')
//     var ret = "";
//     for (var r = 0; r < row; r++) {
//         ret += '<div id="Column' + (r + 1) + '" style="float:left">';
//         for (var c = 0; c < col; c++) {
//             ret += '<div id="sq' + (r * col + c + 1) + '" style="width:40px; height:40px;">';
//             ret += (r * col + c + 1); //just for showing
//             ret += '</div>';

//         }
//         ret += '</div>';
//     }
//     return ret;
// }



