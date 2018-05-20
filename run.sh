#!/bin/bash
set -e

#Use staging db
#export ASPNETCORE_ENVIRONMENT=Staging

dotnet restore && dotnet build

cd src/SimplCommerce.WebHost \
	&& gulp \
	&& dotnet run --no-build --no-restore