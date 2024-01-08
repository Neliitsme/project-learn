FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /App

# Copy project files
COPY devtools-proj ./devtools-proj
COPY devtools-proj.sln .

# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /App
COPY --from=build-env /App/out .

ENV ASPNETCORE_URLS=http://+:5053
ENV ASPNETCORE_ENVIRONMENT=Release

EXPOSE 5053
ENTRYPOINT ["dotnet", "devtools-proj.dll"]
