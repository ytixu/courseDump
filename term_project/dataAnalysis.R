# need to setup working directory to root of this folder

# main result
# I combine all the files in one before running this
data.source<-"term_project/data/result.csv"
links<-read.csv(file=data.source, quote="|")
links$LINK <- as.factor(links$LINK)
links$OBSERVED <- as.factor(links$OBSERVED)
links$NUM <- as.factor(links$NUM)
links$PROB <- as.factor(links$PROB)
levels(links$LINK)
levels(links$LINK) <- c(1:25)


plot(links$LINK, links$NDETECT)
plot(subset(links, PROB=="0.01" & LINK=="5", select = c( NUM, OBSERVED )))

for (p in levels(links$PROB)){
	for (n in levels(links$NUM)){
		jpeg(paste("term_project/image",p,"_",n,".jpeg",sep = "") , 
			width=5, height=6, units="in", res=300)
		plot(subset(links, PROB == p & NUM == n, select = c( LINK, NDETECT )), 
			xlab="link", ylab="number of times detected", ylim=c(0,30),
			main=paste(n, "samples and", p, "probability"))
		dev.off()
	}
}

# set transition time of non-detected links to be 20
# because we don't want to plot it on the graph
for (i in 1:length(links$LINK)){
	if (links$NDETECT[i] == 0){
		links$TRANTIME[i] = 20
	}
}

for (i in list(list("17", "3-4"), list("8", "4-5"))){
	for (p in levels(links$PROB)){
		jpeg(paste("term_project/image",p,"_link",toString(i[2]),
			".jpeg",sep = "") , width=3, height=6, units="in", res=300)
		plot(subset(links, PROB == p & LINK == toString(i[1]), select = c( NUM, TRANTIME )), 
			xlab="sample size", ylab="transition time detected", ylim=c(0,15),
			main=paste(p, "probability,", toString(i[2])))
		dev.off()
	}
}

for (i in list(list("17", "3-4", "2"), list("8", "4-5", "4"))){
	for (p in levels(links$PROB)){
		jpeg(paste("term_project/image",p,"_link",toString(i[2]),
			"transProb.jpeg",sep = "") , width=3, height=6, units="in", res=300)
		plot(subset(links, PROB == p & LINK == toString(i[1]), select = c( TRANTIME, AVGPROB )), 
			xlab="transition time detected", ylab="average probability", ylim=c(0,1),
			xlim=c(0,15), main=paste(p, "probability,", toString(i[2])))
		abline(v=as.numeric(i[3]),lty=3)
		dev.off()
	}
}


	
# accuracy
acc1<-"term_project/result/percentAcc0.010000.csv"
acc2<-"term_project/result/percentAcc0.250000.csv"
acc3<-"term_project/result/percentAcc0.500000.csv"
acc4<-"term_project/result/percentAcc0.750000.csv"
acc5<-"term_project/result/percentAcc0.990000.csv"
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
	jpeg(paste("term_project/image/accuracy",tran[i],".jpeg",sep = ""),
		width=5,height=6, units="in", res=300)
	plot(subset(accP, TRAN == toString(tran[i]), select = c( NUM, ACC )), 
			xlab="sample size", ylab="percentage accuracy", ylim=c(97.5,100),
			main=paste(tran[i], "probability"))
	dev.off()
}

#varying treshold 

res1<-"term_project/data/resultAll0.7500001000M.csv"
res2<-"term_project/data/resultAll0.7500001000.csv"
thr = c(-0.5, 0.5)
reslist = list(res1, res2)
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
THR = c()
ind = 1
for (i in 1:5){
	res<-read.csv(file=toString(reslist[i]), quote="|")
	for (j in 1:length(res$NUM)){
		if (res$NUM[j] == 1000){
			l = 0
			for (k in 1:length(links)){
				if (links[k] == res$LINK[j]){
					l = k
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

for (p in levels(resP$PROB)){
	jpeg(paste("term_project/image/",p,"_1000_solo.jpeg",sep = "") , 
		width=5, height=6, units="in", res=300)
	plot(subset(resP, PROB == p, select = c( LINK, NDETECT )), 
		xlab="link", ylab="number of times detected", ylim=c(0,30),
		main=paste("1000 samples and", p, "probability"))
	dev.off()
}

for (p in levels(resP$PROB)){
	jpeg(paste("term_project/image/",p,"_1000_solo_probs.jpeg",sep = "") , 
		width=5, height=6, units="in", res=300)
	plot(subset(resP, PROB == p, select = c( LINK, TRANTIME )), 
		xlab="link", ylab="transition time detected", ylim=c(0,20),
		main=paste("1000 samples and", p, "probability"))
	dev.off()
}

