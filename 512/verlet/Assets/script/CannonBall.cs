using UnityEngine;
using System.Collections;

public class CannonBall : MonoBehaviour {
	public Wind wind;
	public Canyon canyon;

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
		float tempx = px;
		float tempy = py;
		vx += wind.getWind ();
		vy += g;
		px += vx;
		py += vy;
		//print (py + " " + px);
		if (Screen.isOutOfScene (px, py)) {
			Destroy (gameObject);
			return;
		}
		Vector2 col = canyon.hasCollide (px, py, radius);
		if (Mathf.Abs (col.x) > 0){
			if (px < 0){
				px = col.x + radius;
				// bouncing
				vx = -vx;
			}else{
				px = col.x - radius;
			}
		}
		if (Mathf.Abs (col.y) > 0){
			py = col.y + radius;
			// if not moving anymore 
			if (Mathf.Abs(tempx - px) < 0.01 && Mathf.Abs(tempy - py) < 0.01){
				fired = false;
				Invoke("destroy", 0.5f);
			}
		}
		//print (col.ToString () +  px);
		transform.position = new Vector3 (px, py);
	}

	private void destroy(){
		Destroy (gameObject);
	}
}
