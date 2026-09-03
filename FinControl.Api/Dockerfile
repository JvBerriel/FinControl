# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY FinControl.Domain/FinControl.Domain.csproj FinControl.Domain/
COPY FinControl.Application/FinControl.Application.csproj FinControl.Application/
COPY FinControl.Infrastructure/FinControl.Infrastructure.csproj FinControl.Infrastructure/
COPY FinControl.Api/FinControl.Api.csproj FinControl.Api/
RUN dotnet restore FinControl.Api/FinControl.Api.csproj

COPY FinControl.Domain/ FinControl.Domain/
COPY FinControl.Application/ FinControl.Application/
COPY FinControl.Infrastructure/ FinControl.Infrastructure/
COPY FinControl.Api/ FinControl.Api/

RUN dotnet publish FinControl.Api/FinControl.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

RUN useradd --uid 5678 --user-group --no-create-home appuser
USER appuser

COPY --from=build /app .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "FinControl.Api.dll"]
