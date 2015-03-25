using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Survivor : MonoBehaviour {

	private Dictionary<Vector3, bool> collectibles;
	// a position is false if survivor has already passed there

	// states 
	private Vector3 target;
	private int picked; // this is to keep track of the number of places the survivor needs to go
						// =0 if it has successfully arrive to the goal with all the collectible object picked
	private float speed;
	private bool active = false;
	

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void initialize(GameObject collects, float s){
		picked = 1;
		collectibles = new Dictionary<Vector3, bool> ();
		foreach (Transform c in collects.transform)
		{
			collectibles.Add(c.transform.position, false);
			picked += 1;
		}
		target = closestCollectible ();
		speed = s;
		active = true;
		/////
		behave ();
	}

	/**
	 * BEHAVIOR TREE
	 * get closest collectible object (target)
	 * move closer to there if there is no wall, or zombie 
	 * otherwise move away from zombie
	 * 
	 * use potential field to move towards target 
	 * get all the collectible objects before targetting the goal 
	 */

	private bool collisionCheck(Vector3 pos, float d, Vector3 direction, string obj){
		RaycastHit hit;
		if (Physics.Raycast (pos, direction, out hit, d)){
			//print (name + " " + hit.collider.name);
			if (hit.collider.tag == obj) return true;
		}
		return false;
	}

	// check if hitting wall
	private bool hitWall(Vector3 newPos){
		Vector3 direction = newPos - transform.position;
		if (Mathf.Abs(direction.x) > 0){
			if (collisionCheck(newPos, 0.4f, WallDimention.X_DIR*Mathf.Sign(direction.x),"Wall"))
				return true;
		}else if (Mathf.Abs(direction.z) > 0){
			if (collisionCheck(newPos,0.4f, WallDimention.Z_DIR*Mathf.Sign(direction.z), "Wall"))
				return true;
		}
		return false;
	}

	private Vector3 closestCollectible(){
		float minDist = 1000;
		Vector3 optTarget = WallDimention.GOAL; // if all collectibles are picked, 
												// then just return the goal state
		foreach (Vector3 v in collectibles.Keys){
			if (collectibles[v]) continue;
			float dist = Vector3.Distance(v, transform.position);
			if (dist < 2) return v;
			if (dist < minDist){
				minDist = dist;
				optTarget = v;
			}
		}
		return optTarget;
	}

	// check if we have arrive to the target position
	private bool arriveToTarget(){
		if (Vector3.Distance(target, transform.position) < 0.2f){
			picked -= 1;
			return true;
		}
		return false;
	}

	// get the next 4 positions (up, down, left, right) and order them by the value of their potential
	private void nextPositions(){
		Dictionary<Vector3, float> newPoss = new Dictionary<Vector3, float> ();
		Vector3 temp = transform.position;
		foreach (Vector3 v in WallDimention.dimensions){
			foreach (int i in WallDimention.directions){
				Vector3 newPos = temp + v*i*speed; 
				print(speed );
				newPoss[newPos] = WallDimention.away(newPos,target);
			}
		}
		// sorting
		var sortedPos = from entry in newPoss orderby entry.Value ascending select entry;
		KeyValuePair<Vector3, float>[] newP = sortedPos.ToArray();
		foreach (Vector3 v in newPoss.Keys){
			print (v.ToString()+ " " + v.ToString());
		}
		foreach (KeyValuePair<Vector3, float> v in newP){
			print (v.Key.ToString()+ " " + v.Value.ToString());
		}
	}

	public void behave(){
		// check if we have arrived to the target position
		if (arriveToTarget()){
			if (picked != 0) 
				collectibles[target] = true;
			else{
				destroy();
				return;
			}
			target = closestCollectible();
		}
		// move
		nextPositions ();
	}

	private void destroy(){
		GameObject.Destroy (gameObject);
	}
}
