# Logprocessor

## How to run - Docker
* To run the Logprocessor and/or the case study, you will need to first install Docker [https://docker.com](https://docker.com)

* Clone the repo
    ```
    git clone https://github.com/Vulthil/Logprocessor.git
    ```
* Make a copy of .env.example (in repo root) and rename it to .env
* Put in the values for the environment variables.
	* The defaults are fine unless you have a running, conflicting Docker configuration.
	* The PGAdmin Docker container will be usable if you have supply values for email and password in the .env file, but it is not required for the Logprocessor to run

You can now either
* Run the logprocessor with just its UI (If you will be producing logs yourself):
```
    ./RunLogprocessor
```

Or 
* Run the logprocessor with case study ('Microservices' & Fluentd):
```
    ./RunAll
```

If you do not wish to use Docker, or want to debug the Logprocessor, you can also run ti locally.
## How to run - Locally
To run the Logprocessor locally, you must execute the [Logprocessor](https://github.com/Vulthil/Logprocessor/tree/master/LogProcessor/LogProcessor) .csproj by doing a `dotnet run` command in the [Logprocessor project folder](https://github.com/Vulthil/Logprocessor/tree/master/LogProcessor/LogProcessor) to run the logprocessor server, as well as `dotnet run` in the [UI.Server](https://github.com/Vulthil/Logprocessor/tree/master/LogProcessor/UI/Server) folder to host the UI.
For the logprocessor to work locally, you will also need to change the "ConnectionStrings:Default" connection string in `appsettings.json` to point to a Postgres database that can be used. The application will apply migrations to the database on startup by itself.

Likewise the Logprocessor can be developed and debugged in your editor of choice, so long as a connection to a Postgres database is available.

Building and running the logprocessor locally will require a [.NET 5.0 SDK](https://dotnet.microsoft.com/download/dotnet/5.0) as well as a [Java JRE](https://www.java.com/en/download/manual.jsp) > v1.7


### URLS
The most importants URLS hosted by the Logprocessor are:
* Logprocessor UI
  * [http://localhost:5002](http://localhost:5002)/[https://localhost:5003](https://localhost:5003)
* Logprocessor swagger
  * [http://localhost:5000/swagger](http://localhost:5000/swagger)/[https://localhost:5001/swagger](https://localhost:5001/swagger)
* Microservice Gateway Swagger
  * [http://localhost:41646/swagger/index.html](http://localhost:41646/swagger/index.html) (only accessible when dockerized)
    * If get a  "Sorry there's nothing at this address, try doing a Ctrl+Shift+R
  * If you want to "act" as a client performing requests to the microservices in the case study
  * Remember to copy the 'X-Session-ID' header from the initial request to the next request (or not, depending on scenario) 

* PGAdmin
  * [http://localhost:8080](http://localhost:8080)
    * Only works if values for default email and password have been defined in the .env file
	
### Misc.
* Kibana
  * If you use the inspect button on tracked/faulted sessions, Kibana will ask you to create an 'index pattern' - Simply follow the on screen instructions and create an index pattern called 'fluentd-*' with timestamp field '@timestamp'
