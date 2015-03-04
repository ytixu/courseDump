using UnityEngine;
using System.Collections;

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

	public VerletNode(Point point, Vector3 preV){
		child = new ArrayList ();
		p = point;
		currPos = point.transform.position;
		prePos = currPos - preV;
	}
}
