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
	private bool moving;
	private Vector2 position;
	private Vector2 unitVelocity;
	private float speed;
	//public Vector2 Velocity{
	//	get{ return velocity; }
	//	set{ velocity = value; }
	//}
	private float distanceLeft;
	private Corners.CornerIndex nextCorner;
	private bool attack = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (moving){
			Vector2 nextPos = unitVelocity*speed + position;
			if (cn.distanceLeft(nextCorner, nextPos) > distanceLeft){ // then turn
				unitVelocity = position - cn.turn (nextCorner);
				unitVelocity.Normalize();
				nextPos = unitVelocity*speed + position;
			}
			position = nextPos;
			transform.localPosition = new Vector3(position.x, 0, position.y);
		}
	}

	public void initialize(ZombieBehavior.ZombieType t, Corners.CornerIndex ci, Vector2 pos){
		moving = true;
		type = t;
		speed = zb.getSpeed (t);
		nextCorner = ci;
		position = pos;
		unitVelocity = position - cn.getCorner (ci);
		unitVelocity.Normalize();
		transform.localPosition = new Vector3 (pos.x, 0, pos.y);
	}
}
