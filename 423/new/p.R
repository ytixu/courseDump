library(MASS)

#WAGE,OCCUPATION,SECTOR,UNION,EDUCATION,EXPERIENCE,AGE,SEX,MARR,RACE,SOUTH
dataset <- read.csv("http://www.math.mcgill.ca/dstephens/Regression/Data/wages.csv")
Y <- dataset$WAGE
X1 <- as.factor(dataset$OCCUPATION)
X2 <- as.factor(dataset$SECTOR)
X3 <- as.factor(dataset$UNION)
X4 <- dataset$EDUCATION
X5 <- dataset$EXPERIENCE
X6 <- dataset$AGE
X7 <- as.factor(dataset$SEX)
X8 <- as.factor(dataset$MARR)
X9 <- as.factor(dataset$RACE)
X10 <- as.factor(dataset$SOUTH)
wages <- data.frame(Y, X1, X2, X3, X4, X5, X6, X7, X8, X9, X10)


fit1 <- lm(Y~X1+X2+X3+X4+X5+X6+X7+X8+X9+X10, data=wages)
png('/home/ytixu/courseDump/423/new/plot%03d.png', width=6, height=6, units='in', res=300)
plot(fit1, ask=FALSE)
dev.off()

round(cor(dataset),3)

jpeg("/home/ytixu/courseDump/423/new/boxcox.jpeg", width=6, height=6, units="in", res=300)
lam.fit<-boxcox(fit1,lambda=seq(-1,1,by=0.0001))
dev.off()

# find that lambda=0
ytilde<-exp(mean(log(dataset$WAGE)))
wages$Ynew<-ytilde*log(dataset$WAGE)

fit2 <- lm(Ynew~-1+X1+X2+X3+X4+X5+X6+X7+X8+X9+X10, data=wages)
png('/home/ytixu/courseDump/423/new/plotTrans%03d.png', width=6, height=6, units='in', res=300)
plot(fit2, ask=FALSE)
dev.off()
drop1(fit2)

fit3 <- lm(Ynew~-1+X1+X2+X3+X4+X6+X7+X8+X9+X10, data=wages)
anova(fit3, fit2)
drop1(fit3, test="F")

fit4 <- lm(Ynew~-1+X1+X2+X3+X4+X6+X7+X8+X10, data=wages)
anova(fit4, fit3)
drop1(fit4, test="F")

fit5 <- lm(Ynew~-1+X1+X2+X3+X4+X6+X7+X10, data=wages)
anova(fit5, fit4)
drop1(fit5, test="F")

fit6 <- lm(Ynew~-1+X1+X3+X4+X6+X7+X10, data=wages)
anova(fit6, fit5)
drop1(fit6, test="F")

fit7 <- lm(Ynew~-1+(X1+X3+X4+X6+X7+X10)*(X1+X2+X3+X4+X5+X6+X7+X8+X9+X10), data=wages)
drop1(fit7, test="F")

fit8 <- lm(Ynew~-1+(X1+X3+X4+X6+X7+X10)*(X3+X4+X5+X9+X10), data=wages)
anova(fit8, fit7)
drop1(fit8, test="F")

fit9 <- lm(Ynew~-1+(X1+X3+X4+X7+X10)*(X3+X4+X9+X10)+X5*X6, data=wages)
anova(fit8, fit9)
drop1(fit9, test="F")

fit10 <- lm(Ynew~-1+(X1+X3+X4+X7+X10)*(X3+X4+X10)+X5*X6, data=wages)
anova(fit10, fit9)
drop1(fit10, test="F")

fit11 <- lm(Ynew~-1+(X1+X7)*(X4+X10)+X4*X10+X5*X6+X1*X3, data=wages)
anova(fit11, fit10)
drop1(fit11, test="F")

summary(fit11)
png('/home/ytixu/courseDump/423/new/plotSimplest%03d.png', width=6, height=6, units='in', res=300)
plot(fit11, ask=FALSE)
dev.off()

dffits(fit1)