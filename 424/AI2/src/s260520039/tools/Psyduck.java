package s260520039.tools;

import halma.CCMove;


public class Psyduck extends Pokemon{ 	//tracking the number of pieces 
	public short inBase; 		//in base,
	public short frontDiag; 	//in from of the diagonal,
	public short inGoal; 		//in the goal
	
	public Psyduck(int id, short b, short d, short g){
		super(id);
		this.inBase = b;
		this.frontDiag = d;
		this.inGoal = g;
	}
	
	public Psyduck clone(int id){
		return new Psyduck(id, this.inBase, this.frontDiag, this.inGoal);
	}
	
	//set function
	public void incInBase(int n){ this.inBase += n; }
	public void inFrontDiag(int n){ this.frontDiag += n; }
	public void incInGoal(int n){ this.inGoal += n; }
	
	//get functions
	public int inBase(){ return this.inBase; }
	public int frontDiag(){ return this.frontDiag; }
	public int inGoal(){ return this.inGoal; }
	public void setFeatures(short b, short d, short g){ 
		this.inBase = b;
		this.frontDiag = d;
		this.inGoal = g;
	}
	
	public void move(CCMove m){
		if (m.getTo() == null) return;
		int x1, y1, x2, y2;
		x1 = this.rescaleX(m.getFrom().x); 
		y1 = this.rescaleY(m.getFrom().y);
		x2 = this.rescaleX(m.getTo().x);
		y2 = this.rescaleY(m.getTo().y);
		int newP = x2 + y2;
		int oldP = x1 + y1;
		if (oldP < newP){
			if (oldP <= 4 && newP > 4) incInBase(-1);
			if (oldP <= 15 && newP > 15) inFrontDiag(-1);
			if (oldP <= 27 && newP > 27) incInGoal(1);
		}else{
			if (newP <= 4 && oldP > 4) incInBase(1);
			if (newP <= 15 && oldP > 15) inFrontDiag(1);
			if (newP <= 27 && oldP > 27) incInGoal(-1);
		}
		//System.out.println("Move = " + m.toTransportable() + " " + x1 + " " + y1 + " = "  
					//+ oldP + "; " + x2 + " " + y2 + " =" + newP);
	}
	
	public double getBonus(int turn){
		//double temp = (inGoal() - inBase()/10.0*turn - frontDiag()/30.0*turn)/0.8;
		//System.out.println("duck = " + temp);
		double score = 0.0;
		if (turn > 20){
			score -= Math.log(inBase()*(2.0-10.0/turn));
			if (turn > 30){
				score -= Math.log(frontDiag()*(1.0 - 30.0/turn));
				if (turn > 60){
					score += Math.log((13 - frontDiag())*(1.0 - 60.0/turn));
					if (turn > 100){
						score -= Math.log((13 - inGoal())*(1.0 - 50.0/turn));
					}
				}
				
			}
		}
		return (score)*(2-100.0/turn);
	}
}
