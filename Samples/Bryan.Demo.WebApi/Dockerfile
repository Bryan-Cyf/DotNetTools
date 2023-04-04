#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Samples/Bryan.Demo.WebApi/Bryan.Demo.WebApi.csproj", "Samples/Bryan.Demo.WebApi/"]
COPY ["Notice/Bryan.Tools.Notice/Bryan.Tools.Notice.csproj", "Notice/Bryan.Tools.Notice/"]
COPY ["Tools/Bryan.Tools.Configuation/Bryan.Tools.Configuration.csproj", "Tools/Bryan.Tools.Configuation/"]
COPY ["Tools/Bryan.Tools.SnowFlakeId/Bryan.Tools.SnowFlakeId.csproj", "Tools/Bryan.Tools.SnowFlakeId/"]
COPY ["Middleware/Bryan.Tools.Cache/Bryan.Tools.Cache.csproj", "Middleware/Bryan.Tools.Cache/"]
COPY ["Middleware/Bryan.Tools.ElasticSearch/Bryan.Tools.ElasticSearch.csproj", "Middleware/Bryan.Tools.ElasticSearch/"]
COPY ["Sdk/Bryan.Tools.SpeechRecognition/Bryan.Tools.SpeechRecognition.csproj", "Sdk/Bryan.Tools.SpeechRecognition/"]
COPY ["Middleware/Bryan.Tools.Mongo/Bryan.Tools.Mongo.csproj", "Middleware/Bryan.Tools.Mongo/"]
COPY ["Tools/Bryan.Tools.PinYin/Bryan.Tools.PinYin.csproj", "Tools/Bryan.Tools.PinYin/"]
COPY ["Tools/Bryan.Tools.Swagger/Bryan.Tools.Swagger.csproj", "Tools/Bryan.Tools.Swagger/"]
RUN dotnet restore "Samples/Bryan.Demo.WebApi/Bryan.Demo.WebApi.csproj"
COPY . .
WORKDIR "/src/Samples/Bryan.Demo.WebApi"
RUN dotnet build "Bryan.Demo.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Bryan.Demo.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bryan.Demo.WebApi.dll"]