#!/bin/bash
set -ev

# -ni --> --no-image --> do not build image
if [ -z $1 ] || [ $1 != '-ni' ]
then
    cp -f src/SimplCommerce.WebHost/appsettings.Production.json src/SimplCommerce.WebHost/appsettings.json

    docker build --network kknetwork -t kk-image:latest .
fi

docker run --rm -it -p 80:5000 --network kknetwork kk-image:latest
