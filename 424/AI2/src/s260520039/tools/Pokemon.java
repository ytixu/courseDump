package s260520039.tools;

import java.util.Random;

abstract public class Pokemon { //provide same scaling for all player

	protected int rescaleX = 0;
	protected int rescaleY = 0;

	Random rand = new Random();
	
	public Pokemon(int id){ 
		switch (id){ //set up rescaling factors
		case 2: this.rescaleX = 0;
				this.rescaleY = -15;
				break;
		case 1: this.rescaleY = 0;
				this.rescaleX = -15;
				break;
		case 3: this.rescaleY = -15;
				this.rescaleX = -15;
		}
	}
	
	protected int rescaleX(int x){ return Math.abs(x + this.rescaleX); }
	protected int rescaleY(int y){ return Math.abs(y + this.rescaleY); }
	protected int descaleX(int x){ return Math.abs(- this.rescaleX - x); }
	protected int descaleY(int y){ return Math.abs(- this.rescaleY - y); }
	
	public void printCoord(){
		System.out.println("x = " + rescaleX + " y = " + rescaleY);
	}
	
}
