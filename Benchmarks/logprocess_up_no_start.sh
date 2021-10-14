#!/bin/bash
cDir=$PWD
cd ../
docker-compose -f docker-compose.infrastructure.yml -f docker-compose.yml -f docker-compose.override.yml up --no-start logprocessor &> /dev/null
cd $cDir