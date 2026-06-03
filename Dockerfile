# ---- Stage 1: Build the React frontend ----
FROM node:20-alpine AS frontend
WORKDIR /src
COPY front-end/package.json front-end/package-lock.json ./
RUN npm ci
COPY front-end/ ./
RUN npm run build

# ---- Stage 2: Build the .NET backend ----
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS backend
WORKDIR /src
COPY back-end/Directory.Build.props back-end/Directory.Packages.props ./
COPY back-end/src/Icon.SharedKernel/Icon.SharedKernel.csproj src/Icon.SharedKernel/
COPY back-end/src/Icon.Domain/Icon.Domain.csproj src/Icon.Domain/
COPY back-end/src/Icon.Application/Icon.Application.csproj src/Icon.Application/
COPY back-end/src/Icon.Infrastructure/Icon.Infrastructure.csproj src/Icon.Infrastructure/
COPY back-end/src/Icon.Api/Icon.Api.csproj src/Icon.Api/
RUN dotnet restore src/Icon.Api/Icon.Api.csproj
COPY back-end/. .
RUN dotnet publish src/Icon.Api -c Release -o /app --no-restore

# ---- Stage 3: Runtime ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview
WORKDIR /app
COPY --from=backend /app .
COPY --from=frontend /src/dist ./wwwroot
ENTRYPOINT ["sh", "-c", "dotnet Icon.Api.dll --urls http://+:${PORT:-8080}"]
