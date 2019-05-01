this project can either be debugged in local host,
started as a local container as part of a docker compose
or running in a production environment

since this code will write to google firestore, the code always stays the same,
but the file pointed to by the environment variable of GOOGLE_APPLICATION_CREDENTIALS will decide
which google cloud project to write data to. There is a separate firestore for each google cloud 
project.

A machine running anything local, whether through debug or through docker compose will use a local 
JSON file for the environment variable. (if we want to debug production data, we can technically
get a production service key and set that for a local environment variable.)

on how the secrets get added to use in the Configuration[] see [accessing secrets](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-2.2&tabs=linux#access-a-secret)

in regards to any other configuration, a connection string can be added with 
`dotnet user-secrets set "<keyname>" "<keyvalue>"`
And when running the app in a container (in order that the same exact code works with a different 
connection string (or the same,)) we can make an env variable `<keyname>=<keyvalue>`.

