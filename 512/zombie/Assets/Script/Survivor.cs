using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Survivor : MonoBehaviour {

	// at the places that the survivor need to pass before reaching the goal state
	private Dictionary<GameObject, bool> collectibles;
	// a position is false if survivor has already passed there

	public ZombieFactory zf;
	public SurvivorFactory sf;

	private bool active = false; 

	// states 
	private Vector3 target; // the targeting position (a collectible or the goal position)
	private GameObject targetObj; // the targeting object (the use of this is explained later)
	private bool reachedTarget = false; // (also explained later)
	private int picked; // this is to keep track of the number of places the survivor needs to go
						// =0 if it has successfully arrive to the goal with all the collectible object picked
	private float speed; 
	private float resetTime = 0; // this is to keep track of whether the survivor has been close to a zombie in the past 0.5 seconds
	private bool isSeen = false; // same purpose as previous state variable

	// a part of the behavior tree
	void Update () {
		// if survivor has been close to a zombie 0.5 ago
		// we were running away from the zombie, so we setup a new target
		if (resetTime < Time.time && isSeen){
			isSeen = false;
			getClosestCollectible();
			reachedTarget = false;
		}
		// update position
		if (active){
			behave ();
			zf.highlightVisibleZombies(transform.position);
		}
	}

	// initialize state variables and the positions of the collectibles
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
	 * MAIN BEHAVIOR TREE
	 * in summary:
	 * get closest collectible object (target)
	 * move closer to there if there is no wall, or zombie 
	 * otherwise move away from zombie
	 * 
	 * use potential field to move towards target 
	 * get all the collectible objects before targetting the goal 
	 */

	// use ray casting for collision
	private bool collisionCheck(Vector3 pos, float d, Vector3 direction, string obj){
		RaycastHit hit;
		if (Physics.Raycast (pos, direction, out hit, d)){
			if (hit.collider.tag == obj) return true;
		}
		return false;
	}

	// check if hitting wall
	// ray cast in 8 directions
	private bool hitWall(Vector3 newPos){
		foreach (Vector3 v in WallDimention.dimensions){
			foreach (int i in WallDimention.directions){
				if (collisionCheck(newPos, 0.35f, v*i,"Wall"))
					return true;
			}
		}
		foreach (int i in WallDimention.directions){
			foreach (int j in WallDimention.directions){
				// the length of the ray is longer than the previous ones, this is to form a "square-like" box
				// arround the survivor (it seems to work better this way)
				if (collisionCheck(newPos, 0.4f, WallDimention.X_DIR*i+WallDimention.Z_DIR*j,"Wall"))
					return true;
			}
		}
		return false;
	}

	// check if the survivor is close to a zombie
	// we inflate the FOV of the zombie to do this
	private bool closeZombieFOV(Vector3 newPos){
		foreach (Zombie z in zf.getVisibleZombies(newPos)) {
			if (z.inFOV(newPos, 2, 8, 10)){
				return true;
			}
		}
		return false;
	}

	// check if the survivor is inside the FOV of a zombie
	// again, this FOV is inflated, be less than closeZobieFOV, 
	// so we can be sure that the survivor will not be seen
	private bool inZombieFOV(Vector3 newPos){
		foreach (Zombie z in zf.getVisibleZombies(newPos)) {
			if (z.inFOV(newPos, 1.8f, 5, 8)){
				return true;
			}
		}
		return false;
	}

	// find the closest collectible 
	// if all collectibles are collected, return the goal position
	// the strategy in this algorithm is to define a potential function with no local minimum 
	// according to the targeting position.
	// since some collectibles are physically positionned at places such that local minimums are 
	// difficult to avoid using simple potential functions, we create a "pseudo" collectible 
	// that is an invisible object, for each real collectible, placed somewhere "easier" to reach 
	// according to the potential function. The survivor need to pass there
	// before targetting the real collectible. 
	private void getClosestCollectible(){
		float minDist = 1000;
		target = WallDimention.GOAL;
		foreach (GameObject go in collectibles.Keys){
			if (collectibles[go]) continue; // if we have already collected it
			Vector3 v = go.transform.GetChild(0).transform.position; // get the "pseudo" collectible's position
			float dist = Vector3.Distance(v, transform.position);
			if (dist < 2){ // if the distance is smaller that 2, then just go target that one
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

	// get the next 8 positions (up, down, left, right, diagonals) and order them by the 
	// value of their potential
	// note that we have two potential functions:
	// 1) when survivor is aiming at the target
	// 2) when survivor is running away from the zombie
	private KeyValuePair<Vector3, float>[] nextPositions(){
		bool isSeenCurrent = closeZombieFOV (transform.position) || isSeen;
		Dictionary<Vector3, float> newPoss = new Dictionary<Vector3, float> ();
		Vector3 temp = transform.position;
		// up, down, left, right
		foreach (Vector3 v in WallDimention.dimensions){
			foreach (int i in WallDimention.directions){
				Vector3 newPos = temp + v*i*speed; 
				if (!isSeenCurrent) newPoss[newPos] = WallDimention.away(newPos,target);
				else newPoss[newPos] = zf.away(newPos);
			}
		}
		// diagonals
		foreach (int i in WallDimention.directions){
			foreach (int j in WallDimention.directions){
				Vector3 newPos = temp + (WallDimention.X_DIR*i+WallDimention.Z_DIR*j).normalized*speed;
				if (!isSeenCurrent) newPoss[newPos] = WallDimention.away (newPos, target);
				else newPoss[newPos] = zf.away(newPos);
			}
		}
		// if was close to a zombie
		if (isSeenCurrent && !isSeen){
			isSeen = true;
			resetTime = Time.time + 0.5f;
		}
		// sorting
		var sortedPos = from entry in newPoss orderby entry.Value ascending select entry;
		return sortedPos.ToArray();
	}

	// this is the main part of the bahvior tree
	public void behave(){
		// check if we have arrived to the target position
		if (arriveToTarget()){
			if (picked == 0){ // winning
				print ("WIN");
				sf.destroySurvivor();
				return;
			}
			if (reachedTarget){ // if we have already reached the "pseudo" collectible last time
				collectibles[targetObj] = true; // reached the real collectible
				reachedTarget = false;
			}else{
				target = targetObj.transform.position; // need to reach the real collectible
				reachedTarget = true;
				return;
			}
			getClosestCollectible(); // update target
		}
		// move
		// get the next position available with lowest potential value
		foreach (KeyValuePair<Vector3, float> pair in nextPositions ()){
			if (hitWall(pair.Key) || inZombieFOV(pair.Key)){ 
				// if this position hits the wall or can be seen by a zombie
				continue;
			}else{
				transform.position = pair.Key;
				break;
			}
		}
	}
}
