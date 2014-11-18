function rnd_snd() {
	// from http://www.protonfish.com/random.shtml
	return (Math.random()*2-1)+(Math.random()*2-1)+(Math.random()*2-1);
}

function sign(x) { 
	// from http://stackoverflow.com/questions/7624920/number-sign-in-javascript
	return x ? x < 0 ? -1 : 1 : 0; 
}

var CAR_SPEED = 5;
var MIN_DIST = 10;

function createCarTarget(start){
	return {
		x: Math.ceil(rnd_snd()*10) + start.x,
		y: Math.ceil(rnd_snd()*10) + start.y,
		currentView: start,
		nextView: null,
		startTime: 0.0,

		setNext: function(next){
			nextView = next;
		},

		getNextView: function(){
			var temp = nextView;
			try{
				if (Math.random() <= currentView.p)
					nextView = nextView.child[0]
				else{
					nextView = nextView.child[1]
				}
			}catch(e){
				nextView = null;
			}
			if (temp) currentView = temp;
		},

		updatePos: function(){
			// if nextView is not set yet
			if (! this.nextView) return false;

			// get distance from current position to goal position
			var dx = nextView.x - this.x;
			var dy = nextView.y - this.y;

			// heuristic to randomize movement
			if (Math.abs(dx) > MIN_DIST){
				dx = CAR_SPEED*sign(dx) + Math.ceil(rnd_snd()*CAR_SPEED);
			}else{
				if (Math.abs(dx) > CAR_SPEED){
					dx = CAR_SPEED*sign(dx) + Math.ceil(rnd_snd()*CAR_SPEED/2);
				}else{
					dx = dx + Math.ceil(rnd_snd()*dx);
				}
			}
			if (Math.abs(dy) > MIN_DIST){
				dy = CAR_SPEED*sign(dy) + Math.ceil(rnd_snd()*CAR_SPEED);
			}else{
				if (Math.abs(dy) > CAR_SPEED){
					dy = CAR_SPEED*sign(dy) + Math.ceil(rnd_snd()*CAR_SPEED/2);
				}else{
					dy = dy + Math.ceil(rnd_snd()*dy);
				}
			}
			this.x += dx;
			this.y += dy;

			// if arrive at goal position, get a new goal
			if (Math.abs(nextView.x - this.x) < MIN_DIST && Math.abs(nextView.y - this.y) < MIN_DIST){
				this.getNextView();
			}
			return true;
		},

		setTime: function(time){
			this.startTime = time;
		},

		updateTime: function(endTime){
			return endTime - this.startTime;
		}
	}
}
