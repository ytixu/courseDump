using UnityEngine;
using System.Collections;

public class Zombie : MonoBehaviour {

	public ZombieBehavior zb;
	public Corners cn;

	private ZombieBehavior.ZombieType type;
	public ZombieBehavior.ZombieType getType(){
		return type;
	}

	// states
	private bool moving = false;
	public Vector2 position;
	public Vector2 unitVelocity;
	public float speed;
	//public Vector2 Velocity{
	//	get{ return velocity; }
	//	set{ velocity = value; }
	//}
	public float distanceLeft;
	public Corners.CornerIndex nextCorner;
	private bool attack = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (moving){
			Vector2 nextPos = unitVelocity*speed + position;
			float newDistance = cn.distanceLeft(nextCorner, nextPos);
			if (newDistance > distanceLeft){ // then turn
				unitVelocity = cn.turn (nextCorner) - position;
				unitVelocity.Normalize();
				nextPos = unitVelocity*speed + position;
				newDistance = cn.distanceLeft(nextCorner, nextPos);
			}
			position = nextPos;
			distanceLeft = newDistance;
			transform.localPosition = new Vector3(position.x, 0, position.y);
		}
	}

	public void initialize(ZombieBehavior.ZombieType t, Corners.CornerIndex ci, Vector2 pos){
		moving = true;
		type = t;
		speed = zb.getSpeed (t);
		nextCorner = new Corners.CornerIndex(ci.i, ci.j);
		position = pos;
		distanceLeft = cn.distanceLeft (nextCorner, position);
		unitVelocity = cn.getCorner (ci)-position;
		unitVelocity.Normalize();
		transform.localPosition = new Vector3 (pos.x, 0, pos.y);
		transform.Find("character").renderer.material = zb.getColor (t);
	}
}
