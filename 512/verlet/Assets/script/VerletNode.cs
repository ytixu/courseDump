using UnityEngine;
using System.Collections;

/**
 * Data structure for storing the verlet graph.
 * Each node contains:
 * point GameObject,
 * current position, 
 * previous position
 * 
 * Each edge contains:
 * line GameObject,
 * distance 
 */

public class VerletNode {

	public Point p;
	public ArrayList child;
	public Vector3 currPos;
	public Vector3 prePos;

	public class ChildNode{
		public Line l { get; set; }
		public VerletNode n { get; set; }
		public float dist { get; set; }
	}

	public VerletNode(Point point){
		child = new ArrayList ();
		p = point;
	}
}
