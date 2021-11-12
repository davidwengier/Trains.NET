

function MessageOuterJS(message)
{
    console.log("OuterJS received: " + message);
}

function MessageInnerJS(message) {
    console.log("OuterJS received a message for inner JS but doesn't know how to pass it on: " + message);
    var wasmFrame = document.getElementById('wasmFrame');
    wasmFrame.contentWindow.WASMMessageInnerJS(message);
}