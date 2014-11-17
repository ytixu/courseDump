var XMAP_SIZE = 1000;
var YMAP_SIZE = 500;

function init_canvas(){
	var canvas = document.getElementById("map");
	canvas.width = XMAP_SIZE;
	canvas.height = YMAP_SIZE;
	// canvas.style.width = XMAP_SIZE;
	// canvas.style.height = YMAP_SIZE;
	// console.log("done");
}


window.onload = function(){
	init_canvas();
}