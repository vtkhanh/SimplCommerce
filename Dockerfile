# Build image
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS builder

WORKDIR /app

# Install node
ENV NODE_VERSION 8.9.4
ENV NODE_DOWNLOAD_SHA 21fb4690e349f82d708ae766def01d7fec1b085ce1f5ab30d9bda8ee126ca8fc
RUN curl -SL "https://nodejs.org/dist/v${NODE_VERSION}/node-v${NODE_VERSION}-linux-x64.tar.gz" --output nodejs.tar.gz \
	&& echo "$NODE_DOWNLOAD_SHA nodejs.tar.gz" | sha256sum -c - \
	&& tar -xzf "nodejs.tar.gz" -C /usr/local --strip-components=1 \
	&& rm nodejs.tar.gz \
	&& ln -s /usr/local/bin/node /usr/local/bin/nodejs

# Install npm & bower packages
COPY src/SimplCommerce.WebHost/package.json  src/SimplCommerce.WebHost/
RUN cd src/SimplCommerce.WebHost \
	&& npm install --global gulp-cli \
	&& npm install

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

COPY ./Directory.Build.props ./run-tests.sh ./
RUN dotnet restore

COPY ./src ./src
COPY ./test ./test
RUN dotnet build -c Release --no-restore

# Run test projects
RUN chmod 755 ./run-tests.sh
RUN ./run-tests.sh

# Publish
WORKDIR /app/src/SimplCommerce.WebHost
RUN dotnet publish -c Release -o dist --no-restore --no-build

# App image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1

ENV ASPNETCORE_URLS http://+:5000

WORKDIR /app	

# Enable SSH
EXPOSE 2222 80
RUN apt-get update \
    && apt-get install -y --no-install-recommends openssh-server \
    && mkdir -p /run/sshd \
    && echo "root:Docker!" | chpasswd
COPY sshd_config /etc/ssh/sshd_config

COPY --from=builder /app/src/SimplCommerce.WebHost/dist ./

ENTRYPOINT ["/bin/bash", "-c", "/usr/sbin/sshd && dotnet SimplCommerce.WebHost.dll"]
