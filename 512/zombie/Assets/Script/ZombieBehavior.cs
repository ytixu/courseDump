using UnityEngine;
using System.Collections;

public class ZombieBehavior : MonoBehaviour{

	public float speed; // v
	private float incSpeed; // for zombie with a smart phone, the variation of its speed per time frame
	public enum ZombieType{
		CLASSIC, SHAMBLER, MODERN, PHONEADDICT
	}
	// get the speed of the zombie according to the type
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

	// probability parameters for each type of zombies
	public float[] phoneAddictProb;
	public float shamblerChangeLaneProb;

	void Start(){
		incSpeed = speed/10;
	}

	/**
	 * sub-FSM for the zombies 
	 * according to the type of the zombie, we compute the next position where the zombie goes to
	 * return true if we computed the next position
	 * otherwise return false and we'll compute the position in a later state of the FSM
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
				z.changeNextPos(z.position + z.cn.changeTrack(z.nextCorner, z));
				return true;
			}else{ // change direction
				if (Random.value > phoneAddictProb[3]) break;
				z.changeDirection();
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
