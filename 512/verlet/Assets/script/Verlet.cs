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
		transform.localEulerAngles = new Vector3 (0, 0, 180);
		Vector3 temp = (v - preV) /8f;
		currPos = v;
		prePos = v-temp;
		root = constructVerletTree (0);
		fired = true;
	}

	// recursive construction of the tree
	private VerletNode constructVerletTree(int index){
		if (nodeList [index] != null){
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
	// according to verlet integration
	private void updatePosition(){
		foreach (VerletNode node in nodeList) {
			Vector3 newPos = 2 * node.currPos - node.prePos + acc;
			// collision
			Vector2[] col = canyon.hasCollide (newPos.x, newPos.y, 0.05f);
			if (col.Length == 0) continue;
			if (!col[0].Equals(Vector2.zero)){
				if (Mathf.Abs(col[0][0]) > 0){ // if collide with wall
					//newPos.y = (col[0][0]-node.currPos.x)/(newPos.x-node.currPos.x)
					//	*(newPos.y-node.currPos.y) + node.currPos.y;
					newPos.x = col[0][0];
				}
				if (Mathf.Abs (col[0][1]) > 0){ // if landing
					destroy();
				}
			}
			node.prePos = node.currPos;
			node.currPos = newPos;
			// check if a point has gone outside of the screen
			// this is sufficient to decide whether we should destroy the verlet
			if (Screen.isOutOfScene (newPos.x, newPos.y)) {
				destroy ();
				return;
			}
		}
	}
	
	// one iteration to satisfy distance constraints of the verlet
	private void repositionPoints(){
		foreach(VerletNode node in nodeList){
			foreach(VerletNode.ChildNode n in node.child){
				Vector3 delta = node.currPos - n.n.currPos;
				float deltaLength = Mathf.Sqrt(Vector3.Dot (delta,delta));
				float diff = (deltaLength-n.dist) / deltaLength;
				node.currPos = node.currPos - delta * 0.5f * diff;
				n.n.currPos = n.n.currPos + delta * 0.5f * diff;
			}
		}
	}

	// reposition the lines and points
	private void repaint(){
		for(int i=0; i<nodeList.Length; i++){
			VerletNode node = nodeList[i];
			if (i==0){// update position of the verlet 
				transform.position = node.currPos;
			}
			node.p.transform.position = node.currPos;
			foreach (VerletNode.ChildNode n in node.child){
				if (n.l != null){
					n.l.setPosition(node.currPos, n.n.currPos);
				}
			}
		}
	}

	// animate the verlet
	private void animate(){
		updatePosition ();
		int n = 0;
		while(n < 3){ 
			repositionPoints();
			n ++;
		}
		canRepaint = true;
	}

	// update the entire verlet object's position 
	// treating it as one single point 
	// this is used to get better efficiency
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
		if (col.Length == 0) return;
		if (!col[0].Equals(Vector2.zero)){
			// now treat each point individually
			fired = false;
			setPos();
			animate();
		}
	}

	// helper function to initialize the positions of the points
	private void setPos(){
		Vector3 temp = currPos - prePos;
		foreach (VerletNode n in nodeList) {
			n.currPos = n.p.transform.position;
			n.prePos = n.currPos - temp;
		}
	}

	// recurively update all lines only
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

	// this is called by CannonBall object to see if there is a collision with a verlet
	public bool checkCollision(Vector3 pos){
		foreach (VerletNode n in nodeList){
			if (Vector3.Magnitude(n.p.transform.position - pos) < 1.05){
				destroy();
				return true;
			}
		}
		return false;
	}
}
