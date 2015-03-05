using UnityEngine;
using System.Collections;

public class Canyon : MonoBehaviour {
	// for midpoint bisection
	public int depth; // number of iterations
	private float roughness = (float)1f/Mathf.Sqrt(2);

	public int width; // the distance between the two cannons
	public int hight; // the hight of the two cannons, between 1 and 2^depth
	public float slope; // positive value for the slope of the walls
	public int end; // x position of the border of the canyon

	public Material wallMat;
	public VerletCannon vc;
	public BallCannon bc;

	// storing the position of the walls for collision detection
	public float[] leftWall; 
	public float[] rightWall;
	public float bottom;
	public float top;

	// Use this for initialization
	void Start () {
		initialize ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void initialize(){
		int l = (int)Mathf.Pow (2, depth);
		float[] lineLeft = new float[l];
		float[] lineRight = new float[l];
		top = conv (l - 1);
		bottom = conv (0);
		midpointBisection (lineLeft, 0, l - 1, 1);
		midpointBisection (lineRight, 0, l - 1, 1);
		resize (lineLeft, lineRight, l);
		// draw
		combine(drawCanyon (lineRight, end, l),drawCanyon (lineLeft, -end, l));
		// set cannons
		vc.transform.parent = transform;
		bc.transform.parent = transform;
		vc.transform.localPosition = new Vector3 (lineLeft [hight]-0.2f, conv(hight));
		bc.transform.localPosition = new Vector3 (lineRight [hight]+0.2f, conv(hight));
		// walls
		rightWall = lineRight;
		leftWall = lineLeft;
	}
	
	// midpoint bisection a straight line recursively
	// only move the x component
	private void midpointBisection(float[] line, int s, int e, float sd){
		//print(s + " " +  e);
		if (s > e) return;
		int middle = (e + s) / 2;
		//print (middle);
		line [middle] = (line [e] - line [s])/2f + (Random.value - 0.5f) * sd; 
		sd *= roughness;
		midpointBisection (line, s, middle-1, sd);
		midpointBisection (line, middle + 1, e, sd);
	}

	// resize the x component according to width and height
	private void resize(float[] l, float[] r, int s){
		float diff = (width - r [hight] + l [hight])/2f;
		for (int i = hight; i<s; i++){
			l[i] = l[i] - diff - slope*(i-hight);
			r[i] = r[i] + diff + slope*(i-hight);
		}
		for (int i = hight-1; i>=0; i--){
			l[i] = l[i] - diff - slope*(i-hight);
			r[i] = r[i] + diff + slope*(i-hight);
		}
	}

	// mapping array index to y coordinate
	private float conv(int i){
		return i*3.5f/depth-4;
	}

	// mapping y coordinate to array index (floor of the value)
	private int convInv(float x){
		return (int)((x + 4) * depth / 3.5f);
	}

	private Mesh createMesh(Vector3[] vs){
		Mesh mesh = new Mesh();
		mesh.Clear();
		mesh.vertices = vs;
		mesh.uv = new Vector2[] {new Vector2(0,0),new Vector2(1,0),new Vector2(0,1),new Vector2(1,1)};
		mesh.triangles = new int[] {0, 3, 1, 0, 2, 3};
		mesh.RecalculateBounds();
		mesh.Optimize();
		return mesh;
	}

	private void combine(Mesh[] msL, Mesh[] msR){
		transform.GetComponent<MeshFilter> ();
		
		if(!transform.GetComponent<MeshFilter> () ||  !transform.GetComponent<MeshRenderer> () )
		{
			transform.gameObject.AddComponent<MeshFilter>();
			transform.gameObject.AddComponent<MeshRenderer>();
		}
		MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
		CombineInstance[] combine = new CombineInstance[msL.Length*2+1];
		int i = 0;
		foreach (Mesh m in msL) {
			combine[i].mesh = m;
			combine[i].transform = meshFilters[0].transform.localToWorldMatrix;
			i++;
		}
		foreach (Mesh m in msR) {
			combine[i].mesh = m;
			combine[i].transform = meshFilters[0].transform.localToWorldMatrix;
			i++;
		}
		combine[i].mesh = createMesh(new Vector3[]{
			new Vector3(-end, bottom), new Vector3(end, bottom), 
			new Vector3(-end, bottom-2), new Vector3(end, bottom-2)});
		combine[i].transform = meshFilters[0].transform.localToWorldMatrix;
		transform.GetComponent<MeshFilter>().mesh = new Mesh();
		transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine, true);
		transform.gameObject.AddComponent<MeshCollider>();
		gameObject.GetComponent<MeshCollider> ().isTrigger = true;
		transform.gameObject.active = true;
		transform.gameObject.renderer.material = wallMat;
	}

	private Mesh[] drawCanyon(float[] line, float end, int length){
		Mesh[] ms = new Mesh[length-1];

		float y1 = 0;
		float y2 = 0;
		for(int i=0; i<length-1; i++){
			y1 = conv(i);
			y2 = conv(i+1);
			if (end > 0){
				ms[i] = createMesh(new Vector3[]{new Vector3(line[i], y1),
					new Vector3(end, y1),
					new Vector3(line[i+1], y2),
					new Vector3(end, y2)});
				//rightWall[i] = new Vector2(line[i], y1);
			}else{
				ms[i] = createMesh(new Vector3[]{new Vector3(end, y1),
					new Vector3(line[i], y1),
					new Vector3(end, y2),
					new Vector3(line[i+1], y2)});
				//leftWall[i] = new Vector2(line[i], y1);
			}
		}
		//if (end > 0){
		//	rightWall[length-1] = new Vector2(line[length-1], y2);
		//}else{
		//	leftWall[length-1] = new Vector2(line[length-1], y2);
		//}
		return ms;
	}

	/**
	 * This function is called by other object to check if the latter has collided a walls
	 * if colliding, return the position where there's a collision
	 */
	public Vector2 hasCollide(float posx, float posy, float radius){
		float temp = posy - radius;
		if (temp >= top){
			return Vector2.zero;
		}
		// get relevant y position on wall
		float y = 0;
		if (temp <= bottom){
			y = bottom;
		}
		// get relevant x position on wall
		int ind = convInv (temp);
		float y0 = conv (ind);
		float y1 = conv (ind+1);
		if (posx < 0){ // check for leftwall
			float x0 = leftWall[ind];
			float x1 = leftWall[ind+1];
			float ratio = (posy-y0)/(y1-y0);
			float x = x0 + ratio*(x1-x0);
			if (posx - radius < x) return new Vector2(x, y);

		}else{ // check for right wall
			float x0 = rightWall[ind];
			float x1 = rightWall[ind+1];
			float ratio = (posy-y0)/(y1-y0);
			float x = x0 + ratio*(x1-x0);
			if (posx + radius > x) return new Vector2(x, y);
		}
		return new Vector2(0,y);
	}
}
