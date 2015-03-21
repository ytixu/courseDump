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
		return zombieAhead (transform.position, d);
	}


	public bool zombieAhead(Vector2 pos, float d){
		return zombieAhead (zf.transform.TransformPoint(new Vector3(pos.x, 0, pos.y)), d);
	}

	public bool zombieAhead(Vector3 pos, float d){
		//Vector3 fwd = transform.TransformDirection(Vector3.forward);
		RaycastHit hit;
		if (Physics.Raycast (pos, transform.forward, out hit, d)){
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
}
