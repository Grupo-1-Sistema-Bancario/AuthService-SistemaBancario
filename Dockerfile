# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution file and project files
COPY AuthServiceSistemaBancario.sln ./
COPY src/AuthServiceSistemaBancario.Api/AuthServiceSistemaBancario.Api.csproj src/AuthServiceSistemaBancario.Api/
COPY src/AuthServiceSistemaBancario.Application/AuthServiceSistemaBancario.Application.csproj src/AuthServiceSistemaBancario.Application/
COPY src/AuthServiceSistemaBancario.Domain/AuthServiceSistemaBancario.Domain.csproj src/AuthServiceSistemaBancario.Domain/
COPY src/AuthServiceSistemaBancario.Persistence/AuthServiceSistemaBancario.Persistence.csproj src/AuthServiceSistemaBancario.Persistence/

# Restore dependencies
RUN dotnet restore

# Copy the rest of the source code
COPY src/ src/

# Build and publish in release mode
RUN dotnet publish src/AuthServiceSistemaBancario.Api/AuthServiceSistemaBancario.Api.csproj -c Release -o /out

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /out .

# Expose the API port
EXPOSE 5023

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5023
ENV ASPNETCORE_ENVIRONMENT=Development

# Run the app
ENTRYPOINT ["dotnet", "AuthServiceSistemaBancario.Api.dll"]
