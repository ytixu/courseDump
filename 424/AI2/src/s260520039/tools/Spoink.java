package s260520039.tools;

import halma.CCMove;
import halma.CCBoard;

import java.util.ArrayList;

public class Spoink extends FerociousPokemon{
	// Attack: 'pseudo-'simulate annealing
	
	protected class BestMove{ //tacking the move chosen by a policy/heuristic
		public CCMove bestMove;
		public double maxScore;
	}
	protected BestMove tracker;
	
	protected void resetTraker(){ tracker.bestMove = null; }
	
	public Spoink(int id, boolean i){
		super(id, i); 
		this.tracker = new BestMove();
	}
	
	private void trackBestMove(CCMove m, double score, float prob){ //update tracker
		float r = rand.nextFloat();
		//System.out.println("record = " + r + " " + tracker.maxScore);
		if (tracker.bestMove == null || tracker.maxScore < score 
			|| r < prob){ //if m is not good, still take it with some probability
			//System.out.println("Update best " + m.toPrettyString() + score + " " + prob);
			tracker.bestMove = m; 
			tracker.maxScore = score;
		}else{
			//System.out.println("Not tracked " + m.toPrettyString() + score + " " + prob);
		}
	}
	
	private double moveScoreSpoink(CCMove m, int turnNum){ //score associated to a move
		return (double)this.moveScore(m, turnNum) * MonteCarlo.gamma; // least move points are better
	}
	
	
	private double accScore = 0; //accumulated score for one roll out (in Monte Carlo)
	
	public CCMove attack(CCBoard board) { //pseudo-simulated annealing 
										//for choosing a (sequence of) move(s)
		int turnNum = board.getTurnsPlayed();
		//CCMove temp;
		double score, size;
		ArrayList<CCMove> moves = board.getLegalMoves();
		size = moves.size(); // /4 + 2;
		for(CCMove temp : moves){	//randomly pick and check if a move is good
			//System.out.print("a " + size);
			//temp = moves.get(rand.nextInt(moves.size()));
			if (temp.getTo() == null) score = 0;
			else if (this.isBack(temp)) continue;
			else score = moveScoreSpoink(temp, turnNum);
			trackBestMove(temp, score, 
					(float)Math.exp((score-this.tracker.maxScore)/MonteCarlo.temperature));
			MonteCarlo.temperature *= MonteCarlo.rate;
		}
		this.accScore += this.tracker.maxScore; //update score
		CCMove temp = this.tracker.bestMove;
		if (temp == null) temp = this.terminate;
		this.newPrev(temp);
		//System.out.println("acc score =  " + this.accScore + temp.toTransportable());
		this.resetTraker();
		//System.out.println(temp.toPrettyString());
		return temp;
	}

	public double getScore() {
		double temp = this.accScore;
		this.accScore = 0.0;
		if (this.isEvil){
			//System.out.println("evil");
			return -temp;
		}
		return temp;
	}
}
