using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SurvivorFactory : MonoBehaviour {

	public GameObject collects;
	public ZombieBehavior zb;
	public Survivor survivor;

	public Survivor currentSurvivor;

	private float timeOut = 20f*3;
	private float timeStart;

	// Use this for initialization
	void Start () {
		respawn ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time - timeStart > timeOut){
			destroySurvivor();
		}
	}

	public void respawn(){
		currentSurvivor = (Survivor)Instantiate (survivor);
		currentSurvivor.transform.position = transform.position;
		currentSurvivor.initialize (collects, zb.speed*1.5f);
		timeStart = Time.time;
	}

	public void destroySurvivor(){
		GameObject.Destroy (currentSurvivor.gameObject);
		respawn ();
	}
}
