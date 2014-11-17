function createTarget(xPos, yPos, walkingBehavior){
	return {
		x: xPos,
		y: yPos,
		currentView: null,
		startTime: 0.0,
		updatePos: walkingBehavior,
		setTime: function(time){
			this.startTime = time;
		},
		updateTime: function(endTime){
			return endTime - this.startTime;
		}
	}
}

// pedestrian

// function pedestWalk(
// car