FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["CommerceFlow.NotificationService/CommerceFlow.NotificationService.csproj", "CommerceFlow.NotificationService/"]
COPY ["CommerceFlow.Application/CommerceFlow.Application.csproj", "CommerceFlow.Application/"]
COPY ["CommerceFlow.Domain/CommerceFlow.Domain.csproj", "CommerceFlow.Domain/"]

RUN dotnet restore "CommerceFlow.NotificationService/CommerceFlow.NotificationService.csproj"

COPY . .

WORKDIR "/src/CommerceFlow.NotificationService"

RUN dotnet publish "CommerceFlow.NotificationService.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "CommerceFlow.NotificationService.dll"]