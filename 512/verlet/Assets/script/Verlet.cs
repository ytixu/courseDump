using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Verlet : MonoBehaviour {
	public Canyon canyon;

	public float size;
	public Point aPoint;
	public Line aLine;

	private static Vector3 acc = new Vector3(0, -0.002f, 0);  // gravity * time^2

	private VerletNode root; // tree for the verlet's points and lines
	private VerletNode[] nodeList;

	private float thr = 0.01f; // threshold for the distance between two points
	private bool isGood; // whether we can stop iterating
	private bool canRepaint = false;

	private Vector3 currPos;
	private Vector3 prePos;

	private bool fired = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	//void Update () {
	//	
	void Update () {
		if (fired) updateAllPosition ();
		if (canRepaint){
			canRepaint = false;
			repaint();
			animate();
		}
	}

	public void initialize(Vector3 v, Vector3 preV){
		nodeList = new VerletNode[DogData.dogGraph.Count];
		transform.localScale = new Vector3 (size, size);
		transform.localPosition = new Vector3 (v.x, v.y);
		print (v.ToString () + preV.ToString ());
		Vector3 temp = (v - preV) / 6f;
		currPos = v;
		prePos = v-temp;
		root = constructVerletTree (0);
		fired = true;
	}

	// recursive construction of the tree
	private VerletNode constructVerletTree(int index){
		//print (index); 
		if (nodeList [index] != null){
			print(index);
			return nodeList [index];
		}
		DogData.PosPointPair value = DogData.dogGraph [index];
		// create point
		Point newP = (Point)Instantiate (aPoint);
		newP.name = index.ToString();
		newP.initialize (value.pos, this);
		// create node
		VerletNode node = new VerletNode (newP);
		nodeList [index] = node;
		// iterate child
		foreach (DogData.DistPointPair p in value.child){
			// recursion
			VerletNode c = constructVerletTree(p.pointID);
			// create line
			Line newLine = null;
			if (p.visible){
				newLine = (Line) Instantiate(aLine);
				newLine.initialize(this);
				newLine.setPosition(node.p.transform.position, c.p.transform.position); 
			}
			// add child
			node.child.Add(new VerletNode.ChildNode { l = newLine, n = c, 
				dist = Vector3.Distance(node.p.transform.position, c.p.transform.position)});
		}
		return node;
	}

	// update the position of the points
	private void updatePosition(){
		foreach (VerletNode node in nodeList) {
			Vector3 newPos = 2 * node.currPos - node.prePos + acc;
			// collision
			Vector2[] col = canyon.hasCollide (newPos.x, newPos.y, 0.05f);
			if (!col[0].Equals(Vector2.zero)){
				if (Mathf.Abs(col[0][0]) > 0){
					newPos.x = col[0][0];
				}
				if (Mathf.Abs (col[0][1]) > 0){
					destroy();
				}
			}
			node.prePos = node.currPos;
			node.currPos = newPos;
		}
	}
	
	// iterate to satisfy all distance constraint of the verlet
	private void repositionPoints(){
		foreach(VerletNode node in nodeList){
			foreach(VerletNode.ChildNode n in node.child){
				float dist = Vector3.Distance(node.currPos, n.n.currPos);
				float d = (float)Mathf.Abs(dist - n.dist);
				print (d.ToString() + " " + dist.ToString());
				if (d > thr){
					isGood = false;
					Vector3 diff = (node.currPos - n.n.currPos)*d/2f/dist;
					print (diff.ToString());
					node.currPos = (node.currPos - diff);
					n.n.currPos = (n.n.currPos + diff);
				}
			}
		}
	}
	
	private void repaint(){
		foreach(VerletNode node in nodeList){
			node.p.transform.position = node.currPos;
			foreach (VerletNode.ChildNode n in node.child){
				print (n.n.p.name);
				if (n.l != null){
					n.l.setPosition(node.currPos, n.n.currPos);
				}
			}
		}
	}
	
	private void animate(){
		updatePosition ();
		isGood = false;
		int n = 0;
		while(! isGood && n < 1){ 
			isGood = true;
			repositionPoints();
			n ++;
		}
		canRepaint = true;
	}


	private void updateAllPosition(){
		Vector3 newP = currPos * 2 - prePos + acc;
		// check if it has gone out of the scene
		if (Screen.isOutOfScene (newP.x, newP.y)) {
			Destroy (gameObject);
			return;
		}
		prePos = currPos;
		currPos = newP;
		transform.localPosition = newP;
		repaintLines (root);
		// check collision
		Vector2[] col = canyon.hasCollide (newP.x + 0.6f, newP.y + 0.6f, 0.6f); // approximation of the circle collider
		if (!col[0].Equals(Vector2.zero)){
			fired = false;
			setPos();
			animate();
		}
	}

	private void setPos(){
		Vector3 temp = currPos - prePos;
		foreach (VerletNode n in nodeList) {
			n.currPos = n.p.transform.position;
			n.prePos = n.currPos - temp;
		}
	}

	// recurively update all lines
	private void repaintLines(VerletNode node){
		foreach (VerletNode.ChildNode n in node.child){
			if (n.l != null){
				n.l.setPosition(node.p.transform.position, n.n.p.transform.position);
			}
			repaintLines (n.n);
		}
	}

	private void destroy(){
		Destroy (gameObject);
	}

	public bool checkCollision(Vector3 pos){
		print ("COOO");
		foreach (VerletNode n in nodeList){
			if (Vector3.Magnitude(n.p.transform.position - pos) < 1.05){
				destroy();
				return true;
			}
		}
		return false;
	}
}
