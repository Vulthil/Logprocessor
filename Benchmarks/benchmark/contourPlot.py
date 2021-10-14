import numpy as np
import matplotlib.pyplot as plt
from matplotlib import ticker
import os
import csv
import pickle
from statistics import mean
import sys

if __name__ == "__main__":
    avgs = [[None for i in range(10)] for x in range(10)]
    directory = str(sys.argv[1])

    arr = [ x for x in os.listdir("csv/"+directory) if x.endswith(".csv")]
    count = 0
    for filename in arr:
        if "memory" in filename:
            continue

        with open("csv/"+directory+"/"+filename, newline='\n') as f:
            ls = []
            # startTimes = [] # Not used for this plot
            for row in csv.reader(f, delimiter=":"):
                tuples = list([eval(r) for r in row])
                # startTimes = startTimes + [startTime for (startTime, _) in tuples]
                ls = ls + [time for (_, time) in tuples] 
            x = (count//10)
            y = (count%10)
            #print(filename + " stored at ("+str(x)+","+str(y)+")")
            avgs[x][y] = mean(ls)
        count += 1

    xlist = np.linspace(1, 10, 10)
    ylist = np.linspace(100, 1000, 10)
    X, Y = np.meshgrid(xlist, ylist)
    Z = avgs
    fig,ax=plt.subplots(1,1)
    cp = ax.contourf(X, Y, Z, levels = [(x)*25 for x in range(20)], cmap="viridis")
    cb = fig.colorbar(cp) # Add a colorbar to a plot
    # tick_locator = ticker.MaxNLocator(nbins=10)
    # cb.locator = tick_locator
    # cb.update_ticks()
    #cb.set_label('Avg. response time (ms)', rotation=270)
    ax.set_title('Average request response time (ms)')
    ax.set_xlabel('x (threads)')
    ax.set_ylabel('y (sessions per request)')
    plt.savefig("csv/"+directory +"/"+ directory+".png", dpi=320, format="png")
    plt.show()  
