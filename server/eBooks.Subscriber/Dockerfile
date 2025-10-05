FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY server/eBooks.Subscriber/eBooks.Subscriber.csproj eBooks.Subscriber/
COPY server/eBooks.Models/eBooks.Models.csproj eBooks.Models/
COPY server/eBooks.Database/eBooks.Database.csproj eBooks.Database/
COPY server/eBooks.Interfaces/eBooks.Interfaces.csproj eBooks.Interfaces/
COPY server/eBooks.Services/eBooks.Services.csproj eBooks.Services/

RUN dotnet restore eBooks.Subscriber/eBooks.Subscriber.csproj

COPY server/ .

RUN dotnet publish eBooks.Subscriber/eBooks.Subscriber.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "eBooks.Subscriber.dll"]
