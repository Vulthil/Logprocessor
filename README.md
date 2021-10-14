# Thesis

## Initial Setup

* Clone the repo
    ```
    git clone https://github.com/zornwal/Thesis.git
    ```
* Take a copy of .env.example and rename it to .env
* Put in the values for the environment variables.
* Run the script RunAll_Infra to run the infrastructure.
  * This will spin up docker containers for
    * rabbitmq
    * fluentd
    * elasticsearch
    * kibana
    * postgres
    * pgAdmin
* Run 
    ```
    docker-compose up -d
    ```
    to run the LogProcessing service with a suite of microservices.