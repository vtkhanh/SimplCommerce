#!/bin/bash
set -e

#Use staging db
export ASPNETCORE_ENVIRONMENT=Staging

rm -rf src/SimplCommerce.WebHost/Migrations/*

dotnet restore && dotnet build

cd src/SimplCommerce.WebHost \
	&& npm install \
	&& npm install --global gulp-cli \
	&& gulp \
	&& dotnet ef migrations add initialSchema \
	&& sed -i'' -e '/using SimplCommerce.Module.*.Models;/d' Migrations/SimplDbContextModelSnapshot.cs \
	&& sed -i'' -e '/using SimplCommerce.Module.*.Models;/d' Migrations/*_initialSchema.Designer.cs \
	&& dotnet ef database update
	

# Back to Dev environment
# export ASPNETCORE_ENVIRONMENT=Development

echo "Then type 'dotnet run' in src/SimplCommerce.WebHost to start the app."
