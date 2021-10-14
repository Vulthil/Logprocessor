import numpy as np
import matplotlib.pyplot as plt
from matplotlib import ticker
import os
import csv
import pickle
from statistics import mean
import itertools
import sys
from sklearn.preprocessing import normalize
from math import floor

def running_mean(x, N):
    cumsum = np.cumsum(np.insert(x, 0, 0)) 
    return (cumsum[N:] - cumsum[:-N]) / float(N)

class ResultWrapper:
    def __init__(self, startTime:float, responseTime:float) -> None:
        self.startTime = startTime
        self.responseTime = responseTime

if __name__ == "__main__":
    saves = [[None for i in range(10)] for x in range(10)]
    memoryAndCpu = [[None for i in range(10)] for x in range(10)]

    directory = str(sys.argv[1])
    numReqs = 0
    
    arr = [ x for x in os.listdir("csv/"+directory) if x.endswith(".csv")]
    count = 0
    for filename in arr:
        if "memory" in filename:            
            with open("csv/"+directory+"/"+filename, newline='\n') as f:
                ls = []
                for row in csv.reader(f, delimiter=":"):
                    ls.append(list(map(float, row[1:])))
                ls = np.array(ls).T.tolist()
                x = (count//10)
                y = (count%10)
                memoryAndCpu[x][y] = (ls[0],ls[1],ls[2])
                #print(filename + " stored at ("+str(x)+","+str(y)+")")
            count += 1
        else:
            numThreads = int(filename.split("_")[-3][7:])
            numReqs = floor(int(filename.split("_")[-2][4:])/numThreads)*numThreads
            
            with open("csv/"+directory+"/"+filename, newline='\n') as f:
                ls = []
                startTimes = []
                for row in csv.reader(f, delimiter=":"):
                    tuples = list([eval(r) for r in row])
                    startTimes = startTimes + [startTime for (startTime, _) in tuples]
                    ls = ls + [time for (_, time) in tuples]
                x = (count//10)
                y = (count%10)
                # ls = np.array(ls).T.ravel().tolist()
                saves[x][y] = [ ResultWrapper(startTime,l) for startTime,l in zip(startTimes, ls)]
                #print(filename + " stored at ("+str(x)+","+str(y)+")")


    threads = int(sys.argv[2])
    reqSize = int(sys.argv[3])
    
    responseTimes : list[ResultWrapper] = saves[threads-1][reqSize-1]

    times = memoryAndCpu[threads-1][reqSize-1][0]
    cpuData = memoryAndCpu[threads-1][reqSize-1][1]
    memoryData = (np.array(memoryAndCpu[threads-1][reqSize-1][2])/1_000_000).tolist()

    responseTimes.sort(key = lambda x: x.startTime)
    orderedStartTimes = [x.startTime for x in responseTimes]
    orderedResponseTimes = [x.responseTime for x in responseTimes]
    
    try:
        mvAvgWindow = int(sys.argv[4])
    except:
        mvAvgWindow = int(len(orderedResponseTimes)/30)
    # Create figure and subplot manually
    # fig = plt.figure()
    # host = fig.add_subplot(111)

    # More versatile wrapper
    fig, host = plt.subplots(figsize=(10,6)) # (width, height) in inches
    # (see https://matplotlib.org/3.3.3/api/_as_gen/matplotlib.pyplot.subplots.html)
        
    par1 = host.twinx()
    par2 = host.twinx()
    mvAvg = host.twinx()

    reqTimeLim = max(orderedResponseTimes)*1.25
    host.set_xlim(0, max(orderedStartTimes)-min(orderedStartTimes))
    host.set_ylim(0, reqTimeLim)
    par1.set_ylim(0, 99)
    par2.set_ylim(0, max(memoryData)*1.25)
    mvAvg.set_ylim(0, reqTimeLim)

    host.set_xlabel("Request #")
    host.set_ylabel("Response time (ms)")
    par1.set_ylabel("CPU usage (% of system total)")
    par2.set_ylabel("Memory usage (MB)")
    #mvAvg no y label
    cm = plt.cm.inferno
    # color1 = cm(0)#plt.cm.viridis(0)
    # color2 = cm(0.25)#plt.cm.viridis(0.5)
    # color3 = cm(0.45)#plt.cm.viridis(.8)
    # color4 = plt.cm.tab10(0.6)#plt.cm.inferno(0.55)
    color1 = cm(0.1) #plt.cm.viridis(0)
    color2 = cm(0.55) #plt.cm.viridis(0.5)
    color3 = cm(0.8) #plt.cm.viridis(.8)
    color4 = plt.cm.tab10(0) #plt.cm.inferno(0.55)



    xs = [ t for t in np.array(orderedStartTimes)-min(orderedStartTimes)]

    xweirds = [t for t in np.array(times)-min(orderedStartTimes)]
   

    p1, = host.plot(xs, orderedResponseTimes,  color="C0", label="Request response time")
    p2, = par1.plot(xweirds, cpuData,    color="C3", label="CPU usage")
    p3, = par2.plot(xweirds, memoryData, color="C7", label="Memory usage")
    p4, = mvAvg.plot(xs[mvAvgWindow-1:], running_mean(orderedResponseTimes, mvAvgWindow), color="C1", label="Request response time, moving average")

    lns = [p1, p2, p3, p4]
    host.legend(handles=lns, loc='best')

    # right, left, top, bottom
    par2.spines['right'].set_position(('outward', 80))

    # no y-ticks for moving average              
    mvAvg.yaxis.set_ticks([])

    # Sometimes handy, same for xaxis
    #par2.yaxis.set_ticks_position('right')

    # Move "Velocity"-axis to the left
    # par2.spines['left'].set_position(('outward', 60))
    # par2.spines['left'].set_visible(True)
    # par2.yaxis.set_label_position('left')
    # par2.yaxis.set_ticks_position('left')

    # host.yaxis.label.set_color(p1.get_color())
    # par1.yaxis.label.set_color(p2.get_color())
    # par2.yaxis.label.set_color(p3.get_color())
    #mvAvg.yaxis.label.set_color(p4.get_color())


    # Adjust spacings w.r.t. figsize
    fig.tight_layout()
    # Alternatively: bbox_inches='tight' within the plt.savefig function 
    #                (overwrites figsize)

    # Best for professional typesetting, e.g. LaTeX

    #plt.savefig("csv/"+directory +"/"+ directory+".AWDAWDAWDpng", dpi=320, format="png")
    plt.show()  
