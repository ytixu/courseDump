using UnityEngine;
using System.Collections;

/**
 * Generic class for all type of zombies
 */ 

public class Zombie : MonoBehaviour {

	public ZombieBehavior zb;
	public Corners cn;
	public ZombieFactory zf;
	public SurvivorFactory sf;

	// this specifies the zombie's type
	private ZombieBehavior.ZombieType type;
	public ZombieBehavior.ZombieType getType(){
		return type;
	}

	public bool active = false;

	// states used for the FSM
	public bool moving = false; // for zombies that can stop (to check their phone)
	public Vector2 position; // current position, 2D version of transform.position
	public Vector2 unitVelocity; // normalized velocity vector
	public float speed; // magnetude of the velocity
	public float distanceLeft; // distance between current position and targeting corner of the track
	public Corners.CornerIndex nextCorner; // targeting corner
	public int direction = 1; // specifies whether zombie is moving clockwise or counterclockwise
								// default is clockwise
	public Vector2 nextPos; // the next position on the track
	public GameObject seen; // if seen by the survivor
	
	void Update () {
		if (!active) return; // this is just to prevent the zombie that serve as a template to instentiate zombies on the track to not move
		// FMS
		// check if there's a survivor in sight
		if (sf.currentSurvivor && inCurrentFOV(sf.currentSurvivor.transform.position)){
			sf.destroySurvivor();
		}
		// call the 1st sub FMS
		if (!zb.behave(this)){
			// if nextPos not updated, compute the next position of the zombie 
			// according to current velocity and position
			if (moving) nextPos = unitVelocity*speed + position;
		}
		// 2nd sub FMS
		if (moving){ 
			// check if we have arrived to a corner
			float newDistance = cn.distanceLeft(nextCorner, nextPos, direction==1);
			if (newDistance > distanceLeft && distanceLeft < 1){ // then turn
				if (direction < 1) unitVelocity = cn.turnReverse (nextCorner) - position;
				else unitVelocity = cn.turn (nextCorner) - position;
				unitVelocity.Normalize();
				nextPos = unitVelocity*speed + position;
				newDistance = cn.distanceLeft(nextCorner, nextPos, direction==1);
				transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y+90*direction, 0);
			}
			// if no collision with other zombies, update position
			if (!zombieAhead(nextPos, 0.5f)){
				distanceLeft = newDistance;
				position = nextPos;
				transform.localPosition = new Vector3(position.x, 0, position.y);
			}
		}
	}

	// helper function to assign the nextPos 
	// called by zombieBehavior
	public void changeNextPos(Vector2 newP){
		nextPos = newP;
		distanceLeft = cn.distanceLeft (nextCorner, position, direction==1);
	}

	// helper function to reverse direction
	// called by zombieBehavior
	public void changeDirection(){
		direction *= -1;
		if (direction < 1) unitVelocity = cn.turnReverse (nextCorner) - position;
		else unitVelocity = cn.turn (nextCorner) - position;
		unitVelocity.Normalize();
		distanceLeft = cn.distanceLeft(nextCorner, position, direction==1);
		transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y+180, 0);
	}

	// check for collision in front
	public bool zombieAhead(float d){
		return collisionCheck (transform.position, d, transform.forward);
	}
	// overload
	public bool zombieAhead(Vector2 pos, float d){
		return collisionCheck (zf.transform.TransformPoint(new Vector3(pos.x, 0, pos.y)), d, transform.forward);
	}

	// check for collision on the side 
	// used for when changing lane
	public bool zombieNearBy(Vector2 direction){
		return collisionCheck (transform.position, 1, new Vector3(direction.x, 0, direction.y));
	}

	// for all collision, we use ray casting
	private bool collisionCheck(Vector3 pos, float d, Vector3 direction){
		RaycastHit hit;
		if (Physics.Raycast (pos, direction, out hit, d)){
			if (hit.collider.tag == "Zombie") return true;
		}
		return false;
	}

	// initialize the type and states of the zombie 
	public void initialize(ZombieBehavior.ZombieType t, Corners.CornerIndex ci, Vector2 pos){
		transform.Find("character").renderer.material = zb.getColor (t);
		direction = 1;
		active = true;
		moving = true;
		type = t;
		speed = zb.getSpeed (t);
		nextCorner = new Corners.CornerIndex(ci.i, ci.j);
		position = pos;
		nextPos = Vector2.zero;
		distanceLeft = cn.distanceLeft (nextCorner, position, direction==1);
		unitVelocity = cn.getCorner (ci)-position;
		unitVelocity.Normalize();
		transform.localPosition = new Vector3 (pos.x, 0, pos.y);
	}

	// to check if a point is in a zombie's FOV, we check if the point is in 
	// a box defined by the zombies' FOV + the size of the object represented by the point + 
	// some constant (for detecting ahead of time)
	private bool evalFOV(Vector3 pos, Vector3 center, float shortDist, float longDist){
		Vector3 FOV_long = longDist * transform.forward;
		Vector3 FOV_short = shortDist*Vector3.Cross (Vector3.up, transform.forward);
		Vector3 a = center + FOV_long - FOV_short;
		Vector3 b = center - FOV_long - FOV_short;
		Vector3 c = center + FOV_long + FOV_short;
		Vector3 d = center - FOV_long + FOV_short;
		if (pos.x < Mathf.Max(a.x, b.x, c.x, d.x) && pos.x > Mathf.Min(a.x,b.x,c.x,d.x) &&
		    pos.z < Mathf.Max(a.z, b.z,c.z,d.z) && pos.z > Mathf.Min (a.z,b.z,c.z,d.z)){
			return true;
		}
		return false;
	}

	//To check if a survivor is in its field of view.
	public bool inCurrentFOV(Vector3 pos){
		// shift center to the center of the FOV
		Vector3 center = transform.position + 3 * transform.forward;
		return evalFOV (pos, center, 1.5f, 4.5f);
	}

	/**
	 * called by survivor
	 * estimate whether the survivor will be in zombie's FOV on the next step
	 */
	public bool inFOV(Vector3 pos, float s, float l, float c){
		float shortDist = s;
		float longDist = l;
		// this is for corners, because zombies's FOV can instantly rotate when turning at a corner, 
		// we want the survivor to stay as far as possible. 
		if (distanceLeft < 3){
			shortDist = c;
			longDist = c;
		}
		Vector3 center = transform.position + (3 + speed) * transform.forward;
		return evalFOV (pos, center, shortDist, longDist);
	}
}
