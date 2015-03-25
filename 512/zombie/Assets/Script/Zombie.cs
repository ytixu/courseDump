using UnityEngine;
using System.Collections;

public class Zombie : MonoBehaviour {

	public ZombieBehavior zb;
	public Corners cn;
	public ZombieFactory zf;

	private ZombieBehavior.ZombieType type;
	public ZombieBehavior.ZombieType getType(){
		return type;
	}

	// states
	public bool moving = false;
	public Vector2 position;
	public Vector2 unitVelocity;
	public float speed;
	public float distanceLeft;
	public Corners.CornerIndex nextCorner;
	public bool attack = false;
	public int direction = 1;
	public Vector2 nextPos;
	public bool active = false;

	public GameObject seen;

	// Use this for initialization
	void Start () {

	}
	
	void Update () {
		if (!active) return;
		if (!zb.behave(this)){
			if (moving) nextPos = unitVelocity*speed + position;
		}
		if (moving){
			float newDistance = cn.distanceLeft(nextCorner, nextPos, direction==1);
			//print (newDistance.ToString() + " " + distanceLeft.ToString() + " " 
			//      + nextCorner.j + " " + nextPos.ToString() + " " + this.name);
			if (newDistance > distanceLeft && distanceLeft < 1){ // then turn
				//print ("--- " + name + " " + nextCorner.j + " " + direction);
				if (direction < 1) unitVelocity = cn.turnReverse (nextCorner) - position;
				else unitVelocity = cn.turn (nextCorner) - position;
				//print ( nextCorner.j + " " + direction);
				unitVelocity.Normalize();
				nextPos = unitVelocity*speed + position;
				newDistance = cn.distanceLeft(nextCorner, nextPos, direction==1);
				transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y+90*direction, 0);
			}
			distanceLeft = newDistance;
			if (!zombieAhead(nextPos, 0.5f)){
				position = nextPos;
				transform.localPosition = new Vector3(position.x, 0, position.y);
			}
		}
	}

	public void changeNextPos(Vector2 newP){
		nextPos = newP;
		distanceLeft = cn.distanceLeft (nextCorner, position, direction==1);
	}

	public void changeDirection(){
		direction *= -1;
		if (direction < 1) unitVelocity = cn.turnReverse (nextCorner) - position;
		else unitVelocity = cn.turn (nextCorner) - position;
		unitVelocity.Normalize();
		distanceLeft = cn.distanceLeft(nextCorner, position, direction==1);
		transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y+180, 0);
	}

	public bool zombieAhead(float d){
		return collisionCheck (transform.position, d, transform.forward);
	}
	
	public bool zombieAhead(Vector2 pos, float d){
		return collisionCheck (zf.transform.TransformPoint(new Vector3(pos.x, 0, pos.y)), d, transform.forward);
	}

	public bool zombieNearBy(Vector2 direction){
		return collisionCheck (transform.position, 1, new Vector3(direction.x, 0, direction.y));
	}

	public bool collisionCheck(Vector3 pos, float d, Vector3 direction){
		//Vector3 fwd = transform.TransformDirection(Vector3.forward);
		RaycastHit hit;
		if (Physics.Raycast (pos, direction, out hit, d)){
			//print (name + " " + hit.collider.name);
			if (hit.collider.tag == "Zombie") return true;
		}
		return false;
	}
	
	public void initialize(ZombieBehavior.ZombieType t, Corners.CornerIndex ci, Vector2 pos){
		transform.Find("character").renderer.material = zb.getColor (t);
		direction = 1;
		active = true;
		moving = true;
		attack = false;
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

	/**
	 * called by survivor
	 * estimate whether the survivor will be in zombie's FOV on the next step
	 */
	public bool inFOV(Vector3 pos){
		int shortDist = 2;
		if (distanceLeft < 1){
			shortDist = 5;
		}
		Vector3 center = transform.position + (3 + speed) * transform.forward;
		Vector3 FOV_long = 5 * transform.forward;
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
}
