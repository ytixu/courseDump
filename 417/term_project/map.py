import json
import math
import collections
import csv
import os

import numpy as np

import utils

# constants
w_const = 0
T = 50

zones = {}
data = {}

def loadData(zoneFile, targetFile):
	global zones, data
	zones = json.load(open('%s.json'%zoneFile, 'r'))
	data = json.load(open('%s.json'%targetFile, 'r'))

# p(x|i) = e^{-0.5(x-mean)^T var^-1 (x-mean)} / sqrt(2pi |var|)
# proportional to -(x-mean)^T var^-1 (x-mean) - log(|var|)
def log_normal_p(mean, var, x):
	m = np.array(x)-np.array(mean)
	v = np.array(var)
	return (np.dot(np.dot(-m,  np.linalg.inv(v)), m[np.newaxis, :].T)
			- math.log(np.linalg.det(v)))

def mapClasify(zone):
	tpe, coord,_ = zone 
	# if tpe == 'enter':
	# 	zones = zone_entry
	# else:
	# 	zones = zone_exit
	max_entry = None
	max_prob = 0
	for k,zone in zones.iteritems():
		p = math.log(zone['p']) + log_normal_p(zone['m'], zone['v'], coord)
		# if k in (u'entry_minny', u'exit_daisy'):
		# 	print k, p
		if p > max_prob or not max_entry:
			max_prob = p
			max_entry = k
	# print coord, zones[max_entry]
	return max_entry

def getCrossCorrelation():
	R = collections.defaultdict(dict)
	enter_targets = []
	exit_targets = []
	for obs in data:
		tpe, coord, t = obs
		z = mapClasify(obs)
		# if z in (u'exit_donald', u'entry_minny'):
		# 	print coord, z
		if tpe == 'enter':
			enter_targets.append((z,t))
		else:
			exit_targets.append((z,t))
	for i, ti in enter_targets:
		for j, tj in exit_targets:
			time = math.floor(tj-ti+0.5)
			if abs(time) > T: continue
			try:
				R[(i,j)][time] += 1.0
			except:
				R[(i,j)][time] = 1.0
	return R

def getCovar(R):
	thrs = {}
	cov = collections.defaultdict(dict)
	for k in R:
		Rlst = np.array([v for _,v in R[k].iteritems()])
		# median
		medR = np.median(Rlst)
		# mean
		mR = np.mean(Rlst)
		# standard deviation
		sR = np.std(Rlst)
		# threshold
		thr = mR + sR*w_const
		for t in R[k]:
			cov[k][t] = R[k][t] - medR
		thrs[k] = thr
	return (cov, thrs)

def checkRepeted(k,t,dic):
	try:
		if dic[k][t]:
			raise RunTimeException("repeated transition")
	except:
		pass

def getTransitionProb(fromCovar):
	cov, thrs = fromCovar
	trans = collections.defaultdict(dict)
	for k in cov:
		thr = thrs[k]
		# if k == (u'exit_donald', u'entry_minny'):
		# 	print thr, cov[k]
		for t in cov[k]:
			if cov[k][t] >= thr:
				# if cov[k][t] < 0: print cov[k][t], t
				p = zones[str(k[0])]['p']
				if t < 0:
					checkRepeted(k[::-1], -t, trans)
					trans[k[::-1]][-t] = cov[k][t]/p/(1-p)
				else:
					checkRepeted(k, t, trans)
					trans[k][t] = cov[k][t]/p/(1-p)
	#normalization
	for k in trans:
		sum_tran = 0
		for t in trans[k]:
			sum_tran += trans[k][t]
		# print k, sum_tran, trans[k]
		for t in trans[k]:
			trans[k][t] = trans[k][t]/sum_tran
		# print trans[k]
	return trans

def getProbs(zn, tn):
	global zones, data
	zones, data = utils.gen_data(zn, tn)
	
	R = getCrossCorrelation()
	# print R
	cov_thr = getCovar(R)
	return (cov_thr[0], getTransitionProb(cov_thr))


### for serialization

def saveToCSV(tran, dirName):
	if not os.path.exists(dirName):
		os.makedirs(dirName)
	for k in tran:
		with open(('%s/%s_%s.csv'%(dirName, k[0], k[1])), 'wb') as csvfile:
			spamwriter = csv.writer(csvfile, delimiter=',',
				 quotechar='|', quoting=csv.QUOTE_MINIMAL)
			spamwriter.writerow(["TIME","PROB"])
			for t in tran[k]:
				spamwriter.writerow([t, tran[k][t]])

def findLink(link, cov, tran):
	inCov, inTran = (False, None)
	if tran[link]:
		inTran = tran[link]
	if cov[link]:
		inCov = True #cov[link]
	return (inCov, inTran)


def saveAllLinks(dic, cov, tran, n):
	for link in cov:
		# print link
		if link not in dic:
			dic[link] = {
				n: {}
			}
		elif n not in dic[link]:
			dic[link][n] = {}
		if link not in tran:
			continue
		# take average tran probability
		for t in tran[link]:
			try:
				dic[link][n][t]["n"] += 1
				dic[link][n][t]["p"] += (dic[link][n][t]["p"]-tran[link][t])/dic[link][n][t]["n"] 
			except:
				dic[link][n][t]={
					'n': 1,
					'p': tran[link][t]
				}

def allLinkToCSV(res, filename, ns):
	with open(filename, 'wb') as csvfile:
		spamwriter = csv.writer(csvfile, delimiter=',',
			 quotechar='|', quoting=csv.QUOTE_MINIMAL)
		spamwriter.writerow(["LINK", "NUM", "OBSERVED", "TRANTIME","AVGPROB", "NDETECT"])
		for link, v in res.iteritems():
			for n in ns:
				if n in v:
					if not v[n]:
						spamwriter.writerow([link, n, True, 0.0, 0.0, 0.0])
					for t in v[n]:
						spamwriter.writerow([link, n, True, t, v[n][t]['p'], v[n][t]['n']])
				else:
					spamwriter.writerow([link, n, False, 0.0, 0.0, 0.0])


def saveOneLink(dic, cov, tran):
	if n not in dic:
		dic[n] = []
	dic[n].append(findLink((u'exit_donald', u'entry_minny'), cov, tran))

def oneLinkToCSV(res, filename):
	with open(filename, 'wb') as csvfile:
		spamwriter = csv.writer(csvfile, delimiter=',',
			 quotechar='|', quoting=csv.QUOTE_MINIMAL)
		spamwriter.writerow(["NUM", "OBSERVED", "TIME","PROB"])
		for n, v in res.iteritems():
			for e in v:
				obs, tran = e
				if not obs or not tran:
					spamwriter.writerow([n, obs, 0.0, 0.0])
				else:
					for t, p in tran.iteritems():
						spamwriter.writerow([n, obs, t, p])
	
if __name__ == "__main__":
	result = {}
	ns = [100, 500, 1000, 2000, 3000, 4000, 5000]
	for n in ns:
	# for n in [1000]:
		print n
		for i in range(300):
		# for i in range(1):
			print "-",i
			cov, tran = getProbs(n,n)
			# saveOneLink(result, cov, tran)
			saveAllLinks(result, cov, tran, n)
			# print tran
			# result[n].append(findLink((u'exit_donald', u'entry_minny'), cov, tran))
			# saveToCSV(tran, "data_%d"%n)
		#getProbs('data_%d'%n, 'zone_%d'%n, n, 'target_%d'%n, n)
	# json.dump(result, open("result.json", 'w'))
	# oneLinkToCSV(result,'result.csv')
	allLinkToCSV(result, 'resultAll.csv', ns)