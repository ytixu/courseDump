using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Survivor : MonoBehaviour {

	private Dictionary<GameObject, bool> collectibles;
	// a position is false if survivor has already passed there

	// states 
	private Vector3 target;
	private GameObject targetObj;
	private int picked; // this is to keep track of the number of places the survivor needs to go
						// =0 if it has successfully arrive to the goal with all the collectible object picked
	private float speed;
	private bool active = false;
	

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (active)
			behave ();
	}

	public void initialize(GameObject collects, float s){
		picked = 1;
		collectibles = new Dictionary<GameObject, bool> ();
		foreach (Transform c in collects.transform)
		{
			collectibles.Add(c.gameObject, false);
			picked += 2;
		}
		getClosestCollectible ();
		speed = s;
		active = true;
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
		foreach (Vector3 v in WallDimention.dimensions){
			foreach (int i in WallDimention.directions){
				if (collisionCheck(newPos, 0.4f, v*i,"Wall"))
					return true;
			}
		}
		foreach (int i in WallDimention.directions){
			foreach (int j in WallDimention.directions){
				if (collisionCheck(newPos, 0.5f, WallDimention.X_DIR*i+WallDimention.Z_DIR*j,"Wall"))
					return true;
			}
		}
		return false;
	}

	private void getClosestCollectible(){
		float minDist = 1000;
		target = WallDimention.GOAL; // if all collectibles are picked, 
												// then just return the goal state
		foreach (GameObject go in collectibles.Keys){
			if (collectibles[go]) continue;
			Vector3 v = go.transform.GetChild(0).transform.position;
			float dist = Vector3.Distance(v, transform.position);
			if (dist < 2){
				target = v;
				targetObj = go;
				return;
			}
			if (dist < minDist){
				minDist = dist;
				target = v;
				targetObj = go;
			}
		}
		print ("next target " + target.ToString ());
	}

	// check if we have arrive to the target position
	private bool arriveToTarget(){
		if (Vector3.Distance(target, transform.position) < 1){
			picked -= 1;
			return true;
		}
		return false;
	}

	// get the next 4 positions (up, down, left, right) and order them by the value of their potential
	private KeyValuePair<Vector3, float>[] nextPositions(){
		Dictionary<Vector3, float> newPoss = new Dictionary<Vector3, float> ();
		Vector3 temp = transform.position;
		foreach (Vector3 v in WallDimention.dimensions){
			foreach (int i in WallDimention.directions){
				Vector3 newPos = temp + v*i*speed; 
				newPoss[newPos] = WallDimention.away(newPos,target);
			}
		}
		// sorting
		var sortedPos = from entry in newPoss orderby entry.Value ascending select entry;
		return sortedPos.ToArray();
	}

	public void behave(){
		// check if we have arrived to the target position
		print ("target " + target.ToString () + " " + picked);
		if (arriveToTarget()){
			if (targetObj != null) collectibles[targetObj] = true;
			if (picked == 0){
				destroy();
				return;
			}
			print ("arrived target "+target.ToString());
			if (targetObj != null){
				target = targetObj.transform.position;
				targetObj = null;
				return;
			}
			getClosestCollectible();
		}
		// move
		foreach (KeyValuePair<Vector3, float> pair in nextPositions ()){
			if (hitWall(pair.Key)){
				print ("collision");
				continue;
			}else{
				transform.position = pair.Key;
				break;
			}
		}
	}

	private void destroy(){
		GameObject.Destroy (gameObject);
	}
}
