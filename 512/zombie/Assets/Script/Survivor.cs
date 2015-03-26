using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Survivor : MonoBehaviour {

	private Dictionary<GameObject, bool> collectibles;
	// a position is false if survivor has already passed there

	public ZombieFactory zf;
	public SurvivorFactory sf;

	// states 
	private Vector3 target;
	private GameObject targetObj;
	private bool reachedTarget = false;
	private int picked; // this is to keep track of the number of places the survivor needs to go
						// =0 if it has successfully arrive to the goal with all the collectible object picked
	private float speed;
	private bool active = false;
	private float resetTime = 0;
	private bool isSeen = false;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (resetTime < Time.time && isSeen){
			isSeen = false;
			getClosestCollectible();
			reachedTarget = false;
		}
		if (active){
			behave ();
			zf.highlightVisibleZombies(transform.position);
		}
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
			if (hit.collider.tag == obj) return true;
		}
		return false;
	}

	// check if hitting wall
	private bool hitWall(Vector3 newPos){
		foreach (Vector3 v in WallDimention.dimensions){
			foreach (int i in WallDimention.directions){
				if (collisionCheck(newPos, 0.35f, v*i,"Wall"))
					return true;
			}
		}
		foreach (int i in WallDimention.directions){
			foreach (int j in WallDimention.directions){
				if (collisionCheck(newPos, 0.4f, WallDimention.X_DIR*i+WallDimention.Z_DIR*j,"Wall"))
					return true;
			}
		}
		return false;
	}

	private bool closeZombieFOV(Vector3 newPos){
		foreach (Zombie z in zf.getVisibleZombies(newPos)) {
			if (z.inFOV(newPos, 2, 8, 10)){
				return true;
			}
		}
		return false;
	}

	private bool inZombieFOV(Vector3 newPos){
		foreach (Zombie z in zf.getVisibleZombies(newPos)) {
			if (z.inFOV(newPos, 1.8f, 5, 8)){
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
	}

	// check if we have arrive to the target position
	private bool arriveToTarget(){
		if (Vector3.Distance(target, transform.position) < 0.8){
			picked -= 1;
			return true;
		}
		return false;
	}

	// get the next 4 positions (up, down, left, right) and order them by the value of their potential
	private KeyValuePair<Vector3, float>[] nextPositions(){
		bool isSeenCurrent = closeZombieFOV (transform.position) || isSeen;
		Dictionary<Vector3, float> newPoss = new Dictionary<Vector3, float> ();
		Vector3 temp = transform.position;
		foreach (Vector3 v in WallDimention.dimensions){
			foreach (int i in WallDimention.directions){
				Vector3 newPos = temp + v*i*speed; 
				if (!isSeenCurrent) newPoss[newPos] = WallDimention.away(newPos,target);
				else newPoss[newPos] = zf.away(newPos);
			}
		}
		foreach (int i in WallDimention.directions){
			foreach (int j in WallDimention.directions){
				Vector3 newPos = temp + (WallDimention.X_DIR*i+WallDimention.Z_DIR*j).normalized*speed;
				if (!isSeenCurrent) newPoss[newPos] = WallDimention.away (newPos, target);
				else newPoss[newPos] = zf.away(newPos);
			}
		}
		if (isSeenCurrent && !isSeen){
			isSeen = true;
			resetTime = Time.time + 0.5f;
		}
		// sorting
		var sortedPos = from entry in newPoss orderby entry.Value ascending select entry;
		return sortedPos.ToArray();
	}

	public void behave(){
		// check if we have arrived to the target position
		if (arriveToTarget()){
			print (picked);
			if (picked == 0){
				print ("WIN");
				sf.destroySurvivor();
				return;
			}
			if (reachedTarget){
				collectibles[targetObj] = true;
				reachedTarget = false;
			} else if (!reachedTarget){
				target = targetObj.transform.position;
				reachedTarget = true;
				return;
			}
			getClosestCollectible();
		}
		// move
		foreach (KeyValuePair<Vector3, float> pair in nextPositions ()){
			if (hitWall(pair.Key) || inZombieFOV(pair.Key)){
				continue;
			}else{
				transform.position = pair.Key;
				break;
			}
		}
	}
}
