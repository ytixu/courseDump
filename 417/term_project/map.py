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
		tpe, _, t = obs
		z = mapClasify(obs)
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
		if k in trans:
			sum_tran = 0
			for t in trans[k]:
				sum_tran += trans[k][t]
			for t in trans[k]:
				trans[k][t] = trans[k][t]/sum_tran
	return trans

def run(dirName, zoneFile, zn, targetFile, tn):
	if not os.path.exists(dirName):
		os.makedirs(dirName)
	utils.gen_data(dirName+'/'+zoneFile, zn, dirName+'/'+targetFile, tn)
	loadData(dirName+'/'+zoneFile, dirName+'/'+targetFile)
	
	R = getCrossCorrelation()
	# print R
	cov_thr = getCovar(R)
	tran = getTransitionProb(cov_thr)

	for k in tran:
		with open(('%s/%s_%s.csv'%(dirName, k[0], k[1])), 'wb') as csvfile:
			spamwriter = csv.writer(csvfile, delimiter=',',
				 quotechar='|', quoting=csv.QUOTE_MINIMAL)
			spamwriter.writerow(["TIME","PROB"])
			for t in tran[k]:
				spamwriter.writerow([t, tran[k][t]])
	
if __name__ == "__main__":
	for n in [100, 500, 1000, 5000]:
		run('data_%d'%n, 'zone_%d'%n, n, 'target_%d'%n, n)