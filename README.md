# Asynchronous Request-Reply Pattern

This is a simple project that implement an Asynchronous Request Reply Pattern example using azure functions. This project is very similar to the [Microsoft's documentation example](https://docs.microsoft.com/en-us/azure/architecture/patterns/async-request-reply#example)

## Disclaimer
This repository was created for learning purposes. **The code written in this repository is not production ready!** This is just a little repository made in a few hours.

## How to use it

### Azure setup
1. Create the resources in azure:
    - A [Function App](https://azure.microsoft.com/en-us/services/functions/)
    - One [Service Bus](https://azure.microsoft.com/en-us/services/service-bus/)
    - And one [Storage Account](https://azure.microsoft.com/en-us/product-categories/storage/)
3. Deploy the `AsyncRequestReply.Functions` project on the function app that you created before.
4. In the function app, it is necessary to add the following settings as Application Settings:
    - ServiceBusConnectionString, the connection string to the service bus, with read and write permissions.
    - StorageConnectionString the connection string to the storage accout.
    - StatusFunctionKey, the authentication code to make a request to the `Status` function.

### Running the client

Once you have everything ready on Azure, you can try it with the `AsyncRequestReply.Client` project.
Go to the project's folder, run it, and follow the instructions:

```sh
cd AsyncRequestReply.Client
dotnet run --gateUrl=<< The url to the Gate function here, including the authentication code >>
```
