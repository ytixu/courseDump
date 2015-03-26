using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZombieFactory : MonoBehaviour {
	public Corners cn;
	public int n; // number of zombies
	public Zombie zombie;
	private Zombie[] zombies;
	
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
		//return ZombieBehavior.ZombieType.PHONEADDICT;
	}

	// Use this for initialization
	void Start () {
		// compute the probability density for a uniform initialization of the zombie position 
		// at the start of the game
		zombies = new Zombie[n];
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
					zombies[i] = (Zombie) Instantiate (zombie);
					zombies[i].name = i.ToString();
					Corners.CornerIndex idx = cdf[f];
					zombies[i].transform.parent = transform;
					zombies[i].transform.localEulerAngles = new Vector3(0,90*((idx.j-2)%4),0);
					zombies[i].initialize(randomType(), idx, diff[idx.i,idx.j-1]*Random.value+cn.getCorner(idx));
					break;
				}
			}
		}
	}

	// Update is called once per frame
	void Update () {
		respawn ();
	}

	private void respawn(){
		if (Random.value < p){
			int index = Random.Range (0,n-1);
			zombies[index].active = false;
			Corners.CornerIndex ci = cn.randomCorner();
			zombies[index].transform.localEulerAngles = new Vector3(0,90*((ci.j-2)%4),0);
			zombies[index].initialize(randomType(), ci, cn.getCorner(ci.i, (4+ci.j-1)%4));

		}
	}

	// called by the survivor to check how far away from the zombies a position is
	public float away(Vector3 pos){
		float minDist = 1000;
		foreach (Zombie z in zombies){
			float dist = Vector3.Distance(z.transform.position + 3*z.transform.forward, pos);
			if (dist < minDist){
				minDist = dist;
			}
		}
		return -minDist;
	}

	/**
	 * Called by survivor to check which zombies are visible
	 * Cast a ray to check zombie and see who are visible
	 */
	public List<Zombie> getVisibleZombies(Vector3 pos){
		List<Zombie> lst = new List<Zombie>();
		foreach (Zombie z in zombies){
			float d = Vector3.Distance(pos, z.transform.position);
			RaycastHit hit;
			if (Physics.Raycast (pos, z.transform.position - pos, out hit, d)){
				if (hit.collider.tag == "Zombie"){
					if (d < 10) lst.Add(z); // assume that zombies at distance greater 
											// than 10 will not see survivor
				}
			}
		}
		return lst;
	}

	public void highlightVisibleZombies(Vector3 pos){
		List<Zombie> visibleZombies = getVisibleZombies (pos);
		foreach (Zombie z in zombies){
			if (visibleZombies.Contains (z)){
				z.seen.transform.localScale = new Vector3(1.3f, 0.01f, 1.3f);
			}else{
				z.seen.transform.localScale = Vector3.zero; 
			}
		}
	}
}
