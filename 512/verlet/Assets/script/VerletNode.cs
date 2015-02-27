using UnityEngine;
using System.Collections;

public class VerletNode {

	public Point p;
	public ArrayList child;

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
