using UnityEngine;
using System.Collections;

public class Point : MonoBehaviour {
	public int size;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void initialize(Vector2 v, Verlet ver){
		transform.parent = ver.transform;
		transform.localScale = new Vector3 (size, size);
		transform.localPosition = new Vector3 (v.x, v.y);
	}
}
