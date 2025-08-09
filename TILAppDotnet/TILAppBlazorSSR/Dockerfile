# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR src
COPY ["TILAppBlazorSSR/TILAppBlazorSSR.csproj", "TILAppBlazorSSR/"]
COPY ["TILApp/TILApp.csproj", "./"]
RUN dotnet restore 
COPY ["TILAppBlazorSSR/", "TILAppBlazorSSR/"]
COPY ["TILApp/", "TILApp/"]
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "TILAppBlazorSSR.dll"]