package s260520039.tools;

import java.awt.Point;
import java.util.ArrayList;

import halma.CCBoard;
import halma.CCMove;

public class Drifloon extends FerociousPokemon{
	// Attack: save best sequence of moves given by Monte Carlo
	
	//staring strategy
	private int[][] startingMoves = {{2, 2, 3, 3},
			{0, 0, 2, 2}, {2, 2, 4, 4}, null,
			{0, 3, 2, 3}, {2, 3, 4, 3}, {4, 3, 4, 5}, null,
			{3, 1, 3, 2}, {3, 2, 3, 4}, {3, 4, 5, 4}, null,
			{0, 1, 2, 3}, {2, 3, 4, 3}, {4, 3, 6, 5}, null};
	
	private class node{ //linked-list
		CCMove value;
		node next = null;
	}
	
	private node moveList; //plan 
	private int listSize = 0;
	private node tail;
	
	private void addMove(int[] m){ //add a new move 
		if (m==null){
			this.tail.value = terminate;
		}else{
			this.tail.value = new CCMove(MonteCarlo.myId, 
				new Point(this.descaleX(m[0]), this.descaleY(m[1])), new Point(this.descaleX(m[2]), this.descaleY(m[3])));
		}
		this.tail.next = new node();
		this.tail = this.tail.next;
		this.listSize++;
	}
	
	private CCMove popList(){ //remove first move
		if (this.listSize == 0) return null;
		CCMove m = this.moveList.value;
		this.listSize--;
		if (this.listSize == 0) this.moveList = null;
		else this.moveList = this.moveList.next;
		return m;
	}
	
	public Drifloon(int id){
		super(id, false); 
		this.moveList = new node();
		this.tail = moveList;
		for (int i=0; i<this.startingMoves.length; i++){
			this.addMove(this.startingMoves[i]);
		}
	}
	
	public void printList(){
		if (this.listSize == 0){
			System.out.println("List size = "+ this.listSize);
		}else{
			System.out.println("List size = "+ this.listSize);
			System.out.println("First ele = "+ this.moveList.value.toPrettyString());
		}
	}
	
	private boolean isHop = false;
	
	private void setUpFeatures(CCBoard board){
		short a,b,c;
		int temp;
		a = b = c = 0;
		ArrayList<Point> pieces = board.getPieces(MonteCarlo.myId);
		for (Point p : pieces){
			temp = p.x + p.y;
			if (temp > 27) c++;
			else if (temp <= 15){ 
				b++;
				if (temp <= 4) a++;
			}
		}
		MonteCarlo.states = new Psyduck(MonteCarlo.myId, a, b, c);
	}
	
	public CCMove attack(CCBoard board) throws Exception { 
		CCMove m = null;
		double score = 0;
		//printList();
		if (this.listSize > 0){
			m = this.popList();
			if (this.listSize == 0){
				board.move(m);
				setUpFeatures(board);
			}
		//}else if (this.isHop){
			//throw new callSpoink();
		}else{ //running monte carlo
			ArrayList<CCMove> moves = board.getLegalMoves();
			double maxScore = 0;
			CCMove temp = null;
			int size = moves.size() /2 + 1;
			for (int i=0; i<size; i++){
				temp = moves.get(rand.nextInt(size));
				if (this.isBack(temp)){
					if (i==size-1 && m == null) return this.terminate;
					else continue;
				}
				score = MonteCarlo.rollOut(temp, (CCBoard)board.clone()) 
						+ this.moveScore(temp, board.getTurnsPlayed());
				//System.out.println(temp.toTransportable() + " " + score + " " + this.moveScore(temp, 0));
				if (score > maxScore || m == null){
					maxScore = score;
					m = temp;
				}
			}
			MonteCarlo.states.move(m);
			if (m.isHop()) this.isHop = true;
		}
		this.newPrev(m);
		//System.out.println("Found " + m.toPrettyString() + " " + score);
		return m;
	}
	
	public CCMove updateHop(CCMove m){
		if (this.isBack(m)){
			this.isHop = false;
			return this.terminate;
		}
		this.newPrev(m);
		if (!m.isHop()) this.isHop = false;
		return m;
	}
	
	public double getScore() {
		return 0;
	}
}
