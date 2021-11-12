

function WASMMessageInnerJS(message) {
    console.log("InnerJS received a message, can it be passed to .NetWasm?: " + message);
    DotNet.invokeMethodAsync('BlazingTrains', 'MessageWASM', message);
}