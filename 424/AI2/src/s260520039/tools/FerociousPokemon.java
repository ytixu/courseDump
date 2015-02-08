package s260520039.tools;

import halma.CCBoard;
import halma.CCMove;

public abstract class FerociousPokemon extends Pokemon{ //set up for policies/heuristics classes
	protected boolean isEvil = false; //if this is the algorithm for the opponent	
	
	public FerociousPokemon(int id, boolean isEvil){
		super(id);
		this.isEvil = isEvil;
	}
	//to track if we are jumping back and forth
	protected CCMove previous;
	
	//move that does nothing 
	protected CCMove terminate = new CCMove(MonteCarlo.myId, null, null);
		
	
	protected boolean isBack(CCMove m){ 
		try{
			if (this.previous.getFrom().x == m.getTo().x 
					&& this.previous.getFrom().y == m.getTo().y) return true;
		}catch (Exception e){}
		return false;
	}
	
	protected void newPrev(CCMove m){
		this.previous = m;
	}

	protected int moveScore(CCMove m, int turnNum){ //score associated to a move
		if (m.getFrom() == null) return 0;
		int x1, y1, x2, y2;
		x1 = this.rescaleX(m.getFrom().x); 
		y1 = this.rescaleY(m.getFrom().y);
		x2 = this.rescaleX(m.getTo().x);
		y2 = this.rescaleY(m.getTo().y);
		int newP = x2 + y2;
		int oldP = x1 + y1;
		int score = newP - oldP; 
		if (oldP < newP){
			//going out of base
			if (oldP <= 4 && newP > 4) score += turnNum/10;
			//crossing diagonal
			if (oldP <= 15 && newP > 15) score += turnNum/30;
			//getting in goal
			if (oldP <= 27 && newP > 27) score += turnNum/50;
		}else{
			//reverse
			if (newP <= 4 && oldP > 4) score -= turnNum;
			if (newP <= 15 && oldP > 15) score -= turnNum/20;
			if (newP <= 27 && oldP > 27) score -= turnNum/40;
		}
		score -= turnNum/500*Math.abs(x2 - y2); // penalty from getting further from the middle
		return score;
	}
	
	//get move chosen by some policy/heuristic 
	abstract public CCMove attack(CCBoard board) throws Exception;
	
	//get current rating of the best move tracked so far
	abstract public double getScore();
}
