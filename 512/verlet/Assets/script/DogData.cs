using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Storing the data for the position of the points, distance between the points, ...
 */

public class DogData {
	// for the neighbours of a point
	public class DistPointPair{
		public float dist { get; set; }
		public int pointID { get; set; }
		public bool visible { get; set; } // visibility of the edge
	}

	// for the point itself
	public class PosPointPair{
		public ArrayList child { get; set; }
		public Vector2 pos { get; set; }
	}
	
	public static Dictionary<int, PosPointPair> dogGraph = new Dictionary<int, PosPointPair>()
	{
		// starting at the eye of the beast
		{ 0, new PosPointPair { pos = Vector2.zero,
				child = new ArrayList{new DistPointPair {dist=(float)Mathf.Sqrt(2), pointID=1, visible=false},
					new DistPointPair {dist=(float)Mathf.Sqrt(2)*2f, pointID=4, visible=false}}
			}},
		{ 1, new PosPointPair { pos = new Vector2(-1,-1),
				child = new ArrayList{new DistPointPair {dist=3f, pointID=2, visible=true},
					new DistPointPair {dist=3f, pointID=3, visible=true}}}},
		{ 2, new PosPointPair { pos = new Vector2(2,-1),
				child = new ArrayList{new DistPointPair {dist=3f, pointID=4, visible=true}}},
		{ 3, new PosPointPair { pos = new Vector2(-1,2),
				child = new ArrayList{new DistPointPair {dist=3f, pointID=4, visible=true}}}},
		{ 4, new PosPointPair { pos = new Vector2(2,2),
				child = new ArrayList{new DistPointPair {dist=(float)Mathf.Sqrt(2)*2f, pointID=5, visible=true},
					new DistPointPair {dist=(float)Mathf.Sqrt(2)*2f, pointID=0, visible=false},
					new DistPointPair {dist=3, pointID=2, visible=true}}}},
		{ 5, new PosPointPair { pos = new Vector2(4,4),
				child = new ArrayList{new DistPointPair {dist=5, pointID=6, visible=true},
					new DistPointPair {dist=10, pointID=7, visible=true},
					new DistPointPair {dist=(float)Mathf.Sqrt(5*5+8*8), pointID=10, visible=false}}}},
		{ 6, new PosPointPair { pos = new Vector2(4,9),
				child = new ArrayList{new DistPointPair {dist=2, pointID=11, visible=true}}}},
		{ 7, new PosPointPair { pos = new Vector2(14,4),
				child = new ArrayList{new DistPointPair {dist=5, pointID=8, visible=true},
					new DistPointPair {dist=(float)Mathf.Sqrt(5*5+8*8), pointID=11, visible=false}}}},
		{ 8, new PosPointPair { pos = new Vector2(14,9),
				child = new ArrayList{new DistPointPair {dist=2, pointID=10, visible=true}}}},
		{ 9, new PosPointPair { pos = new Vector2(16,3),
				child = new ArrayList{new DistPointPair {dist=(float)Mathf.Sqrt(2)*2f, pointID=7, visible=true}}}},
		{ 10, new PosPointPair { pos = new Vector2(12,9),
				child = new ArrayList{new DistPointPair {dist=6, pointID=11, visible=true},
					new DistPointPair {dist=2, pointID=12, visible=true}}}},
		{ 11, new PosPointPair { pos = new Vector2(6,9),
				child = new ArrayList{new DistPointPair {dist=2, pointID=13, visible=true}}}},
		{ 12, new PosPointPair { pos = new Vector2(12,11),
				child = new ArrayList{new DistPointPair {dist=2, pointID=14, visible=true}}}},
		{ 13, new PosPointPair { pos = new Vector2(6,11),
				child = new ArrayList{new DistPointPair {dist=2, pointID=15, visible=true}}}},
		{ 14, new PosPointPair { pos = new Vector2(12,13),
				child = new ArrayList{new DistPointPair {dist=2, pointID=16, visible=true}}}},
		{ 15, new PosPointPair { pos = new Vector2(6,13),
				child = new ArrayList{new DistPointPair {dist=2, pointID=17, visible=true}}}},
		{ 16, new PosPointPair { pos = new Vector2(10,13),
				child = new ArrayList()}},
		{ 17, new PosPointPair { pos = new Vector2(4,13),
				child = new ArrayList()}}
	};
}
