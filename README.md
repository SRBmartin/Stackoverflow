# Stackoverflow portal - setup

### Backend
* Before you start to use this service, you need to have **[.NET Framwork 4.8](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net48)** installed!
* After confirming .NET version, let's **[install Azurite](https://learn.microsoft.com/sr-latn-rs/azure/storage/common/storage-use-azurite?tabs=npm)** (Azure storage, queue & table emulator).
After successful install you need to run Azurite with command `azurite --silent`. You should see that services started on ports 10000-10002.
* You can now run backend, and it should start on port 8080 and automatically enter Swagger on [http://localhost:8080/swagger/index/ui](http://localhost:8080/swagger/index/ui).

### Frontend
* Inside **frontend** folder there is **stackoverflow-portal** folder with created Angular project. Position yourself inside the project and run `npm i`.
* After the successful installation of packages, you can start frontend project with `npm start`.