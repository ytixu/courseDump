# Maximum-a-posteriori
import json

from utils import zones

# trajectory data
data = json.load(open('dataZones.json', 'r'))
zone_entry, zone_exit = json.load(open('detaTarget.json', 'r'))

def mapClasify(obs):
	entry, exit = obs
	# find max entry
	coord, time = entry
	max_entry = null
	max_prob = -1
	for zone in zone_entry:
		p = zone['prior']
