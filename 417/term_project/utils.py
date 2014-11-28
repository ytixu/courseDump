import random
import json

import numpy as np

TESTPROBS = [0.99, 0.01]

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

def rand_zone(zone0, zone1):
	# calculate distance 
	entry = np.random.multivariate_normal(zone0['coord'], zone0['var'])
	exit = np.random.multivariate_normal(zone1['coord'], zone0['var'])
	d = np.linalg.norm(entry - exit)
	return (entry, d/randSpeed(), exit)

ZONES = zones()

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
	time = random.random()*n
	while n:
		exit = get_exit(start, random.random())
		prior, tranTime, post = rand_zone(start, exit)
		targets.append((
				(prior.tolist(), time),
				(post.tolist(), time+tranTime) 
		))
		if exit['next']:
			start = exit
			time += tranTime
		else:
			start = ZONES.Mickey
			time = random.random()*n
			n -= 1

	return targets

def addToDict(d, k, v):
	try:
		d[k] += v
	except:
		d[k] = v	

def getZones(n):
	tot = n
	entry_zones = {}
	exit_zones = {}
	start = ZONES.Mickey
	hasExited = True
	# addToDict(entry_zones, start['ID'], 1.0/tot)		
	while n:
		exit = get_exit(start, random.random())
		if hasExited:
			addToDict(entry_zones, start['ID'], 1.0/tot)		
			addToDict(exit_zones, exit['ID'], 1.0/tot)		
			n -= 1
			hasExited = False
		else:
			hasExited = True
		if exit['next']:
			start = exit
		else:
			start = ZONES.Mickey
	return (entry_zones, exit_zones)

json.dump(genTargets(100), open('dataTarget.json', 'w'))
json.dump(getZones(100), open('dataZones.json', 'w'))
# serialize the gmm 
