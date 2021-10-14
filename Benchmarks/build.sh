#!/bin/bash
cDir=$PWD
cd ../
docker-compose -f docker-compose.infrastructure.yml -f docker-compose.yml -f docker-compose.override.yml build logprocessor buyergateway.server bankservice shippingservice inventoryservice
cd $cDir