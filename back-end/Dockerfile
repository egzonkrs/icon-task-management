FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

COPY Directory.Build.props Directory.Packages.props ./
COPY src/Icon.SharedKernel/Icon.SharedKernel.csproj src/Icon.SharedKernel/
COPY src/Icon.Domain/Icon.Domain.csproj src/Icon.Domain/
COPY src/Icon.Application/Icon.Application.csproj src/Icon.Application/
COPY src/Icon.Infrastructure/Icon.Infrastructure.csproj src/Icon.Infrastructure/
COPY src/Icon.Api/Icon.Api.csproj src/Icon.Api/
RUN dotnet restore src/Icon.Api/Icon.Api.csproj

COPY . .
RUN dotnet publish src/Icon.Api -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview
WORKDIR /app
COPY --from=build /app .
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["dotnet", "Icon.Api.dll"]
