from concurrent.futures.thread import ThreadPoolExecutor
import requests
from datetime import datetime
from random import randrange
from time import sleep, time as timeNow
import concurrent.futures
from requests.sessions import PreparedRequest, Request
from sessionDataHelper import sessionDataFactory
from random import randrange
from config import Options, getArgs
from dockerStats import getContainerStats
# from benchmark import dockerStats
from math import floor
import subprocess

def toDateTime(timeString):
    return datetime.strptime(timeString, "%Y-%m-%dT%H:%M:%S.%f")
def timeString():
    return str(datetime.now().isoformat())

def postLogs(dataFactory, sessionsPerRequest, requestsToDo, isAllAtOnce):
    response_times = []
    preparedRequests = []
    if(isAllAtOnce):
        preparedRequests = [requests.Request("POST", "http://localhost:5000/api/log", json = dataFactory.get_sessions(n = sessionsPerRequest, err_rate = 0.01)).prepare() for _ in range(requestsToDo)]
        # response_times.append(r.elapsed.total_seconds()*1000)
    else:
        messageList = dataFactory.get_sessions(n = sessionsPerRequest, err_rate=0.01)
        preparedRequests = [requests.Request("POST", "http://localhost:5000/api/log", json = msg).prepare() for msg in messageList]
        # response_times.append(r.elapsed.total_seconds()*1000)

    with requests.Session() as s:
        for r in preparedRequests:
            startTime = timeNow()
            res = s.send(r)
            timings = (startTime, res.elapsed.total_seconds()*1000)
            response_times.append(timings)

    return response_times

def beClient(iterations):
    hostBaseUrl = "http://localhost:41646/api/product/"
    placeOrderUrl = hostBaseUrl + "PlaceOrder"
    confirmOrderUrl = hostBaseUrl + "ConfirmOrder"
    cancelOrderUrl = hostBaseUrl + "CancelOrder"
    invalidateSessionUrl = hostBaseUrl + "Invalidate"
    sessionIdHeader = 'X-Session-Id'
    responseTime = 0
    for _ in range(iterations):
        r = requests.post(placeOrderUrl)
        responseTime += r.elapsed.total_seconds()*1000
        r = requests.post(confirmOrderUrl, headers = {sessionIdHeader : r.headers[sessionIdHeader]})
        responseTime += r.elapsed.total_seconds()*1000
        while(r.status_code == 400):
            rand = randrange(0,9)
            if(rand == 0):
                r = requests.post(invalidateSessionUrl, headers = {sessionIdHeader : r.headers[sessionIdHeader]})
                responseTime += r.elapsed.total_seconds()*1000
            else:
                rand = randrange(0,1)
                if(rand == 0):
                    r = requests.post(confirmOrderUrl, headers = {sessionIdHeader : r.headers[sessionIdHeader]})
                    responseTime += r.elapsed.total_seconds()*1000
                else:
                    r = requests.post(cancelOrderUrl, headers = {sessionIdHeader : r.headers[sessionIdHeader]})
                    responseTime += r.elapsed.total_seconds()*1000
    return responseTime

    
def init(asClient : bool, sessionsPerRequest : int, totalRequests : int, threads : int, withStats : bool):
    num_threads = threads
    requests_per_thread = floor(totalRequests / threads);
    threads = []
    results = []
    stats = []
    start_time = datetime.now()
    
    dataFactory = sessionDataFactory()
    print(timeString())
    with concurrent.futures.ThreadPoolExecutor() as executor:
        for _ in range(num_threads):
            if asClient:
                t = executor.submit(beClient, totalRequests)
                threads.append(t)
            else:
                t = executor.submit(postLogs, dataFactory, sessionsPerRequest, requests_per_thread, True)
                threads.append(t)

        if withStats:
            while(any(not x.done() for x in threads)):
                stat = getContainerStats("thesis", "logprocessor")
                for s in stat:
                    stats.append((timeNow(),s))

    for t in concurrent.futures.as_completed(threads):
        results.append(t.result())
    print(timeString())

    if(asClient):
        csvSaveFile = "csv/"+str(round(timeNow()*1000)) + "as_client_threads" + str(num_threads) + \
                  "_reqs" + str(totalRequests) + ".csv"
        with open(csvSaveFile, "w") as f:
            for i, item in enumerate(results):
                f.write(str(item) + "\n")
    else:
        csvSaveFile = "csv/"+str(round(timeNow()*1000)) + "_threads" + str(num_threads) + \
                  "_reqs" + str(totalRequests) + "_" + str(sessionsPerRequest) + ".csv"
        with open(csvSaveFile, "w") as f:
            for i, item in enumerate(results):
                line = ':'.join(list(map(str, item)))
                f.write(line + "\n")
        if withStats:
            dockerStatsFilename = 'csv/'+ str(round(timeNow()*1000)) + '_threads' + \
                        str(num_threads) + "_reqs" + str(totalRequests) + "_" + str(sessionsPerRequest) + \
                        "_memory_cpu.csv"

            with open(dockerStatsFilename, "w") as f:
                for i, item in enumerate(stats):
                    line = ":".join([item[1].name, str(item[0]),  str(item[1].getCpuUsage()), str(item[1].getMemoryUsage())])
                    f.write(line + "\n")
    
    return csvSaveFile
    
def main(options: Options):
    if(options.withStats):
        popen = subprocess.Popen(["bash", "logprocess_start.sh"], stdout=subprocess.DEVNULL)
        popen.wait()
        status = True
        while status:
            try:
                r = requests.get("http://localhost:5000/api/log")
                if(r.status_code == 200):
                    status = False
            except:
                pass
        
    path = init(options.asClient, options.sessionsPerRequest, options.totalRequests, options.threads, options.withStats)
    
    if(options.withStats):
        popen = subprocess.Popen(["bash", "logprocess_stop.sh"], stdout=subprocess.DEVNULL)
        popen.wait()

    return path

if __name__ == "__main__":
   main(getArgs())