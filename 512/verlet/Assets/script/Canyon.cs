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
	// cache these values
	public float bottom; // bottom of the canyon (y coordinate)
	public float top; // top of the canyon (y coordinate)
	public float left; // top left edge (x coordinate)
	public float right; // top right edge (x coordinate)

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
		// computer canyon walls
		midpointBisection (lineLeft, 0, l - 1, 1);
		midpointBisection (lineRight, 0, l - 1, 1);
		resize (lineLeft, lineRight, l);
		// draw canyon
		combine(drawCanyon (lineRight, end, l),drawCanyon (lineLeft, -end, l));
		// set cannons
		vc.transform.parent = transform;
		bc.transform.parent = transform;
		vc.transform.localPosition = new Vector3 (lineLeft [hight]-0.2f, conv(hight));
		bc.transform.localPosition = new Vector3 (lineRight [hight]+0.2f, conv(hight));

		rightWall = lineRight;
		leftWall = lineLeft;
		left = leftWall [l-1];
		right = rightWall [l - 1];
	}
	
	// midpoint bisection of a straight line recursively
	// only move the x component of the walls
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

	// resize the x component according to the position of the cannons and the slope
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

	// helper function for mapping array index to y coordinate
	private float conv(int i){
		return i*3.5f/depth-4;
	}

	// helper function for mapping y coordinate to array index (floor of the value)
	private int convInv(float x){
		return (int)((x + 4) * depth / 3.5f);
	}

	// creating a quad mesh
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

	// combining the meshes for the wall and add another one for the bottom of canyon
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
		// add the bottom
		combine[i].mesh = createMesh(new Vector3[]{
			new Vector3(-end, bottom), new Vector3(end, bottom), 
			new Vector3(-end, bottom-2), new Vector3(end, bottom-2)});
		combine[i].transform = meshFilters[0].transform.localToWorldMatrix;
		transform.GetComponent<MeshFilter>().mesh = new Mesh();
		transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine, true);
		transform.gameObject.active = true;
		transform.gameObject.renderer.material = wallMat;
	}

	// convert the list of x coordinates to meshes
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
			}else{
				ms[i] = createMesh(new Vector3[]{new Vector3(end, y1),
					new Vector3(line[i], y1),
					new Vector3(end, y2),
					new Vector3(line[i+1], y2)});
			}
		}
		return ms;
	}

	/**
	 * This function is called by other object to check if the latter has collided a walls
	 * if colliding, return the position where there's a collision
	 * and the normal of the wall at that position
	 * 
	 * This algorithm first checks if the object has penetrated the bottom or top of the canyon.
	 * Then it go gets the section of the canyon wall in leftWall or rightWall array 
	 * that is relevant for where the object is.
	 * This section is a line. So we can project the object's x component onto that line and get the
	 * corresponding x coordinate. Then we compare the x coordinates to check interpenetration.
	 */
	public Vector2[] hasCollide(float posx, float posy, float radius){
		float temp = posy - radius;
		// check if the object is landing on top of the canyon
		if (Mathf.Abs(temp - top) < 0.01 && (posx > right || posx < left)){
			return new Vector2[]{new Vector2(0,top), new Vector2(0,-1)};
		}else if (temp >= top){ // if object is in the air
			return new Vector2[0];
		}
		// get relevant y position on wall
		float y = 0;
		if (temp <= bottom){
			y = bottom;
		}
		// get relevant x position on wall
		int ind = convInv (temp);
		print (ind + " " + temp.ToString());
		float y0 = conv (ind);
		float y1 = conv (ind+1);
		if (posx < 0){ // check for leftwall
			float x0 = leftWall[ind];
			float x1 = leftWall[ind+1];
			float ratio = (posy-y0)/(y1-y0);
			float x = x0 + ratio*(x1-x0);
			if (posx - radius < x){
				if (x1<x0)
				return new Vector2[]{new Vector2(x, y), new Vector2(y0-y1, x0-x1)};
				return new Vector2[]{new Vector2(x, y), new Vector2(y0-y1, x1-x0)};
			}
		}else{ // check for right wall
			float x0 = rightWall[ind];
			float x1 = rightWall[ind+1];
			float ratio = (posy-y0)/(y1-y0);
			float x = x0 + ratio*(x1-x0);
			if (posx + radius > x){
				if (x1<x0)
				return new Vector2[]{new Vector2(x, y), new Vector2(y0-y1, x0-x1)};
				return new Vector2[]{new Vector2(x, y), new Vector2(y0-y1, x1-x0)};
			}
		}
		return new Vector2[]{new Vector2(0,y), new Vector2(0,-1)};
	}
}
