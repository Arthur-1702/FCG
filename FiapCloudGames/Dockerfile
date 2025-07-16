# Image base para o build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia o arquivo de projeto
COPY . ./
# Restaura as dependências e publica o app
RUN dotnet restore
RUN dotnet publish -c Release -o out

# Imagem base para o runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

# Copia os arquivos publicados do build
COPY --from=build /app/out ./

# Define a porta que o app irá escutar
EXPOSE 80

ENTRYPOINT ["dotnet", "FiapCloudGames.Api.dll"]
