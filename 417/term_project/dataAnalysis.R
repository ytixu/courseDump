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
"('exit_daisy', 'exit_donald')"
"('exit_donald', 'entry_micky')"
"('exit_donald', 'entry_minny')"
"('exit_donald', 'exit_daisy')" 
"('exit_donald', 'exit_donald')"
"('exit_donald', 'exit_goofy')" 
"('exit_goofy', 'entry_micky')"
"('exit_goofy', 'entry_minny')" 
"('exit_goofy', 'exit_donald')"
"('exit_goofy', 'exit_goofy')"  



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




data.source<-"/home/ytixu/courseDump/417/term_project/resultAll0.010000.csv"
links<-read.csv(file=data.source, quote="|")
links$LINK <- as.factor(links$LINK)
links$OBSERVED <- as.factor(links$OBSERVED)
links$NUM <- as.factor(links$NUM)
levels(links$LINK)
levels(links$LINK) <- c(1:21)



plot(links$LINK, links$NDETECT)
plot(subset(links, LINK=="11", select = c( NUM, NDETECT )))



data.source<-"/home/ytixu/courseDump/417/term_project/percentAcc.csv"
links<-read.csv(file=data.source, quote="|")
links$NUM <- as.factor(links$NUM)
jpeg("/home/ytixu/courseDump/417/term_project/accuracy1000.jpeg" , width=5, height=4, units="in", res=300)
plot(links$NUM, links$PERCENT.ACCURACY, ,pch=20, xlab="sample size", ylab="accuracy")
dev.off()