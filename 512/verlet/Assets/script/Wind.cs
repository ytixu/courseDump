using UnityEngine;
using System.Collections;

public class Wind : MonoBehaviour {
	public float range; // defining w such that wind is within [-w, w], and w > 1

	// arrow
	public GameObject bar;
	public GameObject head;

	private float value;
	private int count = 0;

	// Use this for initialization
	void Start () {
		scheduleWind ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public float getWind(){
		return value/1000f;
	}

	// random walk fashion
	private void randomValue(){
		float step = Random.value;
		if (Random.value > 0.5){
			step = -step;
		}
		float temp = value + step;
		if (temp > range || temp < -range){
			step = -step;
		}
		if (value * step > 0) count ++;
		value = temp;
		if (count > 10 && Random.value > 5f/count){
			value = -value;
			count = 0;
		}
	}

	private void drawArrow(){
		float temp = (float)Mathf.Abs(value / range);
		bar.transform.localScale = new Vector3 (temp*2, 0.1f, 0);
		if (value < 0){
			head.transform.rotation = Quaternion.Euler(0,0,180);
			head.transform.localPosition = new Vector3(-temp, 0, 0);
		}else{
			head.transform.rotation = Quaternion.Euler(0,0,0);
			head.transform.localPosition = new Vector3(temp, 0, 0);
		}
	}

	private void scheduleWind(){
		randomValue ();
		drawArrow ();
		//print (value.ToString());
		Invoke ("scheduleWind", 0.5f);
	}
}
