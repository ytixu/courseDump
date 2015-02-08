package s260520039;

import halma.CCBoard;
import halma.CCMove;

import java.util.ArrayList;

import s260520039.tools.*;

import boardgame.Board;
import boardgame.Move;
import boardgame.Player;

public class s260520039Player extends Player{
	public s260520039Player() { super("s260520039Player"); }
    public s260520039Player(String s) { super(s); }
    
    private Wobbuffet myWobbuffet = null;
    private Drifloon myDrifloon = null;
    private Spoink mySpoink = null;
    
    private void summonPokemons(int id){ //initialize private variables and Monte Carlo
    	MonteCarlo.setPlayer(id);
    	this.myWobbuffet = new Wobbuffet(id, false);
    	this.myDrifloon = new Drifloon(id);
    	this.mySpoink = new Spoink(id, false);
    }
    
	@Override
	public Move chooseMove(Board theboard) {
		CCBoard board = (CCBoard) theboard;
        CCMove move;
        try{
        	if (this.myDrifloon != null){
        		move = this.myDrifloon.attack(board);
        	}else{ //catch(NullPointerException e){
        		//System.out.println("No pokemon!");
        		summonPokemons(board.getTurn());
        		move = this.myDrifloon.attack(board);
        	}
        }catch (Exception e){
        	MonteCarlo.changeParam(3);
        	move = this.myDrifloon.updateHop(this.mySpoink.attack(board));
        	MonteCarlo.changeParam(10);
        	this.mySpoink.getScore(); //reset the scores
        }
        return move;
	}
}
