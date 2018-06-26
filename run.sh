#!/bin/bash
set -ev

dotnet build $1

cd src/SimplCommerce.WebHost \
	&& gulp \
	&& dotnet run --no-build --no-restore