# Logprocessor

## Initial Setup

* Install Docker [https://docker.com](https://docker.com)

* Clone the repo
    ```
    git clone https://github.com/Vulthil/Logprocessor.git
    ```
* Take a copy of .env.example and rename it to .env
* Put in the values for the environment variables.
	* The defaults are usually alright.
	* if you want to view the logprocessor database, you can use the PGAdmin Docker container if you have supplied values for email and password in the .env file

In order to run the sample

* Running the logprocessor with its maangement UI:
```
    ./RunLogprocessor
```
* Running the logprocessor with case study:
```
    ./RunAll
```


### URLS

* Logprocessor UI
  * [http://localhost:5002](http://localhost:5002)
* Logprocessor swagger
  * [http://localhost:5000/swagger](http://localhost:5000/swagger)
* Microservice Gateway
  * If you want to "act" as a client invoking requests on the microservice system
  * Remember to copy the 'X-SessionID' header from the initial request to the next request (or not, depending on scenario) 
    * [http://localhost:41646/swagger](http://localhost:41646/swagger)
* PGAdmin
  * [http://localhost:8080](http://localhost:8080)
    * Only works if values for default email and password have been defined in the .env file