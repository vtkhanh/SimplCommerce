#!/bin/bash
set -ev

# -ni --> --no-image --> do not build image
if [ -z $1 ] || [ $1 != '-ni' ]
then
    cp -f src/SimplCommerce.WebHost/appsettings.json ./appsettings.json

    cp -f src/SimplCommerce.WebHost/appsettings.Production.json src/SimplCommerce.WebHost/appsettings.json

    docker build --network kknetwork -t kk-image:latest .

    mv ./appsettings.json src/SimplCommerce.WebHost/appsettings.json
fi

docker run --rm -it -p 80:80 kk-image
