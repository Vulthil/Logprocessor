from typing import List
import docker
import re
from docker.client import DockerClient
from docker.models.containers import Container

class Stats:

    def __init__(self, name: str, allStats):
        self.name = name
        self.allStats = allStats
        
    def getCpuUsage(self) -> float:
        cpupercentage = 0.0
        prestats = self.allStats['precpu_stats']
        cpustats = self.allStats['cpu_stats']
        
        prestats_totalusage = prestats['cpu_usage']['total_usage']
        stats_totalusage = cpustats['cpu_usage']['total_usage']
        numOfCPUCore = len(cpustats['cpu_usage']['percpu_usage'])
        #print('prestats_totalusage: %s, stats_totalusage: %s, NoOfCore: %s' % (
        #prestats_totalusage, stats_totalusage, numOfCPUCore))

        prestats_syscpu = prestats['system_cpu_usage']
        stats_syscpu = cpustats['system_cpu_usage']
        #print('prestats_syscpu: %s, stats_syscpu: %s' % (prestats_syscpu, stats_syscpu))

        cpuDelta = stats_totalusage - prestats_totalusage
        systemDelta = stats_syscpu - prestats_syscpu

        if cpuDelta > 0 and systemDelta > 0:
            cpupercentage = (cpuDelta / systemDelta) #* numOfCPUCore

        #formattedcpupert = '{:.1%}'.format(cpupercentage)
        #print('cpuDelta: %s, systemDelta: %s, cpu: %s' % (cpuDelta, systemDelta, cpupercentage))

        #print('"%s" Container CPU: %s ' % (conName, formattedcpupert))
        return cpupercentage*100

    def getMemoryUsage(self) -> float:
        return self.allStats["memory_stats"]['usage']

def __getDockerClient() -> DockerClient:
    cli = docker.from_env()
    return  cli

def __getContainerInComposeMode(compose_folder:str = "", container_to_monitor: str = "") -> List[str]:
    if(compose_folder != "" and  compose_folder[-1] != "_"):
        compose_folder = compose_folder + "_"
    if(container_to_monitor != ""):
        con_name_pattern = compose_folder + container_to_monitor + "_"
    else:
        con_name_pattern = compose_folder + '*'
    listOfSameContainerInCompose = []
    regexptn = con_name_pattern
    if regexptn != '*':
        regexptn = "^" + regexptn 
    else:
        regexptn = "." + regexptn
    pattern = re.compile(regexptn)

    cli = __getDockerClient()

    for con in cli.containers.list():
        conname = con.name
        if (pattern.match(conname)):
            listOfSameContainerInCompose.append(conname)

    return listOfSameContainerInCompose

def __getStats(con) -> Stats:
    conName = con.name
    # Check if the container is running
    if (con.status != 'running'):
        raise ValueError('"%s" container is not running' % conName)

    # Get CPU Usage in percentage
    constat = con.stats(stream=False)
    
    return Stats(conName, constat)

def getContainerStats(compose_folder:str = "", container_to_monitor: str = "") -> List[Stats]:
    cli = __getDockerClient()
    cons = __getContainerInComposeMode(compose_folder, container_to_monitor)
    stats = []
    for con in cons:
        con = cli.containers.get(con)
        stats.append(__getStats(con))
    return stats

if __name__ == "__main__":
    for stats in getContainerStats('thesis', 'logprocessor'):
        print('Container: %s, cpuPercentage: %s, memory: %s' % (stats.name,stats.getCpuUsage(), stats.getMemoryUsage()))
