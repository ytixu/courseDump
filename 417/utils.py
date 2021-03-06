import random
import json

import numpy as np

TESTPROBS = [0.99, 0.01]
MAX_VAR = 1000

# assume normal speed
def randSpeed():
	return np.random.normal(2,0.05)

# fixed zones 
class zones:
	def __init__(self):
		# zone names
		# camera 2
		self.Daisy = { # exit zone
			'ID': "exit_daisy",
			'cam': 2,
			'coord': (27.23, 5.6),
			'var': ((0.5, 0.0), (0.0, 0.5)),
			'prob': 1.0,
			'next': []
		}
		self.Minni = { # entry zone
			'ID': "entry_minny",	
			'cam': 2,
			'coord': (19.3,5.2),
			'var': ((0.5, 0.0), (0.0, 0.5)),
			'prob': 1.0,
			'next': [self.Daisy]
		}
		# camera 1
		self.Goofy = { # exit zone
			'ID': "exit_goofy",
			'cam': 1,
			'coord': (4.9, 16.2),
			'var': ((0.5, 0.0), (0.0, 0.5)),
			'prob': TESTPROBS[0],
			'next': []
		}
		self.Donald = { # exit zone
			'ID': "exit_donald",
			'cam': 1,
			'coord': (15.8, 5.2),
			'var': ((0.5, 0.0), (0.0, 0.5)),
			'prob': TESTPROBS[1],
			'next': [self.Minni]
		}
		self.Mickey = { # entry zone
			'ID': "entry_micky",
			'cam': 1,
			'coord': (5.0,5.0),
			'var': ((0.5, 0.0), (0.0, 0.5)),
			'prob': 1.0,
			'next': [self.Donald, self.Goofy]
		}
		self.entry = [self.Mickey, self.Minni]
		self.exit = [self.Daisy, self.Goofy, self.Donald]

	def removeLinks(self):
		for z in self.entry:
			del z['next']
		for z in self.exit:
			del z['next']


def get_exit(zone, p): # given previous zone and a random float, generate the exit 
	if p<zone['next'][0]['prob']: return zone['next'][0]
	return zone['next'][1]	

def rand_zone(zone):
	return np.random.multivariate_normal(zone['coord'], zone['var'])

def get_dist(coord1,coord2):
	return np.linalg.norm(coord1 - coord2)/randSpeed()

ZONES = zones()

def rand_time(n):
	return random.random()*n

# generate n targets
# according to the GMM
def genTargets(n):
	targets = []
	# format: 
	# 	[( 
	# 		(entry, time)
	# 		(exit, time)
	# 	), ...]
	start = ZONES.Mickey
	time = rand_time(n)
	prior = rand_zone(start)
	hasExited = True
	while n:
		exit = get_exit(start, random.random())
		post = rand_zone(exit)
		tranTime = get_dist(prior, post)
		if hasExited:
			targets.append(("enter", prior.tolist(), time, start["ID"]))
			targets.append(("exit", post.tolist(), time+tranTime, exit["ID"]))
			hasExited = False
		else:
			hasExited = True
		if exit['next']:
			start = exit
			prior = post
			time += tranTime
		else:
			start = ZONES.Mickey
			prior = rand_zone(start)
			time = rand_time(n)
			hasExited = True
			n -= 1

	return targets

def addToDict(d, k, v, coord):
	try:
		d[k]['p'] += v
		d[k]['n'] += 1.0
		d[k]['m'] += (coord - d[k]['m'])/d[k]['n']
		d[k]['sample'].append(coord)
	except:
		d[k] = {
			'n' : 1.0,
			'p' : v,
			'm' : coord,
			'sample' : [coord],
			'v' : [0, 0]
		}

def getZones(n):
	tot = n
	zs = {}
	curr = ZONES.Mickey
	while n:
		position = rand_zone(curr)
		addToDict(zs, curr['ID'], 1.0/tot, position)	
		if curr['next']:
			curr = get_exit(curr, random.random())
		else:
			curr = ZONES.Mickey
		n -= 1

	var = 0
	for k in zs:
		if zs[k]['n'] > 1:
			for s in zs[k]['sample']:
				zs[k]['v'] += (s-zs[k]['m'])*(s-zs[k]['m'])/(zs[k]['n']-1)
			zs[k]['v'] = [[zs[k]['v'][0], 0],[0, zs[k]['v'][1]]]
		else:
			zs[k]['v'] = [[MAX_VAR, 0],[0, MAX_VAR]]
		del zs[k]['sample']
		zs[k]['m'] = zs[k]['m'].tolist()
	return zs

def gen_data(zn, tn):
	return (getZones(zn), genTargets(tn))

def save_data(zoneFile, zn, targetFile, tn):
	json.dump(getZones(zn), open('%s.json'%zoneFile, 'w'))
	json.dump(genTargets(tn), open('%s.json'%targetFile, 'w'))

def changeProb(testprob):
	global TESTPROBS
	TESTPROBS = testprob
	ZONES.Goofy['prob'] = testprob[0]
	ZONES.Donald['prob'] = testprob[1]
	print ZONES.Goofy['prob']

if __name__ == "__main__":
	json.dump(getZones(100), open('dataZones.json', 'w'))
	json.dump(genTargets(100), open('dataTarget.json', 'w'))


if __name__ == "__main__":
	print getZones(100);