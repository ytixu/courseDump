import random
import json

import numpy as np

TESTPROBS = [0.99, 0.01]
MAX_VAR = 1000

# assume normal speed
def randSpeed():
	return np.random.normal(2,1)

# fixed zones 
class zones:
	def __init__(self):
		# zone names
		# camera 2
		self.Daisy = { # exit zone
			'ID': 1,
			'cam': 2,
			'coord': (23.23, 5.6),
			'var': ((0.5, 0.0), (0.0, 0.5)),
			'prob': 1.0,
			'next': []
		}
		self.Minni = { # entry zone
			'ID': 2,	
			'cam': 2,
			'coord': (15.9,5.2),
			'var': ((0.5, 0.0), (0.0, 0.5)),
			'prob': 1.0,
			'next': [self.Daisy]
		}
		# camera 1
		self.Goofy = { # exit zone
			'ID': 3,
			'cam': 1,
			'coord': (4.9, 16.2),
			'var': ((0.5, 0.0), (0.0, 0.5)),
			'prob': TESTPROBS[0],
			'next': []
		}
		self.Donald = { # exit zone
			'ID': 4,
			'cam': 1,
			'coord': (15.8, 5.2),
			'var': ((0.5, 0.0), (0.0, 0.5)),
			'prob': TESTPROBS[1],
			'next': [self.Minni]
		}
		self.Mickey = { # entry zone
			'ID': 5,
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

def rand_time():
	return random.random()*5000

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
	prior = rand_zone(start)
	time = rand_time()
	hasExited = True
	while n:
		exit = get_exit(start, random.random())
		post = rand_zone(exit)
		tranTime = get_dist(prior, post)
		if hasExited:
			targets.append(("enter", prior.tolist(), time))
			targets.append(("exit", post.tolist(), time+tranTime))
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
			time = rand_time()
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
	entry_zones = {}
	exit_zones = {}
	start = ZONES.Mickey
	prior = rand_zone(start)
	hasExited = True
	# addToDict(entry_zones, start['ID'], 1.0/tot)		
	while n:
		exit = get_exit(start, random.random())
		post = rand_zone(exit)
		if hasExited:
			addToDict(entry_zones, start['ID'], 1.0/tot, prior)		
			addToDict(exit_zones, exit['ID'], 1.0/tot, post)		
			n -= 1
			hasExited = False
		else:
			hasExited = True
		if exit['next']:
			start = exit
			prior = post
		else:
			start = ZONES.Mickey
			prior = rand_zone(start)
			hasExited = True

	var = 0
	for zs in (entry_zones, exit_zones):
		for k in zs:
			if zs[k]['n'] > 1:
				for s in zs[k]['sample']:
					zs[k]['v'] += (s-zs[k]['m'])*(s-zs[k]['m'])/(zs[k]['n']-1)
				zs[k]['v'] = [[zs[k]['v'][0], 0],[0, zs[k]['v'][1]]]
			else:
				zs[k]['v'] = [[MAX_VAR, 0],[0, MAX_VAR]]
			del zs[k]['sample']
			zs[k]['m'] = zs[k]['m'].tolist()

	return (entry_zones, exit_zones)

json.dump(genTargets(100), open('dataTarget.json', 'w'))
json.dump(getZones(100), open('dataZones.json', 'w'))
# serialize the gmm 
