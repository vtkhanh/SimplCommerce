#!/bin/bash
set -ev

dotnet build $1

cd src/SimplCommerce.WebHost \
	&& dotnet run --no-build --no-restore