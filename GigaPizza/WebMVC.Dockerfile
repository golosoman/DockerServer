# См. статью по ссылке https://aka.ms/customizecontainer, чтобы узнать как настроить контейнер отладки и как Visual Studio использует этот Dockerfile для создания образов для ускорения отладки.

# Этот этап используется при запуске из VS в быстром режиме (по умолчанию для конфигурации отладки)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

# Создаем папку данных с правильными правами
USER root
RUN mkdir -p /data && \
    chown -R $APP_UID:$APP_UID /data && \
    chmod 755 /data
USER $APP_UID
WORKDIR /app
EXPOSE 5000
# EXPOSE 5001
# EXPOSE 5001ENV ASPNETCORE_URLS=http://+:5000;https://+:5001 ASPNETCORE_ENVIRONMENT=Development MSSQL_ENABLE_HADR=0 MSSQL_AGENT_ENABLED=1
ENV ASPNETCORE_URLS=http://+:5000 ASPNETCORE_ENVIRONMENT=Development MSSQL_ENABLE_HADR=0 MSSQL_AGENT_ENABLED=1



# Этот этап используется для сборки проекта службы
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["GigaPizza.csproj", "."]
RUN dotnet restore "./GigaPizza.csproj"
COPY . .
RUN dotnet build "./GigaPizza.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этот этап используется для публикации проекта службы, который будет скопирован на последний этап
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GigaPizza.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Этап final должен быть исправлен:
FROM base AS final
WORKDIR /app

# Копируем скрипт в корень приложения
COPY --from=publish /app/publish .
#COPY wait-for-db.sh /app/

USER root
RUN mkdir -p /app/wwwroot/images/pizza && \
    chown -R $APP_UID:$APP_UID /app/wwwroot/images/pizza && \
    chmod -R 755 /app/wwwroot/images/pizza

# Возвращаемся к обычному пользователю
USER $APP_UID

# Используем полный путь к скрипту
#ENTRYPOINT ["/app/wait-for-db.sh", "dotnet", "/app/GigaPizza.dll"]
ENTRYPOINT ["dotnet", "/app/GigaPizza.dll"]

