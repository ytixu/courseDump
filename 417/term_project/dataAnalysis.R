<td>1 &#10141; 1</td>
<td>1 &#10141; 4</td>
<td>1 &#10141; 5</td>
<td>1 &#10141; 3</td>
<td>1 &#10141; 2</td>
<td>4 &#10141; 1</td>
<td>4 &#10141; 4</td>
<td>4 &#10141; 5</td>
<td>4 &#10141; 3</td>
<td>4 &#10141; 2</td>
<td>5 &#10141; 1</td>
<td>5 &#10141; 4</td>
<td>5 &#10141; 5</td>
<td>5 &#10141; 3</td>
<td>5 &#10141; 2</td>
<td>3 &#10141; 1</td>
<td>3 &#10141; 4</td>
<td>3 &#10141; 5</td>
<td>3 &#10141; 3</td>
<td>3 &#10141; 2</td>
<td>2 &#10141; 1</td>
<td>2 &#10141; 4</td>
<td>2 &#10141; 5</td>
<td>2 &#10141; 3</td>
<td>2 &#10141; 2</td>


data.source<-"/home/ytixu/courseDump/417/term_project/result300s.csv"
links<-read.csv(file=data.source)
links$OBSERVED <- as.factor(links$OBSERVED)
links$NUM <- as.factor(links$NUM)

plot(links$TIME, links$PROB)
plot( subset( links, PROB>0.0, select = c( TIME, PROB ) ) )
plot( subset( links, NUM=="5000", select = c( TIME, PROB ) ) )

jpeg("/home/ytixu/courseDump/417/term_project/100_1_2" , width=4, height=4, units="in", res=300)
plot(links$PROB~links$TIME, lwd=2, type="l", col="darkred")
dev.off()

jpeg("/home/ytixu/courseDump/417/term_project/exp1-signalVSnum.jpeg" , width=6, height=4.5, units="in", res=400)
plot(links$NUM, links$OBSERVED, xlab="number of observations", ylab="1-3 signal detected")
dev.off()
jpeg("/home/ytixu/courseDump/417/term_project/exp1-probVSnum.jpeg" , width=6, height=4.5, units="in", res=400)
plot(links$NUM, links$PROB, xlab="number of observations", ylab="transition probability found")
dev.off()
jpeg("/home/ytixu/courseDump/417/term_project/exp1-timeVSnum.jpeg" , width=6, height=4.5, units="in", res=400)
plot(links$NUM, links$TIME, xlab="number of observations", ylab="transition time found")
dev.off()


jpeg("/home/ytixu/courseDump/417/term_project/exp5000-probVStime.jpeg" , width=4, height=5, units="in", res=300)
plot( subset( links, NUM=="5000", select = c( TIME, PROB ) ) ,pch=20, main="5000 samples", xlab="transition time", ylab="transition probability")
dev.off()
jpeg("/home/ytixu/courseDump/417/term_project/exp4000-probVStime.jpeg" , width=4, height=5, units="in", res=300)
plot( subset( links, NUM=="4000", select = c( TIME, PROB ) ) ,pch=20, main="4000 samples", xlab="transition time", ylab="transition probability")
dev.off()
jpeg("/home/ytixu/courseDump/417/term_project/exp3000-probVStime.jpeg" , width=4, height=5, units="in", res=300)
plot( subset( links, NUM=="3000", select = c( TIME, PROB ) ) ,pch=20, main="3000 samples", xlab="transition time", ylab="transition probability")
dev.off()
jpeg("/home/ytixu/courseDump/417/term_project/exp2000-probVStime.jpeg" , width=4, height=5, units="in", res=300)
plot( subset( links, NUM=="2000", select = c( TIME, PROB ) ) ,pch=20, main="2000 samples", xlab="transition time", ylab="transition probability")
dev.off()
jpeg("/home/ytixu/courseDump/417/term_project/exp1000-probVStime.jpeg" , width=4, height=5, units="in", res=300)
plot( subset( links, NUM=="1000", select = c( TIME, PROB ) ) ,pch=20, main="1000 samples", xlab="transition time", ylab="transition probability")
dev.off()
jpeg("/home/ytixu/courseDump/417/term_project/exp500-probVStime.jpeg" , width=4, height=5, units="in", res=300)
plot( subset( links, NUM=="500", select = c( TIME, PROB ) ) ,pch=20, main="500 samples", xlab="transition time", ylab="transition probability")
dev.off()
jpeg("/home/ytixu/courseDump/417/term_project/exp100-probVStime.jpeg" , width=4, height=5, units="in", res=300)
plot( subset( links, NUM=="100", select = c( TIME, PROB ) ) ,pch=20, main="100 samples", xlab="transition time", ylab="transition probability")
dev.off()


"('entry_micky', 'entry_micky')"
"('entry_micky', 'entry_minny')"
"('entry_micky', 'exit_daisy')"
"('entry_micky', 'exit_donald')"
"('entry_micky', 'exit_goofy')"
"('entry_minny', 'entry_micky')"
"('entry_minny', 'entry_minny')"
"('entry_minny', 'exit_daisy')" 
"('entry_minny', 'exit_donald')"
"('entry_minny', 'exit_goofy')" 
"('exit_daisy', 'entry_micky')"
"('exit_daisy', 'entry_minny')" 
"('exit_daisy', 'exit_daisy')"
"('exit_daisy', 'exit_donald')" 
"('exit_daisy', 'exit_goofy')"
"('exit_donald', 'entry_micky')"
"('exit_donald', 'entry_minny')"
"('exit_donald', 'exit_daisy')" 
"('exit_donald', 'exit_donald')"
"('exit_donald', 'exit_goofy')" 
"('exit_goofy', 'entry_micky')"
"('exit_goofy', 'entry_minny')" 
"('exit_goofy', 'exit_daisy')"
"('exit_goofy', 'exit_donald')" 
"('exit_goofy', 'exit_goofy')"   

data.source<-"/home/ytixu/courseDump/417/term_project/result.csv"
links<-read.csv(file=data.source, quote="|")
links$LINK <- as.factor(links$LINK)
links$OBSERVED <- as.factor(links$OBSERVED)
links$NUM <- as.factor(links$NUM)
links$PROB <- as.factor(links$PROB)
levels(links$LINK)
levels(links$LINK) <- c(1:25)



plot(links$LINK, links$NDETECT)
plot(subset(links, PROB=="0.01" & LINK=="5", select = c( NUM, OBSERVED )))

# I combine all the files in one before running this
for (p in levels(links$PROB)){
	for (n in levels(links$NUM)){
		jpeg(paste("/home/ytixu/courseDump/417/term_project/",p,"_",n,".jpeg",sep = "") , 
			width=5, height=6, units="in", res=300)
		plot(subset(links, PROB == p & NUM == n, select = c( LINK, NDETECT )), 
			xlab="link", ylab="number of times detected", ylim=c(0,30),
			main=paste(n, "samples and", p, "probability"))
		dev.off()
	}
}

	
# accuracy


acc1<-"/home/ytixu/courseDump/417/term_project/percentAcc0.010000.csv"
acc2<-"/home/ytixu/courseDump/417/term_project/percentAcc0.250000.csv"
acc3<-"/home/ytixu/courseDump/417/term_project/percentAcc0.500000.csv"
acc4<-"/home/ytixu/courseDump/417/term_project/percentAcc0.750000.csv"
acc5<-"/home/ytixu/courseDump/417/term_project/percentAcc0.990000.csv"
acclist = list(acc1, acc2, acc3, acc4, acc5)
tran = c(0.99, 0.75,0.50, 0.25, 0.01)
NUM = c()
ACC = c()
TRAN = c()
ind = 1
for (i in 1:5){
	acc<-read.csv(file=toString(acclist[i]), quote="|")
	for (j in 1:length(acc$NUM)){
		NUM[ind] <- acc$NUM[j]
		ACC[ind] <- acc$PERCENT.ACCURACY[j]
		TRAN[ind] <- tran[i]
		ind = ind+1
	}
}
accP = data.frame(NUM,ACC,TRAN)
accP$NUM <- as.factor(accP$NUM)
accP$TRAN <- as.factor(accP$TRAN)
for (i in 1:5){
	jpeg(paste("/home/ytixu/courseDump/417/term_project/accuracy",tran[i],".jpeg",sep = ""),
		width=6,height=6, units="in", res=300)
	plot(subset(accP, TRAN == toString(tran[i]), select = c( NUM, ACC )), 
			xlab="sample size", ylab="percentage accuracy",
			main=paste(tran[i], "probability"))
	dev.off()
}




res1<-"/home/ytixu/courseDump/417/term_project/resultAll0.0100001000.csv"
res2<-"/home/ytixu/courseDump/417/term_project/resultAll0.2500001000.csv"
res3<-"/home/ytixu/courseDump/417/term_project/resultAll0.5000001000.csv"
res4<-"/home/ytixu/courseDump/417/term_project/resultAll0.7500001000.csv"
res5<-"/home/ytixu/courseDump/417/term_project/resultAll0.9900001000.csv"

reslist = list(res1, res2, res3, res4, res5)
tran = c(0.01, 0.25, 0.50, 0.75, 0.99)
links = c("('entry_micky', 'entry_micky')",
		"('entry_micky', 'entry_minny')",
		"('entry_micky', 'exit_daisy')",
		"('entry_micky', 'exit_donald')",
		"('entry_micky', 'exit_goofy')",
		"('entry_minny', 'entry_micky')",
		"('entry_minny', 'entry_minny')",
		"('entry_minny', 'exit_daisy')", 
		"('entry_minny', 'exit_donald')",
		"('entry_minny', 'exit_goofy')", 
		"('exit_daisy', 'entry_micky')",
		"('exit_daisy', 'entry_minny')", 
		"('exit_daisy', 'exit_daisy')",
		"('exit_daisy', 'exit_donald')", 
		"('exit_daisy', 'exit_goofy')",
		"('exit_donald', 'entry_micky')",
		"('exit_donald', 'entry_minny')",
		"('exit_donald', 'exit_daisy')", 
		"('exit_donald', 'exit_donald')",
		"('exit_donald', 'exit_goofy')", 
		"('exit_goofy', 'entry_micky')",
		"('exit_goofy', 'entry_minny')", 
		"('exit_goofy', 'exit_daisy')",
		"('exit_goofy', 'exit_donald')", 
		"('exit_goofy', 'exit_goofy')"  )
LINK = c()
OBSERVED = c()
TRANTIME = c()
AVGPROB = c()
NDETECT = c()
PROB = c()
ind = 1
for (i in 1:5){
	res<-read.csv(file=toString(reslist[i]), quote="|")
	for (j in 1:length(res$NUM)){
		if (res$NUM[j] == 1000){
			l = 0
			for (k in 1:length(links)){
				if (links[k] == res$LINK[j]){
					l = k
					print(paste(links[k],res$LINK[j], "~~~"))
				}
			}
			if (l == 0){
				print(l)
			}
			LINK[ind] = l
			OBSERVED[ind] = res$OBSERVED[j]
			TRANTIME[ind] = res$TRANTIME[j]
			AVGPROB[ind] = res$AVGPROB[j]
			NDETECT[ind] = res$NDETECT[j]
			PROB[ind] = tran[i]
			ind = ind + 1
		}
	}
}
resP = data.frame(LINK, OBSERVED, TRANTIME, AVGPROB, NDETECT, PROB)
resP$PROB <- as.factor(resP$PROB)
resP$LINK <- as.factor(resP$LINK)
resP$OBSERVED <- as.factor(resP$OBSERVED)
levels(resP$LINK)<-c(1:20)

for (p in levels(resP$PROB)){
	jpeg(paste("/home/ytixu/courseDump/417/term_project/",p,"_1000_solo.jpeg",sep = "") , 
		width=5, height=6, units="in", res=300)
	plot(subset(resP, PROB == p, select = c( LINK, NDETECT )), 
		xlab="link", ylab="number of times detected", ylim=c(0,30),
		main=paste(n, "samples and", p, "probability"))
	dev.off()
}