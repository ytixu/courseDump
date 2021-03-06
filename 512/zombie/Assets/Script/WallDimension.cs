﻿using UnityEngine;
using System.Collections;

/**
 * this class helps to calculate the potential field
 * note that it's called WallDimention because initially, I just wanted this class to store the location 
 * and size of the walls in the middle of the game room
 */

public class WallDimention {
	
	public static float MAX_X = 30.5f;
	public static float MAX_Z = 16.5f;
	public static float MIN_X = 6.5f;
	public static float MIN_Z = 4.5f;

	public static int SIZE_X = 21;
	public static int SIZE_Z = 9;

	public static float CENTER_X = 18.5f;
	public static float CENTER_Z = 10.5f;

	// this is used by Survivor to get the next positions
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


	// POTENTIAL FUNCITON:
	// how much away we are from a target
	public static float away(Vector3 pos, Vector3 target){
		// if we are near the starting position
		if (pos.x > 34.5){
			return pos.x*1000;
		}
		float dist = Vector3.Distance (pos, target);
		// if the distance is less that 2, then just return that distance
		if (dist < 2.5) return dist;
		// check if pos is "inWall"
		float in_wall = inWall (pos);
		if (in_wall > 0) return (in_wall+dist)*1000;
		// check if pos and target are at opposite sides
		if (target.x > 2 || pos.x > MAX_X){
			if (Mathf.Abs(pos.x - target.x) > SIZE_X){
				return (float) Mathf.Abs(pos.x - CENTER_X)*10;
			}else if (Mathf.Abs(pos.z -target.z) > SIZE_Z){
				return (float) Mathf.Abs(pos.z - CENTER_Z)*10;
			}
		}
		// otherwise just return that distance
		return dist;
	}
}

