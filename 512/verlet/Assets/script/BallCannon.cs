using UnityEngine;
using System.Collections;

public class BallCannon : MonoBehaviour {

	private GameObject muzzle;
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
			b.transform.localPosition = muzzle.transform.localPosition;
			Vector3 temp = muzzle.transform.position - transform.position;
			print(temp);
			b.rigidbody2D.AddForce(new Vector2(temp.x, temp.y)*700);
		}
	}


}
