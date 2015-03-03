using UnityEngine;
using System.Collections;

public class BallCannon : MonoBehaviour {

	private GameObject muzzle;

	// Use this for initialization
	void Start () {
		muzzle = GameObject.Find ("BallCannonMuzzle");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
