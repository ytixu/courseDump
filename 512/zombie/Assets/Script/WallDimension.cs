using UnityEngine;
using System.Collections;

/**
 * this class helps to calculate the potential field
 */

public class WallDimention {

	public static int MAX_X = 30;
	public static int MAX_Z = 16;
	public static int MIN_X = 7;
	public static int MIN_Z = 5;

	public static int SIZE_X = 21;
	public static int SIZE_Z = 9;

	public static float CENTER_X = 18.5f;
	public static float CENTER_Z = 10.5f;

	public static Vector3 X_DIR = new Vector3(1,0,0);
	public static Vector3 Z_DIR = new Vector3(0,0,1);

	public static Vector3[] dimensions = new Vector3[]{ X_DIR, Z_DIR };
	public static int[] directions = new int[]{ 1, -1 };
	
	public static Vector3 GOAL = new Vector3 (0.5f, 0, CENTER_Z);


	// this is a heuristic that estimates how much a location is 
	// inside the box defined by the constants
	public static float inWall(Vector3 pos){
		if (pos.x < MAX_X && pos.x > MIN_X && pos.z < MAX_Z && pos.z > MIN_Z) 
			return (float) Mathf.Min(Mathf.Abs(MAX_X - pos.x), 
					                 Mathf.Abs(MAX_Z - pos.z),
			                         Mathf.Abs(pos.x - MIN_X),
					                 Mathf.Abs(pos.z - MIN_Z));
		return -1;
	}

	// this is a heuristic that estimates how much away we are from a target
	public static float away(Vector3 pos, Vector3 target){
		float dist = Vector3.Distance (pos, target);
		// if the distance is less that 2, then just return that distance
		if (dist < 2) return dist;
		// check if pos is "inWall"
		float in_wall = inWall (pos);
		if (in_wall > 0) return (in_wall+dist)*1000;
		// check if pos and target are at opposite sides
		if (Mathf.Abs(pos.x - target.x) > MAX_X){
			return (float) (Mathf.Abs(pos.x - CENTER_X)+dist)*10;
		}else if (Mathf.Abs(pos.z -target.z) > MAX_Z){
			return (float) (Mathf.Abs(pos.z - CENTER_Z)+dist)*10;
		}
		// otherwise just return that distance
		return dist;
	}
}
