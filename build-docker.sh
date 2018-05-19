#!/bin/bash
set -e

cp -f src/SimplCommerce.WebHost/appsettings.json ./appsettings.json
cp -f ./appsettings.docker.json src/SimplCommerce.WebHost/appsettings.json

docker build --network kknetwork -t kk-image:latest .

mv ./appsettings.json src/SimplCommerce.WebHost/appsettings.json