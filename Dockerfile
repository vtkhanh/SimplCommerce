# Build image
FROM microsoft/dotnet:2.1-sdk AS builder

WORKDIR /app

# Install npm
RUN apt-get -qq update && apt-get -qqy --no-install-recommends install git unzip

RUN curl -sL https://deb.nodesource.com/setup_6.x |  bash -
RUN apt-get install -y nodejs

# Copy solution file
COPY ./*.sln ./
# Copy Infrastructure and Webhost csproj files to root directory
COPY src/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p src/${file%.*}/ && mv $file src/${file%.*}/; done
# Copy modules csproj files
COPY src/Modules/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p src/Modules/${file%.*}/ && mv $file src/Modules/${file%.*}/; done
# Copy test csproj files
COPY test/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p test/${file%.*}/ && mv $file test/${file%.*}/; done

RUN dotnet restore 

COPY ./src ./src
COPY ./test ./test
COPY ./Directory.Build.props ./global.json ./run-tests.sh ./

RUN dotnet build -c Release --no-restore

# Run test projects
RUN chmod 755 ./run-tests.sh
RUN ./run-tests.sh

WORKDIR /app/src/SimplCommerce.WebHost
RUN cp -f ./appsettings.docker.json ./appsettings.json
RUN sed -i 's/Debug/Release/' gulpfile.js \
	&& npm install \
	&& npm install --global bower \
	&& npm install --global gulp-cli \
	&& gulp
RUN	dotnet ef database update
RUN dotnet publish -c Release -o dist --no-restore 

# App image
FROM microsoft/dotnet:2.1-aspnetcore-runtime
ENV ASPNETCORE_URLS http://+:5000

WORKDIR /app	
COPY --from=builder /app/src/SimplCommerce.WebHost/dist ./
ENTRYPOINT ["dotnet", "SimplCommerce.WebHost.dll"]
