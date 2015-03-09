using UnityEngine;
using System.Collections;

public class VerletCannon : MonoBehaviour {
	public Verlet verlet;
	public int fireOffset;

	public ArrayList verlets;

	private GameObject muzzle;

	// Use this for initialization
	void Start () {
		muzzle = GameObject.Find ("VerletCannonMuzzle");
		verlets = new ArrayList ();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Tab)){
			Verlet v = (Verlet) Instantiate(verlet);
			v.initialize(muzzle.transform.position, transform.position);
			verlets.Add(v);
			transform.localEulerAngles = new Vector3(0,0,-30 - Random.value*45);
		}
	}

	// this is to detect and resolve any collision between cannon ball and dogs
	public void verletCollision(Vector3 pos){
		ArrayList temp = new ArrayList ();
		for (int i=0; i<verlets.Count; i++){
			try{
				Verlet ver = (Verlet) verlets[i];
				if (Vector3.Magnitude(ver.transform.position - pos) < 2 &&
				    ver.checkCollision(pos)){
						
						temp.Add(i);
				}
			}
			catch(MissingReferenceException e){
				temp.Add (i);
			}
		}
		if (temp.Count > 0){
			foreach(int index in temp){
				verlets.RemoveAt(index);
			}
		}
	}
}
