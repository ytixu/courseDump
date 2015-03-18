using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZombieFactory : MonoBehaviour {
	public Corners cn;
	public int n; // number of zombies
	public Zombie zombie;
	
	public float p; // probability of respawing
	public float[] r; // ratios of the types 
	// [classic/shambler, (classic+shamber)/(modern+phoneAddict), modern/phoneAddict]

	private ZombieBehavior.ZombieType randomType(){
		if (Random.value < r[1]){
			if (Random.value < r[0]){
				return ZombieBehavior.ZombieType.CLASSIC;
			}else{
				return ZombieBehavior.ZombieType.SHAMBLER;
			}
		}else{
			if (Random.value < r[2]){
				return ZombieBehavior.ZombieType.MODERN;
			}else{
				return ZombieBehavior.ZombieType.PHONEADDICT;
			}
		}
	}

	// Use this for initialization
	void Start () {
		// compute the probability density for a uniform initialization of the zombie position 
		// at the start of the game
		float sum = 0;
		Vector2[,] diff = new Vector2[3,3];
		for (int i = 0; i<3; i++){
			for (int j=1; j<4; j++){
				diff[i,j-1] = cn.getCorner(i,j-1)-cn.getCorner(i,j);
				sum += Vector2.SqrMagnitude(diff[i,j-1]);
			}
		}
		Dictionary<float, Corners.CornerIndex> cdf = new Dictionary<float, Corners.CornerIndex> ();
		float cumu = 0;
		for (int i=0; i<3; i++){
			for (int j = 1; j<4; j++){
				cumu += Vector2.SqrMagnitude(diff[i,j-1])/sum;
				cdf.Add(cumu, new Corners.CornerIndex(i,j));
			}
		}
		// TODO: check if the last key is 1
		for (int i=0; i<n; i++){
			float rnd = Random.value;
			foreach (float f in cdf.Keys){
				if (rnd < f){
					// create a zombie here
					Zombie z = (Zombie) Instantiate (zombie);
					Corners.CornerIndex idx = cdf[f];
					z.transform.parent = transform;
					z.initialize(randomType(), idx, diff[idx.i,idx.j-1]*Random.value+cn.getCorner(idx));
					break;
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
