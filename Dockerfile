FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
RUN mkdir -p /app/{Server,Client,Shared}
COPY Server/DominosStockOrder.Server.csproj /app/Server/
COPY Client/DominosStockOrder.Client.csproj /app/Client/
COPY Shared/DominosStockOrder.Shared.csproj /app/Shared/
RUN cd /app/ \
 && dotnet restore /app/Server/DominosStockOrder.Server.csproj
COPY . /app
RUN cd /app/Server \
 && dotnet build DominosStockOrder.Server.csproj -c Release -o /app/build \
 && dotnet publish DominosStockOrder.Server.csproj -c Release -p:PublishChromeDriver=true -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

RUN apt update
RUN apt install -y --no-install-recommends wget gnupg2

# Get Chrome
RUN wget -q -O - https://dl.google.com/linux/linux_signing_key.pub | apt-key add -
RUN sh -c 'echo "deb http://dl.google.com/linux/chrome/deb/ stable main" >> /etc/apt/sources.list.d/google.list'
RUN apt-get update
RUN apt-get install -y google-chrome-stable

COPY --from=build /app/publish /app

EXPOSE 80
EXPOSE 443
EXPOSE 9222
WORKDIR /app
ENTRYPOINT ["dotnet", "DominosStockOrder.Server.dll"]
