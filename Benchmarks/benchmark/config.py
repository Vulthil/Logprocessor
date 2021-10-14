import argparse

class Options:
    def __init__(self, asClient, sessionsPerRequest, totalRequests, threads, withStats):
        self.asClient = asClient
        self.sessionsPerRequest = sessionsPerRequest
        self.totalRequests = totalRequests
        self.threads = threads
        self.withStats = withStats

def getArgs() -> Options:
    parser = argparse.ArgumentParser(prog="benchmark", description='Run Benchmarks on Logprocessing')

    parser.add_argument('-e', dest="asClient",
                        action='store_true',
                        help='Act as client instead of posting logs')

    parser.add_argument('-s', dest="withStats",
                        action='store_true',
                        help='Include docker statistics')

    parser.add_argument('-n', dest="sessionsPerRequest",
                        type=int,
                        default=200,
                        help='Number of sessions to post per request, mutually exclusive with -e, default 200')

    parser.add_argument('-l', dest='totalRequests', type=int,
                        default=1000,
                        help='Total number of requests to perform, default 1000')
                        
    parser.add_argument('-t', dest='threads', type=int,
                        default=2,
                        help='Number of threads, default 2')
                        
    args = parser.parse_args()
    return Options(args.asClient, args.sessionsPerRequest, args.totalRequests, args.threads, args.withStats)
    

