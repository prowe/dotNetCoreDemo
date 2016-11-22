FROM microsoft/dotnet

EXPOSE 5000

ADD . /app
WORKDIR /app

RUN dotnet restore && dotnet build

CMD dotnet run