using UnityEngine;
using System.Collections;

public class Screen{
	private static int sizeX = 9;
	private static int sizeY = 5;

	public static bool isOutOfScene(float posx, float posy){
		if (posx > sizeX || posx < -sizeX || posy > sizeY || posy < -sizeY) 
			return true;
		return false;
	}
}
