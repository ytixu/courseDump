import random
import json

import numpy as np

TESTPROBS = [0.8, 0.2]

# assume normal speed
def randSpeed():
	return np.random.normal(2,1)

# fixed zones 
class zones:
	def __init__(self):
		# zone names
		# camera 2
		self.Daisy = { # exi zone
			'cam': 2,
			'coord': (1.1, 18.3),
			'var': ((1.0, 0.0), (0.0, 1.0)),
			'prob': 1.0,
			'next': []
		}
		self.Minni = { # entry zone
			'cam': 2,
			'coord': (1.2,10.5),
			'var': ((1.0, 0.0), (0.0, 1.0)),
			'prob': 1.0,
			'next': [self.Daisy]
		}
		# camera 1
		self.Goofy = { # exit zone
			'cam': 1,
			'coord': (1.2,10.3),
			'var': ((1.0, 0.0), (0.0, 1.0)),
			'prob': TESTPROBS[0],
			'next': []
		}
		self.Donald = { # exit zone
			'cam': 1,
			'coord': (9.1,3.2),
			'var': ((2.0, 0.0), (0.0, 2.0)),
			'prob': TESTPROBS[1],
			'next': [self.Minni]
		}
		self.Mickey = { # entry zone
			'cam': 1,
			'coord': (0.0,0.0),
			'var': ((2.0, 0.0), (0.0, 2.0)),
			'prob': 1.0,
			'next': [self.Donald, self.Goofy]
		}

def get_exit(zone, p): # given previous zone and a random float, generate the exit 
	if p<zone['next'][0]['prob']: return zone['next'][0]
	return zone['next'][1]

def transitionTime(zone0, zone1):
	# calculate distance 
	d = np.linalg.norm(np.random.multivariate_normal(zone0['coord'], zone0['var'])-
						  np.random.multivariate_normal(zone1['coord'], zone0['var']))
	return d/randSpeed()

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
		tranTime = transitionTime(start, exit)
		targets.append((
				(start['coord'], time),
				(exit['coord'], time+tranTime) 
		))
		if exit['next']:
			start = exit
			time += tranTime
		else:
			start = ZONES.Mickey
			time = random.random()*n
			n -= 1

	return targets

json.dump(genTargets(10), open('data.json', 'w'))