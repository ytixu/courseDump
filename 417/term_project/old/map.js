var ROADSIZE = 30;
var sherbrooke = YMAP_SIZE/3;
var mcgill = XMAP_SIZE/4;
var victoria = mcgill*3;

var CARDROADSIZE = ROADSIZE/2;

var CAR_MAP = {
	i1: {
		x: 0,
		y: sherbrooke + CARDROADSIZE,
		p: 1.0,
		child: [this.i2]
	},
	i2: {
		x: mcgill - CARDROADSIZE,
		y: sherbrooke + CARDROADSIZE,
		p: 0.5,
		child: [this.i3, this.i4]
	},
	i3: {
		x: mcgill - CARDROADSIZE,
		y: YMAP_SIZE,
		child: []
	},
	i4: {
		x: victoria - CARDROADSIZE,
		y: sherbrooke + CARDROADSIZE,
		child: [this.i5, this.i6]
	},
	i5: {
		x: victoria - CARDROADSIZE,
		y: YMAP_SIZE,
		child: []
	},
	i6: {
		x: XMAP_SIZE,
		y: sherbrooke + CARDROADSIZE,
		child: []
	},
	i7: {
		x: XMAP_SIZE,
		y: sherbrooke - CARDROADSIZE,
		p: 1.0,
		child: [this.i8]
	},
	i8: {
		x: victoria - CARDROADSIZE,
		y: sherbrooke - CARDROADSIZE,
		p: 0.5,
		child: [this.i9, this.i10]
	},
	i9: {
		x: victoria - CARDROADSIZE,
		y: YMAP_SIZE,
		child: []
	},
	i10: {
		x: mcgill - CARDROADSIZE,
		y: sherbrooke - CARDROADSIZE,
		p: 0.5,
		child: [this.i11, this.i12]
	},
	i11: {
		x: mcgill - CARDROADSIZE,
		y: YMAP_SIZE,
		child: []
	},
	i12: {
		x: 0,
		y: sherbrooke - CARDROADSIZE,
		child: []
	},
	i13: {
		x: mcgill + CARDROADSIZE,
		y: YMAP_SIZE,
		p: 0.5,
		child: [this.i14, this.i15]
	},
	i14: {
		x: mcgill + CARDROADSIZE,
		y: sherbrooke + CARDROADSIZE,
		p: 1.0,
		child: [this.i4]
	},
	i15: {
		x: mcgill + CARDROADSIZE,
		y: sherbrooke - CARDROADSIZE,
		p: 1.0,
		child: [this.i12]
	},
	i16: {
		x: victoria + CARDROADSIZE,
		y: YMAP_SIZE,
		p: 0.5,
		child: [this.i17, this.i18]
	},
	i17: {
		x: victoria + CARDROADSIZE,
		y: sherbrooke + CARDROADSIZE,
		p: 1.0,
		child: [this.i6]
	},
	i18: {
		x: victoria + CARDROADSIZE,
		y: sherbrooke - CARDROADSIZE,
		p: 1.0,
		child: [this.i10]
	}
};
// var PED_MAP = {

// };

var CAR_START = [
	CAR_MAP.i1, 
	CAR_MAP.i7,
	CAR_MAP.i13,
	CAR_MAP.i16,
]

var CARCOL = "#ffaa99";
// var PEDCOL = "#99aaff";

function init_canvas(canvas){
	canvas.width = XMAP_SIZE;
	canvas.height = YMAP_SIZE;
	// canvas.style.width = XMAP_SIZE;
	// canvas.style.height = YMAP_SIZE;
}

function draw_line(context, x0, y0, x1, y1){
	context.beginPath();
	context.moveTo(x0, y0);
	context.lineTo(x1, y1);
	context.stroke();
}

function draw_disc(context, x, y, r){
	context.beginPath();
	context.arc(x,y,r,0,2*Math.PI);
	context.fill();
}

function draw_roads(context){
	context.lineWidth = 2;
	context.strokeStyle = '#000000';
	draw_line(context, 0, sherbrooke-ROADSIZE, XMAP_SIZE, sherbrooke-ROADSIZE);
	draw_line(context, 0, sherbrooke+ROADSIZE, mcgill-ROADSIZE, sherbrooke+ROADSIZE);
	draw_line(context, mcgill+ROADSIZE, sherbrooke+ROADSIZE, victoria-ROADSIZE, sherbrooke+ROADSIZE);
	draw_line(context, victoria+ROADSIZE, sherbrooke+ROADSIZE, XMAP_SIZE, sherbrooke+ROADSIZE);
	draw_line(context, mcgill-ROADSIZE, sherbrooke+ROADSIZE, mcgill-ROADSIZE, YMAP_SIZE);
	draw_line(context, mcgill+ROADSIZE, sherbrooke+ROADSIZE, mcgill+ROADSIZE, YMAP_SIZE);
	draw_line(context,  victoria-ROADSIZE, sherbrooke+ROADSIZE, victoria-ROADSIZE, YMAP_SIZE);
	draw_line(context,  victoria+ROADSIZE, sherbrooke+ROADSIZE, victoria+ROADSIZE, YMAP_SIZE);
}

function draw_turns(context){
	context.fillStyle = '#999999';
	for (var i in CAR_MAP){
		draw_disc(context, CAR_MAP[i].x, CAR_MAP[i].y, 3);
	}
}

function simulte(max_n){
	if (max_n == 0) return;
	// create a target 
	var target = createCarTarget(CAR_START[Math.ceil(Math.random()*4)]);

}

function readyCam(context){
	context.lineWidth = 3;
	context.strokeStyle = '#999999';
}

window.onload = function(){
	var canvas = document.getElementById("map");
	var context = canvas.getContext('2d');
	init_canvas(canvas);
	draw_roads(context);
	readyCam(context);
	canvas.addEventListener('click',function(evt) {
        if (clickCamera && cameras.length < MAX_CAM){
			var rect = canvas.getBoundingClientRect();
			var cam = createCamera(evt.clientX - rect.left, evt.clientY - rect.top);
			cameras.push(cam);
			cam.draw(context);
		}
    }, false);
    document.getElementById("resetBTN").onclick = function(){
    	context.save();
		context.setTransform(1, 0, 0, 1, 0, 0);
		context.clearRect(0, 0, canvas.width, canvas.height);
		context.restore();
		draw_roads(context);
		readyCam(context);
		cameras=[];
    }
	// draw_turns(context);
}