using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Verlet : MonoBehaviour {

	public float size;
	public Point aPoint;
	public Line aLine;

	private VerletNode root;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void initialize(Vector2 v){
		transform.localScale = new Vector3 (size, size);
		transform.localPosition = new Vector3 (v.x, v.y);
		root = constructVerletTree (0);
	}

	// recursive construction of the tree
	private VerletNode constructVerletTree(int index){
		DogData.PosPointPair value = DogData.dogGraph [index];
		// create point
		Point newP = (Point)Instantiate (aPoint);
		newP.initialize (value.pos, this);
		// create node
		VerletNode node = new VerletNode (newP);
		// iterate child
		foreach (DogData.DistPointPair p in value.child){
			// recursion
			VerletNode c = constructVerletTree(p.pointID);
			// create line
			Line newLine = null;
			if (p.visible){
				newLine = (Line) Instantiate(aLine);
				newLine.initialize(this);
				newLine.setPosition(node.p.transform.localPosition, c.p.transform.localPosition); 
			}
			// add child
			node.child.Add(new VerletNode.ChildNode { l = newLine, n = c, dist = p.dist });
		}
		return node;
	}
}
