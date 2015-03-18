using UnityEngine;
using System.Collections;

/**
 * Static class of all zombie behavior.
 */

public class ZombieBehavior : MonoBehaviour{

	public float speed;
	public enum ZombieType{
		CLASSIC, SHAMBLER, MODERN, PHONEADDICT
	}
	public float getSpeed(ZombieType t){
		switch(t){
		case ZombieType.SHAMBLER:
			return speed/2;
		case ZombieType.MODERN:
			return speed*2;
		default:
			return speed;
		}
	}

	public Material[] ZombieColor;
	public Material getColor(ZombieType t){
		return ZombieColor[(int) t];
	}


	/**
	 * 
	 */
}
