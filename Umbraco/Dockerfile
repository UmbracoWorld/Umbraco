# Use the SDK image to build and publish the website
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Umbraco.csproj", "."]
RUN dotnet restore "Umbraco.csproj"
COPY . .
RUN dotnet publish "Umbraco.csproj" -c Release -o /app/publish

# Copy the published output to the final running image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Umbraco.dll"]