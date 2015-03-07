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
			b.initialize(transform.localEulerAngles.z, 0.1f, muzzle.transform.position);
		}
	}

}
