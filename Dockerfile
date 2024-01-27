FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
RUN mkdir -p /app/{Server,Client,Shared}
COPY Server/DominosStockOrder.Server.csproj /app/Server/
COPY Client/DominosStockOrder.Client.csproj /app/Client/
COPY Shared/DominosStockOrder.Shared.csproj /app/Shared/
RUN cd /app/ \
 && dotnet restore Server/DominosStockOrder.Server.csproj
COPY . /app
RUN cd /app/Server \
 && dotnet build DominosStockOrder.Server.csproj -c Release -o /app/build \
 && dotnet publish DominosStockOrder.Server.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 as final
copy --from=build /app/publish /app
EXPOSE 80
EXPOSE 443
WORKDIR /app
ENTRYPOINT ["dotnet", "DominosStockOrder.Server.dll"]
