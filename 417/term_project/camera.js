// camera object

// half of the sizes
var XCAM_SIZE = 25;
var YCAM_SIZE = 15;

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
		}
	}
}