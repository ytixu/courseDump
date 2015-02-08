package s260520039.tools;

//import java.awt.Point;
//import java.util.ArrayList;

import halma.CCMove;
import halma.CCBoard;

public class MonteCarlo {
	//variables to preform roll outs
	private static int MOVE = 5;
	public static double temperature = 10; 
	public static double gamma = 1;
	public static double rate = 0.2;
	public static double scoreMid = -3;
	//public static double growRate = 0.01;
	
	public static int myId;
	private static FerociousPokemon players[] = new FerociousPokemon[4];
	
	public static Psyduck states;
	
	public static void setPlayer(int id){ //set up
		myId = id;
		players[id] = new Spoink(id, false);
		if (id%2 == 1){
			players[(id+1)%4] = new Wobbuffet((id+1)%4, false);
			players[(id+2)%4] = new Wobbuffet((id+2)%4, true);
			players[(id+3)%4] = new Wobbuffet((id+3)%4, true);
		}else{
			players[(id+1)%4] = new Wobbuffet((id+1)%4, true);
			players[(id+2)%4] = new Wobbuffet((id+2)%4, true);
			players[(id+3)%4] = new Wobbuffet((id+3)%4, false);
		}
		players[id].printCoord();
	}
	
	public static void changeParam(int temp){
		temperature = temp;
	}
	
	private static double othersScore(){
		return (players[(myId+1)%4].getScore() + players[(myId+2)%4].getScore() 
				+ players[(myId+3)%4].getScore())/3.0;
	}
	
	// Perform a roll out with a move
	public static double rollOut(CCMove myMove, CCBoard board) throws Exception{
		int turn;
		double score = 0;
		int roll = 10;
		Psyduck duck;
		CCBoard temp;
		CCMove pickedMove;
		//int win;
		//ArrayList<Point> p = board.getPieces(myId);
		//for (Point i : p){
		//	System.out.println(i.x + " " + i.y);
		//}
		board.move(myMove);
		//System.out.println("move = " + myMove.toPrettyString());
		for (int i=0; i<roll;){ // number of roll out
			temp = (CCBoard)board.clone();
			duck = states.clone(myId);
			players[myId].newPrev(myMove);
			duck.move(myMove);
			//System.out.println("Score = " + score);
			for (int j=0; j<MOVE ; j++){ // length of roll out
				turn = temp.getTurn();
				//System.out.println("player "+ turn);
				do{  //do this until end of turn
					pickedMove = players[turn].attack(temp);
					temp.move(pickedMove);
					if (turn == myId) duck.move(pickedMove);
					//System.out.println("player "+ turn + " end");
					gamma *= rate;
				}while (temp.getTurn() == turn);
			}
			//win = (players[myId].getScore() > othersScore()) ? 1 : 0;
			//System.out.println("before score = " + score);
			score += (players[myId].getScore()// - othersScore() 
					- score + duck.getBonus(temp.getTurnsPlayed())
					)/(++i);
			//System.out.println("score = " + score);
			//score += (duck.getBonus(temp.getTurnsPlayed()))/(i);
			//System.out.println("with = " + score);
			if (score < scoreMid) roll--;
			else if (roll<10 && score > scoreMid) roll++;
			//scoreMid += growRate;
			//System.out.println("temp score =" + score + " " + roll );
			gamma = 1;
		}
		temperature = 10;
		//System.out.println("Score = " + score);
		return score;
	}
}
