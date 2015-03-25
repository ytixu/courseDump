using UnityEngine;
using System.Collections;

public class SurvivorFactory : MonoBehaviour {

	public GameObject collects;
	public ZombieBehavior zb;
	public Survivor survivor;

	// Use this for initialization
	void Start () {
		respawn ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void respawn(){
		Survivor s = (Survivor)Instantiate (survivor);
		s.transform.position = transform.position;
		print (zb.speed);
		s.initialize (collects, zb.speed*1.5f);
	}
}
