# FROM mcr.microsoft.com/dotnet/aspnet:5.0
# COPY ReuseventoryApi/bin/Release/net5.0/publish/ App/
# WORKDIR /App
# # ENTRYPOINT ["dotnet", "ReuseventoryApi.dll"]
# CMD ASPNETCORE_URLS=http://*:$PORT dotnet ReuseventoryApi.dll

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . .
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build-env /app/out .

# Run the app on container startup
# Use your project name for the second parameter
# e.g. MyProject.dll
# ENTRYPOINT [ "dotnet", "HerokuApp.dll" ]
CMD ASPNETCORE_URLS=http://*:$PORT dotnet ReuseventoryApi.dll