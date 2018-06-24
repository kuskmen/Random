FROM microsoft/aspnetcore:2.0-nanoserver-1709 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0-nanoserver-1709 AS build
WORKDIR /src
COPY Consul.Service/Consul.Service.csproj Consul.Service/
RUN dotnet restore Consul.Service/Consul.Service.csproj
COPY . .
WORKDIR /src/Consul.Service
RUN dotnet build Consul.Service.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Consul.Service.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Consul.Service.dll"]
