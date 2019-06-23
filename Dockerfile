FROM microsoft/dotnet:2.2-aspnetcore-runtime-stretch-slim AS base
WORKDIR /app
EXPOSE 5000

FROM microsoft/dotnet:2.2-sdk-stretch AS build
WORKDIR /src
COPY ./PersonalSiteApi/PersonalSiteApi.csproj .
RUN dotnet restore PersonalSiteApi.csproj
COPY ./PersonalSiteApi .
# WORKDIR /src
RUN dotnet build PersonalSiteApi.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish PersonalSiteApi.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
# Must bind the certificate into the container at /root/.aspnet/https
# And must give password for certificate as -e
#ENV Kestrel__Certificates__Default__Path="/root/.aspnet/https/PersonalSiteApi.pfx"
# If someone generated their own certificate and their own password they would have to change this. shouldn't be here.
#ENV Kestrel__Certificates__Default__Password="personalsiteapi"
#ENV ASPNETCORE_HTTPS_PORT="5001"
#ENV ASPNETCORE_URLS="https://*:5001;http://*:5000"
ENV ASPNETCORE_URLS="http://*:5000"
# must also specify the https_port in env variable. because that's what the 307 redirect responds with, so it will be the address used from outside the container.
ENTRYPOINT ["dotnet", "PersonalSiteApi.dll"]

# docker run --rm -it -P -e Kestrel__Certificates__Default__Password=personalsiteapi -v ${HOME}/.aspnet/https:/root/.aspnet/https personalsiteapi:latest

