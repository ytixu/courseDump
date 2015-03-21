using UnityEngine;
using System.Collections;

/**
 * Static class of all zombie behavior.
 */

public class ZombieBehavior : MonoBehaviour{

	public float speed;
	private float incSpeed;
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

	public float[] phoneAddictProb;
	public float shamblerChangeLaneProb;

	void Start(){
		incSpeed = speed/10;
	}

	/**
	 * FSM for the zombies 
	 */
	public bool behave(Zombie z){
		switch(z.getType()){
		case ZombieType.PHONEADDICT:
			float behavior = Random.value;
			if (behavior < 0.25f || !z.moving){ // stop or resume
				if (Random.value < phoneAddictProb[0]){
					z.moving = !z.moving;
				}
				break;
			}else if (behavior < 0.5f){ // change speed
				if (Random.value > phoneAddictProb[1]) break;
				if (z.speed + incSpeed > speed*2 || (Random.value < 0.5 && z.speed-incSpeed > speed/2)){
					z.speed -= incSpeed;
				}else{
					z.speed += incSpeed;
				}
				break;
			}else if (behavior < 0.75f){ // change lane
				if (Random.value > phoneAddictProb[2]) break;
				//print ("lane " + z.name + " " + z.nextCorner.i + " " + z.nextCorner.j + " " + z.direction);
				z.changeNextPos(z.position + z.cn.changeTrack(z.nextCorner, z));
				//print (z.nextCorner.i + " " + z.nextCorner.j);
				return true;
			}else{ // change direction
				if (Random.value > phoneAddictProb[3]) break;
				//print ("cange " + z.name + " " + z.nextCorner.i + " " + z.nextCorner.j + " " + z.direction);
				z.changeDirection();
				//print (z.nextCorner.i + " " + z.nextCorner.j + " " + z.direction);
			}
			break;
		case ZombieType.MODERN:
			if (z.zombieAhead(1)){ // change lane
				z.changeNextPos(z.position + z.cn.changeTrack(z.nextCorner, z));
				return true;
			}
			break;
		case ZombieType.SHAMBLER:
			if (Random.value < shamblerChangeLaneProb){ // change lane
				z.changeNextPos(z.position + z.cn.changeTrack(z.nextCorner, z));
				return true;
			}
			break;
		default:
			break;
		}
		return false;
	}
}
