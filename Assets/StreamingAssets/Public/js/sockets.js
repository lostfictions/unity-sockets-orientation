'use strict'

// var url = 'ws://' + document.location.host + '/Echo';
var url = 'ws://' + document.location.host + '/Orientation';

var output;
var websocket;

function init() {
  output = document.getElementById("output");
  doWebSocket();
}

function doWebSocket() {
  websocket = new WebSocket(url);

  websocket.onopen = function(e) {
    onOpen(e);
  };

  websocket.onmessage = function(e) {
    onMessage(e);
  };

  websocket.onerror = function(e) {
    onError(e);
  };

  websocket.onclose = function(e) {
    onClose(e);
  };
}

function onOpen(event) {
  writeToScreen("CONNECTED");
}

function onMessage(event) {
  writeToScreen('<span style="color: blue;">RESPONSE: ' + event.data + '</span>');
  websocket.close();
}

function onError(event) {
  writeToScreen('<span style="color: red;">ERROR: ' + event.data + '</span>');
}

function onClose(event) {
  writeToScreen("DISCONNECTED");
}

function send(message) {
  writeToScreen("SENT: " + message);
  websocket.send(message);
}

function writeToScreen(message) {
  var pre = document.createElement("p");
  pre.style.wordWrap = "break-word";
  pre.innerHTML = message;
  output.appendChild(pre);
}

window.addEventListener("load", init, false);


var gn = new GyroNorm();

gn.init().then(function(){
    gn.start(function(data){
      websocket.send(data.do.alpha)
        // Process:
        // data.do.alpha    ( deviceorientation event alpha value )
        // data.do.beta     ( deviceorientation event beta value )
        // data.do.gamma    ( deviceorientation event gamma value )
        // data.do.absolute ( deviceorientation event absolute value )

        // data.dm.x        ( devicemotion event acceleration x value )
        // data.dm.y        ( devicemotion event acceleration y value )
        // data.dm.z        ( devicemotion event acceleration z value )

        // data.dm.gx       ( devicemotion event accelerationIncludingGravity x value )
        // data.dm.gy       ( devicemotion event accelerationIncludingGravity y value )
        // data.dm.gz       ( devicemotion event accelerationIncludingGravity z value )

        // data.dm.alpha    ( devicemotion event rotationRate alpha value )
        // data.dm.beta     ( devicemotion event rotationRate beta value )
        // data.dm.gamma    ( devicemotion event rotationRate gamma value )
    });
}).catch(function(e) {
  // Catch if the DeviceOrientation or DeviceMotion is not supported by the browser or device
});
