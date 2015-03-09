using UnityEngine;
using System.Collections;

/**
 * This is for the cannon that shoots projectiles
 */
public class BallCannon : MonoBehaviour {

	private GameObject muzzle; // where CannonBall will be placed once fired
	public CannonBall ball; 

	// Use this for initialization
	void Start () {
		muzzle = GameObject.Find ("BallCannonMuzzle");
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space)){
			CannonBall b = (CannonBall) Instantiate(ball);
			b.transform.parent = transform;
			b.initialize(Mathf.PI/180*(transform.localEulerAngles.z+90), 0.15f, muzzle.transform.position);
			transform.localEulerAngles = new Vector3(0,0,75-Random.value*45);
		}
	}

}
