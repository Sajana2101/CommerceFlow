FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["CommerceFlow.Api/CommerceFlow.Api.csproj", "CommerceFlow.Api/"]
COPY ["CommerceFlow.Application/CommerceFlow.Application.csproj", "CommerceFlow.Application/"]
COPY ["CommerceFlow.Domain/CommerceFlow.Domain.csproj", "CommerceFlow.Domain/"]
COPY ["CommerceFlow.Infrastructure/CommerceFlow.Infrastructure.csproj", "CommerceFlow.Infrastructure/"]

RUN dotnet restore "CommerceFlow.Api/CommerceFlow.Api.csproj"

COPY . .

WORKDIR "/src/CommerceFlow.Api"

RUN dotnet publish "CommerceFlow.Api.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "CommerceFlow.Api.dll"]