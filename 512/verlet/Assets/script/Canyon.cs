using UnityEngine;
using System.Collections;

public class Canyon : MonoBehaviour {
	// for midpoint bisection
	public int depth; // number of iterations
	private float roughness = (float)1f/Mathf.Sqrt(2);

	public int width; // the distance between the two cannons
	public int hight; // the hight of the two cannons
	public int diff; 

	public Material wallMat;

	// Use this for initialization
	void Start () {
		midpointBisection ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void midpointBisection(){
		int l = (int)Mathf.Pow (2, depth);
		float[] line = new float[l];

		randomize (line, 0, l-1, 10);
		// draw
		drawCanyon (line, 10f, l);
	}

	// midpoint bisection a straight line recursively
	// only move the x component
	private void randomize(float[] line, int s, int e, float sd){
		//print(s + " " +  e);
		if (s > e) return;
		int middle = (e + s) / 2;
		//print (middle);
		line [middle] = (line [e] - line [s])/2f + (Random.value - 0.5f) * sd; 
		sd *= roughness;
		randomize (line, s, middle-1, sd);
		randomize (line, middle + 1, e, sd);
	}

	private float conv(int i){
		return i*10;
	}

	private void drawCanyon(float[] line, float end, int length){
		//GL.PushMatrix();
		//wallMat.SetPass(0);
		//GL.LoadOrtho();
		//GL.Begin(GL.QUADS);
		//GL.Color(Color.red);
		//for(int i=0; i<length-1; i++){
		//	float y1 = conv(i);
		//	float y2 = conv(i+1);
		//	GL.Vertex3(line[i], y1, 0);
		//	GL.Vertex3(line[i+1], y2, 0);
		//	GL.Vertex3(end, y2, 0);
		//	GL.Vertex3(end, y1, 0);
		//	print(line[i] +" " + y1 +" " + line[i+1] +" " + y2 +" " +end +" " + y2 +" " + end +" " + y1);
		//}
		//GL.End();
		//GL.PopMatrix();
		for(int i=0; i<length-1; i++){
			Mesh m = new Mesh();
			float y1 = conv(i);
			float y2 = conv(i+1);
			m.vertices = new Vector3[]{new Vector3(line[i], y1),
				new Vector3(line[i+1], y2),
				new Vector3(end, y1),
				new Vector3(end, y2)
			}
		}
	}
}
