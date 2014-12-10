#WAGE,OCCUPATION,SECTOR,UNION,EDUCATION,EXPERIENCE,AGE,SEX,MARR,RACE,SOUTH
dataset <- read.csv("http://www.math.mcgill.ca/dstephens/Regression/Data/wages.csv")
dataset$OCCUPATION <- as.factor(dataset$OCCUPATION)
dataset$SECTOR <- as.factor(dataset$SECTOR)
dataset$UNION <- as.factor(dataset$UNION)
dataset$SEX <- as.factor(dataset$SEX)
dataset$MARR <- as.factor(dataset$MARR)
dataset$RACE <- as.factor(dataset$RACE)
dataset$SOUTH <- as.factor(dataset$SOUTH)

#plots 
for (i in 3:12){
	jpeg(paste("C:\\cygwin64\\home\\Yi Tian\\courseDump\\423\\WAGE_", names(dataset)[i], ".jpg", sep=""), width=6, height=6, units="in", res=300)
	plot(dataset$WAGE~dataset[[i]], xlab=names(dataset)[i],ylab="WAGE")
	dev.off()
}

fit1<-lm(WAGE~OCCUPATION+SECTOR+UNION+EDUCATION+EXPERIENCE+AGE+SEX+MARR+RACE+SOUTH,data=dataset)
summary(fit1)

drop1(fit1,test="F")

fit2<-lm(WAGE~OCCUPATION+UNION+SEX,data=dataset)
summary(fit2)

anova(fit2,fit1)

#interactions
for (i in list(3,5,9)){
	for (j in 3:12){
		if (i!=j){
			jpeg(paste("C:\\cygwin64\\home\\Yi Tian\\courseDump\\423\\",names(dataset)[j], "_", 
				xlab=names(dataset)[i], ".jpg", sep=""), width=6, height=6, units="in", res=300)
			plot(dataset[[j]]~dataset[[i]], xlab=names(dataset)[i],ylab=names(dataset)[j])
			dev.off()
		}
	}
}

fit3<-lm(WAGE~OCCUPATION*UNION*SEX,data=dataset)
summary(fit3)
drop1(fit3,test="F")

fit4<-lm(WAGE~OCCUPATION+UNION+SEX+OCCUPATION:UNION+UNION:SEX+UNION:OCCUPATION,data=dataset)
summary(fit4)
drop1(fit4,test="F")

fit5<-lm(WAGE~OCCUPATION*UNION+SEX,data=dataset)
summary(fit5)
drop1(fit5,test="F")


fit6<-lm(WAGE~(OCCUPATION*UNION+SEX)*(SECTOR+EDUCATION+EXPERIENCE+AGE+MARR+RACE+SOUTH),data=dataset)
summary(fit6)
drop1(fit6,test="F")

fit7<-lm(WAGE~(OCCUPATION*UNION+SEX)*MARR,data=dataset)
summary(fit7)
drop1(fit7,test="F")

anova(fit5, fit7)

fit8<-lm(WAGE~OCCUPATION*UNION+SEX*MARR,data=dataset)
summary(fit8)
drop1(fit8,test="F")

anova(fit5, fit8)

#residual 
res <- resid(fit8)
predict <- predict(fit8) 
jpeg("C:\\cygwin64\\home\\Yi Tian\\courseDump\\423\\Residual_predict.jpg", width=6, height=6, units="in", res=300)
plot(predict, res, xlab="predicted outcome",ylab="residual", main="Residual vs predicted outcome")
abline(h=0, lty="dotted")
dev.off()

for (x in list(3,5,9,10)){
	jpeg(paste("C:\\cygwin64\\home\\Yi Tian\\courseDump\\423\\Residual_",names(dataset)[x],".jpg") , width=6, height=6, units="in", res=300)
	plot(dataset[[x]], res, xlab=names(dataset)[x],ylab="residual", main=paste("Residual vs",names(dataset)[x]))
	abline(h=0, lty="dotted")
	dev.off()
}


# transformation
library(MASS)

lam.fit<-boxcox(fit8,lambda=seq(-1,1,by=0.0001))
lambda.hat<-lam.fit$x[which.max(lam.fit$y)]

ytilde<-exp(mean(log(dataset$WAGE)))
dataset$ynew<-ytilde*log(dataset$WAGE)
fit9<-lm(ynew~OCCUPATION*UNION+SEX*MARR,data=dataset)
summary(fit9)
drop1(fit9,test="F")

res <- resid(fit9)
predict <- predict(fit9) 
jpeg("C:\\cygwin64\\home\\Yi Tian\\courseDump\\423\\Residual_predict_afterTrans.jpg", width=6, height=6, units="in", res=300)
plot(predict, res, xlab="predicted outcome",ylab="residual", main="Residual vs predicted outcome")
abline(h=0, lty="dotted")
dev.off()

for (x in list(3,5,9,10)){
	jpeg(paste("C:\\cygwin64\\home\\Yi Tian\\courseDump\\423\\Residual_",names(dataset)[x],"afterTran",".jpg") , width=6, height=6, units="in", res=300)
	plot(dataset[[x]], res, xlab=names(dataset)[x],ylab="residual", main=paste("Residual vs",names(dataset)[x]))
	abline(h=0, lty="dotted")
	dev.off()
}