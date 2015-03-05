using UnityEngine;
using System.Collections;

public class CannonBall : MonoBehaviour {
	public Wind wind;

	private float radius = 0.25f;

	// gravity
	private float g = -0.001f;

	// velocity
	private float vx;
	private float vy;

	// position
	private float px;
	private float py;

	private bool fired = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (fired) updatePosition ();
	}

	public void initialize(float angle, float vel, Vector3 pos){
		vy = -vel * (float)Mathf.Sin (angle);
		vx = -vel * (float)Mathf.Cos (angle);
		print (vy + " " + vx);
		transform.position = pos;
		transform.localScale = new Vector3 (radius*2, radius*2, 0);
		px = pos.x;
		py = pos.y;
		fired = true;
	}

	public void updatePosition(){
		vx += wind.getWind ();
		vy += g;
		px += vx;
		py += vy;
		print (py + " " + px);
		if (Screen.isOutOfScene (px, py)) {
			Destroy (gameObject);
			return;
		}
		transform.position = new Vector3 (px, py);
	}

}
