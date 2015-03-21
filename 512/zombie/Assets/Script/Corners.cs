using UnityEngine;
using System.Collections;
/**
 * This class computes the direction where the zombie must go to
 * where the latter arrives at a corner
 */

public class Corners : MonoBehaviour {

	// data for the tracks (storing the vertices)
	// each row represents a track
	// direction is clockwise
	private Vector2[][] corners = {
		new Vector2[]{
			new Vector2 (33.5f, 1.5f),
				new Vector2 (3.5f, 1.5f),
				new Vector2 (3.5f, 19.5f), 
				new Vector2 (33.5f, 19.5f)
		},new Vector2[]{
			new Vector2 (32.5f, 2.5f),
				new Vector2 (4.5f, 2.5f),
				new Vector2 (4.5f, 18.5f),
				new Vector2 (32.5f, 18.5f)
		},new Vector2[]{
			new Vector2 (31.5f, 3.5f),
				new Vector2 (5.5f, 3.5f),
				new Vector2 (5.5f, 17.5f),
				new Vector2 (31.5f, 17.5f)
		}
	};

	public GameObject tileParent;

	private Vector2[] trackDelta = new Vector2[]{
		new Vector2(1,0), new Vector2(0,-1), new Vector2(-1,0), new Vector2(0,1)
	};

	// this is to store the corner index to specify which corner a zombie is walking to
	public class CornerIndex{
		public int i;
		public int j;
		public CornerIndex(int i_, int j_){
			i = i_;
			j = j_;
		}
	}

	public Vector2 getCorner(CornerIndex ci){
		return corners [ci.i] [ci.j];
	}
	public Vector2 getCorner(int i, int j){
		return corners [i] [j];
	}

	void Start(){
		foreach (Vector2[] vs in corners){
			foreach (Vector2 v in vs){
				GameObject tile = (GameObject) Instantiate(tileParent);
				tile.transform.parent = this.transform;
				tile.transform.localPosition = new Vector3(v.x, v.y, 0);
				tile.transform.localScale = new Vector3(1,1,1);
				tile.transform.localEulerAngles = new Vector3(0,0);
			}
		}
	}

	// this is called by a zombie when it arrives at a corner
	// returns the next corner to go to
	public Vector2 turn(CornerIndex ci){
		ci.j = (ci.j + 1) % 4;
		return getCorner(ci);
	}
	public Vector2 turnReverse(CornerIndex ci){
		//print (ci.j + " " + (4 + ci.j - 1) % 4);
		ci.j = (4 + ci.j - 1) % 4;
		return getCorner(ci);
	}
	
	// switch to a random track adjacent to the current one
	public Vector2 changeTrack(CornerIndex ci, Zombie z){
		if (ci.i == 2 || (Random.value > 0.5 && ci.i > 0)){
			if (!z.zombieNearBy(trackDelta[ci.j])){	
				ci.i --;
				return trackDelta[ci.j];
			}
		}else if (!z.zombieNearBy(trackDelta[(ci.j+2)%4])){
			ci.i ++;
			return trackDelta[(ci.j+2)%4];
		}
		return Vector2.zero;
	}
	
	// this is called by a zombie to compute the (heuristic) distance between current position
	// and the position of the next corner
	// This distance ignore the x-component if zombie is moving in the y direction
	// or the y-component if zombie is moving in the x direction
	public float distanceLeft(CornerIndex ci, Vector3 position, bool direction){
		Vector2 temp = getCorner(ci);
		if ((ci.j % 2 == 0 && direction) || (ci.j % 2 == 1 && !direction)){
			//print (temp.ToString() + " " + position.ToString () + " y");
			return (float) Mathf.Abs(temp.y - position.y);
		}else{
			//print (temp.ToString() + " " + position.ToString () + " x");
			return (float) Mathf.Abs(temp.x - position.x);
		}
	}

	public CornerIndex randomCorner(){
		return new CornerIndex(Random.Range(0,3), Random.Range(0,4));
	}
}






