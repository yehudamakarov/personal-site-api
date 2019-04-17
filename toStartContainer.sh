# to run the container on mac 
docker run --rm -it -p 5001:5001/tcp -p 5000:5000/tcp -e Kestrel__Certificates__Default__Path="/root/.aspnet/https/PersonalSiteApi.pfx" -e Kestrel__Certificates__Default__Password="personalsiteapi" -e ASPNETCORE_HTTPS_PORT="5001" -e ASPNETCORE_URLS="https://*:5001;http://*:5000" -v ${HOME}/.aspnet/https:/root/.aspnet/https personal-site-api:latest
