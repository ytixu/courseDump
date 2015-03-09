using UnityEngine;
using System.Collections;

/**
 * The ball projectile
 */
public class CannonBall : MonoBehaviour {
	public Wind wind;
	public Canyon canyon;
	public VerletCannon verCannon;

	private float radius = 0.25f;
	private float elasticity = 0.8f;

	// gravity
	private float g = -0.002f;

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

	// this is called when fired from the cannon
	// we set up the initial velocity and position
	public void initialize(float angle, float vel, Vector3 pos){
		vy = vel * (float)Mathf.Sin (angle);
		vx = vel * (float)Mathf.Cos (angle);
		transform.position = pos;
		transform.localScale = new Vector3 (radius*2, radius*2, 0);
		px = pos.x;
		py = pos.y;
		fired = true;
	}

	// this is called every frame when it's fired
	public void updatePosition(){
		float tempx = px;
		float tempy = py;
		// computer new velocity and position
		px += vx;
		py += vy;

		// check if it has gone out of the scene
		if (Screen.isOutOfScene (px, py)) {
			Destroy (gameObject);
			return;
		}
		// collision detection
		Vector2[] col = canyon.hasCollide (px, py, radius);
		// collision handeler 
		if (col.Length > 1){
			Vector2 colPos = col[0];
			Vector2 normal = col[1];
			normal.Normalize();
			print (normal.ToString());
			if (Mathf.Abs (colPos.x) > 0){
				if (px < 0){
					px = colPos.x + radius;
				}else{
					px = colPos.x - radius;
				}
				// bouncing
				vx = vx*normal.x*elasticity;
				vy = vy*normal.y*elasticity;
			}
			if (Mathf.Abs (colPos.y) > 0){
				py = colPos.y + radius;
				// if not moving anymore 
				if (Mathf.Abs(tempx - px) < 0.01 && Mathf.Abs(tempy - py) < 0.01){
					fired = false;
					Invoke("destroy", 0.5f);
				}
			}
		}

		vx += wind.getWind ();
		vy += g;
		transform.position = new Vector3 (px, py);
		// collision with dogs
		verCannon.verletCollision (transform.position);
	}

	private void destroy(){
		Destroy (gameObject);
	}
}
