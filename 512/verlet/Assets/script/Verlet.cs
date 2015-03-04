using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Verlet : MonoBehaviour {

	public float size;
	public Point aPoint;
	public Line aLine;

	private static Vector3 acc = new Vector3(0, -0.01f, 0);  // gravity * time^2

	private VerletNode root; // tree for the verlet's points and lines

	private float thr = 0.01f; // threshold for the distance between two points
	private bool isGood; // whether we can stop iterating
	private bool canRepaint = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (canRepaint){
			canRepaint = false;
			repaint(root);
			animate();
		}
	}

	public void initialize(Vector3 v, Vector3 preV){
		transform.localScale = new Vector3 (size, size);
		transform.localPosition = new Vector3 (v.x, v.y);
		root = constructVerletTree (0, (v-preV)/3f);
		//animate ();
	}

	// recursive construction of the tree
	private VerletNode constructVerletTree(int index, Vector3 preV){
		DogData.PosPointPair value = DogData.dogGraph [index];
		// create point
		Point newP = (Point)Instantiate (aPoint);
		newP.name = index.ToString();
		newP.initialize (value.pos, this);
		// create node
		VerletNode node = new VerletNode (newP, preV);
		// iterate child
		foreach (DogData.DistPointPair p in value.child){
			// recursion
			VerletNode c = constructVerletTree(p.pointID, preV);
			// create line
			Line newLine = null;
			if (p.visible){
				newLine = (Line) Instantiate(aLine);
				newLine.initialize(this);
				newLine.setPosition(node.p.transform.position, c.p.transform.position); 
			}
			// add child
			node.child.Add(new VerletNode.ChildNode { l = newLine, n = c, dist = p.dist });
		}
		return node;
	}

	// recurively update the position of the points
	private void updatePosition(VerletNode node){
		Vector3 newPos = 2 * node.currPos - node.prePos + acc;
		node.prePos = node.currPos;
		node.currPos = newPos;
		foreach (VerletNode.ChildNode n in node.child) {
			updatePosition(n.n);
		}
	}

	// recursively iterate to satisfy all distance constraint of the verlet
	private void repositionPoints(VerletNode node){
		foreach(VerletNode.ChildNode n in node.child){
			Vector3 d = node.currPos - n.n.currPos;
			if (Vector3.Magnitude(d) - n.dist > thr){
				isGood = false;
				d = d/2;
				node.currPos = (node.currPos - d);
				n.n.currPos = (n.n.currPos + d);
			}
		}
	}

	private void repaint(VerletNode node){
		node.p.transform.position = node.currPos;
		foreach (VerletNode.ChildNode n in node.child){
			print (n.n.p.name);
			if (n.l != null){
				n.l.setPosition(node.currPos, n.n.currPos);
			}
			repaint (n.n);
		}
	}

	private void animate(){
		updatePosition (root);
		isGood = false;
		while(! isGood){
			isGood = true;
			repositionPoints(root);
		}
		canRepaint = true;
	}
}
