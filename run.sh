#!/bin/bash
set -ev

dotnet clean

dotnet build $1

cd src/SimplCommerce.WebHost \
	&& dotnet run --no-build --no-restore
