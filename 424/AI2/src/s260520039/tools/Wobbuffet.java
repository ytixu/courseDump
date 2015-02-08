package s260520039.tools;

import halma.CCMove;
import halma.CCBoard;

import java.util.ArrayList;

public class Wobbuffet extends FerociousPokemon{ 
	// Attack: pure randomness
	
	public Wobbuffet(int id, boolean i) { super(id, i); }

	//public void initScore(ArrayList<Point> pieces) { return; }

	public CCMove attack(CCBoard board) {
		ArrayList<CCMove> moves = board.getLegalMoves();	
		//System.out.println("mosing");
		return moves.get(rand.nextInt(moves.size()));
	}
	
	public double getScore() { return 0; }

}
