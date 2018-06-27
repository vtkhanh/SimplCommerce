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

docker run --rm -it -p 8000:80 -p 8001:443 -e ASPNETCORE_HTTPS_PORT=8001 -e ASPNETCORE_Kestrel__Certificates__Default__Password="passw0rd" -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx -v ${HOME}/.aspnet/https:/https/ kk-image