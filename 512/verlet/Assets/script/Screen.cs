using UnityEngine;
using System.Collections;

/**
 * This object determines the bound of the screen.
 */

public class Screen{
	private static int sizeX = 9;
	private static int sizeY = 9;

	public static bool isOutOfScene(float posx, float posy){
		if (posx > sizeX || posx < -sizeX || posy > sizeY || posy < -sizeY) 
			return true;
		return false;
	}
}
