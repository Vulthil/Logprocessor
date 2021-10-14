from sessionMessageBenchmark import main
from config import Options
import contextlib
from defaultlist import defaultlist
import subprocess

if __name__ == "__main__":


    threads  = [x+1 for x in range(10)]
    logs = [x*100 + 100 for x in range (10)]
    files = [defaultlist() for x in range(10)]
    totalRequests = 500
    dockerStats = False
    if dockerStats:
        popen = subprocess.Popen(["bash", "logprocess_build.sh"], stdout=subprocess.DEVNULL)
        popen.wait()
        popen = subprocess.Popen(["bash", "logprocess_up_no_start.sh"], stdout=subprocess.DEVNULL)
        popen.wait()
        popen = subprocess.Popen(["bash", "postgres_up.sh"], stdout=subprocess.DEVNULL)
        popen.wait()


    for tidx, t in enumerate(threads):
        for lidx, l in enumerate(logs):
            with contextlib.redirect_stdout(None):
                options = Options(False, l, totalRequests, t, dockerStats)
                files[tidx][lidx] = main(options)
            print("finished run %s of %s, (t=%s,l=%s)"% (10*tidx+(lidx+1),100,t,l))
    if dockerStats:
        popen = subprocess.Popen(["bash", "logprocess_down.sh"], stdout=subprocess.DEVNULL)
        popen.wait()

