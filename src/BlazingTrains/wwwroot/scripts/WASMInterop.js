

function WASMMessageInnerJS(message) {
    console.log("InnerJS received a message, can it be passed to .NetWasm?: " + message);
    DotNet.invokeMethodAsync('BlazingTrains', 'MessageWASM', message);
}

function WASMUpdateProperty(propertyName, value) {
    console.log("inner update property: " + propertyName + value);
    DotNet.invokeMethodAsync('BlazingTrains', 'UpdateProperty', propertyName, value);

}