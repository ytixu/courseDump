using UnityEngine;
using System.Collections;

public class VerletCannon : MonoBehaviour {
	public Verlet verlet;
	public int fireOffset;

	private GameObject muzzle;

	// Use this for initialization
	void Start () {
		muzzle = GameObject.Find ("VerletCannonMuzzle");
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Space)){
			Verlet v = (Verlet) Instantiate(verlet);
			v.initialize(muzzle.transform.position, transform.position);
		}
	}
}
