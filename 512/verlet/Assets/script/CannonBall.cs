using UnityEngine;
using System.Collections;

public class CannonBall : MonoBehaviour {

	// velocity
	private float vx;
	private float vy;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void initialize(float angle, float vel){
		vy = vel * (float)Mathf.Sin (angle);
		vx = vel * (float)Mathf.Cos (angle);
	}
}
