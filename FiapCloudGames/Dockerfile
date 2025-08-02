# Image base para o build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia o arquivo de projeto
COPY ./ ./
# Restaura as dependências e publica o app
RUN dotnet restore
RUN dotnet publish -c Release -o out

# Imagem base para o runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0

RUN apt-get update && \
    apt-get install -y \
    curl \
    gnupg2 \
    apt-transport-https \
    gcc \
    unixodbc-dev \
    ca-certificates \
    --no-install-recommends && \
    rm -rf /var/lib/apt/lists/*

# Intala Microsoft ODBC Driver SQL Server (Ubuntu)
RUN curl -sSL https://packages.microsoft.com/keys/microsoft.asc > microsoft.asc && \
    gpg --dearmor microsoft.asc && \
    mv microsoft.asc.gpg /etc/apt/trusted.gpg.d/ && \
    curl -sSL https://packages.microsoft.com/config/debian/10/prod.list > /etc/apt/sources.list.d/mssql-release.list && \
    apt-get update && \
    ACCEPT_EULA=Y apt-get install -y msodbcsql17 && \
    rm -rf /var/lib/apt/lists/*
    
WORKDIR /app

# Copia os arquivos publicados do build
COPY --from=build /app/out .


ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT ["dotnet", "FiapCloudGames.Api.dll"]
