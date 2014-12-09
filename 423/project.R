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
	jpeg(paste("C:\\cygwin64\\home\\Yi Tian\\courseDump\\423\\", i, ".jpg", sep=""), width=6, height=6, units="in", res=300)
	plot(dataset$WAGE~dataset[[i]], xlab=names(dataset)[i],ylab="WAGE")
	dev.off()
}

fit1<-lm(WAGE~OCCUPATION+SECTOR+UNION+EDUCATION+EXPERIENCE+AGE+SEX+MARR+RACE+SOUTH,data=dataset)
summary(fit1)

# drop AGE
fit2<-lm(WAGE~OCCUPATION+SECTOR+UNION+EDUCATION+EXPERIENCE+SEX+MARR+RACE+SOUTH,data=dataset)
summary(fit2)
anova(fit2, fit1)

#drop RACE
fit3<-lm(WAGE~OCCUPATION+SECTOR+UNION+EDUCATION+EXPERIENCE+SEX+MARR+SOUTH,data=dataset)
summary(fit3)
anova(fit3, fit2)

#drop MARR
fit4<-lm(WAGE~OCCUPATION+SECTOR+UNION+EDUCATION+EXPERIENCE+SEX+SOUTH,data=dataset)
summary(fit4)
anova(fit4, fit3)

#drop SOUTH
fit5<-lm(WAGE~OCCUPATION+SECTOR+UNION+EDUCATION+EXPERIENCE+SEX,data=dataset)
summary(fit5)
anova(fit5, fit4)

#drop SECTOR
fit6<-lm(WAGE~OCCUPATION+UNION+EDUCATION+EXPERIENCE+SEX,data=dataset)
summary(fit6)
anova(fit6, fit5)

# collinearity 
summary(lm(EXPERIENCE~UNION,data=dataset))
plot(dataset$EXPERIENCE~dataset$UNION)

summary(lm(EXPERIENCE~EDUCATION,data=dataset))
plot(dataset$EXPERIENCE~dataset$EDUCATION)

summary(lm(EXPERIENCE~SEX,data=dataset))
plot(dataset$EXPERIENCE~dataset$SEX)

#interaction
fit7<-lm(WAGE~UNION+OCCUPATION+EDUCATION+EXPERIENCE+SEX,data=dataset)
summary(fit7)
anova(fit7, fit6)
