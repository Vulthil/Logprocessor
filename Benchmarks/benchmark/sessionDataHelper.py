import itertools
import uuid
from datetime import datetime
from random import randrange

def time():
    return str(datetime.now().isoformat())

class sessionDataFactory:
    def __init__(self):
        self.server = "BuyerGateway.Server"
        self.client = "BuyerGateway.Client"
        self.inventory = "InventoryService"


    def get_sessions(self, n = 1000,err_rate = 0, flat = True):
        messages = [self.__getMessages(err_rate=err_rate) for x in range(n)]
        return list(itertools.chain(*messages)) if flat else messages

    def get_partial_sessions(self, n = 1000, flat = True):
        session_list = [self.__getMessages() for x in range(n)]
        for session_messages in session_list:
            session_messages.pop()
        return list(itertools.chain(*session_list)) if flat else session_list


    def __getMessages(self, err_rate = 0):
        err_thresh = err_rate * 100
        sessionId = str(uuid.uuid4())
        return [{"origin":self.client, "destination":self.server, "targetApi":"PlaceOrder", "sessionId":sessionId, "Direction":0, "time":time()},
        {"origin":self.server, "destination":"", "targetApi":"GetSKUDetails", "sessionId":sessionId, "Direction":1, "time":time()},
        {"origin":self.server, "destination":self.inventory, "targetApi":"GetSKUDetails", "sessionId":sessionId, "Direction":0, "time":time()},
        {"origin":self.inventory, "destination":"", "targetApi":"SKUDetails", "sessionId":sessionId, "Direction":1, "time":time()},
        {"origin":self.inventory, "destination":self.server, "targetApi":("ExplicitViolation" if randrange(0,99) < err_thresh else "SKUDetails"), "sessionId":sessionId, "Direction":0, "time":time()},
        {"origin":self.server, "destination":self.client, "targetApi":"PlaceOrder", "sessionId":sessionId, "Direction":1, "time":time()},
        {"origin":self.client, "destination":self.server, "targetApi":"CancelOrder", "sessionId":sessionId, "Direction":0, "time":time()},
        {"origin":self.server, "destination":self.client, "targetApi":"CancelOrder", "sessionId":sessionId, "Direction": 1, "time":time()}]