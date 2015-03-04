using UnityEngine;
using System.Collections;

public class Line : MonoBehaviour {

	private LineRenderer alr;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void initialize(Verlet ver){
		// cache line renderer
		alr = gameObject.GetComponent<LineRenderer> ();
		transform.parent = ver.transform;
		alr.SetWidth (0.02f, 0.02f);
	}

	public void setPosition(Vector3 s, Vector3 e){
		//print (s.ToString () + e.ToString ());
		alr.SetPosition (0, s);
		alr.SetPosition (1, e);
	}
}
