FROM node AS frontend

WORKDIR /src

COPY ["Frontend", "."]

RUN npm install

RUN npm run build

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY ["Identity.sln", "./"]

COPY --from=frontend ["src/dist/", "App/wwwroot/"]

COPY . .

RUN dotnet restore

RUN dotnet test

RUN dotnet build "./App" -c Release --output /app/build

FROM build AS publish

RUN dotnet publish "./App" -c Release --output /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "./App.dll"]