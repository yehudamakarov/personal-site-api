FROM microsoft/dotnet:2.2-aspnetcore-runtime-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk-stretch AS build
WORKDIR /src
COPY ["PersonalSiteApi.csproj", "."]
RUN dotnet restore "PersonalSiteApi.csproj"
COPY . .
WORKDIR /src
RUN dotnet build "PersonalSiteApi.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "PersonalSiteApi.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
# ENV ASPNETCORE_URLS="https://+443;http://+80"
ENV Kestrel__Certificates__Default__Path=/root/.aspnet/https/PersonalSiteApi.pfx
ENV Kestrel__Certificates__Default__Password=personalsiteapi
ENTRYPOINT ["dotnet", "PersonalSiteApi.dll"]

