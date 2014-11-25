// camera object

// half of the sizes
var XCAM_SIZE = 60;
var YCAM_SIZE = 40;

var cameras = [];
var clickCamera = true;
var MAX_CAM = 6;

function createCamera(xPos, yPos){
	return {
		// center
		x: xPos,
		y: yPos,
		// check if a point is in its viewfield
		inView : function(x,y){
			if (this.x + XCAM_SIZE < x ||
				this.x - XCAM_SIZE > x ||
				this.y + YCAM_SIZE < y ||
				this.y - YCAM_SIZE > y) return false;
			return true;
		},
		draw : function(context){
			context.beginPath();
			context.rect(Math.max(0,this.x-XCAM_SIZE), Math.max(0,this.y-YCAM_SIZE),
					Math.min(XMAP_SIZE-this.x+XCAM_SIZE, XCAM_SIZE*2), Math.min(YMAP_SIZE-this.y+YCAM_SIZE, YCAM_SIZE*2));
			context.stroke();
		}
	}
}