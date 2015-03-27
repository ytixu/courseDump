using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * This class regulate the appearance and disappearance of a survivor
 */

public class SurvivorFactory : MonoBehaviour {

	public GameObject collects;
	public ZombieBehavior zb;
	public Survivor survivor;

	public Survivor currentSurvivor;

	// the time for a survivor to complete the task without zombies * 3
	// this is tested when the speed v = 0.09
	private float timeOut = 20f*3;
	private float timeStart;

	void Start () {
		respawn ();
	}
	
	void Update () {
		// if the survivor take too much time
		if (Time.time - timeStart > timeOut){
			destroySurvivor();
		}
	}

	// respawn surivivor at the start state
	// note that this survivorFactory gameObject's position is at the start state
	public void respawn(){
		currentSurvivor = (Survivor)Instantiate (survivor);
		currentSurvivor.transform.position = transform.position;
		currentSurvivor.initialize (collects, zb.speed*1.5f);
		timeStart = Time.time;
	}

	// this is destroys the current survivor and recreate another one
	public void destroySurvivor(){
		GameObject.Destroy (currentSurvivor.gameObject);
		respawn ();
	}
}
